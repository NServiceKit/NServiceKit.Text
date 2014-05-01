using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using NServiceKit.Text.Common;
using NServiceKit.Text.Json;

namespace NServiceKit.Text
{
    /// <summary>A JSON extensions.</summary>
	public static class JsonExtensions
	{
        /// <summary>A Dictionary&lt;string,string&gt; extension method that JSON to.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="map">The map to act on.</param>
        /// <param name="key">The key.</param>
        /// <returns>A T.</returns>
		public static T JsonTo<T>(this Dictionary<string, string> map, string key)
		{
			return Get<T>(map, key);
		}

        /// <summary>Get JSON string value converted to T.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="map">The map to act on.</param>
        /// <param name="key">The key.</param>
        /// <returns>A T.</returns>
        public static T Get<T>(this Dictionary<string, string> map, string key)
		{
			string strVal;
			return map.TryGetValue(key, out strVal) ? JsonSerializer.DeserializeFromString<T>(strVal) : default(T);
		}

        /// <summary>Get JSON string value.</summary>
        /// <param name="map">The map to act on.</param>
        /// <param name="key">The key.</param>
        /// <returns>A string.</returns>
        public static string Get(this Dictionary<string, string> map, string key)
		{
			string strVal;
            return map.TryGetValue(key, out strVal) ? JsonTypeSerializer.Instance.UnescapeString(strVal) : null;
		}

        /// <summary>A string extension method that array objects.</summary>
        /// <param name="json">The JSON to act on.</param>
        /// <returns>The JsonArrayObjects.</returns>
		public static JsonArrayObjects ArrayObjects(this string json)
		{
			return Text.JsonArrayObjects.Parse(json);
		}

        /// <summary>The JsonArrayObjects extension method that convert all.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="jsonArrayObjects">The jsonArrayObjects to act on.</param>
        /// <param name="converter">       The converter.</param>
        /// <returns>all converted.</returns>
		public static List<T> ConvertAll<T>(this JsonArrayObjects jsonArrayObjects, Func<JsonObject, T> converter)
		{
			var results = new List<T>();

			foreach (var jsonObject in jsonArrayObjects)
			{
				results.Add(converter(jsonObject));
			}

			return results;
		}

        /// <summary>A JsonObject extension method that convert to.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="jsonObject">The jsonObject to act on.</param>
        /// <param name="converFn">  The conver function.</param>
        /// <returns>to converted.</returns>
		public static T ConvertTo<T>(this JsonObject jsonObject, Func<JsonObject, T> converFn)
		{
			return jsonObject == null 
				? default(T) 
				: converFn(jsonObject);
		}

        /// <summary>A JsonObject extension method that converts a jsonObject to a dictionary.</summary>
        /// <param name="jsonObject">The jsonObject to act on.</param>
        /// <returns>jsonObject as a Dictionary&lt;string,string&gt;</returns>
		public static Dictionary<string, string> ToDictionary(this JsonObject jsonObject)
		{
			return jsonObject == null 
				? new Dictionary<string, string>() 
				: new Dictionary<string, string>(jsonObject);
		}
	}

    /// <summary>A JSON object.</summary>
	public class JsonObject : Dictionary<string, string>
	{
        /// <summary>Get JSON string value.</summary>
        /// <param name="key">The key.</param>
        /// <returns>The indexed item.</returns>
        public new string this[string key]
        {
            get { return this.Get(key); }
            set { base[key] = value; }
        }

        /// <summary>Parses.</summary>
        /// <param name="json">The JSON.</param>
        /// <returns>A JsonObject.</returns>
        public static JsonObject Parse(string json)
        {
            return JsonSerializer.DeserializeFromString<JsonObject>(json);
        }

        /// <summary>Parse array.</summary>
        /// <param name="json">The JSON.</param>
        /// <returns>The JsonArrayObjects.</returns>
        public static JsonArrayObjects ParseArray(string json)
        {
            return JsonArrayObjects.Parse(json);
        }

        /// <summary>Array objects.</summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>The JsonArrayObjects.</returns>
		public JsonArrayObjects ArrayObjects(string propertyName)
		{
			string strValue;
			return this.TryGetValue(propertyName, out strValue)
				? JsonArrayObjects.Parse(strValue)
				: null;
		}

        /// <summary>Objects.</summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>A JsonObject.</returns>
		public JsonObject Object(string propertyName)
		{
			string strValue;
			return this.TryGetValue(propertyName, out strValue)
				? Parse(strValue)
				: null;
		}

        /// <summary>Get unescaped string value.</summary>
        /// <param name="key">The key.</param>
        /// <returns>The unescaped.</returns>
        public string GetUnescaped(string key)
        {
            return base[key];
        }

        /// <summary>Get unescaped string value.</summary>
        /// <param name="key">The key.</param>
        /// <returns>A string.</returns>
        public string Child(string key)
        {
            return base[key];
        }
#if !SILVERLIGHT && !MONOTOUCH
        /// <summary>Number of register exes.</summary>
        static readonly Regex NumberRegEx = new Regex(@"^[0-9]*(?:\.[0-9]*)?$", RegexOptions.Compiled);
#else
        /// <summary>Number of register exes.</summary>
        static readonly Regex NumberRegEx = new Regex(@"^[0-9]*(?:\.[0-9]*)?$");
#endif

        /// <summary>Write JSON Array, Object, bool or number values as raw string.</summary>
        /// <param name="writer">The writer.</param>
        /// <param name="value"> The value.</param>
        public static void WriteValue(TextWriter writer, object value)
        {
            var strValue = value as string;
            if (!string.IsNullOrEmpty(strValue))
            {
                var firstChar = strValue[0];
                var lastChar = strValue[strValue.Length - 1];
                if ((firstChar == JsWriter.MapStartChar && lastChar == JsWriter.MapEndChar)
                    || (firstChar == JsWriter.ListStartChar && lastChar == JsWriter.ListEndChar) 
                    || JsonUtils.True == strValue
                    || JsonUtils.False == strValue
                    || NumberRegEx.IsMatch(strValue))
                {
                    writer.Write(strValue);
                    return;
                }
            }
            JsonUtils.WriteString(writer, strValue);
        }
    }

    /// <summary>A JSON array objects.</summary>
	public class JsonArrayObjects : List<JsonObject>
	{
        /// <summary>Parses.</summary>
        /// <param name="json">The JSON.</param>
        /// <returns>The JsonArrayObjects.</returns>
		public static JsonArrayObjects Parse(string json)
		{
			return JsonSerializer.DeserializeFromString<JsonArrayObjects>(json);
		}
	}

    /// <summary>A JSON value.</summary>
    public struct JsonValue
    {
        /// <summary>The JSON.</summary>
        private readonly string json;

        /// <summary>Initializes a new instance of the JsonObject class.</summary>
        /// <param name="json">The JSON.</param>
        public JsonValue(string json)
        {
            this.json = json;
        }

        /// <summary>Gets as.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <returns>A T.</returns>
        public T As<T>()
        {
            return JsonSerializer.DeserializeFromString<T>(json);
        }

        /// <summary>Returns the fully qualified type name of this instance.</summary>
        /// <returns>A <see cref="T:System.String" /> containing a fully qualified type name.</returns>
        public override string ToString()
        {
            return json;
        }
    }

}