using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace RoseLynn.Test;

public sealed class SyntaxQueryingExtensionsTests
{
    private static SyntaxNode ParseRoot(string source) =>
        CSharpSyntaxTree.ParseText(source).GetRoot();

    [Test]
    public async Task ContainsStrict_InnerFitsInsideOuter_ReturnsTrue()
    {
        var outer = TextSpan.FromBounds(0, 10);
        var inner = TextSpan.FromBounds(2, 8);
        await Assert.That(outer.ContainsStrict(inner)).IsTrue();
    }

    [Test]
    public async Task ContainsStrict_InnerEqualsOuter_ReturnsTrue()
    {
        var span = TextSpan.FromBounds(0, 10);
        await Assert.That(span.ContainsStrict(span)).IsTrue();
    }

    [Test]
    public async Task ContainsStrict_InnerStartsBeforeOuter_ReturnsFalse()
    {
        var outer = TextSpan.FromBounds(5, 15);
        var inner = TextSpan.FromBounds(3, 10);
        await Assert.That(outer.ContainsStrict(inner)).IsFalse();
    }

    [Test]
    public async Task ContainsStrict_InnerEndsAfterOuter_ReturnsFalse()
    {
        var outer = TextSpan.FromBounds(0, 10);
        var inner = TextSpan.FromBounds(2, 12);
        await Assert.That(outer.ContainsStrict(inner)).IsFalse();
    }

    [Test]
    public async Task DeepestTokenContainingPosition_InsideIdentifier_ReturnsIdentifierToken()
    {
        const string source = "class Foo { }";
        var root = ParseRoot(source);
        // "Foo" starts at position 6
        var token = root.DeepestTokenContainingPosition(6);
        await Assert.That(token.ValueText).IsEqualTo("Foo");
    }

    [Test]
    public async Task DeepestTokenContainingPosition_OutsideSpan_ReturnsDefault()
    {
        const string source = "class Foo { }";
        var root = ParseRoot(source);
        var token = root.DeepestTokenContainingPosition(-1);
        await Assert.That(token).IsEqualTo(default(SyntaxToken));
    }

    [Test]
    public async Task DeepestNodeContainingPosition_InsideSource_ReturnsNodeContainingText()
    {
        const string source = "class Foo { }";
        var root = ParseRoot(source);
        var node = root.DeepestNodeContainingPosition(6);
        await Assert.That(node).IsNotNull();
        await Assert.That(node!.ToString()).Contains("Foo");
    }

    [Test]
    public async Task DeepestNodeContainingPosition_OutsideSpan_ReturnsNull()
    {
        const string source = "class Foo { }";
        var root = ParseRoot(source);
        var node = root.DeepestNodeContainingPosition(-1);
        await Assert.That(node).IsNull();
    }

    [Test]
    public async Task OutermostSameSpanParent_WalksUpToOutermostSameSpanAncestor()
    {
        // Top-level statement: GlobalStatementSyntax wraps LocalDeclarationStatementSyntax,
        // both sharing the same Span over "var x = 1;"
        const string source = "var x = 1;";
        var root = ParseRoot(source);
        var globalStatement = root.ChildNodes().First();
        var localDecl = globalStatement.ChildNodes().First();

        await Assert.That(globalStatement.Span).IsEqualTo(localDecl.Span);

        var outermost = localDecl.OutermostSameSpanParent();
        await Assert.That(outermost.Span).IsEqualTo(localDecl.Span);
        // Outermost should have walked up past localDecl
        await Assert.That(outermost).IsNotEqualTo(localDecl);
    }

    [Test]
    public async Task OutermostSameSpanParent_WhenParentSpanDiffers_ReturnsSelf()
    {
        const string source = "class Foo { void Bar() {} }";
        var root = ParseRoot(source);
        var classDecl = root.ChildNodes().First();
        // Method declaration is clearly inside the class body; its span is smaller than the class's
        var methodDecl = classDecl.ChildNodes()
            .OfType<Microsoft.CodeAnalysis.CSharp.Syntax.MethodDeclarationSyntax>()
            .First();
        await Assert.That(methodDecl.Span).IsNotEqualTo(classDecl.Span);
        var outermost = methodDecl.OutermostSameSpanParent();
        await Assert.That(outermost).IsEqualTo(methodDecl);
    }

    [Test]
    public async Task GetLinePosition_FirstCharacter_IsLineZeroCharZero()
    {
        const string source = "class Foo { }";
        var tree = CSharpSyntaxTree.ParseText(source);
        var pos = tree.GetLinePosition(0);
        await Assert.That(pos.Line).IsEqualTo(0);
        await Assert.That(pos.Character).IsEqualTo(0);
    }

    [Test]
    public async Task GetLinePosition_SecondLine_IsLineOne()
    {
        const string source = "// comment\nclass Foo { }";
        var tree = CSharpSyntaxTree.ParseText(source);
        // position 11 is the start of "class" on the second line
        var pos = tree.GetLinePosition(11);
        await Assert.That(pos.Line).IsEqualTo(1);
        await Assert.That(pos.Character).IsEqualTo(0);
    }

    [Test]
    public async Task SyntaxNodeAtPosition_WithinSource_ReturnsNode()
    {
        const string source = "class Foo { }";
        var tree = CSharpSyntaxTree.ParseText(source);
        var node = tree.SyntaxNodeAtPosition(6);
        await Assert.That(node).IsNotNull();
    }

    [Test]
    public async Task ChildThatContainsSpan_WhenChildCoversSpan_ReturnsChild()
    {
        const string source = "class Foo { }";
        var root = ParseRoot(source);
        // Span of "Foo" (positions 6..8 inclusive → TextSpan [6,9))
        var fooSpan = new TextSpan(6, 3);
        var child = root.ChildThatContainsSpan(fooSpan);
        await Assert.That(child).IsNotEqualTo(default(SyntaxNodeOrToken));
    }

    [Test]
    public async Task ChildThatContainsSpan_WhenSpanSpansMultipleChildren_ReturnsDefault()
    {
        const string source = "class Foo { }";
        var root = ParseRoot(source);
        // A span from the very start to very end covers the entire compilation unit, not a single child
        var classDecl = root.ChildNodes().First();
        var fullSpan = classDecl.FullSpan;
        // Ask the classDecl for a child that spans the whole classDecl – no single child does
        var child = classDecl.ChildThatContainsSpan(fullSpan);
        await Assert.That(child).IsEqualTo(default(SyntaxNodeOrToken));
    }

    [Test]
    public async Task DeepestTokenContainingSpan_WhenTokenMatchesSpan_ReturnsToken()
    {
        const string source = "class Foo { }";
        var root = ParseRoot(source);
        var fooSpan = new TextSpan(6, 3); // "Foo"
        var token = root.DeepestTokenContainingSpan(fooSpan);
        await Assert.That(token.ValueText).IsEqualTo("Foo");
    }

    [Test]
    public async Task DeepestTokenContainingSpan_WhenSpanOutsideRoot_ReturnsDefault()
    {
        const string source = "class Foo { }";
        var root = ParseRoot(source);
        var outsideSpan = TextSpan.FromBounds(1000, 1010);
        var token = root.DeepestTokenContainingSpan(outsideSpan);
        await Assert.That(token).IsEqualTo(default(SyntaxToken));
    }
}
