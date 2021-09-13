using NUnit.Framework;
using RoseLynn.Utilities;
using System.Linq;

namespace RoseLynn.Test.Utilities
{
    public class SingleElementCollectionTests
    {
        [Test]
        public void Test()
        {
            var collection = new SingleElementCollection<int>(2);
            Assert.AreEqual(new[] { 2 }, collection.ToArray());
        }
    }
}