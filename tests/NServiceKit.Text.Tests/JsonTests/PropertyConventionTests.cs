using NUnit.Framework;

namespace NServiceKit.Text.Tests.JsonTests
{
    /// <summary>A property convention tests.</summary>
    [TestFixture]
    public class PropertyConventionTests : TestBase
    {
        /// <summary>Does require exact match by default.</summary>
        [Test]
        public void Does_require_exact_match_by_default()
        {
            Assert.That(JsConfig.PropertyConvention, Is.EqualTo(JsonPropertyConvention.ExactMatch));
            const string bad = "{ \"total_count\":45, \"was_published\":true }";
            const string good = "{ \"TotalCount\":45, \"WasPublished\":true }";
            
            var actual = JsonSerializer.DeserializeFromString<Example>(bad);
            Assert.That(actual, Is.Not.Null);
            Assert.That(actual.TotalCount, Is.EqualTo(0));
            Assert.That(actual.WasPublished, Is.EqualTo(false));

            actual = JsonSerializer.DeserializeFromString<Example>(good);
            Assert.That(actual, Is.Not.Null);
            Assert.That(actual.TotalCount, Is.EqualTo(45));
            Assert.That(actual.WasPublished, Is.EqualTo(true));
        }

        /// <summary>Does deserialize from inexact source when lenient convention is used.</summary>
        [Test]
        public void Does_deserialize_from_inexact_source_when_lenient_convention_is_used()
        {
            JsConfig.PropertyConvention = JsonPropertyConvention.Lenient;
            const string bad = "{ \"total_count\":45, \"was_published\":true }";
            
            var actual = JsonSerializer.DeserializeFromString<Example>(bad);
            Assert.That(actual, Is.Not.Null);
            Assert.That(actual.TotalCount, Is.EqualTo(45));
            Assert.That(actual.WasPublished, Is.EqualTo(true));
            
            JsConfig.Reset();
        }

        /// <summary>An example.</summary>
        public class Example
        {
            /// <summary>Gets or sets the number of totals.</summary>
            /// <value>The total number of count.</value>
            public int TotalCount { get; set; }

            /// <summary>Gets or sets a value indicating whether the was published.</summary>
            /// <value>true if was published, false if not.</value>
            public bool WasPublished { get; set; }
        }

        /// <summary>A hyphens.</summary>
        public class Hyphens
        {
            /// <summary>Gets or sets the snippet format.</summary>
            /// <value>The snippet format.</value>
            public string SnippetFormat { get; set; }

            /// <summary>Gets or sets the number of. </summary>
            /// <value>The total.</value>
            public int Total { get; set; }

            /// <summary>Gets or sets the start.</summary>
            /// <value>The start.</value>
            public int Start { get; set; }

            /// <summary>Gets or sets the length of the page.</summary>
            /// <value>The length of the page.</value>
            public int PageLength { get; set; }
        }

        /// <summary>Can deserialize hyphens.</summary>
        [Test]
        public void Can_deserialize_hyphens()
        {
            var json = @"{
                ""snippet-format"":""raw"",
                ""total"":1,
                ""start"":1,
                ""page-length"":200
             }";

            var map = JsonObject.Parse(json);
            Assert.That(map["snippet-format"], Is.EqualTo("raw"));
            Assert.That(map["total"], Is.EqualTo("1"));
            Assert.That(map["start"], Is.EqualTo("1"));
            Assert.That(map["page-length"], Is.EqualTo("200"));

            JsConfig.PropertyConvention = JsonPropertyConvention.Lenient;

            var dto = json.FromJson<Hyphens>();

            Assert.That(dto.SnippetFormat, Is.EqualTo("raw"));
            Assert.That(dto.Total, Is.EqualTo(1));
            Assert.That(dto.Start, Is.EqualTo(1));
            Assert.That(dto.PageLength, Is.EqualTo(200));

            JsConfig.Reset();
        }
    }
}