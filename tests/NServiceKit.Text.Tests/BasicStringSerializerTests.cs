using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using NUnit.Framework;
#if !MONOTOUCH
using System.ComponentModel.DataAnnotations;
using Northwind.Common.ComplexModel;
using NServiceKit.Common.Extensions;
using NServiceKit.Common.Tests.Models;
#endif
using NServiceKit.Text.Common;

namespace NServiceKit.Text.Tests
{
    /// <summary>A basic string serializer tests.</summary>
    [TestFixture]
    public class BasicStringSerializerTests
    {
        /// <summary>all characters used.</summary>
        readonly char[] allCharsUsed = new[] {
			JsWriter.QuoteChar, JsWriter.ItemSeperator,
			JsWriter.MapStartChar, JsWriter.MapKeySeperator, JsWriter.MapEndChar,
			JsWriter.ListEndChar, JsWriter.ListEndChar,
		};

        /// <summary>The field with invalid characters.</summary>
        readonly string fieldWithInvalidChars = string.Format("all {0} {1} {2} {3} {4} {5} {6} invalid chars",
            JsWriter.QuoteChar, JsWriter.ItemSeperator,
            JsWriter.MapStartChar, JsWriter.MapKeySeperator, JsWriter.MapEndChar,
            JsWriter.ListEndChar, JsWriter.ListEndChar);

        /// <summary>The int values.</summary>
        readonly int[] intValues = new[] { 1, 2, 3, 4, 5 };

        /// <summary>The double values.</summary>
        readonly double[] doubleValues = new[] { 1.0d, 2.0d, 3.0d, 4.0d, 5.0d };

        /// <summary>The string values.</summary>
        readonly string[] stringValues = new[] { "One", "Two", "Three", "Four", "Five" };

        /// <summary>The string values with illegal character.</summary>
        readonly string[] stringValuesWithIllegalChar = new[] { "One", ",", "Three", "Four", "Five" };

        /// <summary>Values that represent TestEnum.</summary>
        public enum TestEnum
        {
            /// <summary>An enum constant representing the enum value 1 option.</summary>
            EnumValue1,

            /// <summary>An enum constant representing the enum value 2 option.</summary>
            EnumValue2,
        }

        /// <summary>Bitfield of flags for specifying UnsignedFlags.</summary>
        [Flags]
        public enum UnsignedFlags : uint
        {
            /// <summary>A binary constant representing the enum value 1 flag.</summary>
            EnumValue1 = 0,

            /// <summary>A binary constant representing the enum value 2 flag.</summary>
            EnumValue2 = 1,
        }

        /// <summary>Can convert comma delimited string to list string.</summary>
        [Test]
        public void Can_convert_comma_delimited_string_to_List_String()
        {
            Assert.That(TypeSerializer.CanCreateFromString(typeof(List<string>)), Is.True);

            var stringValueList = "[" + string.Join(",", stringValues) + "]";

            var convertedJsvValues = TypeSerializer.DeserializeFromString<List<string>>(stringValueList);
            Assert.That(convertedJsvValues, Is.EquivalentTo(stringValues));

            var convertedJsonValues = JsonSerializer.DeserializeFromString<List<string>>(stringValueList);
            Assert.That(convertedJsonValues, Is.EquivalentTo(stringValues));
        }

        /// <summary>Null or empty string returns null.</summary>
        [Test]
        public void Null_or_Empty_string_returns_null()
        {
            var convertedJsvValues = TypeSerializer.DeserializeFromString<List<string>>(null);
            Assert.That(convertedJsvValues, Is.EqualTo(null));

            convertedJsvValues = TypeSerializer.DeserializeFromString<List<string>>(string.Empty);
            Assert.That(convertedJsvValues, Is.EqualTo(null));
        }

        /// <summary>Empty list string returns empty list.</summary>
        [Test]
        public void Empty_list_string_returns_empty_List()
        {
            var convertedStringValues = TypeSerializer.DeserializeFromString<List<string>>("[]");
            Assert.That(convertedStringValues, Is.EqualTo(new List<string>()));
        }

        /// <summary>Null or empty string returns null map.</summary>
        [Test]
        public void Null_or_Empty_string_returns_null_Map()
        {
            var convertedStringValues = TypeSerializer.DeserializeFromString<Dictionary<string, string>>(null);
            Assert.That(convertedStringValues, Is.EqualTo(null));

            convertedStringValues = TypeSerializer.DeserializeFromString<Dictionary<string, string>>(string.Empty);
            Assert.That(convertedStringValues, Is.EqualTo(null));
        }

