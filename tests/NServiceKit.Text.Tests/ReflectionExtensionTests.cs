using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace NServiceKit.Text.Tests
{
    /// <summary>A data Model for the test.</summary>
    public class TestModel
    {
        /// <summary>
        /// Initializes a new instance of the NServiceKit.Text.Tests.TestModel class.
        /// </summary>
        public TestModel()
        {
            var i = 0;
            this.PublicInt = i++;
            this.PublicGetInt = i++;
            this.PublicSetInt = i++;
            this.PublicIntField = i++;
            this.PrivateInt = i++;
            this.ProtectedInt = i++;
        }

        /// <summary>Gets or sets the public int.</summary>
        /// <value>The public int.</value>
        public int PublicInt { get; set; }

        /// <summary>Gets the public get int.</summary>
        /// <value>The public get int.</value>
        public int PublicGetInt { get; private set; }

        /// <summary>Sets the public set int.</summary>
        /// <value>The public set int.</value>
        public int PublicSetInt { private get; set; }

        /// <summary>The public int field.</summary>
        public int PublicIntField;

        /// <summary>Gets or sets the private int.</summary>
        /// <value>The private int.</value>
        private int PrivateInt { get; set; }

        /// <summary>Gets or sets the protected int.</summary>
        /// <value>The protected int.</value>
        protected int ProtectedInt { get; set; }

        /// <summary>Int method.</summary>
        /// <returns>An int.</returns>
        public int IntMethod()
        {
            return this.PublicInt;
        }
    }

    /// <summary>A reflection extension tests.</summary>
	[TestFixture]
	public class ReflectionExtensionTests
		: TestBase
	{
        /// <summary>Only serializes public readable properties.</summary>
		[Test]
		public void Only_serializes_public_readable_properties()
		{
			var model = new TestModel();
			var modelStr = TypeSerializer.SerializeToString(model);

			Assert.That(modelStr, Is.EqualTo("{PublicInt:0,PublicGetInt:1}"));

			Serialize(model);
		}

        /// <summary>Can create instances of common collections.</summary>
        [Test]
        public void Can_create_instances_of_common_collections()
        {
            Assert.That(typeof(IEnumerable<TestModel>).CreateInstance() as IEnumerable<TestModel>, Is.Not.Null);
            Assert.That(typeof(ICollection<TestModel>).CreateInstance() as ICollection<TestModel>, Is.Not.Null);
            Assert.That(typeof(IList<TestModel>).CreateInstance() as IList<TestModel>, Is.Not.Null);
            Assert.That(typeof(IDictionary<string, TestModel>).CreateInstance() as IDictionary<string, TestModel>, Is.Not.Null);
            Assert.That(typeof(IDictionary<int, TestModel>).CreateInstance() as IDictionary<int, TestModel>, Is.Not.Null);
            Assert.That(typeof(TestModel[]).CreateInstance() as TestModel[], Is.Not.Null);
        }
    }
}
