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
using System.IO;
using System.Text;
using NServiceKit.Text.Jsv;

namespace NServiceKit.Text
{
    /// <summary>A type serializer.</summary>
    /// <typeparam name="T">Generic type parameter.</typeparam>
	public class TypeSerializer<T> : ITypeSerializer<T>
	{
        /// <summary>Determine if we can create from string.</summary>
        /// <param name="type">The type.</param>
        /// <returns>true if we can create from string, false if not.</returns>
		public bool CanCreateFromString(Type type)
		{
			return JsvReader.GetParseFn(type) != null;
		}

        /// <summary>Parses the specified value.</summary>
        /// <param name="value">The value.</param>
        /// <returns>A T.</returns>
		public T DeserializeFromString(string value)
		{
			if (string.IsNullOrEmpty(value)) return default(T);
			return (T)JsvReader<T>.Parse(value);
		}

        /// <summary>Deserialize from reader.</summary>
        /// <param name="reader">The reader.</param>
        /// <returns>A T.</returns>
		public T DeserializeFromReader(TextReader reader)
		{
			return DeserializeFromString(reader.ReadToEnd());
		}

        /// <summary>Serialize to string.</summary>
        /// <param name="value">The value.</param>
        /// <returns>A string.</returns>
		public string SerializeToString(T value)
		{
			if (value == null) return null;
			if (typeof(T) == typeof(string)) return value as string;

			var sb = new StringBuilder();
			using (var writer = new StringWriter(sb))
			{
				JsvWriter<T>.WriteObject(writer, value);
			}
			return sb.ToString();
		}

        /// <summary>Serialize to writer.</summary>
        /// <param name="value"> The value.</param>
        /// <param name="writer">The writer.</param>
		public void SerializeToWriter(T value, TextWriter writer)
		{
			if (value == null) return;
			if (typeof(T) == typeof(string))
			{
				writer.Write(value);
				return;
			}

			JsvWriter<T>.WriteObject(writer, value);
		}
	}
}