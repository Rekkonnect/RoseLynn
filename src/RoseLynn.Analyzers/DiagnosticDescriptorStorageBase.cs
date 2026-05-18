
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Frozen;
using System.Collections.Immutable;

namespace RoseLynn.Analyzers;

/// <summary>Provides mechanisms to function a storage of <seealso cref="DiagnosticDescriptor"/> instances that are grouped by analyzer type.</summary>
public abstract class DiagnosticDescriptorStorageBase
{
    private readonly FrozenDictionary<int, DiagnosticDescriptor> descriptorsByNumericId;
    private readonly FrozenDictionary<string, DiagnosticDescriptor> descriptorsById;
    private readonly FrozenDictionary<Type, ImmutableArray<DiagnosticDescriptor>> analyzerGroupedDiagnostics;

    /// <summary>Represents the count of digits the diagnostic ID ends in.</summary>
    /// <remarks>The storage is not intended to support numeric lookups on diagnostic IDs that do not end in exactly 4 decimal digits.</remarks>
    public const int DiagnosticIDDigits = 4;

    /// <summary>Initializes a new <see cref="DiagnosticDescriptorStorageBase"/> instance, discovering diagnostics from the provided analyzers.</summary>
    /// <param name="diagnosticAnalyzerTypes">The analyzer types whose declared diagnostics will be stored.</param>
    protected DiagnosticDescriptorStorageBase(params Type[] diagnosticAnalyzerTypes)
    {
        if (diagnosticAnalyzerTypes is null)
            throw new ArgumentNullException(nameof(diagnosticAnalyzerTypes));

        var groupedDiagnostics = DiscoverDiagnosticDescriptors(diagnosticAnalyzerTypes);

        analyzerGroupedDiagnostics = groupedDiagnostics.ToFrozenDictionary();
        descriptorsById = groupedDiagnostics
            .SelectMany(static kvp => kvp.Value)
            .ToFrozenDictionary(static descriptor => descriptor.Id, StringComparer.Ordinal);

        descriptorsByNumericId = descriptorsById.Values
            .Select(static descriptor => (descriptor, parsed: TryGetNumericDiagnosticID(descriptor.Id, out var id), id))
            .Where(static tuple => tuple.parsed)
            .ToFrozenDictionary(static tuple => tuple.id, static tuple => tuple.descriptor);
    }

    /// <summary>Gets the <seealso cref="DiagnosticDescriptor"/> representing the given rule ID.</summary>
    /// <param name="ruleID">The numeric suffix of the rule ID to get.</param>
    /// <returns>The <seealso cref="DiagnosticDescriptor"/> representing the given rule ID, if it exists, otherwise <see langword="null"/>.</returns>
    public DiagnosticDescriptor? GetDiagnosticDescriptor(int ruleID)
    {
        descriptorsByNumericId.TryGetValue(ruleID, out var descriptor);
        return descriptor;
    }
    /// <summary>Gets the <seealso cref="DiagnosticDescriptor"/> representing the given rule ID.</summary>
    /// <param name="ruleID">The exact rule ID whose <seealso cref="DiagnosticDescriptor"/> to get.</param>
    /// <returns>The <seealso cref="DiagnosticDescriptor"/> representing the given rule ID, if it exists, otherwise <see langword="null"/>.</returns>
    public DiagnosticDescriptor? GetDiagnosticDescriptor(string ruleID)
    {
        descriptorsById.TryGetValue(ruleID, out var descriptor);
        return descriptor;
    }

