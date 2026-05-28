using System.Reflection.Metadata;
using Microsoft.CodeAnalysis;
using RoseLynn.Core.Utilities;

namespace RoseLynn.Core.SignatureViews;

/*
 * General principles followed in the implementations:
 * - Always prefer records for auto-generated Equals implementations
 * - Always override GetHashCode for types with array contents
 * - Prefer overriding GetHashCode generally when there's many declared properties
 * - When overriding GetHashCode, choose few of the most important properties and shift
 * - Use EquatableImmutableArray for the Equals implementation to use SequenceEquals
 */

// Base

/// <summary>
/// Represents a light signature view of an <see cref="ISymbol"/>,
/// implementing the <see cref="ISymbolSignatureView"/> interface as a baseline.
/// </summary>
/// <remarks>
/// Symbol signature views are useful for achieving a lower memory footprint
/// during incremental generators' pipeline steps. Such implementations are only
/// providing absolutely necessary properties that constitute the symbol's
/// signature, discarding parent, nested or otherwise linked information.
/// </remarks>
public abstract record SymbolSignatureView
    : ISymbolSignatureView
{
    /// <inheritdoc/>
    public string Name { get; set; } = null!;

    /// <inheritdoc/>
    public string MetadataName { get; set; } = null!;

    /// <inheritdoc/>
    public string FullyQualifiedIdentifier { get; set; } = null!;

    /// <inheritdoc/>
    public string FullyQualifiedName { get; set; } = null!;

    /// <inheritdoc/>
    public string FullMetadataName { get; set; } = null!;

    /// <inheritdoc/>
    public string? ContainedNamespaceName { get; set; }

    /// <inheritdoc/>
    public SymbolKind Kind { get; set; }

    /// <inheritdoc/>
    public Accessibility DeclaredAccessibility { get; set; }

    /// <inheritdoc/>
    public bool IsStatic { get; set; }

    /// <inheritdoc/>
    public bool IsSealed { get; set; }

    /// <inheritdoc/>
    public bool IsAbstract { get; set; }

    /// <inheritdoc/>
    public bool IsVirtual { get; set; }

    /// <inheritdoc/>
    public bool IsOverride { get; set; }

    /// <inheritdoc/>
    public bool IsExtern { get; set; }

    // A quicker hash to improve performance, saving up lots of
    // unnecessary calculations
    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return FullyQualifiedName.GetHashCode() ^ (int)Kind;
    }
}

/// <summary>
/// Represents a light signature view of an <see cref="INamespaceOrTypeSymbol"/>,
/// implementing the <see cref="INamespaceOrTypeSymbolSignatureView"/>
/// interface as a baseline.
/// </summary>
/// <inheritdoc cref="SymbolSignatureView"/>
public abstract record NamespaceOrTypeSymbolSignatureView
    : SymbolSignatureView, INamespaceOrTypeSymbolSignatureView;

// Types

/// <summary>
/// Represents a light signature view of an <see cref="ITypeSymbol"/>,
/// implementing the <see cref="ITypeSymbolSignatureView"/>
/// interface as a baseline.
/// </summary>
/// <inheritdoc cref="SymbolSignatureView"/>
public abstract record TypeSymbolSignatureView
    : NamespaceOrTypeSymbolSignatureView, ITypeSymbolSignatureView
{
    /// <inheritdoc/>
    public TypeKind TypeKind { get; set; }

    /// <inheritdoc/>
    public SpecialType SpecialType { get; set; }

    /// <inheritdoc/>
    public bool IsRecord { get; set; }

    /// <inheritdoc/>
    public bool IsValueType { get; set; }

    /// <inheritdoc/>
    public bool IsReferenceType { get; set; }

    /// <inheritdoc/>
    public NullableAnnotation NullableAnnotation { get; set; }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return base.GetHashCode()
            ^ ((int)TypeKind << 6)
            ^ (IsRecord.GetHashCode() << 10);
    }
}

