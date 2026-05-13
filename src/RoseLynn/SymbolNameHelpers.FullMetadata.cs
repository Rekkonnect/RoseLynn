using Garyon.Functions;
using Microsoft.CodeAnalysis;
using RoseLynn.CSharp;
using System.Text;

namespace RoseLynn;

public static partial class SymbolNameHelpers
{
    public static string FullMetadataString(
        ISymbol symbol,
        Compilation? compilation = null)
    {
        return FullMetadata(symbol, compilation).Name;
    }

    public static FullMetadataName FullMetadata(
        ISymbol symbol,
        Compilation? compilation = null)
    {
        return new FullMetadataNameBuilder(compilation)
            .Build(symbol);
    }

    public static FullMetadataName FullMetadataOrKeyword(
        ISymbol symbol,
        Compilation? compilation = null)
    {
        if (symbol is ITypeSymbol type)
        {
            return FullMetadataOrKeyword(type, compilation);
        }
        return FullMetadata(symbol, compilation);
    }

    public static FullMetadataName FullMetadataOrKeyword(
        ITypeSymbol type,
        Compilation? compilation = null)
    {
        return type.GetKeywordIdentifierForPredefinedType()
            ?? FullMetadata(type, compilation);
    }

    private sealed class FullMetadataNameBuilder(Compilation? compilation)
        : SymbolVisitor
    {
        private readonly Compilation? _compilation = compilation;

        private readonly StringBuilder _builder = new();

        public override void VisitArrayType(IArrayTypeSymbol symbol)
        {
            Visit(symbol.ElementType);
            _builder.Append('[');
            _builder.Append(',', symbol.Rank - 1);
            _builder.Append(']');
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

        public override void VisitMethod(IMethodSymbol symbol)
        {
            if (symbol.ContainingSymbol is IMethodSymbol parentMethod)
            {
                Visit(parentMethod);
                _builder.Append('.');
            }
            else if (symbol.ContainingType is not null and var type)
            {
                Visit(type);
                _builder.Append('.');
            }

            AppendMethodIdentifier(symbol);
        }

        public override void DefaultVisit(ISymbol symbol)
        {
            _builder.Append(symbol.Name);
        }

        private void AppendNamespace(INamespaceSymbol @namespace)
        {
            if (@namespace.IsGlobalNamespace)
            {
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

        public override void VisitDynamicType(IDynamicTypeSymbol symbol)
        {
            _builder.Append("dynamic");
        }

        public override void Visit(ISymbol? symbol)
        {
            base.Visit(symbol);
        }

        private void AppendNamedTypeIdentifier(INamedTypeSymbol symbol)
        {
            _builder.Append(symbol.Name);

            if (symbol.IsGenericType)
            {
                var arity = symbol.Arity;
                _builder.Append($"`{arity}");
            }
        }

        private void AppendMethodIdentifier(IMethodSymbol symbol)
        {
            _builder.Append(symbol.Name);

            if (symbol.IsGenericMethod)
            {
                var arity = symbol.Arity;
                _builder.Append($"``{arity}");
            }
        }

        public FullMetadataName Build(ISymbol symbol)
        {
            Visit(symbol);
            var name = _builder.ToString();
            return new(name);
        }
    }

    public sealed record FullMetadataName(string Name)
    {
        public static implicit operator FullMetadataName(string name) => new(name);
    }
}
