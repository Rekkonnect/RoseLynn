using RoseLynn.Core.SignatureViews;

namespace RoseLynn.VisualBasic.UnitTests;

public sealed class VisualBasicSignatureViewExtensionsTests
{
    [Test]
    public async Task ForVisualBasic_Integer_HasKeyword()
    {
        var vbView = WellKnownSymbolSignatureViews.System_Int32.ForVisualBasic();
        await Assert.That(vbView.Keyword).IsEqualTo("Integer");
    }

    [Test]
    public async Task ForVisualBasic_Boolean_HasKeyword()
    {
        var vbView = WellKnownSymbolSignatureViews.System_Boolean.ForVisualBasic();
        await Assert.That(vbView.Keyword).IsEqualTo("Boolean");
    }

    [Test]
    public async Task ForVisualBasic_String_HasKeyword()
    {
        var vbView = WellKnownSymbolSignatureViews.System_String.ForVisualBasic();
        await Assert.That(vbView.Keyword).IsEqualTo("String");
    }

    [Test]
    public async Task ForVisualBasic_DateTime_HasDateKeyword()
    {
        var vbView = WellKnownSymbolSignatureViews.System_DateTime.ForVisualBasic();
        await Assert.That(vbView.Keyword).IsEqualTo("Date");
    }

    [Test]
    public async Task ForVisualBasic_Object_HasKeyword()
    {
        var vbView = WellKnownSymbolSignatureViews.System_Object.ForVisualBasic();
        await Assert.That(vbView.Keyword).IsEqualTo("Object");
    }

    [Test]
    public async Task ForVisualBasic_NonPredefinedType_KeywordIsNull()
    {
        var vbView = WellKnownSymbolSignatureViews.System_Array.ForVisualBasic();
        await Assert.That(vbView.Keyword).IsNull();
    }

    [Test]
    public async Task ForVisualBasic_NullableInteger_HasNullableKeyword()
    {
        var view = WellKnownSymbolSignatureViews.System_Nullable_T
            .Construct([WellKnownSymbolSignatureViews.System_Int32]);
        var vbView = view.ForVisualBasic();
        await Assert.That(vbView.Keyword).IsEqualTo("Integer?");
    }

    [Test]
    public async Task ForVisualBasic_NullableBoolean_HasNullableKeyword()
    {
        var view = WellKnownSymbolSignatureViews.System_Nullable_T
            .Construct([WellKnownSymbolSignatureViews.System_Boolean]);
        var vbView = view.ForVisualBasic();
        await Assert.That(vbView.Keyword).IsEqualTo("Boolean?");
    }

    [Test]
    public async Task ForVisualBasic_NamedType_NullableInteger_HasNullableKeyword()
    {
        var view = WellKnownSymbolSignatureViews.System_Nullable_T
            .Construct([WellKnownSymbolSignatureViews.System_Int32]);
        var vbView = ((INamedTypeSymbolSignatureView)view).ForVisualBasic();
        await Assert.That(vbView.Keyword).IsEqualTo("Integer?");
    }
}
