using System.Diagnostics.CodeAnalysis;
using Garyon.Extensions;
using Garyon.Functions;
using Microsoft.CodeAnalysis;
using RoseLynn.Core.Utilities;

namespace RoseLynn.Core.SignatureViews;

public static partial class SignatureViewExtensions
{
    /// <summary>Creates a signature view for the given <see cref="Compilation"/>.</summary>
    /// <param name="compilation">The compilation to view, or <see langword="null"/>.</param>
    /// <returns>The created view, or <see langword="null"/> if <paramref name="compilation"/> is <see langword="null"/>.</returns>
    [return: NotNullIfNotNull(nameof(compilation))]
    public static ICompilationSignatureView? ToSignatureView(
        this Compilation? compilation
    )
    {
        if (compilation is null)
        {
            return null;
        }

        return new CompilationSignatureView
        {
            Assembly = ToUpcastSymbolSignatureView<IAssemblySymbolSignatureView>(
                compilation.Assembly, compilation),
            Language = compilation.Language,
            DynamicType = ToUpcastSymbolSignatureView<IDynamicTypeSymbolSignatureView>(
                compilation.DynamicType, compilation),
        };
    }

    /// <summary>Creates a signature view for the given <see cref="AssemblyIdentity"/>.</summary>
    /// <param name="identity">The identity to view, or <see langword="null"/>.</param>
    /// <returns>The created view, or <see langword="null"/> if <paramref name="identity"/> is <see langword="null"/>.</returns>
    [return: NotNullIfNotNull(nameof(identity))]
    public static IAssemblyIdentitySignatureView? ToSignatureView(
        this AssemblyIdentity? identity
    )
    {
        if (identity is null)
        {
            return null;
        }

        return new AssemblyIdentitySignatureView
        {
            Name = identity.Name,
            Version = identity.Version,
            CultureName = identity.CultureName,
            Flags = identity.Flags,
            ContentType = identity.ContentType,
            PublicKey = identity.PublicKey,
            IsRetargetable = identity.IsRetargetable,
        };
    }

    /// <summary>Creates a signature view for the given <see cref="ISymbol"/>.</summary>
    /// <param name="symbol">The symbol to create a view for.</param>
    /// <param name="compilation">The compilation context used to resolve fully qualified names.</param>
    /// <returns>The created signature view.</returns>
    public static ISymbolSignatureView ToSymbolSignatureView(
        this ISymbol symbol,
        Compilation? compilation
    )
    {
        var constructor = new SignatureViewConstructor(compilation);
        constructor.Visit(symbol);
        return constructor.Constructed;
    }

    // Types

    /// <summary>Creates a signature view for the given <see cref="ITypeSymbol"/>.</summary>
    /// <param name="symbol">The symbol to create a view for.</param>
    /// <param name="compilation">The compilation context used to resolve fully qualified names.</param>
    /// <returns>The created signature view.</returns>
    public static ITypeSymbolSignatureView ToSymbolSignatureView(
        this ITypeSymbol symbol,
        Compilation? compilation
    )
    {
        return ToUpcastSymbolSignatureView<ITypeSymbolSignatureView>(
            symbol, compilation);
    }

    /// <summary>Creates a signature view for the given <see cref="INamedTypeSymbol"/>.</summary>
    /// <param name="symbol">The symbol to create a view for.</param>
    /// <param name="compilation">The compilation context used to resolve fully qualified names.</param>
    /// <returns>The created signature view.</returns>
    public static INamedTypeSymbolSignatureView ToSymbolSignatureView(
        this INamedTypeSymbol symbol,
        Compilation? compilation
    )
    {
        return ToUpcastSymbolSignatureView<INamedTypeSymbolSignatureView>(
            symbol, compilation);
    }

