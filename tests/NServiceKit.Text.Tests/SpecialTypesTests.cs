using System;
using System.Collections;
using NUnit.Framework;

namespace NServiceKit.Text.Tests
{
    /// <summary>A special types tests.</summary>
	[TestFixture]
	public class SpecialTypesTests
		: TestBase
	{
        /// <summary>Can serialize version.</summary>
		[Test]
		public void Can_Serialize_Version()
		{
			Serialize(new Version());
			Serialize(Environment.Version);
		}

        /// <summary>A JSON entity with private getter.</summary>
		public class JsonEntityWithPrivateGetter
		{
            /// <summary>Sets the name.</summary>
            /// <value>The name.</value>
			public string Name { private get; set; }
		}

        /// <summary>A JSON entity with no properties.</summary>
		public class JsonEntityWithNoProperties
		{
		}

        /// <summary>Can serialize type with no public getters.</summary>
		[Test]
		public void Can_Serialize_Type_with_no_public_getters()
		{
			Serialize(new JsonEntityWithPrivateGetter { Name = "Daniel" });
		}

        /// <summary>Can serialize type with no public properties.</summary>
		[Test]
		public void Can_Serialize_Type_with_no_public_properties()
		{
			Serialize(new JsonEntityWithNoProperties());
		}

        /// <summary>Can serialize type with byte array.</summary>
		[Test]
		public void Can_Serialize_Type_with_ByteArray()
		{
			var test = new { Name = "Test", Data = new byte[] { 1, 2, 3, 4, 5 } };
			var json = JsonSerializer.SerializeToString(test);
			Assert.That(json, Is.EquivalentTo("{\"Name\":\"Test\",\"Data\":\"AQIDBAU=\"}"));
		}

        /// <summary>Can serialize byte array.</summary>
		[Test]
		public void Can_Serialize_ByteArray()
		{
			var test = new byte[] { 1, 2, 3, 4, 5 };
			var json = JsonSerializer.SerializeToString(test);
			var fromJson = JsonSerializer.DeserializeFromString<byte[]>(json);

			Assert.That(test, Is.EquivalentTo(fromJson));
		}

        /// <summary>Can serialize hash table.</summary>
	    [Test]
	    public void Can_Serialize_HashTable()
	    {
            var h = new Hashtable { { "A", 1 }, { "B", 2 } };
	        var fromJson = h.ToJson().FromJson<Hashtable>();
            Assert.That(fromJson.Count, Is.EqualTo(h.Count));
            Assert.That(fromJson["A"].ToString(), Is.EqualTo(h["A"].ToString()));
            Assert.That(fromJson["B"].ToString(), Is.EqualTo(h["B"].ToString()));
	    }

        /// <summary>Can serialize delegate.</summary>
	    [Test]
	    public void Can_serialize_delegate()
	    {
            Action x = () => { };

            Assert.That(x.ToJson(), Is.Null);
            Assert.That(x.ToJsv(), Is.Null);
            Assert.That(x.Dump(), Is.Not.Null);
	    }

        /// <summary>Method with arguments.</summary>
        /// <param name="id">  The identifier.</param>
        /// <param name="name">The name.</param>
        /// <returns>A string.</returns>
        string MethodWithArgs(int id, string name)
        {
            return null;
        }

        /// <summary>Does dump delegate information.</summary>
	    [Ignore("TODO: Understand if this is by design or not.")]
	    [Test]
	    public void Does_dump_delegate_info()
	    {
            Action d = Can_Serialize_ByteArray;
            Assert.That(d.Dump(), Is.EqualTo("Void Can_Serialize_ByteArray()"));

	        Func<int, string, string> methodWithArgs = MethodWithArgs;
            Assert.That(methodWithArgs.Dump(), Is.EqualTo("String MethodWithArgs(Int32 arg1, String arg2)"));

            Action x = () => { };
            Assert.That(x.Dump(), Is.EqualTo("Void <Does_dump_delegate_info>b__4()"));
        }
	}
}
