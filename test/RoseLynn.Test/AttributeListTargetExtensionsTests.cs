using Microsoft.CodeAnalysis.CSharp;
using RoseLynn.CSharp.Syntax;
using System.Threading.Tasks;

namespace RoseLynn.Test;

public class AttributeListTargetExtensionsTests
{
    [Test]
    public async Task GetSyntaxKind()
    {
        await Assert.That(AttributeListTarget.Module.GetSyntaxKind()).IsEqualTo(SyntaxKind.ModuleKeyword);
    }
}
