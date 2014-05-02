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
using System.Collections.Generic;
using System.Text;
using NServiceKit.Text.Common;

namespace NServiceKit.Text
{
    /// <summary>A text extensions.</summary>
	public static class TextExtensions
	{
        /// <summary>An object extension method that converts a text to a CSV field.</summary>
        /// <param name="text">The text to act on.</param>
        /// <returns>text as an object.</returns>
        public static string ToCsvField(this string text)
        {
            return string.IsNullOrEmpty(text) || !CsvWriter.HasAnyEscapeChars(text)
                       ? text
                       : string.Concat
                             (
                                 CsvConfig.ItemDelimiterString,
                                 text.Replace(CsvConfig.ItemDelimiterString, CsvConfig.EscapedItemDelimiterString),
                                 CsvConfig.ItemDelimiterString
                             );
        }

        /// <summary>An object extension method that converts a text to a CSV field.</summary>
        /// <param name="text">The text to act on.</param>
        /// <returns>text as an object.</returns>
        public static object ToCsvField(this object text)
        {
            return text == null || !JsWriter.HasAnyEscapeChars(text.ToString())
                       ? text
                       : string.Concat
                             (
                                 JsWriter.QuoteString,
                                 text.ToString().Replace(JsWriter.QuoteString, TypeSerializer.DoubleQuoteString),
                                 JsWriter.QuoteString
                             );
        }

        /// <summary>
        /// A string extension method that initializes this object from the given from CSV field.
        /// </summary>
        /// <param name="text">The text to act on.</param>
        /// <returns>A string.</returns>
	    public static string FromCsvField(this string text)
		{
            return string.IsNullOrEmpty(text) || !text.StartsWith(CsvConfig.ItemDelimiterString, StringComparison.Ordinal)
			       	? text
					: text.Substring(CsvConfig.ItemDelimiterString.Length, text.Length - CsvConfig.EscapedItemDelimiterString.Length)
						.Replace(CsvConfig.EscapedItemDelimiterString, CsvConfig.ItemDelimiterString);
		}

        /// <summary>Initializes this object from the given from CSV fields.</summary>
        /// <param name="texts">A variable-length parameters list containing texts.</param>
        /// <returns>A string[].</returns>
		public static List<string> FromCsvFields(this IEnumerable<string> texts)
		{
			var safeTexts = new List<string>();
			foreach (var text in texts)
			{
				safeTexts.Add(FromCsvField(text));
			}
			return safeTexts;
		}

        /// <summary>Initializes this object from the given from CSV fields.</summary>
        /// <param name="texts">A variable-length parameters list containing texts.</param>
        /// <returns>A string[].</returns>
		public static string[] FromCsvFields(params string[] texts)
		{
			var textsLen = texts.Length;
			var safeTexts = new string[textsLen];
			for (var i = 0; i < textsLen; i++)
			{
				safeTexts[i] = FromCsvField(texts[i]);
			}
			return safeTexts;
		}

        /// <summary>A T extension method that serialize to string.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="value">The value to act on.</param>
        /// <returns>A string.</returns>
		public static string SerializeToString<T>(this T value)
		{
			return JsonSerializer.SerializeToString(value);
		}
	}
}