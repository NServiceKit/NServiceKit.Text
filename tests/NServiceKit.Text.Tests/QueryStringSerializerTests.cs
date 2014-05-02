using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using Funq;
using NUnit.Framework;
using NServiceKit.ServiceHost;
using NServiceKit.ServiceInterface.Testing;
using NServiceKit.WebHost.Endpoints;
using NServiceKit.WebHost.Endpoints.Support.Mocks;

namespace NServiceKit.Text.Tests
{
    /// <summary>A c.</summary>
	public class C
	{
        /// <summary>Gets or sets a.</summary>
        /// <value>a.</value>
		public int? A { get; set; }

        /// <summary>Gets or sets the b.</summary>
        /// <value>The b.</value>
		public int? B { get; set; }
	}

    /// <summary>A query string serializer tests.</summary>
	[TestFixture]
	public class QueryStringSerializerTests
	{
        /// <summary>A d.</summary>
        class D
        {
            /// <summary>Gets or sets a.</summary>
            /// <value>a.</value>
            public string A { get; set; }

            /// <summary>Gets or sets the b.</summary>
            /// <value>The b.</value>
            public string B { get; set; }
        }

        /// <summary>Can serialize query string.</summary>
		[Test]
		public void Can_serialize_query_string()
		{
			Assert.That(QueryStringSerializer.SerializeToString(new C { A = 1, B = 2 }),
				Is.EqualTo("A=1&B=2"));

			Assert.That(QueryStringSerializer.SerializeToString(new C { A = null, B = 2 }),
				Is.EqualTo("B=2"));
		}

        /// <summary>Can serialize unicode query string.</summary>
        [Test]
        public void Can_Serialize_Unicode_Query_String()
        {
            Assert.That(QueryStringSerializer.SerializeToString(new D { A = "믬㼼摄䰸蠧蛛㙷뇰믓堐锗멮ᙒ덃", B = "八敁喖䉬ڵẀ똦⌀羭䥀主䧒蚭㾐타" }), 
                Is.EqualTo("A=%eb%af%ac%e3%bc%bc%e6%91%84%e4%b0%b8%e8%a0%a7%e8%9b%9b%e3%99%b7%eb%87%b0%eb%af%93%e5%a0" +
                "%90%e9%94%97%eb%a9%ae%e1%99%92%eb%8d%83&B=%e5%85%ab%e6%95%81%e5%96%96%e4%89%ac%da%b5%e1%ba%80%eb%98%a6%e2%8c%80%e7%be%ad%e4" +
                "%a5%80%e4%b8%bb%e4%a7%92%e8%9a%ad%e3%be%90%ed%83%80"));

            Assert.That(QueryStringSerializer.SerializeToString(new D { A = "崑⨹堡ꁀᢖ㤹ì㭡줪銬", B = null }),
                Is.EqualTo("A=%e5%b4%91%e2%a8%b9%e5%a0%a1%ea%81%80%e1%a2%96%e3%a4%b9%c3%ac%e3%ad%a1%ec%a4%aa%e9%8a%ac"));
        }

        /// <summary>An empty.</summary>
        class Empty {}

        /// <summary>Can serialize empty object.</summary>
        [Test]
        public void Can_serialize_empty_object()
        {
            Assert.That(QueryStringSerializer.SerializeToString(new Empty()), Is.Empty);
        }

        /// <summary>Can serialize newline.</summary>
	    [Test]
	    public void Can_serialize_newline()
	    {
            Assert.That(QueryStringSerializer.SerializeToString(new {newline = "\r\n"}), Is.EqualTo("newline=%0d%0a"));
	    }

        /// <summary>Can serialize array of strings with colon.</summary>
        [Test]
        public void Can_serialize_array_of_strings_with_colon()
        {
            var t = new List<string>();
            t.Add("Foo:Bar");
            t.Add("Get:Out");
            Assert.That(QueryStringSerializer.SerializeToString(new { list = t }), Is.EqualTo("list=Foo%3aBar,Get%3aOut"));
        }

