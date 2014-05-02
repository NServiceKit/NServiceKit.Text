using NServiceKit.Common.Tests.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace NServiceKit.Text.Tests.Utils
{
    /// <summary>A jsv formatter tests.</summary>
    [TestFixture]
    public class JsvFormatterTests
    {
        /// <summary>Can pretty format generic type.</summary>
        [Test]
        public void Can_PrettyFormat_generic_type()
        {
            var model = new ModelWithIdAndName { Id = 1, Name = "Name" };
            var modelStr = model.Dump();

            Assert.That(modelStr,
                        Is.EqualTo(
                            "{"
                            + Environment.NewLine
                            + "\tId: 1,"
                            + Environment.NewLine
                            + "\tName: Name"
                            + Environment.NewLine
                            + "}"
                        ));
        }

        /// <summary>Can pretty format object.</summary>
        [Test]
        public void Can_PrettyFormat_object()
        {
            object model = new ModelWithIdAndName { Id = 1, Name = "Name" };
            var modelStr = model.Dump();

            Assert.That(modelStr,
                        Is.EqualTo(
                            "{"
                            + Environment.NewLine
                            + "\tId: 1,"
                            + Environment.NewLine
                            + "\tName: Name"
                            + Environment.NewLine
                            + "}"
                        ));
        }

        /// <summary>A data Model for the test.</summary>
        internal class TestModel
        {
            /// <summary>
            /// Initializes a new instance of the NServiceKit.Text.Tests.Utils.JsvFormatterTests.TestModel
            /// class.
            /// </summary>
            public TestModel()
            {
                this.Int = 1;
                this.String = "One";
                this.DateTime = DateTime.UtcNow.Date;
                this.Guid = Guid.NewGuid();
                this.EmptyIntList = new List<int>();
                this.IntList = new List<int> { 1, 2, 3 };
                this.StringList = new List<string> { "one", "two", "three" };
                this.StringIntMap = new Dictionary<string, int>
    				{
    					{"a", 1},{"b", 2},{"c", 3},
    				};
            }

            /// <summary>Gets or sets the int.</summary>
            /// <value>The int.</value>
            public int Int { get; set; }

            /// <summary>Gets or sets the string.</summary>
            /// <value>The string.</value>
            public string String { get; set; }

            /// <summary>Gets or sets the date time.</summary>
            /// <value>The date time.</value>
            public DateTime DateTime { get; set; }

            /// <summary>Gets or sets a unique identifier.</summary>
            /// <value>The identifier of the unique.</value>
            public Guid Guid { get; set; }

            /// <summary>Gets or sets a list of empty ints.</summary>
            /// <value>A List of empty ints.</value>
            public List<int> EmptyIntList { get; set; }

            /// <summary>Gets or sets a list of ints.</summary>
            /// <value>A List of ints.</value>
            public List<int> IntList { get; set; }

            /// <summary>Gets or sets a list of strings.</summary>
            /// <value>A List of strings.</value>
            public List<string> StringList { get; set; }

            /// <summary>Gets or sets the string int map.</summary>
            /// <value>The string int map.</value>
            public Dictionary<string, int> StringIntMap { get; set; }
        }

        /// <summary>Can dump model.</summary>
        [Test]
        public void Can_DumpModel()
        {
            var model = new TestModel();
            Assert.IsNotNullOrEmpty(model.Dump());
        }

    }
}