    /// <summary>Creates a signature view for the given <see cref="ITypeParameterSymbol"/>.</summary>
    /// <param name="symbol">The symbol to create a view for.</param>
    /// <param name="compilation">The compilation context used to resolve fully qualified names.</param>
    /// <returns>The created signature view.</returns>
    public static ITypeParameterSymbolSignatureView ToSymbolSignatureView(
        this ITypeParameterSymbol symbol,
        Compilation? compilation
    )
    {
        return ToUpcastSymbolSignatureView<ITypeParameterSymbolSignatureView>(
            symbol, compilation);
    }

    /// <summary>Creates a signature view for the given <see cref="IArrayTypeSymbol"/>.</summary>
    /// <param name="symbol">The symbol to create a view for.</param>
    /// <param name="compilation">The compilation context used to resolve fully qualified names.</param>
    /// <returns>The created signature view.</returns>
    public static IArrayTypeSymbolSignatureView ToSymbolSignatureView(
        this IArrayTypeSymbol symbol,
        Compilation? compilation
    )
    {
        return ToUpcastSymbolSignatureView<IArrayTypeSymbolSignatureView>(
            symbol, compilation);
    }

    /// <summary>Creates a signature view for the given <see cref="IDynamicTypeSymbol"/>.</summary>
    /// <param name="symbol">The symbol to create a view for.</param>
    /// <param name="compilation">The compilation context used to resolve fully qualified names.</param>
    /// <returns>The created signature view.</returns>
    public static IDynamicTypeSymbolSignatureView ToSymbolSignatureView(
        this IDynamicTypeSymbol symbol,
        Compilation? compilation
    )
    {
        return ToUpcastSymbolSignatureView<IDynamicTypeSymbolSignatureView>(
            symbol, compilation);
    }

    /// <summary>Creates a signature view for the given <see cref="IPointerTypeSymbol"/>.</summary>
    /// <param name="symbol">The symbol to create a view for.</param>
    /// <param name="compilation">The compilation context used to resolve fully qualified names.</param>
    /// <returns>The created signature view.</returns>
    public static IPointerTypeSymbolSignatureView ToSymbolSignatureView(
        this IPointerTypeSymbol symbol,
        Compilation? compilation
    )
    {
        return ToUpcastSymbolSignatureView<IPointerTypeSymbolSignatureView>(
            symbol, compilation);
    }

    /// <summary>Creates a signature view for the given <see cref="IFunctionPointerTypeSymbol"/>.</summary>
    /// <param name="symbol">The symbol to create a view for.</param>
    /// <param name="compilation">The compilation context used to resolve fully qualified names.</param>
    /// <returns>The created signature view.</returns>
    public static IFunctionPointerTypeSymbolSignatureView ToSymbolSignatureView(
        this IFunctionPointerTypeSymbol symbol,
        Compilation? compilation
    )
    {
        return ToUpcastSymbolSignatureView<IFunctionPointerTypeSymbolSignatureView>(
            symbol, compilation);
    }

    // Members

    /// <summary>Creates a signature view for the given <see cref="IFieldSymbol"/>.</summary>
    /// <param name="symbol">The symbol to create a view for.</param>
    /// <param name="compilation">The compilation context used to resolve fully qualified names.</param>
    /// <returns>The created signature view.</returns>
    public static IFieldSymbolSignatureView ToSymbolSignatureView(
        this IFieldSymbol symbol,
        Compilation? compilation
    )
    {
        return ToUpcastSymbolSignatureView<IFieldSymbolSignatureView>(
            symbol, compilation);
    }

    /// <summary>Creates a signature view for the given <see cref="IPropertySymbol"/>.</summary>
    /// <param name="symbol">The symbol to create a view for.</param>
    /// <param name="compilation">The compilation context used to resolve fully qualified names.</param>
    /// <returns>The created signature view.</returns>
    public static IPropertySymbolSignatureView ToSymbolSignatureView(
        this IPropertySymbol symbol,
        Compilation? compilation
    )
    {
        return ToUpcastSymbolSignatureView<IPropertySymbolSignatureView>(
            symbol, compilation);
    }

