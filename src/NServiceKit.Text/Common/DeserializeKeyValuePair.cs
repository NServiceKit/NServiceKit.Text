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
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Linq;
using NServiceKit.Text.Json;

namespace NServiceKit.Text.Common
{
    /// <summary>A deserialize key value pair.</summary>
    /// <typeparam name="TSerializer">Type of the serializer.</typeparam>
    internal static class DeserializeKeyValuePair<TSerializer>
        where TSerializer : ITypeSerializer
    {
        /// <summary>The serializer.</summary>
        private static readonly ITypeSerializer Serializer = JsWriter.GetTypeSerializer<TSerializer>();

        /// <summary>Zero-based index of the key.</summary>
        const int KeyIndex = 0;

        /// <summary>Zero-based index of the value.</summary>
        const int ValueIndex = 1;

        /// <summary>Gets parse method.</summary>
        /// <param name="type">The type.</param>
        /// <returns>The parse method.</returns>
        public static ParseStringDelegate GetParseMethod(Type type)
        {
            var mapInterface = type.GetTypeWithGenericInterfaceOf(typeof(KeyValuePair<,>));

            var keyValuePairArgs = mapInterface.GenericTypeArguments();
            var keyTypeParseMethod = Serializer.GetParseFn(keyValuePairArgs[KeyIndex]);
            if (keyTypeParseMethod == null) return null;

            var valueTypeParseMethod = Serializer.GetParseFn(keyValuePairArgs[ValueIndex]);
            if (valueTypeParseMethod == null) return null;

            var createMapType = type.HasAnyTypeDefinitionsOf(typeof(KeyValuePair<,>))
                ? null : type;

            return value => ParseKeyValuePairType(value, createMapType, keyValuePairArgs, keyTypeParseMethod, valueTypeParseMethod);
        }

        /// <summary>Parse key value pair.</summary>
        /// <exception cref="SerializationException">Thrown when a Serialization error condition occurs.</exception>
        /// <typeparam name="TKey">  Type of the key.</typeparam>
        /// <typeparam name="TValue">Type of the value.</typeparam>
        /// <param name="value">        The value.</param>
        /// <param name="createMapType">Type of the create map.</param>
        /// <param name="parseKeyFn">   The parse key function.</param>
        /// <param name="parseValueFn"> The parse value function.</param>
        /// <returns>An object.</returns>
        public static object ParseKeyValuePair<TKey, TValue>(
            string value, Type createMapType,
            ParseStringDelegate parseKeyFn, ParseStringDelegate parseValueFn)
        {
            if (value == null) return default(KeyValuePair<TKey, TValue>);

            var index = VerifyAndGetStartIndex(value, createMapType);

            if (JsonTypeSerializer.IsEmptyMap(value, index)) return new KeyValuePair<TKey, TValue>();
            var keyValue = default(TKey);
            var valueValue = default(TValue);

            var valueLength = value.Length;
            while (index < valueLength)
            {
                var key = Serializer.EatMapKey(value, ref index);
                Serializer.EatMapKeySeperator(value, ref index);
                var keyElementValue = Serializer.EatTypeValue(value, ref index);

                if (key.CompareIgnoreCase("key") == 0)
                    keyValue = (TKey)parseKeyFn(keyElementValue);
                else if (key.CompareIgnoreCase("value") == 0)
                    valueValue = (TValue)parseValueFn(keyElementValue);
                else
                    throw new SerializationException("Incorrect KeyValuePair property: " + key);
                Serializer.EatItemSeperatorOrMapEndChar(value, ref index);
            }

            return new KeyValuePair<TKey, TValue>(keyValue, valueValue);
        }

        /// <summary>Verify and get start index.</summary>
        /// <param name="value">        The value.</param>
        /// <param name="createMapType">Type of the create map.</param>
        /// <returns>An int.</returns>
        private static int VerifyAndGetStartIndex(string value, Type createMapType)
        {
            var index = 0;
            if (!Serializer.EatMapStartChar(value, ref index))
            {
                //Don't throw ex because some KeyValueDataContractDeserializer don't have '{}'
                Tracer.Instance.WriteDebug("WARN: Map definitions should start with a '{0}', expecting serialized type '{1}', got string starting with: {2}",
                                           JsWriter.MapStartChar, createMapType != null ? createMapType.Name : "Dictionary<,>", value.Substring(0, value.Length < 50 ? value.Length : 50));
            }
            return index;
        }

        /// <summary>The parse delegate cache.</summary>
        private static Dictionary<string, ParseKeyValuePairDelegate> ParseDelegateCache
            = new Dictionary<string, ParseKeyValuePairDelegate>();

        /// <summary>Parse key value pair delegate.</summary>
        /// <param name="value">        The value.</param>
        /// <param name="createMapType">Type of the create map.</param>
        /// <param name="keyParseFn">   The key parse function.</param>
        /// <param name="valueParseFn"> The value parse function.</param>
        /// <returns>An object.</returns>
        private delegate object ParseKeyValuePairDelegate(string value, Type createMapType,
            ParseStringDelegate keyParseFn, ParseStringDelegate valueParseFn);

        /// <summary>Parse key value pair type.</summary>
        /// <param name="value">        The value.</param>
        /// <param name="createMapType">Type of the create map.</param>
        /// <param name="argTypes">     List of types of the arguments.</param>
        /// <param name="keyParseFn">   The key parse function.</param>
        /// <param name="valueParseFn"> The value parse function.</param>
        /// <returns>An object.</returns>
        public static object ParseKeyValuePairType(string value, Type createMapType, Type[] argTypes,
            ParseStringDelegate keyParseFn, ParseStringDelegate valueParseFn)
        {

            ParseKeyValuePairDelegate parseDelegate;
            var key = GetTypesKey(argTypes);
            if (ParseDelegateCache.TryGetValue(key, out parseDelegate))
                return parseDelegate(value, createMapType, keyParseFn, valueParseFn);

            var mi = typeof(DeserializeKeyValuePair<TSerializer>).GetPublicStaticMethod("ParseKeyValuePair");
            var genericMi = mi.MakeGenericMethod(argTypes);
            parseDelegate = (ParseKeyValuePairDelegate)genericMi.MakeDelegate(typeof(ParseKeyValuePairDelegate));

            Dictionary<string, ParseKeyValuePairDelegate> snapshot, newCache;
            do
            {
                snapshot = ParseDelegateCache;
                newCache = new Dictionary<string, ParseKeyValuePairDelegate>(ParseDelegateCache);
                newCache[key] = parseDelegate;

            } while (!ReferenceEquals(
                Interlocked.CompareExchange(ref ParseDelegateCache, newCache, snapshot), snapshot));

            return parseDelegate(value, createMapType, keyParseFn, valueParseFn);
        }

        /// <summary>Gets types key.</summary>
        /// <param name="types">A variable-length parameters list containing types.</param>
        /// <returns>The types key.</returns>
        private static string GetTypesKey(params Type[] types)
        {
            var sb = new StringBuilder(256);
            foreach (var type in types)
            {
                if (sb.Length > 0)
                    sb.Append(">");

                sb.Append(type.FullName);
            }
            return sb.ToString();
        }
    }
}