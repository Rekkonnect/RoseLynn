using Microsoft.CodeAnalysis;

namespace RoseLynn
{
    public static class TypeKindExtensions
    {
        public static bool CanExplicitlyInheritTypes(this TypeKind kind)
        {
            switch (kind)
            {
                case TypeKind.Delegate:
                case TypeKind.Enum:
                    return false;
            }
            return true;
        }
    }
}
