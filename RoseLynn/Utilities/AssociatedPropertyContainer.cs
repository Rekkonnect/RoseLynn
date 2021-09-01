using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RoseLynn.Utilities
{
    /// <summary>A container that stores references to properties associated to specified enum values.</summary>
    public class AssociatedPropertyContainer
    {
        private readonly Dictionary<PropertyInfo, IEnumerable<Attribute>> propertyDictionary = new();
        private readonly EnumAssociativityDictionary associativityDictionary;

        /// <summary>Initializes a new instance of the <seealso cref="AssociatedPropertyContainer"/> for the given type.</summary>
        /// <param name="type">The type whose associatable properties to identify.</param>
        public AssociatedPropertyContainer(Type type)
        {
            var properties = type.GetProperties();
            foreach (var p in properties)
            {
                var attributes = p.GetCustomAttributes().Where(a => a.GetType().GetInterfaces().Any(IsAssociatedEnumValueAttribute)).ToArray();
                if (attributes.Any())
                    propertyDictionary.Add(p, attributes);
            }

            associativityDictionary = new EnumAssociativityDictionary(propertyDictionary);
        }

        /// <summary>Gets the associated property for the given enum value.</summary>
        /// <param name="enumValue">The enum value whose associated property to get.</param>
        /// <returns>The <seealso cref="PropertyInfo"/> for the property associated to <paramref name="enumValue"/>.</returns>
        public PropertyInfo GetAssociatedProperty(Enum enumValue) => associativityDictionary[enumValue];

        private static bool IsAssociatedEnumValueAttribute(Type type) => type.GetGenericTypeDefinitionOrSame() == typeof(IAssociatedEnumValueAttribute<>);

        private class EnumAssociativityDictionary
        {
            private const string associatedValuePropertyName = nameof(IAssociatedEnumValueAttribute<GenericParameterAttributes>.AssociatedValue);

            private readonly Dictionary<Type, SpecificEnumTypePropertyDictionary> associated = new();

            public EnumAssociativityDictionary() { }
            public EnumAssociativityDictionary(IDictionary<PropertyInfo, IEnumerable<Attribute>> associatedProperties)
            {
                foreach (var associatedProperty in associatedProperties)
                    foreach (var associatedValue in associatedProperty.Value)
                        Add(associatedValue.GetType().GetProperty(associatedValuePropertyName).GetValue(associatedValue) as Enum, associatedProperty.Key);
            }

            public bool Add(Enum associatedValue, PropertyInfo property)
            {
                var enumType = associatedValue.GetType();
                // I hate this pattern
                if (!associated.TryGetValue(enumType, out var specificDictionary))
                    associated.Add(enumType, specificDictionary = new SpecificEnumTypePropertyDictionary());

                return specificDictionary.Add(associatedValue, property);
            }

            public PropertyInfo this[Enum enumValue]
            {
                get
                {
                    associated.TryGetValue(enumValue.GetType(), out var specificDictionary);
                    return specificDictionary?[enumValue];
                }
            }

            private class SpecificEnumTypePropertyDictionary
            {
                private readonly Dictionary<Enum, PropertyInfo> associated = new();

                public bool Add(Enum associatedValue, PropertyInfo property)
                {
                    return associated.TryAddPreserve(associatedValue, property);
                }

                public PropertyInfo this[Enum value]
                {
                    get
                    {
                        associated.TryGetValue(value, out var propertyInfo);
                        return propertyInfo;
                    }
                }
            }
        }
    }
}
