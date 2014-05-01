using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using NServiceKit.Text.Common;

namespace NServiceKit.Text.Tests
{
    /// <summary>A structure tests.</summary>
    [TestFixture]
    public class StructTests
    {
        /// <summary>A foo.</summary>
        [Serializable]
        public class Foo
        {
            /// <summary>Gets or sets the name.</summary>
            /// <value>The name.</value>
            public string Name { get; set; }

            /// <summary>Gets or sets the content 1.</summary>
            /// <value>The content 1.</value>
            public Text Content1 { get; set; }

            /// <summary>Gets or sets the content 2.</summary>
            /// <value>The content 2.</value>
            public Text Content2 { get; set; }
        }

        /// <summary>Interface for text.</summary>
        public interface IText { }

        /// <summary>A text.</summary>
        [Serializable]
        public struct Text
        {
            /// <summary>The value.</summary>
            private readonly string _value;

            /// <summary>
            /// Initializes a new instance of the NServiceKit.Text.Tests.StructTests class.
            /// </summary>
            /// <param name="value">The value.</param>
            public Text(string value)
            {
                _value = value;
            }

            /// <summary>Parses.</summary>
            /// <param name="value">The value.</param>
            /// <returns>A Text.</returns>
            public static Text Parse(string value)
            {
                return value == null ? null : new Text(value);
            }

            /// <summary>Text casting operator.</summary>
            /// <param name="value">The value.</param>
            public static implicit operator Text(string value)
            {
                return new Text(value);
            }

            /// <summary>string casting operator.</summary>
            /// <param name="item">The item.</param>
            public static implicit operator string(Text item)
            {
                return item._value;
            }

            /// <summary>Returns the fully qualified type name of this instance.</summary>
            /// <returns>A <see cref="T:System.String" /> containing a fully qualified type name.</returns>
            public override string ToString()
            {
                return _value;
            }
        }

        /// <summary>Sets the up.</summary>
        [TestFixtureSetUp]
        public void SetUp()
        {
#if MONOTOUCH
            JsConfig.RegisterTypeForAot<Text> ();
            JsConfig.RegisterTypeForAot<Foo> ();
            JsConfig.RegisterTypeForAot<PersonStatus> ();
            JsConfig.RegisterTypeForAot<Person> ();
            JsConfig.RegisterTypeForAot<TestDictionary> ();
            JsConfig.RegisterTypeForAot<KeyValuePair<string, string>> ();
            JsConfig.RegisterTypeForAot<Pair> ();
#endif
        }

        /// <summary>Tear down.</summary>
        [TearDown]
        public void TearDown()
        {
            JsConfig.Reset();
        }

        /// <summary>Tests structs.</summary>
        [Test]
        public void Test_structs()
        {
            JsConfig<Text>.SerializeFn = text => text.ToString();

            var dto = new Foo { Content1 = "My content", Name = "My name" };

            var json = JsonSerializer.SerializeToString(dto, dto.GetType());

            Assert.That(json, Is.EqualTo("{\"Name\":\"My name\",\"Content1\":\"My content\"}"));
        }

        /// <summary>Tests structs with double quotes.</summary>
        [Test]
        public void Test_structs_with_double_quotes()
        {
            var dto = new Foo { Content1 = "My \"quoted\" content", Name = "My \"quoted\" name" };

            JsConfig<Text>.SerializeFn = text => text.ToString();
            JsConfig<Text>.DeSerializeFn = v => new Text(v);

            var json = JsonSerializer.SerializeToString(dto, dto.GetType());
            Assert.That(json, Is.EqualTo("{\"Name\":\"My \\\"quoted\\\" name\",\"Content1\":\"My \\\"quoted\\\" content\"}"));

            var foo = JsonSerializer.DeserializeFromString<Foo>(json);
            Assert.That(foo.Name, Is.EqualTo(dto.Name));
            Assert.That(foo.Content1, Is.EqualTo(dto.Content1));
        }

        /// <summary>Values that represent PersonStatus.</summary>
        public enum PersonStatus
        {
            /// <summary>An enum constant representing the none option.</summary>
            None,

            /// <summary>An enum constant representing the active agent option.</summary>
            ActiveAgent,

            /// <summary>An enum constant representing the inactive agent option.</summary>
            InactiveAgent
        }

        /// <summary>A person.</summary>
        public class Person
        {
            /// <summary>A bunch of other properties.</summary>
            /// <value>The status.</value>
            public PersonStatus Status { get; set; }
        }

