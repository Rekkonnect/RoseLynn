using Garyon.Extensions;
using Garyon.Functions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using RoseLynn.CSharp.Syntax;

namespace RoseLynn;

public static class GlobalSyntaxFeaturesHelpers
{
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
