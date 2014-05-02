using System;
using System.Drawing;
using System.Runtime.Serialization;
using NUnit.Framework;
using NServiceKit.Common;

namespace NServiceKit.Text.Tests
{
    /// <summary>A bcl structure tests.</summary>
	public class BclStructTests : TestBase
	{
        /// <summary>
        /// Initializes static members of the NServiceKit.Text.Tests.BclStructTests class.
        /// </summary>
		static BclStructTests()
		{
			JsConfig<Color>.SerializeFn = c => c.ToString().Replace("Color ", "").Replace("[", "").Replace("]", "");
			JsConfig<Color>.DeSerializeFn = Color.FromName;
		}

        /// <summary>Can serialize color.</summary>
		[Test]
		public void Can_serialize_Color()
		{
			var color = Color.Red;

			var fromColor = Serialize(color);

			Assert.That(fromColor, Is.EqualTo(color));
		}

        /// <summary>Values that represent MyEnum.</summary>
		public enum MyEnum
		{
            /// <summary>An enum constant representing the enum 1 option.</summary>
			Enum1,

            /// <summary>An enum constant representing the enum 2 option.</summary>
			Enum2,

            /// <summary>An enum constant representing the enum 3 option.</summary>
			Enum3,
		}

        /// <summary>Can serialize arrays of enums.</summary>
		[Test]
		public void Can_serialize_arrays_of_enums()
		{
			var enums = new[] { MyEnum.Enum1, MyEnum.Enum2, MyEnum.Enum3 };
			var fromEnums = Serialize(enums);

			Assert.That(fromEnums[0], Is.EqualTo(MyEnum.Enum1));
			Assert.That(fromEnums[1], Is.EqualTo(MyEnum.Enum2));
			Assert.That(fromEnums[2], Is.EqualTo(MyEnum.Enum3));
		}

        /// <summary>Bitfield of flags for specifying ExampleEnum.</summary>
        [Flags]
        public enum ExampleEnum
        {
            /// <summary>A binary constant representing the none flag.</summary>
            None = 0,

            /// <summary>A binary constant representing the one flag.</summary>
            One = 1,

            /// <summary>A binary constant representing the two flag.</summary>
            Two = 2,

            /// <summary>A binary constant representing the four flag.</summary>
            Four = 4,

            /// <summary>A binary constant representing the eight flag.</summary>
            Eight = 8
        }

        /// <summary>An example type.</summary>
        public class ExampleType
        {
            /// <summary>Gets or sets the enum.</summary>
            /// <value>The enum.</value>
            public ExampleEnum Enum { get; set; }

            /// <summary>Gets or sets the enum values.</summary>
            /// <value>The enum values.</value>
            public string EnumValues { get; set; }

            /// <summary>Gets or sets the value.</summary>
            /// <value>The value.</value>
            public string Value { get; set; }

            /// <summary>Gets or sets the foo.</summary>
            /// <value>The foo.</value>
            public int Foo { get; set; }
        }

        /// <summary>Can serialize dto with enum flags.</summary>
        [Test]
        public void Can_serialize_dto_with_enum_flags()
        {
            var serialized = TypeSerializer.SerializeToString(new ExampleType
            {
                Value = "test",
                Enum = ExampleEnum.One | ExampleEnum.Four,
                EnumValues = (ExampleEnum.One | ExampleEnum.Four).ToDescription(),
                Foo = 1
            });

            var deserialized = TypeSerializer.DeserializeFromString<ExampleType>(serialized);

            Console.WriteLine(deserialized.ToJsv());

            Assert.That(deserialized.Enum, Is.EqualTo(ExampleEnum.One | ExampleEnum.Four));
        }

        /// <summary>An item.</summary>
        [DataContract]
        public class Item
        {
            /// <summary>Gets or sets a value indicating whether this object is favorite.</summary>
            /// <value>true if this object is favorite, false if not.</value>
            [DataMember(Name = "favorite")]
            public bool IsFavorite { get; set; }
        }

        /// <summary>Can customize bool deserialization.</summary>
        [Test]
        public void Can_customize_bool_deserialization()
        {
            var dto1 = "{\"favorite\":1}".FromJson<Item>();
            Assert.That(dto1.IsFavorite, Is.True);

            var dto0 = "{\"favorite\":0}".FromJson<Item>();
            Assert.That(dto0.IsFavorite, Is.False);

            var dtoTrue = "{\"favorite\":true}".FromJson<Item>();
            Assert.That(dtoTrue.IsFavorite, Is.True);

            var dtoFalse = "{\"favorite\":false}".FromJson<Item>();
            Assert.That(dtoFalse.IsFavorite, Is.False);
        }
    }
}