        /// <summary>Tests enum overloads.</summary>
        [Test]
        public void Test_enum_overloads()
        {
            JsConfig<Person>.EmitCamelCaseNames = true;
            JsConfig.IncludeNullValues = true;
            JsConfig<PersonStatus>.SerializeFn = text => text.ToString().ToCamelCase();

            var dto = new Person { Status = PersonStatus.ActiveAgent };

            var json = JsonSerializer.SerializeToString(dto);

            Assert.That(json, Is.EqualTo("{\"status\":\"activeAgent\"}"));

            Console.WriteLine(json);

            JsConfig.Reset();
        }

        /// <summary>Dictionary of tests.</summary>
        public class TestDictionary
        {
            /// <summary>Gets or sets the dictionary.</summary>
            /// <value>The dictionary.</value>
            public Dictionary<string, string> Dictionary { get; set; }

            /// <summary>Gets or sets a list of kvps.</summary>
            /// <value>A List of kvps.</value>
            public List<KeyValuePair<string, string>> KvpList { get; set; }

            /// <summary>Gets or sets the kvp enumerable.</summary>
            /// <value>The kvp enumerable.</value>
            public IEnumerable<KeyValuePair<string, string>> KvpEnumerable { get; set; }
        }

        /// <summary>A pair.</summary>
        public class Pair
        {
            /// <summary>Gets or sets the key.</summary>
            /// <value>The key.</value>
            public string Key { get; set; }

            /// <summary>Gets or sets the value.</summary>
            /// <value>The value.</value>
            public string Value { get; set; }
        }

        /// <summary>Serializes list of kvp as poco list.</summary>
        [Test]
        public void Serializes_ListOfKvp_AsPocoList()
        {
            var map = new Dictionary<string, string> { { "foo", "bar" }, { "x", "y" } };

            var dto = new TestDictionary
            {
                Dictionary = map,
                KvpList = map.ToList(),
                KvpEnumerable = map,
            };

            var json = dto.ToJson();

            Console.WriteLine(json);

            Assert.That(json, Is.EqualTo("{\"Dictionary\":{\"foo\":\"bar\",\"x\":\"y\"},"
                + "\"KvpList\":[{\"Key\":\"foo\",\"Value\":\"bar\"},{\"Key\":\"x\",\"Value\":\"y\"}],"
                + "\"KvpEnumerable\":[{\"Key\":\"foo\",\"Value\":\"bar\"},{\"Key\":\"x\",\"Value\":\"y\"}]}"));
        }

        /// <summary>Should deserialize key value pair with int date time.</summary>
        [Test]
        public void Should_deserialize_KeyValuePair_with_int_DateTime()
        {
            var t = "{\"Key\":99,\"Value\":\"\\/Date(1320098400000+0200)\\/\"}";
            var b = JsonSerializer.DeserializeFromString<KeyValuePair<int, DateTime>>(t);
            Assert.That(b, Is.Not.Null);
            Assert.That(b.Key, Is.EqualTo(99));
            Assert.That(b.Value, Is.EqualTo(new DateTime(2011, 11, 1)));
        }

        /// <summary>A test key value pair.</summary>
        public class TestKeyValuePair
        {
            /// <summary>Gets or sets the key value.</summary>
            /// <value>The key value.</value>
            public KeyValuePair<int?, bool?> KeyValue { get; set; }

            /// <summary>Gets or sets the name.</summary>
            /// <value>The name.</value>
            public string Name { get; set; }
        }

        /// <summary>Should deserialize class with key value pair with nullables.</summary>
        [Test]
        public void Should_deserialize_class_with_KeyValuePair_with_nullables()
        {
            var t = "{\"KeyValue\":{\"Value\":true},\"Name\":\"test\"}";
            var b = JsonSerializer.DeserializeFromString<TestKeyValuePair>(t);
            Assert.That(b.KeyValue.Key, Is.Null);
            Assert.That(b.KeyValue.Value, Is.EqualTo(true));
            Assert.That(b.Name, Is.EqualTo("test"));
        }

