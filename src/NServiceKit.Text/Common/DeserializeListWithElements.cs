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
using System.Collections.ObjectModel;
using System.Reflection;
using System.Threading;
using NServiceKit.Text.Json;

namespace NServiceKit.Text.Common
{
    /// <summary>A deserialize list with elements.</summary>
    /// <typeparam name="TSerializer">Type of the serializer.</typeparam>
    internal static class DeserializeListWithElements<TSerializer>
        where TSerializer : ITypeSerializer
    {
        /// <summary>The serializer.</summary>
        internal static readonly ITypeSerializer Serializer = JsWriter.GetTypeSerializer<TSerializer>();

        /// <summary>The parse delegate cache.</summary>
        private static Dictionary<Type, ParseListDelegate> ParseDelegateCache
            = new Dictionary<Type, ParseListDelegate>();

        /// <summary>Parse list delegate.</summary>
        /// <param name="value">         The value.</param>
        /// <param name="createListType">Type of the create list.</param>
        /// <param name="parseFn">       The parse function.</param>
        /// <returns>An object.</returns>
        private delegate object ParseListDelegate(string value, Type createListType, ParseStringDelegate parseFn);

        /// <summary>Gets list type parse function.</summary>
        /// <param name="createListType">Type of the create list.</param>
        /// <param name="elementType">   Type of the element.</param>
        /// <param name="parseFn">       The parse function.</param>
        /// <returns>The list type parse function.</returns>
        public static Func<string, Type, ParseStringDelegate, object> GetListTypeParseFn(
            Type createListType, Type elementType, ParseStringDelegate parseFn)
        {
            ParseListDelegate parseDelegate;
            if (ParseDelegateCache.TryGetValue(elementType, out parseDelegate))
                return parseDelegate.Invoke;

            var genericType = typeof(DeserializeListWithElements<,>).MakeGenericType(elementType, typeof(TSerializer));
            var mi = genericType.GetPublicStaticMethod("ParseGenericList");
            parseDelegate = (ParseListDelegate)mi.MakeDelegate(typeof(ParseListDelegate));

            Dictionary<Type, ParseListDelegate> snapshot, newCache;
            do
            {
                snapshot = ParseDelegateCache;
                newCache = new Dictionary<Type, ParseListDelegate>(ParseDelegateCache);
                newCache[elementType] = parseDelegate;

            } while (!ReferenceEquals(
                Interlocked.CompareExchange(ref ParseDelegateCache, newCache, snapshot), snapshot));

            return parseDelegate.Invoke;
        }

        /// <summary>Strip list.</summary>
        /// <param name="value">The value.</param>
        /// <returns>A string.</returns>
        internal static string StripList(string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;

            const int startQuotePos = 1;
            const int endQuotePos = 2;
            var ret = value[0] == JsWriter.ListStartChar
                    ? value.Substring(startQuotePos, value.Length - endQuotePos)
                    : value;

            var pos = 0;
            Serializer.EatWhitespace(ret, ref pos);
            return ret.Substring(pos, ret.Length - pos);
        }

        /// <summary>Parse string list.</summary>
        /// <param name="value">The value.</param>
        /// <returns>A List&lt;string&gt;</returns>
        public static List<string> ParseStringList(string value)
        {
            if ((value = StripList(value)) == null) return null;
            if (value == string.Empty) return new List<string>();

            var to = new List<string>();
            var valueLength = value.Length;

            var i = 0;
            while (i < valueLength)
            {
                var elementValue = Serializer.EatValue(value, ref i);
                var listValue = Serializer.UnescapeString(elementValue);
                to.Add(listValue);
                if (Serializer.EatItemSeperatorOrMapEndChar(value, ref i) && i == valueLength)
                {
                    // If we ate a separator and we are at the end of the value, 
                    // it means the last element is empty => add default
                    to.Add(null);
                }
            }

            return to;
        }

        /// <summary>Parse int list.</summary>
        /// <param name="value">The value.</param>
        /// <returns>A List&lt;int&gt;</returns>
        public static List<int> ParseIntList(string value)
        {
            if ((value = StripList(value)) == null) return null;
            if (value == string.Empty) return new List<int>();

            var intParts = value.Split(JsWriter.ItemSeperator);
            var intValues = new List<int>(intParts.Length);
            foreach (var intPart in intParts)
            {
                intValues.Add(int.Parse(intPart));
            }
            return intValues;
        }
    }

