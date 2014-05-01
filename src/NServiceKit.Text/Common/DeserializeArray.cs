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
using System.Reflection;
using System.Linq;
using System.Threading;

namespace NServiceKit.Text.Common
{
    /// <summary>A deserialize array with elements.</summary>
    /// <typeparam name="TSerializer">Type of the serializer.</typeparam>
    internal static class DeserializeArrayWithElements<TSerializer>
        where TSerializer : ITypeSerializer
    {
        /// <summary>The parse delegate cache.</summary>
        private static Dictionary<Type, ParseArrayOfElementsDelegate> ParseDelegateCache
            = new Dictionary<Type, ParseArrayOfElementsDelegate>();

        /// <summary>Parse array of elements delegate.</summary>
        /// <param name="value">  The value.</param>
        /// <param name="parseFn">The parse function.</param>
        /// <returns>An object.</returns>
        private delegate object ParseArrayOfElementsDelegate(string value, ParseStringDelegate parseFn);

        /// <summary>Gets parse function.</summary>
        /// <param name="type">The type.</param>
        /// <returns>The parse function.</returns>
        public static Func<string, ParseStringDelegate, object> GetParseFn(Type type)
        {
            ParseArrayOfElementsDelegate parseFn;
            if (ParseDelegateCache.TryGetValue(type, out parseFn)) return parseFn.Invoke;

            var genericType = typeof(DeserializeArrayWithElements<,>).MakeGenericType(type, typeof(TSerializer));
#if NETFX_CORE
            var mi = genericType.GetRuntimeMethods().First(p => p.Name.Equals("ParseGenericArray"));
            parseFn = (ParseArrayOfElementsDelegate)mi.CreateDelegate(typeof(ParseArrayOfElementsDelegate));
#else
            var mi = genericType.GetMethod("ParseGenericArray", BindingFlags.Public | BindingFlags.Static);
            parseFn = (ParseArrayOfElementsDelegate)Delegate.CreateDelegate(typeof(ParseArrayOfElementsDelegate), mi);
#endif

            Dictionary<Type, ParseArrayOfElementsDelegate> snapshot, newCache;
            do
            {
                snapshot = ParseDelegateCache;
                newCache = new Dictionary<Type, ParseArrayOfElementsDelegate>(ParseDelegateCache);
                newCache[type] = parseFn;

            } while (!ReferenceEquals(
                Interlocked.CompareExchange(ref ParseDelegateCache, newCache, snapshot), snapshot));

            return parseFn.Invoke;
        }
    }

    /// <summary>A deserialize array with elements.</summary>
    /// <typeparam name="T">          Generic type parameter.</typeparam>
    /// <typeparam name="TSerializer">Type of the serializer.</typeparam>
    internal static class DeserializeArrayWithElements<T, TSerializer>
        where TSerializer : ITypeSerializer
    {
        /// <summary>The serializer.</summary>
        private static readonly ITypeSerializer Serializer = JsWriter.GetTypeSerializer<TSerializer>();

        /// <summary>Parse generic array.</summary>
        /// <param name="value">         The value.</param>
        /// <param name="elementParseFn">The element parse function.</param>
        /// <returns>A T[].</returns>
        public static T[] ParseGenericArray(string value, ParseStringDelegate elementParseFn)
        {
            if ((value = DeserializeListWithElements<TSerializer>.StripList(value)) == null) return null;
            if (value == string.Empty) return new T[0];

            if (value[0] == JsWriter.MapStartChar)
            {
                var itemValues = new List<string>();
                var i = 0;
                do
                {
                    itemValues.Add(Serializer.EatTypeValue(value, ref i));
                    Serializer.EatItemSeperatorOrMapEndChar(value, ref i);
                } while (i < value.Length);

                var results = new T[itemValues.Count];
                for (var j = 0; j < itemValues.Count; j++)
                {
                    results[j] = (T)elementParseFn(itemValues[j]);
                }
                return results;
            }
            else
            {
                var to = new List<T>();
                var valueLength = value.Length;

                var i = 0;
                while (i < valueLength)
                {
                    var elementValue = Serializer.EatValue(value, ref i);
                    var listValue = elementValue;
                    to.Add((T)elementParseFn(listValue));
                    if (Serializer.EatItemSeperatorOrMapEndChar(value, ref i)
                        && i == valueLength)
                    {
                        // If we ate a separator and we are at the end of the value, 
                        // it means the last element is empty => add default
                        to.Add(default(T));
                    }
                }

                return to.ToArray();
            }
        }
    }

