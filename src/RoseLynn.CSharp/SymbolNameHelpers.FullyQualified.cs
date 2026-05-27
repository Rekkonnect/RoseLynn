using Garyon.Functions;
using Microsoft.CodeAnalysis;
using Polyfills;
using RoseLynn;
using RoseLynn.Utilities;
using System.Collections.Immutable;
using System.Text;

namespace RoseLynn.CSharp;

public static partial class SymbolNameHelpers
{
    public static string FullyQualifiedString(
        ISymbol symbol,
        Compilation? compilation = null)
    {
        return FullyQualified(symbol, compilation).Name;
    }

    public static FullyQualifiedName FullyQualified(
        ISymbol symbol,
        Compilation? compilation = null)
    {
        return new FullyQualifiedNameBuilder(compilation)
            .Build(symbol);
    }

    public static FullyQualifiedName FullyQualifiedOrKeyword(
        ISymbol symbol,
        Compilation? compilation = null)
    {
        if (symbol is ITypeSymbol type)
        {
            return FullyQualifiedOrKeyword(type, compilation);
        }
        return FullyQualified(symbol, compilation);
    }

    public static FullyQualifiedName FullyQualifiedOrKeyword(
        ITypeSymbol type,
        Compilation? compilation = null)
    {
        return KeywordIdentifierWithNullability(type)
            ?? FullyQualified(type, compilation);
    }

    public static string? KeywordIdentifierWithNullability(ITypeSymbol type)
    {
        if (type.IsNullableStruct(out var underlying))
        {
            var underlyingKeyword = underlying.GetKeywordIdentifierForPredefinedType();
            return underlyingKeyword.SelectOrNull(WithNullableMarker);
        }

        var keyword = type.GetKeywordIdentifierForPredefinedType();
        if (keyword is null)
        {
            return null;
        }

        if (NeedsNullableMarker(type))
        {
            return WithNullableMarker(keyword);
        }

        return keyword;
    }

    private static string WithNullableMarker(string typeSyntax)
    {
        return $"{typeSyntax}?";
    }

    // REF: Copied from Roslyn's CSharpAddImportFeatureService
    public static string? GetExternAlias(
        INamespaceSymbol namespaceSymbol,
        Compilation compilation)
    {
        var metadataReference = compilation
            .GetMetadataReference(namespaceSymbol.ContainingAssembly);

        var aliases = metadataReference?.Properties.Aliases
            .Where(a => a != MetadataReferenceProperties.GlobalAlias);

        return aliases?.FirstOrDefault();
    }

    private static bool NeedsNullableMarker(ISymbol? symbol)
    {
        return symbol
            is ITypeSymbol { NullableAnnotation: NullableAnnotation.Annotated };
    }

