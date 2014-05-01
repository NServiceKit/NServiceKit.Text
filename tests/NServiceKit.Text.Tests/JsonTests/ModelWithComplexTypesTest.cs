using NUnit.Framework;
using NServiceKit.Text.Tests.DynamicModels;

namespace NServiceKit.Text.Tests.JsonTests
{
    /// <summary>A model with complex types test.</summary>
    [TestFixture]
    public class ModelWithComplexTypesTest
    {
        /// <summary>Can serialize.</summary>
        [Test]
        public void Can_Serialize()
        {
            var m1 = ModelWithComplexTypes.Create(1);
            var s = JsonSerializer.SerializeToString(m1);
            var m2 = JsonSerializer.DeserializeFromString<ModelWithComplexTypes>(s);

            Assert.AreEqual(m1.ListValue[0], m2.ListValue[0]);
            Assert.AreEqual(m1.DictionaryValue["a"], m2.DictionaryValue["a"]);
            Assert.AreEqual(m1.ByteArrayValue[0], m2.ByteArrayValue[0]);
        }

        /// <summary>Can serialize when null.</summary>
        [Test]
        public void Can_Serialize_WhenNull()
        {
            var m1 = new ModelWithComplexTypes();

            JsConfig.IncludeNullValues = false;
            var s = JsonSerializer.SerializeToString(m1);
            var m2 = JsonSerializer.DeserializeFromString<ModelWithComplexTypes>(s);
            JsConfig.Reset();

            Assert.IsNull(m2.DictionaryValue);
            Assert.IsNull(m2.ListValue);
            Assert.IsNull(m2.ArrayValue);
            Assert.IsNull(m2.NestedTypeValue);
            Assert.IsNull(m2.ByteArrayValue);
        }

        /// <summary>Can serialize nulls when null.</summary>
        [Test]
        public void Can_Serialize_NullsWhenNull()
        {
            var m1 = new ModelWithComplexTypes();

            JsConfig.IncludeNullValues = true;
            var s = JsonSerializer.SerializeToString(m1);
            var m2 = JsonSerializer.DeserializeFromString<ModelWithComplexTypes>(s);
            JsConfig.Reset();

            Assert.IsNull(m2.DictionaryValue);
            Assert.IsNull(m2.ListValue);
            Assert.IsNull(m2.ArrayValue);
            Assert.IsNull(m2.NestedTypeValue);
            Assert.IsNull(m2.ByteArrayValue);
        }
    }
}