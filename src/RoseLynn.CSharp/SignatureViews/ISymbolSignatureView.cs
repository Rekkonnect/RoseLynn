using System.Reflection.Metadata;
using Microsoft.CodeAnalysis;
using RoseLynn.Utilities;

namespace RoseLynn.CSharp.SignatureViews;

// This API can be largely auto-generated, including the classes.
// Consider creating a generator for this purpose, which will also be
// tested and run against the core library

// Base

/// <summary>
/// Represents a light signature view of an <see cref="ISymbol"/>.
/// </summary>
/// <remarks>
/// Symbol signature views are useful for achieving a lower memory footprint
/// during incremental generators' pipeline steps. Such implementations are only
/// providing absolutely necessary properties that constitute the symbol's
/// signature, discarding parent, nested or otherwise linked information.
/// </remarks>
public interface ISymbolSignatureView
{
    /// <inheritdoc cref="ISymbol.Name"/>
    public string Name { get; }

    /// <inheritdoc cref="ISymbol.MetadataName"/>
    public string MetadataName { get; }

    /// <summary>
    /// The fully qualified identifier of the symbol, which is the fully
    /// qualified name of the symbol, except without the type arguments.
    /// </summary>
    public string FullyQualifiedIdentifier { get; }

    /// <summary>
    /// The fully qualified name of the symbol, calculated with respect to the
    /// compilation that this symbol had this view created over. One such case
    /// is a compilation having an assembly reference with an alias, and
    /// qualifying the symbol's global namespace with the import alias, instead
    /// of the global alias.
    /// <br/>
    /// The qualified name is defined as such:
    /// <list type="bullet">
    ///     <item>
    ///     For namespaces and types, the namespace is qualified fully,
    ///     including the global namespace alias and the generic type
    ///     arguments or parameters, if any.
    ///     <br/>
    ///     For example, a qualified name can be:
    ///     <c>grid::GridLibrary.ExampleType&lt;global::System.DateTime&gt;</c>
    ///     </item>
    ///     <item>
    ///     For all other symbols, the qualified name is simply the identifier
    ///     of the symbol, including its generic type arguments or parameters,
    ///     if any.
    ///     <br/>
    ///     For example, a qualified name can be:
    ///     <c>ExampleMethod&lt;global::System.DateTime&gt;</c>
    ///     </item>
    /// </list>
    /// </summary>
    public string FullyQualifiedName { get; }

    /// <summary>
    /// The full metadata name of the symbol, which includes the fully qualified
    /// name of the symbol, but without the alias qualification, and including
    /// the generic suffix in the same fashion as the metadata name, if any.
    /// <br/>
    /// The full metadata name is defined as such:
    /// <list type="bullet">
    ///     <item>
    ///     For namespaces and named types, the namespace is qualified fully,
    ///     including the containing types, and the <c>`X</c> suffix if the type
    ///     is generic.
    ///     <br/>
    ///     For example, a full metadata name can be:
    ///     <c>System.Collections.Generic.List`1</c>
    ///     </item>
    ///     <item>
    ///     For pointer types and array types, the underlying element type's
    ///     full metadata name is included, followed by the appropriate suffix,
    ///     which is * for the pointer and [] or equivalent for the array type,
    ///     depending on its rank.
    ///     <br/>
    ///     For example, a full metadata name for an array type can be:
    ///     <c>System.Collections.Generic.List`1[]</c>
    ///     </item>
    ///     <item>
    ///     For function pointer types, the full metadata name is empty.
    ///     This type kind is not yet supported and has no real use so far.
    ///     </item>
    ///     <item>
    ///     For fields, properties, events and methods, the full metadata name
    ///     includes the containing type's full metadata name, followed by the
    ///     metadata name of the member, and the <c>``X</c> suffix if the member
    ///     is generic.
    ///     <br/>
    ///     For example, a full metadata name of a method can be:
    ///     <c>System.Collections.Generic.List`1.GenericMethod``1</c>
    ///     </item>
    ///     <item>
    ///     For all other symbols, the qualified name is simply the identifier
    ///     of the symbol, including its generic suffix, if any.
    ///     <br/>
    ///     For example, a full metadata name of a type parameter can be:
    ///     <c>TView</c>
    ///     </item>
    /// </list>
    /// </summary>
    public string FullMetadataName { get; }

