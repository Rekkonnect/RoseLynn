#nullable enable

using System;
using System.Reflection;

namespace RoseLynn.Utilities
{
    /// <summary>Provides useful extensions for the <seealso cref="MemberInfo"/> type.</summary>
    public static class MemberInfoExtensions
    {
        /// <summary>Gets the value of a field or property <seealso cref="MemberInfo"/>.</summary>
        /// <param name="memberInfo">The <seealso cref="MemberInfo"/> whose value to get based on the given instance. It should be either a <see cref="FieldInfo"/> or a <seealso cref="PropertyInfo"/>.</param>
        /// <param name="instance">The instance on which to get the value of the field or property, or <see langword="null"/> if it's a <see langword="static"/> one.</param>
        /// <returns>The value returned by the field or property on the given instance.</returns>
        /// <exception cref="InvalidOperationException">The given <seealso cref="MemberInfo"/> is not a <seealso cref="FieldInfo"/> or a <seealso cref="PropertyInfo"/>.</exception>
        public static object GetFieldOrPropertyValue(this MemberInfo memberInfo, object? instance)
        {
            return memberInfo switch
            {
                FieldInfo field       => field.GetValue(instance),
                PropertyInfo property => property.GetValue(instance),
                _ => throw new InvalidOperationException("The member is not a field or a property."),
            };
        }
    }
}
