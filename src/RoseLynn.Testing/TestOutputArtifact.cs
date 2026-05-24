namespace RoseLynn.Testing;

/// <summary>
/// Represents a test output artifact. It should be adaptable to any testing
/// framework's artifact type.
/// </summary>
public sealed class TestOutputArtifact
{
    /// <summary>
    /// Gets or sets the full path of the generated artifact file.
    /// </summary>
    public required string FilePath { get; init; }

    /// <summary>
    /// Gets or sets the user-facing name of the artifact.
    /// </summary>
    public required string DisplayName { get; init; }

    /// <summary>
    /// Gets or sets a short description of the artifact contents.
    /// </summary>
    public required string Description { get; init; }
}
