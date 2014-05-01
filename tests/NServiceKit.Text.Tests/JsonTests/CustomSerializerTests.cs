using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using NUnit.Framework;
using System.Runtime.Serialization;

namespace NServiceKit.Text.Tests.JsonTests
{
    /// <summary>A custom serializer tests.</summary>
    public class CustomSerializerTests : TestBase
    {
        /// <summary>
        /// Initializes static members of the NServiceKit.Text.Tests.JsonTests.CustomSerializerTests
        /// class.
        /// </summary>
        static CustomSerializerTests()
        {
            JsConfig<EntityWithValues>.RawSerializeFn = SerializeEntity;
            JsConfig<EntityWithValues>.RawDeserializeFn = DeserializeEntity;
        }

        /// <summary>Tests fixture tear down.</summary>
        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            JsConfig.Reset();
        }

        /// <summary>Can serialize entity.</summary>
        [Test]
        public void Can_serialize_Entity()
        {
            var originalEntity = new EntityWithValues { id = 5, Values = new Dictionary<string, string> { { "dog", "bark" }, { "cat", "meow" } } };
            JsonSerializeAndCompare(originalEntity);
        }

        /// <summary>Can serialize arrays of entities.</summary>
        [Test]
        public void Can_serialize_arrays_of_entities()
        {
            var originalEntities = new[] { new EntityWithValues { id = 5, Values = new Dictionary<string, string> { { "dog", "bark" } } }, new EntityWithValues { id = 6, Values = new Dictionary<string, string> { { "cat", "meow" } } } };
            JsonSerializeAndCompare(originalEntities);
        }

        /// <summary>An entity with values.</summary>
        public class EntityWithValues
        {
            /// <summary>The values.</summary>
            private Dictionary<string, string> _values;

            /// <summary>Gets or sets the identifier.</summary>
            /// <value>The identifier.</value>
            public int id { get; set; }

            /// <summary>Gets or sets the values.</summary>
            /// <value>The values.</value>
            public Dictionary<string, string> Values
            {
                get { return _values ?? (_values = new Dictionary<string, string>()); }
                set { _values = value; }
            }

            /// <summary>Serves as a hash function for a particular type.</summary>
            /// <returns>A hash code for the current <see cref="T:System.Object" />.</returns>
            public override int GetHashCode()
            {
                return this.id.GetHashCode() ^ this.Values.GetHashCode();
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
                return Equals(obj as EntityWithValues);
            }

            /// <summary>Tests if this EntityWithValues is considered equal to another.</summary>
            /// <param name="other">The entity with values to compare to this object.</param>
            /// <returns>true if the objects are considered equal, false if they are not.</returns>
            public bool Equals(EntityWithValues other)
            {
                return ReferenceEquals(this, other)
                       || (this.id == other.id && DictionaryEquality(Values, other.Values));
            }

            /// <summary>Dictionary equality.</summary>
            /// <param name="first"> The first.</param>
            /// <param name="second">The second.</param>
            /// <returns>true if it succeeds, false if it fails.</returns>
            private bool DictionaryEquality(Dictionary<string, string> first, Dictionary<string, string> second)
            {
                return first.Count == second.Count
                       && first.Keys.All(second.ContainsKey)
                       && first.Keys.All(key => first[key] == second[key]);
            }
        }

        /// <summary>Serialize entity.</summary>
        /// <param name="entity">The entity.</param>
        /// <returns>A string.</returns>
        private static string SerializeEntity(EntityWithValues entity)
        {
            var dictionary = entity.Values.ToDictionary(pair => pair.Key, pair => pair.Value);
            if (entity.id > 0)
            {
                dictionary["id"] = entity.id.ToString(CultureInfo.InvariantCulture);
            }
            return JsonSerializer.SerializeToString(dictionary);
        }

        /// <summary>Deserialize entity.</summary>
        /// <param name="value">The value.</param>
        /// <returns>The EntityWithValues.</returns>
        private static EntityWithValues DeserializeEntity(string value)
        {
            var dictionary = JsonSerializer.DeserializeFromString<Dictionary<string, string>>(value);
            if (dictionary == null) return null;
            var entity = new EntityWithValues();
            foreach (var pair in dictionary)
            {
                if (pair.Key == "id")
                {
                    if (!string.IsNullOrEmpty(pair.Value))
                    {
                        entity.id = int.Parse(pair.Value);
                    }
                }
                else
                {
                    entity.Values.Add(pair.Key, pair.Value);
                }
            }
            return entity;
        }

