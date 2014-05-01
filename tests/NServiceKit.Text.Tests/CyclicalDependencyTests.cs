using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace NServiceKit.Text.Tests
{
    /// <summary>A module.</summary>
    public class Module
    {
        /// <summary>Initializes a new instance of the NServiceKit.Text.Tests.Module class.</summary>
        public Module()
        {
            ExtendedData = new Dictionary<string, object>();
        }

        /// <summary>Gets or sets the name.</summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>Gets or sets the version.</summary>
        /// <value>The version.</value>
        public string Version { get; set; }

        /// <summary>Gets or sets information describing the extended.</summary>
        /// <value>Information describing the extended.</value>
        public IDictionary<string, object> ExtendedData { get; set; }
    }

    /// <summary>A stack frame.</summary>
    public class StackFrame
    {
        /// <summary>
        /// Initializes a new instance of the NServiceKit.Text.Tests.StackFrame class.
        /// </summary>
        public StackFrame()
        {
            ExtendedData = new Dictionary<string, object>();
            Parameters = new Collection<Parameter>();
        }

        /// <summary>Gets or sets the filename of the file.</summary>
        /// <value>The name of the file.</value>
        public string FileName { get; set; }

        /// <summary>Gets or sets the line number.</summary>
        /// <value>The line number.</value>
        public int LineNumber { get; set; }

        /// <summary>Gets or sets the column.</summary>
        /// <value>The column.</value>
        public int Column { get; set; }

        /// <summary>Gets or sets information describing the extended.</summary>
        /// <value>Information describing the extended.</value>
        public IDictionary<string, object> ExtendedData { get; set; }

        /// <summary>Gets or sets the type.</summary>
        /// <value>The type.</value>
        public string Type { get; set; }

        /// <summary>Gets or sets the namespace.</summary>
        /// <value>The namespace.</value>
        public string Namespace { get; set; }

        /// <summary>Gets or sets the module.</summary>
        /// <value>The module.</value>
        public Module Module { get; set; }

        /// <summary>Gets or sets the method.</summary>
        /// <value>The method.</value>
        public string Method { get; set; }

        /// <summary>Gets or sets options for controlling the operation.</summary>
        /// <value>The parameters.</value>
        public ICollection<Parameter> Parameters { get; set; }
    }

    /// <summary>A parameter.</summary>
    public class Parameter
    {
        /// <summary>
        /// Initializes a new instance of the NServiceKit.Text.Tests.Parameter class.
        /// </summary>
        public Parameter()
        {
            ExtendedData = new Dictionary<string, object>();
        }

        /// <summary>Gets or sets the name.</summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>Gets or sets the type.</summary>
        /// <value>The type.</value>
        public string Type { get; set; }

        /// <summary>Gets or sets information describing the extended.</summary>
        /// <value>Information describing the extended.</value>
        public IDictionary<string, object> ExtendedData { get; set; }
    }

    /// <summary>An error.</summary>
    public class Error
    {
        /// <summary>Initializes a new instance of the NServiceKit.Text.Tests.Error class.</summary>
        public Error()
        {
            ExtendedData = new Dictionary<string, object>();
            Tags = new HashSet<string>();
            StackTrace = new Collection<StackFrame>();
        }

        /// <summary>Gets or sets the identifier.</summary>
        /// <value>The identifier.</value>
        public string Id { get; set; }

        /// <summary>Gets or sets the message.</summary>
        /// <value>The message.</value>
        public string Message { get; set; }

        /// <summary>Gets or sets the type.</summary>
        /// <value>The type.</value>
        public string Type { get; set; }

        /// <summary>Gets or sets the module.</summary>
        /// <value>The module.</value>
        public Module Module { get; set; }

        /// <summary>Gets or sets the description.</summary>
        /// <value>The description.</value>
        public string Description { get; set; }

        /// <summary>Gets or sets the occurrence date.</summary>
        /// <value>The occurrence date.</value>
        public DateTime OccurrenceDate { get; set; }

        /// <summary>Gets or sets the code.</summary>
        /// <value>The code.</value>
        public string Code { get; set; }

        /// <summary>Gets or sets information describing the extended.</summary>
        /// <value>Information describing the extended.</value>
        public IDictionary<string, object> ExtendedData { get; set; }

        /// <summary>Gets or sets the tags.</summary>
        /// <value>The tags.</value>
        public HashSet<string> Tags { get; set; }

        /// <summary>Gets or sets the inner.</summary>
        /// <value>The inner.</value>
        public Error Inner { get; set; }

        /// <summary>Gets or sets the stack trace.</summary>
        /// <value>The stack trace.</value>
        public ICollection<StackFrame> StackTrace { get; set; }

        /// <summary>Gets or sets the contact.</summary>
        /// <value>The contact.</value>
        public string Contact { get; set; }

        /// <summary>Gets or sets the notes.</summary>
        /// <value>The notes.</value>
        public string Notes { get; set; }
    }

    /// <summary>A cyclical dependency tests.</summary>
    [TestFixture]
    public class CyclicalDependencyTests : TestBase
    {
        /// <summary>Can serialize error.</summary>
        [Test]
        public void Can_serialize_Error()
        {
            var dto = new Error
            {
                Id = "Id",
                Message = "Message",
                Type = "Type",
                Description = "Description",
                OccurrenceDate = new DateTime(2012, 01, 01),
                Code = "Code",
                ExtendedData = new Dictionary<string, object> { { "Key", "Value" } },
                Tags = new HashSet<string> { "C#", "ruby" },
                Inner = new Error
                {
                    Id = "Id2",
                    Message = "Message2",
                    ExtendedData = new Dictionary<string, object> { { "InnerKey", "InnerValue" } },
                    Module = new Module
                    {
                        Name = "Name",
                        Version = "v1.0"
                    },
                    StackTrace = new Collection<StackFrame> {
						new StackFrame {
							Column = 1,
							Module = new Module {
								Name = "StackTrace.Name",
								Version = "StackTrace.v1.0"
							},
							ExtendedData = new Dictionary<string, object> { { "StackTraceKey", "StackTraceValue" } },
							FileName = "FileName",
							Type = "Type",
							LineNumber = 1,
							Method = "Method",
							Namespace = "Namespace",
							Parameters = new Collection<Parameter> {
								new Parameter { Name = "Parameter", Type = "ParameterType" },
							}
						}
					}
                },
                Contact = "Contact",
                Notes = "Notes",
            };

            var from = Serialize(dto, includeXml: false);
            //Console.WriteLine(from.Dump());

            Assert.That(from.Id, Is.EqualTo(dto.Id));
            Assert.That(from.Message, Is.EqualTo(dto.Message));
            Assert.That(from.Type, Is.EqualTo(dto.Type));
            Assert.That(from.Description, Is.EqualTo(dto.Description));
            Assert.That(from.OccurrenceDate, Is.EqualTo(dto.OccurrenceDate));
            Assert.That(from.Code, Is.EqualTo(dto.Code));

            Assert.That(from.Inner.Id, Is.EqualTo(dto.Inner.Id));
            Assert.That(from.Inner.Message, Is.EqualTo(dto.Inner.Message));
            Assert.That(from.Inner.ExtendedData["InnerKey"], Is.EqualTo(dto.Inner.ExtendedData["InnerKey"]));
            Assert.That(from.Inner.Module.Name, Is.EqualTo(dto.Inner.Module.Name));
            Assert.That(from.Inner.Module.Version, Is.EqualTo(dto.Inner.Module.Version));

            var actualStack = from.Inner.StackTrace.First();
            var expectedStack = dto.Inner.StackTrace.First();
            Assert.That(actualStack.Column, Is.EqualTo(expectedStack.Column));
            Assert.That(actualStack.FileName, Is.EqualTo(expectedStack.FileName));
            Assert.That(actualStack.Type, Is.EqualTo(expectedStack.Type));
            Assert.That(actualStack.LineNumber, Is.EqualTo(expectedStack.LineNumber));
            Assert.That(actualStack.Method, Is.EqualTo(expectedStack.Method));

            Assert.That(from.Contact, Is.EqualTo(dto.Contact));
            Assert.That(from.Notes, Is.EqualTo(dto.Notes));
        }

        /// <summary>A person.</summary>
        class person
        {
            /// <summary>Gets or sets the name.</summary>
            /// <value>The name.</value>
            public string name { get; set; }

            /// <summary>Gets or sets the teacher.</summary>
            /// <value>The teacher.</value>
            public person teacher { get; set; }
        }

        /// <summary>Can limit cyclical dependencies.</summary>
        [Test]
        public void Can_limit_cyclical_dependencies()
        {
            using (JsConfig.With(maxDepth: 4))
            {
                var p = new person();
                p.teacher = new person { name = "sam", teacher = p };
                p.name = "bob";
                p.PrintDump();
                p.ToJsv().Print();
                p.ToJson().Print();
            }
        }


    }
}
