using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using NUnit.Framework;
using NServiceKit.Text.Common;

namespace NServiceKit.Text.Tests.JsonTests
{
    /// <summary>A polymorphic instance test.</summary>
	[TestFixture]
	public class PolymorphicInstanceTest 
	{
        /// <summary>Sets the up.</summary>
		[SetUp]
		public void SetUp()
		{
			JsConfig.Reset();
			JsConfig<ICat>.ExcludeTypeInfo = false;
		}

        /// <summary>Can deserialise polymorphic dog exact.</summary>
		[Test]
		public void Can_deserialise_polymorphic_dog_exact()
		{
			var dog =
				JsonSerializer.DeserializeFromString<Dog>(
					@"{""__type"":"""
					+ typeof(Dog).ToTypeString()
					+ @""",""Name"":""Fido""}");

			Assert.That(dog.Name, Is.EqualTo(@"Fido"));

		}

        /// <summary>
        /// Can deserialise polymorphic list exact with no side effect for bad type position.
        /// </summary>
		[Test]
		public void Can_deserialise_polymorphic_list_exact_with_no_side_effect_for_bad_type_position()
		{
			var dog =
				JsonSerializer.DeserializeFromString<Dog>(
					@"{""Name"":""Fido"",""__type"":"""
					+ typeof(Dog).ToTypeString() + @"""}");

			Assert.That(dog.Name, Is.EqualTo(@"Fido"));

		}

	}
}