    /// <summary>
    /// The full metadata name of the symbol, which includes the fully qualified
    /// name of the symbol, but without the alias qualification, and including
    /// the generic suffix in the same fashion as the metadata name, if any.
    /// <br/>
    /// The full metadata name is defined as such:
    /// <list type="bullet">
    ///     <item>
    ///     For namespaces, the contained namespace name does not include the
    ///     current namespace, and only qualifies the parent namespace, without
    ///     a global namespace alias.
    ///     <br/>
    ///     For example, given the namespace <c>System.Collections.Generic</c>
    ///     the contained namespace name is <c>System.Collections</c>.
    ///     </item>
    ///     <item>
    ///     For types, the namespace is qualified fully, even when the type
    ///     is contained inside other types. Only the containing namespaces
    ///     are included, disregarding the potential containing types.
    ///     <br/>
    ///     For example, a contained namespace name can be
    ///     <c>System.Collections.Generic</c>
    ///     </item>
    ///     <item>
    ///     For all other symbols, the contained namespace name is
    ///     <see langword="null"/>.
    ///     </item>
    /// </list>
    /// </summary>
    public string? ContainedNamespaceName { get; }

    /// <summary>
    /// The keyword that represents this symbol, if any.
    /// Otherwise <see langword="null"/>.
    /// </summary>
    /// <remarks>
    /// The keyword will contain potential nullability annotations, for
    /// reference types such as <see langword="string"/> or
    /// <see langword="object"/>, or the nullable struct shorthand for value
    /// types, such as <see langword="int"/>. When the type has the nullability
    /// annotation or wraps the <see cref="Nullable{T}"/> struct, the keyword
    /// will contain the '?' suffix.
    /// </remarks>
    public string? Keyword { get; }

    /// <inheritdoc cref="ISymbol.Kind"/>
    public SymbolKind Kind { get; }

    /// <inheritdoc cref="ISymbol.DeclaredAccessibility"/>
    public Accessibility DeclaredAccessibility { get; }

    /// <inheritdoc cref="ISymbol.IsStatic"/>
    public bool IsStatic { get; }

    /// <inheritdoc cref="ISymbol.IsSealed"/>
    public bool IsSealed { get; }

    /// <inheritdoc cref="ISymbol.IsAbstract"/>
    public bool IsAbstract { get; }

    /// <inheritdoc cref="ISymbol.IsVirtual"/>
    public bool IsVirtual { get; }

    /// <inheritdoc cref="ISymbol.IsOverride"/>
    public bool IsOverride { get; }

    /// <inheritdoc cref="ISymbol.IsExtern"/>
    public bool IsExtern { get; }
}

/// <summary>
/// Represents a light signature view of an <see cref="INamespaceOrTypeSymbol"/>.
/// </summary>
/// <inheritdoc cref="ISymbolSignatureView"/>
public interface INamespaceOrTypeSymbolSignatureView : ISymbolSignatureView;

// Types

/// <summary>
/// Represents a light signature view of an <see cref="ITypeSymbol"/>.
/// </summary>
/// <inheritdoc cref="ISymbolSignatureView"/>
public interface ITypeSymbolSignatureView : INamespaceOrTypeSymbolSignatureView
{
    /// <inheritdoc cref="ITypeSymbol.TypeKind"/>
    public TypeKind TypeKind { get; }

    /// <inheritdoc cref="ITypeSymbol.SpecialType"/>
    public SpecialType SpecialType { get; }

    /// <inheritdoc cref="ITypeSymbol.IsRecord"/>
    public bool IsRecord { get; }

    /// <inheritdoc cref="ITypeSymbol.IsValueType"/>
    public bool IsValueType { get; }

    /// <inheritdoc cref="ITypeSymbol.IsReferenceType"/>
    public bool IsReferenceType { get; }

    /// <inheritdoc cref="ITypeSymbol.NullableAnnotation"/>
    public NullableAnnotation NullableAnnotation { get; }

}

