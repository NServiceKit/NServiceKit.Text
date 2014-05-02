using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Runtime.Serialization;
using System.Threading;
using System.Xml;
using NUnit.Framework;
using NServiceKit.Common.Extensions;
using NServiceKit.Text.Jsv;

namespace NServiceKit.Text.Tests
{
    /// <summary>An adhoc model tests.</summary>
    [TestFixture]
    public class AdhocModelTests
        : TestBase
    {
        /// <summary>Values that represent FlowPostType.</summary>
        public enum FlowPostType
        {
            /// <summary>An enum constant representing the content option.</summary>
            Content,

            /// <summary>An enum constant representing the text option.</summary>
            Text,

            /// <summary>An enum constant representing the promo option.</summary>
            Promo,
        }

        /// <summary>A flow post transient.</summary>
        public class FlowPostTransient
        {
            /// <summary>
            /// Initializes a new instance of the NServiceKit.Text.Tests.AdhocModelTests.FlowPostTransient
            /// class.
            /// </summary>
            public FlowPostTransient()
            {
                this.TrackUrns = new List<string>();
            }

            /// <summary>Gets or sets the identifier.</summary>
            /// <value>The identifier.</value>
            public long Id { get; set; }

            /// <summary>Gets or sets the URN.</summary>
            /// <value>The URN.</value>
            public string Urn { get; set; }

            /// <summary>Gets or sets the identifier of the user.</summary>
            /// <value>The identifier of the user.</value>
            public Guid UserId { get; set; }

            /// <summary>Gets or sets the Date/Time of the date added.</summary>
            /// <value>The date added.</value>
            public DateTime DateAdded { get; set; }

            /// <summary>Gets or sets the Date/Time of the date modified.</summary>
            /// <value>The date modified.</value>
            public DateTime DateModified { get; set; }

            /// <summary>Gets or sets the identifier of the target user.</summary>
            /// <value>The identifier of the target user.</value>
            public Guid? TargetUserId { get; set; }

            /// <summary>Gets or sets the identifier of the forwarded post.</summary>
            /// <value>The identifier of the forwarded post.</value>
            public long? ForwardedPostId { get; set; }

            /// <summary>Gets or sets the identifier of the origin user.</summary>
            /// <value>The identifier of the origin user.</value>
            public Guid OriginUserId { get; set; }

            /// <summary>Gets or sets the name of the origin user.</summary>
            /// <value>The name of the origin user.</value>
            public string OriginUserName { get; set; }

            /// <summary>Gets or sets the identifier of the source user.</summary>
            /// <value>The identifier of the source user.</value>
            public Guid SourceUserId { get; set; }

            /// <summary>Gets or sets the name of the source user.</summary>
            /// <value>The name of the source user.</value>
            public string SourceUserName { get; set; }

            /// <summary>Gets or sets the subject URN.</summary>
            /// <value>The subject URN.</value>
            public string SubjectUrn { get; set; }

            /// <summary>Gets or sets the content URN.</summary>
            /// <value>The content URN.</value>
            public string ContentUrn { get; set; }

            /// <summary>Gets or sets the track urns.</summary>
            /// <value>The track urns.</value>
            public IList<string> TrackUrns { get; set; }

            /// <summary>Gets or sets the caption.</summary>
            /// <value>The caption.</value>
            public string Caption { get; set; }

            /// <summary>Gets or sets the identifier of the caption user.</summary>
            /// <value>The identifier of the caption user.</value>
            public Guid CaptionUserId { get; set; }

            /// <summary>Gets or sets the name of the caption source.</summary>
            /// <value>The name of the caption source.</value>
            public string CaptionSourceName { get; set; }

            /// <summary>Gets or sets the forwarded post URN.</summary>
            /// <value>The forwarded post URN.</value>
            public string ForwardedPostUrn { get; set; }

            /// <summary>Gets or sets the type of the post.</summary>
            /// <value>The type of the post.</value>
            public FlowPostType PostType { get; set; }

            /// <summary>Gets or sets the identifier of the on behalf of user.</summary>
            /// <value>The identifier of the on behalf of user.</value>
            public Guid? OnBehalfOfUserId { get; set; }

            /// <summary>Creates a new FlowPostTransient.</summary>
            /// <returns>A FlowPostTransient.</returns>
            public static FlowPostTransient Create()
            {
                return new FlowPostTransient
                {
                    Caption = "Caption",
                    CaptionSourceName = "CaptionSourceName",
                    CaptionUserId = Guid.NewGuid(),
                    ContentUrn = "ContentUrn",
                    DateAdded = DateTime.Now,
                    DateModified = DateTime.Now,
                    ForwardedPostId = 1,
                    ForwardedPostUrn = "ForwardedPostUrn",
                    Id = 1,
                    OnBehalfOfUserId = Guid.NewGuid(),
                    OriginUserId = Guid.NewGuid(),
                    OriginUserName = "OriginUserName",
                    PostType = FlowPostType.Content,
                    SourceUserId = Guid.NewGuid(),
                    SourceUserName = "SourceUserName",
                    SubjectUrn = "SubjectUrn ",
                    TargetUserId = Guid.NewGuid(),
                    TrackUrns = new List<string> { "track1", "track2" },
                    Urn = "Urn ",
                    UserId = Guid.NewGuid(),
                };
            }

            /// <summary>
            /// Determines whether the specified <see cref="T:System.Object" /> is equal to the current
            /// <see cref="T:System.Object" />.
            /// </summary>
            /// <param name="other">The flow post transient to compare to this object.</param>
            /// <returns>
            /// true if the specified <see cref="T:System.Object" /> is equal to the current
            /// <see cref="T:System.Object" />; otherwise, false.
            /// </returns>
            public bool Equals(FlowPostTransient other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return other.Id == Id && Equals(other.Urn, Urn) && other.UserId.Equals(UserId) && other.DateAdded.RoundToMs().Equals(DateAdded.RoundToMs()) && other.DateModified.RoundToMs().Equals(DateModified.RoundToMs()) && other.TargetUserId.Equals(TargetUserId) && other.ForwardedPostId.Equals(ForwardedPostId) && other.OriginUserId.Equals(OriginUserId) && Equals(other.OriginUserName, OriginUserName) && other.SourceUserId.Equals(SourceUserId) && Equals(other.SourceUserName, SourceUserName) && Equals(other.SubjectUrn, SubjectUrn) && Equals(other.ContentUrn, ContentUrn) && TrackUrns.EquivalentTo(other.TrackUrns) && Equals(other.Caption, Caption) && other.CaptionUserId.Equals(CaptionUserId) && Equals(other.CaptionSourceName, CaptionSourceName) && Equals(other.ForwardedPostUrn, ForwardedPostUrn) && Equals(other.PostType, PostType) && other.OnBehalfOfUserId.Equals(OnBehalfOfUserId);
            }

            /// <summary>
            /// Determines whether the specified <see cref="T:System.Object" /> is equal to the current
            /// <see cref="T:System.Object" />.
            /// </summary>
            /// <param name="obj">The <see cref="T:System.Object" /> to compare with the current
            /// <see cref="T:System.Object" />.</param>
            /// <returns>
            /// true if the specified <see cref="T:System.Object" /> is equal to the current
            /// <see cref="T:System.Object" />; otherwise, false.
            /// </returns>
            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != typeof(FlowPostTransient)) return false;
                return Equals((FlowPostTransient)obj);
            }

