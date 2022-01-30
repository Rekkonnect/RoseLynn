using NUnit.Framework;
using RoseLynn.Utilities;
using System;

namespace RoseLynn.Test.Utilities;

public class InterlinkedDictionaryTests
{
    private const int intValue = 1;
    private const string stringValue = "one";

    private readonly InterlinkedDictionary<int, string> baseDictionary = new();

    public InterlinkedDictionaryTests()
    {
        // Mutation functions should never be invoked for the base dictionary instance
        // If they are invoked in copied instances, and the base instance is affected,
        // some tests will catch it by randomly failing
        baseDictionary.Add(intValue, stringValue);
    }

    [Test]
    public void AddTest()
    {
        Assert.AreEqual(1, baseDictionary.Count);

        Assert.AreEqual(intValue, baseDictionary[stringValue]);
        Assert.AreEqual(stringValue, baseDictionary[intValue]);

        // + 1 ensures that the values are different
        Assert.Throws<ArgumentException>(GetDictionaryAddDelegate(baseDictionary, intValue + 1, stringValue));
        Assert.Throws<ArgumentException>(GetDictionaryAddDelegate(baseDictionary, intValue, stringValue + 1));

        // We don't care about the type of the exception that is thrown if both invalid cases
        // occur, for long as the individual invalid cases are caught
        Assert.Throws<ArgumentNullException>(GetDictionaryAddDelegate(baseDictionary, intValue + 1, null));

        Assert.AreEqual(1, baseDictionary.Count);

        static TestDelegate GetDictionaryAddDelegate<T1, T2>(InterlinkedDictionary<T1, T2> dictionary, T1 t1, T2 t2)
        {
            return () => dictionary.Add(t1, t2);
        }
    }

    [Test]
    public void RemoveTest()
    {
        var dictionary = new InterlinkedDictionary<int, string>(baseDictionary);

        Assert.True(dictionary.Remove(intValue));
        AssertEmptyDictionary(dictionary);

        // For the string version too
        dictionary = new InterlinkedDictionary<int, string>(baseDictionary);

        Assert.True(dictionary.Remove(stringValue));
        AssertEmptyDictionary(dictionary);
    }

    [Test]
    public void AccessorTest()
    {
        var dictionary = new InterlinkedDictionary<int, string>(baseDictionary);

        int oldCount = dictionary.Count;

        var newStringValue = stringValue + 1;
        dictionary[intValue] = newStringValue;

        Assert.AreNotEqual(stringValue, dictionary[intValue]);
        Assert.AreEqual(stringValue, baseDictionary[intValue]);

        Assert.False(dictionary.Contains(stringValue));

        int newIntValue = intValue + 1;
        dictionary[newStringValue] = newIntValue;
        Assert.AreNotEqual(intValue, dictionary[newStringValue]);

        Assert.False(dictionary.Contains(intValue));

        Assert.AreEqual(oldCount, dictionary.Count);
    }

    [Test]
    public void ClearTest()
    {
        var dictionary = new InterlinkedDictionary<int, string>(baseDictionary);

        dictionary.Clear();
        AssertEmptyDictionary(dictionary);
    }

    [Test]
    public void TryGetValue()
    {
        Assert.True(baseDictionary.TryGetValue(intValue, out var outputStringValue));
        Assert.AreEqual(stringValue, outputStringValue);

        Assert.True(baseDictionary.TryGetValue(stringValue, out int outputIntValue));
        Assert.AreEqual(intValue, outputIntValue);
    }

    private static void AssertEmptyDictionary(InterlinkedDictionary<int, string> dictionary)
    {
        Assert.AreEqual(0, dictionary.Count);

        Assert.False(dictionary.Contains(intValue));
        Assert.False(dictionary.Contains(stringValue));
    }
}
