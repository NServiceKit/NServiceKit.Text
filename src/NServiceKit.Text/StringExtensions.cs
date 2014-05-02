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
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using NServiceKit.Text.Common;
using NServiceKit.Text.Support;
#if NETFX_CORE
using System.Threading.Tasks;
#endif

#if WINDOWS_PHONE
using System.IO.IsolatedStorage;
#if  !WP8
using NServiceKit.Text.WP;
#endif
#endif

namespace NServiceKit.Text
{
    /// <summary>A string extensions.</summary>
    public static class StringExtensions
    {
        /// <summary>A string extension method that toes.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="value">The value to act on.</param>
        /// <returns>A T.</returns>
        public static T To<T>(this string value)
        {
            return TypeSerializer.DeserializeFromString<T>(value);
        }

        /// <summary>A string extension method that toes.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="value">       The value to act on.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>A T.</returns>
        public static T To<T>(this string value, T defaultValue)
        {
            return String.IsNullOrEmpty(value) ? defaultValue : TypeSerializer.DeserializeFromString<T>(value);
        }

        /// <summary>A string extension method that converts a value to an or default value.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="value">The value to act on.</param>
        /// <returns>value as a T.</returns>
        public static T ToOrDefaultValue<T>(this string value)
        {
            return String.IsNullOrEmpty(value) ? default(T) : TypeSerializer.DeserializeFromString<T>(value);
        }

        /// <summary>A string extension method that toes.</summary>
        /// <param name="value">The value to act on.</param>
        /// <param name="type"> The type to act on.</param>
        /// <returns>An object.</returns>
        public static object To(this string value, Type type)
        {
            return TypeSerializer.DeserializeFromString(value, type);
        }

        /// <summary>Converts from base: 0 - 62.</summary>
        /// <param name="source">The source.</param>
        /// <param name="from">  From.</param>
        /// <param name="to">    To.</param>
        /// <returns>A string.</returns>
        public static string BaseConvert(this string source, int from, int to)
        {
            const string chars = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var result = "";
            var length = source.Length;
            var number = new int[length];

            for (var i = 0; i < length; i++)
            {
                number[i] = chars.IndexOf(source[i]);
            }

            int newlen;

            do
            {
                var divide = 0;
                newlen = 0;

                for (var i = 0; i < length; i++)
                {
                    divide = divide * @from + number[i];

                    if (divide >= to)
                    {
                        number[newlen++] = divide / to;
                        divide = divide % to;
                    }
                    else if (newlen > 0)
                    {
                        number[newlen++] = 0;
                    }
                }

                length = newlen;
                result = chars[divide] + result;
            }
            while (newlen != 0);

            return result;
        }

        /// <summary>A string extension method that encode XML.</summary>
        /// <param name="value">The value to act on.</param>
        /// <returns>A string.</returns>
        public static string EncodeXml(this string value)
        {
            return value.Replace("<", "&lt;").Replace(">", "&gt;").Replace("&", "&amp;");
        }

        /// <summary>A string extension method that encode JSON.</summary>
        /// <param name="value">The value to act on.</param>
        /// <returns>A string.</returns>
        public static string EncodeJson(this string value)
        {
            return String.Concat
            ("\"",
                value.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\r", "").Replace("\n", "\\n"),
                "\""
            );
        }

        /// <summary>A string extension method that encode jsv.</summary>
        /// <param name="value">The value to act on.</param>
        /// <returns>A string.</returns>
        public static string EncodeJsv(this string value)
        {
            if (JsState.QueryStringMode)
            {
                return UrlEncode(value);
            }
            return String.IsNullOrEmpty(value) || !JsWriter.HasAnyEscapeChars(value)
                ? value
                : String.Concat
                    (
                        JsWriter.QuoteString,
                        value.Replace(JsWriter.QuoteString, TypeSerializer.DoubleQuoteString),
                        JsWriter.QuoteString
                    );
        }

        /// <summary>A string extension method that decode jsv.</summary>
        /// <param name="value">The value to act on.</param>
        /// <returns>A string.</returns>
        public static string DecodeJsv(this string value)
        {
            const int startingQuotePos = 1;
            const int endingQuotePos = 2;
            return String.IsNullOrEmpty(value) || value[0] != JsWriter.QuoteChar
                    ? value
                    : value.Substring(startingQuotePos, value.Length - endingQuotePos)
                        .Replace(TypeSerializer.DoubleQuoteString, JsWriter.QuoteString);
        }

