using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using NUnit.Framework;
using NServiceKit.Text.Tests.Support;

namespace NServiceKit.Text.Tests.JsonTests
{
    /// <summary>A camel case tests.</summary>
    [TestFixture]
	public class CamelCaseTests : TestBase
	{
        /// <summary>Sets the up.</summary>
		[SetUp]
		public void SetUp()
		{
			JsConfig.EmitCamelCaseNames = true;
		}

        /// <summary>Tear down.</summary>
		[TearDown]
		public void TearDown()
		{
			JsConfig.Reset();
		}

        /// <summary>Does serialize to camel case.</summary>
		[Test]
		public void Does_serialize_To_CamelCase()
		{
			var dto = new Movie
			{
				Id = 1,
				ImdbId = "tt0111161",
				Title = "The Shawshank Redemption",
				Rating = 9.2m,
				Director = "Frank Darabont",
				ReleaseDate = new DateTime(1995, 2, 17, 0, 0, 0, DateTimeKind.Utc),
				TagLine = "Fear can hold you prisoner. Hope can set you free.",
				Genres = new List<string> { "Crime", "Drama" },
			};

			var json = dto.ToJson();

			Assert.That(json, Is.EqualTo(
                "{\"id\":1,\"title\":\"The Shawshank Redemption\",\"imdbId\":\"tt0111161\",\"rating\":9.2,\"director\":\"Frank Darabont\",\"releaseDate\":\"\\/Date(792979200000)\\/\",\"tagLine\":\"Fear can hold you prisoner. Hope can set you free.\",\"genres\":[\"Crime\",\"Drama\"]}"));

			Serialize(dto);
		}

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

			Assert.That(TypeSerializer.SerializeToString(person), Is.EqualTo("{MyID:123,name:Abc}"));
			Assert.That(JsonSerializer.SerializeToString(person), Is.EqualTo("{\"MyID\":123,\"name\":\"Abc\"}"));
		}

	}
}