using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace RoseLynn.CSharp.Syntax;

/// <summary>Contains helper methods involving representations of <seealso cref="ISymbol"/> instances.</summary>
public static partial class ISymbolRepresentationExtensions
{
    /// <summary>Gets the full representation of the type symbol.</summary>
    /// <param name="typeSymbol">The type symbol whose representation to get.</param>
    /// <returns>The C# representation of the provided type.</returns>
    /// <remarks>
    /// This supports the following types:
    /// <list type="bullet">
    /// <item><seealso cref="ITypeParameterSymbol"/></item>
    /// <item><seealso cref="INamedTypeSymbol"/></item>
    /// <item><seealso cref="IArrayTypeSymbol"/></item>
    /// <item><seealso cref="IPointerTypeSymbol"/></item>
    /// <item><seealso cref="IFunctionPointerTypeSymbol"/></item>
    /// <item><seealso cref="IDynamicTypeSymbol"/></item>
    /// </list>
    /// </remarks>
    public static string GetRepresentationForType(this ITypeSymbol typeSymbol)
    {
        switch (typeSymbol)
        {
            case ITypeParameterSymbol typeParameter:
                return typeParameter.Name;

            case INamedTypeSymbol named:
                return GetRepresentationForType(named);

            case IArrayTypeSymbol array:
                return GetRepresentationForType(array);

            case IPointerTypeSymbol pointer:
                return GetRepresentationForType(pointer);

            case IFunctionPointerTypeSymbol functionPointer:
                return GetRepresentationForType(functionPointer);

            case IDynamicTypeSymbol:
                return "dynamic";
        }

        return null;
    }
    public static string GetRepresentationForType(this INamedTypeSymbol typeSymbol)
    {
        if (typeSymbol.IsGenericType)
        {
            return GetClosedGenericName(typeSymbol.Name, typeSymbol.TypeArguments);
        }

        return typeSymbol.GetKeywordIdentifierForPredefinedType()
            ?? typeSymbol.Name;
    }
    public static string GetRepresentationForType(this IArrayTypeSymbol arraySymbol)
    {
        int rank = arraySymbol.Rank;
        string commas = new(',', rank - 1);
        var elementIdentifier = GetRepresentationForType(arraySymbol.ElementType);
        return $"{elementIdentifier}[{commas}]";
    }
    public static string GetRepresentationForType(this IPointerTypeSymbol pointerSymbol)
    {
        var elementIdentifier = GetRepresentationForType(pointerSymbol.PointedAtType);
        return $"{elementIdentifier}*";
    }
    public static string GetRepresentationForType(this IFunctionPointerTypeSymbol functionPointerSymbol)
    {
        var signatureTypes = new List<ITypeSymbol>();

        var parameterTypes = functionPointerSymbol.GetParameterTypes();
        signatureTypes.AddRange(parameterTypes);

        signatureTypes.Add(functionPointerSymbol.Signature.ReturnType);

        return GetClosedGenericName("delegate*", signatureTypes);
    }
}
