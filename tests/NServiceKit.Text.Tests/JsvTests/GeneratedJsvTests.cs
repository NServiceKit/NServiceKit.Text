using System;
using System.Linq;
using NUnit.Framework;

namespace NServiceKit.Text.Tests.JsvTests
{
    /// <summary>A generated jsv tests.</summary>
    [TestFixture]
    public class GeneratedJsvTests
    {
        /// <summary>Interface for test.</summary>
        public interface ITest
        {}

        /// <summary>A test object.</summary>
        public class TestObject : ITest
        {
            /// <summary>Gets or sets the parameter.</summary>
            /// <value>The parameter.</value>
            public TestParameter Parameter { get; set; }

            /// <summary>Gets or sets the interface parameter.</summary>
            /// <value>The interface parameter.</value>
            public ITest InterfaceParameter { get; set; }
        }

        /// <summary>A test parameter.</summary>
        public class TestParameter : ITest
        {
            /// <summary>Gets or sets the value.</summary>
            /// <value>The value.</value>
            public string Value { get; set; }
        }

        /// <summary>
        /// Interface typed property in derived class should have type information.
        /// </summary>
        [Test]
        public void Interface_typed_property_in_derived_class_should_have_type_info()
        {
            const string expected =
                "{__type:\"NServiceKit.Text.Tests.JsvTests.GeneratedJsvTests+TestObject, " +
                "NServiceKit.Text.Tests\",InterfaceParameter:{__type:" +
                "\"NServiceKit.Text.Tests.JsvTests.GeneratedJsvTests+TestParameter, " +
                "NServiceKit.Text.Tests\",Value:Some Value}}";

            ITest test = new TestObject {InterfaceParameter = new TestParameter {Value = "Some Value"}};
            var jsv = test.ToJsv();
            Assert.That(jsv, Is.EqualTo(expected));
        }

        /// <summary>Property in derived class should not have type information.</summary>
        [Test]
        public void Property_in_derived_class_should_not_have_type_info()
        {
            const string expected =
                "{__type:\"NServiceKit.Text.Tests.JsvTests.GeneratedJsvTests+TestObject, " +
                "NServiceKit.Text.Tests\",Parameter:{Value:Some Value}}";

            ITest test = new TestObject {Parameter = new TestParameter {Value = "Some Value"}};
            var jsv = test.ToJsv();
            Assert.That(jsv, Is.EqualTo(expected));
        }
    }
}