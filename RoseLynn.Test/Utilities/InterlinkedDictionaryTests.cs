using RoseLynn.Utilities;
using System;
using System.Threading.Tasks;

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
    public async Task AddTest()
    {
        await Assert.That(baseDictionary.Count).IsEqualTo(1);

        await Assert.That(baseDictionary[stringValue]).IsEqualTo(intValue);
        await Assert.That(baseDictionary[intValue]).IsEqualTo(stringValue);

        await Assert.That(GetDictionaryAddDelegate(baseDictionary, intValue + 1, stringValue)).Throws<ArgumentException>();
        await Assert.That(GetDictionaryAddDelegate(baseDictionary, intValue, stringValue + 1)).Throws<ArgumentException>();

        await Assert.That(GetDictionaryAddDelegate(baseDictionary, intValue + 1, null)).Throws<ArgumentNullException>();

        await Assert.That(baseDictionary.Count).IsEqualTo(1);

        static Action GetDictionaryAddDelegate<T1, T2>(InterlinkedDictionary<T1, T2> dictionary, T1 t1, T2 t2)
        {
            return () => dictionary.Add(t1, t2);
        }
    }

    [Test]
    public async Task RemoveTest()
    {
        var dictionary = new InterlinkedDictionary<int, string>(baseDictionary);

        await Assert.That(dictionary.Remove(intValue)).IsTrue();
        await AssertEmptyDictionary(dictionary);

        // For the string version too
        dictionary = new InterlinkedDictionary<int, string>(baseDictionary);

        await Assert.That(dictionary.Remove(stringValue)).IsTrue();
        await AssertEmptyDictionary(dictionary);
    }

    [Test]
    public async Task AccessorTest()
    {
        var dictionary = new InterlinkedDictionary<int, string>(baseDictionary);

        int oldCount = dictionary.Count;

        var newStringValue = stringValue + 1;
        dictionary[intValue] = newStringValue;

        await Assert.That(dictionary[intValue]).IsNotEqualTo(stringValue);
        await Assert.That(baseDictionary[intValue]).IsEqualTo(stringValue);

        await Assert.That(dictionary.Contains(stringValue)).IsFalse();

        int newIntValue = intValue + 1;
        dictionary[newStringValue] = newIntValue;
        await Assert.That(dictionary[newStringValue]).IsNotEqualTo(intValue);

        await Assert.That(dictionary.Contains(intValue)).IsFalse();

        await Assert.That(dictionary.Count).IsEqualTo(oldCount);
    }

    [Test]
    public async Task ClearTest()
    {
        var dictionary = new InterlinkedDictionary<int, string>(baseDictionary);

        dictionary.Clear();
        await AssertEmptyDictionary(dictionary);
    }

    [Test]
    public async Task TryGetValue()
    {
        await Assert.That(baseDictionary.TryGetValue(intValue, out var outputStringValue)).IsTrue();
        await Assert.That(outputStringValue).IsEqualTo(stringValue);

        await Assert.That(baseDictionary.TryGetValue(stringValue, out int outputIntValue)).IsTrue();
        await Assert.That(outputIntValue).IsEqualTo(intValue);
    }

    private static async Task AssertEmptyDictionary(InterlinkedDictionary<int, string> dictionary)
    {
        await Assert.That(dictionary.Count).IsEqualTo(0);

        await Assert.That(dictionary.Contains(intValue)).IsFalse();
        await Assert.That(dictionary.Contains(stringValue)).IsFalse();
    }
}
