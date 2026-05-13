using Garyon.Objects.Strings;
using System.Text;

namespace RoseLynn.Writing;

public sealed class StringBuilderSeparatableBlockList(StringBuilder builder)
    : BaseSeparatableBlockList
{
    private readonly StringBuilder _builder = builder;

    protected override void AppendSeparator()
    {
        _builder.Append(WhitespaceFacts.CrLf);
    }
}
