using NUnit.Framework;
using System;

namespace NServiceKit.Text.Tests.JsonTests
{
    /// <summary>A conditional serialization tests.</summary>
    [TestFixture]
    public class ConditionalSerializationTests
    {
        /// <summary>Tests serialize respected.</summary>
        [Test]
        public void TestSerializeRespected()
        {
            var obj = new Foo { X = "abc", Z = "def" }; // don't touch Y...

            string json = JsonSerializer.SerializeToString(obj);
            Assert.That(json, Is.StringMatching("{\"X\":\"abc\",\"Z\":\"def\"}"));   
        }

        /// <summary>Tests serialize respected with inheritance.</summary>
        [Test]
        public void TestSerializeRespectedWithInheritance()
        {
            var obj = new SuperFoo { X = "abc", Z = "def", A =123, C = 456 }; // don't touch Y or B...

            string json = JsonSerializer.SerializeToString(obj);
            Assert.That(json, Is.StringMatching("{\"A\":123,\"C\":456,\"X\":\"abc\",\"Z\":\"def\"}"));
        }

        /// <summary>A foo.</summary>
        public class Foo
        {
            /// <summary>Gets or sets the x coordinate.</summary>
            /// <value>The x coordinate.</value>
            public string X { get; set; } // not conditional

            /// <summary>Gets or sets the y coordinate.</summary>
            /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
            /// <value>The y coordinate.</value>
            public string Y // conditional: never serialized
            {
                get { throw new NotImplementedException(); }
                set { throw new NotImplementedException(); }
            }

            /// <summary>Determine if we should serialize y coordinate.</summary>
            /// <returns>true if it succeeds, false if it fails.</returns>
            public bool ShouldSerializeY()
            {
                return false;
            }

            /// <summary>Gets or sets the z coordinate.</summary>
            /// <value>The z coordinate.</value>
            public string Z { get;set;} // conditional: always serialized

            /// <summary>Determine if we should serialize z coordinate.</summary>
            /// <returns>true if it succeeds, false if it fails.</returns>
            public bool ShouldSerializeZ()
            {
                return true;
            }
        }

        /// <summary>A super foo.</summary>
        public class SuperFoo : Foo
        {
            /// <summary>Gets or sets a.</summary>
            /// <value>a.</value>
            public int A { get; set; } // not conditional

            /// <summary>Gets or sets the b.</summary>
            /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
            /// <value>The b.</value>
            public int B // conditional: never serialized
            {
                get { throw new NotImplementedException(); }
                set { throw new NotImplementedException(); }
            }

            /// <summary>Determine if we should serialize b.</summary>
            /// <returns>true if it succeeds, false if it fails.</returns>
            public bool ShouldSerializeB()
            {
                return false;
            }

            /// <summary>Gets or sets the c.</summary>
            /// <value>The c.</value>
            public int  C { get; set; } // conditional: always serialized

            /// <summary>Determine if we should serialize c.</summary>
            /// <returns>true if it succeeds, false if it fails.</returns>
            public bool ShouldSerializeC()
            {
                return true;
            }
        }
    }
}