    /// <summary>Creates a signature view for the given <see cref="IEventSymbol"/>.</summary>
    /// <param name="symbol">The symbol to create a view for.</param>
    /// <param name="compilation">The compilation context used to resolve fully qualified names.</param>
    /// <returns>The created signature view.</returns>
    public static IEventSymbolSignatureView ToSymbolSignatureView(
        this IEventSymbol symbol,
        Compilation? compilation
    )
    {
        return ToUpcastSymbolSignatureView<IEventSymbolSignatureView>(
            symbol, compilation);
    }

    /// <summary>Creates a signature view for the given <see cref="IMethodSymbol"/>.</summary>
    /// <param name="symbol">The symbol to create a view for.</param>
    /// <param name="compilation">The compilation context used to resolve fully qualified names.</param>
    /// <returns>The created signature view.</returns>
    public static IMethodSymbolSignatureView ToSymbolSignatureView(
        this IMethodSymbol symbol,
        Compilation? compilation
    )
    {
        return ToUpcastSymbolSignatureView<IMethodSymbolSignatureView>(
            symbol, compilation);
    }

    // Values

    /// <summary>Creates a signature view for the given <see cref="IDiscardSymbol"/>.</summary>
    /// <param name="symbol">The symbol to create a view for.</param>
    /// <param name="compilation">The compilation context used to resolve fully qualified names.</param>
    /// <returns>The created signature view.</returns>
    public static IDiscardSymbolSignatureView ToSymbolSignatureView(
        this IDiscardSymbol symbol,
        Compilation? compilation
    )
    {
        return ToUpcastSymbolSignatureView<IDiscardSymbolSignatureView>(
            symbol, compilation);
    }

    /// <summary>Creates a signature view for the given <see cref="IParameterSymbol"/>.</summary>
    /// <param name="symbol">The symbol to create a view for.</param>
    /// <param name="compilation">The compilation context used to resolve fully qualified names.</param>
    /// <returns>The created signature view.</returns>
    public static IParameterSymbolSignatureView ToSymbolSignatureView(
        this IParameterSymbol symbol,
        Compilation? compilation
    )
    {
        return ToUpcastSymbolSignatureView<IParameterSymbolSignatureView>(
            symbol, compilation);
    }

    /// <summary>Creates a signature view for the given <see cref="ILocalSymbol"/>.</summary>
    /// <param name="symbol">The symbol to create a view for.</param>
    /// <param name="compilation">The compilation context used to resolve fully qualified names.</param>
    /// <returns>The created signature view.</returns>
    public static ILocalSymbolSignatureView ToSymbolSignatureView(
        this ILocalSymbol symbol,
        Compilation? compilation
    )
    {
        return ToUpcastSymbolSignatureView<ILocalSymbolSignatureView>(
            symbol, compilation);
    }

    /// <summary>Creates a signature view for the given <see cref="IRangeVariableSymbol"/>.</summary>
    /// <param name="symbol">The symbol to create a view for.</param>
    /// <param name="compilation">The compilation context used to resolve fully qualified names.</param>
    /// <returns>The created signature view.</returns>
    public static IRangeVariableSymbolSignatureView ToSymbolSignatureView(
        this IRangeVariableSymbol symbol,
        Compilation? compilation
    )
    {
        return ToUpcastSymbolSignatureView<IRangeVariableSymbolSignatureView>(
            symbol, compilation);
    }

    // Namespace

    /// <summary>Creates a signature view for the given <see cref="INamespaceSymbol"/>.</summary>
    /// <param name="symbol">The symbol to create a view for.</param>
    /// <param name="compilation">The compilation context used to resolve fully qualified names.</param>
    /// <returns>The created signature view.</returns>
    public static INamespaceSymbolSignatureView ToSymbolSignatureView(
        this INamespaceSymbol symbol,
        Compilation? compilation
    )
    {
        return ToUpcastSymbolSignatureView<INamespaceSymbolSignatureView>(
            symbol, compilation);
    }

    // Compilation

