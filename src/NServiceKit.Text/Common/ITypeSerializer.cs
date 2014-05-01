using System;
using System.IO;
using NServiceKit.Text.Json;

namespace NServiceKit.Text.Common
{
    /// <summary>Interface for type serializer.</summary>
    internal interface ITypeSerializer
    {
        /// <summary>Gets a value indicating whether the null values should be included.</summary>
        /// <value>true if include null values, false if not.</value>
        bool IncludeNullValues { get; }

        /// <summary>Gets the type attribute in object.</summary>
        /// <value>The type attribute in object.</value>
        string TypeAttrInObject { get; }

        /// <summary>Gets write function.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <returns>The write function.</returns>
        WriteObjectDelegate GetWriteFn<T>();

        /// <summary>Gets write function.</summary>
        /// <param name="type">The type.</param>
        /// <returns>The write function.</returns>
        WriteObjectDelegate GetWriteFn(Type type);

        /// <summary>Gets type information.</summary>
        /// <param name="type">The type.</param>
        /// <returns>The type information.</returns>
        TypeInfo GetTypeInfo(Type type);

        /// <summary>Writes a raw string.</summary>
        /// <param name="writer">The writer.</param>
        /// <param name="value"> The value.</param>
        void WriteRawString(TextWriter writer, string value);

        /// <summary>Writes a property name.</summary>
        /// <param name="writer">The writer.</param>
        /// <param name="value"> The value.</param>
        void WritePropertyName(TextWriter writer, string value);

        /// <summary>Writes a built in.</summary>
        /// <param name="writer">The writer.</param>
        /// <param name="value"> The value.</param>
        void WriteBuiltIn(TextWriter writer, object value);

        /// <summary>Writes an object string.</summary>
        /// <param name="writer">The writer.</param>
        /// <param name="value"> The value.</param>
        void WriteObjectString(TextWriter writer, object value);

        /// <summary>Writes an exception.</summary>
        /// <param name="writer">The writer.</param>
        /// <param name="value"> The value.</param>
        void WriteException(TextWriter writer, object value);

        /// <summary>Writes a string.</summary>
        /// <param name="writer">The writer.</param>
        /// <param name="value"> The value.</param>
        void WriteString(TextWriter writer, string value);

        /// <summary>Writes a formattable object string.</summary>
        /// <param name="writer">The writer.</param>
        /// <param name="value"> The value.</param>
        void WriteFormattableObjectString(TextWriter writer, object value);

        /// <summary>Writes a date time.</summary>
        /// <param name="writer">   The writer.</param>
        /// <param name="oDateTime">The date time.</param>
        void WriteDateTime(TextWriter writer, object oDateTime);

        /// <summary>Writes a nullable date time.</summary>
        /// <param name="writer">  The writer.</param>
        /// <param name="dateTime">The date time.</param>
        void WriteNullableDateTime(TextWriter writer, object dateTime);

        /// <summary>Writes a date time offset.</summary>
        /// <param name="writer">         The writer.</param>
        /// <param name="oDateTimeOffset">The date time offset.</param>
        void WriteDateTimeOffset(TextWriter writer, object oDateTimeOffset);

        /// <summary>Writes a nullable date time offset.</summary>
        /// <param name="writer">        The writer.</param>
        /// <param name="dateTimeOffset">The date time offset.</param>
        void WriteNullableDateTimeOffset(TextWriter writer, object dateTimeOffset);

        /// <summary>Writes a time span.</summary>
        /// <param name="writer">        The writer.</param>
        /// <param name="dateTimeOffset">The date time offset.</param>
        void WriteTimeSpan(TextWriter writer, object dateTimeOffset);

        /// <summary>Writes a nullable time span.</summary>
        /// <param name="writer">        The writer.</param>
        /// <param name="dateTimeOffset">The date time offset.</param>
        void WriteNullableTimeSpan(TextWriter writer, object dateTimeOffset);

        /// <summary>Writes a unique identifier.</summary>
        /// <param name="writer">The writer.</param>
        /// <param name="oValue">The value.</param>
        void WriteGuid(TextWriter writer, object oValue);

        /// <summary>Writes a nullable unique identifier.</summary>
        /// <param name="writer">The writer.</param>
        /// <param name="oValue">The value.</param>
        void WriteNullableGuid(TextWriter writer, object oValue);

        /// <summary>Writes the bytes.</summary>
        /// <param name="writer">    The writer.</param>
        /// <param name="oByteValue">The byte value.</param>
        void WriteBytes(TextWriter writer, object oByteValue);

        /// <summary>Writes a character.</summary>
        /// <param name="writer">   The writer.</param>
        /// <param name="charValue">The character value.</param>
        void WriteChar(TextWriter writer, object charValue);

        /// <summary>Writes a byte.</summary>
        /// <param name="writer">   The writer.</param>
        /// <param name="byteValue">The byte value.</param>
        void WriteByte(TextWriter writer, object byteValue);

        /// <summary>Writes an int 16.</summary>
        /// <param name="writer">  The writer.</param>
        /// <param name="intValue">The int value.</param>
        void WriteInt16(TextWriter writer, object intValue);

