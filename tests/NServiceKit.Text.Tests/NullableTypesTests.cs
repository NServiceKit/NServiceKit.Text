using System;
using System.Collections.Generic;
using NUnit.Framework;
#if !MONOTOUCH
using NServiceKit.Common.Tests.Models;
#endif

namespace NServiceKit.Text.Tests
{
    /// <summary>A nullable types tests.</summary>
    [TestFixture]
    public class NullableTypesTests
    {
        /// <summary>Sets the up.</summary>
        [SetUp]
        public void SetUp()
        {
            JsConfig.Reset();
        }

#if !MONOTOUCH
        /// <summary>Can serialize populated model of nullable types.</summary>
        [Test]
        public void Can_Serialize_populated_model_of_NullableTypes()
        {
            var model = ModelWithFieldsOfNullableTypes.Create(1);

            var json = JsonSerializer.SerializeToString(model);

            var fromJson = JsonSerializer.DeserializeFromString<ModelWithFieldsOfNullableTypes>(json);

            ModelWithFieldsOfNullableTypes.AssertIsEqual(model, fromJson);
        }

        /// <summary>Can serialize empty model of nullable types.</summary>
        [Test]
        public void Can_Serialize_empty_model_of_NullableTypes()
        {
            var model = new ModelWithFieldsOfNullableTypes();

            var json = JsonSerializer.SerializeToString(model);

            var fromJson = JsonSerializer.DeserializeFromString<ModelWithFieldsOfNullableTypes>(json);

            ModelWithFieldsOfNullableTypes.AssertIsEqual(model, fromJson);
        }
#endif

        /// <summary>Serialize array with null should always produce valid JSON.</summary>
        [Test]
        public void Serialize_array_with_null_should_always_produce_Valid_JSON()
        {
            var hold = JsConfig.IncludeNullValues;
            JsConfig.IncludeNullValues = true;
            string json = new Object[] { 1, 2, 3, null, 5 }.ToJson();  // [1,2,3,,5]  - Should be [1,2,3,null,5]
            Assert.That(json, Is.EqualTo("[1,2,3,null,5]"));
            JsConfig.IncludeNullValues = hold;
        }

        /// <summary>An answer.</summary>
        public class Answer
        {
            /// <summary>Gets or sets the name of the tag.</summary>
            /// <value>The name of the tag.</value>
            public string tag_name { get; set; }

            /// <summary>Gets or sets the question score.</summary>
            /// <value>The question score.</value>
            public int question_score { get; set; }

            /// <summary>Gets or sets the number of questions.</summary>
            /// <value>The number of questions.</value>
            public int question_count { get; set; }

            /// <summary>Gets or sets the answer score.</summary>
            /// <value>The answer score.</value>
            public int answer_score { get; set; }

            /// <summary>Gets or sets the number of answers.</summary>
            /// <value>The number of answers.</value>
            public int answer_count { get; set; }

            /// <summary>Gets or sets the identifier of the user.</summary>
            /// <value>The identifier of the user.</value>
            public int user_id { get; set; }
        }

        /// <summary>A top answers.</summary>
        public class TopAnswers
        {
            /// <summary>
            /// Initializes a new instance of the NServiceKit.Text.Tests.NullableTypesTests.TopAnswers
            /// class.
            /// </summary>
            public TopAnswers()
            {
                this.Items = new List<Answer>();
            }

            /// <summary>Gets or sets the items.</summary>
            /// <value>The items.</value>
            public List<Answer> Items { get; set; }
        }

        /// <summary>Can handle null in quotes in top answers.</summary>
        [Test]
        public void Can_handle_null_in_quotes_in_TopAnswers()
        {
            var topAnswers = new TopAnswers
            {
                Items = {
                    new Answer {
                        tag_name = "null",
                        question_score= 0,
                        question_count= 0,
                        answer_score= 17,
                        answer_count= 2,
                        user_id= 236255
                    },
                }
            };

            var json = topAnswers.ToJson();
            var fromJson = json.FromJson<TopAnswers>();

            fromJson.PrintDump();

            Assert.That(fromJson.Items[0].tag_name, Is.EqualTo("null"));
        }