/// <summary>
/// Represents a light signature view of an <see cref="INamedTypeSymbol"/>,
/// implementing the <see cref="INamedTypeSymbolSignatureView"/>
/// interface as a baseline.
/// </summary>
/// <inheritdoc cref="SymbolSignatureView"/>
public record NamedTypeSymbolSignatureView
    : TypeSymbolSignatureView, INamedTypeSymbolSignatureView
{
    /// <inheritdoc/>
    public required bool IsFileLocal { get; set; }

    /// <inheritdoc/>
    public required bool IsGenericType { get; set; }

    /// <inheritdoc/>
    public required EquatableImmutableArray<ITypeSymbolSignatureView> TypeArguments { get; set; }

    /// <inheritdoc/>
    public required EquatableImmutableArray<ITypeParameterSymbolSignatureView> TypeParameters { get; set; }

    /// <inheritdoc/>
    public required SpecialType EnumUnderlyingSpecialType { get; set; }

    /// <inheritdoc/>
    public required SpecialType ConstructedFromSpecialType { get; set; }

    /// <inheritdoc/>
    public required EquatableImmutableArray<INamedTypeSymbolSignatureView> ContainingTypes { get; set; }

    /// <inheritdoc/>
    public EquatableImmutableArray<string> TupleFieldNames { get; set; }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return base.GetHashCode()
            ^ (TypeParameters.Array.Length << 4);
    }
}

/// <summary>
/// Represents a light signature view of an <see cref="ITypeParameterSymbol"/>,
/// implementing the <see cref="ITypeParameterSymbolSignatureView"/>
/// interface as a baseline.
/// </summary>
/// <inheritdoc cref="SymbolSignatureView"/>
public record TypeParameterSymbolSignatureView
    : TypeSymbolSignatureView, ITypeParameterSymbolSignatureView
{
    /// <inheritdoc/>
    public required VarianceKind Variance { get; set; }

    /// <inheritdoc/>
    public required TypeParameterKind TypeParameterKind { get; set; }

    /// <inheritdoc/>
    public required int Ordinal { get; set; }

    /// <inheritdoc/>
    public required bool HasReferenceTypeConstraint { get; set; }

    /// <inheritdoc/>
    public required NullableAnnotation ReferenceTypeConstraintNullableAnnotation { get; set; }

    /// <inheritdoc/>
    public required bool HasValueTypeConstraint { get; set; }

    /// <inheritdoc/>
    public required bool AllowsRefLikeType { get; set; }

    /// <inheritdoc/>
    public required bool HasUnmanagedTypeConstraint { get; set; }

    /// <inheritdoc/>
    public required bool HasNotNullConstraint { get; set; }

    /// <inheritdoc/>
    public required bool HasConstructorConstraint { get; set; }

    /// <inheritdoc/>
    public required EquatableImmutableArray<ITypeSymbolSignatureView> ConstraintTypes { get; set; }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return Name.GetHashCode()
            ^ ((int)Variance)
            ^ (Ordinal << 6);
    }
}

/// <summary>
/// Represents a light signature view of an <see cref="IArrayTypeSymbol"/>,
/// implementing the <see cref="IArrayTypeSymbolSignatureView"/>
/// interface as a baseline.
/// </summary>
/// <inheritdoc cref="SymbolSignatureView"/>
public record ArrayTypeSymbolSignatureView
    : TypeSymbolSignatureView, IArrayTypeSymbolSignatureView
{
    /// <inheritdoc/>
    public required int Rank { get; set; }

    /// <inheritdoc/>
    public required EquatableImmutableArray<int> Sizes { get; set; }

    /// <inheritdoc/>
    public required ITypeSymbolSignatureView ElementType { get; set; }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return base.GetHashCode()
            ^ (Rank << 12);
    }
}

