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
using System.IO;
using System.Reflection;
using System.Threading;
using System.Linq;
using NServiceKit.Text.Json;

namespace NServiceKit.Text.Common
{
    /// <summary>Writes a map delegate.</summary>
    /// <param name="writer">      The writer.</param>
    /// <param name="oMap">        The map.</param>
    /// <param name="writeKeyFn">  The write key function.</param>
    /// <param name="writeValueFn">The write value function.</param>
    internal delegate void WriteMapDelegate(
        TextWriter writer,
        object oMap,
        WriteObjectDelegate writeKeyFn,
        WriteObjectDelegate writeValueFn);

    /// <summary>Dictionary of writes.</summary>
    /// <typeparam name="TSerializer">Type of the serializer.</typeparam>
    internal static class WriteDictionary<TSerializer>
        where TSerializer : ITypeSerializer
    {
        /// <summary>The serializer.</summary>
        private static readonly ITypeSerializer Serializer = JsWriter.GetTypeSerializer<TSerializer>();

        /// <summary>A map key.</summary>
        internal class MapKey
        {
            /// <summary>Type of the key.</summary>
            internal Type KeyType;

            /// <summary>Type of the value.</summary>
            internal Type ValueType;

            /// <summary>
            /// Initializes a new instance of the NServiceKit.Text.Common.WriteDictionary&lt;TSerializer&gt;
            /// .MapKey class.
            /// </summary>
            /// <param name="keyType">  Type of the key.</param>
            /// <param name="valueType">Type of the value.</param>
            public MapKey(Type keyType, Type valueType)
            {
                KeyType = keyType;
                ValueType = valueType;
            }

            /// <summary>
            /// Determines whether the specified <see cref="T:System.Object" /> is equal to the current
            /// <see cref="T:System.Object" />.
            /// </summary>
            /// <param name="other">The map key to compare to this object.</param>
            /// <returns>
            /// true if the specified <see cref="T:System.Object" /> is equal to the current
            /// <see cref="T:System.Object" />; otherwise, false.
            /// </returns>
            public bool Equals(MapKey other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return Equals(other.KeyType, KeyType) && Equals(other.ValueType, ValueType);
            }

            /// <summary>
            /// Determines whether the specified <see cref="T:System.Object" /> is equal to the current
            /// <see cref="T:System.Object" />.
            /// </summary>
            /// <param name="obj">The <see cref="T:System.Object" /> to compare with the current
            /// <see cref="T:System.Object" />.</param>
            /// <returns>
            /// true if the specified <see cref="T:System.Object" /> is equal to the current
            /// <see cref="T:System.Object" />; otherwise, false.
            /// </returns>
            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != typeof(MapKey)) return false;
                return Equals((MapKey)obj);
            }

