using Microsoft.CodeAnalysis.CSharp;
using RoseLynn.CSharp.Syntax;

namespace RoseLynn.Test;

/// <summary>Tests conversions between <see cref="AttributeListTarget"/> values and Roslyn syntax kinds.</summary>
public class AttributeListTargetExtensionsTests
{
    [Test]
    public async Task GetSyntaxKind()
    {
        await Assert.That(AttributeListTarget.Module.GetSyntaxKind()).IsEqualTo(SyntaxKind.ModuleKeyword);
    }
}