    /// <summary>Gets all the stored <seealso cref="DiagnosticDescriptor"/>s in this storage mapped to their associated <seealso cref="DiagnosticAnalyzer"/> types.</summary>
    /// <returns>A frozen dictionary mapping the types of <seealso cref="DiagnosticAnalyzer"/>s to their respective <seealso cref="ImmutableArray{T}"/> of associated <seealso cref="DiagnosticDescriptor"/>s.</returns>
    public FrozenDictionary<Type, ImmutableArray<DiagnosticDescriptor>> GetDiagnosticDescriptorsByAnalyzersImmutable()
    {
        return analyzerGroupedDiagnostics;
    }
    /// <summary>Gets the <see cref="DiagnosticDescriptor"/>s associated to the specified analyzer type.</summary>
    /// <typeparam name="T">The type of the <see cref="DiagnosticAnalyzer"/> whose associated <seealso cref="DiagnosticDescriptor"/>s to get.</typeparam>
    /// <returns>An <seealso cref="ImmutableArray{T}"/> containing the <seealso cref="DiagnosticDescriptor"/>s.</returns>
    public ImmutableArray<DiagnosticDescriptor> GetDiagnosticDescriptors<T>()
        where T : DiagnosticAnalyzer
    {
        return GetDiagnosticDescriptors(typeof(T));
    }
    /// <summary>Gets the <see cref="DiagnosticDescriptor"/>s associated to the specified analyzer type.</summary>
    /// <param name="diagnosticAnalyzerType">The type of the <see cref="DiagnosticAnalyzer"/> whose associated <seealso cref="DiagnosticDescriptor"/>s to get.</param>
    /// <returns>An <seealso cref="ImmutableArray{T}"/> containing the <seealso cref="DiagnosticDescriptor"/>s.</returns>
    public ImmutableArray<DiagnosticDescriptor> GetDiagnosticDescriptors(Type diagnosticAnalyzerType)
    {
        analyzerGroupedDiagnostics.TryGetValue(diagnosticAnalyzerType, out var descriptors);
        return descriptors;
    }

    /// <inheritdoc cref="GetDiagnosticDescriptor(int)"/>
    public DiagnosticDescriptor? this[int ruleID] => GetDiagnosticDescriptor(ruleID);
    /// <inheritdoc cref="GetDiagnosticDescriptor(string)"/>
    public DiagnosticDescriptor? this[string ruleID] => GetDiagnosticDescriptor(ruleID);

    private static Dictionary<Type, ImmutableArray<DiagnosticDescriptor>> DiscoverDiagnosticDescriptors(IEnumerable<Type> diagnosticAnalyzerTypes)
    {
        var groupedDiagnostics = new Dictionary<Type, ImmutableArray<DiagnosticDescriptor>>();
        var seenRuleIDs = new HashSet<string>(StringComparer.Ordinal);
        var seenNumericRuleIDs = new HashSet<int>();

        foreach (var analyzerType in diagnosticAnalyzerTypes.Distinct())
        {
            if (!typeof(DiagnosticAnalyzer).IsAssignableFrom(analyzerType))
                throw new ArgumentException($"The provided type '{analyzerType}' is not a {nameof(DiagnosticAnalyzer)}.", nameof(diagnosticAnalyzerTypes));

            if (Activator.CreateInstance(analyzerType) is not DiagnosticAnalyzer analyzer)
                throw new ArgumentException($"The provided analyzer type '{analyzerType}' could not be instantiated.", nameof(diagnosticAnalyzerTypes));

            var diagnostics = analyzer.SupportedDiagnostics;
            groupedDiagnostics.Add(analyzerType, diagnostics);

            foreach (var descriptor in diagnostics)
            {
                if (!seenRuleIDs.Add(descriptor.Id))
                    throw new ArgumentException($"The diagnostic ID '{descriptor.Id}' is declared more than once.", nameof(diagnosticAnalyzerTypes));

                if (TryGetNumericDiagnosticID(descriptor.Id, out var numericRuleID) && !seenNumericRuleIDs.Add(numericRuleID))
                    throw new ArgumentException($"The numeric diagnostic ID '{numericRuleID:D4}' is declared more than once.", nameof(diagnosticAnalyzerTypes));
            }
        }

        return groupedDiagnostics;
    }

    private static bool TryGetNumericDiagnosticID(string diagnosticID, out int ruleID)
    {
        if (diagnosticID.Length < DiagnosticIDDigits)
        {
            ruleID = default;
            return false;
        }

        return int.TryParse(diagnosticID.Substring(diagnosticID.Length - DiagnosticIDDigits), out ruleID);
    }
}
