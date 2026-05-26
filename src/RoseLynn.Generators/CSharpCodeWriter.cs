using Microsoft.CodeAnalysis;
using RoseLynn.CSharp.SignatureViews;
using RoseLynn.Utilities;
using RoseLynn.Writing;
using System.Collections.Immutable;

namespace RoseLynn.Generators;

/// <summary>Writes common C# constructs while tracking indentation and nested block structure.</summary>
/// <param name="indentationCharacter">The character used for each indentation position.</param>
/// <param name="indentationSize">The number of indentation characters per nesting level.</param>
public class CSharpCodeWriter(char indentationCharacter = ' ', int indentationSize = 4)
    : IndentedStringBuilder(indentationCharacter, indentationSize)
{
    /// <summary>Gets the shared block list used for top-level separations in the writer.</summary>
    protected SeparatableBlockList GlobalSeparatableBlockList
        => field ??= NewSeparatableBlockList();

    /// <summary>Begins a new top-level block, inserting a separator when needed.</summary>
    public void BeginNewBlock()
    {
        GlobalSeparatableBlockList.BeginNewBlock();
    }

    /// <summary>Commits the current top-level block.</summary>
    public void CommitBlock()
    {
        GlobalSeparatableBlockList.CommitBlock();
    }

    /// <summary>Creates a block list that inserts blank lines using this writer.</summary>
    /// <returns>A new separatable block list bound to this writer.</returns>
    public SeparatableBlockList NewSeparatableBlockList() => new(this);

    /// <summary>Appends two consecutive line breaks.</summary>
    public void AppendDoubleLine()
    {
        AppendLine();
        AppendLine();
    }

    /// <summary>Appends content and then terminates the line.</summary>
    /// <param name="content">The content to append before the line break.</param>
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
    /// <param name="content">The multi-line content to append.</param>
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

    /// <summary>Writes the namespace and containing types needed for a partial type declaration scope.</summary>
    /// <param name="type">The type whose declaration scope will be written.</param>
    /// <returns>A disposable scope that closes the written declarations on disposal.</returns>
    public TypeScope AppendPartialType(INamedTypeSymbolSignatureView type)
    {
        var scope = new TypeScope(this, type);
        return scope;
    }

    /// <summary>Writes an argument expression for a parameter, including its ref-like modifier when required.</summary>
    /// <param name="parameter">The parameter whose pass-through expression will be written.</param>
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

    /// <summary>Writes a parameter declaration using simplified type names and parameter modifiers.</summary>
    /// <param name="parameter">The parameter to declare.</param>
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

    /// <summary>Writes a comma-separated parameter list using declaration syntax.</summary>
    /// <param name="parameters">The parameters to write.</param>
    public void WriteParameterList(
        ImmutableArray<IParameterSymbolSignatureView> parameters)
    {
        WriteCommaSeparated(
            parameters,
            WriteParameterDeclaration);
    }

    /// <summary>Writes type parameters enclosed in angle brackets when any exist.</summary>
    /// <param name="typeParameters">The type parameters to write.</param>
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

    /// <summary>Writes a comma-separated type parameter list without brackets.</summary>
    /// <param name="typeParameters">The type parameters to write.</param>
    public void WriteTypeParameterList(
        ImmutableArray<ITypeParameterSymbolSignatureView> typeParameters)
    {
        WriteCommaSeparated(
            typeParameters,
            s => Append(s.Name));
    }

    /// <summary>Writes values separated by commas using a custom element writer.</summary>
    /// <typeparam name="T">The element type being written.</typeparam>
    /// <param name="source">The elements to write.</param>
    /// <param name="writer">The callback that writes each element.</param>
    public void WriteCommaSeparated<T>(
        IEnumerable<T> source,
        Action<T> writer
    )
    {
        source.ApplyWithIntermediate(writer, WriteCommaSeparator);
    }

    /// <summary>Writes a comma-separated sequence of strings.</summary>
    /// <param name="strings">The strings to write.</param>
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

    /// <summary>Writes the C# keyword for an accessibility value.</summary>
    /// <param name="accessibility">The accessibility to write.</param>
    public void WriteAccessibility(Accessibility accessibility)
    {
        var keyword = GetAccessibilityKeyword(accessibility);
        Append(keyword);
    }

    /// <summary>Gets the C# keyword sequence for an accessibility value.</summary>
    /// <param name="accessibility">The accessibility to translate.</param>
    /// <returns>The corresponding C# keyword sequence.</returns>
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

    /// <summary>Gets the C# declaration keywords for an identifiable type kind.</summary>
    /// <param name="identifiableKind">The type kind to translate.</param>
    /// <returns>The declaration keywords for the type kind.</returns>
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

    /// <summary>Enters a bracketed block and increases indentation until disposal.</summary>
    /// <param name="open">The opening bracket character to write.</param>
    /// <param name="close">The closing bracket character to write on disposal.</param>
    /// <returns>A disposable bracket block scope.</returns>
    public BracketBlock EnterBracketBlock(char open = '{', char close = '}')
    {
        return new BracketBlock(this, open, close);
    }

    /// <summary>Enters an indentation-only block without writing brackets.</summary>
    /// <returns>A disposable nesting scope.</returns>
    public NestingBlock EnterNestingBlock()
    {
        return new NestingBlock(this);
    }

    /// <summary>Represents a disposable scope that writes and closes nested partial type declarations.</summary>
    public sealed class TypeScope : IDisposable
    {
        private readonly CSharpCodeWriter _builder;
        private int _nestingLevels;

        /// <summary>Initializes the scope for the specified type.</summary>
        /// <param name="builder">The writer that receives the generated declarations.</param>
        /// <param name="type">The type whose containing scope should be opened.</param>
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

        /// <summary>Closes all type scopes opened by this instance.</summary>
        public void Dispose()
        {
            for (int i = 0; i < _nestingLevels; i++)
            {
                _builder.NestingLevel--;
                _builder.AppendLine('}');
            }
        }
    }

    /// <summary>Represents a disposable scope that writes matching opening and closing bracket lines.</summary>
    public readonly struct BracketBlock : IDisposable
    {
        private readonly CSharpCodeWriter _builder;

        private readonly char _close;

        [Obsolete("Do not use the parameterless constructor", true)]
        public BracketBlock()
        {
            _builder = null!;
        }

        /// <summary>Initializes the scope, writes the opening bracket, and increases indentation.</summary>
        /// <param name="builder">The writer that receives the bracketed block.</param>
        /// <param name="open">The opening bracket character.</param>
        /// <param name="close">The closing bracket character to write on disposal.</param>
        public BracketBlock(CSharpCodeWriter builder, char open, char close)
        {
            _builder = builder;
            _close = close;
            _builder.AppendLine(open);
            _builder.NestingLevel++;
        }

        /// <summary>Writes the closing bracket and restores the previous indentation level.</summary>
        public void Dispose()
        {
            _builder.NestingLevel--;
            _builder.AppendLine(_close);
        }
    }

    /// <summary>Represents a disposable scope that temporarily increases indentation.</summary>
    public readonly struct NestingBlock : IDisposable
    {
        private readonly CSharpCodeWriter _builder;

        [Obsolete("Do not use the parameterless constructor", true)]
        public NestingBlock()
        {
            _builder = null!;
        }

        /// <summary>Initializes the scope and increases the writer's nesting level.</summary>
        /// <param name="builder">The writer whose nesting level will be increased.</param>
        public NestingBlock(CSharpCodeWriter builder)
        {
            _builder = builder;
            _builder.NestingLevel++;
        }

        /// <summary>Restores the previous nesting level.</summary>
        public void Dispose()
        {
            _builder.NestingLevel--;
        }
    }

    /// <summary>Inserts blank lines between committed writer blocks.</summary>
    /// <param name="builder">The writer that receives the separating blank lines.</param>
    public sealed class SeparatableBlockList(CSharpCodeWriter builder)
        : BaseSeparatableBlockList
    {
        private readonly CSharpCodeWriter _builder = builder;

        /// <summary>Appends the separator between committed blocks.</summary>
        protected override void AppendSeparator()
        {
            _builder.AppendLine();
        }
    }
}
