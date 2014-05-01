
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
using System.Net;
using System.Text;
using System.Reflection;
using NServiceKit.Text.Common;
using NServiceKit.Text.Json;

namespace NServiceKit.Text
{
    /// <summary>Creates an instance of a Type from a string value.</summary>
	public static class JsonSerializer
	{
        /// <summary>The UTF 8 encoding without bom.</summary>
		private static readonly UTF8Encoding UTF8EncodingWithoutBom = new UTF8Encoding(false);

        /// <summary>Deserialize from string.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="value">The value.</param>
        /// <returns>A T.</returns>
		public static T DeserializeFromString<T>(string value)
		{
			if (string.IsNullOrEmpty(value)) return default(T);
			return (T)JsonReader<T>.Parse(value);
		}

        /// <summary>Deserialize from reader.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="reader">The reader.</param>
        /// <returns>A T.</returns>
		public static T DeserializeFromReader<T>(TextReader reader)
		{
			return DeserializeFromString<T>(reader.ReadToEnd());
		}

        /// <summary>Deserialize from string.</summary>
        /// <param name="value">The value.</param>
        /// <param name="type"> The type.</param>
        /// <returns>An object.</returns>
		public static object DeserializeFromString(string value, Type type)
		{
			return string.IsNullOrEmpty(value)
					? null
					: JsonReader.GetParseFn(type)(value);
		}

        /// <summary>Deserialize from reader.</summary>
        /// <param name="reader">The reader.</param>
        /// <param name="type">  The type.</param>
        /// <returns>An object.</returns>
		public static object DeserializeFromReader(TextReader reader, Type type)
		{
			return DeserializeFromString(reader.ReadToEnd(), type);
		}

        /// <summary>Serialize to string.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="value">The value.</param>
        /// <returns>A string.</returns>
		public static string SerializeToString<T>(T value)
		{
            if (value == null || value is Delegate) return null;
            if (typeof(T) == typeof(object) || typeof(T).IsAbstract() || typeof(T).IsInterface())
            {
                if (typeof(T).IsAbstract() || typeof(T).IsInterface()) JsState.IsWritingDynamic = true;
                var result = SerializeToString(value, value.GetType());
                if (typeof(T).IsAbstract() || typeof(T).IsInterface()) JsState.IsWritingDynamic = false;
                return result;
            }

			var sb = new StringBuilder();
			using (var writer = new StringWriter(sb, CultureInfo.InvariantCulture))
			{
				if (typeof(T) == typeof(string))
				{
					JsonUtils.WriteString(writer, value as string);
				}
				else
				{
					JsonWriter<T>.WriteRootObject(writer, value);
				}
			}
			return sb.ToString();
		}

        /// <summary>Serialize to string.</summary>
        /// <param name="value">The value.</param>
        /// <param name="type"> The type.</param>
        /// <returns>A string.</returns>
		public static string SerializeToString(object value, Type type)
		{
			if (value == null) return null;

			var sb = new StringBuilder();
			using (var writer = new StringWriter(sb, CultureInfo.InvariantCulture))
			{
				if (type == typeof(string))
				{
					JsonUtils.WriteString(writer, value as string);
				}
				else
				{
					JsonWriter.GetWriteFn(type)(writer, value);
				}
			}
			return sb.ToString();
		}

        /// <summary>Serialize to writer.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="value"> The value.</param>
        /// <param name="writer">The writer.</param>
		public static void SerializeToWriter<T>(T value, TextWriter writer)
		{
			if (value == null) return;
			if (typeof(T) == typeof(string))
			{
				writer.Write(value);
				return;
			}
            if (typeof(T) == typeof(object) || typeof(T).IsAbstract() || typeof(T).IsInterface())
            {
                if (typeof(T).IsAbstract() || typeof(T).IsInterface()) JsState.IsWritingDynamic = true;
                SerializeToWriter(value, value.GetType(), writer);
                if (typeof(T).IsAbstract() || typeof(T).IsInterface()) JsState.IsWritingDynamic = false;
                return;
            }

			JsonWriter<T>.WriteRootObject(writer, value);
		}

        /// <summary>Serialize to writer.</summary>
        /// <param name="value"> The value.</param>
        /// <param name="type">  The type.</param>
        /// <param name="writer">The writer.</param>
		public static void SerializeToWriter(object value, Type type, TextWriter writer)
		{
			if (value == null) return;
			if (type == typeof(string))
			{
				writer.Write(value);
				return;
			}

			JsonWriter.GetWriteFn(type)(writer, value);
		}