    /// <summary>Builds fully qualified symbol names, including extern alias tracking when needed.</summary>
    private sealed class FullyQualifiedNameBuilder(Compilation? compilation)
        : SymbolVisitor
    {
        private const string _globalNamespaceAlias = "global";

        private readonly Compilation? _compilation = compilation;

        private readonly StringBuilder _builder = new();
        private HashSet<string>? _externAliases = null;

        public override void VisitArrayType(IArrayTypeSymbol symbol)
        {
            Visit(symbol.ElementType);
            _builder.Append('[');
            _builder.Append(',', symbol.Rank - 1);
            _builder.Append(']');
        }

        public override void VisitEvent(IEventSymbol symbol)
        {
            base.VisitEvent(symbol);
        }

        public override void VisitPointerType(IPointerTypeSymbol symbol)
        {
            Visit(symbol.PointedAtType);
            _builder.Append('*');
        }

        public override void VisitNamespace(INamespaceSymbol symbol)
        {
            if (symbol.ContainingNamespace is not null and var parentNamespace)
            {
                VisitNamespace(parentNamespace);
                AppendRegularNamespaceSeparator(parentNamespace);
            }

            AppendNamespace(symbol);
        }

        public override void VisitNamedType(INamedTypeSymbol symbol)
        {
            if (symbol.IsAnonymousType)
            {
                return;
            }

            if (symbol.ContainingType is not null and var parentType)
            {
                Visit(parentType);
                _builder.Append('.');
            }
            else if (symbol.ContainingNamespace is not null and var @namespace)
            {
                Visit(@namespace);
                AppendRegularNamespaceSeparator(@namespace);
            }

            AppendNamedTypeIdentifier(symbol);
        }

        public override void DefaultVisit(ISymbol symbol)
        {
            _builder.Append(symbol.Name);
        }

        private void AppendNamespace(INamespaceSymbol @namespace)
        {
            if (@namespace.IsGlobalNamespace)
            {
                AppendNamespaceAlias(@namespace);
                return;
            }

            AppendRegularNamespace(@namespace);
        }

        private void AppendRegularNamespaceSeparator(INamespaceSymbol @namespace)
        {
            if (!@namespace.IsGlobalNamespace)
            {
                _builder.Append('.');
            }
        }

        private void AppendRegularNamespace(INamespaceSymbol @namespace)
        {
            Asserts.False(@namespace.IsGlobalNamespace);
            _builder.Append(@namespace.Name);
        }

        private void AppendNamespaceAlias(
            INamespaceSymbol @namespace,
            bool appendDoubleColon = true)
        {
            Asserts.True(@namespace.IsGlobalNamespace);

            var prefix = GlobalNamespacePrefix(@namespace);
            _builder.Append(prefix);
            if (appendDoubleColon)
            {
                _builder.Append("::");
            }
        }

        public override void VisitFunctionPointerType(IFunctionPointerTypeSymbol symbol)
        {
            // TODO
        }

        public override void VisitTypeParameter(ITypeParameterSymbol symbol)
        {
            _builder.Append(symbol.Name);
        }

        public override void VisitDynamicType(IDynamicTypeSymbol symbol)
        {
            _builder.Append("dynamic");
        }

        public override void Visit(ISymbol? symbol)
        {
            base.Visit(symbol);

            if (NeedsNullableMarker(symbol))
            {
                _builder.Append('?');
            }
        }

        private void AppendNamedTypeIdentifier(INamedTypeSymbol symbol)
        {
            if (symbol.IsExtension)
            {
                AppendExtensionBlockIdentifier(symbol);
                return;
            }

            _builder.Append(symbol.Name);

            if (symbol.IsGenericType)
            {
                AppendGenericArguments(symbol.TypeArguments);
            }
        }

        private void AppendExtensionBlockIdentifier(INamedTypeSymbol symbol)
        {
            _builder.Append("extension");

            if (symbol.IsGenericType)
            {
                AppendGenericArguments(symbol.TypeArguments);
            }

            _builder.Append('(');
            Visit(symbol.ExtensionParameter?.Type);
            _builder.Append(')');
        }

        private void AppendGenericArguments(ImmutableArray<ITypeSymbol> typeArguments)
        {
            _builder.Append('<');

            var hasPrevious = false;
            foreach (var typeArgument in typeArguments)
            {
                if (hasPrevious)
                {
                    _builder.Append(", ");
                }
                Visit(typeArgument);
                hasPrevious = true;
            }

            _builder.Append('>');
        }

        private HashSet<string> ExternAliasesSet() => _externAliases = new();

        public FullyQualifiedName Build(ISymbol symbol)
        {
            Visit(symbol);
            var name = _builder.ToString();
            var externAliases = _externAliases?.ToImmutableArray() ?? [];
            return new(name, externAliases);
        }

        private string GlobalNamespacePrefix(INamespaceSymbol @namespace)
        {
            string? alias = null;

            if (_compilation is not null)
            {
                alias = GetExternAlias(@namespace, _compilation);
                alias.IfNotNull(RegisterExternAlias);
            }

            return alias ?? _globalNamespaceAlias;
        }

        private void RegisterExternAlias(string alias)
        {
            ExternAliasesSet().Add(alias);
        }
    }

    /// <summary>Wraps a symbol's fully qualified name together with any referenced extern aliases.</summary>
    /// <param name="Name">The fully qualified symbol name.</param>
    /// <param name="ExternAliases">The extern aliases used while constructing the name.</param>
    public sealed record FullyQualifiedName(
        string Name,
        ImmutableArray<string> ExternAliases)
    {
        /// <summary>Initializes a fully qualified name with no extern aliases.</summary>
        /// <param name="name">The fully qualified symbol name.</param>
        public FullyQualifiedName(string name)
            : this(name, []) { }

        /// <summary>Converts a string to a <see cref="FullyQualifiedName"/> with no extern aliases.</summary>
        /// <param name="name">The fully qualified symbol name.</param>
        public static implicit operator FullyQualifiedName(string name) => new(name);
    }
}
