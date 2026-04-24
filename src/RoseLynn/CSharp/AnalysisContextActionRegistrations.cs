using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;

namespace RoseLynn.CSharp;

#nullable enable

/// <summary>Provides registration extensions for <seealso cref="AnalysisContext"/> instances.</summary>
public static class AnalysisContextActionRegistrations
{
    /// <summary>Registers a syntax node action targeting a specified attribute, performing the specified action if the syntax node represents an attribute placement with the specified name.</summary>
    /// <param name="context">The <seealso cref="AnalysisContext"/> on which to register the action.</param>
    /// <param name="action">The action to register on the context. It will only be executed on attributes matching the desired name.</param>
    /// <param name="attributeName">The name of the attribute that will trigger the registered action.</param>
    public static void RegisterTargetAttributeSyntaxNodeAction(this AnalysisContext context, Action<SyntaxNodeAnalysisContext> action, string attributeName)
    {
        RegisterTargetAttributeSyntaxNodeAction(context, action, new FullSymbolName(attributeName), SymbolNameMatchingLevel.SymbolName);
    }
    /// <summary>Registers a syntax node action targeting a specified attribute, performing the specified action if the syntax node represents an attribute placement with the specified name.</summary>
    /// <param name="context">The <seealso cref="AnalysisContext"/> on which to register the action.</param>
    /// <param name="action">The action to register on the context. It will only be executed on attributes matching the desired name.</param>
    /// <param name="attributeName">The name of the attribute that will trigger the registered action.</param>
    /// <param name="nameMatchingLevel">The level of comparison to perform when attempting to match the attribute names.</param>
    public static void RegisterTargetAttributeSyntaxNodeAction(this AnalysisContext context, Action<SyntaxNodeAnalysisContext> action, FullSymbolName attributeName, SymbolNameMatchingLevel nameMatchingLevel = SymbolNameMatchingLevel.SymbolName)
    {
        context.RegisterSyntaxNodeAction(Boilerplate, SyntaxKind.Attribute);

        void Boilerplate(SyntaxNodeAnalysisContext context)
        {
            var attributeNode = (context.Node as AttributeSyntax)!;

            var sourceAttributeName = context.SemanticModel.GetAttributeTypeSymbol(attributeNode).GetFullSymbolName()!;
            if (!sourceAttributeName.Matches(attributeName, nameMatchingLevel))
                return;

            action(context);
        }
    }
}
