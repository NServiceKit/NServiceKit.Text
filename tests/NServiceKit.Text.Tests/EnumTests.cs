using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace NServiceKit.Text.Tests
{
    /// <summary>An enum tests.</summary>
    [TestFixture]
    public class EnumTests
    {
        /// <summary>Sets the up.</summary>
        [SetUp]
        public void SetUp()
        {
            JsConfig.Reset();
        }

        /// <summary>Values that represent EnumWithoutFlags.</summary>
        public enum EnumWithoutFlags
        {
            /// <summary>An enum constant representing the one option.</summary>
            One = 1,

            /// <summary>An enum constant representing the two option.</summary>
            Two = 2
        }

        /// <summary>Bitfield of flags for specifying EnumWithFlags.</summary>
        [Flags]
        public enum EnumWithFlags
        {
            /// <summary>A binary constant representing the one flag.</summary>
            One = 1,

            /// <summary>A binary constant representing the two flag.</summary>
            Two = 2
        }

        /// <summary>The class with enums.</summary>
        public class ClassWithEnums
        {
            /// <summary>Gets or sets the flags enum.</summary>
            /// <value>The flags enum.</value>
            public EnumWithFlags FlagsEnum { get; set; }

            /// <summary>Gets or sets the no flags enum.</summary>
            /// <value>The no flags enum.</value>
            public EnumWithoutFlags NoFlagsEnum { get; set; }

            /// <summary>Gets or sets the nullable flags enum.</summary>
            /// <value>The nullable flags enum.</value>
            public EnumWithFlags? NullableFlagsEnum { get; set; }

            /// <summary>Gets or sets the nullable no flags enum.</summary>
            /// <value>The nullable no flags enum.</value>
            public EnumWithoutFlags? NullableNoFlagsEnum { get; set; }
        }

        /// <summary>Can correctly serialize enums.</summary>
        [Test]
        public void Can_correctly_serialize_enums()
        {
            var item = new ClassWithEnums
            {
                FlagsEnum = EnumWithFlags.One,
                NoFlagsEnum = EnumWithoutFlags.One,
                NullableFlagsEnum = EnumWithFlags.Two,
                NullableNoFlagsEnum = EnumWithoutFlags.Two
            };

            const string expected = "{\"FlagsEnum\":1,\"NoFlagsEnum\":\"One\",\"NullableFlagsEnum\":2,\"NullableNoFlagsEnum\":\"Two\"}";
            var text = JsonSerializer.SerializeToString(item);

            Assert.AreEqual(expected, text);
        }

        /// <summary>Should deserialize enum.</summary>
        public void Should_deserialize_enum()
        {
            Assert.That(JsonSerializer.DeserializeFromString<EnumWithoutFlags>("\"Two\""), Is.EqualTo(EnumWithoutFlags.Two));
        }

        /// <summary>Should handle empty enum.</summary>
        public void Should_handle_empty_enum()
        {
            Assert.That(JsonSerializer.DeserializeFromString<EnumWithoutFlags>(""), Is.EqualTo((EnumWithoutFlags)0));
        }

        /// <summary>Can serialize int flag.</summary>
        [Test]
        public void CanSerializeIntFlag()
        {
            JsConfig.TreatEnumAsInteger = true;
            var val = JsonSerializer.SerializeToString(FlagEnum.A);

            Assert.AreEqual("0", val);
        }

        /// <summary>Can serialize sbyte flag.</summary>
        [Test]
        public void CanSerializeSbyteFlag()
        {
            JsConfig.TryToParsePrimitiveTypeValues = true;
            JsConfig.TreatEnumAsInteger = true;
            JsConfig.IncludeNullValues = true;
            var val = JsonSerializer.SerializeToString(SbyteFlagEnum.A);

            Assert.AreEqual("0", val);
        }

        /// <summary>Bitfield of flags for specifying FlagEnum.</summary>
        [Flags]
        public enum FlagEnum
        {
            /// <summary>A binary constant representing a flag.</summary>
            A,

            /// <summary>A binary constant representing the b flag.</summary>
            B
        }

        /// <summary>Bitfield of flags for specifying SbyteFlagEnum.</summary>
        [Flags]
        public enum SbyteFlagEnum : sbyte
        {
            /// <summary>A binary constant representing a flag.</summary>
            A,

            /// <summary>A binary constant representing the b flag.</summary>
            B
        }

        /// <summary>Bitfield of flags for specifying AnEnum.</summary>
        [Flags]
        public enum AnEnum
        {
            /// <summary>A binary constant representing this flag.</summary>
            This,

            /// <summary>A binary constant representing the is flag.</summary>
            Is,

            /// <summary>A binary constant representing an flag.</summary>
            An,

            /// <summary>A binary constant representing the enum flag.</summary>
            Enum
        }

        /// <summary>Can use enum as key in map.</summary>
        [Test]
        public void Can_use_enum_as_key_in_map()
        {
            var dto = new Dictionary<AnEnum, int> { { AnEnum.This, 1 } };
            var json = dto.ToJson();
            json.Print();
            
            var map = json.FromJson<Dictionary<AnEnum, int>>();
            Assert.That(map[AnEnum.This], Is.EqualTo(1));
        }

    }
}

