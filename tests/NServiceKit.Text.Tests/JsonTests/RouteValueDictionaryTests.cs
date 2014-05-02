using System;
using System.Runtime.Serialization;
using NUnit.Framework;
using NServiceKit.Html;

namespace NServiceKit.Text.Tests.JsonTests
{
    /// <summary>A route value dictionary tests.</summary>
    [TestFixture]
    public class RouteValueDictionaryTests : TestBase
    {
        /// <summary>Does deserialize route value dictionary.</summary>
        [Test, Ignore("Has both: ICollection<KeyValuePair> and IDictionary, test should be fixed or redesigned.")]
        public void Does_deserialize_RouteValueDictionary()
        {
            var item = new TestObject
            {
                PropA = "foo",
                Values = new RouteValueDictionary { { "something", "lies here" } }
            };

            var jsonSerialized = JsonSerializer.SerializeToString(item);
            var typeSerialized = TypeSerializer.SerializeToString(item);

            var jsonResult = JsonSerializer.DeserializeFromString<TestObject>(jsonSerialized);
            var typeResult = TypeSerializer.DeserializeFromString<TestObject>(typeSerialized);

            Assert.AreEqual(item.PropA, jsonResult.PropA);
            Assert.NotNull(jsonResult.Values);
            Assert.AreEqual(item.Values.Count, jsonResult.Values.Count);
            Assert.AreEqual(item.Values["something"], jsonResult.Values["something"]);

            Assert.AreEqual(item.PropA, typeResult.PropA);
            Assert.NotNull(typeResult.Values);
            Assert.AreEqual(item.Values.Count, typeResult.Values.Count);
            Assert.AreEqual(item.Values["something"], typeResult.Values["something"]);
        }

        /// <summary>A test object.</summary>
        [DataContract]
        class TestObject
        {
            /// <summary>Gets or sets the property a.</summary>
            /// <value>The property a.</value>
            [DataMember]
            public string PropA { get; set; }

            /// <summary>Gets or sets the values.</summary>
            /// <value>The values.</value>
            [DataMember]
            public RouteValueDictionary Values { get; set; }
        }
    }
}