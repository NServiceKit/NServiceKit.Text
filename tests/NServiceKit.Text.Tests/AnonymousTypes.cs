using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace NServiceKit.Text.Tests
{
    /// <summary>The anonymous types.</summary>
	[TestFixture]
	public class AnonymousTypes
		: TestBase
	{
        /// <summary>Can serialize anonymous types.</summary>
		[Test]
		public void Can_serialize_anonymous_types()
		{
			Serialize(new { Id = 1, Name = "Name", IntList = new[] { 1, 2, 3 } }, includeXml: false); // xmlserializer cannot serialize anonymous types.
		}

        /// <summary>Can serialize anonymous type and read as string dictionary.</summary>
		[Test]
		public void Can_serialize_anonymous_type_and_read_as_string_Dictionary()
		{
			var json = JsonSerializer.SerializeToString(
				new { Id = 1, Name = "Name", IntList = new[] { 1, 2, 3 } });

			Console.WriteLine("JSON: " + json);

			var map = JsonSerializer.DeserializeFromString<Dictionary<string, string>>(json);

			Console.WriteLine("MAP: " + map.Dump());
		}

        /// <summary>A test object.</summary>
        public class TestObj
        {
            /// <summary>Gets or sets the title 1.</summary>
            /// <value>The title 1.</value>
            public string Title1 { get; set; }

            /// <summary>Gets or sets the title 2.</summary>
            /// <value>The title 2.</value>
            public object Title2 { get; set; }
        }

        /// <summary>Escapes string in object correctly.</summary>
        [Test]
        public void Escapes_string_in_object_correctly()
        {
            const string expectedValue = @"a\nb";
            string json = string.Format(@"{{""Title1"":""{0}"",""Title2"":""{0}""}}", expectedValue);

            var value = JsonSerializer.DeserializeFromString<TestObj>(json);

            value.PrintDump();

            Assert.That(value.Title1, Is.EqualTo(value.Title2.ToString()));
        }
	}

}