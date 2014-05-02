using System;
using NUnit.Framework;

namespace NServiceKit.Text.Tests.JsonTests
{
    /// <summary>A custom raw serializer tests.</summary>
    [TestFixture]
    public class CustomRawSerializerTests
    {
        /// <summary>Tests fixture tear down.</summary>
        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            JsConfig.Reset();
        }

        /// <summary>A real type.</summary>
        public class RealType
        {
            /// <summary>Gets or sets the name.</summary>
            /// <value>The name.</value>
            public string Name { get; set; }

            /// <summary>Gets or sets the data.</summary>
            /// <value>The data.</value>
            public byte[] Data { get; set; }
        }

        /// <summary>Can serialize type properties with custom function.</summary>
        [Test]
        public void Can_Serialize_TypeProperties_WithCustomFunction()
        {
            var test = new RealType { Name = "Test", Data = new byte[] { 1, 2, 3, 4, 5 } };

            // Act: now we set a custom function for byte[]
            JsConfig<byte[]>.RawSerializeFn = c =>
                {
                    var temp = new int[c.Length];
                    Array.Copy(c, temp, c.Length);
                    return JsonSerializer.SerializeToString(temp);
                };
            var json = JsonSerializer.SerializeToString(test);

            // Assert:
            Assert.That(json, Is.EquivalentTo("{\"Name\":\"Test\",\"Data\":[1,2,3,4,5]}"));
        }

        /// <summary>Can serialize anonymous type properties with custom function.</summary>
        [Test]
        public void Can_Serialize_AnonymousTypeProperties_WithCustomFunction()
        {
            var test = new { Name = "Test", Data = new byte[] { 1, 2, 3, 4, 5 } };

            // Act: now we set a custom function for byte[]
            JsConfig<byte[]>.RawSerializeFn = c =>
                {
                    var temp = new int[c.Length];
                    Array.Copy(c, temp, c.Length);
                    return JsonSerializer.SerializeToString(temp);
                };
            var json = JsonSerializer.SerializeToString(test);

            // Assert:
            Assert.That(json, Is.EquivalentTo("{\"Name\":\"Test\",\"Data\":[1,2,3,4,5]}"));
        }

        /// <summary>Resets the should clear js configuration t cached functions.</summary>
        [Test]
        public void Reset_ShouldClear_JsConfigT_CachedFunctions()
        {
            var test = new { Name = "Test", Data = new byte[] { 1, 2, 3, 4, 5 } };
            JsConfig<byte[]>.RawSerializeFn = c =>
                {
                    var temp = new int[c.Length];
                    Array.Copy(c, temp, c.Length);
                    return JsonSerializer.SerializeToString(temp);
                };
            var json = JsonSerializer.SerializeToString(test);

            Assert.That(json, Is.EquivalentTo("{\"Name\":\"Test\",\"Data\":[1,2,3,4,5]}"));
            // Act: now we set a custom function for byte[]
            JsConfig.Reset();
            json = JsonSerializer.SerializeToString(test);
            // Assert:
            Assert.That(json, Is.EquivalentTo("{\"Name\":\"Test\",\"Data\":\"AQIDBAU=\"}"));
        }        
    }
}