/// <summary>
/// Represents a light signature view of an <see cref="INamedTypeSymbol"/>.
/// </summary>
/// <inheritdoc cref="ISymbolSignatureView"/>
public interface INamedTypeSymbolSignatureView : ITypeSymbolSignatureView
{
    /// <inheritdoc cref="INamedTypeSymbol.IsFileLocal"/>
    public bool IsFileLocal { get; }

    /// <inheritdoc cref="INamedTypeSymbol.IsGenericType"/>
    public bool IsGenericType { get; }

    /// <inheritdoc cref="INamedTypeSymbol.TypeArguments"/>
    public EquatableImmutableArray<ITypeSymbolSignatureView> TypeArguments { get; }

    /// <inheritdoc cref="INamedTypeSymbol.TypeParameters"/>
    public EquatableImmutableArray<ITypeParameterSymbolSignatureView> TypeParameters { get; }

    /// <summary>
    /// The <see cref="SpecialType"/> of the underlying enum type of the symbol,
    /// Returns <see cref="SpecialType.None"/> if the type is not an enum type.
    /// </summary>
    public SpecialType EnumUnderlyingSpecialType { get; }

    /// <summary>
    /// The <see cref="SpecialType"/> of the type this type is constructed from,
    /// as returned from the <see cref="INamedTypeSymbol.ConstructedFrom"/>
    /// property. Returns <see cref="SpecialType.None"/> if the property returns
    /// <see langword="null"/>.
    /// </summary>
    public SpecialType ConstructedFromSpecialType { get; }

    /// <summary>
    /// Gets the containing types in the order in which they must be qualified
    /// in order to reach the current type.
    /// <br/>
    /// Consider the type StartType.InnerType.FinalType, where all three
    /// identifiers in the example represent a type. This property will return
    /// an array equivalent to [StartType, InnerType].
    /// </summary>
    public EquatableImmutableArray<INamedTypeSymbolSignatureView> ContainingTypes { get; }

    /// <summary>
    /// Gets the names of the tuple fields, if the type is a tuple, in the order
    /// of the tuple fields that match the type arguments' order.
    /// </summary>
    public EquatableImmutableArray<string> TupleFieldNames { get; }
}

/// <summary>
/// Represents a light signature view of an <see cref="ITypeParameterSymbol"/>.
/// </summary>
/// <inheritdoc cref="ISymbolSignatureView"/>
public interface ITypeParameterSymbolSignatureView : ITypeSymbolSignatureView
{
    /// <inheritdoc cref="ITypeParameterSymbol.Variance"/>
    public VarianceKind Variance { get; }

    /// <inheritdoc cref="ITypeParameterSymbol.TypeParameterKind"/>
    public TypeParameterKind TypeParameterKind { get; }

    /// <inheritdoc cref="ITypeParameterSymbol.Ordinal"/>
    public int Ordinal { get; }

    /// <inheritdoc cref="ITypeParameterSymbol.HasReferenceTypeConstraint"/>
    public bool HasReferenceTypeConstraint { get; }

    /// <inheritdoc cref="ITypeParameterSymbol.ReferenceTypeConstraintNullableAnnotation"/>
    public NullableAnnotation ReferenceTypeConstraintNullableAnnotation { get; }

    /// <inheritdoc cref="ITypeParameterSymbol.HasValueTypeConstraint"/>
    public bool HasValueTypeConstraint { get; }

    /// <inheritdoc cref="ITypeParameterSymbol.AllowsRefLikeType"/>
    public bool AllowsRefLikeType { get; }

    /// <inheritdoc cref="ITypeParameterSymbol.HasUnmanagedTypeConstraint"/>
    public bool HasUnmanagedTypeConstraint { get; }

    /// <inheritdoc cref="ITypeParameterSymbol.HasNotNullConstraint"/>
    public bool HasNotNullConstraint { get; }

    /// <inheritdoc cref="ITypeParameterSymbol.HasConstructorConstraint"/>
    public bool HasConstructorConstraint { get; }

    /// <inheritdoc cref="ITypeParameterSymbol.ConstraintTypes"/>
    public EquatableImmutableArray<ITypeSymbolSignatureView> ConstraintTypes { get; }
}

