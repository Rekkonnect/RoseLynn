using Microsoft.CodeAnalysis;
using RoseLynn.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RoseLynn;

#nullable enable annotations

/// <summary>Contains extensions for the <seealso cref="INamespaceSymbol"/> interface.</summary>
public static class INamespaceSymbolExtensions
{
    /// <summary>Gets a collection of all the parent namespaces including the provided one.</summary>
    /// <param name="namespaceSymbol">The final <see cref="INamespaceSymbol"/> whose ancestors to get.</param>
    /// <returns>
    /// A collection of all the ancestor namespace symbols, including the provided one in the end.<br/>
    /// For example: given the namespace A.B.C, the result will be a list containing the namespace symbols for A, B and C in that order.
    /// </returns>
    public static IEnumerable<INamespaceSymbol> AncestorNamespacesIncludingThis(this INamespaceSymbol namespaceSymbol)
    {
        return namespaceSymbol.GetContainingNamespaces()
                              .Concat(new SingleElementCollection<INamespaceSymbol>(namespaceSymbol));
    }

    /// <summary>Gets a collection of all the child namespaces, recursing to every child namespace.</summary>
    /// <param name="namespaceSymbol">The root <see cref="INamespaceSymbol"/> whose children to get.</param>
    /// <returns>
    /// A collection of all the ancestor namespace symbols.<br/>
    /// For example: given the namespace A, with the presence of the namespaces A.B.B1, A.C.D.E, A.F,
    /// the result will be a list containing the namespace symbols for A.B, A.B.B1, A.C, A.C.D, A.C.D.E, etc. in that order.
    /// </returns>
    public static IEnumerable<INamespaceSymbol> DeepChildNamespaces(this INamespaceSymbol namespaceSymbol)
    {
        return DeepChildNamespacesIncludingThis(namespaceSymbol).Skip(1);
    }
    /// <summary>Gets a collection of all the child namespaces, recursing to every child namespace, including the provided one.</summary>
    /// <param name="namespaceSymbol">The root <see cref="INamespaceSymbol"/> whose children to get.</param>
    /// <returns>
    /// A collection of all the ancestor namespace symbols, including the provided one in the end.<br/>
    /// For example: given the namespace A, with the presence of the namespaces A.B.B1, A.C.D.E, A.F,
    /// the result will be a list containing the namespace symbols for A, A.B, A.B.B1, A.C, A.C.D, A.C.D.E, etc. in that order.
    /// </returns>
    public static IEnumerable<INamespaceSymbol> DeepChildNamespacesIncludingThis(this INamespaceSymbol namespaceSymbol)
    {
        var result = new List<INamespaceSymbol>
        {
            namespaceSymbol
        };

        for (int i = 0; i < result.Count; i++)
        {
            var current = result[i];
            var children = current.GetNamespaceMembers();
            result.AddRange(children);
        }

        return result;
    }

    /// <summary>Gets all the contained <seealso cref="INamedTypeSymbol"/> members in the given <seealso cref="INamespaceSymbol"/> and its nested namespaces.</summary>
    /// <param name="namespaceSymbol">The <seealso cref="INamespaceSymbol"/> whose types to get.</param>
    /// <returns>All the contained types in the <seealso cref="INamespaceSymbol"/>.</returns>
    /// <remarks>This applies recursively to all nested namespaces.</remarks>
    public static IEnumerable<INamedTypeSymbol> GetAllContainedTypes(this INamespaceSymbol namespaceSymbol)
    {
        var types = namespaceSymbol.GetTypeMembers();
        var namespaces = namespaceSymbol.GetNamespaceMembers();
        return types.Concat(namespaces.SelectMany(GetAllContainedTypes));
    }

    /// <inheritdoc cref="GetQualifiedMember(INamespaceSymbol, Span{string})"/>
    public static ISymbol? GetQualifiedMember(this INamespaceSymbol namespaceSymbol, params string[] qualifiers)
    {
        return GetQualifiedMember(namespaceSymbol, qualifiers.AsSpan());
    }

    /// <summary>
    /// Gets a member accessed by its qualifiers from a higher level.
    /// </summary>
    /// <param name="namespaceSymbol">The namespace symbol whose members to get.</param>
    /// <param name="qualifiers">The qualifiers directing the member to get.</param>
    /// <returns>
    /// The member accessed via the given qualifiers.<br/>
    /// For example, if a namespace A.B.C contains a type D with a <see langword="string"/> property
    /// named String1, the qualifiers { "A", "B", "C", "D", "String1", "Length" } with the initial
    /// namespace symbol being the compilation's global namespace, will return the
    /// <seealso cref="string.Length"/> property of the <see langword="string"/> type.
    /// </returns>
    public static ISymbol? GetQualifiedMember(this INamespaceSymbol namespaceSymbol, Span<string> qualifiers)
    {
        var currentNamespace = namespaceSymbol;
        for (int i = 0; i < qualifiers.Length; i++)
        {
            var qualifier = qualifiers[i];
            var members = currentNamespace.GetMembers();
            var targetMember = members.FirstOrDefault(m => m.Name == qualifier);
            switch (targetMember)
            {
                case null:
                    return null;

                case ITypeSymbol targetType:
                    if (i == qualifier.Length - 1)
                    {
                        return targetType;
                    }

                    var nextQualifiers = qualifiers.Slice(i + 1);
                    return targetType.GetQualifiedMember(nextQualifiers);

                case INamespaceSymbol targetNamespace:
                    currentNamespace = targetNamespace;
                    break;
            }
        }

        return currentNamespace;
    }
}
