using Microsoft.CodeAnalysis;
using RoseLynn.SignatureViews;
using RoseLynn.Utilities;
using RoseLynn.Writing;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace RoseLynn.Generators;

public class CSharpCodeWriter(char indentationCharacter = ' ', int indentationSize = 4)
    : IndentedStringBuilder(indentationCharacter, indentationSize)
{
    protected SeparatableBlockList GlobalSeparatableBlockList
        => field ??= NewSeparatableBlockList();

    public void BeginNewBlock()
    {
        GlobalSeparatableBlockList.BeginNewBlock();
    }

    public void CommitBlock()
    {
        GlobalSeparatableBlockList.CommitBlock();
    }

    public SeparatableBlockList NewSeparatableBlockList() => new(this);

    public void AppendDoubleLine()
    {
        AppendLine();
        AppendLine();
    }

    public void CommitSingleLineContent(string content)
    {
        AppendSingleLineContent(content);
        AppendLine();
    }

    /// <summary>
    /// Alternative to the regular append methods, since there is a
    /// bug that does not respect indentation on new lines
    /// when multi-line content is appended.
    /// </summary>
    public void AppendMultilineContent(string content)
    {
        foreach (var token in content.EnumerateLineTokens())
        {
            if (token.Kind is LineToken.TokenKind.NewLine)
            {
                AppendLine();
                continue;
            }

            AppendSingleLineContent(token.Span);
        }
    }

    public TypeScope AppendPartialType(INamedTypeSymbolSignatureView type)
    {
        var scope = new TypeScope(this, type);
        return scope;
    }

    public void WriteParameterPass(
        IParameterSymbolSignatureView parameter)
    {
        var prefix = parameter.RefKind switch
        {
            RefKind.In => "in",
            RefKind.RefReadOnlyParameter => "in",
            RefKind.Out => "out",
            RefKind.Ref => "ref",
            _ => null,
        };

        if (prefix is not null)
        {
            Append($"{prefix} ");
        }

        Append(parameter.Name);
    }

    public void WriteParameterDeclaration(
        IParameterSymbolSignatureView parameter)
    {
        if (parameter.ScopedKind is ScopedKind.ScopedRef or ScopedKind.ScopedValue)
        {
            Append("scoped ");
        }

        var refPrefix = parameter.RefKind switch
        {
            RefKind.In => "in",
            RefKind.RefReadOnlyParameter => "ref readonly",
            RefKind.Out => "out",
            RefKind.Ref => "ref",
            _ => null,
        };

        if (refPrefix is not null)
        {
            Append($"{refPrefix} ");
        }

        Append(parameter.Type.SimplerIdentifier());
        Append(" ");
        Append(parameter.Name);
    }

    public void WriteParameterList(
        ImmutableArray<IParameterSymbolSignatureView> parameters)
    {
        WriteCommaSeparated(
            parameters,
            WriteParameterDeclaration);
    }

    public void WriteTypeParameterListWithBrackets(
        ImmutableArray<ITypeParameterSymbolSignatureView> typeParameters)
    {
        if (typeParameters is [])
        {
            return;
        }

        Append('<');
        WriteTypeParameterList(typeParameters);
        Append('>');
    }

    public void WriteTypeParameterList(
        ImmutableArray<ITypeParameterSymbolSignatureView> typeParameters)
    {
        WriteCommaSeparated(
            typeParameters,
            s => Append(s.Name));
    }

    public void WriteCommaSeparated<T>(
        IEnumerable<T> source,
        Action<T> writer
    )
    {
        source.ApplyWithIntermediate(writer, WriteCommaSeparator);
    }

    public void WriteCommaSeparatedList(
        IEnumerable<string> strings)
    {
        bool first = true;
        foreach (var @string in strings)
        {
            if (!first)
            {
                WriteCommaSeparator();
            }

            Append(@string);
            first = false;
        }
    }

    private void WriteCommaSeparator()
    {
        Append(", ");
    }

    public void WriteAccessibility(Accessibility accessibility)
    {
        var keyword = GetAccessibilityKeyword(accessibility);
        Append(keyword);
    }

    protected static string GetAccessibilityKeyword(Accessibility accessibility)
    {
        return accessibility switch
        {
            Accessibility.Public => "public",
            Accessibility.Private => "private",
            Accessibility.Internal => "internal",
            Accessibility.Protected => "protected",
            Accessibility.ProtectedOrInternal => "protected internal",
            Accessibility.ProtectedAndInternal => "private protected",
            _ => string.Empty,
        };
    }

    protected static string GetTypeKindKeywords(IdentifiableSymbolKind identifiableKind)
    {
        return identifiableKind switch
        {
            IdentifiableSymbolKind.Class => "class",
            IdentifiableSymbolKind.Struct => "struct",
            IdentifiableSymbolKind.Interface => "interface",
            IdentifiableSymbolKind.RecordClass => "record class",
            IdentifiableSymbolKind.RecordStruct => "record struct",
            _ => throw new InvalidOperationException("Unsupported type kind."),
        };
    }

    public BracketBlock EnterBracketBlock(char open = '{', char close = '}')
    {
        return new BracketBlock(this, open, close);
    }

    public NestingBlock EnterNestingBlock()
    {
        return new NestingBlock(this);
    }

    public sealed class TypeScope : IDisposable
    {
        private readonly CSharpCodeWriter _builder;
        private int _nestingLevels;

        public TypeScope(CSharpCodeWriter builder, INamedTypeSymbolSignatureView type)
        {
            _builder = builder;
            var initialNesting = builder.NestingLevel;
            InitializeForType(type);
            _nestingLevels = builder.NestingLevel - initialNesting;
        }

        private void InitializeForType(INamedTypeSymbolSignatureView type)
        {
            var containingTypes = type.ContainingTypes;
            foreach (var containingType in containingTypes)
            {
                WriteType(containingType);
            }

            var containingNamespaceName = type.ContainedNamespaceName;
            WriteNamespace(containingNamespaceName);

            WriteType(type);
        }

        private void WriteNamespace(string? namespaceName)
        {
            if (string.IsNullOrEmpty(namespaceName))
                return;

            _builder.AppendSingleLineContent(
                $"namespace {namespaceName};");

            _builder.AppendDoubleLine();
        }

        private void WriteType(INamedTypeSymbolSignatureView type)
        {
            var identifiableKind = type.GetIdentifiableSymbolKind();
            var typeKind = GetTypeKindKeywords(identifiableKind);
            _builder.Append($"partial {typeKind} {type.Name}");
            _builder.WriteTypeParameterListWithBrackets(type.TypeParameters);
            _builder.AppendLine();
            _builder.AppendLine('{');
            _builder.IncrementNestingLevel();
        }

        public void Dispose()
        {
            for (int i = 0; i < _nestingLevels; i++)
            {
                _builder.NestingLevel--;
                _builder.AppendLine('}');
            }
        }
    }

    public readonly struct BracketBlock : IDisposable
    {
        private readonly CSharpCodeWriter _builder;

        private readonly char _close;

        [Obsolete("Do not use the parameterless constructor", true)]
        public BracketBlock()
        {
            _builder = null!;
        }

        public BracketBlock(CSharpCodeWriter builder, char open, char close)
        {
            _builder = builder;
            _close = close;
            _builder.AppendLine(open);
            _builder.NestingLevel++;
        }

        public void Dispose()
        {
            _builder.NestingLevel--;
            _builder.AppendLine(_close);
        }
    }

    public readonly struct NestingBlock : IDisposable
    {
        private readonly CSharpCodeWriter _builder;

        [Obsolete("Do not use the parameterless constructor", true)]
        public NestingBlock()
        {
            _builder = null!;
        }

        public NestingBlock(CSharpCodeWriter builder)
        {
            _builder = builder;
            _builder.NestingLevel++;
        }

        public void Dispose()
        {
            _builder.NestingLevel--;
        }
    }

    public sealed class SeparatableBlockList(CSharpCodeWriter builder)
        : BaseSeparatableBlockList
    {
        private readonly CSharpCodeWriter _builder = builder;

        protected override void AppendSeparator()
        {
            _builder.AppendLine();
        }
    }
}