/// <summary>
/// Represents a light signature view of an <see cref="IArrayTypeSymbol"/>.
/// </summary>
/// <inheritdoc cref="ISymbolSignatureView"/>
public interface IArrayTypeSymbolSignatureView : ITypeSymbolSignatureView
{
    /// <inheritdoc cref="IArrayTypeSymbol.Rank"/>
    public int Rank { get; }

    /// <inheritdoc cref="IArrayTypeSymbol.Sizes"/>
    public EquatableImmutableArray<int> Sizes { get; }

    /// <inheritdoc cref="IArrayTypeSymbol.ElementType"/>
    public ITypeSymbolSignatureView ElementType { get; }
}

/// <summary>
/// Represents a light signature view of an <see cref="IDynamicTypeSymbol"/>.
/// </summary>
/// <inheritdoc cref="ISymbolSignatureView"/>
public interface IDynamicTypeSymbolSignatureView : ITypeSymbolSignatureView;

/// <summary>
/// Represents a light signature view of an <see cref="IPointerTypeSymbol"/>.
/// </summary>
/// <inheritdoc cref="ISymbolSignatureView"/>
public interface IPointerTypeSymbolSignatureView : ITypeSymbolSignatureView
{
    /// <inheritdoc cref="IPointerTypeSymbol.PointedAtType"/>
    public ITypeSymbolSignatureView PointedAtType { get; }
}

/// <summary>
/// Represents a light signature view of an <see cref="IFunctionPointerTypeSymbol"/>.
/// </summary>
/// <inheritdoc cref="ISymbolSignatureView"/>
public interface IFunctionPointerTypeSymbolSignatureView : ITypeSymbolSignatureView
{
    /// <inheritdoc cref="IFunctionPointerTypeSymbol.Signature"/>
    public IMethodSymbolSignatureView Signature { get; }
}

// Members

/// <summary>
/// Represents a light signature view of an <see cref="IFieldSymbol"/>.
/// </summary>
/// <inheritdoc cref="ISymbolSignatureView"/>
public interface IFieldSymbolSignatureView : ISymbolSignatureView
{
    /// <inheritdoc cref="IFieldSymbol.RefKind"/>
    public RefKind RefKind { get; }

    /// <inheritdoc cref="IFieldSymbol.Type"/>
    public ITypeSymbolSignatureView Type { get; }

    /// <inheritdoc cref="IFieldSymbol.IsConst"/>
    public bool IsConst { get; }

    /// <inheritdoc cref="IFieldSymbol.IsReadOnly"/>
    public bool IsReadOnly { get; }

    /// <inheritdoc cref="IFieldSymbol.IsVolatile"/>
    public bool IsVolatile { get; }

    /// <inheritdoc cref="IFieldSymbol.IsRequired"/>
    public bool IsRequired { get; }

    /// <inheritdoc cref="IFieldSymbol.IsFixedSizeBuffer"/>
    public bool IsFixedSizeBuffer { get; }

    /// <inheritdoc cref="IFieldSymbol.FixedSize"/>
    public int FixedSize { get; }

    /// <inheritdoc cref="IFieldSymbol.IsExplicitlyNamedTupleElement"/>
    public bool IsExplicitlyNamedTupleElement { get; }

    /// <inheritdoc cref="IFieldSymbol.HasConstantValue"/>
    public bool HasConstantValue { get; }

    /// <inheritdoc cref="IFieldSymbol.ConstantValue"/>
    public object? ConstantValue { get; }
}

/// <summary>
/// Represents a light signature view of an <see cref="IPropertySymbol"/>.
/// </summary>
/// <inheritdoc cref="ISymbolSignatureView"/>
public interface IPropertySymbolSignatureView : ISymbolSignatureView
{
    /// <inheritdoc cref="IPropertySymbol.RefKind"/>
    public RefKind RefKind { get; }

    /// <inheritdoc cref="IPropertySymbol.Type"/>
    public ITypeSymbolSignatureView Type { get; }

    /// <inheritdoc cref="IPropertySymbol.IsReadOnly"/>
    public bool IsReadOnly { get; }

    /// <inheritdoc cref="IPropertySymbol.IsIndexer"/>
    public bool IsIndexer { get; }

    /// <inheritdoc cref="IPropertySymbol.IsRequired"/>
    public bool IsRequired { get; }

