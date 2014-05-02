using NServiceKit.Text.Tests.DynamicModels.DataModel;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace NServiceKit.Text.Tests
{
    /// <summary>A dictionary tests.</summary>
    [TestFixture]
    public class DictionaryTests
        : TestBase
    {
        /// <summary>Sets the up.</summary>
        [TestFixtureSetUp]
        public void SetUp()
        {
#if MONOTOUCH
			JsConfig.RegisterTypeForAot<Dictionary<string, int>> ();
			JsConfig.RegisterTypeForAot<KeyValuePair<int, string>> ();

			JsConfig.RegisterTypeForAot<KeyValuePair<string, int>> ();
			JsConfig.RegisterTypeForAot<Dictionary<string, int>> ();

			JsConfig.RegisterTypeForAot<KeyValuePair<string, Dictionary<string, int>>> ();
			JsConfig.RegisterTypeForAot<Dictionary<string, Dictionary<string, int>>> ();

			JsConfig.RegisterTypeForAot<KeyValuePair<int, Dictionary<string, int>>> ();
			JsConfig.RegisterTypeForAot<Dictionary<int, Dictionary<string, int>>> ();


#endif
        }

        /// <summary>Tear down.</summary>
        [TearDown]
        public void TearDown()
        {
            JsConfig.Reset();
        }

        /// <summary>Can serialize one level dictionary.</summary>
        [Test]
        public void Can_serialize_one_level_dictionary()
        {
            var map = new Dictionary<string, int>
          	{
				{"One", 1}, {"Two", 2}, {"Three", 3}, 
          	};

            Serialize(map);
        }

        /// <summary>Can serialize empty map.</summary>
        [Test]
        public void Can_serialize_empty_map()
        {
            var emptyMap = new Dictionary<string, int>();

            Serialize(emptyMap);
        }

        /// <summary>Can serialize empty string map.</summary>
        [Test]
        public void Can_serialize_empty_string_map()
        {
            var emptyMap = new Dictionary<string, string>();

            Serialize(emptyMap);
        }

        /// <summary>Can serialize two level dictionary.</summary>
        [Test]
        public void Can_serialize_two_level_dictionary()
        {
            var map = new Dictionary<string, Dictionary<string, int>>
          		{
					{"map1", new Dictionary<string, int>
			         	{
							{"One", 1}, {"Two", 2}, {"Three", 3}, 
			         	}
					},
					{"map2", new Dictionary<string, int>
			         	{
							{"Four", 4}, {"Five", 5}, {"Six", 6}, 
			         	}
					},
          		};

            Serialize(map);
        }

        /// <summary>Can serialize two level dictionary with int key.</summary>
        [Test]
        public void Can_serialize_two_level_dictionary_with_int_key()
        {
            var map = new Dictionary<int, Dictionary<string, int>>
          		{
					{1, new Dictionary<string, int>
			         	{
							{"One", 1}, {"Two", 2}, {"Three", 3}, 
			         	}
					},
					{2, new Dictionary<string, int>
			         	{
							{"Four", 4}, {"Five", 5}, {"Six", 6}, 
			         	}
					},
          		};

            Serialize(map);
        }

        /// <summary>Can deserialize two level dictionary with array.</summary>
        [Test]
        public void Can_deserialize_two_level_dictionary_with_array()
        {
            JsConfig.TryToParsePrimitiveTypeValues = true;
            JsConfig.ConvertObjectTypesIntoStringDictionary = true;
            var original = new Dictionary<string, StrictType[]>
          		{
					{"array", 
                        new [] { 
                            new StrictType { Name = "First" }, 
                            new StrictType { Name = "Second" }, 
                            new StrictType { Name = "Third" }, 
                        }
					},
          		};
            var json = JsonSerializer.SerializeToString(original);
            var deserialized = JsonSerializer.DeserializeFromString<Dictionary<string, object>>(json);

            Assert.That(deserialized, Is.Not.Null);
            Assert.That(deserialized["array"], Is.Not.Null);
            Assert.That(((List<object>)deserialized["array"]).Count, Is.EqualTo(3));
            Assert.That(((List<object>)deserialized["array"])[0].ToJson(), Is.EqualTo("{\"Name\":\"First\"}"));
            Assert.That(((List<object>)deserialized["array"])[1].ToJson(), Is.EqualTo("{\"Name\":\"Second\"}"));
            Assert.That(((List<object>)deserialized["array"])[2].ToJson(), Is.EqualTo("{\"Name\":\"Third\"}"));
        }

        /// <summary>Can deserialize dictionary with special characters in strings.</summary>
        [Test]
        public void Can_deserialize_dictionary_with_special_characters_in_strings()
        {
            JsConfig.TryToParsePrimitiveTypeValues = true;
            JsConfig.ConvertObjectTypesIntoStringDictionary = true;

            var original = new Dictionary<string, string>
          		{
					{"embeddedtypecharacters", "{{body}}"},
					{"embeddedlistcharacters", "[stuff]"},
					{"ShortDateTimeFormat", "yyyy-MM-dd"},
					{"DefaultDateTimeFormat", "dd/MM/yyyy HH:mm:ss"},
					{"DefaultDateTimeFormatWithFraction", "dd/MM/yyyy HH:mm:ss.fff"},
					{"XsdDateTimeFormat", "yyyy-MM-ddTHH:mm:ss.fffffffZ"},
					{"XsdDateTimeFormat3F", "yyyy-MM-ddTHH:mm:ss.fffZ"},
					{"XsdDateTimeFormatSeconds", "yyyy-MM-ddTHH:mm:ssZ"},
					{"ShouldBeAZeroInAString", "0"},
					{"ShouldBeAPositiveIntegerInAString", "12345"},
					{"ShouldBeANegativeIntegerInAString", "-12345"},
          		};
            var json = JsonSerializer.SerializeToString(original);
            var deserialized = JsonSerializer.DeserializeFromString<Dictionary<string, object>>(json);

            Console.WriteLine(json);

            Assert.That(deserialized, Is.Not.Null);
            Assert.That(deserialized["embeddedtypecharacters"], Is.Not.Null);
            Assert.That(deserialized["embeddedtypecharacters"], Is.EqualTo("{{body}}"));
            Assert.That(deserialized["embeddedlistcharacters"], Is.EqualTo("[stuff]"));
            Assert.That(deserialized["ShortDateTimeFormat"], Is.EqualTo("yyyy-MM-dd"));
            Assert.That(deserialized["DefaultDateTimeFormat"], Is.EqualTo("dd/MM/yyyy HH:mm:ss"));
            Assert.That(deserialized["DefaultDateTimeFormatWithFraction"], Is.EqualTo("dd/MM/yyyy HH:mm:ss.fff"));
            Assert.That(deserialized["XsdDateTimeFormat"], Is.EqualTo("yyyy-MM-ddTHH:mm:ss.fffffffZ"));
            Assert.That(deserialized["XsdDateTimeFormat3F"], Is.EqualTo("yyyy-MM-ddTHH:mm:ss.fffZ"));
            Assert.That(deserialized["XsdDateTimeFormatSeconds"], Is.EqualTo("yyyy-MM-ddTHH:mm:ssZ"));
            Assert.That(deserialized["ShouldBeAZeroInAString"], Is.EqualTo("0"));
            Assert.That(deserialized["ShouldBeAZeroInAString"], Is.InstanceOf<string>());
            Assert.That(deserialized["ShouldBeAPositiveIntegerInAString"], Is.EqualTo("12345"));
            Assert.That(deserialized["ShouldBeAPositiveIntegerInAString"], Is.InstanceOf<string>());
            Assert.That(deserialized["ShouldBeANegativeIntegerInAString"], Is.EqualTo("-12345"));
        }

        /// <summary>Sets up the dictionary.</summary>
        /// <returns>A Dictionary&lt;string,object&gt;</returns>
        private static Dictionary<string, object> SetupDict()
        {
            return new Dictionary<string, object> {
                { "a", "text" },
                { "b", 32 },
                { "c", false },
                { "d", new[] {1, 2, 3} },
				{ "e", 1m },
            };
        }

        /// <summary>A mix type.</summary>
        public class MixType
        {
            /// <summary>Gets or sets a.</summary>
            /// <value>a.</value>
            public string a { get; set; }

            /// <summary>Gets or sets the b.</summary>
            /// <value>The b.</value>
            public int b { get; set; }

            /// <summary>Gets or sets a value indicating whether the c.</summary>
            /// <value>true if c, false if not.</value>
            public bool c { get; set; }

            /// <summary>Gets or sets the d.</summary>
            /// <value>The d.</value>
            public int[] d { get; set; }
        }

        /// <summary>Assert dictionary.</summary>
        /// <param name="dict">The dictionary.</param>
        private static void AssertDict(Dictionary<string, object> dict)
        {
            Assert.AreEqual("text", dict["a"]);
            Assert.AreEqual(32, dict["b"]);
            Assert.AreEqual(false, dict["c"]);
        }

        //[Test]
        //public void Test_JsonNet()
        //{
        //    var dict = SetupDict();
        //    var json = JsonConvert.SerializeObject(dict);
        //    var deserializedDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
        //    AssertDict(deserializedDict);
        //}

        /// <summary>Tests n service kit text type serializer.</summary>
        [Test]
        public void Test_NServiceKit_Text_TypeSerializer()
        {
            JsConfig.TryToParsePrimitiveTypeValues = true;
            JsConfig.ConvertObjectTypesIntoStringDictionary = true;

            var dict = SetupDict();
            var json = TypeSerializer.SerializeToString(dict);
            var deserializedDict = TypeSerializer.DeserializeFromString<Dictionary<string, object>>(json);
            AssertDict(deserializedDict);
        }

        /// <summary>Tests n service kit text JSON serializer.</summary>
        [Test]
        public void Test_NServiceKit_Text_JsonSerializer()
        {
            JsConfig.TryToParsePrimitiveTypeValues = true;
            JsConfig.ConvertObjectTypesIntoStringDictionary = true;

            var dict = SetupDict();
            var json = JsonSerializer.SerializeToString(dict);
            var deserializedDict = JsonSerializer.DeserializeFromString<Dictionary<string, object>>(json);
            AssertDict(deserializedDict);
        }

        /// <summary>
        /// Tests n service kit text JSON serializer array value deserializes correctly.
        /// </summary>
        [Test]
        public void Test_NServiceKit_Text_JsonSerializer_Array_Value_Deserializes_Correctly()
        {
            JsConfig.TryToParsePrimitiveTypeValues = true;
            JsConfig.ConvertObjectTypesIntoStringDictionary = true;

            var dict = SetupDict();
            var json = JsonSerializer.SerializeToString(dict);
            var deserializedDict = JsonSerializer.DeserializeFromString<Dictionary<string, object>>(json);
            Assert.AreEqual("text", deserializedDict["a"]);
            Assert.AreEqual(new List<int> { 1, 2, 3 }, deserializedDict["d"]);
        }

        /// <summary>Deserizes to decimal by default.</summary>
        [Test]
        public void deserizes_to_decimal_by_default()
        {
            JsConfig.TryToParsePrimitiveTypeValues = true;

            var dict = SetupDict();
            var json = JsonSerializer.SerializeToString(dict);
            var deserializedDict = JsonSerializer.DeserializeFromString<IDictionary<string, object>>(json);
            Assert.That(deserializedDict["e"], Is.TypeOf<decimal>());
            Assert.That(deserializedDict["e"], Is.EqualTo(1m));

        }

        /// <summary>A numeric type.</summary>
        class NumericType
        {
            /// <summary>
            /// Initializes a new instance of the NServiceKit.Text.Tests.DictionaryTests.NumericType
            /// class.
            /// </summary>
            /// <param name="max"> The maximum value.</param>
            /// <param name="type">The type.</param>
            public NumericType(decimal max, Type type)
                : this(0, max, type)
            {

            }

            /// <summary>
            /// Initializes a new instance of the NServiceKit.Text.Tests.DictionaryTests.NumericType
            /// class.
            /// </summary>
            /// <param name="min"> The minimum value.</param>
            /// <param name="max"> The maximum value.</param>
            /// <param name="type">The type.</param>
            public NumericType(decimal min, decimal max, Type type)
            {
                Min = min;
                Max = max;
                Type = type;
            }

            /// <summary>Gets the minimum.</summary>
            /// <value>The minimum value.</value>
            public decimal Min { get; private set; }

            /// <summary>Gets the maximum.</summary>
            /// <value>The maximum value.</value>
            public decimal Max { get; private set; }

            /// <summary>Gets the type.</summary>
            /// <value>The type.</value>
            public Type Type { get; private set; }
        }

        /// <summary>Deserizes signed bytes into best fit numeric.</summary>
        [Test]
        public void deserizes_signed_bytes_into_to_best_fit_numeric()
        {
            JsConfig.TryToParsePrimitiveTypeValues = true;
            JsConfig.TryToParseNumericType = true;

            var deserializedDict = JsonSerializer.DeserializeFromString<IDictionary<string, object>>("{\"min\":-128,\"max\":127}");
            Assert.That(deserializedDict["min"], Is.TypeOf<sbyte>());
            Assert.That(deserializedDict["min"], Is.EqualTo(sbyte.MinValue));
            //it seemed strange having zero return as a signed byte
            Assert.That(deserializedDict["max"], Is.TypeOf<byte>());
            Assert.That(deserializedDict["max"], Is.EqualTo(sbyte.MaxValue));
        }

        /// <summary>Deserizes signed types into best fit numeric.</summary>
        [Test]
        public void deserizes_signed_types_into_to_best_fit_numeric()
        {
            var unsignedTypes = new[]
				{
					new NumericType(Int16.MinValue,Int16.MaxValue, typeof (Int16)),
					new NumericType(Int32.MinValue,Int32.MaxValue, typeof (Int32)),
					new NumericType(Int64.MinValue,Int64.MaxValue, typeof (Int64)),
				};

            JsConfig.TryToParsePrimitiveTypeValues = true;
            JsConfig.TryToParseNumericType = true;


            foreach (var signedType in unsignedTypes)
            {
                var dict = new Dictionary<string, object>
				{
					{"min",signedType.Min},
					{"max",signedType.Max},
				};

                var json = JsonSerializer.SerializeToString(dict);
                var deserializedDict = JsonSerializer.DeserializeFromString<IDictionary<string, object>>(json);
                Assert.That(deserializedDict["min"], Is.TypeOf(signedType.Type));
                Assert.That(deserializedDict["min"], Is.EqualTo(signedType.Min));
                Assert.That(deserializedDict["max"], Is.TypeOf(signedType.Type));
                Assert.That(deserializedDict["max"], Is.EqualTo(signedType.Max));

            }
        }

        /// <summary>Deserizes unsigned types into best fit numeric.</summary>
        [Test]
        public void deserizes_unsigned_types_into_to_best_fit_numeric()
        {
            var unsignedTypes = new[]
				{
					new NumericType(byte.MinValue,byte.MaxValue, typeof (byte)),
					new NumericType(UInt16.MaxValue, typeof (UInt16)),
					new NumericType(UInt32.MaxValue, typeof (UInt32)),
					new NumericType(UInt64.MaxValue, typeof (UInt64)),
				};

            JsConfig.TryToParsePrimitiveTypeValues = true;
            JsConfig.TryToParseNumericType = true;


            foreach (var unsignedType in unsignedTypes)
            {
                var dict = new Dictionary<string, object>
				{
					{"min",unsignedType.Min},
					{"max",unsignedType.Max},
				};

                var json = JsonSerializer.SerializeToString(dict);
                var deserializedDict = JsonSerializer.DeserializeFromString<IDictionary<string, object>>(json);
                Assert.That(deserializedDict["min"], Is.EqualTo(0));
                Assert.That(deserializedDict["min"], Is.TypeOf<byte>());
                Assert.That(deserializedDict["max"], Is.TypeOf(unsignedType.Type));
                Assert.That(deserializedDict["max"], Is.EqualTo(unsignedType.Max));

            }
        }

        /// <summary>Can deserialize mixed dictionary into strongtyped map.</summary>
        [Test]
        public void Can_deserialize_mixed_dictionary_into_strongtyped_map()
        {
            var mixedMap = SetupDict();

            var json = JsonSerializer.SerializeToString(mixedMap);
            Console.WriteLine("JSON:\n" + json);

            var mixedType = json.FromJson<MixType>();
            Assert.AreEqual("text", mixedType.a);
            Assert.AreEqual(32, mixedType.b);
            Assert.AreEqual(false, mixedType.c);
            Assert.AreEqual(new[] { 1, 2, 3 }, mixedType.d);
        }

        /// <summary>Can serialise null values from dictionary correctly.</summary>
        [Test]
        public void Can_serialise_null_values_from_dictionary_correctly()
        {
            JsConfig.IncludeNullValues = true;
            var dictionary = new Dictionary<string, object> { { "value", null } };

            Serialize(dictionary, includeXml: false);

            var json = JsonSerializer.SerializeToString(dictionary);
            Log(json);

            Assert.That(json, Is.EqualTo("{\"value\":null}"));
            JsConfig.Reset();
        }

        /// <summary>Will ignore null values from dictionary correctly.</summary>
        [Test]
        public void Will_ignore_null_values_from_dictionary_correctly()
        {
            JsConfig.IncludeNullValues = false;
            var dictionary = new Dictionary<string, string> { { "value", null } };

            Serialize(dictionary, includeXml: false);

            var json = JsonSerializer.SerializeToString(dictionary);
            Log(json);

            Assert.That(json, Is.EqualTo("{}"));
            JsConfig.Reset();
        }

        /// <summary>A foo slash.</summary>
        public class FooSlash
        {
            /// <summary>Gets or sets the nested.</summary>
            /// <value>The nested.</value>
            public Dictionary<string, string> Nested { get; set; }

            /// <summary>Gets or sets the bar.</summary>
            /// <value>The bar.</value>
            public string Bar { get; set; }
        }

        /// <summary>Can serialize dictionary with end slash.</summary>
        [Test]
        public void Can_serialize_Dictionary_with_end_slash()
        {
            var foo = new FooSlash
            {
                Nested = new Dictionary<string, string> { { "key", "value\"" } },
                Bar = "BarValue"
            };
            Serialize(foo);
        }

        /// <summary>Can serialise null values from nested dictionary correctly.</summary>
        [Test]
        public void Can_serialise_null_values_from_nested_dictionary_correctly()
        {
            JsConfig.IncludeNullValues = true;
            var foo = new FooSlash();
            var json = JsonSerializer.SerializeToString(foo);
            Assert.That(json, Is.EqualTo("{\"Nested\":null,\"Bar\":null}"));
            JsConfig.Reset();
        }

        /// <summary>Can serialize dictionary with quotes.</summary>
        [Test]
        public void Can_serialize_Dictionary_with_quotes()
        {
            var dto = new Dictionary<string, string> { { "title", "\"test\"" } };
            var to = Serialize(dto);

            Assert.That(to["title"], Is.EqualTo(dto["title"]));
        }

        /// <summary>Can serialize dictionary with escaped symbols in key.</summary>
        [Test]
        public void Can_serialize_Dictionary_with_escaped_symbols_in_key()
        {
            var dto = new Dictionary<string, string> { { @"a\fb", "\"test\"" } };
            var to = Serialize(dto);

            Assert.That(to.Keys.ToArray()[0], Is.EqualTo(@"a\fb"));
        }

        /// <summary>Can serialize dictionary with escaped symbols in key and binary value.</summary>
        [Test]
        public void Can_serialize_Dictionary_with_escaped_symbols_in_key_and_binary_value()
        {
            var dto = new Dictionary<string, byte[]> { { @"a\fb", new byte[] { 1 } } };
            var to = Serialize(dto);

            Assert.That(to.Keys.ToArray()[0], Is.EqualTo(@"a\fb"));
        }

        /// <summary>Can serialize dictionary with int key and string with quote.</summary>
        [Test]
        public void Can_serialize_Dictionary_with_int_key_and_string_with_quote()
        {
            var dto = new Dictionary<int, string> { { 1, @"a""b" } };
            var to = Serialize(dto);

            Assert.That(to.Keys.ToArray()[0], Is.EqualTo(1));
            Assert.That(to[1], Is.EqualTo(@"a""b"));
        }

        /// <summary>Can serialize string byte dictionary with UTF 8.</summary>
        [Test]
        public void Can_serialize_string_byte_Dictionary_with_UTF8()
        {
            var dto = new Dictionary<string, byte[]> { { "aфаž\"a", new byte[] { 1 } } };
            var to = Serialize(dto);

            Assert.That(to.Keys.ToArray()[0], Is.EqualTo("aфаž\"a"));
        }

        /// <summary>Can serialize string dictionary with UTF 8.</summary>
        [Test]
        public void Can_serialize_string_string_Dictionary_with_UTF8()
        {
            var dto = new Dictionary<string, string> { { "aфаž\"a", "abc" } };
            var to = Serialize(dto);

            Assert.That(to.Keys.ToArray()[0], Is.EqualTo("aфаž\"a"));
        }

        /// <summary>Can deserialize object to dictionary.</summary>
        [Test]
        public void Can_Deserialize_Object_To_Dictionary()
        {
            const string json = "{\"Id\":1}";
            var d = json.To<Dictionary<string, string>>();
            Assert.That(d.ContainsKey("Id"));
            Assert.That(d["Id"], Is.EqualTo("1"));
        }

#if NET40
        [Test]
        public void Nongeneric_implementors_of_IDictionary_K_V_Should_serialize_like_Dictionary_K_V()
        {
            dynamic expando = new System.Dynamic.ExpandoObject();
            expando.Property = "Value";
            IDictionary<string, object> dict = expando;
            Assert.AreEqual(dict.Dump(), new Dictionary<string, object>(dict).Dump());
        }
#endif
    }

}