        /// <summary>Empty map string returns empty list.</summary>
        [Test]
        public void Empty_map_string_returns_empty_List()
        {
            var convertedStringValues = TypeSerializer.DeserializeFromString<Dictionary<string, string>>("{}");
            Assert.That(convertedStringValues, Is.EqualTo(new Dictionary<string, string>()));
        }

        /// <summary>Can convert string collection.</summary>
        [Test]
        public void Can_convert_string_collection()
        {
            Assert.That(TypeSerializer.CanCreateFromString(typeof(string[])), Is.True);

            var stringValue = TypeSerializer.SerializeToString(stringValues);
            var expectedString = "[" + string.Join(",", stringValues.ToArray()) + "]";
            Assert.That(stringValue, Is.EqualTo(expectedString));
        }

        /// <summary>Can convert enum.</summary>
        [Test]
        public void Can_convert_enum()
        {
            var enumValue = TestEnum.EnumValue1;
            var stringValue = TypeSerializer.SerializeToString(enumValue);
            var expectedString = TestEnum.EnumValue1.ToString();
            Assert.That(stringValue, Is.EqualTo(expectedString));
        }

        /// <summary>Can convert nullable enum.</summary>
        [Test]
        public void Can_convert_nullable_enum()
        {
            TestEnum? enumValue = TestEnum.EnumValue1;
            var stringValue = TypeSerializer.SerializeToString(enumValue);
            var expectedString = TestEnum.EnumValue1.ToString();
            Assert.That(stringValue, Is.EqualTo(expectedString));
        }

        /// <summary>Can convert to nullable enum.</summary>
        [Test]
        public void Can_convert_to_nullable_enum()
        {
            Assert.That(TypeSerializer.CanCreateFromString(typeof(TestEnum?)), Is.True);

            TestEnum? enumValue = TestEnum.EnumValue1;
            var actualValue = TypeSerializer.DeserializeFromString<TestEnum?>(enumValue.ToString());
            Assert.That(actualValue, Is.EqualTo(enumValue));
        }

        /// <summary>Can convert to nullable enum with null value.</summary>
        [Test]
        public void Can_convert_to_nullable_enum_with_null_value()
        {
            var enumValue = TypeSerializer.DeserializeFromString<TestEnum?>(null);
            Assert.That(enumValue, Is.Null);
        }

        /// <summary>Can convert nullable enum with null value.</summary>
        [Test]
        public void Can_convert_nullable_enum_with_null_value()
        {
            TestEnum? enumValue = null;
            var stringValue = TypeSerializer.SerializeToString(enumValue);
            Assert.That(stringValue, Is.Null);
        }

        /// <summary>Can convert unsigned flags enum.</summary>
        [Test]
        public void Can_convert_unsigned_flags_enum()
        {
            var enumValue = UnsignedFlags.EnumValue1;
            var stringValue = TypeSerializer.SerializeToString(enumValue);
            var expectedString = UnsignedFlags.EnumValue1.ToString("D");
            Assert.That(stringValue, Is.EqualTo(expectedString));
        }

        /// <summary>Can convert unique identifier.</summary>
        [Test]
        public void Can_convert_Guid()
        {
            Assert.That(TypeSerializer.CanCreateFromString(typeof(Guid)), Is.True);

            var guidValue = Guid.NewGuid();
            var stringValue = TypeSerializer.SerializeToString(guidValue);
            var expectedString = guidValue.ToString("N");
            Assert.That(stringValue, Is.EqualTo(expectedString));
        }

        /// <summary>Can convert datetime.</summary>
        [Test]
        public void Can_convert_datetime()
        {
            var dateValue = new DateTime(1979, 5, 9);
            var stringValue = TypeSerializer.SerializeToString(dateValue);
            var expectedString = "1979-05-09";
            Assert.That(stringValue, Is.EqualTo(expectedString));
        }

        /// <summary>Can convert to datetime.</summary>
        [Test]
        public void Can_convert_to_datetime()
        {
            Assert.That(TypeSerializer.CanCreateFromString(typeof(DateTime)), Is.True);

            var dateValue = new DateTime(1979, 5, 9);
            var actualValue = TypeSerializer.DeserializeFromString<DateTime>("1979-05-09");
            Assert.That(actualValue, Is.EqualTo(dateValue));
        }

