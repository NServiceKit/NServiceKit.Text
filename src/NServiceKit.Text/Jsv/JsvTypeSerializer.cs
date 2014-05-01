//
// https://github.com/NServiceKit/NServiceKit.Text
// NServiceKit.Text: .NET C# POCO JSON, JSV and CSV Text Serializers.
//
// Authors:
//   Demis Bellot (demis.bellot@gmail.com)
//
// Copyright 2012 ServiceStack Ltd.
//
// Licensed under the same terms of ServiceStack: new BSD license.
//

using System;
using System.Globalization;
using System.IO;
using NServiceKit.Text.Common;
using NServiceKit.Text.Json;

namespace NServiceKit.Text.Jsv
{
    /// <summary>A jsv type serializer.</summary>
	internal class JsvTypeSerializer
		: ITypeSerializer
	{
        /// <summary>The instance.</summary>
		public static ITypeSerializer Instance = new JsvTypeSerializer();

        /// <summary>Gets a value indicating whether the null values should be included.</summary>
        /// <value>true if include null values, false if not.</value>
	    public bool IncludeNullValues
	    {
            get { return false; } //Doesn't support null values, treated as "null" string literal
	    }

        /// <summary>Gets the type attribute in object.</summary>
        /// <value>The type attribute in object.</value>
        public string TypeAttrInObject
        {
            get { return JsConfig.JsvTypeAttrInObject; }
        }

        /// <summary>Gets type attribute in object.</summary>
        /// <param name="typeAttr">The type attribute.</param>
        /// <returns>The type attribute in object.</returns>
        internal static string GetTypeAttrInObject(string typeAttr)
        {
            return string.Format("{{{0}:", typeAttr);
        }

        /// <summary>Gets write function.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <returns>The write function.</returns>
		public WriteObjectDelegate GetWriteFn<T>()
		{
			return JsvWriter<T>.WriteFn();
		}

        /// <summary>Gets write function.</summary>
        /// <param name="type">The type.</param>
        /// <returns>The write function.</returns>
		public WriteObjectDelegate GetWriteFn(Type type)
		{
			return JsvWriter.GetWriteFn(type);
		}

        /// <summary>The default type information.</summary>
		static readonly TypeInfo DefaultTypeInfo = new TypeInfo { EncodeMapKey = false };

        /// <summary>Gets type information.</summary>
        /// <param name="type">The type.</param>
        /// <returns>The type information.</returns>
		public TypeInfo GetTypeInfo(Type type)
		{
			return DefaultTypeInfo;
		}

        /// <summary>Writes a raw string.</summary>
        /// <param name="writer">The writer.</param>
        /// <param name="value"> The value.</param>
		public void WriteRawString(TextWriter writer, string value)
		{
			writer.Write(value.EncodeJsv());
		}

        /// <summary>Writes a property name.</summary>
        /// <param name="writer">The writer.</param>
        /// <param name="value"> The value.</param>
		public void WritePropertyName(TextWriter writer, string value)
		{
			writer.Write(value);
		}

        /// <summary>Writes a built in.</summary>
        /// <param name="writer">The writer.</param>
        /// <param name="value"> The value.</param>
		public void WriteBuiltIn(TextWriter writer, object value)
		{
			writer.Write(value);
		}

        /// <summary>Writes an object string.</summary>
        /// <param name="writer">The writer.</param>
        /// <param name="value"> The value.</param>
		public void WriteObjectString(TextWriter writer, object value)
		{
			if (value != null)
			{
                if(value is string)
                    WriteString(writer, value as string);
                else
				    writer.Write(value.ToString().EncodeJsv());
			}
		}

        /// <summary>Writes an exception.</summary>
        /// <param name="writer">The writer.</param>
        /// <param name="value"> The value.</param>
		public void WriteException(TextWriter writer, object value)
		{
			writer.Write(((Exception)value).Message.EncodeJsv());
		}

        /// <summary>Writes a string.</summary>
        /// <param name="writer">The writer.</param>
        /// <param name="value"> The value.</param>
		public void WriteString(TextWriter writer, string value)
		{
            if(JsState.QueryStringMode && !string.IsNullOrEmpty(value) && value.StartsWith(JsWriter.QuoteString) && value.EndsWith(JsWriter.QuoteString))
                value = String.Concat(JsWriter.QuoteChar, value, JsWriter.QuoteChar);
		    else if (JsState.QueryStringMode && !string.IsNullOrEmpty(value) && value.Contains(JsWriter.ItemSeperatorString))
		        value = String.Concat(JsWriter.QuoteChar, value, JsWriter.QuoteChar);
            
			writer.Write(value.EncodeJsv());
		}

        /// <summary>Writes a formattable object string.</summary>
        /// <param name="writer">The writer.</param>
        /// <param name="value"> The value.</param>
	    public void WriteFormattableObjectString(TextWriter writer, object value)
	    {
	        var f = (IFormattable)value;
	        writer.Write(f.ToString(null,CultureInfo.InvariantCulture).EncodeJsv());
	    }

        /// <summary>Writes a date time.</summary>
        /// <param name="writer">   The writer.</param>
        /// <param name="oDateTime">The date time.</param>
	    public void WriteDateTime(TextWriter writer, object oDateTime)
		{
			writer.Write(DateTimeSerializer.ToShortestXsdDateTimeString((DateTime)oDateTime));
		}

        /// <summary>Writes a nullable date time.</summary>
        /// <param name="writer">  The writer.</param>
        /// <param name="dateTime">The date time.</param>
		public void WriteNullableDateTime(TextWriter writer, object dateTime)
		{
			if (dateTime == null) return;
			writer.Write(DateTimeSerializer.ToShortestXsdDateTimeString((DateTime)dateTime));
		}

        /// <summary>Writes a date time offset.</summary>
        /// <param name="writer">         The writer.</param>
        /// <param name="oDateTimeOffset">The date time offset.</param>
		public void WriteDateTimeOffset(TextWriter writer, object oDateTimeOffset)
		{
			writer.Write(((DateTimeOffset) oDateTimeOffset).ToString("o"));
		}

        /// <summary>Writes a nullable date time offset.</summary>
        /// <param name="writer">        The writer.</param>
        /// <param name="dateTimeOffset">The date time offset.</param>
		public void WriteNullableDateTimeOffset(TextWriter writer, object dateTimeOffset)
		{
			if (dateTimeOffset == null) return;
			this.WriteDateTimeOffset(writer, dateTimeOffset);
		}

        /// <summary>Writes a time span.</summary>
        /// <param name="writer">            The writer.</param>
        /// <param name="oTimeSpan">         The date time offset.</param>
        /// ### <param name="dateTimeOffset">The date time offset.</param>
        public void WriteTimeSpan(TextWriter writer, object oTimeSpan)
        {
            writer.Write(DateTimeSerializer.ToXsdTimeSpanString((TimeSpan)oTimeSpan));
        }

        /// <summary>Writes a nullable time span.</summary>
        /// <param name="writer">            The writer.</param>
        /// <param name="oTimeSpan">         The date time offset.</param>
        /// ### <param name="dateTimeOffset">The date time offset.</param>
        public void WriteNullableTimeSpan(TextWriter writer, object oTimeSpan)
        {
            if (oTimeSpan == null) return;
            writer.Write(DateTimeSerializer.ToXsdTimeSpanString((TimeSpan?)oTimeSpan));
        }

        /// <summary>Writes a unique identifier.</summary>
        /// <param name="writer">The writer.</param>
        /// <param name="oValue">The value.</param>
		public void WriteGuid(TextWriter writer, object oValue)
		{
			writer.Write(((Guid)oValue).ToString("N"));
		}

        /// <summary>Writes a nullable unique identifier.</summary>
        /// <param name="writer">The writer.</param>
        /// <param name="oValue">The value.</param>
		public void WriteNullableGuid(TextWriter writer, object oValue)
		{
			if (oValue == null) return;
			writer.Write(((Guid)oValue).ToString("N"));
		}

        /// <summary>Writes the bytes.</summary>
        /// <param name="writer">    The writer.</param>
        /// <param name="oByteValue">The byte value.</param>
		public void WriteBytes(TextWriter writer, object oByteValue)
		{
			if (oByteValue == null) return;
			writer.Write(Convert.ToBase64String((byte[])oByteValue));
		}

        /// <summary>Writes a character.</summary>
        /// <param name="writer">   The writer.</param>
        /// <param name="charValue">The character value.</param>
		public void WriteChar(TextWriter writer, object charValue)
		{
			if (charValue == null) return;
			writer.Write((char)charValue);
		}

        /// <summary>Writes a byte.</summary>
        /// <param name="writer">   The writer.</param>
        /// <param name="byteValue">The byte value.</param>
		public void WriteByte(TextWriter writer, object byteValue)
		{
			if (byteValue == null) return;
			writer.Write((byte)byteValue);
		}

        /// <summary>Writes an int 16.</summary>
        /// <param name="writer">  The writer.</param>
        /// <param name="intValue">The int value.</param>
		public void WriteInt16(TextWriter writer, object intValue)
		{
			if (intValue == null) return;
			writer.Write((short)intValue);
		}

        /// <summary>Writes an u int 16.</summary>
        /// <param name="writer">  The writer.</param>
        /// <param name="intValue">The int value.</param>
		public void WriteUInt16(TextWriter writer, object intValue)
		{
			if (intValue == null) return;
			writer.Write((ushort)intValue);
		}

        /// <summary>Writes an int 32.</summary>
        /// <param name="writer">  The writer.</param>
        /// <param name="intValue">The int value.</param>
		public void WriteInt32(TextWriter writer, object intValue)
		{
			if (intValue == null) return;
			writer.Write((int)intValue);
		}

        /// <summary>Writes an u int 32.</summary>
        /// <param name="writer">   The writer.</param>
        /// <param name="uintValue">The value.</param>
		public void WriteUInt32(TextWriter writer, object uintValue)
		{
			if (uintValue == null) return;
			writer.Write((uint)uintValue);
		}

        /// <summary>Writes an u int 64.</summary>
        /// <param name="writer">    The writer.</param>
        /// <param name="ulongValue">The ulong value.</param>
		public void WriteUInt64(TextWriter writer, object ulongValue)
		{
			if (ulongValue == null) return;
			writer.Write((ulong)ulongValue);
		}

        /// <summary>Writes an int 64.</summary>
        /// <param name="writer">   The writer.</param>
        /// <param name="longValue">The long value.</param>
		public void WriteInt64(TextWriter writer, object longValue)
		{
			if (longValue == null) return;
			writer.Write((long)longValue);
		}

        /// <summary>Writes a bool.</summary>
        /// <param name="writer">   The writer.</param>
        /// <param name="boolValue">The value.</param>
		public void WriteBool(TextWriter writer, object boolValue)
		{
			if (boolValue == null) return;
			writer.Write((bool)boolValue);
		}

        /// <summary>Writes a float.</summary>
        /// <param name="writer">    The writer.</param>
        /// <param name="floatValue">The float value.</param>
		public void WriteFloat(TextWriter writer, object floatValue)
		{
			if (floatValue == null) return;
			var floatVal = (float)floatValue;
			if (Equals(floatVal, float.MaxValue) || Equals(floatVal, float.MinValue))
				writer.Write(floatVal.ToString("r", CultureInfo.InvariantCulture));
			else
				writer.Write(floatVal.ToString(CultureInfo.InvariantCulture));
		}

        /// <summary>Writes a double.</summary>
        /// <param name="writer">     The writer.</param>
        /// <param name="doubleValue">The double value.</param>
		public void WriteDouble(TextWriter writer, object doubleValue)
		{
			if (doubleValue == null) return;
			var doubleVal = (double)doubleValue;
			if (Equals(doubleVal, double.MaxValue) || Equals(doubleVal, double.MinValue))
				writer.Write(doubleVal.ToString("r", CultureInfo.InvariantCulture));
			else
				writer.Write(doubleVal.ToString(CultureInfo.InvariantCulture));
		}

        /// <summary>Writes a decimal.</summary>
        /// <param name="writer">      The writer.</param>
        /// <param name="decimalValue">The decimal value.</param>
		public void WriteDecimal(TextWriter writer, object decimalValue)
		{
			if (decimalValue == null) return;
			writer.Write(((decimal)decimalValue).ToString(CultureInfo.InvariantCulture));
		}

        /// <summary>Writes an enum.</summary>
        /// <param name="writer">   The writer.</param>
        /// <param name="enumValue">The enum value.</param>
		public void WriteEnum(TextWriter writer, object enumValue)
		{
			if (enumValue == null) return;
			if (JsConfig.TreatEnumAsInteger)
				JsWriter.WriteEnumFlags(writer, enumValue);
			else
				writer.Write(enumValue.ToString());
		}

        /// <summary>Writes an enum flags.</summary>
        /// <param name="writer">       The writer.</param>
        /// <param name="enumFlagValue">The enum flag value.</param>
        public void WriteEnumFlags(TextWriter writer, object enumFlagValue)
        {
			JsWriter.WriteEnumFlags(writer, enumFlagValue);
        }

        /// <summary>Writes a linq binary.</summary>
        /// <param name="writer">         The writer.</param>
        /// <param name="linqBinaryValue">The linq binary value.</param>
		public void WriteLinqBinary(TextWriter writer, object linqBinaryValue)
        {
#if !MONOTOUCH && !SILVERLIGHT && !XBOX  && !ANDROID
			WriteRawString(writer, Convert.ToBase64String(((System.Data.Linq.Binary)linqBinaryValue).ToArray()));
#endif
        }

        /// <summary>Encode map key.</summary>
        /// <param name="value">The value.</param>
        /// <returns>An object.</returns>
		public object EncodeMapKey(object value)
		{
			return value;
		}

        /// <summary>Gets parse function.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <returns>The parse function.</returns>
		public ParseStringDelegate GetParseFn<T>()
		{
			return JsvReader.Instance.GetParseFn<T>();
		}

        /// <summary>Gets parse function.</summary>
        /// <param name="type">The type.</param>
        /// <returns>The parse function.</returns>
		public ParseStringDelegate GetParseFn(Type type)
		{
			return JsvReader.GetParseFn(type);
		}

        /// <summary>Unescape safe string.</summary>
        /// <param name="value">The value.</param>
        /// <returns>A string.</returns>
        public string UnescapeSafeString(string value)
        {
            return value.FromCsvField();
        }

        /// <summary>Parse raw string.</summary>
        /// <param name="value">The value.</param>
        /// <returns>A string.</returns>
		public string ParseRawString(string value)
		{
			return value;
		}

        /// <summary>Parse string.</summary>
        /// <param name="value">The value.</param>
        /// <returns>A string.</returns>
		public string ParseString(string value)
		{
			return value.FromCsvField();
		}

        /// <summary>Unescape string.</summary>
        /// <param name="value">The value.</param>
        /// <returns>A string.</returns>
	    public string UnescapeString(string value)
	    {
            return value.FromCsvField();
        }

        /// <summary>Eat type value.</summary>
        /// <param name="value">The value.</param>
        /// <param name="i">    Zero-based index of the.</param>
        /// <returns>A string.</returns>
	    public string EatTypeValue(string value, ref int i)
		{
			return EatValue(value, ref i);
		}

        /// <summary>Eat map start character.</summary>
        /// <param name="value">The value.</param>
        /// <param name="i">    Zero-based index of the.</param>
        /// <returns>true if it succeeds, false if it fails.</returns>
		public bool EatMapStartChar(string value, ref int i)
		{
			var success = value[i] == JsWriter.MapStartChar;
			if (success) i++;
			return success;
		}

        /// <summary>Eat map key.</summary>
        /// <param name="value">The value.</param>
        /// <param name="i">    Zero-based index of the.</param>
        /// <returns>A string.</returns>
		public string EatMapKey(string value, ref int i)
		{
			var tokenStartPos = i;

			var valueLength = value.Length;

			var valueChar = value[tokenStartPos];

			switch (valueChar)
			{
				case JsWriter.QuoteChar:
					while (++i < valueLength)
					{
						valueChar = value[i];

						if (valueChar != JsWriter.QuoteChar) continue;

						var isLiteralQuote = i + 1 < valueLength && value[i + 1] == JsWriter.QuoteChar;

						i++; //skip quote
						if (!isLiteralQuote)
							break;
					}
					return value.Substring(tokenStartPos, i - tokenStartPos);

				//Is Type/Map, i.e. {...}
				case JsWriter.MapStartChar:
					var endsToEat = 1;
					var withinQuotes = false;
					while (++i < valueLength && endsToEat > 0)
					{
						valueChar = value[i];

						if (valueChar == JsWriter.QuoteChar)
							withinQuotes = !withinQuotes;

						if (withinQuotes)
							continue;

						if (valueChar == JsWriter.MapStartChar)
							endsToEat++;

						if (valueChar == JsWriter.MapEndChar)
							endsToEat--;
					}
					return value.Substring(tokenStartPos, i - tokenStartPos);
			}

			while (value[++i] != JsWriter.MapKeySeperator) { }
			return value.Substring(tokenStartPos, i - tokenStartPos);
		}

        /// <summary>Eat map key seperator.</summary>
        /// <param name="value">The value.</param>
        /// <param name="i">    Zero-based index of the.</param>
        /// <returns>true if it succeeds, false if it fails.</returns>
		public bool EatMapKeySeperator(string value, ref int i)
		{
			return value[i++] == JsWriter.MapKeySeperator;
		}

        /// <summary>Eat item seperator map end character.</summary>
        /// <param name="value">The value.</param>
        /// <param name="i">    Zero-based index of the.</param>
        /// <returns>true if it succeeds, false if it fails.</returns>
		public bool EatItemSeperatorOrMapEndChar(string value, ref int i)
		{
			if (i == value.Length) return false;

			var success = value[i] == JsWriter.ItemSeperator
				|| value[i] == JsWriter.MapEndChar;
			i++;
			return success;
		}

        /// <summary>Eat whitespace.</summary>
        /// <param name="value">The value.</param>
        /// <param name="i">    Zero-based index of the.</param>
        public void EatWhitespace(string value, ref int i)
        {
        }

        /// <summary>Eat value.</summary>
        /// <param name="value">The value.</param>
        /// <param name="i">    Zero-based index of the.</param>
        /// <returns>A string.</returns>
		public string EatValue(string value, ref int i)
		{
			var tokenStartPos = i;
			var valueLength = value.Length;
			if (i == valueLength) return null;

			var valueChar = value[i];
			var withinQuotes = false;
			var endsToEat = 1;

			switch (valueChar)
			{
				//If we are at the end, return.
				case JsWriter.ItemSeperator:
				case JsWriter.MapEndChar:
					return null;

				//Is Within Quotes, i.e. "..."
				case JsWriter.QuoteChar:
					while (++i < valueLength)
					{
						valueChar = value[i];

						if (valueChar != JsWriter.QuoteChar) continue;

						var isLiteralQuote = i + 1 < valueLength && value[i + 1] == JsWriter.QuoteChar;

						i++; //skip quote
						if (!isLiteralQuote)
							break;
					}
					return value.Substring(tokenStartPos, i - tokenStartPos);

				//Is Type/Map, i.e. {...}
				case JsWriter.MapStartChar:
					while (++i < valueLength && endsToEat > 0)
					{
						valueChar = value[i];

						if (valueChar == JsWriter.QuoteChar)
							withinQuotes = !withinQuotes;

						if (withinQuotes)
							continue;

						if (valueChar == JsWriter.MapStartChar)
							endsToEat++;

						if (valueChar == JsWriter.MapEndChar)
							endsToEat--;
					}
					return value.Substring(tokenStartPos, i - tokenStartPos);

				//Is List, i.e. [...]
				case JsWriter.ListStartChar:
					while (++i < valueLength && endsToEat > 0)
					{
						valueChar = value[i];

						if (valueChar == JsWriter.QuoteChar)
							withinQuotes = !withinQuotes;

						if (withinQuotes)
							continue;

						if (valueChar == JsWriter.ListStartChar)
							endsToEat++;

						if (valueChar == JsWriter.ListEndChar)
							endsToEat--;
					}
					return value.Substring(tokenStartPos, i - tokenStartPos);
			}

			//Is Value
			while (++i < valueLength)
			{
				valueChar = value[i];

				if (valueChar == JsWriter.ItemSeperator
					|| valueChar == JsWriter.MapEndChar)
				{
					break;
				}
			}

			return value.Substring(tokenStartPos, i - tokenStartPos);
		}
	}
}