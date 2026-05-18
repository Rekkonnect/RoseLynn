
using Garyon.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using RoseLynn.CSharp;
using System.Collections.Immutable;

namespace RoseLynn;

/// <summary>Represents a mapped view of an attribute's constructor and named arguments.</summary>
public sealed class AttributeDataViewModel
{
    /// <summary>Gets the underlying Roslyn attribute data.</summary>
    public required AttributeData AttributeData { get; init; }
    /// <summary>Gets the mapped constructor arguments.</summary>
    public required ImmutableArray<MappedAttributeArgument> ConstructorArguments { get; init; }
    /// <summary>Gets the mapped named arguments.</summary>
    public required ImmutableArray<MappedAttributeArgument> NamedArguments { get; init; }

    /// <summary>Gets all mapped arguments in declaration order, starting with constructor arguments.</summary>
    public ImmutableArray<MappedAttributeArgument> AllArguments
    {
        get
        {
            return
            [
                .. ConstructorArguments,
                .. NamedArguments,
            ];
        }
    }

    private AttributeDataViewModel() { }

    /// <summary>Creates a view model asynchronously from attribute data.</summary>
    /// <param name="data">The attribute data to model.</param>
    /// <param name="cancellationToken">The token used to cancel syntax retrieval.</param>
    /// <returns>The created view model, or <see langword="null"/> if the syntax cannot be resolved.</returns>
    public static async Task<AttributeDataViewModel?> CreateAsync(
        AttributeData data,
        CancellationToken cancellationToken = default)
    {
        if (data.HasNoArguments())
            return Empty(data);

        if (data.ApplicationSyntaxReference is null)
            return null;

        var syntax = await data.ApplicationSyntaxReference.GetSyntaxAsync(cancellationToken);
        if (syntax is null)
            return null;

        return GetModelForSyntax(data, syntax);
    }

    /// <summary>Creates a view model synchronously from attribute data.</summary>
    /// <param name="data">The attribute data to model.</param>
    /// <param name="cancellationToken">The token used to cancel syntax retrieval.</param>
    /// <returns>The created view model, or <see langword="null"/> if the syntax cannot be resolved.</returns>
    public static AttributeDataViewModel? Create(
        AttributeData data,
        CancellationToken cancellationToken = default)
    {
        if (data.HasNoArguments())
            return Empty(data);

        var syntax = data.ApplicationSyntaxReference?.GetSyntax(cancellationToken);
        if (syntax is null)
            return null;

        return GetModelForSyntax(data, syntax);
    }

    private static AttributeDataViewModel? GetModelForSyntax(AttributeData data, SyntaxNode syntax)
    {
        return syntax switch
        {
            CSharpSyntaxNode csAttribute => CreateCSharpModel(data, csAttribute),
            _ => null,
        };
    }

    private static AttributeDataViewModel Empty(AttributeData data)
    {
        return new()
        {
            AttributeData = data,
            ConstructorArguments = [],
            NamedArguments = [],
        };
    }

    private static AttributeDataViewModel? CreateCSharpModel(
        AttributeData data, CSharpSyntaxNode csAttribute)
    {
        var attribute = csAttribute as Microsoft.CodeAnalysis.CSharp.Syntax.AttributeSyntax;
        if (attribute is null)
            return null;

        var argumentList = attribute.ArgumentList;
        var arguments = argumentList?.Arguments ?? [];
        return ConstructFromArguments(data, arguments);
    }

