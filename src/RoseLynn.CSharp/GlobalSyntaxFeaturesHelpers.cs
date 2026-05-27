using Garyon.Extensions;
using Garyon.Functions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using RoseLynn;
using RoseLynn.CSharp.Syntax;

namespace RoseLynn.CSharp;

/// <summary>Provides helpers for detecting top-level C# compilation-unit features such as global usings, assembly attributes, and top-level statements.</summary>
public static class GlobalSyntaxFeaturesHelpers
{
    /// <summary>Gets the <seealso cref="GlobalSyntaxFeatures"/> flags present in the given syntax node, dispatching on the node's language.</summary>
    /// <param name="node">The root syntax node to inspect.</param>
    public static GlobalSyntaxFeatures GetGlobalSyntaxFeatures(this SyntaxNode node)
    {
        return node switch
        {
            Microsoft.CodeAnalysis.CSharp.Syntax.CompilationUnitSyntax csharp => csharp.GetGlobalSyntaxFeatures(),
            // TODO: Introduce this helper when RoseLynn becomes cross-language
            // Microsoft.CodeAnalysis.VisualBasic.Syntax.CompilationUnitSyntax csharp => csharp.GetGlobalSyntaxFeatures(),
            _ => GlobalSyntaxFeatures.None,
        };
    }

    /// <summary>Gets the <seealso cref="GlobalSyntaxFeatures"/> flags present in the given C# compilation unit.</summary>
    /// <param name="compilationUnit">The C# compilation unit syntax to inspect.</param>
    public static GlobalSyntaxFeatures GetGlobalSyntaxFeatures(
        this Microsoft.CodeAnalysis.CSharp.Syntax.CompilationUnitSyntax compilationUnit)
    {
        var features = GlobalSyntaxFeatures.None;

        if (compilationUnit.Usings
            .Any(s => s.GlobalKeyword != default))
        {
            features |= GlobalSyntaxFeatures.GlobalUsings;
        }

        if (compilationUnit.AttributeLists
            .Any(s => s.HasTarget(AttributeListTarget.Assembly)))
        {
            features |= GlobalSyntaxFeatures.AssemblyAttributes;
        }

        if (compilationUnit.AttributeLists
            .Any(s => s.HasTarget(AttributeListTarget.Module)))
        {
            features |= GlobalSyntaxFeatures.ModuleAttributes;
        }

        if (compilationUnit.Members.FirstOrDefault() is Microsoft.CodeAnalysis.CSharp.Syntax.GlobalStatementSyntax)
        {
            features |= GlobalSyntaxFeatures.FileProgram;
        }

        return features;
    }
}
