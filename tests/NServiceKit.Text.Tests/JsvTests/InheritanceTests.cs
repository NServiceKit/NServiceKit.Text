using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NServiceKit.Text.Jsv;

namespace NServiceKit.Text.Tests.JsvTests
{
    /// <summary>An inheritance tests.</summary>
    [TestFixture]
    public class InheritanceTests
    {
        /// <summary>A test parent.</summary>
        public class TestParent
        {
            /// <summary>Gets the parameter.</summary>
            /// <value>The parameter.</value>
            public string Parameter { get; private set; }

            /// <summary>
            /// Initializes a new instance of the NServiceKit.Text.Tests.JsvTests.InheritanceTests.TestParent
            /// class.
            /// </summary>
            /// <param name="parameter">The parameter.</param>
            public TestParent(string parameter)
            {
                Parameter = parameter;
            }
        }

        /// <summary>A test child.</summary>
        public class TestChild : TestParent
        {
            /// <summary>
            /// Initializes a new instance of the NServiceKit.Text.Tests.JsvTests.InheritanceTests.TestChild
            /// class.
            /// </summary>
            /// <param name="parameter">The parameter.</param>
            public TestChild(string parameter)
                : base(parameter)
            { }
        }

        /// <summary>Should set property of parent class.</summary>
        [Test]
        public void Should_set_property_of_parent_class()
        {
            var serializer = new JsvSerializer<TestChild>();
            var serialized = serializer.SerializeToString(new TestChild("Test Value"));
            var deserialized = serializer.DeserializeFromString(serialized);
            Assert.That(deserialized.Parameter, Is.EqualTo("Test Value"));
        }
    }
}
