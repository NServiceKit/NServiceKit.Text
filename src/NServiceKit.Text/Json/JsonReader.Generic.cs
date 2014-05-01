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
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using NServiceKit.Text.Common;

namespace NServiceKit.Text.Json
{
    /// <summary>A JSON reader.</summary>
	internal static class JsonReader
	{
        /// <summary>The instance.</summary>
		public static readonly JsReader<JsonTypeSerializer> Instance = new JsReader<JsonTypeSerializer>();

        /// <summary>The parse function cache.</summary>
		private static Dictionary<Type, ParseFactoryDelegate> ParseFnCache = new Dictionary<Type, ParseFactoryDelegate>();

        /// <summary>Gets parse function.</summary>
        /// <param name="type">The type.</param>
        /// <returns>The parse function.</returns>
		public static ParseStringDelegate GetParseFn(Type type)
		{
			ParseFactoryDelegate parseFactoryFn;
            ParseFnCache.TryGetValue(type, out parseFactoryFn);

            if (parseFactoryFn != null) return parseFactoryFn();

            var genericType = typeof(JsonReader<>).MakeGenericType(type);
            var mi = genericType.GetPublicStaticMethod("GetParseFn");
            parseFactoryFn = (ParseFactoryDelegate)mi.MakeDelegate(typeof(ParseFactoryDelegate));

            Dictionary<Type, ParseFactoryDelegate> snapshot, newCache;
            do
            {
                snapshot = ParseFnCache;
                newCache = new Dictionary<Type, ParseFactoryDelegate>(ParseFnCache);
                newCache[type] = parseFactoryFn;

            } while (!ReferenceEquals(
                Interlocked.CompareExchange(ref ParseFnCache, newCache, snapshot), snapshot));
            
            return parseFactoryFn();
		}
	}

    /// <summary>A JSON reader.</summary>
    /// <typeparam name="T">Generic type parameter.</typeparam>
	public static class JsonReader<T>
	{
        /// <summary>The read function.</summary>
		private static readonly ParseStringDelegate ReadFn;

        /// <summary>
        /// Initializes static members of the NServiceKit.Text.Json.JsonReader&lt;T&gt; class.
        /// </summary>
		static JsonReader()
		{
			ReadFn = JsonReader.Instance.GetParseFn<T>();
		}

        /// <summary>Gets parse function.</summary>
        /// <returns>The parse function.</returns>
		public static ParseStringDelegate GetParseFn()
		{
			return ReadFn ?? Parse;
		}

        /// <summary>Parses.</summary>
        /// <exception cref="NotSupportedException">Thrown when the requested operation is not supported.</exception>
        /// <param name="value">The value.</param>
        /// <returns>An object.</returns>
		public static object Parse(string value)
		{
			if (ReadFn == null)
			{
                if (typeof(T).IsAbstract() || typeof(T).IsInterface())
                {
					if (string.IsNullOrEmpty(value)) return null;
					var concreteType = DeserializeType<JsonTypeSerializer>.ExtractType(value);
					if (concreteType != null)
					{
						return JsonReader.GetParseFn(concreteType)(value);
					}
					throw new NotSupportedException("Can not deserialize interface type: "
						+ typeof(T).Name);
				}
			}
			return value == null 
			       	? null 
			       	: ReadFn(value);
		}
	}
}