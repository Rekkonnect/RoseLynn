namespace RoseLynn.Testing;

/// <summary>
/// Provides common skip reasons for compilation-based efforts that can be used across multiple tests.
/// </summary>
public static class SkipReasons
{
    /// <summary>
    /// Describes FAWMN's limitation against attribute targets.
    /// </summary>
    public const string FawmnAndAttributeTargets = """
        FAWMN is not supported on attribute targets that differ from the
        attributed declaration symbol. This is a known limitation that has an
        open issue tracking it here:
        https://github.com/dotnet/roslyn/issues/80511
        """;
}
