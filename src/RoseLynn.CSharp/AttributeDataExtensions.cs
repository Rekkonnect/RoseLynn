
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoseLynn.CSharp.Syntax;

namespace RoseLynn.CSharp;

/// <summary>Provides useful extensions for the <seealso cref="AttributeData"/> type used for C# syntax trees.</summary>
public static class AttributeDataExtensions
{
    /// <summary>Gets the <seealso cref="AttributeListTarget"/> type of the <seealso cref="AttributeData"/>.</summary>
    /// <param name="attribute">The <seealso cref="AttributeData"/> whose <seealso cref="AttributeListTarget"/> to identify.</param>
    /// <returns>The <seealso cref="AttributeListTarget"/> of <paramref name="attribute"/>.</returns>
    public static AttributeListTarget GetTarget(this AttributeData attribute)
    {
        return attribute.GetAttributeApplicationSyntax().GetTarget();
    }
    /// <summary>Determines whether the <seealso cref="AttributeListTarget"/> type of the <seealso cref="AttributeData"/> is a provided one.</summary>
    /// <param name="attribute">The <seealso cref="AttributeData"/> whose <seealso cref="AttributeListTarget"/> to identify.</param>
    /// <param name="target">The desired <seealso cref="AttributeListTarget"/> to check.</param>
    /// <returns>The <seealso cref="AttributeListTarget"/> of <paramref name="attribute"/>.</returns>
    public static bool HasTarget(this AttributeData attribute, AttributeListTarget target)
    {
        return attribute.GetAttributeApplicationSyntax().HasTarget(target);
    }

    /// <summary>Gets the appropriate <seealso cref="AttributeSyntax"/> node representing the attribute whose information is contained in the provided <seealso cref="AttributeData"/> instance.</summary>
    /// <param name="attribute">The <seealso cref="AttributeData"/> whose representing <seealso cref="AttributeSyntax"/> to get.</param>
    /// <returns>The <seealso cref="AttributeSyntax"/> representing the attribute whose information is in <paramref name="attribute"/>.</returns>
    public static AttributeSyntax? GetAttributeApplicationSyntax(this AttributeData? attribute)
    {
        return attribute?.ApplicationSyntaxReference?.GetSyntax() as AttributeSyntax;
    }
    /// <summary>Gets the appropriate <seealso cref="AttributeSyntax"/> node representing the attribute whose information is contained in the provided <seealso cref="AttributeData"/> instance.</summary>
    /// <param name="attribute">The <seealso cref="AttributeData"/> whose representing <seealso cref="AttributeSyntax"/> to get.</param>
    /// <param name="cancellationToken">The cancellation token that may cancel the operation of getting the source syntax.</param>
    /// <returns>The <seealso cref="AttributeSyntax"/> representing the attribute whose information is in <paramref name="attribute"/>.</returns>
    public static async Task<AttributeSyntax?> GetAttributeApplicationSyntaxAsync(this AttributeData? attribute, CancellationToken cancellationToken = default)
    {
        var task = attribute?.ApplicationSyntaxReference?.GetSyntaxAsync(cancellationToken);
        if (task is null)
            return null;

        return await task as AttributeSyntax;
    }

    /// <summary>
    /// Gets the appropriate <seealso cref="AttributeArgumentListSyntax"/> node representing the argument
    /// list whose arguments are contained in the provided <seealso cref="AttributeData"/> instance.
    /// </summary>
    /// <param name="attribute">The <seealso cref="AttributeData"/> whose representing <seealso cref="AttributeArgumentListSyntax"/> to get.</param>
    /// <returns>The <seealso cref="AttributeArgumentListSyntax"/> representing the arguments contained in the <seealso cref="AttributeData"/>.</returns>
    public static AttributeArgumentListSyntax? GetAttributeListApplicationSyntax(this AttributeData? attribute)
    {
        return GetAttributeApplicationSyntax(attribute)?.ArgumentList;
    }
    /// <summary>
    /// Gets the appropriate <seealso cref="AttributeArgumentListSyntax"/> node representing the argument
    /// list whose arguments are contained in the provided <seealso cref="AttributeData"/> instance.
    /// </summary>
    /// <param name="attribute">The <seealso cref="AttributeData"/> whose representing <seealso cref="AttributeArgumentListSyntax"/> to get.</param>
    /// <param name="cancellationToken">The cancellation token that may cancel the operation of getting the source syntax.</param>
    /// <returns>The <seealso cref="AttributeArgumentListSyntax"/> representing the arguments contained in the <seealso cref="AttributeData"/>.</returns>
    public static async Task<AttributeArgumentListSyntax?> GetAttributeListApplicationSyntaxAsync(this AttributeData? attribute, CancellationToken cancellationToken = default)
    {
        var syntax = await GetAttributeApplicationSyntaxAsync(attribute, cancellationToken);
        return syntax?.ArgumentList;
    }

    /// <summary>Returns <see langword="true"/> if the attribute was applied without any constructor arguments or named arguments.</summary>
    /// <param name="attribute">The attribute data to inspect.</param>
    public static bool HasNoArguments(this AttributeData attribute)
    {
        return attribute.ConstructorArguments is []
            && attribute.NamedArguments is []
            ;
    }

    /// <summary>Asynchronously creates an <seealso cref="AttributeDataViewModel"/> from the attribute data.</summary>
    /// <param name="attribute">The attribute data to project.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    public static async Task<AttributeDataViewModel?> ViewModelAsync(
        this AttributeData attribute,
        CancellationToken cancellationToken)
    {
        return await AttributeDataViewModel.CreateAsync(attribute, cancellationToken);
    }

    /// <summary>Creates an <seealso cref="AttributeDataViewModel"/> from the attribute data.</summary>
    /// <param name="attribute">The attribute data to project.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    public static AttributeDataViewModel? ViewModel(
        this AttributeData attribute,
        CancellationToken cancellationToken)
    {
        return AttributeDataViewModel.Create(attribute, cancellationToken);
    }
}
