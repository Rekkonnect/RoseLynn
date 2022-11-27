using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

#nullable enable annotations

namespace RoseLynn.CSharp.Syntax;

/// <summary>
/// Provides useful extensions for the <seealso cref="RecordDeclarationSyntax"/> node.
/// </summary>
public static class RecordDeclarationSyntaxExtensions
{
    /// <summary>
    /// Gets the declared <seealso cref="TypeKind"/> of a record declaration syntax.
    /// </summary>
    /// <param name="recordDeclarationSyntax">
    /// The <seealso cref="RecordDeclarationSyntax"/> whose declared member's <seealso cref="TypeKind"/> to get.
    /// </param>
    /// <returns>
    /// <seealso cref="TypeKind.Class"/> if it's a record class declaration, <seealso cref="TypeKind.Struct"/>
    /// if it's a record struct declaration, or <seealso cref="TypeKind.Error"/> if the provided instance
    /// is <see langword="null"/>.
    /// </returns>
    public static TypeKind GetRecordTypeKind(this RecordDeclarationSyntax? recordDeclarationSyntax)
    {
        if (recordDeclarationSyntax is null)
            return TypeKind.Error;

        return recordDeclarationSyntax.Kind() switch
        {
            SyntaxKind.RecordStructDeclaration => TypeKind.Struct,
            SyntaxKind.RecordDeclaration => TypeKind.Class,
        };
    }
}
