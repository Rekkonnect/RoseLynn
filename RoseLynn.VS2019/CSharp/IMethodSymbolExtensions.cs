using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoseLynn.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace RoseLynn.CSharp;

#nullable enable

/// <summary>
/// Provides extensions for the <seealso cref="IMethodSymbol"/> class specifically applying for C#.
/// </summary>
public static class IMethodSymbolExtensions
{
    /// <summary>
    /// Gets the first declaring syntax of the method that contains a body.
    /// </summary>
    /// <param name="method">The method whose declaring syntax to get.</param>
    /// <returns>
    /// The declaring syntax for the provided method that has a body, if such exists.
    /// If the method does not have a declaring syntax with a body, or if it does not
    /// have any declaring syntax references, <see langword="null"/> is returned.
    /// </returns>
    public static MethodDeclarationSyntax? GetFirstBodyDeclaration(this IMethodSymbol method)
    {
        if (method.DeclaringSyntaxReferences.Length is 0)
            return null;

        var declarationSyntaxes = method.DeclaringSyntaxReferences
                                        .Select(d => d.GetSyntax())
                                        .OfType<MethodDeclarationSyntax>();

        return declarationSyntaxes.FirstOrDefault(d => d.Body is not null);
    }

    /// <summary>
    /// Gets all the defined locals inside a method. If the symbol does not come with a
    /// defined syntax declaration.
    /// </summary>
    /// <param name="method">
    /// The method whose declared methods to get. The method must have a syntax declaration
    /// that comes with a body.
    /// </param>
    /// <param name="compilation">
    /// The <seealso cref="Compilation"/> where the method was declared at. If the method is
    /// not part of the current compilation, this will not yield any results.
    /// </param>
    /// <returns>
    /// The locals that the method defines, if there exists one available syntax declaration
    /// with a body for the method, otherwise <see langword="null"/>. This means that
    /// <see langword="abstract"/>, interface, <see langword="extern"/> methods, signatures
    /// for implementing delegate and function pointer types, or methods loaded from metadata
    /// (that do not have a bound syntax declaration) will always return <see langword="null"/>.
    /// </returns>
    public static IEnumerable<ILocalSymbol>? GetAllDefinedLocals(this IMethodSymbol method, Compilation compilation)
    {
        var declaration = method.GetFirstBodyDeclaration();
        if (declaration is null)
            return null;

        var semanticModel = compilation.GetSemanticModel(declaration.SyntaxTree, true);

        var body = declaration.Body!;
        var variableDeclarations = body.ChildNodes().OfType<VariableDeclarationSyntax>();
        return variableDeclarations.SelectMany(v => v.GetDeclaredVariables(semanticModel));
    }
}
