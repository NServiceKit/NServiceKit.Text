using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;
using NServiceKit.Common.Extensions;
using NServiceKit.Common.Tests.Models;

namespace NServiceKit.Text.Tests.JsonTests
{
    /// <summary>An escaped characters tests.</summary>
	[TestFixture]
	public class EscapedCharsTests
		: TestBase
	{
        /// <summary>A data Model for the nested.</summary>
		public class NestedModel
		{
            /// <summary>Gets or sets the identifier.</summary>
            /// <value>The identifier.</value>
			public string Id { get; set; }

            /// <summary>Gets or sets the model.</summary>
            /// <value>The model.</value>
			public ModelWithIdAndName Model { get; set; }
		}

        /// <summary>Tear down.</summary>
        [TearDown]
        public void TearDown()
        {
            JsConfig.Reset();
        }

        /// <summary>Can deserialize text with escaped characters.</summary>
		[Test]
		public void Can_deserialize_text_with_escaped_chars()
		{
			var model = new ModelWithIdAndName
			{
				Id = 1,
				Name = @"1 \ 2 \r 3 \n 4 \b 5 \f 6 """
			};

			SerializeAndCompare(model);
		}

        /// <summary>Can short circuit string with no escape characters.</summary>
		[Test]
		public void Can_short_circuit_string_with_no_escape_chars()
		{
			var model = new ModelWithIdAndName
			{
				Id = 1,
				Name = @"Simple string"
			};

			SerializeAndCompare(model);
		}

        /// <summary>Can deserialize JSON with whitespace.</summary>
		[Test]
		public void Can_deserialize_json_with_whitespace()
		{
			var model = new ModelWithIdAndName
			{
				Id = 1,
				Name = @"Simple string"
			};

			const string json = "\t { \t \"Id\" \t : 1 , \t \"Name\" \t  : \t \"Simple string\" \t } \t ";

			var fromJson = JsonSerializer.DeserializeFromString<ModelWithIdAndName>(json);

			Assert.That(fromJson, Is.EqualTo(model));
		}

        /// <summary>An inner.</summary>
        public class Inner
        {
            /// <summary>Gets or sets the int.</summary>
            /// <value>The int.</value>
            public int Int { get; set; }
        }

        /// <summary>A program.</summary>
        public class Program
        {
            /// <summary>Gets or sets the inner.</summary>
            /// <value>The inner.</value>
            public Inner[] Inner { get; set; }
        }

        /// <summary>Can deserialize inner whitespace.</summary>
	    [Test]
	    public void Can_deserialize_inner_whitespace()
	    {
            var fromJson = JsonSerializer.DeserializeFromString<Program>("{\"Inner\":[{\"Int\":0} , {\"Int\":1}\r\n]}");
            Assert.That(fromJson.Inner.Length, Is.EqualTo(2));
            Assert.That(fromJson.Inner[0].Int, Is.EqualTo(0));
            Assert.That(fromJson.Inner[1].Int, Is.EqualTo(1));
            
            var dto = new Program { Inner = new[] { new Inner { Int = 0 } } };
	        Serialize(dto);
            var json = JsonSerializer.SerializeToString(dto);
            Assert.That(json, Is.EqualTo(@"{""Inner"":[{""Int"":0}]}"));
        }

        /// <summary>Can deserialize nested JSON with whitespace.</summary>
		[Test]
		public void Can_deserialize_nested_json_with_whitespace()
		{
			var model = new NestedModel
			{
				Id = "Nested with space",
				Model = new ModelWithIdAndName
				{
					Id = 1,
					Name = @"Simple string"
				}
			};

			const string json = "\t { \"Id\" : \"Nested with space\" \n , \r \t \"Model\" \t : \n { \t \"Id\" \t : 1 , \t \"Name\" \t  : \t \"Simple string\" \t } \t } \n ";

			var fromJson = JsonSerializer.DeserializeFromString<NestedModel>(json);

			Assert.That(fromJson.Id, Is.EqualTo(model.Id));
			Assert.That(fromJson.Model, Is.EqualTo(model.Model));
		}

        /// <summary>List of model withs.</summary>
		public class ModelWithList
		{
            /// <summary>
            /// Initializes a new instance of the
            /// NServiceKit.Text.Tests.JsonTests.EscapedCharsTests.ModelWithList class.
            /// </summary>
			public ModelWithList()
			{
				this.StringList = new List<string>();
			}

            /// <summary>Gets or sets the identifier.</summary>
            /// <value>The identifier.</value>
			public int Id { get; set; }

            /// <summary>Gets or sets a list of strings.</summary>
            /// <value>A List of strings.</value>
            public List<string> StringList { get; set; }

            /// <summary>Gets or sets an array of strings.</summary>
            /// <value>An Array of strings.</value>
            public string[] StringArray { get; set; }

            /// <summary>
            /// Determines whether the specified <see cref="T:System.Object" /> is equal to the current
            /// <see cref="T:System.Object" />.
            /// </summary>
            /// <param name="other">The model with list to compare to this object.</param>
            /// <returns>
            /// true if the specified <see cref="T:System.Object" /> is equal to the current
            /// <see cref="T:System.Object" />; otherwise, false.
            /// </returns>
			public bool Equals(ModelWithList other)
			{
				if (ReferenceEquals(null, other)) return false;
				if (ReferenceEquals(this, other)) return true;
				return other.Id == Id && StringList.EquivalentTo(other.StringList);
			}

            /// <summary>
            /// Determines whether the specified <see cref="T:System.Object" /> is equal to the current
            /// <see cref="T:System.Object" />.
            /// </summary>
            /// <param name="obj">The <see cref="T:System.Object" /> to compare with the current
            /// <see cref="T:System.Object" />.</param>
            /// <returns>
            /// true if the specified <see cref="T:System.Object" /> is equal to the current
            /// <see cref="T:System.Object" />; otherwise, false.
            /// </returns>
			public override bool Equals(object obj)
			{
				if (ReferenceEquals(null, obj)) return false;
				if (ReferenceEquals(this, obj)) return true;
				if (obj.GetType() != typeof(ModelWithList)) return false;
				return Equals((ModelWithList)obj);
			}

            /// <summary>Serves as a hash function for a particular type.</summary>
            /// <returns>A hash code for the current <see cref="T:System.Object" />.</returns>
			public override int GetHashCode()
			{
				unchecked
				{
					return (Id * 397) ^ (StringList != null ? StringList.GetHashCode() : 0);
				}
			}
		}

        /// <summary>Can serialize model with array.</summary>
        [Test]
        public void Can_serialize_Model_with_array()
        {
            var model = new ModelWithList {
                Id = 1,
                StringArray = new[]{ "One", "Two", "Three" }
            };

            SerializeAndCompare(model);
        }

        /// <summary>Can serialize model with list.</summary>
        [Test]
        public void Can_serialize_Model_with_list()
        {
            var model = new ModelWithList {
                Id = 1,
                StringList = { "One", "Two", "Three" }
            };

            SerializeAndCompare(model);
        }

        /// <summary>Can serialize model with array of escape characters.</summary>
        [Test]
        public void Can_serialize_Model_with_array_of_escape_chars()
        {
            var model = new ModelWithList {
                Id = 1,
                StringArray = new[]{ @"1 \ 2 \r 3 \n 4 \b 5 \f 6 """, @"1 \ 2 \r 3 \n 4 \b 5 \f 6 """ }
            };

            SerializeAndCompare(model);
        }

        /// <summary>Can serialize model with list of escape characters.</summary>
        [Test]
        public void Can_serialize_Model_with_list_of_escape_chars()
        {
            var model = new ModelWithList {
                Id = 1,
                StringList = { @"1 \ 2 \r 3 \n 4 \b 5 \f 6 """, @"1 \ 2 \r 3 \n 4 \b 5 \f 6 """ }
            };

            SerializeAndCompare(model);
        }

        /// <summary>Can deserialize JSON list with whitespace.</summary>
		[Test]
		public void Can_deserialize_json_list_with_whitespace()
		{
			var model = new ModelWithList
			{
				Id = 1,
				StringList = { " One ", " Two " }
			};

			Log(JsonSerializer.SerializeToString(model));

			const string json = "\t { \"Id\" : 1 , \n \"StringList\" \t : \n [ \t \" One \" \t , \t \" Two \" \t ] \n } \t ";

			var fromJson = JsonSerializer.DeserializeFromString<ModelWithList>(json);

			Assert.That(fromJson, Is.EqualTo(model));
		}

        /// <summary>Can deserialize basic latin unicode.</summary>
		[Test]
		public void Can_deserialize_basic_latin_unicode()
		{
			const string json = "{\"Id\":1,\"Name\":\"\\u0041 \\u0042 \\u0043 | \\u0031 \\u0032 \\u0033\"}";

			var model = new ModelWithIdAndName { Id = 1, Name = "A B C | 1 2 3" };

			var fromJson = JsonSerializer.DeserializeFromString<ModelWithIdAndName>(json);

			Assert.That(fromJson, Is.EqualTo(model));
		}

        /// <summary>Can serialize unicode without escape.</summary>
        [Test]
        public void Can_serialize_unicode_without_escape()
        {
            var model = new Model { Name = "JříАбвĀašū" };
            var toJson = JsonSerializer.SerializeToString(model);
            Assert.That(toJson, Is.EqualTo("{\"Name\":\"JříАбвĀašū\"}"));
        }

        /// <summary>Can deserialize unicode without escape.</summary>
        [Test]
        public void Can_deserialize_unicode_without_escape()
        {
            var fromJson = JsonSerializer.DeserializeFromString<Model>("{\"Name\":\"JříАбвĀašū\"}");
            Assert.That(fromJson.Name, Is.EqualTo("JříАбвĀašū"));
        }

        /// <summary>Can serialize unicode with escape.</summary>
        [Test]
        public void Can_serialize_unicode_with_escape()
        {
            JsConfig.EscapeUnicode = true;
            var model = new Model { Name = "JříАбвĀašū" };
            var toJson = JsonSerializer.SerializeToString(model);
            Assert.That(toJson, Is.EqualTo("{\"Name\":\"J\\u0159\\u00ED\\u0410\\u0431\\u0432\\u0100a\\u0161\\u016B\"}"));
        }

        /// <summary>Can deserialize unicode with escape.</summary>
        [Test]
        public void Can_deserialize_unicode_with_escape()
        {
            JsConfig.EscapeUnicode = true;
            var fromJson = JsonSerializer.DeserializeFromString<Model>("{\"Name\":\"J\\u0159\\u00ED\\u0410\\u0431\\u0432\\u0100a\\u0161\\u016B\"}");
            Assert.That(fromJson.Name, Is.EqualTo("JříАбвĀašū"));
        }

        /// <summary>Can serialize array of control characters and unicode.</summary>
        [Test]
        public void Can_serialize_array_of_control_chars_and_unicode()
        {
            // we want to ensure control chars are escaped, but other unicode is fine to be serialized
            Assert.IsFalse(JsConfig.EscapeUnicode, "for this test, JsConfig.EscapeUnicode must be false");

            var array = new[] { ((char)0x18).ToString(), "Ω" };
            var json = JsonSerializer.SerializeToString(array);
            Assert.That(json, Is.EqualTo(@"[""\u0018"",""Ω""]"));
        }

        /// <summary>A model.</summary>
        public class Model
        {
            /// <summary>Gets or sets the name.</summary>
            /// <value>The name.</value>
            public string Name { get; set; }
        }
	}
}