            /// <summary>Serves as a hash function for a particular type.</summary>
            /// <returns>A hash code for the current <see cref="T:System.Object" />.</returns>
            public override int GetHashCode()
            {
                unchecked
                {
                    return ((KeyType != null ? KeyType.GetHashCode() : 0) * 397) ^ (ValueType != null ? ValueType.GetHashCode() : 0);
                }
            }
        }

        /// <summary>The cache fns.</summary>
        static Dictionary<MapKey, WriteMapDelegate> CacheFns = new Dictionary<MapKey, WriteMapDelegate>();

        /// <summary>Gets write generic dictionary.</summary>
        /// <param name="keyType">  Type of the key.</param>
        /// <param name="valueType">Type of the value.</param>
        /// <returns>The write generic dictionary.</returns>
        public static Action<TextWriter, object, WriteObjectDelegate, WriteObjectDelegate>
            GetWriteGenericDictionary(Type keyType, Type valueType)
        {
            WriteMapDelegate writeFn;
            var mapKey = new MapKey(keyType, valueType);
            if (CacheFns.TryGetValue(mapKey, out writeFn)) return writeFn.Invoke;

            var genericType = typeof(ToStringDictionaryMethods<,,>).MakeGenericType(keyType, valueType, typeof(TSerializer));
            var mi = genericType.GetPublicStaticMethod("WriteIDictionary");
            writeFn = (WriteMapDelegate)mi.MakeDelegate(typeof(WriteMapDelegate));

            Dictionary<MapKey, WriteMapDelegate> snapshot, newCache;
            do
            {
                snapshot = CacheFns;
                newCache = new Dictionary<MapKey, WriteMapDelegate>(CacheFns);
                newCache[mapKey] = writeFn;

            } while (!ReferenceEquals(
                Interlocked.CompareExchange(ref CacheFns, newCache, snapshot), snapshot));

            return writeFn.Invoke;
        }

        /// <summary>Writes an i dictionary.</summary>
        /// <param name="writer">The writer.</param>
        /// <param name="oMap">  The map.</param>
        public static void WriteIDictionary(TextWriter writer, object oMap)
        {
            WriteObjectDelegate writeKeyFn = null;
            WriteObjectDelegate writeValueFn = null;

            writer.Write(JsWriter.MapStartChar);
            var encodeMapKey = false;
            Type lastKeyType = null;
            Type lastValueType = null;

            var map = (IDictionary)oMap;
            var ranOnce = false;
            foreach (var key in map.Keys)
            {
                var dictionaryValue = map[key];

                var isNull = (dictionaryValue == null);
                if (isNull && !Serializer.IncludeNullValues) continue;

                var keyType = key.GetType();
                if (writeKeyFn == null || lastKeyType != keyType)
                {
                    lastKeyType = keyType;
                    writeKeyFn = Serializer.GetWriteFn(keyType);
                    encodeMapKey = Serializer.GetTypeInfo(keyType).EncodeMapKey;
                }

                JsWriter.WriteItemSeperatorIfRanOnce(writer, ref ranOnce);

                JsState.WritingKeyCount++;
                JsState.IsWritingValue = false;

                if (encodeMapKey)
                {
                    JsState.IsWritingValue = true; //prevent ""null""
                    writer.Write(JsWriter.QuoteChar);
                    writeKeyFn(writer, key);
                    writer.Write(JsWriter.QuoteChar);
                }
                else
                {
                    writeKeyFn(writer, key);
                }

                JsState.WritingKeyCount--;

                writer.Write(JsWriter.MapKeySeperator);

                if (isNull)
                {
                    writer.Write(JsonUtils.Null);
                }
                else
                {
                    var valueType = dictionaryValue.GetType();
                    if (writeValueFn == null || lastValueType != valueType)
                    {
                        lastValueType = valueType;
                        writeValueFn = Serializer.GetWriteFn(valueType);
                    }

                    JsState.IsWritingValue = true;
                    writeValueFn(writer, dictionaryValue);
                    JsState.IsWritingValue = false;
                }
            }

            writer.Write(JsWriter.MapEndChar);
        }
    }

    /// <summary>to string dictionary methods.</summary>
    /// <typeparam name="TKey">       Type of the key.</typeparam>
    /// <typeparam name="TValue">     Type of the value.</typeparam>
    /// <typeparam name="TSerializer">Type of the serializer.</typeparam>
    internal static class ToStringDictionaryMethods<TKey, TValue, TSerializer>
        where TSerializer : ITypeSerializer
    {
        /// <summary>The serializer.</summary>
        private static readonly ITypeSerializer Serializer = JsWriter.GetTypeSerializer<TSerializer>();

        /// <summary>Writes an i dictionary.</summary>
        /// <param name="writer">      The writer.</param>
        /// <param name="oMap">        The map.</param>
        /// <param name="writeKeyFn">  The write key function.</param>
        /// <param name="writeValueFn">The write value function.</param>
        public static void WriteIDictionary(
            TextWriter writer,
            object oMap,
            WriteObjectDelegate writeKeyFn,
            WriteObjectDelegate writeValueFn)
        {
            if (writer == null) return; //AOT
            WriteGenericIDictionary(writer, (IDictionary<TKey, TValue>)oMap, writeKeyFn, writeValueFn);
        }

        /// <summary>Writes a generic i dictionary.</summary>
        /// <param name="writer">      The writer.</param>
        /// <param name="map">         The map.</param>
        /// <param name="writeKeyFn">  The write key function.</param>
        /// <param name="writeValueFn">The write value function.</param>
        public static void WriteGenericIDictionary(
            TextWriter writer,
            IDictionary<TKey, TValue> map,
            WriteObjectDelegate writeKeyFn,
            WriteObjectDelegate writeValueFn)
        {
            if (map == null)
            {
                writer.Write(JsonUtils.Null);
                return;
            }
            writer.Write(JsWriter.MapStartChar);

            var encodeMapKey = Serializer.GetTypeInfo(typeof(TKey)).EncodeMapKey;

            var ranOnce = false;
            foreach (var kvp in map)
            {
                var isNull = (kvp.Value == null);
                if (isNull && !Serializer.IncludeNullValues) continue;

                JsWriter.WriteItemSeperatorIfRanOnce(writer, ref ranOnce);

                JsState.WritingKeyCount++;
                JsState.IsWritingValue = false;

                if (encodeMapKey)
                {
                    JsState.IsWritingValue = true; //prevent ""null""
                    writer.Write(JsWriter.QuoteChar);
                    writeKeyFn(writer, kvp.Key);
                    writer.Write(JsWriter.QuoteChar);
                }
                else
                {
                    writeKeyFn(writer, kvp.Key);
                }

                JsState.WritingKeyCount--;

                writer.Write(JsWriter.MapKeySeperator);

                if (isNull)
                {
                    writer.Write(JsonUtils.Null);
                }
                else
                {
                    JsState.IsWritingValue = true;
                    writeValueFn(writer, kvp.Value);
                    JsState.IsWritingValue = false;
                }
            }

            writer.Write(JsWriter.MapEndChar);
        }
    }
}