    /// <inheritdoc cref="IPropertySymbol.Parameters"/>
    public EquatableImmutableArray<IParameterSymbolSignatureView> Parameters { get; }

    /// <summary>
    /// Gets the accessibility of the <see cref="IPropertySymbol.GetMethod"/>,
    /// or <see cref="Accessibility.NotApplicable"/> if the method is
    /// <see langword="null"/>.
    /// </summary>
    public Accessibility GetMethodAccessibility { get; }

    /// <summary>
    /// Gets the accessibility of the <see cref="IPropertySymbol.SetMethod"/>,
    /// or <see cref="Accessibility.NotApplicable"/> if the method is
    /// <see langword="null"/>.
    /// </summary>
    public Accessibility SetMethodAccessibility { get; }
}

/// <summary>
/// Represents a light signature view of an <see cref="IEventSymbol"/>.
/// </summary>
/// <inheritdoc cref="ISymbolSignatureView"/>
public interface IEventSymbolSignatureView : ISymbolSignatureView
{
    /// <inheritdoc cref="IEventSymbol.Type"/>
    public ITypeSymbolSignatureView Type { get; }
}

/// <summary>
/// Represents a light signature view of an <see cref="IMethodSymbol"/>.
/// </summary>
/// <inheritdoc cref="ISymbolSignatureView"/>
public interface IMethodSymbolSignatureView : ISymbolSignatureView
{
    /// <inheritdoc cref="IMethodSymbol.MethodKind"/>
    public MethodKind MethodKind { get; }

    /// <inheritdoc cref="IMethodSymbol.RefKind"/>
    public RefKind RefKind { get; }

    /// <inheritdoc cref="IMethodSymbol.ReturnType"/>
    public ITypeSymbolSignatureView ReturnType { get; }

    /// <inheritdoc cref="IMethodSymbol.Parameters"/>
    public EquatableImmutableArray<IParameterSymbolSignatureView> Parameters { get; }

    /// <inheritdoc cref="IMethodSymbol.TypeParameters"/>
    public EquatableImmutableArray<ITypeParameterSymbolSignatureView> TypeParameters { get; }

    /// <inheritdoc cref="IMethodSymbol.TypeArguments"/>
    public EquatableImmutableArray<ITypeSymbolSignatureView> TypeArguments { get; }

    /// <inheritdoc cref="IMethodSymbol.IsAsync"/>
    public bool IsAsync { get; }

    /// <inheritdoc cref="IMethodSymbol.IsVararg"/>
    public bool IsVararg { get; }

    /// <inheritdoc cref="IMethodSymbol.CallingConvention"/>
    public SignatureCallingConvention CallingConvention { get; }
}

// Values

/// <summary>
/// Represents a light signature view of an <see cref="IDiscardSymbol"/>.
/// </summary>
/// <inheritdoc cref="ISymbolSignatureView"/>
public interface IDiscardSymbolSignatureView : ISymbolSignatureView
{
    /// <inheritdoc cref="IDiscardSymbol.Type"/>
    public ITypeSymbolSignatureView Type { get; }
}

/// <summary>
/// Represents a light signature view of an <see cref="IParameterSymbol"/>.
/// </summary>
/// <inheritdoc cref="ISymbolSignatureView"/>
public interface IParameterSymbolSignatureView : ISymbolSignatureView
{
    /// <inheritdoc cref="IParameterSymbol.Type"/>
    public ITypeSymbolSignatureView Type { get; }

    /// <inheritdoc cref="IParameterSymbol.RefKind"/>
    public RefKind RefKind { get; }

    /// <inheritdoc cref="IParameterSymbol.ScopedKind"/>
    public ScopedKind ScopedKind { get; }

    /// <inheritdoc cref="IParameterSymbol.IsParams"/>
    public bool IsParams { get; }

    /// <inheritdoc cref="IParameterSymbol.IsParamsArray"/>
    public bool IsParamsArray { get; }

    /// <inheritdoc cref="IParameterSymbol.IsParamsCollection"/>
    public bool IsParamsCollection { get; }

    /// <inheritdoc cref="IParameterSymbol.IsThis"/>
    public bool IsThis { get; }

    /// <inheritdoc cref="IParameterSymbol.IsDiscard"/>
    public bool IsDiscard { get; }

