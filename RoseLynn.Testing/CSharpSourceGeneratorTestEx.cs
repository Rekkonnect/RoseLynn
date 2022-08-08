using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace RoseLynn.Generators.Testing;

/// <summary>
/// Provides additional abstractions over commonly-encountered boilerplate code from
/// <seealso cref="CSharpSourceGeneratorTest{TSourceGenerator, TVerifier}"/>.
/// </summary>
/// <typeparam name="TSourceGenerator">The type of the source generator.</typeparam>
/// <typeparam name="TVerifier">The type of the verifier.</typeparam>
public abstract class CSharpSourceGeneratorTestEx<TSourceGenerator, TVerifier> : CSharpSourceGeneratorTest<TSourceGenerator, TVerifier>
    where TSourceGenerator : ISourceGenerator, new()
    where TVerifier : IVerifier, new()
{
    /// <summary>
    /// Gets the default references that are to be added to the <see cref="AnalyzerTest{TVerifier}.TestState"/>.
    /// </summary>
    public virtual IEnumerable<MetadataReference> AdditionalReferences => Enumerable.Empty<MetadataReference>();

    /// <summary>
    /// Gets the language version to operate upon.
    /// </summary>
    public LanguageVersion LanguageVersion { get; set; } = LanguageVersion.LatestMajor;
    
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

    public CSharpSourceGeneratorTestEx()
    {
        TestState.AdditionalReferences.AddRange(AdditionalReferences);
    }
    
    protected override ParseOptions CreateParseOptions()
    {
        return (base.CreateParseOptions() as CSharpParseOptions).WithLanguageVersion(LanguageVersion);
    }

    protected override CompilationOptions CreateCompilationOptions()
    {
        var compilationOptions = base.CreateCompilationOptions();
        return compilationOptions.WithSpecificDiagnosticOptions(
             compilationOptions.SpecificDiagnosticOptions.SetItems(GetNullableWarningsFromCompiler()));
    }

    private static ImmutableDictionary<string, ReportDiagnostic> GetNullableWarningsFromCompiler()
    {
        string[] args = { "/warnaserror:nullable" };
        var commandLineArguments = CSharpCommandLineParser.Default.Parse(args, baseDirectory: Environment.CurrentDirectory, sdkDirectory: Environment.CurrentDirectory);
        var nullableWarnings = commandLineArguments.CompilationOptions.SpecificDiagnosticOptions;

        return nullableWarnings;
    }
}