/// <summary>
/// Represents a light signature view of an <see cref="IDynamicTypeSymbol"/>,
/// implementing the <see cref="IDynamicTypeSymbolSignatureView"/>
/// interface as a baseline.
/// </summary>
/// <inheritdoc cref="SymbolSignatureView"/>
public record DynamicTypeSymbolSignatureView
    : TypeSymbolSignatureView, IDynamicTypeSymbolSignatureView;

/// <summary>
/// Represents a light signature view of an <see cref="IPointerTypeSymbol"/>,
/// implementing the <see cref="IPointerTypeSymbolSignatureView"/>
/// interface as a baseline.
/// </summary>
/// <inheritdoc cref="SymbolSignatureView"/>
public record PointerTypeSymbolSignatureView
    : TypeSymbolSignatureView, IPointerTypeSymbolSignatureView
{
    /// <inheritdoc/>
    public required ITypeSymbolSignatureView PointedAtType { get; set; }
}

/// <summary>
/// Represents a light signature view of an <see cref="IFunctionPointerTypeSymbol"/>,
/// implementing the <see cref="IFunctionPointerTypeSymbolSignatureView"/>
/// interface as a baseline.
/// </summary>
/// <inheritdoc cref="SymbolSignatureView"/>
public record FunctionPointerTypeSymbolSignatureView
    : TypeSymbolSignatureView, IFunctionPointerTypeSymbolSignatureView
{
    /// <inheritdoc/>
    public required IMethodSymbolSignatureView Signature { get; set; }
}

// Members

/// <summary>
/// Represents a light signature view of an <see cref="IFieldSymbol"/>,
/// implementing the <see cref="IFieldSymbolSignatureView"/>
/// interface as a baseline.
/// </summary>
/// <inheritdoc cref="SymbolSignatureView"/>
public record FieldSymbolSignatureView
    : SymbolSignatureView, IFieldSymbolSignatureView
{
    /// <inheritdoc/>
    public required RefKind RefKind { get; set; }

    /// <inheritdoc/>
    public required ITypeSymbolSignatureView Type { get; set; }

    /// <inheritdoc/>
    public required bool IsConst { get; set; }

    /// <inheritdoc/>
    public required bool IsReadOnly { get; set; }

    /// <inheritdoc/>
    public required bool IsVolatile { get; set; }

    /// <inheritdoc/>
    public required bool IsRequired { get; set; }

    /// <inheritdoc/>
    public required bool IsFixedSizeBuffer { get; set; }

    /// <inheritdoc/>
    public required int FixedSize { get; set; }

    /// <inheritdoc/>
    public required bool IsExplicitlyNamedTupleElement { get; set; }

    /// <inheritdoc/>
    public required bool HasConstantValue { get; set; }

    /// <inheritdoc/>
    public required object? ConstantValue { get; set; }
}

/// <summary>
/// Represents a light signature view of an <see cref="IPropertySymbol"/>,
/// implementing the <see cref="IPropertySymbolSignatureView"/>
/// interface as a baseline.
/// </summary>
/// <inheritdoc cref="SymbolSignatureView"/>
public record PropertySymbolSignatureView
    : SymbolSignatureView, IPropertySymbolSignatureView
{
    /// <inheritdoc/>
    public required RefKind RefKind { get; set; }

    /// <inheritdoc/>
    public required ITypeSymbolSignatureView Type { get; set; }

    /// <inheritdoc/>
    public required bool IsReadOnly { get; set; }

    /// <inheritdoc/>
    public required bool IsIndexer { get; set; }

    /// <inheritdoc/>
    public required bool IsRequired { get; set; }

    /// <inheritdoc/>
    public required EquatableImmutableArray<IParameterSymbolSignatureView> Parameters { get; set; }

    /// <inheritdoc/>
    public required Accessibility GetMethodAccessibility { get; set; }

    /// <inheritdoc/>
    public required Accessibility SetMethodAccessibility { get; set; }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return base.GetHashCode()
            ^ ((int)RefKind << 7);
    }
}

