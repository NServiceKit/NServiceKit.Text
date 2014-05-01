using System;
using System.Collections.Generic;

namespace NServiceKit.Text.Tests.DynamicModels
{
    /// <summary>A model with complex types.</summary>
    public class ModelWithComplexTypes
    {
        /// <summary>A nested type.</summary>
        public class NestedType
        {
            /// <summary>Gets or sets the int value.</summary>
            /// <value>The int value.</value>
            public int IntValue { get; set; }
        }

        /// <summary>Bitfield of flags for specifying MyEnum.</summary>
        [Flags]
        public enum MyEnum
        {
            /// <summary>A binary constant representing the value flag.</summary>
            Value = 12
        }

        /// <summary>Gets or sets the list value.</summary>
        /// <value>The list value.</value>
        public IList<string> ListValue { get; set; }

        /// <summary>Gets or sets the dictionary value.</summary>
        /// <value>The dictionary value.</value>
        public IDictionary<string, string> DictionaryValue { get; set; }

        /// <summary>Gets or sets the array value.</summary>
        /// <value>The array value.</value>
        public string[] ArrayValue { get; set; }

        /// <summary>Gets or sets the byte array value.</summary>
        /// <value>The byte array value.</value>
        public byte[] ByteArrayValue { get; set; }

        /// <summary>Gets or sets the enum value.</summary>
        /// <value>The enum value.</value>
        public MyEnum? EnumValue { get; set; }

        /// <summary>Gets or sets the nested type value.</summary>
        /// <value>The nested type value.</value>
        public NestedType NestedTypeValue { get; set; }

        /// <summary>Creates a new ModelWithComplexTypes.</summary>
        /// <param name="i">Zero-based index of the.</param>
        /// <returns>The ModelWithComplexTypes.</returns>
        public static ModelWithComplexTypes Create(int i)
        {
            return new ModelWithComplexTypes
                       {
                           DictionaryValue = new Dictionary<string, string> {{"a", i.ToString()}},
                           ListValue = new List<string> {i.ToString()},
                           ArrayValue = new string[]{},
                           EnumValue = MyEnum.Value,
                           ByteArrayValue = new byte[]{(byte)i},
                       };
        }
    }
}