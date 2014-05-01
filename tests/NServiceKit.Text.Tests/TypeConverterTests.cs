using System;
using System.ComponentModel;
using System.Security.Policy;
using NUnit.Framework;

namespace NServiceKit.Text.Tests
{
    /// <summary>A type converter tests.</summary>
	[TestFixture]
	public class TypeConverterTests
	{
        /// <summary>Exception for signalling custom errors.</summary>
		public class CustomException
			: Exception
		{
            /// <summary>
            /// Initializes a new instance of the NServiceKit.Text.Tests.TypeConverterTests.CustomException
            /// class.
            /// </summary>
            /// <param name="message">The message.</param>
			public CustomException(string message) : base(message)
			{
				this.CustomMessage = "Custom" + message;
			}

            /// <summary>Gets or sets a message describing the custom.</summary>
            /// <value>A message describing the custom.</value>
			public string CustomMessage { get; set; }
		}

        /// <summary>View type converter outputs.</summary>
		[Test]
		public void View_TypeConverter_outputs()
		{
			var converter1 = TypeDescriptor.GetConverter(typeof(Url));
			Console.WriteLine(converter1.ConvertToString(new Url("http://io/")));

			var converter2 = TypeDescriptor.GetConverter(typeof(Type));
			Console.WriteLine(converter2.ConvertToString(typeof(TypeConverterTests)));

			var converter3 = TypeDescriptor.GetConverter(typeof(CustomException));
			var string3 = converter3.ConvertToString(new Exception("Test 123"));
			Console.WriteLine(string3);
			//var value3 = converter3.ConvertFromString(string3);
			//Console.WriteLine(value3.Dump());
		}
	}
}