    /// <summary>A deserialize list with elements.</summary>
    /// <typeparam name="T">          Generic type parameter.</typeparam>
    /// <typeparam name="TSerializer">Type of the serializer.</typeparam>
    internal static class DeserializeListWithElements<T, TSerializer>
        where TSerializer : ITypeSerializer
    {
        /// <summary>The serializer.</summary>
        private static readonly ITypeSerializer Serializer = JsWriter.GetTypeSerializer<TSerializer>();

        /// <summary>Parse generic list.</summary>
        /// <param name="value">         The value.</param>
        /// <param name="createListType">Type of the create list.</param>
        /// <param name="parseFn">       The parse function.</param>
        /// <returns>A list of.</returns>
        public static ICollection<T> ParseGenericList(string value, Type createListType, ParseStringDelegate parseFn)
        {
            if ((value = DeserializeListWithElements<TSerializer>.StripList(value)) == null) return null;

            var isReadOnly = createListType != null
                && (createListType.IsGeneric() && createListType.GenericTypeDefinition() == typeof(ReadOnlyCollection<>));

            var to = (createListType == null || isReadOnly)
                ? new List<T>()
                : (ICollection<T>)createListType.CreateInstance();

            if (value == string.Empty) return to;

            var tryToParseItemsAsPrimitiveTypes =
                JsConfig.TryToParsePrimitiveTypeValues && typeof(T) == typeof(object);

            if (!string.IsNullOrEmpty(value))
            {
                var valueLength = value.Length;
                var i = 0;
                Serializer.EatWhitespace(value, ref i);
                if (i < valueLength && value[i] == JsWriter.MapStartChar)
                {
                    do
                    {
                        var itemValue = Serializer.EatTypeValue(value, ref i);
                        to.Add((T)parseFn(itemValue));
                        Serializer.EatWhitespace(value, ref i);
                    } while (++i < value.Length);
                }
                else
                {

                    while (i < valueLength)
                    {
                        var startIndex = i;
                        var elementValue = Serializer.EatValue(value, ref i);
                        var listValue = elementValue;
                        if (listValue != null)
                        {
                            if (tryToParseItemsAsPrimitiveTypes)
                            {
                                Serializer.EatWhitespace(value, ref startIndex);
                                to.Add((T)DeserializeType<TSerializer>.ParsePrimitive(elementValue, value[startIndex]));
                            }
                            else
                            {
                                to.Add((T)parseFn(elementValue));
                            }
                        }

                        if (Serializer.EatItemSeperatorOrMapEndChar(value, ref i) && i == valueLength)
                        {
                            // If we ate a separator and we are at the end of the value, 
                            // it means the last element is empty => add default
                            to.Add(default(T));
                            continue;
                        }

                        if (listValue == null)
                            to.Add(default(T));
                    }

                }
            }

            //TODO: 8-10-2011 -- this CreateInstance call should probably be moved over to ReflectionExtensions, 
            //but not sure how you'd like to go about caching constructors with parameters -- I would probably build a NewExpression, .Compile to a LambdaExpression and cache
            return isReadOnly ? (ICollection<T>)Activator.CreateInstance(createListType, to) : to;
        }
    }

    /// <summary>List of deserializes.</summary>
    /// <typeparam name="T">          Generic type parameter.</typeparam>
    /// <typeparam name="TSerializer">Type of the serializer.</typeparam>
    internal static class DeserializeList<T, TSerializer>
        where TSerializer : ITypeSerializer
    {
        /// <summary>The cache function.</summary>
        private readonly static ParseStringDelegate CacheFn;

        /// <summary>
        /// Initializes static members of the NServiceKit.Text.Common.DeserializeList&lt;T,
        /// TSerializer&gt; class.
        /// </summary>
        static DeserializeList()
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
            var listInterface = typeof(T).GetTypeWithGenericInterfaceOf(typeof(IList<>));
            if (listInterface == null)
                throw new ArgumentException(string.Format("Type {0} is not of type IList<>", typeof(T).FullName));

            //optimized access for regularly used types
            if (typeof(T) == typeof(List<string>))
                return DeserializeListWithElements<TSerializer>.ParseStringList;

            if (typeof(T) == typeof(List<int>))
                return DeserializeListWithElements<TSerializer>.ParseIntList;

            var elementType = listInterface.GenericTypeArguments()[0];

            var supportedTypeParseMethod = DeserializeListWithElements<TSerializer>.Serializer.GetParseFn(elementType);
            if (supportedTypeParseMethod != null)
            {
                var createListType = typeof(T).HasAnyTypeDefinitionsOf(typeof(List<>), typeof(IList<>))
                    ? null : typeof(T);

                var parseFn = DeserializeListWithElements<TSerializer>.GetListTypeParseFn(createListType, elementType, supportedTypeParseMethod);
                return value => parseFn(value, createListType, supportedTypeParseMethod);
            }

            return null;
        }

    }

    /// <summary>A deserialize enumerable.</summary>
    /// <typeparam name="T">          Generic type parameter.</typeparam>
    /// <typeparam name="TSerializer">Type of the serializer.</typeparam>
    internal static class DeserializeEnumerable<T, TSerializer>
        where TSerializer : ITypeSerializer
    {
        /// <summary>The cache function.</summary>
        private readonly static ParseStringDelegate CacheFn;

        /// <summary>
        /// Initializes static members of the NServiceKit.Text.Common.DeserializeEnumerable&lt;T,
        /// TSerializer&gt; class.
        /// </summary>
        static DeserializeEnumerable()
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
            var enumerableInterface = typeof(T).GetTypeWithGenericInterfaceOf(typeof(IEnumerable<>));
            if (enumerableInterface == null)
                throw new ArgumentException(string.Format("Type {0} is not of type IEnumerable<>", typeof(T).FullName));

            //optimized access for regularly used types
            if (typeof(T) == typeof(IEnumerable<string>))
                return DeserializeListWithElements<TSerializer>.ParseStringList;

            if (typeof(T) == typeof(IEnumerable<int>))
                return DeserializeListWithElements<TSerializer>.ParseIntList;

            var elementType = enumerableInterface.GenericTypeArguments()[0];

            var supportedTypeParseMethod = DeserializeListWithElements<TSerializer>.Serializer.GetParseFn(elementType);
            if (supportedTypeParseMethod != null)
            {
                const Type createListTypeWithNull = null; //Use conversions outside this class. see: Queue

                var parseFn = DeserializeListWithElements<TSerializer>.GetListTypeParseFn(
                    createListTypeWithNull, elementType, supportedTypeParseMethod);

                return value => parseFn(value, createListTypeWithNull, supportedTypeParseMethod);
            }

            return null;
        }

    }
}