    /// <summary>A deserialize array.</summary>
    /// <typeparam name="TSerializer">Type of the serializer.</typeparam>
    internal static class DeserializeArray<TSerializer>
        where TSerializer : ITypeSerializer
    {
        /// <summary>The parse delegate cache.</summary>
        private static Dictionary<Type, ParseStringDelegate> ParseDelegateCache = new Dictionary<Type, ParseStringDelegate>();

        /// <summary>Gets parse function.</summary>
        /// <param name="type">The type.</param>
        /// <returns>The parse function.</returns>
        public static ParseStringDelegate GetParseFn(Type type)
        {
            ParseStringDelegate parseFn;
            if (ParseDelegateCache.TryGetValue(type, out parseFn)) return parseFn;

            var genericType = typeof(DeserializeArray<,>).MakeGenericType(type, typeof(TSerializer));

            var mi = genericType.GetPublicStaticMethod("GetParseFn");
            var parseFactoryFn = (Func<ParseStringDelegate>)mi.MakeDelegate(
                typeof(Func<ParseStringDelegate>));
            parseFn = parseFactoryFn();

            Dictionary<Type, ParseStringDelegate> snapshot, newCache;
            do
            {
                snapshot = ParseDelegateCache;
                newCache = new Dictionary<Type, ParseStringDelegate>(ParseDelegateCache);
                newCache[type] = parseFn;

            } while (!ReferenceEquals(
                Interlocked.CompareExchange(ref ParseDelegateCache, newCache, snapshot), snapshot));

            return parseFn;
        }
    }

    /// <summary>A deserialize array.</summary>
    /// <typeparam name="T">          Generic type parameter.</typeparam>
    /// <typeparam name="TSerializer">Type of the serializer.</typeparam>
    internal static class DeserializeArray<T, TSerializer>
        where TSerializer : ITypeSerializer
    {
        /// <summary>The serializer.</summary>
        private static readonly ITypeSerializer Serializer = JsWriter.GetTypeSerializer<TSerializer>();

        /// <summary>The cache function.</summary>
        private static readonly ParseStringDelegate CacheFn;

        /// <summary>
        /// Initializes static members of the NServiceKit.Text.Common.DeserializeArray&lt;T,
        /// TSerializer&gt; class.
        /// </summary>
        static DeserializeArray()
        {
            CacheFn = GetParseFn();
        }

        /// <summary>Gets the parse.</summary>
        /// <value>The parse.</value>
        public static ParseStringDelegate Parse
        {
            get { return CacheFn; }
        }

        /// <summary>Gets parse function.</summary>
        /// <exception cref="ArgumentException">Thrown when one or more arguments have unsupported or
        /// illegal values.</exception>
        /// <returns>The parse function.</returns>
        public static ParseStringDelegate GetParseFn()
        {
            var type = typeof(T);
            if (!type.IsArray)
                throw new ArgumentException(string.Format("Type {0} is not an Array type", type.FullName));

            if (type == typeof(string[]))
                return ParseStringArray;
            if (type == typeof(byte[]))
                return ParseByteArray;

            var elementType = type.GetElementType();
            var elementParseFn = Serializer.GetParseFn(elementType);
            if (elementParseFn != null)
            {
                var parseFn = DeserializeArrayWithElements<TSerializer>.GetParseFn(elementType);
                return value => parseFn(value, elementParseFn);
            }
            return null;
        }

        /// <summary>Parse string array.</summary>
        /// <param name="value">The value.</param>
        /// <returns>A string[].</returns>
        public static string[] ParseStringArray(string value)
        {
            if ((value = DeserializeListWithElements<TSerializer>.StripList(value)) == null) return null;
            return value == string.Empty
                    ? new string[0]
                    : DeserializeListWithElements<TSerializer>.ParseStringList(value).ToArray();
        }

        /// <summary>Parse byte array.</summary>
        /// <param name="value">The value.</param>
        /// <returns>A byte[].</returns>
        public static byte[] ParseByteArray(string value)
        {
            if ((value = DeserializeListWithElements<TSerializer>.StripList(value)) == null) return null;
            if ((value = Serializer.UnescapeSafeString(value)) == null) return null;
            return value == string.Empty
                    ? new byte[0]
                    : Convert.FromBase64String(value);
        }
    }
}