        /// <summary>Can convert nullable datetime.</summary>
        [Test]
        public void Can_convert_nullable_datetime()
        {
            DateTime? dateValue = new DateTime(1979, 5, 9);
            var stringValue = TypeSerializer.SerializeToString(dateValue);
            var expectedString = "1979-05-09";
            Assert.That(stringValue, Is.EqualTo(expectedString));
        }

        /// <summary>Can convert to nullable datetime.</summary>
        [Test]
        public void Can_convert_to_nullable_datetime()
        {
            Assert.That(TypeSerializer.CanCreateFromString(typeof(DateTime?)), Is.True);

            DateTime? dateValue = new DateTime(1979, 5, 9);
            var actualValue = TypeSerializer.DeserializeFromString<DateTime?>("1979-05-09");
            Assert.That(actualValue, Is.EqualTo(dateValue));
        }

        /// <summary>Can convert string list.</summary>
        [Test]
        public void Can_convert_string_List()
        {
            var stringValue = TypeSerializer.SerializeToString(stringValues.ToList());
            var expectedString = "[" + string.Join(",", stringValues.ToArray()) + "]";
            Assert.That(stringValue, Is.EqualTo(expectedString));
        }

        /// <summary>Can convert string array.</summary>
        [Test]
        public void Can_convert_string_array()
        {
            var stringValue = TypeSerializer.SerializeToString(stringValues.ToArray());
            var expectedString = "[" + string.Join(",", stringValues.ToArray()) + "]";
            Assert.That(stringValue, Is.EqualTo(expectedString));
        }

        /// <summary>Can convert string list as object.</summary>
        [Test]
        public void Can_convert_string_List_as_object()
        {
            var stringValue = TypeSerializer.SerializeToString((object)stringValues.ToList());
            var expectedString = "[" + string.Join(",", stringValues.ToArray()) + "]";
            Assert.That(stringValue, Is.EqualTo(expectedString));
        }

        /// <summary>Can convert empty list.</summary>
        [Test]
        public void Can_convert_empty_List()
        {
            var stringValue = TypeSerializer.SerializeToString(new List<string>());
            var expectedString = "[]";
            Assert.That(stringValue, Is.EqualTo(expectedString));
        }

        /// <summary>Can convert multidimensional array.</summary>
        [Test]
        public void Can_convert_multidimensional_array()
        {
            var data = new double[,] { { 1, 0 }, { 0, 1 } };
            var result = TypeSerializer.SerializeToString(data);

            Assert.That(result, Is.EqualTo("[[1,0],[0,1]]"));

            var data2 = new double[,,] { { { 1, 0 }, { 1, 0 } }, { { 0, 1 }, { 0, 1 } } };
            result = TypeSerializer.SerializeToString(data2);

            Assert.That(result, Is.EqualTo("[[[1,0],[1,0]],[[0,1],[0,1]]]"));
        }

        /// <summary>Can convert empty list as object.</summary>
        [Test]
        public void Can_convert_empty_List_as_object()
        {
            var stringValue = TypeSerializer.SerializeToString((object)new List<string>());
            var expectedString = "[]";
            Assert.That(stringValue, Is.EqualTo(expectedString));
        }

        /// <summary>Can convert string dictionary.</summary>
        [Test]
        public void Can_convert_string_dictionary()
        {
            var stringDictionary = new Dictionary<string, string> 
				{
					{ "One", "1st" }, { "Two", "2nd" }, { "Three", "3rd" }
				};
            var expectedString = "{One:1st,Two:2nd,Three:3rd}";
            var stringValue = TypeSerializer.SerializeToString(stringDictionary);
            Assert.That(stringValue, Is.EqualTo(expectedString));
        }

        /// <summary>Can parse string dictionary.</summary>
        [Test]
        public void Can_parse_string_dictionary()
        {
            var stringDictionary = new Dictionary<string, string> 
				{
					{ "One", "1st" }, { "Two", "2nd" }, { "Three", "3rd" }
				};
            const string mapValues = "{One:1st,Two:2nd,Three:3rd}";
            var parsedDictionary = TypeSerializer.DeserializeFromString(mapValues, stringDictionary.GetType());
            Assert.That(parsedDictionary, Is.EquivalentTo(stringDictionary));
        }

        /// <summary>Can convert string dictionary as object.</summary>
        [Test]
        public void Can_convert_string_dictionary_as_object()
        {
            var stringDictionary = new Dictionary<string, string> {
			                                                      	{ "One", "1st" }, { "Two", "2nd" }, { "Three", "3rd" }
			                                                      };
            var expectedString = "{One:1st,Two:2nd,Three:3rd}";
            var stringValue = TypeSerializer.SerializeToString((object)stringDictionary);
            Assert.That(stringValue, Is.EqualTo(expectedString));
        }

