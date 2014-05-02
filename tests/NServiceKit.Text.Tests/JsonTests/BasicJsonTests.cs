using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using NUnit.Framework;
using NServiceKit.Text.Json;


#if !MONOTOUCH
using NServiceKit.Common.Tests.Models;
#endif

namespace NServiceKit.Text.Tests.JsonTests
{
    /// <summary>A basic JSON tests.</summary>
	[TestFixture]
	public class BasicJsonTests
		: TestBase
	{
        /// <summary>A JSON primitives.</summary>
		public class JsonPrimitives
		{
            /// <summary>Gets or sets the int.</summary>
            /// <value>The int.</value>
			public int Int { get; set; }

            /// <summary>Gets or sets the long.</summary>
            /// <value>The long.</value>
			public long Long { get; set; }

            /// <summary>Gets or sets the float.</summary>
            /// <value>The float.</value>
			public float Float { get; set; }

            /// <summary>Gets or sets the double.</summary>
            /// <value>The double.</value>
			public double Double { get; set; }

            /// <summary>Gets or sets a value indicating whether the boolean.</summary>
            /// <value>true if boolean, false if not.</value>
			public bool Boolean { get; set; }

            /// <summary>Gets or sets the date time.</summary>
            /// <value>The date time.</value>
			public DateTime DateTime { get; set; }

            /// <summary>Gets or sets the null string.</summary>
            /// <value>The null string.</value>
			public string NullString { get; set; }

            /// <summary>Creates a new JsonPrimitives.</summary>
            /// <param name="i">Zero-based index of the.</param>
            /// <returns>The JsonPrimitives.</returns>
			public static JsonPrimitives Create(int i)
			{
				return new JsonPrimitives
				{
					Int = i,
					Long = i,
					Float = i,
					Double = i,
					Boolean = i % 2 == 0,
					DateTime = DateTimeExtensions.FromUnixTimeMs(1),
				};
			}
		}

        /// <summary>A nullable value types.</summary>
        public class NullableValueTypes
        {
            /// <summary>Gets or sets the int.</summary>
            /// <value>The int.</value>
            public int? Int { get; set; }

            /// <summary>Gets or sets the long.</summary>
            /// <value>The long.</value>
            public long? Long { get; set; }

            /// <summary>Gets or sets the decimal.</summary>
            /// <value>The decimal.</value>
            public decimal? Decimal { get; set; }

            /// <summary>Gets or sets the double.</summary>
            /// <value>The double.</value>
            public double? Double { get; set; }

            /// <summary>Gets or sets the boolean.</summary>
            /// <value>The boolean.</value>
            public bool? Boolean { get; set; }

            /// <summary>Gets or sets the date time.</summary>
            /// <value>The date time.</value>
            public DateTime? DateTime { get; set; }
        }

        /// <summary>Setups this object.</summary>
		[SetUp]
		public void Setup ()
		{
#if MONOTOUCH
			JsConfig.Reset();
			JsConfig.RegisterTypeForAot<ExampleEnumWithoutFlagsAttribute>();
			JsConfig.RegisterTypeForAot<ExampleEnum>();
#endif
		}

        /// <summary>Tear down.</summary>
        [TearDown]
        public void TearDown()
        {
            JsConfig.Reset();
        }

        /// <summary>Can parse JSON with nullable valuetypes.</summary>
	    [Test]
	    public void Can_parse_json_with_nullable_valuetypes()
	    {
	        var json = "{}";

            var item = JsonSerializer.DeserializeFromString<NullableValueTypes>(json);

            Assert.That(item.Int, Is.Null, "int");
            Assert.That(item.Long, Is.Null, "long");
            Assert.That(item.Decimal, Is.Null, "decimal");
            Assert.That(item.Double, Is.Null, "double");
            Assert.That(item.Boolean, Is.Null, "boolean");
            Assert.That(item.DateTime, Is.Null, "datetime");
	    }

        /// <summary>Can parse JSON with nullable valuetypes that has included null values.</summary>
        [Test]
        public void Can_parse_json_with_nullable_valuetypes_that_has_included_null_values()
        {
            var json = "{\"Int\":null,\"Long\":null,\"Decimal\":null,\"Double\":null,\"Boolean\":null,\"DateTime\":null}";

            var item = JsonSerializer.DeserializeFromString<NullableValueTypes>(json);

            Assert.That(item.Int, Is.Null, "int");
            Assert.That(item.Long, Is.Null, "long");
            Assert.That(item.Decimal, Is.Null, "decimal");
            Assert.That(item.Double, Is.Null, "double");
            Assert.That(item.Boolean, Is.Null, "boolean");
            Assert.That(item.DateTime, Is.Null, "datetime");
        }

        /// <summary>Can parse JSON with nulls or empty string in nullables.</summary>
		[Test]
		public void Can_parse_json_with_nulls_or_empty_string_in_nullables()
		{
			const string json = "{\"Int\":null,\"Boolean\":\"\"}";
			var value = JsonSerializer.DeserializeFromString<NullableValueTypes>(json);

			Assert.That(value.Int, Is.EqualTo(null));
			Assert.That(value.Boolean, Is.EqualTo(null));
		}

        /// <summary>Can parse JSON with nullable valuetypes that has no value specified.</summary>
        [Test]
        public void Can_parse_json_with_nullable_valuetypes_that_has_no_value_specified()
        {
            var json = "{\"Int\":,\"Long\":,\"Decimal\":,\"Double\":,\"Boolean\":,\"DateTime\":}";

            var item = JsonSerializer.DeserializeFromString<NullableValueTypes>(json);

            Assert.That(item.Int, Is.Null, "int");
            Assert.That(item.Long, Is.Null, "long");
            Assert.That(item.Decimal, Is.Null, "decimal");
            Assert.That(item.Double, Is.Null, "double");
            Assert.That(item.Boolean, Is.Null, "boolean");
            Assert.That(item.DateTime, Is.Null, "datetime");
        }

        /// <summary>Can parse mixed list nulls.</summary>
        [Test]
        public void Can_parse_mixed_list_nulls()
        {
            Assert.That(JsonSerializer.DeserializeFromString<List<string>>("[\"abc\",null,\"cde\",null]"), 
                Is.EqualTo(new string[] { "abc", null, "cde", null }));
        }

        /// <summary>Can parse mixed enumarable nulls.</summary>
        [Test]
        public void Can_parse_mixed_enumarable_nulls()
        {
            Assert.That(JsonSerializer.DeserializeFromString<IEnumerable<string>>("[\"abc\",null,\"cde\",null]"),
                Is.EqualTo(new string[] { "abc", null, "cde", null }));
        }

        /// <summary>Can handle JSON primitives.</summary>
		[Test]
		public void Can_handle_json_primitives()
		{
			var json = JsonSerializer.SerializeToString(JsonPrimitives.Create(1));
			Log(json);

			Assert.That(json, Is.EqualTo(
				"{\"Int\":1,\"Long\":1,\"Float\":1,\"Double\":1,\"Boolean\":false,\"DateTime\":\"\\/Date(1)\\/\"}"));
		}

        /// <summary>Can parse JSON with nulls.</summary>
		[Test]
		public void Can_parse_json_with_nulls()
		{
			const string json = "{\"Int\":1,\"NullString\":null}";
			var value = JsonSerializer.DeserializeFromString<JsonPrimitives>(json);

			Assert.That(value.Int, Is.EqualTo(1));
			Assert.That(value.NullString, Is.Null);
		}

        /// <summary>Can serialize dictionary of int.</summary>
		[Test]
		public void Can_serialize_dictionary_of_int_int()
		{
			var json = JsonSerializer.SerializeToString<IntIntDictionary>(new IntIntDictionary() { Dictionary = { { 10, 100 }, { 20, 200 } } });
			const string expected = "{\"Dictionary\":{\"10\":100,\"20\":200}}";
			Assert.That(json, Is.EqualTo(expected));
		}

        /// <summary>Dictionary of ints.</summary>
		private class IntIntDictionary
		{
            /// <summary>
            /// Initializes a new instance of the
            /// NServiceKit.Text.Tests.JsonTests.BasicJsonTests.IntIntDictionary class.
            /// </summary>
			public IntIntDictionary()
			{
				Dictionary = new Dictionary<int, int>();
			}

            /// <summary>Gets or sets the dictionary.</summary>
            /// <value>The dictionary.</value>
			public IDictionary<int, int> Dictionary { get; set; }
		}

        /// <summary>Serialize skips null values by default.</summary>
		[Test]
		public void Serialize_skips_null_values_by_default()
		{
			var o = new NullValueTester
			{
				Name = "Brandon",
				Type = "Programmer",
				SampleKey = 12,
				Nothing = (string)null,
				NullableDateTime = null
			};

			var s = JsonSerializer.SerializeToString(o);
			Assert.That(s, Is.EqualTo("{\"Name\":\"Brandon\",\"Type\":\"Programmer\",\"SampleKey\":12}"));
		}

        /// <summary>Serialize can include null values.</summary>
		[Test]
		public void Serialize_can_include_null_values()
		{
			var o = new NullValueTester
			{
				Name = "Brandon",
				Type = "Programmer",
				SampleKey = 12,
				Nothing = null,
				NullClass = null,
				NullableDateTime = null,
			};

			JsConfig.IncludeNullValues = true;
			var s = JsonSerializer.SerializeToString(o);
			JsConfig.Reset();
			Assert.That(s, Is.EqualTo("{\"Name\":\"Brandon\",\"Type\":\"Programmer\",\"SampleKey\":12,\"Nothing\":null,\"NullClass\":null,\"NullableDateTime\":null}"));
		}

        /// <summary>A null class.</summary>
		private class NullClass
		{

		}

        /// <summary>Deserialize sets null values.</summary>
		[Test]
		public void Deserialize_sets_null_values()
		{
			var s = "{\"Name\":\"Brandon\",\"Type\":\"Programmer\",\"SampleKey\":12,\"Nothing\":null}";
			var o = JsonSerializer.DeserializeFromString<NullValueTester>(s);
			Assert.That(o.Name, Is.EqualTo("Brandon"));
			Assert.That(o.Type, Is.EqualTo("Programmer"));
			Assert.That(o.SampleKey, Is.EqualTo(12));
			Assert.That(o.Nothing, Is.Null);
		}

        /// <summary>Deserialize ignores omitted values.</summary>
		[Test]
		public void Deserialize_ignores_omitted_values()
		{
			var s = "{\"Type\":\"Programmer\",\"SampleKey\":2}";
			var o = JsonSerializer.DeserializeFromString<NullValueTester>(s);
			Assert.That(o.Name, Is.EqualTo("Miguel"));
			Assert.That(o.Type, Is.EqualTo("Programmer"));
			Assert.That(o.SampleKey, Is.EqualTo(2));
			Assert.That(o.Nothing, Is.EqualTo("zilch"));
		}

        /// <summary>A null value tester.</summary>
		private class NullValueTester
		{
            /// <summary>Gets or sets the name.</summary>
            /// <value>The name.</value>
			public string Name
			{
				get;
				set;
			}

            /// <summary>Gets or sets the type.</summary>
            /// <value>The type.</value>
			public string Type
			{
				get;
				set;
			}

            /// <summary>Gets or sets the sample key.</summary>
            /// <value>The sample key.</value>
			public int SampleKey
			{
				get;
				set;
			}

            /// <summary>Gets or sets the nothing.</summary>
            /// <value>The nothing.</value>
			public string Nothing
			{
				get;
				set;
			}

            /// <summary>Gets or sets the null class.</summary>
            /// <value>The null class.</value>
			public NullClass NullClass { get; set; }

            /// <summary>Gets or sets the nullable date time.</summary>
            /// <value>The nullable date time.</value>
			public DateTime? NullableDateTime { get; set; }

            /// <summary>
            /// Initializes a new instance of the
            /// NServiceKit.Text.Tests.JsonTests.BasicJsonTests.NullValueTester class.
            /// </summary>
			public NullValueTester()
			{
				Name = "Miguel";
				Type = "User";
				SampleKey = 1;
				Nothing = "zilch";
				NullableDateTime = new DateTime(2012, 01, 01);
			}
		}

#if !MONOTOUCH
        /// <summary>A person.</summary>
		[DataContract]
		class Person
		{
            /// <summary>Gets or sets the identifier.</summary>
            /// <value>The identifier.</value>
			[DataMember(Name = "MyID")]
			public int Id { get; set; }

            /// <summary>Gets or sets the name.</summary>
            /// <value>The name.</value>
			[DataMember]
			public string Name { get; set; }
		}

        /// <summary>Can override name.</summary>
		[Test]
		public void Can_override_name()
		{
			var person = new Person
			{
				Id = 123,
				Name = "Abc"
			};

			Assert.That(TypeSerializer.SerializeToString(person), Is.EqualTo("{MyID:123,Name:Abc}"));
			Assert.That(JsonSerializer.SerializeToString(person), Is.EqualTo("{\"MyID\":123,\"Name\":\"Abc\"}"));
		}
#endif

#if !MONOTOUCH
        /// <summary>A person dto.</summary>
        [DataContract]
        class PersonDTO
        {
            /// <summary>The identifier.</summary>
            [DataMember]
            public int Id;

            /// <summary>Gets or sets the name.</summary>
            /// <value>The name.</value>
            [DataMember]
            public string Name { get; set; }
        }

        /// <summary>Should honor datamember attribute.</summary>
        [Test]
        public void Should_honor_datamember_attribute()
        {
            var person = new PersonDTO
            {
                Id = 123,
                Name = "Abc"
            };

            Assert.That(TypeSerializer.SerializeToString(person), Is.EqualTo("{Id:123,Name:Abc}"));
            Assert.That(JsonSerializer.SerializeToString(person), Is.EqualTo("{\"Id\":123,\"Name\":\"Abc\"}"));
        }
#endif

        /// <summary>Bitfield of flags for specifying ExampleEnum.</summary>
        [Flags]
        public enum ExampleEnum : ulong
        {
            /// <summary>A binary constant representing the none flag.</summary>
            None = 0,

            /// <summary>A binary constant representing the one flag.</summary>
            One = 1,

            /// <summary>A binary constant representing the two flag.</summary>
            Two = 2,

            /// <summary>A binary constant representing the four flag.</summary>
            Four = 4,

            /// <summary>A binary constant representing the eight flag.</summary>
            Eight = 8
        }

        /// <summary>Can serialize unsigned flags enum.</summary>
        [Test]
        public void Can_serialize_unsigned_flags_enum()
        {
            var anon = new
            {
                EnumProp1 = ExampleEnum.One | ExampleEnum.Two,
                EnumProp2 = ExampleEnum.Eight,
            };

            Assert.That(TypeSerializer.SerializeToString(anon), Is.EqualTo("{EnumProp1:3,EnumProp2:8}"));
            Assert.That(JsonSerializer.SerializeToString(anon), Is.EqualTo("{\"EnumProp1\":3,\"EnumProp2\":8}"));
        }

        /// <summary>Values that represent ExampleEnumWithoutFlagsAttribute.</summary>
        public enum ExampleEnumWithoutFlagsAttribute : ulong
        {
            /// <summary>An enum constant representing the none option.</summary>
            None = 0,

            /// <summary>An enum constant representing the one option.</summary>
            One = 1,

            /// <summary>An enum constant representing the two option.</summary>
            Two = 2
        }

        /// <summary>Attribute for class with enum without flags.</summary>
        public class ClassWithEnumWithoutFlagsAttribute
        {
            /// <summary>Gets or sets the enum property 1.</summary>
            /// <value>The enum property 1.</value>
            public ExampleEnumWithoutFlagsAttribute EnumProp1 { get; set; }

            /// <summary>Gets or sets the enum property 2.</summary>
            /// <value>The enum property 2.</value>
            public ExampleEnumWithoutFlagsAttribute EnumProp2 { get; set; }
        }

        /// <summary>Attribute for class with nullable enum without flags.</summary>
        public class ClassWithNullableEnumWithoutFlagsAttribute
        {
            /// <summary>Gets or sets the enum property 1.</summary>
            /// <value>The enum property 1.</value>
            public ExampleEnumWithoutFlagsAttribute ? EnumProp1 { get; set; }
        }

        /// <summary>Can serialize unsigned enum with turned on treat enum as integer.</summary>
        [Test]
        public void Can_serialize_unsigned_enum_with_turned_on_TreatEnumAsInteger()
        {
            JsConfig.TreatEnumAsInteger = true;

            var anon = new ClassWithEnumWithoutFlagsAttribute
            {
                EnumProp1 = ExampleEnumWithoutFlagsAttribute.One,
                EnumProp2 = ExampleEnumWithoutFlagsAttribute.Two
            };

            Assert.That(JsonSerializer.SerializeToString(anon), Is.EqualTo("{\"EnumProp1\":1,\"EnumProp2\":2}"));
			Assert.That(TypeSerializer.SerializeToString(anon), Is.EqualTo("{EnumProp1:1,EnumProp2:2}"));
		}

        /// <summary>Can serialize nullable enum with turned on treat enum as integer.</summary>
        [Test]
        public void Can_serialize_nullable_enum_with_turned_on_TreatEnumAsInteger()
        {
            JsConfig.TreatEnumAsInteger = true;

            var anon = new ClassWithNullableEnumWithoutFlagsAttribute
            {
                EnumProp1 = ExampleEnumWithoutFlagsAttribute.One
            };

            Assert.That(JsonSerializer.SerializeToString(anon), Is.EqualTo("{\"EnumProp1\":1}"));
        }

        /// <summary>Can deserialize unsigned enum with turned on treat enum as integer.</summary>
        [Test]
        public void Can_deserialize_unsigned_enum_with_turned_on_TreatEnumAsInteger()
        {
            JsConfig.TreatEnumAsInteger = true;

            var s = "{\"EnumProp1\":1,\"EnumProp2\":2}";
            var o = JsonSerializer.DeserializeFromString<ClassWithEnumWithoutFlagsAttribute>(s);

            Assert.That(o.EnumProp1, Is.EqualTo(ExampleEnumWithoutFlagsAttribute.One));
            Assert.That(o.EnumProp2, Is.EqualTo(ExampleEnumWithoutFlagsAttribute.Two));
        }

        /// <summary>Can serialize object array with nulls.</summary>
        [Test]
        public void Can_serialize_object_array_with_nulls()
        {
            var objs = new[] { (object)"hello", (object)null };
            JsConfig.IncludeNullValues = false;

            Assert.That(objs.ToJson(), Is.EqualTo("[\"hello\",null]"));
        }

        /// <summary>Should return null instance for empty JSON.</summary>
        [Test]
        public void Should_return_null_instance_for_empty_json()
        {
            var o = JsonSerializer.DeserializeFromString("", typeof(JsonPrimitives));
            Assert.IsNull(o);
        }

        /// <summary>Can parse empty string dictionary with leading whitespace.</summary>
        [Test]
        public void Can_parse_empty_string_dictionary_with_leading_whitespace()
        {
            var serializer = new JsonSerializer<Dictionary<string, string>>();
            Assert.That(serializer.DeserializeFromString(" {}"), Is.Empty);
        }

        /// <summary>Can parse nonempty string dictionary with leading whitespace.</summary>
        [Test]
        public void Can_parse_nonempty_string_dictionary_with_leading_whitespace()
        {
            var serializer = new JsonSerializer<Dictionary<string, string>>();
            var dictionary = serializer.DeserializeFromString(" {\"A\":\"N\",\"B\":\"O\"}");
            Assert.That(dictionary.Count, Is.EqualTo(2));
            Assert.That(dictionary["A"], Is.EqualTo("N"));
            Assert.That(dictionary["B"], Is.EqualTo("O"));
        }

        /// <summary>Can parse empty dictionary with leading whitespace.</summary>
        [Test]
        public void Can_parse_empty_dictionary_with_leading_whitespace()
        {
            var serializer = new JsonSerializer<Dictionary<int, double>>();
            Assert.That(serializer.DeserializeFromString(" {}"), Is.Empty);
        }

        /// <summary>Can parse nonempty dictionary with leading whitespace.</summary>
        [Test]
        public void Can_parse_nonempty_dictionary_with_leading_whitespace()
        {
            var serializer = new JsonSerializer<Dictionary<int, double>>();
            var dictionary = serializer.DeserializeFromString(" {\"1\":2.5,\"2\":5}");
            Assert.That(dictionary.Count, Is.EqualTo(2));
            Assert.That(dictionary[1], Is.EqualTo(2.5));
            Assert.That(dictionary[2], Is.EqualTo(5.0));
        }

        /// <summary>Can parse empty hashtable with leading whitespace.</summary>
        [Test]
        public void Can_parse_empty_hashtable_with_leading_whitespace()
        {
            var serializer = new JsonSerializer<Hashtable>();
            Assert.That(serializer.DeserializeFromString(" {}"), Is.Empty);
        }

        /// <summary>Can parse nonempty hashtable with leading whitespace.</summary>
        [Test]
        public void Can_parse_nonempty_hashtable_with_leading_whitespace()
        {
            var serializer = new JsonSerializer<Hashtable>();
            var hashtable = serializer.DeserializeFromString(" {\"A\":1,\"B\":2}");
            Assert.That(hashtable.Count, Is.EqualTo(2));
            Assert.That(hashtable["A"].ToString(), Is.EqualTo(1.ToString()));
            Assert.That(hashtable["B"].ToString(), Is.EqualTo(2.ToString()));
        }

        /// <summary>Can parse empty JSON object with leading whitespace.</summary>
        [Test]
        public void Can_parse_empty_json_object_with_leading_whitespace()
        {
            var serializer = new JsonSerializer<JsonObject>();
            Assert.That(serializer.DeserializeFromString(" {}"), Is.Empty);
        }

        /// <summary>Can parse nonempty JSON object with leading whitespace.</summary>
        [Test]
        public void Can_parse_nonempty_json_object_with_leading_whitespace()
        {
            var serializer = new JsonSerializer<JsonObject>();
            var jsonObject = serializer.DeserializeFromString(" {\"foo\":\"bar\"}");
            Assert.That(jsonObject, Is.Not.Empty);
            Assert.That(jsonObject["foo"], Is.EqualTo("bar"));
        }

        /// <summary>Can parse empty key value pair with leading whitespace.</summary>
        [Test]
        public void Can_parse_empty_key_value_pair_with_leading_whitespace()
        {
            var serializer = new JsonSerializer<KeyValuePair<string, string>>();
            Assert.That(serializer.DeserializeFromString(" {}"), Is.EqualTo(default(KeyValuePair<string, string>)));
        }

        /// <summary>Can parse nonempty key value pair with leading whitespace.</summary>
        [Test]
        public void Can_parse_nonempty_key_value_pair_with_leading_whitespace()
        {
            var serializer = new JsonSerializer<KeyValuePair<string, string>>();
            var keyValuePair = serializer.DeserializeFromString(" {\"Key\":\"foo\",\"Value\":\"bar\"}");
            Assert.That(keyValuePair, Is.EqualTo(new KeyValuePair<string, string>("foo", "bar")));
        }

        /// <summary>Can parse empty object with leading whitespace.</summary>
        [Test]
        public void Can_parse_empty_object_with_leading_whitespace()
        {
            var serializer = new JsonSerializer<Foo>();
            var foo = serializer.DeserializeFromString(" {}");
            Assert.That(foo, Is.Not.Null);
            Assert.That(foo.Bar, Is.Null);
        }

        /// <summary>Can parse nonempty object with leading whitespace.</summary>
        [Test]
        public void Can_parse_nonempty_object_with_leading_whitespace()
        {
            var serializer = new JsonSerializer<Foo>();
            var foo = serializer.DeserializeFromString(" {\"Bar\":\"baz\"}");
            Assert.That(foo, Is.Not.Null);
            Assert.That(foo.Bar, Is.EqualTo("baz"));
        }

        /// <summary>A foo.</summary>
        public class Foo
        {
            /// <summary>Gets or sets the bar.</summary>
            /// <value>The bar.</value>
            public string Bar { get; set; }
        }
	}
}