/// <summary>
/// Represents a light signature view of an <see cref="IEventSymbol"/>,
/// implementing the <see cref="IEventSymbolSignatureView"/>
/// interface as a baseline.
/// </summary>
/// <inheritdoc cref="SymbolSignatureView"/>
public record EventSymbolSignatureView
    : SymbolSignatureView, IEventSymbolSignatureView
{
    /// <inheritdoc/>
    public required ITypeSymbolSignatureView Type { get; set; }
}

/// <summary>
/// Represents a light signature view of an <see cref="IMethodSymbol"/>,
/// implementing the <see cref="IMethodSymbolSignatureView"/>
/// interface as a baseline.
/// </summary>
/// <inheritdoc cref="SymbolSignatureView"/>
public record MethodSymbolSignatureView
    : SymbolSignatureView, IMethodSymbolSignatureView
{
    /// <inheritdoc/>
    public required MethodKind MethodKind { get; set; }

    /// <inheritdoc/>
    public required RefKind RefKind { get; set; }

    /// <inheritdoc/>
    public required ITypeSymbolSignatureView ReturnType { get; set; }

    /// <inheritdoc/>
    public required EquatableImmutableArray<IParameterSymbolSignatureView> Parameters { get; set; }

    /// <inheritdoc/>
    public required EquatableImmutableArray<ITypeParameterSymbolSignatureView> TypeParameters { get; set; }

    /// <inheritdoc/>
    public required EquatableImmutableArray<ITypeSymbolSignatureView> TypeArguments { get; set; }

    /// <inheritdoc/>
    public required bool IsAsync { get; set; }

    /// <inheritdoc/>
    public required bool IsVararg { get; set; }

    /// <inheritdoc/>
    public required SignatureCallingConvention CallingConvention { get; set; }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return base.GetHashCode()
            ^ (Parameters.Array.Length << 6)
            ^ (TypeParameters.Array.Length << 10);
    }
}

// Values

/// <summary>
/// Represents a light signature view of an <see cref="IDiscardSymbol"/>,
/// implementing the <see cref="IDiscardSymbolSignatureView"/>
/// interface as a baseline.
/// </summary>
/// <inheritdoc cref="SymbolSignatureView"/>
public record DiscardSymbolSignatureView
    : SymbolSignatureView, IDiscardSymbolSignatureView
{
    /// <inheritdoc/>
    public required ITypeSymbolSignatureView Type { get; set; }
}

/// <summary>
/// Represents a light signature view of an <see cref="IParameterSymbol"/>,
/// implementing the <see cref="IParameterSymbolSignatureView"/>
/// interface as a baseline.
/// </summary>
/// <inheritdoc cref="SymbolSignatureView"/>
public record ParameterSymbolSignatureView
    : SymbolSignatureView, IParameterSymbolSignatureView
{
    /// <inheritdoc/>
    public required ITypeSymbolSignatureView Type { get; set; }

    /// <inheritdoc/>
    public required RefKind RefKind { get; set; }

    /// <inheritdoc/>
    public required ScopedKind ScopedKind { get; set; }

    /// <inheritdoc/>
    public required bool IsParams { get; set; }

    /// <inheritdoc/>
    public required bool IsParamsArray { get; set; }

    /// <inheritdoc/>
    public required bool IsParamsCollection { get; set; }

    /// <inheritdoc/>
    public required bool IsThis { get; set; }

    /// <inheritdoc/>
    public required bool IsDiscard { get; set; }

    /// <inheritdoc/>
    public required int Ordinal { get; set; }

    /// <inheritdoc/>
    public required bool HasExplicitDefaultValue { get; set; }

    /// <inheritdoc/>
    public required object? ExplicitDefaultValue { get; set; }
}