    // Disgustingly large method
    private static AttributeDataViewModel ConstructFromArguments<TSyntax>(
        AttributeData data, SeparatedSyntaxList<TSyntax> arguments)
        where TSyntax : SyntaxNode
    {
        ImmutableArray<MappedAttributeArgument> regularArguments = [];
        ImmutableArray<MappedAttributeArgument> namedArguments = [];

        int constructorArgumentSyntaxCount = ConstructorArgumentSyntaxCount(arguments);

        if (data.ConstructorArguments is not [] and var constructorArgumentsData)
        {
            var regularArgumentsBuilder = ImmutableArray.CreateBuilder<MappedAttributeArgument>(
                constructorArgumentsData.Length);

            // We always have a constructor if we have constructor arguments
            // We expect a potential breaking API change that returns a null
            // constructor with a non-empty argument list, in which case we show the param order
            var constructor = data.AttributeConstructor;
            var constructorParameters = constructor?.Parameters;

            var unconsumedDefaultValueParameters = constructorParameters
                ?.Where(s => s.HasExplicitDefaultValue)
                .ToDictionary(s => s.Name) ?? [];

            var defaultMappingKind =
                constructorParameters is not null
                ? AttributeArgumentMappingKind.Parameter
                : AttributeArgumentMappingKind.ParameterIndex;

            for (int i = 0; i < constructorArgumentsData.Length; i++)
            {
                // When the further arguments are named constructors, and we have information
                // about constructor arguments for parameters with default values, we can go
                // out of bounds for the constructor arguments and evaluate the named argument
                // here, which will be misattributed as a constructor argument
                if (i >= constructorArgumentSyntaxCount)
                {
                    break;
                }

                var argumentSyntax = arguments[i];

                var argumentName = CrossSyntaxFactsEx.GetArgumentName(argumentSyntax);
                var parameter = constructorParameters?[i];
                if (argumentName is not null
                    && constructorParameters is not null)
                {
                    // Instead, lookup the parameter matching the named argument
                    parameter = constructorParameters.Value
                        .FirstOrDefault(s => s.Name == argumentName);
                }

                if (parameter is not null)
                {
                    unconsumedDefaultValueParameters.Remove(parameter.Name);
                }

                var argumentValue = constructorArgumentsData[parameter?.Ordinal ?? i];
                var displayName = argumentName ?? parameter?.Name ?? i.ToString();
                regularArgumentsBuilder.Add(new(
                    argumentSyntax,
                    displayName,
                    argumentValue,
                    defaultMappingKind));
            }

            var constructorArgumentCount = regularArgumentsBuilder.Count;
            var paramsMergedArguments = constructorArgumentSyntaxCount - constructorArgumentCount + 1;
            var hasParamsExpansion = paramsMergedArguments > 1;
            if (hasParamsExpansion)
            {
                FixParamsArgument();
            }

            ConsumeDefaultValueParameters();

            regularArguments = regularArgumentsBuilder.ToImmutable();

            void FixParamsArgument()
            {
                var lastMapping = regularArgumentsBuilder[^1];

                var includedArguments = arguments
                    .Skip(constructorArgumentCount - 1)
                    .Take(paramsMergedArguments)
                    .Select(static s => (SyntaxNodeOrToken)s)
                    .ToArray();
                var separators = arguments.GetSeparators().ToList();
                var includedSeparators = separators
                    .Skip(constructorArgumentCount - 1)
                    .Take(paramsMergedArguments - 1)
                    .Select(static s => (SyntaxNodeOrToken)s)
                    .ToArray();
                var paramsArgumentSyntaxes = InterleaveSeparated(includedArguments, includedSeparators);
                var paramsArgumentList = new SyntaxNodeOrTokenList(paramsArgumentSyntaxes);
                regularArgumentsBuilder[^1] = lastMapping with
                {
                    ArgumentSyntax = null,
                    ArgumentSyntaxList = paramsArgumentList,
                    MappingKind = AttributeArgumentMappingKind.ParameterParamsExpansion,
                };
            }

            void ConsumeDefaultValueParameters()
            {
                // We have exceeded the argument nodes, and are now trying to figure
                // out which default values are assigned from which typed constants
                // that can fit the parameter; identical values can be distributed
                //.across the multiple candidate parameters non-deterministically,
                // though without a real problem, as any combination will be equally valid

                while (unconsumedDefaultValueParameters.Count > 0)
                {
                    var parameter = unconsumedDefaultValueParameters.Values.First();
                    var argumentValue = constructorArgumentsData[parameter.Ordinal];
                    var parameterName = parameter.Name;
                    unconsumedDefaultValueParameters.Remove(parameterName);

                    var matches = MatchesDefaultValue(argumentValue, parameter);
                    if (!matches)
                    {
                        // This should never be the case, but we might have missed something
                        // Ignore the error but silently ignore the parameter also
                        continue;
                    }

                    regularArgumentsBuilder.Add(new(
                        parameterName,
                        argumentValue,
                        AttributeArgumentMappingKind.ParameterDefaultValue));
                }
            }
        }

        if (data.NamedArguments is not [] and var namedArgumentsData)
        {
            var namedArgumentsBuilder = ImmutableArray.CreateBuilder<MappedAttributeArgument>(
                namedArgumentsData.Length);

            for (int i = 0; i < namedArgumentsData.Length; i++)
            {
                var argumentSyntax = arguments.AtIndexOrDefaultReadOnly(i + constructorArgumentSyntaxCount);
                if (argumentSyntax is null)
                {
                    break;
                }

                var kvp = namedArgumentsData[i];
                var name = kvp.Key;
                var value = kvp.Value;
                namedArgumentsBuilder.Add(new(
                    argumentSyntax,
                    name,
                    value,
                    AttributeArgumentMappingKind.Named));
            }
            namedArguments = namedArgumentsBuilder.ToImmutable();
        }

        return new()
        {
            AttributeData = data,
            ConstructorArguments = regularArguments,
            NamedArguments = namedArguments,
        };
    }

    private static bool MatchesDefaultValue(
        TypedConstant typedConstant,
        IParameterSymbol parameter)
    {
        if (!parameter.HasExplicitDefaultValue)
        {
            return false;
        }

        // Special-casing to avoid traps in further type checks
        if (typedConstant.IsNull && parameter.ExplicitDefaultValue is null)
        {
            return true;
        }

        return parameter.Type.Equals(typedConstant.Type, SymbolEqualityComparer.Default)
            && Equals(typedConstant.Value, parameter.ExplicitDefaultValue);
    }

