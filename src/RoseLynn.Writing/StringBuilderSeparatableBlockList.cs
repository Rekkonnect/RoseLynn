using Garyon.Objects.Strings;
using System.Text;

namespace RoseLynn.Writing;

/// <summary>Writes blank-line separators between committed blocks in a <see cref="StringBuilder"/>.</summary>
/// <param name="builder">The builder that receives separators between blocks.</param>
public sealed class StringBuilderSeparatableBlockList(StringBuilder builder)
    : BaseSeparatableBlockList
{
    private readonly StringBuilder _builder = builder;

    /// <summary>Appends the separator between committed blocks.</summary>
    protected override void AppendSeparator()
    {
        _builder.Append(WhitespaceFacts.CrLf);
    }
}
