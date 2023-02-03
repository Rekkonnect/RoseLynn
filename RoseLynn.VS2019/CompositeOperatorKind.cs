#nullable enable


namespace RoseLynn;

/// <summary>
/// Combines the infromation of the operator kind and the checking mode for which it is defined.
/// </summary>
/// <param name="CheckingMode">The checking mode for which the operator is defined.</param>
/// <param name="Kind">The kind of the operator.</param>
public record struct CompositeOperatorKind(OperatorCheckingMode CheckingMode, OperatorKind Kind)
{
    /// <summary>
    /// Creates two <seealso cref="CompositeOperatorKind"/> instances, one for
    /// the <seealso cref="OperatorCheckingMode.Unchecked"/> mode, and one
    /// for the <seealso cref="OperatorCheckingMode.Checked"/> mode.
    /// </summary>
    /// <param name="kind">The operator kind.</param>
    /// <param name="uncheckedMode">The instance reflecting the operator kind in unchecked mode.</param>
    /// <param name="checkedMode">The instance reflecting the operator kind in checked mode.</param>
    public static void ForBothCheckingModes(OperatorKind kind, out CompositeOperatorKind uncheckedMode, out CompositeOperatorKind checkedMode)
    {
        uncheckedMode = new(OperatorCheckingMode.Unchecked, kind);
        checkedMode = new(OperatorCheckingMode.Checked, kind);
    }
}
