#nullable enable


namespace RoseLynn;

/// <summary>Denotes the direction of the conversion operator.</summary>
public enum ConversionOperatorDirection
{
    /// <summary>Denotes that the conversion operator converts an instance of the declaring type into a foreign one.</summary>
    FromDeclaring,
    /// <summary>Denotes that the conversion operator converts an instance of a foreign type into the declaring one.</summary>
    ToDeclaring,

    /// <summary>
    /// Denotes that the operator is not related to the declaring type, and that both
    /// types are foreign to the declaring type.
    /// </summary>
    /// <remarks>
    /// This should be very rarely and under exceptional circumstances encountered.
    /// </remarks>
    Unrelated,
}
