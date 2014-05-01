using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
#if !MONOTOUCH
using NServiceKit.ServiceInterface.ServiceModel;
#endif

namespace NServiceKit.Text.Tests
{
    /// <summary>A reported issues.</summary>
	[TestFixture]
	public class ReportedIssues
		: TestBase
	{
        /// <summary>Issue 5 can serialize dictionary with null value.</summary>
		[Test]
		public void Issue5_Can_serialize_Dictionary_with_null_value()
		{
			var map = new Dictionary<string, string> {
				{"p1","v1"},{"p2","v2"},{"p3",null},
			};

			Serialize(map);
		}

        /// <summary>A correlative data base.</summary>
		public abstract class CorrelativeDataBase
		{
            /// <summary>
            /// Initializes a new instance of the NServiceKit.Text.Tests.ReportedIssues.CorrelativeDataBase
            /// class.
            /// </summary>
			protected CorrelativeDataBase()
			{
				CorrelationIdentifier = GetNextId();
			}

            /// <summary>Gets or sets the identifier of the correlation.</summary>
            /// <value>The identifier of the correlation.</value>
			public Guid CorrelationIdentifier { get; set; }

            /// <summary>Gets the next identifier.</summary>
            /// <returns>The next identifier.</returns>
			protected static Guid GetNextId()
			{
				return Guid.NewGuid();
			}
		}

        /// <summary>A test object.</summary>
		public sealed class TestObject : CorrelativeDataBase
		{
            /// <summary>Gets or sets the type of some.</summary>
            /// <value>The type of some.</value>
			public Type SomeType { get; set; }

            /// <summary>Gets or sets some string.</summary>
            /// <value>some string.</value>
			public string SomeString { get; set; }

            /// <summary>Gets or sets a list of some types.</summary>
            /// <value>The types of the somes.</value>
			public IEnumerable<Type> SomeTypeList { get; set; }

            /// <summary>Gets or sets some type list 2.</summary>
            /// <value>some type list 2.</value>
			public IEnumerable<Type> SomeTypeList2 { get; set; }

            /// <summary>Gets or sets a list of some objects.</summary>
            /// <value>A List of some objects.</value>
			public IEnumerable<object> SomeObjectList { get; set; }
		}

        /// <summary>Serialize object with type field.</summary>
		[Test]
		public void Serialize_object_with_type_field()
		{
			var obj = new TestObject
			{
				SomeType = typeof(string),
				SomeString = "Test",
				SomeObjectList = new object[0]
			};

			Serialize(obj, includeXml: false); // xml cannot serialize Type objects.
		}

        /// <summary>Serialize object with type field 2.</summary>
		[Test]
		public void Serialize_object_with_type_field2()
		{

			var obj = new TestObject
			{
				SomeType = typeof(string),
				SomeString = "Test",
				SomeObjectList = new object[0]
			};

			var strModel = TypeSerializer.SerializeToString<object>(obj);
			Console.WriteLine("Len: " + strModel.Length + ", " + strModel);
			var toModel = TypeSerializer.DeserializeFromString<TestObject>(strModel);
		}

        /// <summary>An article.</summary>
		public class Article
		{
            /// <summary>Gets or sets the title.</summary>
            /// <value>The title.</value>
			public string title { get; set; }

            /// <summary>Gets or sets URL of the document.</summary>
            /// <value>The URL.</value>
			public string url { get; set; }

            /// <summary>Gets or sets the author.</summary>
            /// <value>The author.</value>
			public string author { get; set; }

            /// <summary>Gets or sets the identifier of the author.</summary>
            /// <value>The identifier of the author.</value>
			public string author_id { get; set; }

            /// <summary>Gets or sets the date.</summary>
            /// <value>The date.</value>
			public string date { get; set; }

            /// <summary>Gets or sets the type.</summary>
            /// <value>The type.</value>
			public string type { get; set; }
		}

        /// <summary>Serialize dictionary with backslash as last character.</summary>
		[Test]
		public void Serialize_Dictionary_with_backslash_as_last_char()
		{
			var map = new Dictionary<string, Article>
          	{
				{
					"http://www.eurogamer.net/articles/2010-09-14-vanquish-limited-edition-has-7-figurine",
					new Article
					{
						title = "Vanquish Limited Edition has 7\" figurine",
						url = "articles/2010-09-14-vanquish-limited-edition-has-7-figurine",
						author = "Wesley Yin-Poole",
						author_id = "621",
						date = "14/09/2010",
						type = "news",
					}
				},
				{
					"http://www.eurogamer.net/articles/2010-09-14-supercar-challenge-devs-next-detailed",
					new Article
					{
						title = "SuperCar Challenge dev's next detailed",
						url = "articles/2010-09-14-supercar-challenge-devs-next-detailed",
						author = "Wesley Yin-Poole",
						author_id = "621",
						date = "14/09/2010",
						type = "news",
					}
				},
				{
					"http://www.eurogamer.net/articles/2010-09-14-hmv-to-sell-dead-rising-2-a-day-early",
					new Article
					{
						title = "HMV to sell Dead Rising 2 a day early",
						url = "articles/2010-09-14-hmv-to-sell-dead-rising-2-a-day-early",
						author = "Wesley Yin-Poole",
						author_id = "621",
						date = "14/09/2010",
						type = "News",
					}
				},
          	};

			Serialize(map);

			var json = JsonSerializer.SerializeToString(map);
			var fromJson = JsonSerializer.DeserializeFromString<Dictionary<string, Article>>(json);

			Assert.That(fromJson, Has.Count.EqualTo(map.Count));
		}

        /// <summary>An item.</summary>
		public class Item
		{
            /// <summary>Gets or sets the type.</summary>
            /// <value>The type.</value>
			public int type { get; set; }

            /// <summary>Gets or sets the color.</summary>
            /// <value>The color.</value>
			public int color { get; set; }
		}

        /// <summary>A basket.</summary>
		public class Basket
		{
            /// <summary>
            /// Initializes a new instance of the NServiceKit.Text.Tests.ReportedIssues.Basket class.
            /// </summary>
			public Basket()
			{
				Items = new Dictionary<Item, int>();
			}

            /// <summary>Gets or sets the items.</summary>
            /// <value>The items.</value>
			public Dictionary<Item, int> Items { get; set; }
		}

        /// <summary>Can serialize class with typed dictionary.</summary>
		[Test]
		public void Can_Serialize_Class_with_Typed_Dictionary()
		{
			var basket = new Basket();
			basket.Items.Add(new Item { type = 1, color = 2 }, 10);
			basket.Items.Add(new Item { type = 4, color = 1 }, 20);

			Serialize(basket);
		}

        /// <summary>A book.</summary>
        public class Book
        {
            /// <summary>Gets or sets the identifier.</summary>
            /// <value>The identifier.</value>
            public UInt64 Id { get; set; }

            /// <summary>Gets or sets the user identifier that owns this item.</summary>
            /// <value>The identifier of the owner user.</value>
            public UInt64 OwnerUserId { get; set; }

            /// <summary>Gets or sets the title.</summary>
            /// <value>The title.</value>
            public String Title { get; set; }

            /// <summary>Gets or sets the description.</summary>
            /// <value>The description.</value>
            public String Description { get; set; }

            /// <summary>Gets or sets the identifier of the category.</summary>
            /// <value>The identifier of the category.</value>
            public UInt16 CategoryId { get; set; }

            /// <summary>Gets or sets the created date time.</summary>
            /// <value>The created date time.</value>
            public DateTime CreatedDateTime { get; set; }

            /// <summary>Gets or sets the icon.</summary>
            /// <value>The icon.</value>
            public Byte Icon { get; set; }

            /// <summary>Gets or sets a value indicating whether the canceled.</summary>
            /// <value>true if canceled, false if not.</value>
            public Boolean Canceled { get; set; }
        }
#if !MONOTOUCH
        /// <summary>A book response.</summary>
        public class BookResponse : IHasResponseStatus
        {
            /// <summary>Gets or sets the book.</summary>
            /// <value>The book.</value>
            public Book Book { get; set; }

            /// <summary>Gets or sets the response status.</summary>
            /// <value>The response status.</value>
            public ResponseStatus ResponseStatus { get; set; }
        }

        /// <summary>Gets the book.</summary>
        /// <returns>The book.</returns>
        public object GetBook()
        {
            var response = new BookResponse();
            return response;
        }

        /// <summary>Does not serialize typeinfo for concrete types.</summary>
        [Test]
        public void Does_not_serialize_typeinfo_for_concrete_types()
        {
            var json = GetBook().ToJson();
            Console.WriteLine(json);
            Assert.That(json.IndexOf("__"), Is.EqualTo(-1));

        	var jsv = GetBook().ToJsv();
			Assert.That(jsv.IndexOf("__"), Is.EqualTo(-1));
		}
#endif

        /// <summary>A text tags.</summary>
		public class TextTags
		{
            /// <summary>Gets or sets the text.</summary>
            /// <value>The text.</value>
			public string Text { get; set; }

            /// <summary>Gets or sets the tags.</summary>
            /// <value>The tags.</value>
			public string[] Tags { get; set; }
		}

        /// <summary>Can serialize sweedish characters.</summary>
		[Test]
		public void Can_serialize_sweedish_chars()
		{
			var dto = new TextTags { Text = "Olle är en ÖL ål", Tags = new[] { "öl", "ål", "mål" } };
			Serialize(dto);
		}

        /// <summary>
        /// Objects do not survive round trips via string dictionary due to double quoted properties.
        /// </summary>
        [Test]
        public void Objects_Do_Not_Survive_RoundTrips_Via_StringStringDictionary_Due_To_DoubleQuoted_Properties()
        {
            var book = new Book();
            book.Id = 1234;
            book.Title = "NServiceKit in Action";
            book.CategoryId = 16;
            book.Description = "Manning eBooks";


            var json = book.ToJson();
            Console.WriteLine("Book to Json: " + json);

            var dictionary = json.FromJson<Dictionary<string, string>>();
            Console.WriteLine("Json to Dictionary: " + dictionary.Dump());

            var fromDictionary = dictionary.ToJson();
            Console.WriteLine("Json from Dictionary: " + fromDictionary);

			var fromJsonViaDictionary = fromDictionary.FromJson<Book>();

            Assert.AreEqual(book.Description, fromJsonViaDictionary.Description);
            Assert.AreEqual(book.Id, fromJsonViaDictionary.Id);
            Assert.AreEqual(book.Title, fromJsonViaDictionary.Title);
            Assert.AreEqual(book.CategoryId, fromJsonViaDictionary.CategoryId);
        }

        /// <summary>A test.</summary>
		public class Test
		{
            /// <summary>Gets or sets the items.</summary>
            /// <value>The items.</value>
			public IDictionary<string, string> Items { get; set; }

            /// <summary>Gets or sets the test string.</summary>
            /// <value>The test string.</value>
			public string TestString { get; set; }
		}

        /// <summary>Does trailing backslashes.</summary>
		[Test]
		public void Does_Trailing_Backslashes()
		{
			var test = new Test {
				TestString = "Test",
				Items = new Dictionary<string, string> { { "foo", "bar\\" } }
			};

			var serialized = JsonSerializer.SerializeToString(test);
			Console.WriteLine(serialized);
			var deserialized = JsonSerializer.DeserializeFromString<Test>(serialized);

			Assert.That(deserialized.TestString, Is.EqualTo("Test")); // deserialized.TestString is NULL
		}

        /// <summary>Deserialize correctly when last item is null in array.</summary>
        [Test]
        public void Deserialize_Correctly_When_Last_Item_Is_Null_in_array()
        {
            var arrayOfInt = new int?[2] {1, null };
            var serialized = TypeSerializer.SerializeToString(arrayOfInt);
            Console.WriteLine(serialized);
            var deserialized = TypeSerializer.DeserializeFromString<int?[]>(serialized);
            Assert.That(deserialized, Is.EqualTo(arrayOfInt));
        }

        /// <summary>Deserialize correctly when last item is null in list.</summary>
        [Test]
        public void Deserialize_Correctly_When_Last_Item_Is_Null_in_list()
        {
            var arrayOfInt = new List<int?> { 1, null };
            var serialized = TypeSerializer.SerializeToString(arrayOfInt);
            Console.WriteLine(serialized);
            var deserialized = TypeSerializer.DeserializeFromString<List<int?>>(serialized);
            Assert.That(deserialized, Is.EqualTo(arrayOfInt));
        }

        /// <summary>Deserialize correctly when last item is null in string list property.</summary>
       [Test]
        public void Deserialize_Correctly_When_Last_Item_Is_Null_in_String_list_prop()
        {
            var type = new TestMappedList() { StringListProp = new List<String> { "hello", null } };
            var serialized = TypeSerializer.SerializeToString(type);
            Console.WriteLine(serialized);
            var deserialized = TypeSerializer.DeserializeFromString<TestMappedList>(serialized);
            Assert.That(deserialized.StringListProp, Is.EqualTo(type.StringListProp));
        }

        /// <summary>Deserialize correctly when last item is null in string array property.</summary>
        [Test]
        public void Deserialize_Correctly_When_Last_Item_Is_Null_in_String_array_prop()
        {
            var type = new TestMappedList() { StringArrayProp = new string [] { "hello", null } };
            var serialized = TypeSerializer.SerializeToString(type);
            Console.WriteLine(serialized);
            var deserialized = TypeSerializer.DeserializeFromString<TestMappedList>(serialized);
            Assert.That(deserialized.StringArrayProp, Is.EqualTo(type.StringArrayProp));
        }

        /// <summary>Deserialize correctly when last item is null in int array property.</summary>
        [Test]
        public void Deserialize_Correctly_When_Last_Item_Is_Null_in_Int_array_prop()
        {
            var type = new TestMappedList() { IntArrayProp = new List<int?> { 1, null } };
            var serialized = TypeSerializer.SerializeToString(type);
            Console.WriteLine(serialized);
            var deserialized = TypeSerializer.DeserializeFromString<TestMappedList>(serialized);
            Assert.That(deserialized.IntArrayProp, Is.EqualTo(type.IntArrayProp));
        }
	}

    /// <summary>List of test mapped.</summary>
    public class TestMappedList
    {
        /// <summary>Gets or sets the string list property.</summary>
        /// <value>The string list property.</value>
        public List<String> StringListProp { get; set; }

        /// <summary>Gets or sets the string array property.</summary>
        /// <value>The string array property.</value>
        public string[] StringArrayProp { get; set; }

        /// <summary>Gets or sets the int array property.</summary>
        /// <value>The int array property.</value>
        public List<int?> IntArrayProp { get; set; }
    }
}