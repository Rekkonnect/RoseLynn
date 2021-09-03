#nullable enable

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Resources;

namespace RoseLynn.Analyzers
{
    /// <summary>Provides mechanisms to function a storage of <seealso cref="DiagnosticDescriptor"/> instances that are grouped by analyzer type.</summary>
    public abstract class DiagnosticDescriptorStorageBase
    {
        private readonly RuleStorage ruleStorage = new();
        private readonly Dictionary<Type, HashSet<DiagnosticDescriptor>> analyzerGroupedDiagnostics = new();

        /// <summary>Represents the count of digits the diagnostic ID ends in.</summary>
        /// <remarks>The storage is not intended to support diagnostic IDs that do not end in exactly 4 decimal digits. It is advised to follow conventions followed by other parties.</remarks>
        public const int DiagnosticIDDigits = 4;

        /// <summary>Gets the length of a diagnostic ID stored in this storage.</summary>
        public int DiagnosticIDLength => DiagnosticIDPrefix.Length + DiagnosticIDDigits;

        /// <summary>Gets or sets the default <seealso cref="DiagnosticAnalyzer"/> that will be assigned to the created diagnostics. Defaults to <see langword="null"/>.</summary>
        protected Type? DefaultDiagnosticAnalyzer { get; set; }

        /// <summary>Sets the default <seealso cref="DiagnosticAnalyzer"/> that will be assigned to the created diagnostics.</summary>
        /// <typeparam name="T">The type of the analyzer that will be considered the default <seealso cref="DiagnosticAnalyzer"/>.</typeparam>
        protected void SetDefaultDiagnosticAnalyzer<T>() => DefaultDiagnosticAnalyzer = typeof(T);

        /// <summary>Gets the <seealso cref="DiagnosticDescriptor"/> representing the given rule ID.</summary>
        /// <param name="ruleID">The rule ID whose <seealso cref="DiagnosticDescriptor"/> to get. The numeric portion of the rule ID will be combined with <seealso cref="DiagnosticIDPrefix"/>.</param>
        /// <returns>The <seealso cref="DiagnosticDescriptor"/> representing the given rule ID, if it exists, otherwise <see langword="null"/>.</returns>
        public DiagnosticDescriptor? GetDiagnosticDescriptor(int ruleID)
        {
            return ruleStorage[ruleID];
        }
        /// <summary>Gets the <seealso cref="DiagnosticDescriptor"/> representing the given rule ID.</summary>
        /// <param name="ruleID">The rule ID whose <seealso cref="DiagnosticDescriptor"/> to get.</param>
        /// <returns>The <seealso cref="DiagnosticDescriptor"/> representing the given rule ID, if it is a valid rule ID for this storage and it exists, otherwise <see langword="null"/>.</returns>
        public DiagnosticDescriptor? GetDiagnosticDescriptor(string ruleID)
        {
            if (ruleID.Length != DiagnosticIDLength || !ruleID.StartsWith(DiagnosticIDPrefix))
                return null;

            if (!int.TryParse(ruleID.Substring(DiagnosticIDPrefix.Length), out int id))
                return null;

            return GetDiagnosticDescriptor(id);
        }

        /// <summary>Gets all the stored <seealso cref="DiagnosticDescriptor"/>s in this storage mapped to their associated <seealso cref="DiagnosticAnalyzer"/> types.</summary>
        /// <returns>A dictionary mapping the types of <seealso cref="DiagnosticAnalyzer"/>s to their respective <seealso cref="ImmutableArray{T}"/> of associated <seealso cref="DiagnosticDescriptor"/>s.</returns>
        public IDictionary<Type, ImmutableArray<DiagnosticDescriptor>> GetDiagnosticDescriptorsByAnalyzersImmutable()
        {
            return analyzerGroupedDiagnostics.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToImmutableArray());
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
            analyzerGroupedDiagnostics.TryGetValue(diagnosticAnalyzerType, out var set);
            return set.ToImmutableArray();
        }

        /// <inheritdoc cref="GetDiagnosticDescriptor(int)"/>
        public DiagnosticDescriptor? this[int ruleID] => ruleStorage[ruleID];
        /// <inheritdoc cref="GetDiagnosticDescriptor(string)"/>
        public DiagnosticDescriptor? this[string ruleID] => GetDiagnosticDescriptor(ruleID);