        /// <summary>A string extension method that URL encode.</summary>
        /// <param name="text">The text to act on.</param>
        /// <returns>A string.</returns>
        public static string UrlEncode(this string text)
        {
            if (String.IsNullOrEmpty(text)) return text;

            var sb = new StringBuilder();

            foreach (var charCode in Encoding.UTF8.GetBytes(text))
            {

                if (
                    charCode >= 65 && charCode <= 90        // A-Z
                    || charCode >= 97 && charCode <= 122    // a-z
                    || charCode >= 48 && charCode <= 57     // 0-9
                    || charCode >= 44 && charCode <= 46     // ,-.
                    )
                {
                    sb.Append((char)charCode);
                }
                else
                {
                    sb.Append('%' + charCode.ToString("x2"));
                }
            }

            return sb.ToString();
        }

        /// <summary>A string extension method that URL decode.</summary>
        /// <param name="text">The text to act on.</param>
        /// <returns>A string.</returns>
        public static string UrlDecode(this string text)
        {
            if (String.IsNullOrEmpty(text)) return null;

            var bytes = new List<byte>();

            var textLength = text.Length;
            for (var i = 0; i < textLength; i++)
            {
                var c = text[i];
                if (c == '+')
                {
                    bytes.Add(32);
                }
                else if (c == '%')
                {
                    var hexNo = Convert.ToByte(text.Substring(i + 1, 2), 16);
                    bytes.Add(hexNo);
                    i += 2;
                }
                else
                {
                    bytes.Add((byte)c);
                }
            }
#if SILVERLIGHT
            byte[] byteArray = bytes.ToArray();
            return Encoding.UTF8.GetString(byteArray, 0, byteArray.Length);
#else
            return Encoding.UTF8.GetString(bytes.ToArray());
#endif
        }

#if !XBOX
        /// <summary>A string extension method that hexadecimal escape.</summary>
        /// <param name="text">     The text to act on.</param>
        /// <param name="anyCharOf">A variable-length parameters list containing any character of.</param>
        /// <returns>A string.</returns>
        public static string HexEscape(this string text, params char[] anyCharOf)
        {
            if (String.IsNullOrEmpty(text)) return text;
            if (anyCharOf == null || anyCharOf.Length == 0) return text;

            var encodeCharMap = new HashSet<char>(anyCharOf);

            var sb = new StringBuilder();
            var textLength = text.Length;
            for (var i = 0; i < textLength; i++)
            {
                var c = text[i];
                if (encodeCharMap.Contains(c))
                {
                    sb.Append('%' + ((int)c).ToString("x"));
                }
                else
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }
#endif

        /// <summary>A string extension method that hexadecimal unescape.</summary>
        /// <param name="text">     The text to act on.</param>
        /// <param name="anyCharOf">A variable-length parameters list containing any character of.</param>
        /// <returns>A string.</returns>
        public static string HexUnescape(this string text, params char[] anyCharOf)
        {
            if (String.IsNullOrEmpty(text)) return null;
            if (anyCharOf == null || anyCharOf.Length == 0) return text;

            var sb = new StringBuilder();

            var textLength = text.Length;
            for (var i = 0; i < textLength; i++)
            {
                var c = text.Substring(i, 1);
                if (c == "%")
                {
                    var hexNo = Convert.ToInt32(text.Substring(i + 1, 2), 16);
                    sb.Append((char)hexNo);
                    i += 2;
                }
                else
                {
                    sb.Append(c);
                }
            }

            return sb.ToString();
        }

        /// <summary>A string extension method that URL format.</summary>
        /// <param name="url">          The URL to act on.</param>
        /// <param name="urlComponents">A variable-length parameters list containing URL components.</param>
        /// <returns>A string.</returns>
        public static string UrlFormat(this string url, params string[] urlComponents)
        {
            var encodedUrlComponents = new string[urlComponents.Length];
            for (var i = 0; i < urlComponents.Length; i++)
            {
                var x = urlComponents[i];
                encodedUrlComponents[i] = x.UrlEncode();
            }

            return String.Format(url, encodedUrlComponents);
        }

        /// <summary>A string extension method that converts a value to a rot 13.</summary>
        /// <param name="value">The value to act on.</param>
        /// <returns>value as a string.</returns>
        public static string ToRot13(this string value)
        {
            var array = value.ToCharArray();
            for (var i = 0; i < array.Length; i++)
            {
                var number = (int)array[i];

                if (number >= 'a' && number <= 'z')
                    number += (number > 'm') ? -13 : 13;

                else if (number >= 'A' && number <= 'Z')
                    number += (number > 'M') ? -13 : 13;

                array[i] = (char)number;
            }
            return new string(array);
        }

        /// <summary>A string extension method that with trailing slash.</summary>
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        /// <param name="path">The path to act on.</param>
        /// <returns>A string.</returns>
        public static string WithTrailingSlash(this string path)
        {
            if (String.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            if (path[path.Length - 1] != '/')
            {
                return path + "/";
            }
            return path;
        }

        /// <summary>A string extension method that appends a path.</summary>
        /// <param name="uri">          The URI to act on.</param>
        /// <param name="uriComponents">A variable-length parameters list containing URI components.</param>
        /// <returns>A string.</returns>
        public static string AppendPath(this string uri, params string[] uriComponents)
        {
            return AppendUrlPaths(uri, uriComponents);
        }

        /// <summary>A string extension method that appends an URL paths.</summary>
        /// <param name="uri">          The URI to act on.</param>
        /// <param name="uriComponents">A variable-length parameters list containing URI components.</param>
        /// <returns>A string.</returns>
        public static string AppendUrlPaths(this string uri, params string[] uriComponents)
        {
            var sb = new StringBuilder(uri.WithTrailingSlash());
            var i = 0;
            foreach (var uriComponent in uriComponents)
            {
                if (i++ > 0) sb.Append('/');
                sb.Append(uriComponent.UrlEncode());
            }
            return sb.ToString();
        }

        /// <summary>A string extension method that appends an URL paths raw.</summary>
        /// <param name="uri">          The URI to act on.</param>
        /// <param name="uriComponents">A variable-length parameters list containing URI components.</param>
        /// <returns>A string.</returns>
        public static string AppendUrlPathsRaw(this string uri, params string[] uriComponents)
        {
            var sb = new StringBuilder(uri.WithTrailingSlash());
            var i = 0;
            foreach (var uriComponent in uriComponents)
            {
                if (i++ > 0) sb.Append('/');
                sb.Append(uriComponent);
            }
            return sb.ToString();
        }

#if !SILVERLIGHT
        /// <summary>
        /// A byte[] extension method that initializes this object from the given from ASCII bytes.
        /// </summary>
        /// <param name="bytes">The bytes to act on.</param>
        /// <returns>A string.</returns>
        public static string FromAsciiBytes(this byte[] bytes)
        {
            return bytes == null ? null
                : Encoding.ASCII.GetString(bytes, 0, bytes.Length);
        }

        /// <summary>A string extension method that converts a value to an ASCII bytes.</summary>
        /// <param name="value">The value to act on.</param>
        /// <returns>value as a byte[].</returns>
        public static byte[] ToAsciiBytes(this string value)
        {
            return Encoding.ASCII.GetBytes(value);
        }
#endif

        /// <summary>
        /// A byte[] extension method that initializes this object from the given from UTF 8 bytes.
        /// </summary>
        /// <param name="bytes">The bytes to act on.</param>
        /// <returns>A string.</returns>
        public static string FromUtf8Bytes(this byte[] bytes)
        {
            return bytes == null ? null
                : Encoding.UTF8.GetString(bytes, 0, bytes.Length);
        }

        /// <summary>A double extension method that converts a doubleVal to an UTF 8 bytes.</summary>
        /// <param name="value">The value to act on.</param>
        /// <returns>doubleVal as a byte[].</returns>
        public static byte[] ToUtf8Bytes(this string value)
        {
            return Encoding.UTF8.GetBytes(value);
        }

        /// <summary>A double extension method that converts a doubleVal to an UTF 8 bytes.</summary>
        /// <param name="intVal">The intVal to act on.</param>
        /// <returns>doubleVal as a byte[].</returns>
        public static byte[] ToUtf8Bytes(this int intVal)
        {
            return FastToUtf8Bytes(intVal.ToString());
        }

        /// <summary>A double extension method that converts a doubleVal to an UTF 8 bytes.</summary>
        /// <param name="longVal">The longVal to act on.</param>
        /// <returns>doubleVal as a byte[].</returns>
        public static byte[] ToUtf8Bytes(this long longVal)
        {
            return FastToUtf8Bytes(longVal.ToString());
        }

        /// <summary>A double extension method that converts a doubleVal to an UTF 8 bytes.</summary>
        /// <param name="doubleVal">The doubleVal to act on.</param>
        /// <returns>doubleVal as a byte[].</returns>
        public static byte[] ToUtf8Bytes(this double doubleVal)
        {
            var doubleStr = doubleVal.ToString(CultureInfo.InvariantCulture.NumberFormat);

            if (doubleStr.IndexOf('E') != -1 || doubleStr.IndexOf('e') != -1)
                doubleStr = DoubleConverter.ToExactString(doubleVal);

            return FastToUtf8Bytes(doubleStr);
        }

        /// <summary>Skip the encoding process for 'safe strings'.</summary>
        /// <param name="strVal">.</param>
        /// <returns>A byte[].</returns>
        private static byte[] FastToUtf8Bytes(string strVal)
        {
            var bytes = new byte[strVal.Length];
            for (var i = 0; i < strVal.Length; i++)
                bytes[i] = (byte)strVal[i];

            return bytes;
        }

        /// <summary>A string extension method that splits on first.</summary>
        /// <param name="strVal">.</param>
        /// <param name="needle">The needle.</param>
        /// <returns>A string[].</returns>
        public static string[] SplitOnFirst(this string strVal, char needle)
        {
            if (strVal == null) return new string[0];
            var pos = strVal.IndexOf(needle);
            return pos == -1
                ? new[] { strVal }
                : new[] { strVal.Substring(0, pos), strVal.Substring(pos + 1) };
        }

        /// <summary>A string extension method that splits on first.</summary>
        /// <param name="strVal">.</param>
        /// <param name="needle">The needle.</param>
        /// <returns>A string[].</returns>
        public static string[] SplitOnFirst(this string strVal, string needle)
        {
            if (strVal == null) return new string[0];
            var pos = strVal.IndexOf(needle);
            return pos == -1
                ? new[] { strVal }
                : new[] { strVal.Substring(0, pos), strVal.Substring(pos + 1) };
        }

        /// <summary>A string extension method that splits on last.</summary>
        /// <param name="strVal">.</param>
        /// <param name="needle">The needle.</param>
        /// <returns>A string[].</returns>
        public static string[] SplitOnLast(this string strVal, char needle)
        {
            if (strVal == null) return new string[0];
            var pos = strVal.LastIndexOf(needle);
            return pos == -1
                ? new[] { strVal }
                : new[] { strVal.Substring(0, pos), strVal.Substring(pos + 1) };
        }

        /// <summary>A string extension method that splits on last.</summary>
        /// <param name="strVal">.</param>
        /// <param name="needle">The needle.</param>
        /// <returns>A string[].</returns>
        public static string[] SplitOnLast(this string strVal, string needle)
        {
            if (strVal == null) return new string[0];
            var pos = strVal.LastIndexOf(needle);
            return pos == -1
                ? new[] { strVal }
                : new[] { strVal.Substring(0, pos), strVal.Substring(pos + 1) };
        }

        /// <summary>A string extension method that without extension.</summary>
        /// <param name="filePath">The filePath to act on.</param>
        /// <returns>A string.</returns>
        public static string WithoutExtension(this string filePath)
        {
            if (String.IsNullOrEmpty(filePath)) return null;

            var extPos = filePath.LastIndexOf('.');
            if (extPos == -1) return filePath;

            var dirPos = filePath.LastIndexOfAny(DirSeps);
            return extPos > dirPos ? filePath.Substring(0, extPos) : filePath;
        }

#if NETFX_CORE
        private static readonly char DirSep = '\\';//Path.DirectorySeparatorChar;
        private static readonly char AltDirSep = '/';//Path.DirectorySeparatorChar == '/' ? '\\' : '/';
#else
        /// <summary>The dir separator.</summary>
        private static readonly char DirSep = Path.DirectorySeparatorChar;

        /// <summary>The path. directory separator character.</summary>
        private static readonly char AltDirSep = Path.DirectorySeparatorChar == '/' ? '\\' : '/';
#endif

        /// <summary>The dir separators.</summary>
        static readonly char[] DirSeps = new[] { '\\', '/' };

        /// <summary>A string extension method that parent directory.</summary>
        /// <param name="filePath">The filePath to act on.</param>
        /// <returns>A string.</returns>
        public static string ParentDirectory(this string filePath)
        {
            if (String.IsNullOrEmpty(filePath)) return null;

            var dirSep = filePath.IndexOf(DirSep) != -1
                         ? DirSep
                         : filePath.IndexOf(AltDirSep) != -1 ? AltDirSep : (char)0;

            return dirSep == 0 ? null : filePath.TrimEnd(dirSep).SplitOnLast(dirSep)[0];
        }

        /// <summary>A T extension method that converts an obj to a jsv.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="obj">The obj to act on.</param>
        /// <returns>obj as a string.</returns>
        public static string ToJsv<T>(this T obj)
        {
            return TypeSerializer.SerializeToString(obj);
        }

        /// <summary>
        /// A string extension method that initializes this object from the given from jsv.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="jsv">The jsv to act on.</param>
        /// <returns>A T.</returns>
        public static T FromJsv<T>(this string jsv)
        {
            return TypeSerializer.DeserializeFromString<T>(jsv);
        }

        /// <summary>A T extension method that converts an obj to a JSON.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="obj">The obj to act on.</param>
        /// <returns>obj as a string.</returns>
        public static string ToJson<T>(this T obj) {
            return JsConfig.PreferInterfaces
                ? JsonSerializer.SerializeToString(obj, AssemblyUtils.MainInterface<T>())
                : JsonSerializer.SerializeToString(obj);
        }

        /// <summary>
        /// A string extension method that initializes this object from the given from JSON.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="json">The JSON to act on.</param>
        /// <returns>A T.</returns>
        public static T FromJson<T>(this string json)
        {
            return JsonSerializer.DeserializeFromString<T>(json);
        }

        /// <summary>A T extension method that converts an obj to a CSV.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="obj">The obj to act on.</param>
        /// <returns>obj as a string.</returns>
        public static string ToCsv<T>(this T obj)
        {
            return CsvSerializer.SerializeToString(obj);
        }

#if !XBOX && !SILVERLIGHT && !MONOTOUCH
        /// <summary>A T extension method that converts an obj to an XML.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="obj">The obj to act on.</param>
        /// <returns>obj as a string.</returns>
        public static string ToXml<T>(this T obj)
        {
            return XmlSerializer.SerializeToString(obj);
        }
#endif

#if !XBOX && !SILVERLIGHT && !MONOTOUCH
        /// <summary>
        /// A string extension method that initializes this object from the given from XML.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="json">The JSON to act on.</param>
        /// <returns>A T.</returns>
        public static T FromXml<T>(this string json)
        {
            return XmlSerializer.DeserializeFromString<T>(json);
        }
#endif

        /// <summary>A string extension method that format with.</summary>
        /// <param name="text">The text to act on.</param>
        /// <param name="args">A variable-length parameters list containing arguments.</param>
        /// <returns>The formatted with.</returns>
        public static string FormatWith(this string text, params object[] args)
        {
            return String.Format(text, args);
        }

        /// <summary>A string extension method that fmts.</summary>
        /// <param name="text">The text to act on.</param>
        /// <param name="args">A variable-length parameters list containing arguments.</param>
        /// <returns>The formatted value.</returns>
        public static string Fmt(this string text, params object[] args)
        {
            return String.Format(text, args);
        }

        /// <summary>A string extension method that starts with ignore case.</summary>
        /// <param name="text">      The text to act on.</param>
        /// <param name="startsWith">The starts with.</param>
        /// <returns>true if it succeeds, false if it fails.</returns>
        public static bool StartsWithIgnoreCase(this string text, string startsWith)
        {
#if NETFX_CORE
            return text != null
                && text.StartsWith(startsWith, StringComparison.CurrentCultureIgnoreCase);
#else
            return text != null
                && text.StartsWith(startsWith, StringComparison.InvariantCultureIgnoreCase);
#endif
        }

        /// <summary>A string extension method that ends with ignore case.</summary>
        /// <param name="text">    The text to act on.</param>
        /// <param name="endsWith">The ends with.</param>
        /// <returns>true if it succeeds, false if it fails.</returns>
        public static bool EndsWithIgnoreCase(this string text, string endsWith)
        {
#if NETFX_CORE
            return text != null
                && text.EndsWith(endsWith, StringComparison.CurrentCultureIgnoreCase);
#else
            return text != null
                && text.EndsWith(endsWith, StringComparison.InvariantCultureIgnoreCase);
#endif
        }

        /// <summary>A string extension method that reads all text.</summary>
        /// <param name="filePath">The filePath to act on.</param>
        /// <returns>all text.</returns>
        public static string ReadAllText(this string filePath)
        {
#if XBOX && !SILVERLIGHT
            using( var fileStream = new FileStream( filePath, FileMode.Open, FileAccess.Read ) )
            {
                return new StreamReader( fileStream ).ReadToEnd( ) ;
            }
#elif NETFX_CORE
            var task = Windows.Storage.StorageFile.GetFileFromPathAsync(filePath);
            task.AsTask().Wait();

            var file = task.GetResults();
            
            var streamTask = file.OpenStreamForReadAsync();
            streamTask.Wait();

            var fileStream = streamTask.Result;

            return new StreamReader( fileStream ).ReadToEnd( ) ;
#elif WINDOWS_PHONE
            using (var isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (var fileStream = isoStore.OpenFile(filePath, FileMode.Open))
                {
                    return new StreamReader(fileStream).ReadToEnd();
                }
            }
#else
            return File.ReadAllText(filePath);
#endif
        }

        /// <summary>
        /// A string extension method that searches for any matches from the given list.
        /// </summary>
        /// <param name="text">   The text to act on.</param>
        /// <param name="needles">A variable-length parameters list containing needles.</param>
        /// <returns>The zero-based index of the found any, or -1 if no match was found.</returns>
        public static int IndexOfAny(this string text, params string[] needles)
        {
            return IndexOfAny(text, 0, needles);
        }

        /// <summary>
        /// A string extension method that searches for any matches from the given list.
        /// </summary>
        /// <param name="text">      The text to act on.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="needles">   A variable-length parameters list containing needles.</param>
        /// <returns>The zero-based index of the found any, or -1 if no match was found.</returns>
        public static int IndexOfAny(this string text, int startIndex, params string[] needles)
        {
            if (text == null) return -1;

            var firstPos = -1;
            foreach (var needle in needles)
            {
                var pos = text.IndexOf(needle);
                if (firstPos == -1 || pos < firstPos) firstPos = pos;
            }
            return firstPos;
        }

        /// <summary>A string extension method that extracts the contents.</summary>
        /// <param name="fromText">  The fromText to act on.</param>
        /// <param name="startAfter">The start after.</param>
        /// <param name="endAt">     The end at.</param>
        /// <returns>The extracted contents.</returns>
        public static string ExtractContents(this string fromText, string startAfter, string endAt)
        {
            return ExtractContents(fromText, startAfter, startAfter, endAt);
        }

        /// <summary>A string extension method that extracts the contents.</summary>
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        /// <param name="fromText">    The fromText to act on.</param>
        /// <param name="uniqueMarker">The unique marker.</param>
        /// <param name="startAfter">  The start after.</param>
        /// <param name="endAt">       The end at.</param>
        /// <returns>The extracted contents.</returns>
        public static string ExtractContents(this string fromText, string uniqueMarker, string startAfter, string endAt)
        {
            if (String.IsNullOrEmpty(uniqueMarker))
                throw new ArgumentNullException("uniqueMarker");
            if (String.IsNullOrEmpty(startAfter))
                throw new ArgumentNullException("startAfter");
            if (String.IsNullOrEmpty(endAt))
                throw new ArgumentNullException("endAt");

            if (String.IsNullOrEmpty(fromText)) return null;

            var markerPos = fromText.IndexOf(uniqueMarker);
            if (markerPos == -1) return null;

            var startPos = fromText.IndexOf(startAfter, markerPos);
            if (startPos == -1) return null;
            startPos += startAfter.Length;

            var endPos = fromText.IndexOf(endAt, startPos);
            if (endPos == -1) endPos = fromText.Length;

            return fromText.Substring(startPos, endPos - startPos);
        }

#if XBOX && !SILVERLIGHT
        static readonly Regex StripHtmlRegEx = new Regex(@"<(.|\n)*?>", RegexOptions.Compiled);
#else
        /// <summary>The strip HTML register ex.</summary>
        static readonly Regex StripHtmlRegEx = new Regex(@"<(.|\n)*?>");
#endif

        /// <summary>A string extension method that strip HTML.</summary>
        /// <param name="html">The HTML to act on.</param>
        /// <returns>A string.</returns>
        public static string StripHtml(this string html)
        {
            return String.IsNullOrEmpty(html) ? null : StripHtmlRegEx.Replace(html, "");
        }

#if XBOX && !SILVERLIGHT
        static readonly Regex StripBracketsRegEx = new Regex(@"\[(.|\n)*?\]", RegexOptions.Compiled);
        static readonly Regex StripBracesRegEx = new Regex(@"\((.|\n)*?\)", RegexOptions.Compiled);
#else
        /// <summary>The strip brackets register ex.</summary>
        static readonly Regex StripBracketsRegEx = new Regex(@"\[(.|\n)*?\]");

        /// <summary>The strip braces register ex.</summary>
        static readonly Regex StripBracesRegEx = new Regex(@"\((.|\n)*?\)");
#endif

        /// <summary>A string extension method that strip markdown markup.</summary>
        /// <param name="markdown">The markdown to act on.</param>
        /// <returns>A string.</returns>
        public static string StripMarkdownMarkup(this string markdown)
        {
            if (String.IsNullOrEmpty(markdown)) return null;
            markdown = StripBracketsRegEx.Replace(markdown, "");
            markdown = StripBracesRegEx.Replace(markdown, "");
            markdown = markdown
                .Replace("*", "")
                .Replace("!", "")
                .Replace("\r", "")
                .Replace("\n", "")
                .Replace("#", "");

            return markdown;
        }

        /// <summary>The lower case offset.</summary>
        private const int LowerCaseOffset = 'a' - 'A';

        /// <summary>A string extension method that converts a value to a camel case.</summary>
        /// <param name="value">The value to act on.</param>
        /// <returns>value as a string.</returns>
        public static string ToCamelCase(this string value)
        {
            if (String.IsNullOrEmpty(value)) return value;

            var len = value.Length;
            var newValue = new char[len];
            var firstPart = true;

            for (var i = 0; i < len; ++i) {
                var c0 = value[i];
                var c1 = i < len - 1 ? value[i + 1] : 'A';
                var c0isUpper = c0 >= 'A' && c0 <= 'Z';
                var c1isUpper = c1 >= 'A' && c1 <= 'Z';

                if (firstPart && c0isUpper && (c1isUpper || i == 0))
                    c0 = (char)(c0 + LowerCaseOffset);
                else
                    firstPart = false;

                newValue[i] = c0;
            }

            return new string(newValue);
        }

        /// <summary>Information describing the text.</summary>
        private static readonly TextInfo TextInfo = CultureInfo.InvariantCulture.TextInfo;

        /// <summary>A string extension method that converts a value to a title case.</summary>
        /// <param name="value">The value to act on.</param>
        /// <returns>value as a string.</returns>
        public static string ToTitleCase(this string value)
        {
#if SILVERLIGHT || __MonoCS__
            string[] words = value.Split('_');

            for (int i = 0; i <= words.Length - 1; i++)
            {
                if ((!object.ReferenceEquals(words[i], string.Empty)))
                {
                    string firstLetter = words[i].Substring(0, 1);
                    string rest = words[i].Substring(1);
                    string result = firstLetter.ToUpper() + rest.ToLower();
                    words[i] = result;
                }
            }
            return String.Join("", words);
#else
            return TextInfo.ToTitleCase(value).Replace("_", String.Empty);
#endif
        }

        /// <summary>A string extension method that converts a value to a lowercase underscore.</summary>
        /// <param name="value">The value to act on.</param>
        /// <returns>value as a string.</returns>
        public static string ToLowercaseUnderscore(this string value)
        {
            if (String.IsNullOrEmpty(value)) return value;
            value = value.ToCamelCase();
            
            var sb = new StringBuilder(value.Length);
            foreach (var t in value)
            {
                if (Char.IsDigit(t) || (Char.IsLetter(t) && Char.IsLower(t)) || t == '_')
                {
                    sb.Append(t);
                }
                else
                {
                    sb.Append("_");
                    sb.Append(Char.ToLowerInvariant(t));
                }
            }
            return sb.ToString();
        }

        /// <summary>A string extension method that safe substring.</summary>
        /// <param name="value"> The value to act on.</param>
        /// <param name="length">The length.</param>
        /// <returns>A string.</returns>
        public static string SafeSubstring(this string value, int length)
        {
            return String.IsNullOrEmpty(value)
                ? String.Empty
                : value.Substring(Math.Min(length, value.Length));
        }

        /// <summary>A string extension method that safe substring.</summary>
        /// <param name="value">     The value to act on.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="length">    The length.</param>
        /// <returns>A string.</returns>
        public static string SafeSubstring(this string value, int startIndex, int length)
        {
            if (String.IsNullOrEmpty(value)) return String.Empty;
            if (value.Length >= (startIndex + length))
                return value.Substring(startIndex, length);

            return value.Length > startIndex ? value.Substring(startIndex) : String.Empty;
        }

        /// <summary>A Type extension method that query if 'type' is anonymous type.</summary>
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        /// <param name="type">The type to act on.</param>
        /// <returns>true if anonymous type, false if not.</returns>
        public static bool IsAnonymousType(this Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            // HACK: The only way to detect anonymous types right now.
#if NETFX_CORE
            return type.IsGeneric() && type.Name.Contains("AnonymousType")
                && (type.Name.StartsWith("<>", StringComparison.Ordinal) || type.Name.StartsWith("VB$", StringComparison.Ordinal));
#else
            return Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute), false)
                && type.IsGeneric() && type.Name.Contains("AnonymousType")
                && (type.Name.StartsWith("<>", StringComparison.Ordinal) || type.Name.StartsWith("VB$", StringComparison.Ordinal))
                && (type.Attributes & TypeAttributes.NotPublic) == TypeAttributes.NotPublic;
#endif
        }