        /// <summary>Can convert string dictionary with special characters as object.</summary>
        [Test]
        public void Can_convert_string_dictionary_with_special_chars_as_object()
        {
            var stringDictionary = new Dictionary<string, string> 
				{
					{ "One", "\"1st" }, { "Two", "2:nd" }, { "Three", "3r,d" }, { "Four", "four%" }
				};
            var expectedString = "{One:\"\"\"1st\",Two:\"2:nd\",Three:\"3r,d\",Four:four%}";
            var stringValue = TypeSerializer.SerializeToString(stringDictionary);
            Assert.That(stringValue, Is.EqualTo(expectedString));

            Serialize(stringDictionary);
        }

        /// <summary>Can parse string dictionary with special characters as object.</summary>
        [Test]
        public void Can_parse_string_dictionary_with_special_chars_as_object()
        {
            var stringDictionary = new Dictionary<string, string> 
				{
					{ "One", "\"1st" }, { "Two", "2:nd" }, { "Three", "3r,d" }
				};
            const string mapValues = "{One:\"\"\"1st\",Two:2:nd,Three:\"3r,d\"}";
            var parsedDictionary = TypeSerializer.DeserializeFromString(mapValues, stringDictionary.GetType());
            Assert.That(parsedDictionary, Is.EquivalentTo(stringDictionary));

            Serialize(stringDictionary);
        }

        /// <summary>Can convert string list with special characters as object.</summary>
        [Test]
        public void Can_convert_string_list_with_special_chars_as_object()
        {
            var stringList = new List<string> 
				{
					"\"1st", "2:nd", "3r,d", "four%"
				};
            var expectedString = "[\"\"\"1st\",\"2:nd\",\"3r,d\",four%]";
            var stringValue = TypeSerializer.SerializeToString(stringList);
            Assert.That(stringValue, Is.EqualTo(expectedString));

            Serialize(stringList);
        }

        /// <summary>Can parse string list with special characters as object.</summary>
        [Test]
        public void Can_parse_string_list_with_special_chars_as_object()
        {
            var stringList = new List<string> 
				{
					"\"1st", "2:nd", "3r,d", "four%"
				};
            const string listValues = "[\"\"\"1st\",2:nd,\"3r,d\",four%]";
            var parsedList = TypeSerializer.DeserializeFromString(listValues, stringList.GetType());
            Assert.That(parsedList, Is.EquivalentTo(stringList));

            Serialize(stringList);
        }

        /// <summary>Can convert byte array with JSON serializer.</summary>
        [Test]
        public void Can_convert_Byte_array_with_JsonSerializer()
        {
            var byteArrayValue = new byte[] { 0, 65, 97, 255, };
            var stringValue = JsonSerializer.SerializeToString(byteArrayValue);
            var expectedString = Convert.ToBase64String(byteArrayValue);
            Assert.That(stringValue, Is.EqualTo('"' + expectedString + '"'));
        }

        /// <summary>Can convert byte array.</summary>
        [Test]
        public void Can_convert_Byte_array()
        {
            var byteArrayValue = new byte[] { 0, 65, 97, 255, };
            var stringValue = TypeSerializer.SerializeToString(byteArrayValue);
            var expectedString = Convert.ToBase64String(byteArrayValue);
            Assert.That(stringValue, Is.EqualTo(expectedString));
        }

        /// <summary>Can convert to byte array.</summary>
        [Test]
        public void Can_convert_to_Byte_array()
        {
            Assert.That(TypeSerializer.CanCreateFromString(typeof(byte[])), Is.True);

            var byteArrayValue = new byte[] { 0, 65, 97, 255, };
            var byteArrayString = TypeSerializer.SerializeToString(byteArrayValue);
            var actualValue = TypeSerializer.DeserializeFromString<byte[]>(byteArrayString);
            Assert.That(actualValue, Is.EqualTo(byteArrayValue));
        }

