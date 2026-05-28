using Polyfills;
using System.Text;

namespace RoseLynn.Writing;

/// <summary>Builds text while automatically applying indentation at the start of each line.</summary>
public class IndentedStringBuilder
{
    private readonly StringBuilder _builder = new();
    private bool _startedLine;

    /// <summary>Gets or sets the current indentation nesting level.</summary>
    public int NestingLevel { get; set; }
    /// <summary>Gets the indentation settings applied for each nesting level.</summary>
    public Indentation NestingIndentation { get; init; }

    /// <summary>Initializes a builder with the specified indentation settings.</summary>
    /// <param name="indentationCharacter">The character used for indentation.</param>
    /// <param name="indentationSize">The number of indentation characters per nesting level.</param>
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

    /// <summary>Increments the nesting level for the lifetime of a disposable scope.</summary>
    /// <returns>A scope that restores the previous nesting level on disposal.</returns>
    public NestingLevelNode IncrementNestingLevel()
    {
        return new(this);
    }

    /// <summary>Appends a character, treating newline characters as line breaks.</summary>
    /// <param name="text">The character to append.</param>
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

    /// <summary>Appends text while applying indentation to each logical line.</summary>
    /// <param name="text">The text to append.</param>
    public void Append(string text)
    {
        Append(text.AsSpan());
    }
    /// <summary>Appends text while applying indentation to each logical line.</summary>
    /// <param name="text">The text to append.</param>
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

    /// <summary>Appends text followed by a line break.</summary>
    /// <param name="text">The text to append before the line break.</param>
    public void AppendLine(string text)
    {
        AppendLine(text.AsSpan());
    }
    /// <summary>Appends text followed by a line break.</summary>
    /// <param name="text">The text to append before the line break.</param>
    public void AppendLine(ReadOnlySpan<char> text)
    {
        foreach (var line in text.EnumerateLines())
        {
            AppendSingleLineContent(line);
            AppendLine();
        }
    }

    /// <summary>Appends a character followed by a line break.</summary>
    /// <param name="c">The character to append before the line break.</param>
    public void AppendLine(char c)
    {
        Append(c);
        AppendLine();
    }

    /// <summary>Appends a line break and resets the line-start indentation state.</summary>
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

    /// <summary>Returns the accumulated text.</summary>
    /// <returns>The current contents of the builder.</returns>
    public override string ToString()
    {
        return _builder.ToString();
    }

    /// <summary>Represents a disposable scope that temporarily increases the nesting level.</summary>
    public readonly struct NestingLevelNode
        : IDisposable
    {
        private readonly IndentedStringBuilder _builder;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        [Obsolete(message: "Do not use the parameterless constructor", error: true)]
        public NestingLevelNode() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        /// <summary>Initializes the scope and increments the builder's nesting level.</summary>
        /// <param name="builder">The builder whose nesting level will be incremented.</param>
        public NestingLevelNode(IndentedStringBuilder builder)
        {
            _builder = builder;
            _builder.NestingLevel++;
        }

        /// <summary>Restores the previous nesting level.</summary>
        public void Dispose()
        {
            _builder.NestingLevel--;
        }
    }
}
