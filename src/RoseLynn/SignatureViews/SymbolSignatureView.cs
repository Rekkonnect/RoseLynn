using System.Reflection.Metadata;
using Microsoft.CodeAnalysis;
using RoseLynn.Utilities;

namespace RoseLynn.SignatureViews;

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
    public string Name { get; set; } = null!;

    public string MetadataName { get; set; } = null!;

    public string FullyQualifiedIdentifier { get; set; } = null!;

    public string FullyQualifiedName { get; set; } = null!;

    public string FullMetadataName { get; set; } = null!;

    public string? ContainedNamespaceName { get; set; }

    public string? Keyword { get; set; }

    public SymbolKind Kind { get; set; }

    public Accessibility DeclaredAccessibility { get; set; }

    public bool IsStatic { get; set; }

    public bool IsSealed { get; set; }

    public bool IsAbstract { get; set; }

    public bool IsVirtual { get; set; }

    public bool IsOverride { get; set; }

    public bool IsExtern { get; set; }

    // A quicker hash to improve performance, saving up lots of
    // unnecessary calculations
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
    public TypeKind TypeKind { get; set; }

    public SpecialType SpecialType { get; set; }

    public bool IsRecord { get; set; }

    public bool IsValueType { get; set; }

    public bool IsReferenceType { get; set; }

    public NullableAnnotation NullableAnnotation { get; set; }

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
    public required bool IsFileLocal { get; set; }

    public required bool IsGenericType { get; set; }

    public required EquatableImmutableArray<ITypeSymbolSignatureView> TypeArguments { get; set; }

    public required EquatableImmutableArray<ITypeParameterSymbolSignatureView> TypeParameters { get; set; }

    public required SpecialType EnumUnderlyingSpecialType { get; set; }

    public required SpecialType ConstructedFromSpecialType { get; set; }

    public required EquatableImmutableArray<INamedTypeSymbolSignatureView> ContainingTypes { get; set; }

    public EquatableImmutableArray<string> TupleFieldNames { get; set; }

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
    public required VarianceKind Variance { get; set; }

    public required TypeParameterKind TypeParameterKind { get; set; }

    public required int Ordinal { get; set; }

    public required bool HasReferenceTypeConstraint { get; set; }

    public required NullableAnnotation ReferenceTypeConstraintNullableAnnotation { get; set; }

    public required bool HasValueTypeConstraint { get; set; }

    public required bool AllowsRefLikeType { get; set; }

    public required bool HasUnmanagedTypeConstraint { get; set; }

    public required bool HasNotNullConstraint { get; set; }

    public required bool HasConstructorConstraint { get; set; }

    public required EquatableImmutableArray<ITypeSymbolSignatureView> ConstraintTypes { get; set; }

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
    public required int Rank { get; set; }

    public required EquatableImmutableArray<int> Sizes { get; set; }

    public required ITypeSymbolSignatureView ElementType { get; set; }

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
    public required RefKind RefKind { get; set; }

    public required ITypeSymbolSignatureView Type { get; set; }

    public required bool IsConst { get; set; }

    public required bool IsReadOnly { get; set; }

    public required bool IsVolatile { get; set; }

    public required bool IsRequired { get; set; }

    public required bool IsFixedSizeBuffer { get; set; }

    public required int FixedSize { get; set; }

    public required bool IsExplicitlyNamedTupleElement { get; set; }

    public required bool HasConstantValue { get; set; }

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
    public required RefKind RefKind { get; set; }

    public required ITypeSymbolSignatureView Type { get; set; }

    public required bool IsReadOnly { get; set; }

    public required bool IsIndexer { get; set; }

    public required bool IsRequired { get; set; }

    public required EquatableImmutableArray<IParameterSymbolSignatureView> Parameters { get; set; }

    public required Accessibility GetMethodAccessibility { get; set; }

    public required Accessibility SetMethodAccessibility { get; set; }

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
    public required MethodKind MethodKind { get; set; }

    public required RefKind RefKind { get; set; }

    public required ITypeSymbolSignatureView ReturnType { get; set; }

    public required EquatableImmutableArray<IParameterSymbolSignatureView> Parameters { get; set; }

    public required EquatableImmutableArray<ITypeParameterSymbolSignatureView> TypeParameters { get; set; }

    public required EquatableImmutableArray<ITypeSymbolSignatureView> TypeArguments { get; set; }

    public required bool IsAsync { get; set; }

    public required bool IsVararg { get; set; }

    public required SignatureCallingConvention CallingConvention { get; set; }

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
    public required ITypeSymbolSignatureView Type { get; set; }

    public required RefKind RefKind { get; set; }

    public required ScopedKind ScopedKind { get; set; }

    public required bool IsParams { get; set; }

    public required bool IsParamsArray { get; set; }

    public required bool IsParamsCollection { get; set; }

    public required bool IsThis { get; set; }

    public required bool IsDiscard { get; set; }

    public required int Ordinal { get; set; }

    public required bool HasExplicitDefaultValue { get; set; }

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
    public required ITypeSymbolSignatureView Type { get; set; }

    public required RefKind RefKind { get; set; }

    public required ScopedKind ScopedKind { get; set; }

    public required bool IsConst { get; set; }

    public required bool HasConstantValue { get; set; }

    public required object? ConstantValue { get; set; }

    public required bool IsFixed { get; set; }

    public required bool IsForEach { get; set; }

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
    public required bool IsGlobalNamespace { get; set; }

    public required NamespaceKind NamespaceKind { get; set; }

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

    public required IAssemblyIdentitySignatureView Identity { get; set; }
}