        /// <summary>Can treat value as reference type.</summary>
        [Test]
        public void Can_TreatValueAsRefType()
        {
            JsConfig<UserStruct>.TreatValueAsRefType = true;

            var dto = new UserStruct { Id = 1, Name = "foo" };

            Assert.That(dto.ToJson(),
                Is.EqualTo("{\"Id\":1,\"Name\":\"foo\"}"));

            Assert.That(dto.ToJsv(),
                Is.EqualTo("{Id:1,Name:foo}"));
#if !XBOX && !SILVERLIGHT && !MONOTOUCH
            Assert.That(dto.ToXml(),
                Is.EqualTo("<?xml version=\"1.0\" encoding=\"utf-8\"?><UserStruct xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"http://schemas.datacontract.org/2004/07/NServiceKit.Text.Tests\"><Id>1</Id><Name>foo</Name></UserStruct>"));
#endif
            JsConfig.Reset();
        }

        /// <summary>The dangerous text 1.</summary>
        [Serializable]
        protected struct DangerousText1
        {
            /// <summary>Parses.</summary>
            /// <param name="text">The text.</param>
            /// <returns>An object.</returns>
            public static object Parse(string text)
            {
                return new DangerousText1();
            }
        }

        /// <summary>The dangerous text 2.</summary>
        [Serializable]
        protected struct DangerousText2
        {
            /// <summary>Parses.</summary>
            /// <param name="text">The text.</param>
            /// <returns>An int.</returns>
            public static int Parse(string text)
            {
                return 42;
            }
        }

        /// <summary>Static parse method will not throw on standard usage.</summary>
        [Test]
        public void StaticParseMethod_will_not_throw_on_standard_usage()
        {
            ParseStringDelegate ret = null;
            Assert.DoesNotThrow(() => ret = StaticParseMethod<Text>.Parse);
            Assert.IsNotNull(ret);
        }

        /// <summary>Static parse method will not throw on old usage.</summary>
        [Test]
        public void StaticParseMethod_will_not_throw_on_old_usage()
        {
            ParseStringDelegate ret = null;
            Assert.DoesNotThrow(() => ret = StaticParseMethod<DangerousText1>.Parse);
            Assert.IsNotNull(ret);
        }

        /// <summary>Static parse method will not throw on unstandard usage.</summary>
        [Test]
        public void StaticParseMethod_will_not_throw_on_unstandard_usage()
        {
            ParseStringDelegate ret = null;
            Assert.DoesNotThrow(() => ret = StaticParseMethod<DangerousText2>.Parse);
            Assert.IsNull(ret);
        }

        /// <summary>Tests rectangle different cultures.</summary>
        /// <param name="culture">The culture.</param>
        [Explicit("Ensure this test has proven to work, before adding it to the test suite")]
        [Test]
        [TestCase("en")]
        [TestCase("en-US")]
        [TestCase("de-CH")]
        [TestCase("de")]
        public void test_rect_different_cultures(string culture)
        {
            var currentCulture = CultureInfo.GetCultureInfo(culture);
            Thread.CurrentThread.CurrentCulture = currentCulture;
            Thread.CurrentThread.CurrentUICulture = currentCulture;
            var s = new JsonSerializer<Rect>();
            var r = new Rect(23, 34, 1024, 768);
            var interim = s.SerializeToString(r);
            var r2 = s.DeserializeFromString(interim);
            Assert.AreEqual(r, r2);
        }
    }

    /// <summary>A user structure.</summary>
    public struct UserStruct
    {
        /// <summary>Gets or sets the identifier.</summary>
        /// <value>The identifier.</value>
        public int Id { get; set; }

        /// <summary>Gets or sets the name.</summary>
        /// <value>The name.</value>
        public string Name { get; set; }
    }

    /// <summary>A rectangle.</summary>
    public struct Rect : IFormattable
    {
        /// <summary>The x coordinate.</summary>
        double x;

        /// <summary>The y coordinate.</summary>
        double y;

        /// <summary>The width.</summary>
        double width;

        /// <summary>The height.</summary>
        double height;

        /// <summary>Initializes a new instance of the StructTests class.</summary>
        /// <exception cref="ArgumentException">Thrown when one or more arguments have unsupported or
        /// illegal values.</exception>
        /// <param name="x">     The x coordinate.</param>
        /// <param name="y">     The y coordinate.</param>
        /// <param name="width"> The width.</param>
        /// <param name="height">The height.</param>
        public Rect(double x, double y, double width, double height)
        {
            if (width < 0 || height < 0)
                throw new ArgumentException("width and height must be non-negative.");
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        /// <summary>Formats the value of the current instance using the specified format.</summary>
        /// <param name="format">        The <see cref="T:System.String" /> specifying the format to use.
        /// 
        ///                     -or-
        ///                 null to use the default format defined for the type of the
        ///                 <see cref="T:System.IFormattable" /> implementation.</param>
        /// <param name="formatProvider">The <see cref="T:System.IFormatProvider" /> to use to format the
        /// value.
        /// 
        ///                     -or-
        ///                 null to obtain the numeric format information from the current locale setting
        ///                 of the operating system.</param>
        /// <returns>
        /// A <see cref="T:System.String" /> containing the value of the current instance in the
        /// specified format.
        /// </returns>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            return "{0},{1},{2},{3}".Fmt(x, y, width, height);
        }

        /// <summary>Parses.</summary>
        /// <param name="input">The input.</param>
        /// <returns>A Rect.</returns>
        public static Rect Parse(string input)
        {
            var parts = input.Split(',');
            return new Rect(
                double.Parse(parts[0]),
                double.Parse(parts[1]),
                double.Parse(parts[2]),
                double.Parse(parts[3])
            );
        }
    }
}