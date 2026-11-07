namespace RoseLynn.CSharp.UnitTests.Resources;

/// <summary>Provides a simple non-static example type for using-directive tests.</summary>
public class ExampleClass { }
/// <summary>Provides a simple static example type for using-directive tests.</summary>
public static class ExampleStatic { }

/// <summary>Provides shared type metadata used by resource-oriented tests.</summary>
public static class ExampleTypes
{
    /// <summary>Gets the base namespace that contains the example test types.</summary>
    public static string BaseNamespace => typeof(ExampleTypes).Namespace!;
}
