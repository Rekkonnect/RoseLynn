using System;
using System.Threading.Tasks;

namespace RoseLynn.Test;

public class FullSymbolNameTests
{
    [Test]
    public async Task FullNames()
    {
        var namespaces = new[] { nameof(RoseLynn), nameof(Test) };
        var containerTypes = new[] { nameof(FullSymbolName), nameof(FullSymbolNameTests) };
        var containerMethods = new[] { nameof(Equals), nameof(FullNames) };
        var symbolName = nameof(String);

        var fullName = new FullSymbolName(symbolName, namespaces, containerTypes, containerMethods);

        var expectedNamespacesString = $"{nameof(RoseLynn)}{fullName.NamespaceDelimiter}{nameof(Test)}";
        var actualNamespacesString = fullName.FullNamespaceString;
        await Assert.That(actualNamespacesString).IsEqualTo(expectedNamespacesString);

        var expectedContainerTypeString = $"{nameof(FullSymbolName)}{fullName.ContainerTypeDelimiter}{nameof(FullSymbolNameTests)}";
        var actualContainerTypeString = fullName.FullContainerTypeString;
        await Assert.That(actualContainerTypeString).IsEqualTo(expectedContainerTypeString);

        var expectedContainerMethodString = $"{nameof(Equals)}{fullName.ContainerMethodDelimiter}{nameof(FullNames)}";
        var actualContainerMethodString = fullName.FullContainerMethodString;
        await Assert.That(actualContainerMethodString).IsEqualTo(expectedContainerMethodString);

        var expectedFullNameString = $"{expectedNamespacesString}{fullName.NamespaceDelimiter}{expectedContainerTypeString}{fullName.ContainerTypeDelimiter}{expectedContainerMethodString}{fullName.ContainerMethodDelimiter}{symbolName}";
        await Assert.That(fullName.FullNameString).IsEqualTo(expectedFullNameString);
    }
    [Test]
    public async Task FullNamesOnlySingleNamespace()
    {
        var namespaces = new[] { nameof(RoseLynn) };
        var symbolName = nameof(String);

        var fullName = new FullSymbolName(symbolName, namespaces, null, null);

        await Assert.That(fullName.FullNamespaceString).IsEqualTo(nameof(RoseLynn));
        await Assert.That(fullName.FullContainerTypeString).IsEqualTo("");
        await Assert.That(fullName.FullContainerMethodString).IsEqualTo("");

        await Assert.That(fullName.FullNameString).IsEqualTo($"{nameof(RoseLynn)}{fullName.NamespaceDelimiter}{symbolName}");
    }
    [Test]
    public async Task FullNamesOnlySingleContainerTypeAndMethod()
    {
        var containerTypes = new[] { nameof(FullSymbolNameTests) };
        var containerMethods = new[] { nameof(FullNames) };
        var symbolName = nameof(String);

        var fullName = new FullSymbolName(symbolName, null, containerTypes, containerMethods);

        await Assert.That(fullName.FullNamespaceString).IsEqualTo("");
        await Assert.That(fullName.FullContainerTypeString).IsEqualTo(nameof(FullSymbolNameTests));
        await Assert.That(fullName.FullContainerMethodString).IsEqualTo(nameof(FullNames));

        await Assert.That(fullName.FullNameString).IsEqualTo($"{nameof(FullSymbolNameTests)}{fullName.ContainerTypeDelimiter}{nameof(FullNames)}{fullName.ContainerMethodDelimiter}{symbolName}");
    }

    [Test]
    public async Task FullNameForNestedType()
    {
        var name = FullSymbolName.ForType<TestType>();
        
        var containerNamespaces = new[] { nameof(RoseLynn), nameof(Test) };
        var containerTypes = new[] { nameof(FullSymbolNameTests) };
        var symbolName = nameof(TestType);

        var targetFullName = new FullSymbolName(symbolName, containerNamespaces, containerTypes, null);

        await Assert.That(name.Matches(targetFullName, SymbolNameMatchingLevel.Namespace)).IsTrue();
    }
    [Test]
    public async Task FullNameForMethodTypeParameter()
    {
        var typeParameter = typeof(TestType).GetMethod(nameof(TestType.Method))!.GetGenericArguments()[0];
        var name = FullSymbolName.ForType(typeParameter);

        var containerNamespaces = new[] { nameof(RoseLynn), nameof(Test) };
        var containerTypes = new[] { nameof(FullSymbolNameTests), nameof(TestType) };
        var containerMethods = new[] { nameof(TestType.Method) };
        var symbolName = "T";

        var targetFullName = new FullSymbolName(symbolName, containerNamespaces, containerTypes, containerMethods);

        await Assert.That(name.Matches(targetFullName, SymbolNameMatchingLevel.Namespace)).IsTrue();
    }

    private abstract class TestType
    {
        public abstract void Method<T>();
    }
}
