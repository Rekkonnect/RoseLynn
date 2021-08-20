using System;

namespace RoseLynn.Utilities
{
    /// <summary>Decorates an attribute as an enum value associativity attribute.</summary>
    /// <typeparam name="T">The type of the enum whose values the decorated attribute associates.</typeparam>
    public interface IAssociatedEnumValueAttribute<T>
        where T : struct, Enum
    {
        /// <summary>Gets the associated enum value.</summary>
        T AssociatedValue { get; }
    }
}