        /// <summary>Serialize to stream.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="value"> The value.</param>
        /// <param name="stream">The stream.</param>
		public static void SerializeToStream<T>(T value, Stream stream)
		{
			if (value == null) return;
            if (typeof(T) == typeof(object) || typeof(T).IsAbstract() || typeof(T).IsInterface())
            {
                if (typeof(T).IsAbstract() || typeof(T).IsInterface()) JsState.IsWritingDynamic = true;
                SerializeToStream(value, value.GetType(), stream);
                if (typeof(T).IsAbstract() || typeof(T).IsInterface()) JsState.IsWritingDynamic = false;
                return;
            }

			var writer = new StreamWriter(stream, UTF8EncodingWithoutBom);
			JsonWriter<T>.WriteRootObject(writer, value);
			writer.Flush();
		}

        /// <summary>Serialize to stream.</summary>
        /// <param name="value"> The value.</param>
        /// <param name="type">  The type.</param>
        /// <param name="stream">The stream.</param>
		public static void SerializeToStream(object value, Type type, Stream stream)
		{
			var writer = new StreamWriter(stream, UTF8EncodingWithoutBom);
			JsonWriter.GetWriteFn(type)(writer, value);
			writer.Flush();
		}

        /// <summary>Deserialize from stream.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="stream">The stream.</param>
        /// <returns>A T.</returns>
		public static T DeserializeFromStream<T>(Stream stream)
		{
			using (var reader = new StreamReader(stream, UTF8EncodingWithoutBom))
			{
				return DeserializeFromString<T>(reader.ReadToEnd());
			}
		}

        /// <summary>Deserialize from stream.</summary>
        /// <param name="type">  The type.</param>
        /// <param name="stream">The stream.</param>
        /// <returns>An object.</returns>
		public static object DeserializeFromStream(Type type, Stream stream)
		{
			using (var reader = new StreamReader(stream, UTF8EncodingWithoutBom))
			{
				return DeserializeFromString(reader.ReadToEnd(), type);
			}
		}

#if !WINDOWS_PHONE && !SILVERLIGHT
        /// <summary>Deserialize response.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="webRequest">The web request.</param>
        /// <returns>A T.</returns>
		public static T DeserializeResponse<T>(WebRequest webRequest)
		{
#if NETFX_CORE
            var async = webRequest.GetResponseAsync();
            async.Wait();

            var webRes = async.Result;
            using (var stream = webRes.GetResponseStream())
            {
                return DeserializeFromStream<T>(stream);
            }
#else
            using (var webRes = webRequest.GetResponse())
            {
                using (var stream = webRes.GetResponseStream())
                {
                    return DeserializeFromStream<T>(stream);
                }
            }
#endif
		}

        /// <summary>Deserialize response.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="type">      The type.</param>
        /// <param name="webRequest">The web request.</param>
        /// <returns>A T.</returns>
		public static object DeserializeResponse<T>(Type type, WebRequest webRequest)
		{
#if NETFX_CORE
            var async = webRequest.GetResponseAsync();
            async.Wait();

            var webRes = async.Result;
            using (var stream = webRes.GetResponseStream())
            {
                return DeserializeFromStream(type, stream);
            }
#else
			using (var webRes = webRequest.GetResponse())
			{
				using (var stream = webRes.GetResponseStream())
				{
					return DeserializeFromStream(type, stream);
				}
			}
#endif
		}

        /// <summary>Deserialize request.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="webRequest">The web request.</param>
        /// <returns>A T.</returns>
		public static T DeserializeRequest<T>(WebRequest webRequest)
		{
#if NETFX_CORE
            var async = webRequest.GetResponseAsync();
            async.Wait();

            var webRes = async.Result;
			return DeserializeResponse<T>(webRes);
#else
			using (var webRes = webRequest.GetResponse())
			{
				return DeserializeResponse<T>(webRes);
            }
#endif
		}

        /// <summary>Deserialize request.</summary>
        /// <param name="type">      The type.</param>
        /// <param name="webRequest">The web request.</param>
        /// <returns>An object.</returns>
		public static object DeserializeRequest(Type type, WebRequest webRequest)
		{
#if NETFX_CORE
            var async = webRequest.GetResponseAsync();
            async.Wait();

            var webRes = async.Result;
			return DeserializeResponse(type, webRes);
#else
			using (var webRes = webRequest.GetResponse())
			{
				return DeserializeResponse(type, webRes);
			}
#endif
		}
#endif

        /// <summary>Deserialize response.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="webResponse">The web response.</param>
        /// <returns>A T.</returns>
		public static T DeserializeResponse<T>(WebResponse webResponse)
		{
			using (var stream = webResponse.GetResponseStream())
			{
				return DeserializeFromStream<T>(stream);
			}
		}

        /// <summary>Deserialize response.</summary>
        /// <param name="type">       The type.</param>
        /// <param name="webResponse">The web response.</param>
        /// <returns>An object.</returns>
		public static object DeserializeResponse(Type type, WebResponse webResponse)
		{
			using (var stream = webResponse.GetResponseStream())
			{
				return DeserializeFromStream(type, stream);
			}
		}

	}
}