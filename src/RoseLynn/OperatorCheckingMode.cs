#nullable enable

namespace RoseLynn;

/// <summary>
/// Represents the numerical checking mode of the operator, specifically
/// the context in which they can be applied. An operator with an undeclared
/// checking mode defaults to <see langword="unchecked"/> context.
/// </summary>
public enum OperatorCheckingMode
{
    /// <summary>
    /// Declares that the method can be used in both <see langword="unchecked"/>
    /// and <see langword="checked"/> contexts.
    /// </summary>
    Unchecked,
    /// <summary>
    /// Declares that the method can only be used in <see langword="checked"/> context.
    /// </summary>
    Checked,

    /// <summary>
    /// Reflects that the method has no defined checking context restrictions,
    /// defaulting to <seealso cref="Unchecked"/>.
    /// </summary>
    Undefined = Unchecked,
}
