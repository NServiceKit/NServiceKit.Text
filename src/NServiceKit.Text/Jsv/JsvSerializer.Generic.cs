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
using System.IO;
using System.Text;
using System.Threading;
using System.Reflection;
using System.Linq;
using NServiceKit.Text.Common;

namespace NServiceKit.Text.Jsv
{
    /// <summary>A jsv serializer.</summary>
    /// <typeparam name="T">Generic type parameter.</typeparam>
	public class JsvSerializer<T>
	{
        /// <summary>The deserializer cache.</summary>
		Dictionary<Type, ParseStringDelegate> DeserializerCache = new Dictionary<Type, ParseStringDelegate>();

        /// <summary>Deserialize from string.</summary>
        /// <param name="value">The value.</param>
        /// <param name="type"> The type.</param>
        /// <returns>A T.</returns>
		public T DeserializeFromString(string value, Type type)
		{
			ParseStringDelegate parseFn;
            if (DeserializerCache.TryGetValue(type, out parseFn)) return (T)parseFn(value);

            var genericType = typeof(T).MakeGenericType(type);
            var mi = genericType.GetMethodInfo("DeserializeFromString", new[] { typeof(string) });
            parseFn = (ParseStringDelegate)mi.MakeDelegate(typeof(ParseStringDelegate));

            Dictionary<Type, ParseStringDelegate> snapshot, newCache;
            do
            {
                snapshot = DeserializerCache;
                newCache = new Dictionary<Type, ParseStringDelegate>(DeserializerCache);
                newCache[type] = parseFn;

            } while (!ReferenceEquals(
                Interlocked.CompareExchange(ref DeserializerCache, newCache, snapshot), snapshot));
            
            return (T)parseFn(value);
		}

        /// <summary>Deserialize from string.</summary>
        /// <param name="value">The value.</param>
        /// <returns>A T.</returns>
		public T DeserializeFromString(string value)
		{
			if (typeof(T) == typeof(string)) return (T)(object)value;

			return (T)JsvReader<T>.Parse(value);
		}

        /// <summary>Serialize to writer.</summary>
        /// <param name="value"> The value.</param>
        /// <param name="writer">The writer.</param>
		public void SerializeToWriter(T value, TextWriter writer)
		{
			JsvWriter<T>.WriteObject(writer, value);
		}

        /// <summary>Serialize to string.</summary>
        /// <param name="value">The value.</param>
        /// <returns>A string.</returns>
		public string SerializeToString(T value)
		{
			if (value == null) return null;
			if (value is string) return value as string;

			var sb = new StringBuilder();
			using (var writer = new StringWriter(sb))
			{
				JsvWriter<T>.WriteObject(writer, value);
			}
			return sb.ToString();
		}
	}
}