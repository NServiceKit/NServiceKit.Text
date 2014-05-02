using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using NUnit.Framework;
using NServiceKit.Text.Tests.DynamicModels;

namespace NServiceKit.Text.Tests.JsonTests
{
    /// <summary>A dictionary tests.</summary>
	[TestFixture]
	public class DictionaryTests
	{
        /// <summary>An edge case properties.</summary>
		public class EdgeCaseProperties : Dictionary<string, string>
		{
            /// <summary>The identifier.</summary>
            private const string Id = "id";

            /// <summary>Gets or sets the identifier.</summary>
            /// <value>The identifier.</value>
            [DataMember]
            public int id
            {
                get
                {
                    int value;
                    return (ContainsKey(Id) && int.TryParse(this[Id], out value))
                               ? value
                               : 0;
                }
                set { this[Id] = value.ToString(CultureInfo.InvariantCulture); }
            }

            /// <summary>Creates a new EdgeCaseProperties.</summary>
            /// <param name="i">Zero-based index of the.</param>
            /// <returns>The EdgeCaseProperties.</returns>
			public static EdgeCaseProperties Create(int i)
			{
			    var value = new EdgeCaseProperties { id = i };
			    value[i.ToString()] = i.ToString();
			    return value;
			}
		}

        /// <summary>Can serialize.</summary>
		[Test]
		public void Can_Serialize()
		{
			var model = EdgeCaseProperties.Create(1);
			var s = JsonSerializer.SerializeToString(model);

			Console.WriteLine(s);
		}

        /// <summary>Can serialize list.</summary>
		[Test]
		public void Can_Serialize_list()
		{
			var model = new List<EdgeCaseProperties>
           	{
				EdgeCaseProperties.Create(1),
				EdgeCaseProperties.Create(2)
           	};
			var s = JsonSerializer.SerializeToString(model);

			Console.WriteLine(s);
		}

        /// <summary>Can serialize map.</summary>
		[Test]
		public void Can_Serialize_map()
		{
			var model = new Dictionary<string, EdgeCaseProperties>
           	{
				{"A", EdgeCaseProperties.Create(1)},
				{"B", EdgeCaseProperties.Create(2)},
           	};
			var s = JsonSerializer.SerializeToString(model);

			Console.WriteLine(s);
		}

        /// <summary>Can deserialize.</summary>
        [Test]
        public void Can_Deserialize()
        {
			const string json = "{\"id\":\"1\",\"1\":\"1\"}";

            var model = EdgeCaseProperties.Create(1);

			var fromJson = JsonSerializer.DeserializeFromString<EdgeCaseProperties>(json);

			Assert.That(fromJson, Is.EqualTo(model));
        }

        /// <summary>A tree.</summary>
        [DataContract]
        public class Tree
        {
            /// <summary>Gets or sets the value.</summary>
            /// <value>The value.</value>
            [DataMember]
            public string Value { get; set; }

            /// <summary>Gets or sets the nodes.</summary>
            /// <value>The nodes.</value>
            [DataMember]
            public List<Tree> Nodes { get; set; }
        }

        /// <summary>Can serialize and deserialize tree.</summary>
        [Test]
        public void CanSerializeAndDeserializeTree()
        {
            var original = new Tree
                           {
                               Value = "root",
                               Nodes = new List<Tree>
                                       {
                                           new Tree {Value = "foo"},
                                           new Tree {Value = "bar"},
                                           new Tree {Value = "baz"}
                                       }
                           };
            var json = original.ToJson();
            Console.WriteLine(json);
            var result = JsonSerializer.DeserializeFromString<Tree>(json);
            var resultJson = result.ToJson();
            Assert.AreEqual(json, resultJson);
        }
	}
}