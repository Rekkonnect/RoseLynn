using Polyfills;
using System.Text;

namespace RoseLynn.Writing;

public class IndentedStringBuilder
{
    private readonly StringBuilder _builder = new();
    private bool _startedLine;

    public int NestingLevel { get; set; }
    public Indentation NestingIndentation { get; init; }

    public IndentedStringBuilder(char indentationCharacter, int indentationSize)
    {
        NestingIndentation = new(indentationCharacter, indentationSize);
    }

    private void AppendSingleLineContent(char text)
    {
        BeginAppendIndentation();
        _builder.Append(text);
    }

    /// <summary>
    /// Appends the given text content without checking for newlines.
    /// If newlines are contained, the indentation on the lines of the
    /// content will not be applied. The rest of the builder remains
    /// unaffected.
    /// </summary>
    /// <param name="text">
    /// The text to append. It should not contain newlines.
    /// </param>
    public void AppendSingleLineContent(ReadOnlySpan<char> text)
    {
        if (text.Length is 0)
            return;

        BeginAppendIndentation();
        _builder.Append(text);
    }

    private void BeginAppendIndentation()
    {
        if (!_startedLine)
        {
            ApplyIndentation();
            _startedLine = true;
        }
    }

    public NestingLevelNode IncrementNestingLevel()
    {
        return new(this);
    }

    public void Append(char text)
    {
        if (text is '\n')
        {
            AppendLine();
        }
        else
        {
            AppendSingleLineContent(text);
        }
    }

    public void Append(string text)
    {
        Append(text.AsSpan());
    }
    public void Append(ReadOnlySpan<char> text)
    {
        bool addedLine = false;
        foreach (var line in text.EnumerateLines())
        {
            if (addedLine)
                AppendLine();

            AppendSingleLineContent(line);
            addedLine = true;
        }
    }

    public void AppendLine(string text)
    {
        AppendLine(text.AsSpan());
    }
    public void AppendLine(ReadOnlySpan<char> text)
    {
        foreach (var line in text.EnumerateLines())
        {
            AppendSingleLineContent(line);
            AppendLine();
        }
    }

    public void AppendLine(char c)
    {
        Append(c);
        AppendLine();
    }

    public void AppendLine()
    {
        _builder.AppendLine();
        _startedLine = false;
    }

    private void ApplyIndentation()
    {
        int count = NestingLevel * NestingIndentation.Size;
        _builder.Append(NestingIndentation.Character, count);
    }

    public override string ToString()
    {
        return _builder.ToString();
    }

    public readonly struct NestingLevelNode
        : IDisposable
    {
        private readonly IndentedStringBuilder _builder;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        [Obsolete(message: "Do not use the parameterless constructor", error: true)]
        public NestingLevelNode() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public NestingLevelNode(IndentedStringBuilder builder)
        {
            _builder = builder;
            _builder.NestingLevel++;
        }

        public void Dispose()
        {
            _builder.NestingLevel--;
        }
    }
}
