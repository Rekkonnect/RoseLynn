using Microsoft.CodeAnalysis;

namespace RoseLynn
{
    public static class TypeKindExtensions
    {
        public static bool CanExplicitlyInheritTypes(this TypeKind kind)
        {
            return kind switch
            {
                TypeKind.Delegate or
                TypeKind.Enum => false,
                _ => true,
            };
        }
    }
}
