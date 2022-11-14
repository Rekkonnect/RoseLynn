using System;

namespace RoseLynn;

/// <summary>Represents the kind of a symbol that can have an identifier.</summary>
[Flags]
public enum IdentifiableSymbolKind : uint
{
    /// <summary>Represents no symbol kind.</summary>
    None = 0,

    /// <summary>Represents the namespace symbol kind.</summary>
    Namespace = 1,

    // Types
    /// <summary>Represents the class symbol kind.</summary>
    Class = 1 << 1,
    /// <summary>Represents the struct symbol kind.</summary>
    Struct = 1 << 2,
    /// <summary>Represents the interface symbol kind.</summary>
    Interface = 1 << 3,
    /// <summary>Represents the delegate symbol kind.</summary>
    Delegate = 1 << 4,
    /// <summary>Represents the enum symbol kind.</summary>
    Enum = 1 << 5,
    /// <summary>Represents the record symbol kind. This flag is paired with <seealso cref="Class"/> or <seealso cref="Struct"/>.</summary>
    Record = 1 << 6,

    /// <summary>Represents the record class symbol kind.</summary>
    RecordClass = Record | Class,
    /// <summary>Represents the record struct symbol kind.</summary>
    RecordStruct = Record | Struct,
    /// <summary>Represents the combination of <seealso cref="RecordClass"/> and <seealso cref="RecordStruct"/>.</summary>
    AnyRecord = RecordClass | RecordStruct,

    /// <summary>The mask for the class, struct, interface, delegate, enum and record symbol kinds.</summary>
    TypeKindMask = AnyRecord | Interface | Delegate | Enum,

    // Parameters
    /// <summary>Represents the parameter symbol kind.</summary>
    Parameter = 1 << 10,
    /// <summary>Represents the generic type parameter symbol kind.</summary>
    GenericParameter = 1 << 11,

    /// <summary>The mask for the parameter and generic type parameter symbol kinds.</summary>
    ParameterKindMask = Parameter | GenericParameter,

    // Members
    /// <summary>Represents the field symbol kind.</summary>
    Field = 1 << 16,
    /// <summary>Represents the property symbol kind.</summary>
    Property = 1 << 17,
    /// <summary>Represents the event symbol kind.</summary>
    Event = 1 << 18,
    /// <summary>Represents the method symbol kind.</summary>
    Method = 1 << 19,

    /// <summary>The mask for the field, property, event and method symbol kinds.</summary>
    MemberMask = Field | Property | Event | Method,

    // Flags
    /// <summary>Represents the alias symbol kind. This flag indicates that the symbol kind appeared with an alias. The flag is paired with the actual symbol kind.</summary>
    Alias = 1U << 31,

    /// <summary>The mask for the alias flag.</summary>
    FlagMask = Alias,

    // Reserve all flags that might exist in the future
    /// <summary>The mask for all symbol kinds.</summary>
    All = ~None,
}
