using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using RoseLynn.Analyzers;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace RoseLynn.InternalGenerators;

#nullable enable

// A generator that also reports diagnostics; not so bad for such simple solutions
[Generator]
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class OperatorKindFactsGenerator : CSharpDiagnosticAnalyzer, ISourceGenerator
{
    private const string OperatorKindFactsName = "OperatorKindFacts";
    private const string OperatorKindName = "OperatorKind";
    private const string FullOperatorKindName = $"{nameof(RoseLynn)}.{OperatorKindName}";

    protected override DiagnosticDescriptorStorageBase DiagnosticDescriptorStorage => GeneratorDiagnosticDescriptorStorage.Instance;

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
    }

    public void Execute(GeneratorExecutionContext context)
    {
        var matchedSymbol = context.Compilation.GetTypeByMetadataName(FullOperatorKindName);

        if (matchedSymbol is null)
        {
            throw new Exception($"Failed to find {FullOperatorKindName}; which is the component we're operating on today.");
        }

        var methodsSource = GenerateMethods(context, matchedSymbol);

        context.AddSource($"{OperatorKindFactsName}.g.cs", methodsSource);
    }

    void ISourceGenerator.Initialize(GeneratorInitializationContext context)
    {
        
    }

    private string GenerateMethods(GeneratorExecutionContext context, INamedTypeSymbol operatorKindSymbol)
    {
        const string header = $$"""
                                using {{nameof(System)}};
                                using static {{FullOperatorKindName}};

                                namespace {{nameof(RoseLynn)}};

                                partial class {{OperatorKindFactsName}}
                                {
                                """;

        const string footer = "}";

        var builder = new StringBuilder();
        builder.Append(header);

        var methodGenerator = new MethodGenerator(context, operatorKindSymbol, builder);
        methodGenerator.GenerateMethods();

        builder.AppendLine(footer);
        return builder.ToString();
    }

    // Totally abusing records
    private sealed record MethodGenerator(GeneratorExecutionContext Context, INamedTypeSymbol OperatorKindSymbol, StringBuilder Builder)
    {
        private const string indentation = "    ";

        private ImmutableArray<OperatorKindInfo> operatorKinds;

        public void GenerateMethods()
        {
            DiscoverKindInformation();
            GenerateMethodSource();
        }

        private void DiscoverKindInformation()
        {
            operatorKinds = OperatorKindSymbol.GetEnumDefinitions()
                .Select(OperatorKindInfo.Parse)
                .ToImmutableArray();

            var undefinedArities = operatorKinds.Where(info => info.Arity is OperatorArity.None);
            foreach (var undefinedArity in undefinedArities)
            {
                var reference = undefinedArity.FieldSymbol.DeclaringSyntaxReferences.First();
                // Definitely not too great as a solution, but at least we can report diagnostics through SGs
                Context.ReportDiagnostic(GeneratorDiagnosticDescriptorStorage.Instance[0001]!, reference);
            }
        }

        private void GenerateMethodSource()
        {
            GenerateCanBeChecked();
            GenerateIsUnary();
            GenerateMapNameToKind();
        }

        private void AppendLine(string line)
        {
            AppendIndentation();
            Builder.AppendLine(line);
        }
        private void AppendIndentation()
        {
            Builder.Append(indentation);
        }

        private void GenerateCanBeChecked()
        {
            GenerateSimplePatternMethod("CanBeChecked", kind => kind.CanBeChecked);
        }
        private void GenerateIsUnary()
        {
            GenerateSimplePatternMethod("IsUnary", kind => kind.Arity is OperatorArity.Unary);
        }
        private void GenerateMapNameToKind()
        {
            var nameofGenerator = new NameOfSwitchArmGenerator(Builder, 2);

            AppendLine($"private static OperatorKind MapNameToKind_Generated(string name) => name switch");
            AppendLine("{");

            var filteredKinds = operatorKinds.Select(kind => kind.Name);
            nameofGenerator.AppendPatternValues(filteredKinds);

            AppendLine("    _ => throw new ArgumentException(\"The given name does not reflect an operator name.\")");
            AppendLine("};");
        }

        private void GenerateSimplePatternMethod(string methodName, Func<OperatorKindInfo, bool> operatorKindFilter)
        {
            var patternGenerator = new IsPatternGenerator(Builder, 3);

            AppendLine($"private static bool {methodName}_Generated(OperatorKind kind)");
            AppendLine("{");
            AppendLine("    return kind");

            // Assume there will always be at least one operator kind passing the filter
            var filteredKinds = operatorKinds.Where(operatorKindFilter).Select(kind => kind.Name);
            patternGenerator.AppendPatternValues(filteredKinds);

            AppendLine("        ;");
            AppendLine("}");
        }

        private abstract record IndentedPatternLineGenerator(StringBuilder Builder, int IndentationLevel)
        {
            protected void AppendIndentation()
            {
                Builder.Append(' ', IndentationLevel * 4);
            }

            public void AppendPatternValues(IEnumerable<string> values)
            {
                foreach (var value in values)
                    AppendPatternValue(value);
            }

            public void AppendPatternValue(string value)
            {
                AppendIndentation();
                AppendPatternValueImpl(value);
            }

            protected abstract void AppendPatternValueImpl(string value);
        }

        private sealed record IsPatternGenerator(StringBuilder Builder, int IndentationLevel)
            : IndentedPatternLineGenerator(Builder, IndentationLevel)
        {
            private int current = 0;

            protected override void AppendPatternValueImpl(string value)
            {
                Builder.Append(PatternDelimiter()).Append(' ');
                Builder.AppendLine(value);
                current++;
            }

            private string PatternDelimiter() => current switch
            {
                0 => "is",
                _ => "or",
            };
        }

        private sealed record NameOfSwitchArmGenerator(StringBuilder Builder, int IndentationLevel)
            : IndentedPatternLineGenerator(Builder, IndentationLevel)
        {
            protected override void AppendPatternValueImpl(string value)
            {
                Builder.Append("nameof(").Append(value).Append(") => ").Append(value).Append(',').AppendLine();
            }
        }

        private sealed record OperatorKindInfo(IFieldSymbol FieldSymbol, OperatorKindTokens Tokens, OperatorArity Arity, bool CanBeChecked)
        {
            public string Name => FieldSymbol.Name;

            public static OperatorKindInfo Parse(IFieldSymbol fieldSymbol)
            {
                var tokens = OperatorKindTokens.Parse(fieldSymbol);
                var arity = InferArity(fieldSymbol);
                bool canBeChecked = fieldSymbol.HasAttributeNamed(KnownSymbolNames.Full.CanBeCheckedOperatorAttribute);
                return new(fieldSymbol, tokens, arity, canBeChecked);
            }

            private static OperatorArity InferArity(IFieldSymbol fieldSymbol)
            {
                var arityAttribute = fieldSymbol.AttributesInheriting(KnownSymbolNames.Full.OperatorArityAttribute).FirstOrDefault();
                return InferArity(arityAttribute);
            }
            private static OperatorArity InferArity(AttributeData? arityAttribute)
            {
                return arityAttribute?.AttributeClass!.Name switch
                {
                    KnownSymbolNames.UnaryOperatorAttribute => OperatorArity.Unary,
                    KnownSymbolNames.BinaryOperatorAttribute => OperatorArity.Binary,

                    _ => OperatorArity.None,
                };
            }
        }

        private sealed record OperatorKindTokens(string? CSharp, string? VisualBasic)
        {
            public static OperatorKindTokens Parse(IFieldSymbol fieldSymbol)
            {
                var csharpOperatorTokenAttribute = fieldSymbol.GetAttributesNamed(KnownSymbolNames.CSharpOperatorTokenAttribute).FirstOrDefault();
                var csharpOperatorToken = csharpOperatorTokenAttribute?.ConstructorArguments.First().Value as string;
                return new(csharpOperatorToken, null);
            }
        }

        private enum OperatorArity
        {
            None = 0,

            Unary = 1,
            Binary = 2,
        }
    }
}
