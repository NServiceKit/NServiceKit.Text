using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace NServiceKit.Text.Tests.JsonTests {
	#region Test case types
    /// <summary>Dictionary of contains is.</summary>
	public class ContainsIDictionary {

        /// <summary>Gets or sets the container.</summary>
        /// <value>The container.</value>
		public IDictionary Container { get; set; }
	}

    /// <summary>Dictionary of contains generic strings.</summary>
	public class ContainsGenericStringDictionary {

        /// <summary>Gets or sets the container.</summary>
        /// <value>The container.</value>
		public Dictionary<string, string> Container { get; set; }
	}

    /// <summary>Dictionary of several types ofs.</summary>
	public class SeveralTypesOfDictionary {

        /// <summary>Gets or sets the unique identifier to int.</summary>
        /// <value>The unique identifier to int.</value>
		public IDictionary GuidToInt { get; set; }

        /// <summary>Gets or sets the date time to dictionary string.</summary>
        /// <value>The date time to dictionary string.</value>
		public IDictionary DateTimeTo_DictStrStr { get; set; }
	}

	#endregion

    /// <summary>A basic properties tests.</summary>
	[TestFixture]
	public class BasicPropertiesTests {

        /// <summary>Generic dictionary backed i dictionary round trips ok.</summary>
		[Test]
		public void Generic_dictionary_backed_IDictionary_round_trips_ok () {
			var original = new ContainsIDictionary {
				Container = new Dictionary<string, string>
                {
                    {"one", "header one"},
                    {"two", "header two"}
                }
			};

			var str = JsonSerializer.SerializeToString(original);
			var obj = JsonSerializer.DeserializeFromString<ContainsIDictionary>(str);

			Console.WriteLine(DictStr(obj.Container));
			Assert.That(DictStr(obj.Container), Is.EqualTo(DictStr(original.Container)));
		}

        /// <summary>
        /// Generic dictionary backed i dictionary deserialises to generic dictionary.
        /// </summary>
		[Test]
		public void Generic_dictionary_backed_IDictionary_deserialises_to_generic_dictionary () {
			var original = new ContainsIDictionary // Using IDictionary backing
			{
				Container = new Dictionary<string, string>
                {
                    {"one", "header one"},
                    {"two", "header two"}
                }
			};

			var str = JsonSerializer.SerializeToString(original);
			var obj = JsonSerializer.DeserializeFromString<ContainsGenericStringDictionary>(str); // decoding to Dictionary<,>

			Console.WriteLine(DictStr(obj.Container));
			Assert.That(DictStr(obj.Container), Is.EqualTo(DictStr(original.Container)));
		}

        /// <summary>Generic dictionary deserialises to i dictionary.</summary>
		[Test]
		public void Generic_dictionary_deserialises_to_IDictionary () {
			var original = new ContainsGenericStringDictionary // Using Dictionary<,> backing
			{
				Container = new Dictionary<string, string>
                {
                    {"one", "header one"},
                    {"two", "header two"}
                }
			};

			var str = JsonSerializer.SerializeToString(original);
			var obj = JsonSerializer.DeserializeFromString<ContainsIDictionary>(str); // decoding to IDictionary

			Console.WriteLine(DictStr(obj.Container));
			Assert.That(DictStr(obj.Container), Is.EqualTo(DictStr(original.Container)));
		}

        /// <summary>Generic dictionary round trips ok.</summary>
		[Test]
		public void Generic_dictionary_round_trips_ok () {
			var original = new ContainsGenericStringDictionary {
				Container = new Dictionary<string, string>
                {
                    {"one", "header one"},
                    {"two", "header two"}
                }
			};

			var str = JsonSerializer.SerializeToString(original);
			var obj = JsonSerializer.DeserializeFromString<ContainsGenericStringDictionary>(str);

			Console.WriteLine(DictStr(obj.Container));
			Assert.That(DictStr(obj.Container), Is.EqualTo(DictStr(original.Container)));
		}

        /// <summary>Generic dictionary and i dictionary serialise the same.</summary>
		[Test]
		public void Generic_dictionary_and_IDictionary_serialise_the_same () {
			JsConfig.PreferInterfaces = true;
			JsConfig.ExcludeTypeInfo = false;
			JsConfig.ConvertObjectTypesIntoStringDictionary = false;

			var genericStringDictionary = new ContainsGenericStringDictionary {
				Container = new Dictionary<string, string>
                {
                    {"one", "header one"},
                    {"two", "header two"}
                }
			};
			var iDictionary = new ContainsIDictionary {
				Container = new Dictionary<string, string>
                {
                    {"one", "header one"},
                    {"two", "header two"}
                }
			};

			var genDict = genericStringDictionary.ToJson();
			var iDict = iDictionary.ToJson();

			Console.WriteLine("Dictionary<string,string> --> " + genDict);
			Console.WriteLine();
			Console.WriteLine("IDictionary               --> " + iDict);

			Assert.That(genDict, Is.EqualTo(iDict));
		}

        /// <summary>Complex dictionaries round trip.</summary>
		[Test]
		[Ignore("Very complex mappings, not needed for most tasks.")]
		public void Complex_dictionaries_round_trip () {
			var original = new SeveralTypesOfDictionary {
				GuidToInt = new Dictionary<Guid, int>
                {
                    {Guid.Empty, 10},
                    {Guid.NewGuid(), 25}
                },
				DateTimeTo_DictStrStr = new Dictionary<DateTime, Dictionary<string, string>> {
					{DateTime.Today, new Dictionary<string, string> {{"a","b"},{"c","d"}}},
					{DateTime.Now, new Dictionary<string, string> {{"a","b"},{"c","d"}}}
				}
			};
			// see WriteDictionary.cs line 105
			// Problems:
			//   - Int is turning into String on Deserialise
			//   - Dictionary of dictionaries is totally failing on Deserialise
			var string_a = original.ToJson();
			var copy_a = string_a.FromJson<SeveralTypesOfDictionary>();
			var string_b = copy_a.ToJson();
			var copy_b = string_b.FromJson<SeveralTypesOfDictionary>();

			Console.WriteLine(string_a);
			Console.WriteLine(string_b);
			Assert.That(copy_a.GuidToInt[Guid.Empty], Is.EqualTo(10), "First copy was incorrect");
			Assert.That(copy_b.GuidToInt[Guid.Empty], Is.EqualTo(10), "Second copy was incorrect");
			Assert.That(string_a, Is.EqualTo(string_b), "Serialised forms not same");
		}

        /// <summary>Dictionary string.</summary>
        /// <param name="d">The IDictionary to process.</param>
        /// <returns>A string.</returns>
		static string DictStr (IDictionary d) {
			var sb = new StringBuilder();
			foreach (var key in d.Keys) { sb.AppendLine(key + " = " + d[key]); }
			return sb.ToString();
		}
	}
}