    /// <summary>Creates a signature view for the given <see cref="IPreprocessingSymbol"/>.</summary>
    /// <param name="symbol">The symbol to create a view for.</param>
    /// <param name="compilation">The compilation context used to resolve fully qualified names.</param>
    /// <returns>The created signature view.</returns>
    public static IPreprocessingSymbolSignatureView ToSymbolSignatureView(
        this IPreprocessingSymbol symbol,
        Compilation? compilation
    )
    {
        return ToUpcastSymbolSignatureView<IPreprocessingSymbolSignatureView>(
            symbol, compilation);
    }

    /// <summary>Creates a signature view for the given <see cref="IAliasSymbol"/>.</summary>
    /// <param name="symbol">The symbol to create a view for.</param>
    /// <param name="compilation">The compilation context used to resolve fully qualified names.</param>
    /// <returns>The created signature view.</returns>
    public static IAliasSymbolSignatureView ToSymbolSignatureView(
        this IAliasSymbol symbol,
        Compilation? compilation
    )
    {
        return ToUpcastSymbolSignatureView<IAliasSymbolSignatureView>(
            symbol, compilation);
    }

    /// <summary>Creates a signature view for the given <see cref="IModuleSymbol"/>.</summary>
    /// <param name="symbol">The symbol to create a view for.</param>
    /// <param name="compilation">The compilation context used to resolve fully qualified names.</param>
    /// <returns>The created signature view.</returns>
    public static IModuleSymbolSignatureView ToSymbolSignatureView(
        this IModuleSymbol symbol,
        Compilation? compilation
    )
    {
        return ToUpcastSymbolSignatureView<IModuleSymbolSignatureView>(
            symbol, compilation);
    }

    /// <summary>Creates a signature view for the given <see cref="IAssemblySymbol"/>.</summary>
    /// <param name="symbol">The symbol to create a view for.</param>
    /// <param name="compilation">The compilation context used to resolve fully qualified names.</param>
    /// <returns>The created signature view.</returns>
    public static IAssemblySymbolSignatureView ToSymbolSignatureView(
        this IAssemblySymbol symbol,
        Compilation? compilation
    )
    {
        return ToUpcastSymbolSignatureView<IAssemblySymbolSignatureView>(
            symbol, compilation);
    }

    // Private Helpers

    private static TView ToUpcastSymbolSignatureView<TView>(
        ISymbol symbol,
        Compilation? compilation
    )
        where TView : class, ISymbolSignatureView
    {
        return UpcastView<TView>(
            ToSymbolSignatureView(symbol, compilation));
    }

    private static EquatableImmutableArray<TView> ToUpcastSymbolSignatureViews<TView>(
        IEnumerable<ISymbol> symbols,
        Compilation? compilation
    )
        where TView : class, ISymbolSignatureView
    {
        return symbols
            .Select(symbol => ToUpcastSymbolSignatureView<TView>(
                symbol, compilation))
            .ToEquatableImmutableArray();
    }

    private static TView UpcastView<TView>(ISymbolSignatureView view)
        where TView : class
    {
        return view as TView
            ?? throw new InvalidOperationException("Unexpected signature view type");
    }

