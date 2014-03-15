using System;
using System.Linq;
using NUnit.Framework;

namespace NServiceKit.Text.Tests.JsvTests
{
    [TestFixture]
    public class GeneratedJsvTests
    {
        public interface ITest
        {}

        public class TestObject : ITest
        {
            public TestParameter Parameter { get; set; }
            public ITest InterfaceParameter { get; set; }
        }

        public class TestParameter : ITest
        {
            public string Value { get; set; }
        }

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