        /// <summary>Can serialize tab.</summary>
	    [Test]
	    public void Can_serialize_tab()
	    {
            Assert.That(QueryStringSerializer.SerializeToString(new { tab = "\t" }), Is.EqualTo("tab=%09"));
	    }

        /// <summary>
        /// NOTE: QueryStringSerializer doesn't have Deserialize, but this is how QS is parsed in
        /// NServiceKit.
        /// </summary>
	    [Test]
	    public void Can_deserialize_query_string_nullableInt_null_yields_null()
	    {
	    	Assert.That(NServiceKit.Text.Common.DeserializeBuiltin<int?>.Parse(null), Is.EqualTo(null));
	    }

        /// <summary>Can deserialize query string nullable int empty yields null.</summary>
	    [Test]
	    public void Can_deserialize_query_string_nullableInt_empty_yields_null()
	    {
	    	Assert.That(NServiceKit.Text.Common.DeserializeBuiltin<int?>.Parse(string.Empty), Is.EqualTo(null));
	    }

        /// <summary>Can deserialize query string nullable int values yields null.</summary>
	    [Test]
	    public void Can_deserialize_query_string_nullableInt_intValues_yields_null()
	    {
	    	Assert.That(NServiceKit.Text.Common.DeserializeBuiltin<int?>.Parse(int.MaxValue.ToString()), Is.EqualTo(int.MaxValue));
            Assert.That(NServiceKit.Text.Common.DeserializeBuiltin<int?>.Parse(int.MinValue.ToString()), Is.EqualTo(int.MinValue));
	    	Assert.That(NServiceKit.Text.Common.DeserializeBuiltin<int?>.Parse(0.ToString()), Is.EqualTo(0));
	    	Assert.That(NServiceKit.Text.Common.DeserializeBuiltin<int?>.Parse((-1).ToString()), Is.EqualTo(-1));
	    	Assert.That(NServiceKit.Text.Common.DeserializeBuiltin<int?>.Parse(1.ToString()), Is.EqualTo(1));
	    }

        /// <summary>Can deserialize query string nullable int na n throws.</summary>
	    [Test]
	    public void Can_deserialize_query_string_nullableInt_NaN_throws()
	    {
	    	Assert.Throws(typeof(FormatException), delegate { NServiceKit.Text.Common.DeserializeBuiltin<int?>.Parse("NaN"); });
    	}

        /// <summary>Deos serialize query strings.</summary>
	    [Test]
	    public void Deos_serialize_QueryStrings()
	    {
            var testPocos = new TestPocos { ListOfA = new List<A> { new A { ListOfB = new List<B> { new B { Property = "prop1" }, new B { Property = "prop2" } } } } };

            Assert.That(QueryStringSerializer.SerializeToString(testPocos), Is.EqualTo(
                "ListOfA={ListOfB:[{Property:prop1},{Property:prop2}]}"));
            
            Assert.That(QueryStringSerializer.SerializeToString(new[] { 1, 2, 3 }), Is.EqualTo(
                "[1,2,3]"));

            Assert.That(QueryStringSerializer.SerializeToString(new[] { "AA", "BB", "CC" }), Is.EqualTo(
                "[AA,BB,CC]"));
	    }

        /// <summary>Can serialize quoted strings.</summary>
        [Test]
        public void Can_serialize_quoted_strings()
        {
            Assert.That(QueryStringSerializer.SerializeToString(new B { Property = "\"quoted content\"" }), Is.EqualTo("Property=%22%22quoted%20content%22%22"));
            Assert.That(QueryStringSerializer.SerializeToString(new B { Property = "\"quoted content, and with a comma\"" }), Is.EqualTo("Property=%22%22quoted%20content,%20and%20with%20a%20comma%22%22"));
        }