    /// <summary>Builds concrete signature-view instances by visiting Roslyn symbols.</summary>
    private sealed class SignatureViewConstructor(Compilation? compilation)
        : SymbolVisitor
    {
        private SymbolSignatureView? _view;

        /// <summary>Gets the signature view produced by the last visited symbol.</summary>
        public ISymbolSignatureView? Constructed => _view;

        // Types

        /// <inheritdoc/>
        public override void VisitNamedType(INamedTypeSymbol symbol)
        {
            _view = new NamedTypeSymbolSignatureView
            {
                IsFileLocal = symbol.IsFileLocal,
                IsGenericType = symbol.IsGenericType,
                TypeParameters = ToUpcastSymbolSignatureViews<ITypeParameterSymbolSignatureView>(
                    symbol.TypeParameters, compilation),
                TypeArguments = ToUpcastSymbolSignatureViews<ITypeSymbolSignatureView>(
                    symbol.TypeArguments, compilation),
                EnumUnderlyingSpecialType = symbol.EnumUnderlyingType?.SpecialType
                    ?? SpecialType.None,
                ConstructedFromSpecialType = symbol.ConstructedFrom?.SpecialType
                    ?? SpecialType.None,
                ContainingTypes = symbol.GetContainingTypes()
                    .Select(
                        containingType => ToUpcastSymbolSignatureView<INamedTypeSymbolSignatureView>(
                            containingType, compilation))
                    .ToEquatableImmutableArray(),
                TupleFieldNames = symbol.TupleElements.Safe()
                    .Select(tupleElement => tupleElement.Name)
                    .ToEquatableImmutableArray(),
            };
        }

        /// <inheritdoc/>
        public override void VisitTypeParameter(ITypeParameterSymbol symbol)
        {
            _view = new TypeParameterSymbolSignatureView
            {
                Variance = symbol.Variance,
                TypeParameterKind = symbol.TypeParameterKind,
                Ordinal = symbol.Ordinal,
                HasReferenceTypeConstraint = symbol.HasReferenceTypeConstraint,
                ReferenceTypeConstraintNullableAnnotation = symbol.ReferenceTypeConstraintNullableAnnotation,
                HasValueTypeConstraint = symbol.HasValueTypeConstraint,
                AllowsRefLikeType = symbol.AllowsRefLikeType,
                HasUnmanagedTypeConstraint = symbol.HasUnmanagedTypeConstraint,
                HasNotNullConstraint = symbol.HasNotNullConstraint,
                HasConstructorConstraint = symbol.HasConstructorConstraint,
                ConstraintTypes = ToUpcastSymbolSignatureViews<ITypeSymbolSignatureView>(
                    symbol.ConstraintTypes, compilation),
            };
        }

        /// <inheritdoc/>
        public override void VisitArrayType(IArrayTypeSymbol symbol)
        {
            _view = new ArrayTypeSymbolSignatureView
            {
                Rank = symbol.Rank,
                Sizes = symbol.Sizes.ToEquatableImmutableArray(),
                ElementType = ToUpcastSymbolSignatureView<ITypeSymbolSignatureView>(
                    symbol.ElementType, compilation),
            };
        }

        /// <inheritdoc/>
        public override void VisitDynamicType(IDynamicTypeSymbol symbol)
        {
            _view = new DynamicTypeSymbolSignatureView();
        }

        /// <inheritdoc/>
        public override void VisitPointerType(IPointerTypeSymbol symbol)
        {
            _view = new PointerTypeSymbolSignatureView
            {
                PointedAtType = ToUpcastSymbolSignatureView<ITypeSymbolSignatureView>(
                    symbol.PointedAtType, compilation),
            };
        }

        /// <inheritdoc/>
        public override void VisitFunctionPointerType(IFunctionPointerTypeSymbol symbol)
        {
            _view = new FunctionPointerTypeSymbolSignatureView
            {
                Signature = ToUpcastSymbolSignatureView<IMethodSymbolSignatureView>(
                    symbol.Signature, compilation),
            };
        }

        private void VisitType(ITypeSymbol symbol, TypeSymbolSignatureView view)
        {
            view.TypeKind = symbol.TypeKind;
            view.SpecialType = symbol.SpecialType;
            view.IsRecord = symbol.IsRecord;
            view.IsValueType = symbol.IsValueType;
            view.IsReferenceType = symbol.IsReferenceType;
            view.NullableAnnotation = symbol.NullableAnnotation;
        }

        // Members

        /// <inheritdoc/>
        public override void VisitField(IFieldSymbol symbol)
        {
            _view = new FieldSymbolSignatureView
            {
                Type = ToUpcastSymbolSignatureView<ITypeSymbolSignatureView>(
                    symbol.Type, compilation),
                RefKind = symbol.RefKind,
                IsConst = symbol.IsConst,
                IsReadOnly = symbol.IsReadOnly,
                IsVolatile = symbol.IsVolatile,
                IsRequired = symbol.IsRequired,
                IsFixedSizeBuffer = symbol.IsFixedSizeBuffer,
                FixedSize = symbol.FixedSize,
                IsExplicitlyNamedTupleElement = symbol.IsExplicitlyNamedTupleElement,
                HasConstantValue = symbol.HasConstantValue,
                ConstantValue = symbol.ConstantValue,
            };
        }

        /// <inheritdoc/>
        public override void VisitProperty(IPropertySymbol symbol)
        {
            _view = new PropertySymbolSignatureView
            {
                Type = ToUpcastSymbolSignatureView<ITypeSymbolSignatureView>(
                    symbol.Type, compilation),
                RefKind = symbol.RefKind,
                IsReadOnly = symbol.IsReadOnly,
                IsIndexer = symbol.IsIndexer,
                IsRequired = symbol.IsRequired,
                Parameters = ToUpcastSymbolSignatureViews<IParameterSymbolSignatureView>(
                    symbol.Parameters, compilation),
                GetMethodAccessibility = symbol.GetMethod?.DeclaredAccessibility
                    ?? Accessibility.NotApplicable,
                SetMethodAccessibility = symbol.SetMethod?.DeclaredAccessibility
                    ?? Accessibility.NotApplicable,
            };
        }

        /// <inheritdoc/>
        public override void VisitEvent(IEventSymbol symbol)
        {
            _view = new EventSymbolSignatureView
            {
                Type = ToUpcastSymbolSignatureView<ITypeSymbolSignatureView>(
                    symbol.Type, compilation),
            };
        }

        /// <inheritdoc/>
        public override void VisitMethod(IMethodSymbol symbol)
        {
            _view = new MethodSymbolSignatureView
            {
                ReturnType = ToUpcastSymbolSignatureView<ITypeSymbolSignatureView>(
                    symbol.ReturnType, compilation),
                MethodKind = symbol.MethodKind,
                RefKind = symbol.RefKind,
                Parameters = ToUpcastSymbolSignatureViews<IParameterSymbolSignatureView>(
                    symbol.Parameters, compilation),
                TypeParameters = ToUpcastSymbolSignatureViews<ITypeParameterSymbolSignatureView>(
                    symbol.TypeParameters, compilation),
                TypeArguments = ToUpcastSymbolSignatureViews<ITypeSymbolSignatureView>(
                    symbol.TypeArguments, compilation),
                IsAsync = symbol.IsAsync,
                IsVararg = symbol.IsVararg,
                CallingConvention = symbol.CallingConvention,
            };
        }

        // Values

        /// <inheritdoc/>
        public override void VisitDiscard(IDiscardSymbol symbol)
        {
            _view = new DiscardSymbolSignatureView
            {
                Type = ToUpcastSymbolSignatureView<ITypeSymbolSignatureView>(
                    symbol.Type, compilation),
            };
        }

        /// <inheritdoc/>
        public override void VisitParameter(IParameterSymbol symbol)
        {
            _view = new ParameterSymbolSignatureView
            {
                Type = ToUpcastSymbolSignatureView<ITypeSymbolSignatureView>(
                    symbol.Type, compilation),
                RefKind = symbol.RefKind,
                ScopedKind = symbol.ScopedKind,
                IsParams = symbol.IsParams,
                IsParamsArray = symbol.IsParamsArray,
                IsParamsCollection = symbol.IsParamsCollection,
                IsThis = symbol.IsThis,
                IsDiscard = symbol.IsDiscard,
                Ordinal = symbol.Ordinal,
                HasExplicitDefaultValue = symbol.HasExplicitDefaultValue,
                ExplicitDefaultValue = symbol.SafeExplicitDefaultValue(),
            };
        }

        /// <inheritdoc/>
        public override void VisitLocal(ILocalSymbol symbol)
        {
            _view = new LocalSymbolSignatureView
            {
                Type = ToUpcastSymbolSignatureView<ITypeSymbolSignatureView>(
                    symbol.Type, compilation),
                RefKind = symbol.RefKind,
                ScopedKind = symbol.ScopedKind,
                IsConst = symbol.IsConst,
                HasConstantValue = symbol.HasConstantValue,
                ConstantValue = symbol.ConstantValue,
                IsFixed = symbol.IsFixed,
                IsForEach = symbol.IsForEach,
                IsUsing = symbol.IsUsing,
            };
        }

        /// <inheritdoc/>
        public override void VisitRangeVariable(IRangeVariableSymbol symbol)
        {
            _view = new RangeVariableSymbolSignatureView();
        }

        // Namespace

        /// <inheritdoc/>
        public override void VisitNamespace(INamespaceSymbol symbol)
        {
            _view = new NamespaceSymbolSignatureView
            {
                IsGlobalNamespace = symbol.IsGlobalNamespace,
                NamespaceKind = symbol.NamespaceKind,
                ContainingCompilation = ToSignatureView(symbol.ContainingCompilation),
            };
        }

        // Compilation

        private void VisitPreprocessing(IPreprocessingSymbol preprocessing)
        {
            _view = new PreprocessingSymbolSignatureView();
        }

        /// <inheritdoc/>
        public override void VisitAlias(IAliasSymbol symbol)
        {
            _view = new AliasSymbolSignatureView
            {
                Target = ToUpcastSymbolSignatureView<INamespaceOrTypeSymbolSignatureView>(
                    symbol.Target, compilation),
            };
        }

        /// <inheritdoc/>
        public override void VisitModule(IModuleSymbol symbol)
        {
            _view = new ModuleSymbolSignatureView();
        }

        /// <inheritdoc/>
        public override void VisitAssembly(IAssemblySymbol symbol)
        {
            _view = new AssemblySymbolSignatureView
            {
                Identity = symbol.Identity.ToSignatureView(),
            };
        }

        /// <inheritdoc/>
        [MemberNotNull(nameof(Constructed))]
        public override void Visit(
            ISymbol? symbol)
        {
            Asserts.NotNull(symbol, "Do not visit a null symbol");

            // Let the object be first constructed and
            // then assign the rest of the properties
            VisitCore(symbol);

            Asserts.NotNull(_view, "The view must have been constructed");
            // Only for nullability analysis
            Asserts.NotNull(Constructed);

            _view.Name = symbol.Name;
            _view.MetadataName = symbol.MetadataName;
            _view.FullyQualifiedName = SymbolNameHelpers.FullyQualifiedString(symbol, compilation);
            _view.FullMetadataName = SymbolNameHelpers.FullMetadataString(symbol);
            _view.ContainedNamespaceName = SymbolNameHelpers.ContainedNamespaceName(symbol);
            _view.Kind = symbol.Kind;
            _view.DeclaredAccessibility = symbol.DeclaredAccessibility;
            _view.IsStatic = symbol.IsStatic;
            _view.IsSealed = symbol.IsSealed;
            _view.IsAbstract = symbol.IsAbstract;
            _view.IsVirtual = symbol.IsVirtual;
            _view.IsOverride = symbol.IsOverride;
            _view.IsExtern = symbol.IsExtern;

            _view.FullyQualifiedIdentifier
                = GetFullyQualifiedIdentifier(_view.FullyQualifiedName);
        }

        private static string GetFullyQualifiedIdentifier(string fullyQualifiedName)
        {
            return fullyQualifiedName.AsSpan().SliceBefore('<').ToString();
        }

        private void VisitCore(ISymbol symbol)
        {
            if (symbol is IPreprocessingSymbol preprocessing)
            {
                VisitPreprocessing(preprocessing);
                return;
            }

            base.Visit(symbol);

            if (symbol is ITypeSymbol type)
            {
                VisitType(
                    type,
                    _view as TypeSymbolSignatureView
                        ?? throw new InvalidOperationException(
                            "The view is expected to represent a type symbol"));
            }
        }
    }
}
