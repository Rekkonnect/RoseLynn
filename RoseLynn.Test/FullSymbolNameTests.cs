using NUnit.Framework;
using System;

namespace RoseLynn.Test;

public class FullSymbolNameTests
{
    [Test]
    public void FullNames()
    {
        var namespaces = new[] { nameof(RoseLynn), nameof(Test) };
        var containerTypes = new[] { nameof(FullSymbolName), nameof(FullSymbolNameTests) };
        var containerMethods = new[] { nameof(Equals), nameof(FullNames) };
        var symbolName = nameof(String);

        var fullName = new FullSymbolName(symbolName, namespaces, containerTypes, containerMethods);

        var expectedNamespacesString = $"{nameof(RoseLynn)}{fullName.NamespaceDelimiter}{nameof(Test)}";
        var actualNamespacesString = fullName.FullNamespaceString;
        Assert.AreEqual(expectedNamespacesString, actualNamespacesString);

        var expectedContainerTypeString = $"{nameof(FullSymbolName)}{fullName.ContainerTypeDelimiter}{nameof(FullSymbolNameTests)}";
        var actualContainerTypeString = fullName.FullContainerTypeString;
        Assert.AreEqual(expectedContainerTypeString, actualContainerTypeString);

        var expectedContainerMethodString = $"{nameof(Equals)}{fullName.ContainerMethodDelimiter}{nameof(FullNames)}";
        var actualContainerMethodString = fullName.FullContainerMethodString;
        Assert.AreEqual(expectedContainerMethodString, actualContainerMethodString);

        var expectedFullNameString = $"{expectedNamespacesString}{fullName.NamespaceDelimiter}{expectedContainerTypeString}{fullName.ContainerTypeDelimiter}{expectedContainerMethodString}{fullName.ContainerMethodDelimiter}{symbolName}";
        Assert.AreEqual(expectedFullNameString, fullName.FullNameString);
    }
    [Test]
    public void FullNamesOnlySingleNamespace()
    {
        var namespaces = new[] { nameof(RoseLynn) };
        var symbolName = nameof(String);

        var fullName = new FullSymbolName(symbolName, namespaces, null, null);

        Assert.AreEqual(nameof(RoseLynn), fullName.FullNamespaceString);
        Assert.AreEqual("", fullName.FullContainerTypeString);
        Assert.AreEqual("", fullName.FullContainerMethodString);

        Assert.AreEqual($"{nameof(RoseLynn)}{fullName.NamespaceDelimiter}{symbolName}", fullName.FullNameString);
    }
    [Test]
    public void FullNamesOnlySingleContainerTypeAndMethod()
    {
        var containerTypes = new[] { nameof(FullSymbolNameTests) };
        var containerMethods = new[] { nameof(FullNames) };
        var symbolName = nameof(String);

        var fullName = new FullSymbolName(symbolName, null, containerTypes, containerMethods);

        Assert.AreEqual("", fullName.FullNamespaceString);
        Assert.AreEqual(nameof(FullSymbolNameTests), fullName.FullContainerTypeString);
        Assert.AreEqual(nameof(FullNames), fullName.FullContainerMethodString);

        Assert.AreEqual($"{nameof(FullSymbolNameTests)}{fullName.ContainerTypeDelimiter}{nameof(FullNames)}{fullName.ContainerMethodDelimiter}{symbolName}", fullName.FullNameString);
    }
}
