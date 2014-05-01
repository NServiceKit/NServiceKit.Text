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
using System.Threading;
using System.Linq;
#if WINDOWS_PHONE && !WP8
using NServiceKit.Text.WP;
#endif

namespace NServiceKit.Text.Common
{
    /// <summary>Collection of deserializes.</summary>
    /// <typeparam name="TSerializer">Type of the serializer.</typeparam>
    internal static class DeserializeCollection<TSerializer>
        where TSerializer : ITypeSerializer
    {
        /// <summary>The serializer.</summary>
        private static readonly ITypeSerializer Serializer = JsWriter.GetTypeSerializer<TSerializer>();

        /// <summary>Gets parse method.</summary>
        /// <exception cref="ArgumentException">Thrown when one or more arguments have unsupported or
        /// illegal values.</exception>
        /// <param name="type">The type.</param>
        /// <returns>The parse method.</returns>
        public static ParseStringDelegate GetParseMethod(Type type)
        {
            var collectionInterface = type.GetTypeWithGenericInterfaceOf(typeof(ICollection<>));
            if (collectionInterface == null)
                throw new ArgumentException(string.Format("Type {0} is not of type ICollection<>", type.FullName));

            //optimized access for regularly used types
            if (type.HasInterface(typeof(ICollection<string>)))
                return value => ParseStringCollection(value, type);

            if (type.HasInterface(typeof(ICollection<int>)))
                return value => ParseIntCollection(value, type);

            var elementType = collectionInterface.GenericTypeArguments()[0];
            var supportedTypeParseMethod = Serializer.GetParseFn(elementType);
            if (supportedTypeParseMethod != null)
            {
                var createCollectionType = type.HasAnyTypeDefinitionsOf(typeof(ICollection<>))
                    ? null : type;

                return value => ParseCollectionType(value, createCollectionType, elementType, supportedTypeParseMethod);
            }

            return null;
        }

        /// <summary>Parse string collection.</summary>
        /// <param name="value">     The value.</param>
        /// <param name="createType">Type of the create.</param>
        /// <returns>A list of.</returns>
        public static ICollection<string> ParseStringCollection(string value, Type createType)
        {
            var items = DeserializeArrayWithElements<string, TSerializer>.ParseGenericArray(value, Serializer.ParseString);
            return CollectionExtensions.CreateAndPopulate(createType, items);
        }

        /// <summary>Parse int collection.</summary>
        /// <param name="value">     The value.</param>
        /// <param name="createType">Type of the create.</param>
        /// <returns>A list of.</returns>
        public static ICollection<int> ParseIntCollection(string value, Type createType)
        {
            var items = DeserializeArrayWithElements<int, TSerializer>.ParseGenericArray(value, x => int.Parse(x));
            return CollectionExtensions.CreateAndPopulate(createType, items);
        }

        /// <summary>Parse collection.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="value">     The value.</param>
        /// <param name="createType">Type of the create.</param>
        /// <param name="parseFn">   The parse function.</param>
        /// <returns>A list of.</returns>
        public static ICollection<T> ParseCollection<T>(string value, Type createType, ParseStringDelegate parseFn)
        {
            if (value == null) return null;

            var items = DeserializeArrayWithElements<T, TSerializer>.ParseGenericArray(value, parseFn);
            return CollectionExtensions.CreateAndPopulate(createType, items);
        }

        /// <summary>The parse delegate cache.</summary>
        private static Dictionary<Type, ParseCollectionDelegate> ParseDelegateCache
            = new Dictionary<Type, ParseCollectionDelegate>();

        /// <summary>Parse collection delegate.</summary>
        /// <param name="value">     The value.</param>
        /// <param name="createType">Type of the create.</param>
        /// <param name="parseFn">   The parse function.</param>
        /// <returns>An object.</returns>
        private delegate object ParseCollectionDelegate(string value, Type createType, ParseStringDelegate parseFn);

        /// <summary>Parse collection type.</summary>
        /// <param name="value">      The value.</param>
        /// <param name="createType"> Type of the create.</param>
        /// <param name="elementType">Type of the element.</param>
        /// <param name="parseFn">    The parse function.</param>
        /// <returns>An object.</returns>
        public static object ParseCollectionType(string value, Type createType, Type elementType, ParseStringDelegate parseFn)
        {
            ParseCollectionDelegate parseDelegate;
            if (ParseDelegateCache.TryGetValue(elementType, out parseDelegate))
                return parseDelegate(value, createType, parseFn);

            var mi = typeof(DeserializeCollection<TSerializer>).GetPublicStaticMethod("ParseCollection");
            var genericMi = mi.MakeGenericMethod(new[] { elementType });
            parseDelegate = (ParseCollectionDelegate)genericMi.MakeDelegate(typeof(ParseCollectionDelegate));

            Dictionary<Type, ParseCollectionDelegate> snapshot, newCache;
            do
            {
                snapshot = ParseDelegateCache;
                newCache = new Dictionary<Type, ParseCollectionDelegate>(ParseDelegateCache);
                newCache[elementType] = parseDelegate;

            } while (!ReferenceEquals(
                Interlocked.CompareExchange(ref ParseDelegateCache, newCache, snapshot), snapshot));

            return parseDelegate(value, createType, parseFn);
        }
    }
}