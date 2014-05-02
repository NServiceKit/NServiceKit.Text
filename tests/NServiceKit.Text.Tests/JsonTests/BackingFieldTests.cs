using System;
using NUnit.Framework;

namespace NServiceKit.Text.Tests.JsonTests
{

	#region Test types
    /// <summary>A get only with backing.</summary>
	public class GetOnlyWithBacking
	{
        /// <summary>The backing.</summary>
		long backing;

        /// <summary>
        /// Initializes a new instance of the NServiceKit.Text.Tests.JsonTests.GetOnlyWithBacking
        /// class.
        /// </summary>
        /// <param name="i">Zero-based index of the.</param>
		public GetOnlyWithBacking(long i)
		{
			backing = i;
		}

        /// <summary>Gets the property.</summary>
        /// <value>The property.</value>
		public long Property
		{
			get { return backing; }
		}
	}

    /// <summary>A get set with backing.</summary>
	public class GetSetWithBacking
	{
        /// <summary>The backing.</summary>
		long backing;

        /// <summary>
        /// Initializes a new instance of the NServiceKit.Text.Tests.JsonTests.GetSetWithBacking
        /// class.
        /// </summary>
        /// <param name="i">Zero-based index of the.</param>
		public GetSetWithBacking(long i)
		{
			Property = i;
		}

        /// <summary>Gets or sets the property.</summary>
        /// <value>The property.</value>
		public long Property
		{
			get { return backing; }
			set { backing = value; }
		}
	}

	#endregion

    /// <summary>A backing field tests.</summary>
	[TestFixture]
	public class BackingFieldTests
	{
        /// <summary>Backed get set properties can be deserialised.</summary>
		[Test]
		public void Backed_get_set_properties_can_be_deserialised()
		{
			var original = new GetSetWithBacking(123344044);
			var str1 = original.ToJson();
			var copy = str1.FromJson<GetSetWithBacking>();

			Console.WriteLine(str1);

			Assert.That(copy.Property, Is.EqualTo(original.Property));
		}

        /// <summary>Backed get properties can be deserialised.</summary>
        [Ignore("By Design: Deserialization doesn't use constructor injection, Properties need to be writeable")]
		[Test]
		public void Backed_get_properties_can_be_deserialised()
		{
			var original = new GetOnlyWithBacking(123344044);
			var str1 = original.ToJson();
			var copy = str1.FromJson<GetOnlyWithBacking>();

			Console.WriteLine(str1);

			// ReflectionExtensions.cs Line 417 is being used to determine *deserialisable*
			// for properties type based on if the property is *readable*, not *writable* -- by design

            //Rule: To be emitted properties should be readable, to be deserialized properties should be writeable

			Assert.That(copy.Property, Is.EqualTo(original.Property));
		}
	}
}