        /// <summary>Writes an u int 16.</summary>
        /// <param name="writer">  The writer.</param>
        /// <param name="intValue">The int value.</param>
        void WriteUInt16(TextWriter writer, object intValue);

        /// <summary>Writes an int 32.</summary>
        /// <param name="writer">  The writer.</param>
        /// <param name="intValue">The int value.</param>
        void WriteInt32(TextWriter writer, object intValue);

        /// <summary>Writes an u int 32.</summary>
        /// <param name="writer">   The writer.</param>
        /// <param name="uintValue">The value.</param>
        void WriteUInt32(TextWriter writer, object uintValue);

        /// <summary>Writes an int 64.</summary>
        /// <param name="writer">   The writer.</param>
        /// <param name="longValue">The long value.</param>
        void WriteInt64(TextWriter writer, object longValue);

        /// <summary>Writes an u int 64.</summary>
        /// <param name="writer">    The writer.</param>
        /// <param name="ulongValue">The ulong value.</param>
        void WriteUInt64(TextWriter writer, object ulongValue);

        /// <summary>Writes a bool.</summary>
        /// <param name="writer">   The writer.</param>
        /// <param name="boolValue">The value.</param>
        void WriteBool(TextWriter writer, object boolValue);

        /// <summary>Writes a float.</summary>
        /// <param name="writer">    The writer.</param>
        /// <param name="floatValue">The float value.</param>
        void WriteFloat(TextWriter writer, object floatValue);

        /// <summary>Writes a double.</summary>
        /// <param name="writer">     The writer.</param>
        /// <param name="doubleValue">The double value.</param>
        void WriteDouble(TextWriter writer, object doubleValue);

        /// <summary>Writes a decimal.</summary>
        /// <param name="writer">      The writer.</param>
        /// <param name="decimalValue">The decimal value.</param>
        void WriteDecimal(TextWriter writer, object decimalValue);

        /// <summary>Writes an enum.</summary>
        /// <param name="writer">   The writer.</param>
        /// <param name="enumValue">The enum value.</param>
        void WriteEnum(TextWriter writer, object enumValue);

        /// <summary>Writes an enum flags.</summary>
        /// <param name="writer">       The writer.</param>
        /// <param name="enumFlagValue">The enum flag value.</param>
        void WriteEnumFlags(TextWriter writer, object enumFlagValue);

        /// <summary>Writes a linq binary.</summary>
        /// <param name="writer">         The writer.</param>
        /// <param name="linqBinaryValue">The linq binary value.</param>
        void WriteLinqBinary(TextWriter writer, object linqBinaryValue);

        //object EncodeMapKey(object value);

        /// <summary>Gets parse function.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <returns>The parse function.</returns>
        ParseStringDelegate GetParseFn<T>();

        /// <summary>Gets parse function.</summary>
        /// <param name="type">The type.</param>
        /// <returns>The parse function.</returns>
        ParseStringDelegate GetParseFn(Type type);

        /// <summary>Parse raw string.</summary>
        /// <param name="value">The value.</param>
        /// <returns>A string.</returns>
        string ParseRawString(string value);

        /// <summary>Parse string.</summary>
        /// <param name="value">The value.</param>
        /// <returns>A string.</returns>
        string ParseString(string value);

        /// <summary>Unescape string.</summary>
        /// <param name="value">The value.</param>
        /// <returns>A string.</returns>
        string UnescapeString(string value);

        /// <summary>Unescape safe string.</summary>
        /// <param name="value">The value.</param>
        /// <returns>A string.</returns>
        string UnescapeSafeString(string value);

        /// <summary>Eat type value.</summary>
        /// <param name="value">The value.</param>
        /// <param name="i">    Zero-based index of the.</param>
        /// <returns>A string.</returns>
        string EatTypeValue(string value, ref int i);

        /// <summary>Eat map start character.</summary>
        /// <param name="value">The value.</param>
        /// <param name="i">    Zero-based index of the.</param>
        /// <returns>true if it succeeds, false if it fails.</returns>
        bool EatMapStartChar(string value, ref int i);

        /// <summary>Eat map key.</summary>
        /// <param name="value">The value.</param>
        /// <param name="i">    Zero-based index of the.</param>
        /// <returns>A string.</returns>
        string EatMapKey(string value, ref int i);

        /// <summary>Eat map key seperator.</summary>
        /// <param name="value">The value.</param>
        /// <param name="i">    Zero-based index of the.</param>
        /// <returns>true if it succeeds, false if it fails.</returns>
        bool EatMapKeySeperator(string value, ref int i);

        /// <summary>Eat whitespace.</summary>
        /// <param name="value">The value.</param>
        /// <param name="i">    Zero-based index of the.</param>
        void EatWhitespace(string value, ref int i);

        /// <summary>Eat value.</summary>
        /// <param name="value">The value.</param>
        /// <param name="i">    Zero-based index of the.</param>
        /// <returns>A string.</returns>
        string EatValue(string value, ref int i);

        /// <summary>Eat item seperator map end character.</summary>
        /// <param name="value">The value.</param>
        /// <param name="i">    Zero-based index of the.</param>
        /// <returns>true if it succeeds, false if it fails.</returns>
        bool EatItemSeperatorOrMapEndChar(string value, ref int i);
    }
}