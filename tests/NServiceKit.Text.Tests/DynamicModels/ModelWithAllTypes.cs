using System;
using NServiceKit.Text.Tests.DynamicModels.DataModel;

namespace NServiceKit.Text.Tests.DynamicModels
{
    /// <summary>A model with all types.</summary>
	public class ModelWithAllTypes
	{
        /// <summary>Gets or sets the exception.</summary>
        /// <value>The exception.</value>
		public Exception Exception { get; set; }

        /// <summary>Gets or sets details of the exception.</summary>
        /// <value>The custom exception.</value>
		public CustomException CustomException { get; set; }

        /// <summary>Gets or sets the URI value.</summary>
        /// <value>The URI value.</value>
		public Uri UriValue { get; set; }

        /// <summary>Gets or sets the type value.</summary>
        /// <value>The type value.</value>
		public Type TypeValue { get; set; }

        /// <summary>Gets or sets the character value.</summary>
        /// <value>The character value.</value>
		public char CharValue { get; set; }

        /// <summary>Gets or sets the byte value.</summary>
        /// <value>The byte value.</value>
		public byte ByteValue { get; set; }

        /// <summary>Gets or sets the byte value.</summary>
        /// <value>The s byte value.</value>
		public sbyte SByteValue { get; set; }

        /// <summary>Gets or sets the short value.</summary>
        /// <value>The short value.</value>
		public short ShortValue { get; set; }

        /// <summary>Gets or sets the short value.</summary>
        /// <value>The u short value.</value>
		public ushort UShortValue { get; set; }

        /// <summary>Gets or sets the int value.</summary>
        /// <value>The int value.</value>
		public int IntValue { get; set; }

        /// <summary>Gets or sets the int value.</summary>
        /// <value>The u int value.</value>
		public uint UIntValue { get; set; }

        /// <summary>Gets or sets the long value.</summary>
        /// <value>The long value.</value>
		public long LongValue { get; set; }

        /// <summary>Gets or sets the long value.</summary>
        /// <value>The u long value.</value>
		public ulong ULongValue { get; set; }

        /// <summary>Gets or sets the float value.</summary>
        /// <value>The float value.</value>
		public float FloatValue { get; set; }

        /// <summary>Gets or sets the double value.</summary>
        /// <value>The double value.</value>
		public double DoubleValue { get; set; }

        /// <summary>Gets or sets the decimal value.</summary>
        /// <value>The decimal value.</value>
		public decimal DecimalValue { get; set; }

        /// <summary>Gets or sets the Date/Time of the date time value.</summary>
        /// <value>The date time value.</value>
		public DateTime DateTimeValue { get; set; }

        /// <summary>Gets or sets the time span value.</summary>
        /// <value>The time span value.</value>
		public TimeSpan TimeSpanValue { get; set; }

        /// <summary>Gets or sets the unique identifier value.</summary>
        /// <value>The unique identifier value.</value>
		public Guid GuidValue { get; set; }

        /// <summary>Creates a new ModelWithAllTypes.</summary>
        /// <param name="i">Zero-based index of the.</param>
        /// <returns>The ModelWithAllTypes.</returns>
		public static ModelWithAllTypes Create(byte i)
		{
			return new ModelWithAllTypes
			{
				ByteValue = i,
				CharValue = (char)i,
				CustomException = new CustomException("CustomException " + i),
				DateTimeValue = new DateTime(2000, 1, 1 + i),
				DecimalValue = i,
				DoubleValue = i,
				Exception = new Exception("Exception " + i),
				FloatValue = i,
				IntValue = i,
				LongValue = i,
				SByteValue = (sbyte)i,
				ShortValue = i,
				TimeSpanValue = new TimeSpan(i),
				TypeValue = typeof(ModelWithAllTypes),
				UIntValue = i,
				ULongValue = i,
				UriValue = new Uri("http://domain.com/" + i),
				UShortValue = i,
				GuidValue = Guid.NewGuid(),
			};
		}

	}
}