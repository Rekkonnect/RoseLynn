using Garyon.Functions;
using Microsoft.CodeAnalysis;
using System.Text;

namespace RoseLynn.Core;

/// <summary>Provides helpers for building full metadata names for Roslyn symbols.</summary>
public static partial class SymbolNameHelpers
{
    /// <summary>Gets the full metadata name string for a symbol.</summary>
    /// <param name="symbol">The symbol to format.</param>
    /// <param name="compilation">An optional compilation used when formatting context-sensitive details.</param>
    /// <returns>The full metadata name.</returns>
    public static string FullMetadataString(
        ISymbol symbol,
        Compilation? compilation = null)
    {
        return FullMetadata(symbol, compilation).Name;
    }

    /// <summary>Gets the full metadata name for a symbol.</summary>
    /// <param name="symbol">The symbol to format.</param>
    /// <param name="compilation">An optional compilation used when formatting context-sensitive details.</param>
    /// <returns>The full metadata name wrapper.</returns>
    public static FullMetadataName FullMetadata(
        ISymbol symbol,
        Compilation? compilation = null)
    {
        return new FullMetadataNameBuilder(compilation)
            .Build(symbol);
    }

    /// <summary>Builds metadata names by visiting Roslyn symbols recursively.</summary>
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

    /// <summary>Wraps a symbol's full metadata name.</summary>
    /// <param name="Name">The metadata name string.</param>
    public sealed record FullMetadataName(string Name)
    {
        /// <summary>Converts a string to a <see cref="FullMetadataName"/>.</summary>
        /// <param name="name">The metadata name string.</param>
        public static implicit operator FullMetadataName(string name) => new(name);
    }
}