        /// <summary>A string extension method that compare ignore case.</summary>
        /// <param name="strA">The strA to act on.</param>
        /// <param name="strB">The b.</param>
        /// <returns>An int.</returns>
        public static int CompareIgnoreCase(this string strA, string strB)
        {
            return String.Compare(strA, strB, InvariantComparisonIgnoreCase());
        }

        /// <summary>A string extension method that ends with invariant.</summary>
        /// <param name="str">     The str to act on.</param>
        /// <param name="endsWith">The ends with.</param>
        /// <returns>true if it succeeds, false if it fails.</returns>
        public static bool EndsWithInvariant(this string str, string endsWith)
        {
            return str.EndsWith(endsWith, InvariantComparison());
        }

#if NETFX_CORE
        public static char DirSeparatorChar = '\\';
        public static StringComparison InvariantComparison()
        {
            return StringComparison.CurrentCulture;
        }
        public static StringComparison InvariantComparisonIgnoreCase()
        {
            return StringComparison.CurrentCultureIgnoreCase;
        }
        public static StringComparer InvariantComparer()
        {
            return StringComparer.CurrentCulture;
        }
        public static StringComparer InvariantComparerIgnoreCase()
        {
            return StringComparer.CurrentCultureIgnoreCase;
        }
#else
        /// <summary>The dir separator character.</summary>
        public static char DirSeparatorChar = Path.DirectorySeparatorChar;

        /// <summary>Invariant comparison.</summary>
        /// <returns>A StringComparison.</returns>
        public static StringComparison InvariantComparison()
        {
            return StringComparison.InvariantCulture;
        }

        /// <summary>Invariant comparison ignore case.</summary>
        /// <returns>A StringComparison.</returns>
        public static StringComparison InvariantComparisonIgnoreCase()
        {
            return StringComparison.InvariantCultureIgnoreCase;
        }

        /// <summary>Invariant comparer.</summary>
        /// <returns>A StringComparer.</returns>
        public static StringComparer InvariantComparer()
        {
            return StringComparer.InvariantCulture;
        }

        /// <summary>Invariant comparer ignore case.</summary>
        /// <returns>A StringComparer.</returns>
        public static StringComparer InvariantComparerIgnoreCase()
        {
            return StringComparer.InvariantCultureIgnoreCase;
        }
#endif

    }
}