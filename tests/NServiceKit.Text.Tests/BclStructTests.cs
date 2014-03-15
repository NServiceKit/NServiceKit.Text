﻿using System;
using System.Drawing;
using System.Runtime.Serialization;
using NUnit.Framework;
using NServiceKit.Common;

namespace NServiceKit.Text.Tests
{
	public class BclStructTests : TestBase
	{
		static BclStructTests()
		{
			JsConfig<Color>.SerializeFn = c => c.ToString().Replace("Color ", "").Replace("[", "").Replace("]", "");
			JsConfig<Color>.DeSerializeFn = Color.FromName;
		}

		[Test]
		public void Can_serialize_Color()
		{
			var color = Color.Red;

			var fromColor = Serialize(color);

			Assert.That(fromColor, Is.EqualTo(color));
		}

		public enum MyEnum
		{
			Enum1,
			Enum2,
			Enum3,
		}

		[Test]
		public void Can_serialize_arrays_of_enums()
		{
			var enums = new[] { MyEnum.Enum1, MyEnum.Enum2, MyEnum.Enum3 };
			var fromEnums = Serialize(enums);

			Assert.That(fromEnums[0], Is.EqualTo(MyEnum.Enum1));
			Assert.That(fromEnums[1], Is.EqualTo(MyEnum.Enum2));
			Assert.That(fromEnums[2], Is.EqualTo(MyEnum.Enum3));
		}

        [Flags]
        public enum ExampleEnum
        {
            None = 0,
            One = 1,
            Two = 2,
            Four = 4,
            Eight = 8
        }

        public class ExampleType
        {
            public ExampleEnum Enum { get; set; }
            public string EnumValues { get; set; }
            public string Value { get; set; }
            public int Foo { get; set; }
        }

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

        [DataContract]
        public class Item
        {
            [DataMember(Name = "favorite")]
            public bool IsFavorite { get; set; }
        }

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