using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using NServiceKit.Text.Common;
using NServiceKit.Text.Jsv;

namespace NServiceKit.Text.Tests.JsvTests
{
    /// <summary>A jsv deserialize type tests.</summary>
    [TestFixture]
    public class JsvDeserializeTypeTests
    {
        /// <summary>Gets setter method for simple properties.</summary>
        [Test]
        public void Get_setter_method_for_simple_properties()
        {
            Type type = typeof (Test);
            PropertyInfo propertyInfo = type.GetProperty("TestProperty");
            SetPropertyDelegate setMethod = JsvDeserializeType.GetSetPropertyMethod(type, propertyInfo);
            Test test = new Test();
            setMethod.Invoke(test, "test");
            Assert.AreEqual("test", test.TestProperty);
        }

        /// <summary>Gets setter method for dictionary properties.</summary>
        [Test]
        public void Get_setter_method_for_dictionary_properties()
        {
            var dict = new Dictionary<string, string>();
            Type type = typeof (Dictionary<string,string>);
            foreach (var propertyInfo in type.GetProperties()) {
                SetPropertyDelegate setMethod = JsvDeserializeType.GetSetPropertyMethod(type, propertyInfo);
                if (setMethod == null) continue;
                Console.WriteLine(propertyInfo.Name);
                setMethod.Invoke(dict, propertyInfo.Name);
            }
        }

        /// <summary>A test.</summary>
        private class Test
        {
            /// <summary>Gets or sets the test property.</summary>
            /// <value>The test property.</value>
            public string TestProperty { get; set; }
        }
    }
}