        #region Diagnotsic Descriptor Construction
        /// <summary>Gets the URI for the base rule documentation directory.</summary>
        /// <remarks>The final directory separator does not have to be included.</remarks>
        protected abstract string BaseRuleDocsURI { get; }
        /// <summary>Gets the prefix of the diagnostic IDs of this storage.</summary>
        /// <remarks>Multiple analyzers may emit diagnostics whose diagnostic ID prefix is the same. For different categories of diagnostics, a new storage type must be created.</remarks>
        protected abstract string DiagnosticIDPrefix { get; }

        /// <summary>Gets the <seealso cref="System.Resources.ResourceManager"/> that contains the string resources for the diagnostics that are stored.</summary>
        protected abstract ResourceManager ResourceManager { get; }

        /// <summary>Creates a <seealso cref="DiagnosticDescriptor"/> from a diagnostic ID, a category, and a severity.</summary>
        /// <param name="id">The numeric portion of the diagnostic ID. The numeric value will be expanded to 4 decimal digits.</param>
        /// <param name="category">The category of the diagnostic.</param>
        /// <param name="severity">The severity of the diagnostic.</param>
        /// <param name="diagnosticAnalyzerType">The type of the <seealso cref="DiagnosticAnalyzer"/> that emits the created <seealso cref="DiagnosticDescriptor"/>. If <see langword="null"/>, the currently set <see cref="DefaultDiagnosticAnalyzer"/> will be used.</param>
        /// <returns>The resulting created <seealso cref="DiagnosticDescriptor"/>.</returns>
        protected DiagnosticDescriptor CreateDiagnosticDescriptor(int id, string category, DiagnosticSeverity severity, Type? diagnosticAnalyzerType = null)
        {
            var descriptor = new DiagnosticDescriptor(GetDiagnosticID(id), GetTitle(id), GetMessageFormat(id), category, severity, true, helpLinkUri: GetHelpLinkURI(id), description: GetDescription(id));
            ruleStorage[id] = descriptor;

            diagnosticAnalyzerType ??= DefaultDiagnosticAnalyzer;

            if (diagnosticAnalyzerType is not null)
            {
                if (!analyzerGroupedDiagnostics.TryGetValue(diagnosticAnalyzerType, out var set))
                    analyzerGroupedDiagnostics.Add(diagnosticAnalyzerType, set = new());

                set.Add(descriptor!);
            }

            return descriptor;
        }

        /// <summary>Gets the resource string associated with the specified diagnostic ID and property.</summary>
        /// <param name="id">The diagnostic ID.</param>
        /// <param name="property">The string property associated to the rule.</param>
        /// <returns>The resource string associated with the given values.</returns>
        protected virtual LocalizableString GetResourceString(int id, string property)
        {
            return ResourceManager.GetString($"{GetDiagnosticID(id)}_{property}");
        }

        private string GetHelpLinkURI(int id) => $"{BaseRuleDocsURI}/{GetDiagnosticID(id)}.md";
        private string GetDiagnosticID(int id) => $"{DiagnosticIDPrefix}{id.ToString($"D{DiagnosticIDDigits}")}";

        private LocalizableString GetTitle(int id) => GetResourceString(id, "Title");
        private LocalizableString GetMessageFormat(int id) => GetResourceString(id, "MessageFormat");
        private LocalizableString GetDescription(int id) => GetResourceString(id, "Description");
        #endregion

        private class RuleStorage
        {
            private const int BucketCount = 100;
            private const int BucketLength = 100;

            private readonly DiagnosticDescriptor?[]?[] buckets = new DiagnosticDescriptor[BucketCount][];

            public DiagnosticDescriptor? this[int id]
            {
                get
                {
                    if (!BreakID(id, out int bucketIndex, out int innerIndex))
                        return null;

                    return buckets[bucketIndex]?[innerIndex];
                }
                set
                {
                    if (!BreakID(id, out int bucketIndex, out int innerIndex))
                        return;

                    ref var bucket = ref buckets[bucketIndex];
                    if (bucket is null)
                        bucket = new DiagnosticDescriptor?[BucketLength];

                    bucket[innerIndex] = value;
                }
            }

            private static bool BreakID(int id, out int bucketIndex, out int innerIndex)
            {
                bucketIndex = Math.DivRem(id, BucketLength, out innerIndex);
                return id is >= 0 and < 10000;
            }
        }
    }
}
