using Microsoft.CodeAnalysis.CSharp;
using NUnit.Framework;
using RoseLynn.CSharp.Syntax;

namespace RoseLynn.Test;

public class AttributeListTargetExtensionsTests
{
    [Test]
    public void GetSyntaxKind()
    {
        Assert.AreEqual(SyntaxKind.ModuleKeyword, AttributeListTarget.Module.GetSyntaxKind());
    }
}
