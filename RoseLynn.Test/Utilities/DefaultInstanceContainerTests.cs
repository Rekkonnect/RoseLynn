using NUnit.Framework;
using RoseLynn.Utilities;
using System;

namespace RoseLynn.Test.Utilities
{
    public class DefaultInstanceContainerTests
    {
        private readonly NotInvalidTypeInstanceContainer container = new();

        [Test]
        public void GetDefaultInstanceTests()
        {
            Assert.IsNull(container.GetDefaultInstance<Base>());
            Assert.IsNull(container.GetDefaultInstance<InvalidImplementation>());
            AssertInstanceOf<ImplementationA>();
            AssertInstanceOf<ImplementationB>();
            AssertInstanceOf<ImplementationC>();

            void AssertInstanceOf<T>()
                where T : Base
            {
                Assert.IsInstanceOf<T>(container.GetDefaultInstance<T>());
            }
        }

        [Test]
        public void GetIrrelevantDefaultInstancesTests()
        {
            // Some other random types that are found in the assemblies
            Assert.IsNull(container.GetDefaultInstance(typeof(Enum)));
            Assert.IsNull(container.GetDefaultInstance(typeof(ImplementationA[])));
            Assert.IsNull(container.GetDefaultInstance(typeof(Delegate)));
            Assert.IsNull(container.GetDefaultInstance(typeof(DayOfWeek)));
        }

        private sealed class NotInvalidTypeInstanceContainer : DefaultInstanceContainer<Base>
        {
            protected override object[] GetDefaultInstanceArguments()
            {
                return new object[] { Array.Empty<int>() };
            }

            protected override bool IsValidInstanceType(Type type)
            {
                return !type.Name.Contains("Invalid");
            }
        }

        private abstract class Base
        {
            protected Base(params int[] values) { }
        }

        private sealed class ImplementationA : Base
        {
            public ImplementationA(params int[] values)
                : base(values) { }
        }
        private sealed class ImplementationB : Base
        {
            public ImplementationB(params int[] values)
                : base(values) { }
        }
        private sealed class ImplementationC : Base
        {
            public ImplementationC(params int[] values)
                : base(values) { }
        }

        private sealed class InvalidImplementation : Base
        {
            public InvalidImplementation(params int[] values)
                : base(values) { }
        }
    }
}