using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

using NServiceKit.ServiceInterface.ServiceModel;

using NUnit.Framework;
using NServiceKit.Text.Tests.Support;

namespace NServiceKit.Text.Tests
{
    /// <summary>A data contract tests.</summary>
    [TestFixture]
    public class DataContractTests
    : TestBase
    {
        /// <summary>Only serializes data member fields for data contracts.</summary>
        [Test]
        public void Only_Serializes_DataMember_fields_for_DataContracts()
        {
            var dto = new ResponseStatus {
                ErrorCode = "ErrorCode",
                Message = "Message",
                StackTrace = "StackTrace",
                Errors = new List<ResponseError>(),
            };

            Serialize(dto);
        }

        /// <summary>A request with ignored members.</summary>
        public class RequestWithIgnoredMembers
        {
            /// <summary>Gets or sets the name.</summary>
            /// <value>The name.</value>
            public string Name { get; set; }

            /// <summary>Gets or sets the comment.</summary>
            /// <value>The comment.</value>
            [IgnoreDataMember]
            public string Comment { get; set; }
        }

        /// <summary>Executes the ignore member test operation.</summary>
        /// <param name="serialize">  The serialize.</param>
        /// <param name="deserialize">The deserialize.</param>
        private void DoIgnoreMemberTest(Func<RequestWithIgnoredMembers, string> serialize,
                                        Func<string, RequestWithIgnoredMembers> deserialize)
        {
            var dto = new RequestWithIgnoredMembers() {
                Name = "John",
                Comment = "Some Comment"
            };

            var clone = deserialize(serialize(dto));

            Assert.AreEqual(dto.Name, clone.Name);
            Assert.IsNull(clone.Comment);
        }

        /// <summary>JSON serializer honors ignore member attribute.</summary>
        [Test]
        public void JsonSerializerHonorsIgnoreMemberAttribute()
        {
            DoIgnoreMemberTest(r => JsonSerializer.SerializeToString(r),
                               s => JsonSerializer.DeserializeFromString<RequestWithIgnoredMembers>(s));
        }

        /// <summary>Jsv serializer honors ignore member attribute.</summary>
        [Test]
        public void JsvSerializerHonorsIgnoreMemberAttribute()
        {
            DoIgnoreMemberTest(r => TypeSerializer.SerializeToString(r),
                               s => TypeSerializer.DeserializeFromString<RequestWithIgnoredMembers>(s));
        }

        /// <summary>XML serializer honors ignore member attribute.</summary>
        [Test]
        public void XmlSerializerHonorsIgnoreMemberAttribute()
        {
            DoIgnoreMemberTest(r => XmlSerializer.SerializeToString(r),
                               s => XmlSerializer.DeserializeFromString<RequestWithIgnoredMembers>(s));
        }

        /// <summary>An empty data contract.</summary>
        [DataContract]
        public class EmptyDataContract { }

        /// <summary>Can serialize empty data contract.</summary>
        [Test]
        public void Can_Serialize_Empty_DataContract()
        {
            var dto = new EmptyDataContract();
            Serialize(dto);
        }

        /// <summary>Collection of mies.</summary>
        [CollectionDataContract]
        public class MyCollection : ICollection<MyType>
        {
            /// <summary>The internal.</summary>
            private List<MyType> _internal = new List<MyType> { new MyType() };

            /// <summary>Gets the enumerator.</summary>
            /// <returns>The enumerator.</returns>
            public IEnumerator<MyType> GetEnumerator()
            {
                return _internal.GetEnumerator();
            }

            /// <summary>Gets the enumerator.</summary>
            /// <returns>The enumerator.</returns>
            IEnumerator IEnumerable.GetEnumerator()
            {
                return _internal.GetEnumerator();
            }

            /// <summary>Adds item.</summary>
            /// <param name="item">The item to add.</param>
            public void Add(MyType item)
            {
                _internal.Add(item);
            }

            /// <summary>Clears this object to its blank/initial state.</summary>
            public void Clear()
            {
                _internal.Clear();
            }

            /// <summary>Query if this object contains the given item.</summary>
            /// <param name="item">The MyType to test for containment.</param>
            /// <returns>true if the object is in this collection, false if not.</returns>
            public bool Contains(MyType item)
            {
                return _internal.Contains(item);
            }

            /// <summary>Copies to.</summary>
            /// <param name="array">     The array.</param>
            /// <param name="arrayIndex">Zero-based index of the array.</param>
            public void CopyTo(MyType[] array, int arrayIndex)
            {
                _internal.CopyTo(array, arrayIndex);
            }

            /// <summary>Removes the given item.</summary>
            /// <param name="item">The item to remove.</param>
            /// <returns>true if it succeeds, false if it fails.</returns>
            public bool Remove(MyType item)
            {
                return _internal.Remove(item);
            }

            /// <summary>Gets the number of. </summary>
            /// <value>The count.</value>
            public int Count
            {
                get { return _internal.Count; }
            }

            /// <summary>Gets a value indicating whether this object is read only.</summary>
            /// <value>true if this object is read only, false if not.</value>
            public bool IsReadOnly
            {
                get { return false; }
            }
        }

        /// <summary>my type.</summary>
        [DataContract]
        public class MyType { }

        /// <summary>Can serialize my collection.</summary>
        [Test]
        public void Can_Serialize_MyCollection()
        {
            var dto = new MyCollection();
            Serialize(dto);
        }

        /// <summary>Information about the person.</summary>
        [DataContract]
        public class PersonRecord
        {
            /// <summary>Gets or sets the identifier.</summary>
            /// <value>The identifier.</value>
            public int Id { get; set; }

            /// <summary>Gets or sets the name.</summary>
            /// <value>The name.</value>
            public string Name { get; set; }
        }

        /// <summary>Replicate serialization bug.</summary>
        [Test] //https://github.com/NServiceKit/NServiceKit.Text/issues/46
        public void Replicate_serialization_bug()
        {
            var p = new PersonRecord { Id = 27, Name = "John" };

            // Fails at this point, with a "Cannot access a closed Stream." exception.
            // Am I doing something wrong? 
            string output = XmlSerializer.SerializeToString(p);

            Console.WriteLine(output);
        }

        /// <summary>The class one.</summary>
        [DataContract]
        public class ClassOne
        {
            /// <summary>Gets or sets the identifier.</summary>
            /// <value>The identifier.</value>
            [DataMember]
            public int Id { get; set; }

            /// <summary>Gets or sets the list.</summary>
            /// <value>The list.</value>
            [DataMember(Name = "listClassTwo", Order = 1)]
            public List<ClassTwo> List { get; set; }

            /// <summary>
            /// Initializes a new instance of the NServiceKit.Text.Tests.DataContractTests.ClassOne class.
            /// </summary>
            public ClassOne()
            {
                List = new List<ClassTwo>();
            }
        }

        /// <summary>The class two.</summary>
        [DataContract]
        public class ClassTwo
        {
            /// <summary>Gets or sets the name.</summary>
            /// <value>The name.</value>
            [DataMember(Name = "NewName")]
            public string Name { get; set; }
        }

        /// <summary>The class three.</summary>
        [DataContract]
        public class ClassThree
        {
            /// <summary>Gets or sets the title.</summary>
            /// <value>The title.</value>
            [DataMember(Name = "some-title")]
            public string Title { get; set; }
        }

        /// <summary>CSV serialize should respects data contract name.</summary>
        [Test]
        public void Csv_Serialize_Should_Respects_DataContract_Name()
        {
            var classTwo = new ClassTwo {
                Name = "Value"
            };

            Assert.That(CsvSerializer.SerializeToString(classTwo),
                        Is.EqualTo(String.Format("NewName{0}Value{0}", Environment.NewLine)));
        }

        /// <summary>Deserialize from string with the data member name.</summary>
        [Test]
        public void deserialize_from_string_with_the_dataMember_name()
        {
            const string jsonList =
            "{\"Id\":1,\"listClassTwo\":[{\"NewName\":\"Name One\"},{\"NewName\":\"Name Two\"}]}";

            var classOne = JsonSerializer.DeserializeFromString<ClassOne>(jsonList);

            Assert.AreEqual(1, classOne.Id);
            Assert.AreEqual(2, classOne.List.Count);
        }

        /// <summary>JSON serialize should respects data contract name when deserialize.</summary>
        [Test]
        public void Json_Serialize_Should_Respects_DataContract_Name_When_Deserialize()
        {
            var t = JsonSerializer.DeserializeFromString<ClassThree>("{\"some-title\": \"right\", \"Title\": \"wrong\"}");
            Assert.That(t.Title, Is.EqualTo("right"));
        }

        /// <summary>JSON serialize should respects data contract name.</summary>
        [Test]
        public void Json_Serialize_Should_Respects_DataContract_Name()
        {
            var classOne = new ClassOne {
                Id = 1,
                List =
                new List<ClassTwo> { new ClassTwo { Name = "Name One" }, new ClassTwo { Name = "Name Two" } }
            };
            Assert.That(JsonSerializer.SerializeToString(classOne),
                        Is.EqualTo("{\"Id\":1,\"listClassTwo\":[{\"NewName\":\"Name One\"},{\"NewName\":\"Name Two\"}]}"));
        }

        /// <summary>Can get weak data member.</summary>
        [Test]
        public void Can_get_weak_DataMember()
        {
            var dto = new ClassOne();
            var dataMemberAttr = dto.GetType().GetProperty("Id").GetWeakDataMember();
            Assert.That(dataMemberAttr.Name, Is.Null);

            dataMemberAttr = dto.GetType().GetProperty("List").GetWeakDataMember();
            Assert.That(dataMemberAttr.Name, Is.EqualTo("listClassTwo"));
            Assert.That(dataMemberAttr.Order, Is.EqualTo(1));
        }

        [DataContract(Name = "my-class", Namespace = "http://schemas.ns.com")]
        public class MyClass
        {
            /// <summary>Gets or sets the title.</summary>
            /// <value>The title.</value>

            /// <summary>Gets or sets the title.</summary>
            /// <value>The title.</value>
            [DataMember(Name = "some-title")]
            public string Title { get; set; }
        }

        /// <summary>Can get weak data contract.</summary>
        [Test]
        public void Can_get_weak_DataContract()
        {
            var mc = new MyClass { Title = "Some random title" };

            var attr = mc.GetType().GetWeakDataContract();

            Assert.That(attr.Name, Is.EqualTo("my-class"));
            Assert.That(attr.Namespace, Is.EqualTo("http://schemas.ns.com"));
        }

        /// <summary>Does use data member name.</summary>
        [Test]
        public void Does_use_DataMember_Name()
        {
            var mc = new MyClass { Title = "Some random title" };

            Assert.That(mc.ToJson(), Is.EqualTo("{\"some-title\":\"Some random title\"}"));
        }

    }
}