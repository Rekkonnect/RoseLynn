using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;

namespace RoseLynn.CSharp.Syntax;

/// <summary>Provides more useful functions for generating syntax nodes.</summary>
public static class ExtendedSyntaxFactory
{
    /// <summary>Generates a new <seealso cref="AttributeSyntax"/> for the given type.</summary>
    /// <typeparam name="T">The attribute type which the <seealso cref="AttributeSyntax"/> will refer to.</typeparam>
    /// <param name="assumeImported">If <see langword="true"/>, type's short name will be used as the identifier, assuming that it will be properly bound to one of the present usings. Otherwise, the type's full name (including namespaces) is used as the identifier.</param>
    /// <returns>The resulting <seealso cref="AttributeSyntax"/>.</returns>
    public static AttributeSyntax Attribute<T>(bool assumeImported = true)
        where T : Attribute
    {
        return Attribute<T>(null, assumeImported);
    }
    /// <summary>Generates a new <seealso cref="AttributeSyntax"/> for the given type.</summary>
    /// <param name="attributeType">The attribute type which the <seealso cref="AttributeSyntax"/> will refer to.</param>
    /// <param name="assumeImported">If <see langword="true"/>, type's short name will be used as the identifier, assuming that it will be properly bound to one of the present usings. Otherwise, the type's full name (including namespaces) is used as the identifier.</param>
    /// <returns>The resulting <seealso cref="AttributeSyntax"/>.</returns>
    public static AttributeSyntax Attribute(Type attributeType, bool assumeImported = true)
    {
        return Attribute(attributeType, null, assumeImported);
    }
    /// <summary>Generates a new <seealso cref="AttributeSyntax"/> for the given type and with the specified <seealso cref="AttributeArgumentListSyntax"/>.</summary>
    /// <typeparam name="T">The attribute type which the <seealso cref="AttributeSyntax"/> will refer to.</typeparam>
    /// <param name="attributeArgumentList">The <seealso cref="AttributeArgumentListSyntax"/> for the attribute.</param>
    /// <param name="assumeImported">If <see langword="true"/>, type's short name will be used as the identifier, assuming that it will be properly bound to one of the present usings. Otherwise, the type's full name (including namespaces) is used as the identifier.</param>
    /// <returns>The resulting <seealso cref="AttributeSyntax"/>.</returns>
    public static AttributeSyntax Attribute<T>(AttributeArgumentListSyntax attributeArgumentList, bool assumeImported = true)
        where T : Attribute
    {
        return Attribute(typeof(T), attributeArgumentList, assumeImported);
    }
    /// <summary>Generates a new <seealso cref="AttributeSyntax"/> for the given type and with the specified <seealso cref="AttributeArgumentListSyntax"/>.</summary>
    /// <param name="attributeType">The attribute type which the <seealso cref="AttributeSyntax"/> will refer to.</param>
    /// <param name="attributeArgumentList">The <seealso cref="AttributeArgumentListSyntax"/> for the attribute.</param>
    /// <param name="assumeImported">If <see langword="true"/>, type's short name will be used as the identifier, assuming that it will be properly bound to one of the present usings. Otherwise, the type's full name (including namespaces) is used as the identifier.</param>
    /// <returns>The resulting <seealso cref="AttributeSyntax"/>.</returns>
    public static AttributeSyntax Attribute(Type attributeType, AttributeArgumentListSyntax attributeArgumentList, bool assumeImported = true)
    {
        var attributeName = SimplifyAttributeNameUsage(assumeImported ? attributeType.Name : attributeType.FullName);
        return SyntaxFactory.Attribute(SyntaxFactory.ParseName(attributeName), attributeArgumentList);
    }

    /// <summary>Generates a new <seealso cref="AttributeListSyntax"/> from a single <seealso cref="AttributeSyntax"/>.</summary>
    /// <param name="attributeNode">The single <seealso cref="AttributeSyntax"/> in the <seealso cref="AttributeListSyntax"/>.</param>
    /// <returns>The resulting <seealso cref="AttributeListSyntax"/>.</returns>
    public static AttributeListSyntax AttributeList(AttributeSyntax attributeNode)
    {
        return AttributeList(null, attributeNode);
    }
    /// <summary>Generates a new <seealso cref="AttributeListSyntax"/> from a single <seealso cref="AttributeSyntax"/> with the specified target specifier.</summary>
    /// <param name="attributeTargetSpecifierNode">The <seealso cref="AttributeTargetSpecifierSyntax"/>.</param>
    /// <param name="attributeNode">The single <seealso cref="AttributeSyntax"/> in the <seealso cref="AttributeListSyntax"/>.</param>
    /// <returns>The resulting <seealso cref="AttributeListSyntax"/>.</returns>
    public static AttributeListSyntax AttributeList(AttributeTargetSpecifierSyntax attributeTargetSpecifierNode, AttributeSyntax attributeNode)
    {
        return SyntaxFactory.AttributeList(attributeTargetSpecifierNode, SyntaxFactory.SeparatedList(new[] { attributeNode }));
    }

    /// <summary>Removes the suffix "Attribute" from the attribute type name to be used as an identifier in an <seealso cref="AttributeSyntax"/>.</summary>
    /// <param name="attributeTypeName">The name of the attribute that may also include the suffix. The instance is replaced with the simplified version.</param>
    public static void SimplifyAttributeNameUsage(ref string attributeTypeName)
    {
        attributeTypeName = SimplifyAttributeNameUsage(attributeTypeName);
    }
    /// <summary>Removes the suffix "Attribute" from the attribute type name to be used as an identifier in an <seealso cref="AttributeSyntax"/>.</summary>
    /// <param name="attributeTypeName">The name of the attribute that may also include the suffix.</param>
    /// <returns>The simplified version of the attribute, with the suffix removed, if present.</returns>
    public static string SimplifyAttributeNameUsage(string attributeTypeName)
    {
        // System.Attribute is preffered from the collision since it's a fully fixed name
        const string attributeSuffix = nameof(System.Attribute);

        if (attributeTypeName.EndsWith(attributeSuffix))
            return attributeTypeName.Remove(attributeTypeName.Length - attributeSuffix.Length);

        return attributeTypeName;
    }

    /// <summary>Creates a new <seealso cref="TypeArgumentListSyntax"/>.</summary>
    /// <param name="nodes">The nodes the <seealso cref="TypeArgumentListSyntax"/> will contain.</param>
    /// <returns>A <seealso cref="TypeArgumentListSyntax"/> containing the given <seealso cref="TypeSyntax"/> nodes in the same order.</returns>
    public static TypeArgumentListSyntax TypeArgumentList(params TypeSyntax[] nodes)
    {
        return SyntaxFactory.TypeArgumentList(SeparatedList(nodes));
    }

    /// <summary>Creates a new <seealso cref="SeparatedSyntaxList{TNode}"/>.</summary>
    /// <param name="nodes">The nodes the <seealso cref="SeparatedSyntaxList{TNode}"/> will contain.</param>
    /// <returns>A <seealso cref="SeparatedSyntaxList{TNode}"/> containing the given <typeparamref name="TNode"/> nodes in the same order.</returns>
    public static SeparatedSyntaxList<TNode> SeparatedList<TNode>(params TNode[] nodes)
        where TNode : SyntaxNode
    {
        return SyntaxFactory.SeparatedList(nodes);
    }
}
