using System.Collections.Generic;
using NUnit.Framework;

namespace NServiceKit.Text.Tests.JsonTests
{
    /// <summary>A JSON array object tests.</summary>
    [TestFixture]
    public class JsonArrayObjectTests
    {
        /// <summary>Can serialize int array.</summary>
		[Test]
		public void Can_serialize_int_array() 
		{
			var array = new [] {1,2};
			Assert.That(JsonSerializer.SerializeToString(array), Is.EqualTo("[1,2]"));
		}

        /// <summary>Can parse empty array.</summary>
        [Test]
        public void Can_parse_empty_array()
        {
            Assert.That(JsonArrayObjects.Parse("[]"), Is.Empty);
        }

        /// <summary>Can parse empty array with tab.</summary>
        [Test]
        public void Can_parse_empty_array_with_tab()
        {
            Assert.That(JsonArrayObjects.Parse("[\t]"), Is.Empty);
        }

        /// <summary>Can parse array with null.</summary>
        [Test]
        public void Can_parse_array_with_null()
        {
            Assert.That(JsonArrayObjects.Parse("[null]"), Is.EqualTo(new string[]{null}));
        }

        /// <summary>Can parse array with nulls.</summary>
        [Test]
        public void Can_parse_array_with_nulls()
        {
            Assert.That(JsonArrayObjects.Parse("[null,null]"), Is.EqualTo(new string[]{null, null}));
        }

        /// <summary>Can parse empty array with whitespaces.</summary>
        [Test]
        public void Can_parse_empty_array_with_whitespaces()
        {
            Assert.That(JsonArrayObjects.Parse("[    ]"), Is.Empty);
            Assert.That(JsonArrayObjects.Parse("[\n\n]"), Is.Empty);
            Assert.That(JsonArrayObjects.Parse("[\t\t]"), Is.Empty);
        }

        /// <summary>Can parse empty array with mixed whitespaces.</summary>
        [Test]
        public void Can_parse_empty_array_with_mixed_whitespaces()
        {
            Assert.That(JsonArrayObjects.Parse("[ \n\t  \n\r]"), Is.Empty);
        }

        /// <summary>The names test.</summary>
        public class NamesTest
        {
            /// <summary>
            /// Initializes a new instance of the
            /// NServiceKit.Text.Tests.JsonTests.JsonArrayObjectTests.NamesTest class.
            /// </summary>
            /// <param name="names">The names.</param>
            public NamesTest(List<string> names)
            {
                Names = names;
            }

            /// <summary>Gets or sets the names.</summary>
            /// <value>The names.</value>
            public List<string> Names { get; set; }
        }

        /// <summary>Can parse empty array in dto with tab.</summary>
        [Test]
        public void Can_parse_empty_array_in_dto_with_tab()
        {
            var prettyJson = "{\"Names\":[\t]}";
            var oPretty = prettyJson.FromJson<NamesTest>();
            Assert.That(oPretty.Names.Count, Is.EqualTo(0));
        }
    }
}
