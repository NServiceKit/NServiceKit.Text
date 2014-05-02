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

using System.Collections.Generic;
using System.IO;

namespace NServiceKit.Text
{
    /// <summary>A CSV stream extensions.</summary>
	public static class CsvStreamExtensions
	{
        /// <summary>A TextWriter extension method that writes a CSV.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="outputStream">The outputStream to act on.</param>
        /// <param name="records">     The records.</param>
		public static void WriteCsv<T>(this Stream outputStream, IEnumerable<T> records)
		{
			using (var textWriter = new StreamWriter(outputStream))
			{
				textWriter.WriteCsv(records);
			}
		}

        /// <summary>A TextWriter extension method that writes a CSV.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="writer"> The writer to act on.</param>
        /// <param name="records">The records.</param>
		public static void WriteCsv<T>(this TextWriter writer, IEnumerable<T> records)
		{
			CsvWriter<T>.Write(writer, records);
		}

	}
}