/// <summary>
/// Represents a light signature view of an <see cref="ILocalSymbol"/>,
/// implementing the <see cref="ILocalSymbolSignatureView"/>
/// interface as a baseline.
/// </summary>
/// <inheritdoc cref="SymbolSignatureView"/>
public record LocalSymbolSignatureView
    : SymbolSignatureView, ILocalSymbolSignatureView
{
    /// <inheritdoc/>
    public required ITypeSymbolSignatureView Type { get; set; }

    /// <inheritdoc/>
    public required RefKind RefKind { get; set; }

    /// <inheritdoc/>
    public required ScopedKind ScopedKind { get; set; }

    /// <inheritdoc/>
    public required bool IsConst { get; set; }

    /// <inheritdoc/>
    public required bool HasConstantValue { get; set; }

    /// <inheritdoc/>
    public required object? ConstantValue { get; set; }

    /// <inheritdoc/>
    public required bool IsFixed { get; set; }

    /// <inheritdoc/>
    public required bool IsForEach { get; set; }

    /// <inheritdoc/>
    public required bool IsUsing { get; set; }
}

/// <summary>
/// Represents a light signature view of an <see cref="IRangeVariableSymbol"/>,
/// implementing the <see cref="IRangeVariableSymbolSignatureView"/>
/// interface as a baseline.
/// </summary>
/// <inheritdoc cref="SymbolSignatureView"/>
public record RangeVariableSymbolSignatureView
    : SymbolSignatureView, IRangeVariableSymbolSignatureView;

// Namespace

/// <summary>
/// Represents a light signature view of an <see cref="INamespaceSymbol"/>,
/// implementing the <see cref="INamespaceSymbolSignatureView"/>
/// interface as a baseline.
/// </summary>
/// <inheritdoc cref="SymbolSignatureView"/>
public record NamespaceSymbolSignatureView
    : NamespaceOrTypeSymbolSignatureView, INamespaceSymbolSignatureView
{
    /// <inheritdoc/>
    public required bool IsGlobalNamespace { get; set; }

    /// <inheritdoc/>
    public required NamespaceKind NamespaceKind { get; set; }

    /// <inheritdoc/>
    public required ICompilationSignatureView? ContainingCompilation { get; set; }
}

// Compilation

/// <summary>
/// Represents a light signature view of an <see cref="IPreprocessingSymbol"/>,
/// implementing the <see cref="IPreprocessingSymbolSignatureView"/>
/// interface as a baseline.
/// </summary>
/// <inheritdoc cref="SymbolSignatureView"/>
public record PreprocessingSymbolSignatureView
    : SymbolSignatureView, IPreprocessingSymbolSignatureView;

/// <summary>
/// Represents a light signature view of an <see cref="IAliasSymbol"/>,
/// implementing the <see cref="IAliasSymbolSignatureView"/>
/// interface as a baseline.
/// </summary>
/// <inheritdoc cref="SymbolSignatureView"/>
public record AliasSymbolSignatureView
    : SymbolSignatureView, IAliasSymbolSignatureView
{
    /// <inheritdoc/>
    public required INamespaceOrTypeSymbolSignatureView Target { get; set; }
}

/// <summary>
/// Represents a light signature view of an <see cref="IModuleSymbol"/>,
/// implementing the <see cref="IModuleSymbolSignatureView"/>
/// interface as a baseline.
/// </summary>
/// <inheritdoc cref="SymbolSignatureView"/>
public record ModuleSymbolSignatureView
    : SymbolSignatureView, IModuleSymbolSignatureView;

/// <summary>
/// Represents a light signature view of an <see cref="IAssemblySymbol"/>,
/// implementing the <see cref="IAssemblySymbolSignatureView"/>
/// interface as a baseline.
/// </summary>
/// <inheritdoc cref="SymbolSignatureView"/>
public record AssemblySymbolSignatureView
    : SymbolSignatureView, IAssemblySymbolSignatureView
{
    // IsInteractive left out because of the impracticality of supporting it,
    // as the main driver of symbol signature views implies the consumption in
    // incremental generators

    /// <inheritdoc/>
    public required IAssemblyIdentitySignatureView Identity { get; set; }
}