        /// <summary>true this object to the given stream.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="model">The model.</param>
        /// <returns>A T.</returns>
        public T Serialize<T>(T model)
        {
            var jsvModel = TypeSerializer.SerializeToString(model);
            // Console.WriteLine("Len: " + jsvModel.Length + ", " + jsvModel);
            Assert.IsNotNullOrEmpty(jsvModel);
            var fromJsvModel = TypeSerializer.DeserializeFromString<T>(jsvModel);

            var jsonModel = JsonSerializer.SerializeToString(model);
            // Console.WriteLine("Len: " + jsonModel.Length + ", " + jsonModel);
            Assert.IsNotNullOrEmpty(jsonModel);
            var fromJsonModel = JsonSerializer.DeserializeFromString<T>(jsonModel);

            return fromJsonModel;
        }

#if !MONOTOUCH
        /// <summary>A test class.</summary>
        public class TestClass
        {
            /// <summary>Gets or sets the member 1.</summary>
            /// <value>The member 1.</value>
            [Required]
            public string Member1 { get; set; }

            /// <summary>Gets or sets the member 2.</summary>
            /// <value>The member 2.</value>
            public string Member2 { get; set; }

            /// <summary>Gets or sets the member 3.</summary>
            /// <value>The member 3.</value>
            [Required]
            public string Member3 { get; set; }

            /// <summary>Gets or sets the member 4.</summary>
            /// <value>The member 4.</value>
            [StringLength(1)]
            public string Member4 { get; set; }
        }

        /// <summary>Can convert string to list.</summary>
        [Test]
        public void Can_convert_string_to_List()
        {
            var fromHashSet = stringValues;
            var toHashSet = Serialize(fromHashSet);

            Assert.That(toHashSet.EquivalentTo(fromHashSet), Is.True);
        }

        /// <summary>Can convert string to string hash set.</summary>
        [Test]
        public void Can_convert_string_to_string_HashSet()
        {
            var fromHashSet = new HashSet<string>(stringValues);
            var toHashSet = Serialize(fromHashSet);

            Assert.That(toHashSet.EquivalentTo(fromHashSet), Is.True);
        }

        /// <summary>Can convert string to int hash set.</summary>
        [Test]
        public void Can_convert_string_to_int_HashSet()
        {
            var fromHashSet = new HashSet<int>(intValues);
            var toHashSet = Serialize(fromHashSet);

            Assert.That(toHashSet.EquivalentTo(fromHashSet), Is.True);
        }

        /// <summary>Can convert string to double hash set.</summary>
        [Test]
        public void Can_convert_string_to_double_HashSet()
        {
            var fromHashSet = new HashSet<double>(doubleValues);
            var toHashSet = Serialize(fromHashSet);

            Assert.That(toHashSet.EquivalentTo(fromHashSet), Is.True);
        }

        /// <summary>Can convert string to string read only collection.</summary>
        [Test]
        public void Can_convert_string_to_string_ReadOnlyCollection()
        {
            var fromCollection = new ReadOnlyCollection<string>(stringValues);
            var toCollection = Serialize(fromCollection);

            Assert.That(toCollection.EquivalentTo(fromCollection), Is.True);
        }

        /// <summary>Can convert string to int read only collection.</summary>
        [Test]
        public void Can_convert_string_to_int_ReadOnlyCollection()
        {
            var fromCollection = new ReadOnlyCollection<int>(intValues);
            var toCollection = Serialize(fromCollection);

            Assert.That(toCollection.EquivalentTo(fromCollection), Is.True);
        }

        /// <summary>Can convert string to double read only collection.</summary>
        [Test]
        public void Can_convert_string_to_double_ReadOnlyCollection()
        {
            var fromCollection = new ReadOnlyCollection<double>(doubleValues);
            var toCollection = Serialize(fromCollection);

            Assert.That(toCollection.EquivalentTo(fromCollection), Is.True);
        }

        /// <summary>Can convert model with fields of different types.</summary>
        [Test]
        public void Can_convert_ModelWithFieldsOfDifferentTypes()
        {
            var model = ModelWithFieldsOfDifferentTypes.Create(1);
            var toModel = Serialize(model);

            ModelWithFieldsOfDifferentTypes.AssertIsEqual(toModel, model);
        }

        /// <summary>Can convert model with fields of nullable types.</summary>
        [Test]
        public void Can_convert_ModelWithFieldsOfNullableTypes()
        {
            var model = ModelWithFieldsOfNullableTypes.Create(1);
            var toModel = Serialize(model);

            ModelWithFieldsOfNullableTypes.AssertIsEqual(toModel, model);
        }