        /// <summary>An entity for overriding deserialization.</summary>
        public class EntityForOverridingDeserialization
        {
            /// <summary>Gets or sets the int value.</summary>
            /// <value>The int value.</value>
            public int? IntValue { get; set; }

            /// <summary>Gets or sets the value.</summary>
            /// <value>The bool value.</value>
            public bool? BoolValue { get; set; }

            /// <summary>Gets or sets the long value.</summary>
            /// <value>The long value.</value>
            public long? LongValue { get; set; }

            /// <summary>Gets or sets the unique identifier value.</summary>
            /// <value>The unique identifier value.</value>
            public Guid? GuidValue { get; set; }
        }

        /// <summary>Tests override deserialize function.</summary>
        [Test]
		public void Test_override_DeserializeFn()
		{
            JsConfig<bool?>.DeSerializeFn = value => string.IsNullOrEmpty(value) ? (bool?)null : bool.Parse(value);
			JsConfig<int?>.DeSerializeFn = value => string.IsNullOrEmpty(value) ? (int?)null : int.Parse(value);
			JsConfig<long?>.DeSerializeFn = value => string.IsNullOrEmpty(value) ? (long?)null : long.Parse(value);
			JsConfig<Guid?>.DeSerializeFn = value => string.IsNullOrEmpty(value) ? (Guid?)null : new Guid(value);				

            try {
                var json = "{\"intValue\":1,\"boolValue\":\"\",\"longValue\":null}";
                var fromJson = json.FromJson<EntityForOverridingDeserialization>();
                Assert.That(fromJson.IntValue, Is.EqualTo(1));
                Assert.That(fromJson.BoolValue, Is.Null);
                Assert.That(fromJson.LongValue, Is.Null);
                Assert.That(fromJson.GuidValue, Is.Null);
            } finally {
                JsConfig.Reset();
            }
		}

        /// <summary>Can handle null in answer.</summary>
        [Test]
        public void Can_handle_null_in_Answer()
        {
            var json = "{\"tag_name\":null,\"question_score\":0,\"question_count\":0,\"answer_score\":17,\"answer_count\":2,\"user_id\":236255}";
            var fromJson = json.FromJson<Answer>();

            Assert.That(fromJson.tag_name, Is.Null);
        }

        /// <summary>Can handle null in quotes in answer.</summary>
        [Test]
        public void Can_handle_null_in_quotes_in_Answer()
        {
            var answer = new Answer
            {
                tag_name = "null",
                question_score = 0,
                question_count = 0,
                answer_score = 17,
                answer_count = 2,
                user_id = 236255
            };

            var json = answer.ToJson();
            json.Print();
            var fromJson = json.FromJson<Answer>();

            fromJson.PrintDump();

            Assert.That(fromJson.tag_name, Is.EqualTo("null"));
        }

        /// <summary>Deserialize with null collection is null.</summary>
        [Test]
        public void Deserialize_WithNullCollection_CollectionIsNull()
        {
            JsConfig.IncludeNullValues = true;
            
            var item = new Foo { Strings = null };
            var json = JsonSerializer.SerializeToString(item);
            var result = JsonSerializer.DeserializeFromString<Foo>(json);
            Assert.IsNull(result.Strings);

            var jsv = TypeSerializer.SerializeToString(item);
            result = TypeSerializer.DeserializeFromString<Foo>(jsv);
            Assert.That(result.Strings, Is.Empty); //JSV doesn't support setting null values explicitly

            JsConfig.IncludeNullValues = false;
        }

        /// <summary>A foo.</summary>
        public class Foo
        {
            /// <summary>
            /// Initializes a new instance of the NServiceKit.Text.Tests.NullableTypesTests.Foo class.
            /// </summary>
            public Foo()
            {
                Strings = new List<string>();
            }

            /// <summary>Gets or sets the strings.</summary>
            /// <value>The strings.</value>
            public List<string> Strings { get; set; }
        }
    }

}