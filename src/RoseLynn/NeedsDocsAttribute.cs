namespace RoseLynn;

/// <summary>
/// This attribute denotes that the annotated member needs docs, possibly
/// due to its non-triviality, and the fact that it can be a very important
/// member to document. This attribute can also be helpful to pick up from
/// AI models so that they can generate documentation in a focused manner.
/// </summary>
public sealed class NeedsDocsAttribute : Attribute;
