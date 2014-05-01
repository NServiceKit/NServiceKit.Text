using System;
using NUnit.Framework;

namespace NServiceKit.Text.Tests.JsonTests
{
    /// <summary>A type with public fields.</summary>
	[Serializable]
	public class TypeWithPublicFields
	{
        /// <summary>The text.</summary>
		public readonly string Text;

        /// <summary>
        /// Initializes a new instance of the NServiceKit.Text.Tests.JsonTests.TypeWithPublicFields
        /// class.
        /// </summary>
        /// <param name="text">The text.</param>
		public TypeWithPublicFields(string text)
		{
			Text = text;
		}
	}

    /// <summary>A public field test.</summary>
	[TestFixture]
	public class PublicFieldTest : TestBase
	{
        /// <summary>Public readonly fields can be deserialized.</summary>
		[Test]
		public void Public_readonly_fields_can_be_deserialized()
		{
			using (JsConfig.BeginScope())
			{
				JsConfig.IncludePublicFields = true;
				var instance = new TypeWithPublicFields("Hello");
				var deserilized = instance.ToJson();

				var copy = deserilized.FromJson<TypeWithPublicFields>();

				Assert.That(copy.Text, Is.EqualTo(instance.Text));
			}
		}
	}
}