        /// <summary>String to poco.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="str">The.</param>
        /// <returns>A T.</returns>
        private T StringToPoco<T>(string str)
        {
            var testAppHost = new TestAppHost(new Container(), GetType().Assembly);
            NameValueCollection queryString = HttpUtility.ParseQueryString(str);
            var restPath = new RestPath(typeof(T), "/query", "GET, POST");
            var restHandler = new RestHandler()
            {
                RestPath = restPath
            };
            var httpReq = new HttpRequestMock("query", "GET", "application/json", "query", queryString,
                                              new MemoryStream(), new NameValueCollection());
            var request = (T)restHandler.CreateRequest(httpReq, "query");
            return request;
        }

        /// <summary>Can deserialize quoted strings.</summary>
        [Test]
        public void Can_deserialize_quoted_strings()
        {
            Assert.That(StringToPoco<B>("Property=%22%22quoted%20content%22%22").Property, Is.EqualTo("\"\"quoted content\"\""));
            Assert.That(StringToPoco<B>("Property=%22%22quoted%20content,%20and%20with%20a%20comma%22%22").Property, Is.EqualTo("\"\"quoted content, and with a comma\"\""));
        }

        /// <summary>Can serialize with comma in property in list.</summary>
        [Test]
        public void Can_serialize_with_comma_in_property_in_list()
        {
            var testPocos = new TestPocos
                {
                    ListOfA = new List<A> {new A {ListOfB = new List<B> {new B {Property = "Doe, John", Property2 = "Doe", Property3 = "John"}}}}
                };
            Assert.That(QueryStringSerializer.SerializeToString(testPocos), Is.EqualTo("ListOfA={ListOfB:[{Property:%22Doe,%20John%22,Property2:Doe,Property3:John}]}"));
        }

        /// <summary>
        /// Can deserialize with comma in property in list from query string serializer.
        /// </summary>
        [Test]
        public void Can_deserialize_with_comma_in_property_in_list_from_QueryStringSerializer()
        {
            var testPocos = new TestPocos
            {
                ListOfA = new List<A> { new A { ListOfB = new List<B> { new B { Property = "Doe, John", Property2 = "Doe", Property3 = "John" } } } }
            };
            var str = QueryStringSerializer.SerializeToString(testPocos);
            var poco = StringToPoco<TestPocos>(str);
            Assert.That(poco.ListOfA[0].ListOfB[0].Property, Is.EqualTo("Doe, John"));
            Assert.That(poco.ListOfA[0].ListOfB[0].Property2, Is.EqualTo("Doe"));
            Assert.That(poco.ListOfA[0].ListOfB[0].Property3, Is.EqualTo("John"));
        }

        /// <summary>Can deserialize with comma in property in list from static.</summary>
        [Test]
        public void Can_deserialize_with_comma_in_property_in_list_from_static()
        {
            var str = "ListOfA={ListOfB:[{Property:\"Doe,%20John\",Property2:Doe,Property3:John}]}";
            var poco = StringToPoco<TestPocos>(str);
            Assert.That(poco.ListOfA[0].ListOfB[0].Property, Is.EqualTo("Doe, John"));
            Assert.That(poco.ListOfA[0].ListOfB[0].Property2, Is.EqualTo("Doe"));
            Assert.That(poco.ListOfA[0].ListOfB[0].Property3, Is.EqualTo("John"));
        }

        /// <summary>A test pocos.</summary>
	    public class TestPocos
        {
            /// <summary>Gets or sets the list of a.</summary>
            /// <value>The list of a.</value>
            public List<A> ListOfA { get; set; }
        }

        /// <summary>An a.</summary>
        public class A
        {
            /// <summary>Gets or sets the list of b.</summary>
            /// <value>The list of b.</value>
            public List<B> ListOfB { get; set; }
        }

        /// <summary>A b.</summary>
        public class B
        {
            /// <summary>Gets or sets the property.</summary>
            /// <value>The property.</value>
            public string Property { get; set; }

            /// <summary>Gets or sets the property 2.</summary>
            /// <value>The property 2.</value>
            public string Property2 { get; set; }

            /// <summary>Gets or sets the property 3.</summary>
            /// <value>The property 3.</value>
            public string Property3 { get; set; }
        }
    }
}