            /// <summary>Serves as a hash function for a particular type.</summary>
            /// <returns>A hash code for the current <see cref="T:System.Object" />.</returns>
            public override int GetHashCode()
            {
                unchecked
                {
                    int result = Id.GetHashCode();
                    result = (result * 397) ^ (Urn != null ? Urn.GetHashCode() : 0);
                    result = (result * 397) ^ UserId.GetHashCode();
                    result = (result * 397) ^ DateAdded.GetHashCode();
                    result = (result * 397) ^ DateModified.GetHashCode();
                    result = (result * 397) ^ (TargetUserId.HasValue ? TargetUserId.Value.GetHashCode() : 0);
                    result = (result * 397) ^ (ForwardedPostId.HasValue ? ForwardedPostId.Value.GetHashCode() : 0);
                    result = (result * 397) ^ OriginUserId.GetHashCode();
                    result = (result * 397) ^ (OriginUserName != null ? OriginUserName.GetHashCode() : 0);
                    result = (result * 397) ^ SourceUserId.GetHashCode();
                    result = (result * 397) ^ (SourceUserName != null ? SourceUserName.GetHashCode() : 0);
                    result = (result * 397) ^ (SubjectUrn != null ? SubjectUrn.GetHashCode() : 0);
                    result = (result * 397) ^ (ContentUrn != null ? ContentUrn.GetHashCode() : 0);
                    result = (result * 397) ^ (TrackUrns != null ? TrackUrns.GetHashCode() : 0);
                    result = (result * 397) ^ (Caption != null ? Caption.GetHashCode() : 0);
                    result = (result * 397) ^ CaptionUserId.GetHashCode();
                    result = (result * 397) ^ (CaptionSourceName != null ? CaptionSourceName.GetHashCode() : 0);
                    result = (result * 397) ^ (ForwardedPostUrn != null ? ForwardedPostUrn.GetHashCode() : 0);
                    result = (result * 397) ^ PostType.GetHashCode();
                    result = (result * 397) ^ (OnBehalfOfUserId.HasValue ? OnBehalfOfUserId.Value.GetHashCode() : 0);
                    return result;
                }
            }
        }

        /// <summary>Sets the up.</summary>
        [SetUp]
        public void SetUp()
        {
            JsConfig.Reset();
        }

        /// <summary>Can deserialize text.</summary>
        [Test]
        public void Can_Deserialize_text()
        {
            var dtoString = "[{Id:1,Urn:urn:post:3a944f18-920c-498a-832d-cf38fed3d0d7/1,UserId:3a944f18920c498a832dcf38fed3d0d7,DateAdded:2010-02-17T12:04:45.2845615Z,DateModified:2010-02-17T12:04:45.2845615Z,OriginUserId:3a944f18920c498a832dcf38fed3d0d7,OriginUserName:testuser1,SourceUserId:3a944f18920c498a832dcf38fed3d0d7,SourceUserName:testuser1,SubjectUrn:urn:track:1,ContentUrn:urn:track:1,TrackUrns:[],CaptionUserId:3a944f18920c498a832dcf38fed3d0d7,CaptionSourceName:testuser1,PostType:Content}]";
            var fromString = TypeSerializer.DeserializeFromString<List<FlowPostTransient>>(dtoString);
        }

        /// <summary>Can serialize single flow post transient.</summary>
        [Test]
        public void Can_Serialize_single_FlowPostTransient()
        {
            var dto = FlowPostTransient.Create();
            SerializeAndCompare(dto);
        }

        /// <summary>Can serialize jsv dates.</summary>
        [Test]
        public void Can_serialize_jsv_dates()
        {
            var now = DateTime.Now;

            var jsvDate = TypeSerializer.SerializeToString(now);
            var fromJsvDate = TypeSerializer.DeserializeFromString<DateTime>(jsvDate);
            Assert.That(fromJsvDate, Is.EqualTo(now));
        }

        /// <summary>Can serialize JSON dates.</summary>
        [Test]
        public void Can_serialize_json_dates()
        {
            var now = DateTime.Now;

            var jsonDate = JsonSerializer.SerializeToString(now);
            var fromJsonDate = JsonSerializer.DeserializeFromString<DateTime>(jsonDate);

            Assert.That(fromJsonDate.RoundToMs(), Is.EqualTo(now.RoundToMs()));
        }

        /// <summary>Can serialize multiple flow post transient.</summary>
        [Test]
        public void Can_Serialize_multiple_FlowPostTransient()
        {
            var dtos = new List<FlowPostTransient> {
				FlowPostTransient.Create(), 
				FlowPostTransient.Create()
			};
            Serialize(dtos);
        }

        /// <summary>A test object.</summary>
        [DataContract]
        public class TestObject
        {
            /// <summary>Gets or sets the value.</summary>
            /// <value>The value.</value>
            [DataMember]
            public string Value { get; set; }

            /// <summary>Gets or sets the value no member.</summary>
            /// <value>The value no member.</value>
            public TranslatedString ValueNoMember { get; set; }

            /// <summary>
            /// Determines whether the specified <see cref="T:System.Object" /> is equal to the current
            /// <see cref="T:System.Object" />.
            /// </summary>
            /// <param name="other">The test object to compare to this object.</param>
            /// <returns>
            /// true if the specified <see cref="T:System.Object" /> is equal to the current
            /// <see cref="T:System.Object" />; otherwise, false.
            /// </returns>
            public bool Equals(TestObject other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return Equals(other.Value, Value);
            }

            /// <summary>
            /// Determines whether the specified <see cref="T:System.Object" /> is equal to the current
            /// <see cref="T:System.Object" />.
            /// </summary>
            /// <param name="obj">The <see cref="T:System.Object" /> to compare with the current
            /// <see cref="T:System.Object" />.</param>
            /// <returns>
            /// true if the specified <see cref="T:System.Object" /> is equal to the current
            /// <see cref="T:System.Object" />; otherwise, false.
            /// </returns>
            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != typeof(TestObject)) return false;
                return Equals((TestObject)obj);
            }

            /// <summary>Serves as a hash function for a particular type.</summary>
            /// <returns>A hash code for the current <see cref="T:System.Object" />.</returns>
            public override int GetHashCode()
            {
                return (Value != null ? Value.GetHashCode() : 0);
            }
        }

        /// <summary>A test.</summary>
        public class Test
        {
            /// <summary>Gets or sets the value.</summary>
            /// <value>The value.</value>
            public string Val { get; set; }
        }

        /// <summary>A test response.</summary>
        public class TestResponse
        {
            /// <summary>Gets or sets the result.</summary>
            /// <value>The result.</value>
            public TestObject Result { get; set; }
        }

        /// <summary>A translated string.</summary>
        public class TranslatedString : ListDictionary
        {
            /// <summary>Gets or sets the current language.</summary>
            /// <value>The current language.</value>
            public string CurrentLanguage { get; set; }

            /// <summary>Gets or sets the value.</summary>
            /// <value>The value.</value>
            public string Value
            {
                get
                {
                    if (this.Contains(CurrentLanguage))
                        return this[CurrentLanguage] as string;
                    return null;
                }
                set
                {
                    if (this.Contains(CurrentLanguage))
                        this[CurrentLanguage] = value;
                    else
                        Add(CurrentLanguage, value);
                }
            }

            /// <summary>
            /// Initializes a new instance of the NServiceKit.Text.Tests.AdhocModelTests.TranslatedString
            /// class.
            /// </summary>
            public TranslatedString()
            {
                CurrentLanguage = "en";
            }

            /// <summary>Sets language on strings.</summary>
            /// <param name="lang">   The language.</param>
            /// <param name="strings">A variable-length parameters list containing strings.</param>
            public static void SetLanguageOnStrings(string lang, params TranslatedString[] strings)
            {
                foreach (TranslatedString str in strings)
                    str.CurrentLanguage = lang;
            }
        }

        /// <summary>Should ignore non data member translated string.</summary>
        [Test]
        public void Should_ignore_non_DataMember_TranslatedString()
        {
            var dto = new TestObject
            {
                Value = "value",
                ValueNoMember = new TranslatedString
                {
                    {"key1", "val1"},
                    {"key2", "val2"},
                }
            };
            SerializeAndCompare(dto);
        }

        /// <summary>Interface for parent.</summary>
        public interface IParent
        {
            /// <summary>Gets or sets the identifier.</summary>
            /// <value>The identifier.</value>
            int Id { get; set; }

            /// <summary>Gets or sets the name of the parent.</summary>
            /// <value>The name of the parent.</value>
            string ParentName { get; set; }
        }

        /// <summary>A parent.</summary>
        public class Parent : IParent
        {
            /// <summary>Gets or sets the identifier.</summary>
            /// <value>The identifier.</value>
            public int Id { get; set; }

            /// <summary>Gets or sets the name of the parent.</summary>
            /// <value>The name of the parent.</value>
            public string ParentName { get; set; }

            /// <summary>Gets or sets the child.</summary>
            /// <value>The child.</value>
            public Child Child { get; set; }
        }

        /// <summary>A child.</summary>
        public class Child
        {
            /// <summary>Gets or sets the identifier.</summary>
            /// <value>The identifier.</value>
            public int Id { get; set; }

            /// <summary>Gets or sets the name of the child.</summary>
            /// <value>The name of the child.</value>
            public string ChildName { get; set; }

            /// <summary>Gets or sets the parent.</summary>
            /// <value>The parent.</value>
            public IParent Parent { get; set; }
        }

        /// <summary>Can serialize cyclical dependency via interface.</summary>
        [Test]
        public void Can_Serialize_Cyclical_Dependency_via_interface()
        {
            JsConfig.PreferInterfaces = true;

            var dto = new Parent
            {
                Id = 1,
                ParentName = "Parent",
                Child = new Child { Id = 2, ChildName = "Child" }
            };
            dto.Child.Parent = dto;

            var fromDto = Serialize(dto, includeXml: false);

            var parent = (IParent)fromDto.Child.Parent;
            Assert.That(parent.Id, Is.EqualTo(dto.Id));
            Assert.That(parent.ParentName, Is.EqualTo(dto.ParentName));
        }

        /// <summary>An exclude.</summary>
        public class Exclude
        {
            /// <summary>Gets or sets the identifier.</summary>
            /// <value>The identifier.</value>
            public int Id { get; set; }

            /// <summary>Gets or sets the key.</summary>
            /// <value>The key.</value>
            public string Key { get; set; }
        }

        /// <summary>Can exclude properties.</summary>
        [Test]
        public void Can_exclude_properties()
        {
            JsConfig<Exclude>.ExcludePropertyNames = new[] { "Id" };

            var dto = new Exclude { Id = 1, Key = "Value" };

            Assert.That(dto.ToJson(), Is.EqualTo("{\"Key\":\"Value\"}"));
            Assert.That(dto.ToJsv(), Is.EqualTo("{Key:Value}"));
        }

        /// <summary>Can exclude properties scoped.</summary>
        [Test]
        public void Can_exclude_properties_scoped() {
            var dto = new Exclude {Id = 1, Key = "Value"};
            using (var config = JsConfig.BeginScope()) {
                config.ExcludePropertyReferences = new[] {"Exclude.Id"};
                Assert.That(dto.ToJson(), Is.EqualTo("{\"Key\":\"Value\"}"));
                Assert.That(dto.ToJsv(), Is.EqualTo("{Key:Value}"));
            }
        }

        /// <summary>An include exclude.</summary>
        public class IncludeExclude {

            /// <summary>Gets or sets the identifier.</summary>
            /// <value>The identifier.</value>
            public int Id { get; set; }

            /// <summary>Gets or sets the name.</summary>
            /// <value>The name.</value>
            public string Name { get; set; }

            /// <summary>Gets or sets the object.</summary>
            /// <value>The object.</value>
            public Exclude Obj { get; set; }
        }

        /// <summary>Can include nested only.</summary>
        [Test]
        public void Can_include_nested_only() {
            var dto = new IncludeExclude {
                Id = 1234,
                Name = "TEST",
                Obj = new Exclude {
                    Id = 1,
                    Key = "Value"
                }
            };

            using (var config = JsConfig.BeginScope()) {
                config.ExcludePropertyReferences = new[] { "Exclude.Id", "IncludeExclude.Id", "IncludeExclude.Name" };
                Assert.That(dto.ToJson(), Is.EqualTo("{\"Obj\":{\"Key\":\"Value\"}}"));
                Assert.That(dto.ToJsv(), Is.EqualTo("{Obj:{Key:Value}}"));
            }
            Assert.That(JsConfig.ExcludePropertyReferences, Is.EqualTo(null));

        }

        /// <summary>Exclude all nested.</summary>
        [Test]
        public void Exclude_all_nested()
        {
            var dto = new IncludeExclude
            {
                Id = 1234,
                Name = "TEST",
                Obj = new Exclude
                {   
                    Id = 1,
                    Key = "Value"
                }
            };
            
            using (var config = JsConfig.BeginScope())
            {
                config.ExcludePropertyReferences = new[] { "Exclude.Id", "Exclude.Key" };
                Assert.AreEqual(2, config.ExcludePropertyReferences.Length);

                var actual = dto.ToJson();
                Assert.That(actual, Is.EqualTo("{\"Id\":1234,\"Name\":\"TEST\",\"Obj\":{}}"));
                Assert.That(dto.ToJsv(), Is.EqualTo("{Id:1234,Name:TEST,Obj:{}}"));
            }
        }

        /// <summary>List of excludes.</summary>
        public class ExcludeList {

            /// <summary>Gets or sets the identifier.</summary>
            /// <value>The identifier.</value>
            public int Id { get; set; }

            /// <summary>Gets or sets the excludes.</summary>
            /// <value>The excludes.</value>
            public List<Exclude> Excludes { get; set; }
        }

        /// <summary>Exclude list scope.</summary>
        [Test]
        public void Exclude_List_Scope() {
            var dto = new ExcludeList {
                Id = 1234,
                Excludes = new List<Exclude>() {
                    new Exclude {
                        Id = 2345,
                        Key = "Value"
                    },
                    new Exclude {
                        Id = 3456,
                        Key = "Value"
                    }
                }
            };
            using (var config = JsConfig.BeginScope())
            {
                config.ExcludePropertyReferences = new[] { "ExcludeList.Id", "Exclude.Id" };
                Assert.That(dto.ToJson(), Is.EqualTo("{\"Excludes\":[{\"Key\":\"Value\"},{\"Key\":\"Value\"}]}"));
                Assert.That(dto.ToJsv(), Is.EqualTo("{Excludes:[{Key:Value},{Key:Value}]}"));
            }
        }

        /// <summary>The has index.</summary>
        public class HasIndex
        {
            /// <summary>Gets or sets the identifier.</summary>
            /// <value>The identifier.</value>
            public int Id { get; set; }

            /// <summary>
            /// Indexer to get or set items within this collection using array index syntax.
            /// </summary>
            /// <param name="id">The identifier.</param>
            /// <returns>The indexed item.</returns>
            public int this[int id]
            {
                get { return Id; }
                set { Id = value; }
            }
        }

        /// <summary>Can serialize type with indexer.</summary>
        [Test]
        public void Can_serialize_type_with_indexer()
        {
            var dto = new HasIndex { Id = 1 };
            Serialize(dto);
        }

        /// <summary>A size.</summary>
        public struct Size
        {
            /// <summary>
            /// Initializes a new instance of the NServiceKit.Text.Tests.AdhocModelTests class.
            /// </summary>
            /// <param name="value">The value.</param>
            public Size(string value)
            {
                var parts = value.Split(',');
                this.Width = parts[0];
                this.Height = parts[1];
            }

            /// <summary>
            /// Initializes a new instance of the NServiceKit.Text.Tests.AdhocModelTests class.
            /// </summary>
            /// <param name="width"> The width.</param>
            /// <param name="height">The height.</param>
            public Size(string width, string height)
            {
                Width = width;
                Height = height;
            }

            /// <summary>The width.</summary>
            public string Width;

            /// <summary>The height.</summary>
            public string Height;

            /// <summary>Returns the fully qualified type name of this instance.</summary>
            /// <returns>A <see cref="T:System.String" /> containing a fully qualified type name.</returns>
            public override string ToString()
            {
                return this.Width + "," + this.Height;
            }
        }

        /// <summary>Can serialize structure in list.</summary>
        [Test]
        public void Can_serialize_struct_in_list()
        {
            var structs = new[] {
				new Size("10px", "10px"),
				new Size("20px", "20px"),
			};

            Serialize(structs);
        }

        /// <summary>Can serialize list of bools.</summary>
        [Test]
        public void Can_serialize_list_of_bools()
        {
            Serialize(new List<bool> { true, false, true });
            Serialize(new[] { true, false, true });
        }

        /// <summary>A polar values.</summary>
        public class PolarValues
        {
            /// <summary>Gets or sets the int.</summary>
            /// <value>The int.</value>
            public int Int { get; set; }

            /// <summary>Gets or sets the long.</summary>
            /// <value>The long.</value>
            public long Long { get; set; }

            /// <summary>Gets or sets the float.</summary>
            /// <value>The float.</value>
            public float Float { get; set; }

            /// <summary>Gets or sets the double.</summary>
            /// <value>The double.</value>
            public double Double { get; set; }

            /// <summary>Gets or sets the decimal.</summary>
            /// <value>The decimal.</value>
            public decimal Decimal { get; set; }

            /// <summary>
            /// Determines whether the specified <see cref="T:System.Object" /> is equal to the current
            /// <see cref="T:System.Object" />.
            /// </summary>
            /// <param name="other">The polar values to compare to this object.</param>
            /// <returns>
            /// true if the specified <see cref="T:System.Object" /> is equal to the current
            /// <see cref="T:System.Object" />; otherwise, false.
            /// </returns>
            public bool Equals(PolarValues other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return other.Int == Int
                    && other.Long == Long
                    && other.Float.Equals(Float)
                    && other.Double.Equals(Double)
                    && other.Decimal == Decimal;
            }

            /// <summary>
            /// Determines whether the specified <see cref="T:System.Object" /> is equal to the current
            /// <see cref="T:System.Object" />.
            /// </summary>
            /// <param name="obj">The <see cref="T:System.Object" /> to compare with the current
            /// <see cref="T:System.Object" />.</param>
            /// <returns>
            /// true if the specified <see cref="T:System.Object" /> is equal to the current
            /// <see cref="T:System.Object" />; otherwise, false.
            /// </returns>
            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != typeof(PolarValues)) return false;
                return Equals((PolarValues)obj);
            }

            /// <summary>Serves as a hash function for a particular type.</summary>
            /// <returns>A hash code for the current <see cref="T:System.Object" />.</returns>
            public override int GetHashCode()
            {
                unchecked
                {
                    int result = Int;
                    result = (result * 397) ^ Long.GetHashCode();
                    result = (result * 397) ^ Float.GetHashCode();
                    result = (result * 397) ^ Double.GetHashCode();
                    result = (result * 397) ^ Decimal.GetHashCode();
                    return result;
                }
            }
        }

        /// <summary>Can serialize maximum values.</summary>
        [Test]
        public void Can_serialize_max_values()
        {
            var dto = new PolarValues
            {
                Int = int.MaxValue,
                Long = long.MaxValue,
                Float = float.MaxValue,
                Double = double.MaxValue,
                Decimal = decimal.MaxValue,
            };
            var to = Serialize(dto);
            Assert.That(to, Is.EqualTo(dto));
        }

        /// <summary>Can serialize maximum values less 1.</summary>
        [Test]
        public void Can_serialize_max_values_less_1()
        {
            var dto = new PolarValues
            {
                Int = int.MaxValue - 1,
                Long = long.MaxValue - 1,
                Float = float.MaxValue - 1,
                Double = double.MaxValue - 1,
                Decimal = decimal.MaxValue - 1,
            };
            var to = Serialize(dto);
            Assert.That(to, Is.EqualTo(dto));
        }

        /// <summary>Can serialize minimum values.</summary>
        [Test]
        public void Can_serialize_min_values()
        {
            var dto = new PolarValues
            {
                Int = int.MinValue,
                Long = long.MinValue,
                Float = float.MinValue,
                Double = double.MinValue,
                Decimal = decimal.MinValue,
            };
            var to = Serialize(dto);
            Assert.That(to, Is.EqualTo(dto));
        }

        /// <summary>A test class.</summary>
        public class TestClass
        {
            /// <summary>Gets or sets the description.</summary>
            /// <value>The description.</value>
            public string Description { get; set; }

            /// <summary>Gets or sets the inner.</summary>
            /// <value>The inner.</value>
            public TestClass Inner { get; set; }
        }

        /// <summary>Can serialize 1 level cyclical dto.</summary>
        [Test]
        public void Can_serialize_1_level_cyclical_dto()
        {
            var dto = new TestClass
            {
                Description = "desc",
                Inner = new TestClass { Description = "inner" }
            };

            var from = Serialize(dto, includeXml: false);

            Assert.That(from.Description, Is.EqualTo(dto.Description));
            Assert.That(from.Inner.Description, Is.EqualTo(dto.Inner.Description));
            Console.WriteLine(from.Dump());
        }

        /// <summary>Values that represent EnumValues.</summary>
        public enum EnumValues
        {
            /// <summary>An enum constant representing the enum 1 option.</summary>
            Enum1,

            /// <summary>An enum constant representing the enum 2 option.</summary>
            Enum2,

            /// <summary>An enum constant representing the enum 3 option.</summary>
            Enum3,
        }

        /// <summary>Can deserialize.</summary>
        [Test]
        public void Can_Deserialize()
        {
            var items = TypeSerializer.DeserializeFromString<List<string>>(
                "/CustomPath35/api,/CustomPath40/api,/RootPath35,/RootPath40,:82,:83,:5001/api,:5002/api,:5003,:5004");

            Console.WriteLine(items.Dump());
        }

        /// <summary>Can serialize array of enums.</summary>
        [Test]
        public void Can_Serialize_Array_of_enums()
        {
            var enumArr = new[] { EnumValues.Enum1, EnumValues.Enum2, EnumValues.Enum3, };
            var json = JsonSerializer.SerializeToString(enumArr);
            Assert.That(json, Is.EqualTo("[\"Enum1\",\"Enum2\",\"Enum3\"]"));
        }

        /// <summary>A dictionary enum type.</summary>
        public class DictionaryEnumType
        {
            /// <summary>Gets or sets the type of the dictionary enum.</summary>
            /// <value>The type of the dictionary enum.</value>
            public Dictionary<EnumValues, Test> DictEnumType { get; set; }
        }

        /// <summary>Can serialize dictionary with enums.</summary>
        [Test]
        public void Can_Serialize_Dictionary_With_Enums()
        {
            Dictionary<EnumValues, Test> dictEnumType =
                new Dictionary<EnumValues, Test> 
                {
                    {
                        EnumValues.Enum1, new Test { Val = "A Value" }
                    }
                };

            var item = new DictionaryEnumType
            {
                DictEnumType = dictEnumType
            };
            const string expected = "{\"DictEnumType\":{\"Enum1\":{\"Val\":\"A Value\"}}}";

            var jsonItem = JsonSerializer.SerializeToString(item);
            //Log(jsonItem);
            Assert.That(jsonItem, Is.EqualTo(expected));

            var deserializedItem = JsonSerializer.DeserializeFromString<DictionaryEnumType>(jsonItem);
            Assert.That(deserializedItem, Is.TypeOf<DictionaryEnumType>());
        }

        /// <summary>Can serialize array of characters.</summary>
        [Test]
        public void Can_Serialize_Array_of_chars()
        {
            var enumArr = new[] { 'A', 'B', 'C', };
            var json = JsonSerializer.SerializeToString(enumArr);
            Assert.That(json, Is.EqualTo("[\"A\",\"B\",\"C\"]"));
        }

        /// <summary>Can serialize array with nulls.</summary>
        [Test]
        public void Can_Serialize_Array_with_nulls()
        {
            var t = new
            {
                Name = "MyName",
                Number = (int?)null,
                Data = new object[] { 5, null, "text" }
            };

            NServiceKit.Text.JsConfig.IncludeNullValues = true;
            var json = NServiceKit.Text.JsonSerializer.SerializeToString(t);
            Assert.That(json, Is.EqualTo("{\"Name\":\"MyName\",\"Number\":null,\"Data\":[5,null,\"text\"]}"));
            JsConfig.Reset();
        }

        /// <summary>An a.</summary>
        class A
        {
            /// <summary>Gets or sets the value.</summary>
            /// <value>The value.</value>
            public string Value { get; set; }
        }

        /// <summary>Dumps the fail.</summary>
        [Test]
        public void DumpFail()
        {
            var arrayOfA = new[] { new A { Value = "a" }, null, new A { Value = "b" } };
            Console.WriteLine(arrayOfA.Dump());
        }

        /// <summary>Deserialize array with null elements.</summary>
        [Test]
        public void Deserialize_array_with_null_elements()
        {
            var json = "[{\"Value\": \"a\"},null,{\"Value\": \"b\"}]";
            var o = JsonSerializer.DeserializeFromString<A[]>(json);
        }

        /// <summary>Can serialize string collection.</summary>
        [Test]
        public void Can_serialize_StringCollection()
        {
            var sc = new StringCollection { "one", "two", "three" };
            var from = Serialize(sc, includeXml: false);
            Console.WriteLine(from.Dump());
        }

        /// <summary>A breaker.</summary>
        public class Breaker
        {
            /// <summary>Gets or sets the blah.</summary>
            /// <value>The blah.</value>
            public IEnumerable Blah { get; set; }
        }

        /// <summary>Can serialize i enumerable.</summary>
        [Test]
        public void Can_serialize_IEnumerable()
        {
            var dto = new Breaker
            {
                Blah = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 }
            };

            var from = Serialize(dto, includeXml: false);
            Assert.IsNotNull(from.Blah);
            from.PrintDump();
        }

        /// <summary>Collection of breakers.</summary>
        public class BreakerCollection
        {
            /// <summary>Gets or sets the blah.</summary>
            /// <value>The blah.</value>
            public ICollection Blah { get; set; }
        }

        /// <summary>Can serialize i collection.</summary>
        [Test]
        public void Can_serialize_ICollection()
        {
            var dto = new BreakerCollection
            {
                Blah = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 }
            };
            
            var from = Serialize(dto, includeXml: false);
            Assert.IsNotNull(from.Blah);
            Assert.AreEqual(dto.Blah.Count, from.Blah.Count);
            from.PrintDump();
        }

        /// <summary>An XML any.</summary>
        public class XmlAny
        {
            /// <summary>Gets or sets any.</summary>
            /// <value>any.</value>
            public XmlElement[] Any { get; set; }
        }

        /// <summary>Can serialize specialized i enumerable.</summary>
        [Test]
        public void Can_serialize_Specialized_IEnumerable()
        {
            var getParseFn = JsvReader.GetParseFn(typeof (XmlAny));
            Assert.IsNotNull(getParseFn);
        }
    }
}
