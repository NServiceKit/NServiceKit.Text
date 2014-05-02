using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using NUnit.Framework;
using NServiceKit.Text.Tests.Support;

namespace NServiceKit.Text.Tests.JsonTests
{
    /// <summary>A lowercase underscore tests.</summary>
    [TestFixture]
    public class LowercaseUnderscoreTests : TestBase
    {
        /// <summary>Sets the up.</summary>
        [SetUp]
        public void SetUp()
        {
            JsConfig.EmitLowercaseUnderscoreNames = true;
        }

        /// <summary>Tear down.</summary>
        [TearDown]
        public void TearDown()
        {
            JsConfig.Reset();
        }

        /// <summary>Does serialize to lowercase underscore.</summary>
        [Test]
        public void Does_serialize_To_lowercase_underscore()
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
                "{\"id\":1,\"title\":\"The Shawshank Redemption\",\"imdb_id\":\"tt0111161\",\"rating\":9.2,\"director\":\"Frank Darabont\",\"release_date\":\"\\/Date(792979200000)\\/\",\"tag_line\":\"Fear can hold you prisoner. Hope can set you free.\",\"genres\":[\"Crime\",\"Drama\"]}"));

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

        /// <summary>A with underscore.</summary>
        class WithUnderscore
        {
            /// <summary>Gets or sets the identifier of the user.</summary>
            /// <value>The identifier of the user.</value>
            public int? user_id { get; set; }
        }

        /// <summary>A with underscore digits.</summary>
        class WithUnderscoreDigits
        {
            /// <summary>Gets or sets the user identifier 0.</summary>
            /// <value>The user identifier 0.</value>
            public int? user_id_0 { get; set; }
        }

        /// <summary>Should not put double underscore.</summary>
        [Test]
        public void Should_not_put_double_underscore()
        {
            var t = new WithUnderscore { user_id = 0 };
            Assert.That(t.ToJson(), Is.EqualTo("{\"user_id\":0}"));

            var u = new WithUnderscoreDigits { user_id_0 = 0 };
            Assert.That(u.ToJson(), Is.EqualTo("{\"user_id_0\":0}"));
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

        /// <summary>A with underscore several digits.</summary>
        class WithUnderscoreSeveralDigits
        {
            /// <summary>Gets or sets the user identifier 00 11.</summary>
            /// <value>The user identifier 00 11.</value>
            public int? user_id_00_11 { get; set; }
        }

        /// <summary>Should not split digits.</summary>
        [Test]
        public void Should_not_split_digits()
        {
            var u = new WithUnderscoreSeveralDigits { user_id_00_11 = 0 };
            Assert.That(u.ToJson(), Is.EqualTo("{\"user_id_00_11\":0}"));
        }

        /// <summary>A with abbreviation and digit.</summary>
        private class WithAbbreviationAndDigit
        {
            /// <summary>Gets or sets the digest ha 1 hash.</summary>
            /// <value>The digest ha 1 hash.</value>
            public string DigestHa1Hash { get; set; }
        }

        /// <summary>Converts non uppercase acronyms nicely.</summary>
        [Test]
        public void Converts_non_uppercase_acronyms_nicely()
        {
            var a = new WithAbbreviationAndDigit { DigestHa1Hash = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa" };
            Assert.That(a.ToJson(), Is.EqualTo("{\"digest_ha1_hash\":\"aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa\"}"));
        }
    }
}