    private static ImmutableArray<T> InterleaveSeparated<T>(
        IReadOnlyList<T> items,
        IReadOnlyList<T> separators)
    {
        int totalCount = items.Count + separators.Count;
        int minCount = Math.Min(items.Count, separators.Count);
        var builder = ImmutableArray.CreateBuilder<T>(totalCount);

        for (int i = 0; i < minCount; i++)
        {
            builder.Add(items[i]);
            builder.Add(separators[i]);
        }

        // Only one of the two will have remaining items
        builder.AddRange(items.Skip(minCount));
        builder.AddRange(separators.Skip(minCount));

        return builder.ToImmutable();
    }

    private static int ConstructorArgumentSyntaxCount<TSyntax>(
        SeparatedSyntaxList<TSyntax> arguments)
        where TSyntax : SyntaxNode
    {
        for (int i = 0; i < arguments.Count; i++)
        {
            var argument = arguments[i];
            var argumentName = CrossSyntaxFactsEx.GetNamedArgumentMemberName(argument);

            if (argumentName is not null)
            {
                return i;
            }
        }

        return arguments.Count;
    }

    /// <summary>
    /// Represents a mapping between an attribute argument's value and the
    /// syntax.
    /// </summary>
    /// <param name="ArgumentSyntax">
    /// <para>
    /// The argument syntax node.
    /// </para>
    /// <para>
    /// May be <see langword="null"/> if the argument is an array expression
    /// targeting a <see langword="params"/> parameter and spread out across
    /// multiple argument nodes. For example, when the attribute constructor is
    /// defined as <c>A(params int[] a)</c>, and the attribute is created as
    /// <c>[A(1, 2)]</c>, the <c>1, 2</c> argument list is not isolated in a
    /// single syntax node, and thus this property will be
    /// <see langword="null"/>. The argument syntax nodes will be provided in
    /// the <see cref="ArgumentSyntaxList"/>.
    /// </para>
    /// </param>
    /// <param name="ArgumentSyntaxList">
    /// Contains all the argument syntaxes that compose the attribute argument,
    /// which may include more than one argument nodes in the case of a
    /// <see langword="params"/> parameter. See the documentation for
    /// <see cref="ArgumentSyntax"/> for more information.
    /// </param>
    /// <param name="Name">
    /// The name of the mapped parameter, or type member that the argument
    /// involves.
    /// </param>
    /// <param name="Value">
    /// The argument's value.
    /// </param>
    /// <param name="MappingKind">
    /// The kind of the argument mapping.
    /// </param>
    public sealed record class MappedAttributeArgument(
        SyntaxNode? ArgumentSyntax,
        SyntaxNodeOrTokenList ArgumentSyntaxList,
        string Name,
        TypedConstant Value,
        AttributeArgumentMappingKind MappingKind)
    {
        public MappedAttributeArgument(
            string name,
            TypedConstant value,
            AttributeArgumentMappingKind mappingKind)
            : this(null, [], name, value, mappingKind)
        {
        }

        public MappedAttributeArgument(
            SyntaxNode argumentSyntax,
            string name,
            TypedConstant value,
            AttributeArgumentMappingKind mappingKind)
            : this(argumentSyntax, [argumentSyntax], name, value, mappingKind)
        {
        }

        public MappedAttributeArgument(
            SyntaxNodeOrTokenList argumentSyntaxList,
            string name,
            TypedConstant value,
            AttributeArgumentMappingKind mappingKind)
            : this(null, argumentSyntaxList, name, value, mappingKind)
        {
        }
    }

    /// <summary>Describes how an attribute argument was matched back to constructor or named-member data.</summary>
    public enum AttributeArgumentMappingKind
    {
        /// <summary>
        /// Invalid mapping kind value.
        /// </summary>
        None,

        /// <summary>
        /// Denotes that the argument was mapped to a constructor parameter.
        /// </summary>
        Parameter,

        /// <summary>
        /// Denotes that the argument is the default value of a constructor
        /// parameter with no syntax assigning its value.
        /// </summary>
        ParameterDefaultValue,

        /// <summary>
        /// Denotes that the argument is an expansion of a
        /// <see langword="params"/> parameter and thus accommodates multiple
        /// syntaxes to compose the array value being assigned to the parameter.
        /// </summary>
        ParameterParamsExpansion,

        /// <summary>
        /// Denotes that the argument could not mapped to a constructor
        /// parameter, and that its index is the only known information.
        /// </summary>
        ParameterIndex,

        /// <summary>
        /// Denotes that the argument was mapped to a member, as it's a named
        /// attribute constructor argument.
        /// </summary>
        Named,
    }
}
