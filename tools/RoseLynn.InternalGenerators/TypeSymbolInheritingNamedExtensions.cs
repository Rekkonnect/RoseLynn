using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;

namespace RoseLynn.InternalGenerators;

public static class TypeSymbolInheritingNamedExtensions
{
    public static bool Inherits(this ITypeSymbol symbol, FullSymbolName inheritedSymbolName)
    {
        return symbol.GetAllBaseTypesAndInterfaces().Any(b => b.GetFullSymbolName().Matches(inheritedSymbolName, SymbolNameMatchingLevel.Namespace));
    }
    public static IEnumerable<AttributeData> AttributesInheriting(this ISymbol symbol, FullSymbolName inheritedAttributeName)
    {
        return symbol.GetAttributes().Where(a => a.AttributeClass.Inherits(inheritedAttributeName));
    }
}
