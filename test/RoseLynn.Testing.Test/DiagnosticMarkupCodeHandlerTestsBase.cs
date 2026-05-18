namespace RoseLynn.Testing.Test;

/// <summary>Provides the base contract for diagnostic markup code-handler tests.</summary>
public abstract class DiagnosticMarkupCodeHandlerTestsBase
{
    /// <summary>Verifies that diagnostic markup is removed correctly.</summary>
    public abstract Task RemoveMarkupTest();
}