        /// <summary>Can convert model with fields of nullable types of nullables.</summary>
        [Test]
        public void Can_convert_ModelWithFieldsOfNullableTypes_of_nullables()
        {
            var model = new ModelWithFieldsOfNullableTypes();
            var toModel = Serialize(model);

            ModelWithFieldsOfNullableTypes.AssertIsEqual(toModel, model);
        }

        /// <summary>Can convert model with complex types.</summary>
        [Ignore("Causing infinite recursion in TypeToString")]
        [Test]
        public void Can_convert_ModelWithComplexTypes()
        {
            var model = ModelWithComplexTypes.Create(1);
            var toModel = Serialize(model);

            ModelWithComplexTypes.AssertIsEqual(toModel, model);
        }

        /// <summary>Can convert model with type character.</summary>
        [Test]
        public void Can_convert_model_with_TypeChar()
        {
            var model = new ModelWithIdAndName { Id = 1, Name = "in } valid" };
            var toModel = Serialize(model);

            ModelWithIdAndName.AssertIsEqual(toModel, model);
        }

        /// <summary>Can convert model with list character.</summary>
        [Test]
        public void Can_convert_model_with_ListChar()
        {
            var model = new ModelWithIdAndName { Id = 1, Name = "in [ valid" };
            var toModel = Serialize(model);
            ModelWithIdAndName.AssertIsEqual(toModel, model);

            var model2 = new ModelWithIdAndName { Id = 1, Name = "in valid]" };
            var toModel2 = Serialize(model2);
            ModelWithIdAndName.AssertIsEqual(toModel2, model2);
        }

        /// <summary>Can convert model with map and list with list character.</summary>
        [Test]
        public void Can_convert_ModelWithMapAndList_with_ListChar()
        {
            var model = new ModelWithMapAndList<ModelWithIdAndName>
            {
                Id = 1,
                Name = "in [ valid",
                List = new List<ModelWithIdAndName> {
					new ModelWithIdAndName{Id = 1, Name = "field [in valid] has stuff"},
					new ModelWithIdAndName{Id = 1, Name = "field [in valid] has stuff"},
				},
            };
            var toModel = Serialize(model);
            //ModelWithMapAndList.AssertIsEqual(toModel, model);
        }

        /// <summary>Can convert array dto with orders.</summary>
        [Test]
        public void Can_convert_ArrayDtoWithOrders()
        {
            var model = DtoFactory.ArrayDtoWithOrders;
            var toModel = Serialize(model);

            Assert.That(model.Equals(toModel), Is.True);
        }

        /// <summary>Can convert field map or list with invalid characters.</summary>
        [Test]
        public void Can_convert_Field_Map_or_List_with_invalid_chars()
        {
            var instance = new ModelWithMapAndList<string>
            {
                Id = 1,
                Name = fieldWithInvalidChars,
                List = new List<string> { fieldWithInvalidChars, fieldWithInvalidChars },
                Map = new Dictionary<string, string> { { fieldWithInvalidChars, fieldWithInvalidChars } },
            };

            Serialize(instance);
        }

        /// <summary>Can convert field map or list with single invalid character.</summary>
        [Test]
        public void Can_convert_Field_Map_or_List_with_single_invalid_char()
        {
            foreach (var invalidChar in allCharsUsed)
            {
                var singleInvalidChar = string.Format("a {0} b", invalidChar);

                var instance = new ModelWithMapAndList<string>
                {
                    Id = 1,
                    Name = singleInvalidChar,
                    List = new List<string> { singleInvalidChar, singleInvalidChar },
                    Map = new Dictionary<string, string> { { singleInvalidChar, singleInvalidChar } },
                };

                Serialize(instance);
            }
        }

        /// <summary>Can convert customer dto.</summary>
        [Test]
        public void Can_convert_CustomerDto()
        {
            var model = DtoFactory.CustomerDto;
            var toModel = Serialize(model);

            Assert.That(model.Equals(toModel), Is.True);
        }

        /// <summary>Can convert customer order list dto.</summary>
        [Test]
        public void Can_convert_CustomerOrderListDto()
        {
            var model = DtoFactory.CustomerOrderListDto;
            var toModel = Serialize(model);

            Assert.That(model.Equals(toModel), Is.True);
        }

        /// <summary>Can convert list unique identifier.</summary>
        [Test]
        public void Can_convert_List_Guid()
        {
            var model = new List<Guid> {
				Guid.NewGuid(),
				Guid.NewGuid(),
				Guid.NewGuid(),
			};

            var toModel = Serialize(model);

            Assert.That(toModel, Is.EquivalentTo(model));
        }
#endif
    }
}