        /// <summary>A test 1 base.</summary>
        [DataContract]
        private class Test1Base
        {
            /// <summary>
            /// Initializes a new instance of the
            /// NServiceKit.Text.Tests.JsonTests.CustomSerializerTests.Test1Base class.
            /// </summary>
            /// <param name="itb"> true to itb.</param>
            /// <param name="itbm">true to itbm.</param>
            public Test1Base(bool itb, bool itbm)
            {
                InTest1Base = itb; InTest1BaseM = itbm;
            }

            /// <summary>Gets or sets a value indicating whether the in test 1 base m.</summary>
            /// <value>true if in test 1 base m, false if not.</value>
            [DataMember]
            public bool InTest1BaseM { get; set; }

            /// <summary>Gets or sets a value indicating whether the in test 1 base.</summary>
            /// <value>true if in test 1 base, false if not.</value>
            public bool InTest1Base { get; set; }
        }

        /// <summary>A test 1.</summary>
        [DataContract]
        private class Test1 : Test1Base
        {
            /// <summary>
            /// Initializes a new instance of the
            /// NServiceKit.Text.Tests.JsonTests.CustomSerializerTests.Test1 class.
            /// </summary>
            /// <param name="it">  true to iterator.</param>
            /// <param name="itm"> true to itm.</param>
            /// <param name="itb"> true to itb.</param>
            /// <param name="itbm">true to itbm.</param>
            public Test1(bool it, bool itm, bool itb, bool itbm)
                : base(itb, itbm)
            {
                InTest1 = it; InTest1M = itm;
            }

            /// <summary>Gets or sets a value indicating whether the in test 1 m.</summary>
            /// <value>true if in test 1 m, false if not.</value>
            [DataMember]
            public bool InTest1M { get; set; }

            /// <summary>Gets or sets a value indicating whether the in test 1.</summary>
            /// <value>true if in test 1, false if not.</value>
            public bool InTest1 { get; set; }
        }

        /// <summary>Can serialize with custom constructor.</summary>
        [Test]
        public void Can_Serialize_With_Custom_Constructor()
        {
            bool hit = false;
            JsConfig.ModelFactory = type => {
                if (typeof(Test1) == type)
                {
                    hit = true;
                    return () => new Test1(false, false, true, false);
                }
                return null;
            };

            var t1 = new Test1(true, true, true, true);

            var data = JsonSerializer.SerializeToString(t1);

            var t2 = JsonSerializer.DeserializeFromString<Test1>(data);

            Assert.IsTrue(hit);
            Assert.IsTrue(t2.InTest1BaseM);
            Assert.IsTrue(t2.InTest1M);
            Assert.IsTrue(t2.InTest1Base);
            Assert.IsFalse(t2.InTest1);
        }

        /// <summary>A dto.</summary>
        public class Dto
        {
            /// <summary>Gets or sets the name.</summary>
            /// <value>The name.</value>
            public string Name { get; set; }
        }

        /// <summary>Interface for has version.</summary>
        public interface IHasVersion
        {
            /// <summary>Gets or sets the version.</summary>
            /// <value>The version.</value>
            int Version { get; set; }
        }

        /// <summary>A dto v 1.</summary>
        public class DtoV1 : IHasVersion
        {
            /// <summary>Gets or sets the version.</summary>
            /// <value>The version.</value>
            public int Version { get; set; }

            /// <summary>Gets or sets the name.</summary>
            /// <value>The name.</value>
            public string Name { get; set; }

            /// <summary>
            /// Initializes a new instance of the
            /// NServiceKit.Text.Tests.JsonTests.CustomSerializerTests.DtoV1 class.
            /// </summary>
            public DtoV1()
            {
                Version = 1;
            }
        }

        /// <summary>Can detect dto with no version.</summary>
        [Test]
        public void Can_detect_dto_with_no_Version()
        {
            using (JsConfig.With(modelFactory:type => {
                if (typeof(IHasVersion).IsAssignableFrom(type))
                {
                    return () => {
                        var obj = (IHasVersion)type.CreateInstance();
                        obj.Version = 0;
                        return obj;
                    };
                }
                return () => type.CreateInstance();
            }))
            {
                var dto = new Dto { Name = "Foo" };
                var fromDto = dto.ToJson().FromJson<DtoV1>();
                Assert.That(fromDto.Version, Is.EqualTo(0));
                Assert.That(fromDto.Name, Is.EqualTo("Foo"));

                var dto1 = new DtoV1 { Name = "Foo 1" };
                var fromDto1 = dto1.ToJson().FromJson<DtoV1>();
                Assert.That(fromDto1.Version, Is.EqualTo(1));
                Assert.That(fromDto1.Name, Is.EqualTo("Foo 1"));
            }
        }
    }
}