    /// <inheritdoc cref="IParameterSymbol.Ordinal"/>
    public int Ordinal { get; }

    /// <inheritdoc cref="IParameterSymbol.HasExplicitDefaultValue"/>
    public bool HasExplicitDefaultValue { get; }

    /// <inheritdoc cref="IParameterSymbol.ExplicitDefaultValue"/>
    public object? ExplicitDefaultValue { get; }
}

/// <summary>
/// Represents a light signature view of an <see cref="ILocalSymbol"/>.
/// </summary>
/// <inheritdoc cref="ISymbolSignatureView"/>
public interface ILocalSymbolSignatureView : ISymbolSignatureView
{
    /// <inheritdoc cref="ILocalSymbol.Type"/>
    public ITypeSymbolSignatureView Type { get; }

    /// <inheritdoc cref="ILocalSymbol.RefKind"/>
    public RefKind RefKind { get; }

    /// <inheritdoc cref="ILocalSymbol.ScopedKind"/>
    public ScopedKind ScopedKind { get; }

    /// <inheritdoc cref="ILocalSymbol.IsConst"/>
    public bool IsConst { get; }

    /// <inheritdoc cref="ILocalSymbol.HasConstantValue"/>
    public bool HasConstantValue { get; }

    /// <inheritdoc cref="ILocalSymbol.ConstantValue"/>
    public object? ConstantValue { get; }

    /// <inheritdoc cref="ILocalSymbol.IsFixed"/>
    public bool IsFixed { get; }

    /// <inheritdoc cref="ILocalSymbol.IsForEach"/>
    public bool IsForEach { get; }

    /// <inheritdoc cref="ILocalSymbol.IsUsing"/>
    public bool IsUsing { get; }
}

/// <summary>
/// Represents a light signature view of an <see cref="IRangeVariableSymbol"/>.
/// </summary>
/// <inheritdoc cref="ISymbolSignatureView"/>
public interface IRangeVariableSymbolSignatureView : ISymbolSignatureView;

// Namespace

/// <summary>
/// Represents a light signature view of an <see cref="INamespaceSymbol"/>.
/// </summary>
/// <inheritdoc cref="ISymbolSignatureView"/>
public interface INamespaceSymbolSignatureView : INamespaceOrTypeSymbolSignatureView
{
    /// <inheritdoc cref="INamespaceSymbol.IsGlobalNamespace"/>
    public bool IsGlobalNamespace { get; }

    /// <inheritdoc cref="INamespaceSymbol.NamespaceKind"/>
    public NamespaceKind NamespaceKind { get; }

    /// <inheritdoc cref="INamespaceSymbol.ContainingCompilation"/>
    public ICompilationSignatureView? ContainingCompilation { get; }
}

// Compilation

/// <summary>
/// Represents a light signature view of an <see cref="IPreprocessingSymbol"/>.
/// </summary>
/// <inheritdoc cref="ISymbolSignatureView"/>
public interface IPreprocessingSymbolSignatureView : ISymbolSignatureView;

/// <summary>
/// Represents a light signature view of an <see cref="IAliasSymbol"/>.
/// </summary>
/// <inheritdoc cref="ISymbolSignatureView"/>
public interface IAliasSymbolSignatureView : ISymbolSignatureView
{
    /// <inheritdoc cref="IAliasSymbol.Target"/>
    public INamespaceOrTypeSymbolSignatureView Target { get; }
}

/// <summary>
/// Represents a light signature view of an <see cref="IModuleSymbol"/>.
/// </summary>
/// <inheritdoc cref="ISymbolSignatureView"/>
public interface IModuleSymbolSignatureView : ISymbolSignatureView;

/// <summary>
/// Represents a light signature view of an <see cref="IAssemblySymbol"/>.
/// </summary>
/// <inheritdoc cref="ISymbolSignatureView"/>
public interface IAssemblySymbolSignatureView : ISymbolSignatureView
{
    // IsInteractive left out because of the impracticality of supporting it,
    // as the main driver of symbol signature views implies the consumption in
    // incremental generators

    /// <inheritdoc cref="IAssemblySymbol.Identity"/>
    public IAssemblyIdentitySignatureView Identity { get; }
}
