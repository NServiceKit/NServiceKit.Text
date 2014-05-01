using NUnit.Framework;

namespace NServiceKit.Text.Tests.JsonTests
{
    /// <summary>The anonymous deserialization tests.</summary>
	[TestFixture]
	public class AnonymousDeserializationTests
		: TestBase
	{
        /// <summary>An item.</summary>
		private class Item
		{
            /// <summary>Gets or sets the int value.</summary>
            /// <value>The int value.</value>
			public int IntValue { get; set; }

            /// <summary>Gets or sets the string value.</summary>
            /// <value>The string value.</value>
			public string StringValue { get; set; }

            /// <summary>Creates a new Item.</summary>
            /// <returns>An Item.</returns>
			public static Item Create()
			{
				return new Item { IntValue = 42, StringValue = "Foo" };
			}
		}

        /// <summary>Can deserialize to anonymous type.</summary>
		[Test]
		public void Can_deserialize_to_anonymous_type()
		{
			var original = Item.Create();
			var json = JsonSerializer.SerializeToString(original);
			
			var item = DeserializeAnonymousType(new { IntValue = default(int), StringValue = default(string) }, json);

			Assert.That(item.IntValue, Is.EqualTo(42));
			Assert.That(item.StringValue, Is.EqualTo("Foo"));
		}

        /// <summary>Deserialize anonymous type.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="template">The template.</param>
        /// <param name="json">    The JSON.</param>
        /// <returns>A T.</returns>
		private static T DeserializeAnonymousType<T>(T template, string json) where T : class
		{
			TypeConfig<T>.EnableAnonymousFieldSetters = true;
			return JsonSerializer.DeserializeFromString(json, template.GetType()) as T;
		}
	}
}