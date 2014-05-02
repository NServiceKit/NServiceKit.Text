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

namespace NServiceKit.Text.Common
{
    /// <summary>A write lists of elements.</summary>
    /// <typeparam name="TSerializer">Type of the serializer.</typeparam>
    internal static class WriteListsOfElements<TSerializer>
        where TSerializer : ITypeSerializer
    {
        /// <summary>The serializer.</summary>
        private static readonly ITypeSerializer Serializer = JsWriter.GetTypeSerializer<TSerializer>();

        /// <summary>The list cache fns.</summary>
        static Dictionary<Type, WriteObjectDelegate> ListCacheFns = new Dictionary<Type, WriteObjectDelegate>();

        /// <summary>Gets list write function.</summary>
        /// <param name="elementType">Type of the element.</param>
        /// <returns>The list write function.</returns>
        public static WriteObjectDelegate GetListWriteFn(Type elementType)
        {
            WriteObjectDelegate writeFn;
            if (ListCacheFns.TryGetValue(elementType, out writeFn)) return writeFn;

            var genericType = typeof(WriteListsOfElements<,>).MakeGenericType(elementType, typeof(TSerializer));
            var mi = genericType.GetPublicStaticMethod("WriteList");
            writeFn = (WriteObjectDelegate)mi.MakeDelegate(typeof(WriteObjectDelegate));

            Dictionary<Type, WriteObjectDelegate> snapshot, newCache;
            do
            {
                snapshot = ListCacheFns;
                newCache = new Dictionary<Type, WriteObjectDelegate>(ListCacheFns);
                newCache[elementType] = writeFn;

            } while (!ReferenceEquals(
                Interlocked.CompareExchange(ref ListCacheFns, newCache, snapshot), snapshot));

            return writeFn;
        }

        /// <summary>Zero-based index of the list cache fns.</summary>
        static Dictionary<Type, WriteObjectDelegate> IListCacheFns = new Dictionary<Type, WriteObjectDelegate>();

        /// <summary>Gets i list write function.</summary>
        /// <param name="elementType">Type of the element.</param>
        /// <returns>The i list write function.</returns>
        public static WriteObjectDelegate GetIListWriteFn(Type elementType)
        {
            WriteObjectDelegate writeFn;
            if (IListCacheFns.TryGetValue(elementType, out writeFn)) return writeFn;

            var genericType = typeof(WriteListsOfElements<,>).MakeGenericType(elementType, typeof(TSerializer));
            var mi = genericType.GetPublicStaticMethod("WriteIList");
            writeFn = (WriteObjectDelegate)mi.MakeDelegate(typeof(WriteObjectDelegate));

            Dictionary<Type, WriteObjectDelegate> snapshot, newCache;
            do
            {
                snapshot = IListCacheFns;
                newCache = new Dictionary<Type, WriteObjectDelegate>(IListCacheFns);
                newCache[elementType] = writeFn;

            } while (!ReferenceEquals(
                Interlocked.CompareExchange(ref IListCacheFns, newCache, snapshot), snapshot));

            return writeFn;
        }

        /// <summary>The cache fns.</summary>
        static Dictionary<Type, WriteObjectDelegate> CacheFns = new Dictionary<Type, WriteObjectDelegate>();

        /// <summary>Gets generic write array.</summary>
        /// <param name="elementType">Type of the element.</param>
        /// <returns>The generic write array.</returns>
        public static WriteObjectDelegate GetGenericWriteArray(Type elementType)
        {
            WriteObjectDelegate writeFn;
            if (CacheFns.TryGetValue(elementType, out writeFn)) return writeFn;

            var genericType = typeof(WriteListsOfElements<,>).MakeGenericType(elementType, typeof(TSerializer));
            var mi = genericType.GetPublicStaticMethod("WriteArray");
            writeFn = (WriteObjectDelegate)mi.MakeDelegate(typeof(WriteObjectDelegate));

            Dictionary<Type, WriteObjectDelegate> snapshot, newCache;
            do
            {
                snapshot = CacheFns;
                newCache = new Dictionary<Type, WriteObjectDelegate>(CacheFns);
                newCache[elementType] = writeFn;

            } while (!ReferenceEquals(
                Interlocked.CompareExchange(ref CacheFns, newCache, snapshot), snapshot));

            return writeFn;
        }

        /// <summary>The enumerable cache fns.</summary>
        static Dictionary<Type, WriteObjectDelegate> EnumerableCacheFns = new Dictionary<Type, WriteObjectDelegate>();

        /// <summary>Gets generic write enumerable.</summary>
        /// <param name="elementType">Type of the element.</param>
        /// <returns>The generic write enumerable.</returns>
        public static WriteObjectDelegate GetGenericWriteEnumerable(Type elementType)
        {
            WriteObjectDelegate writeFn;
            if (EnumerableCacheFns.TryGetValue(elementType, out writeFn)) return writeFn;

            var genericType = typeof(WriteListsOfElements<,>).MakeGenericType(elementType, typeof(TSerializer));
            var mi = genericType.GetPublicStaticMethod("WriteEnumerable");
            writeFn = (WriteObjectDelegate)mi.MakeDelegate(typeof(WriteObjectDelegate));

            Dictionary<Type, WriteObjectDelegate> snapshot, newCache;
            do
            {
                snapshot = EnumerableCacheFns;
                newCache = new Dictionary<Type, WriteObjectDelegate>(EnumerableCacheFns);
                newCache[elementType] = writeFn;

            } while (!ReferenceEquals(
                Interlocked.CompareExchange(ref EnumerableCacheFns, newCache, snapshot), snapshot));

            return writeFn;
        }

        /// <summary>The list value type cache fns.</summary>
        static Dictionary<Type, WriteObjectDelegate> ListValueTypeCacheFns = new Dictionary<Type, WriteObjectDelegate>();

        /// <summary>Gets write list value type.</summary>
        /// <param name="elementType">Type of the element.</param>
        /// <returns>The write list value type.</returns>
        public static WriteObjectDelegate GetWriteListValueType(Type elementType)
        {
            WriteObjectDelegate writeFn;
            if (ListValueTypeCacheFns.TryGetValue(elementType, out writeFn)) return writeFn;

            var genericType = typeof(WriteListsOfElements<,>).MakeGenericType(elementType, typeof(TSerializer));
            var mi = genericType.GetPublicStaticMethod("WriteListValueType");
            writeFn = (WriteObjectDelegate)mi.MakeDelegate(typeof(WriteObjectDelegate));

            Dictionary<Type, WriteObjectDelegate> snapshot, newCache;
            do
            {
                snapshot = ListValueTypeCacheFns;
                newCache = new Dictionary<Type, WriteObjectDelegate>(ListValueTypeCacheFns);
                newCache[elementType] = writeFn;

            } while (!ReferenceEquals(
                Interlocked.CompareExchange(ref ListValueTypeCacheFns, newCache, snapshot), snapshot));

            return writeFn;
        }

        /// <summary>Zero-based index of the list value type cache fns.</summary>
        static Dictionary<Type, WriteObjectDelegate> IListValueTypeCacheFns = new Dictionary<Type, WriteObjectDelegate>();

        /// <summary>Gets write i list value type.</summary>
        /// <param name="elementType">Type of the element.</param>
        /// <returns>The write i list value type.</returns>
        public static WriteObjectDelegate GetWriteIListValueType(Type elementType)
        {
            WriteObjectDelegate writeFn;

            if (IListValueTypeCacheFns.TryGetValue(elementType, out writeFn)) return writeFn;

            var genericType = typeof(WriteListsOfElements<,>).MakeGenericType(elementType, typeof(TSerializer));
            var mi = genericType.GetPublicStaticMethod("WriteIListValueType");
            writeFn = (WriteObjectDelegate)mi.MakeDelegate(typeof(WriteObjectDelegate));

            Dictionary<Type, WriteObjectDelegate> snapshot, newCache;
            do
            {
                snapshot = IListValueTypeCacheFns;
                newCache = new Dictionary<Type, WriteObjectDelegate>(IListValueTypeCacheFns);
                newCache[elementType] = writeFn;

            } while (!ReferenceEquals(
                Interlocked.CompareExchange(ref IListValueTypeCacheFns, newCache, snapshot), snapshot));

            return writeFn;
        }

        /// <summary>Writes an i enumerable.</summary>
        /// <param name="writer">          The writer.</param>
        /// <param name="oValueCollection">Collection of values.</param>
        public static void WriteIEnumerable(TextWriter writer, object oValueCollection)
        {
            WriteObjectDelegate toStringFn = null;

            writer.Write(JsWriter.ListStartChar);

            var valueCollection = (IEnumerable)oValueCollection;
            var ranOnce = false;
            foreach (var valueItem in valueCollection)
            {
                if (toStringFn == null)
                    toStringFn = Serializer.GetWriteFn(valueItem.GetType());

                JsWriter.WriteItemSeperatorIfRanOnce(writer, ref ranOnce);

                toStringFn(writer, valueItem);
            }

            writer.Write(JsWriter.ListEndChar);
        }
    }

    /// <summary>A write lists of elements.</summary>
    /// <typeparam name="T">          Generic type parameter.</typeparam>
    /// <typeparam name="TSerializer">Type of the serializer.</typeparam>
    internal static class WriteListsOfElements<T, TSerializer>
        where TSerializer : ITypeSerializer
    {
        /// <summary>The element write function.</summary>
        private static readonly WriteObjectDelegate ElementWriteFn;

        /// <summary>
        /// Initializes static members of the NServiceKit.Text.Common.WriteListsOfElements&lt;T,
        /// TSerializer&gt; class.
        /// </summary>
        static WriteListsOfElements()
        {
            ElementWriteFn = JsWriter.GetTypeSerializer<TSerializer>().GetWriteFn<T>();
        }

        /// <summary>Writes a list.</summary>
        /// <param name="writer">The writer.</param>
        /// <param name="oList"> The list.</param>
        public static void WriteList(TextWriter writer, object oList)
        {
            WriteGenericIList(writer, (IList<T>)oList);
        }

        /// <summary>Writes a generic list.</summary>
        /// <param name="writer">The writer.</param>
        /// <param name="list">  The list.</param>
        public static void WriteGenericList(TextWriter writer, List<T> list)
        {
            writer.Write(JsWriter.ListStartChar);

            var ranOnce = false;
            var listLength = list.Count;
            for (var i = 0; i < listLength; i++)
            {
                JsWriter.WriteItemSeperatorIfRanOnce(writer, ref ranOnce);
                ElementWriteFn(writer, list[i]);
            }

            writer.Write(JsWriter.ListEndChar);
        }

        /// <summary>Writes a list value type.</summary>
        /// <param name="writer">The writer.</param>
        /// <param name="list">  The list.</param>
        public static void WriteListValueType(TextWriter writer, object list)
        {
            WriteGenericListValueType(writer, (List<T>)list);
        }

        /// <summary>Writes a generic list value type.</summary>
        /// <param name="writer">The writer.</param>
        /// <param name="list">  The list.</param>
        public static void WriteGenericListValueType(TextWriter writer, List<T> list)
        {
            if (list == null) return; //AOT

            writer.Write(JsWriter.ListStartChar);

            var ranOnce = false;
            var listLength = list.Count;
            for (var i = 0; i < listLength; i++)
            {
                JsWriter.WriteItemSeperatorIfRanOnce(writer, ref ranOnce);
                ElementWriteFn(writer, list[i]);
            }

            writer.Write(JsWriter.ListEndChar);
        }

        /// <summary>Writes an i list.</summary>
        /// <param name="writer">The writer.</param>
        /// <param name="oList"> The list.</param>
        public static void WriteIList(TextWriter writer, object oList)
        {
            WriteGenericIList(writer, (IList<T>)oList);
        }

        /// <summary>Writes a generic i list.</summary>
        /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
        /// <param name="writer">The writer.</param>
        /// <param name="list">  The list.</param>
        public static void WriteGenericIList(TextWriter writer, IList<T> list)
        {
            if (list == null) return;
            writer.Write(JsWriter.ListStartChar);

            var ranOnce = false;
            var listLength = list.Count;
            try
            {
                for (var i = 0; i < listLength; i++)
                {
                    JsWriter.WriteItemSeperatorIfRanOnce(writer, ref ranOnce);
                    ElementWriteFn(writer, list[i]);
                }

            }
            catch (Exception ex)
            {
                Tracer.Instance.WriteError(ex);
                throw;
            }
            writer.Write(JsWriter.ListEndChar);
        }

        /// <summary>Writes an i list value type.</summary>
        /// <param name="writer">The writer.</param>
        /// <param name="list">  The list.</param>
        public static void WriteIListValueType(TextWriter writer, object list)
        {
            WriteGenericIListValueType(writer, (IList<T>)list);
        }

        /// <summary>Writes a generic i list value type.</summary>
        /// <param name="writer">The writer.</param>
        /// <param name="list">  The list.</param>
        public static void WriteGenericIListValueType(TextWriter writer, IList<T> list)
        {
            if (list == null) return; //AOT

            writer.Write(JsWriter.ListStartChar);

            var ranOnce = false;
            var listLength = list.Count;
            for (var i = 0; i < listLength; i++)
            {
                JsWriter.WriteItemSeperatorIfRanOnce(writer, ref ranOnce);
                ElementWriteFn(writer, list[i]);
            }

            writer.Write(JsWriter.ListEndChar);
        }

        /// <summary>Writes an array.</summary>
        /// <param name="writer">     The writer.</param>
        /// <param name="oArrayValue">The array value.</param>
        public static void WriteArray(TextWriter writer, object oArrayValue)
        {
            if (oArrayValue == null) return;
            WriteGenericArray(writer, (Array)oArrayValue);
        }

        /// <summary>Writes a generic array value type.</summary>
        /// <param name="writer">The writer.</param>
        /// <param name="oArray">The array.</param>
        public static void WriteGenericArrayValueType(TextWriter writer, object oArray)
        {
            WriteGenericArrayValueType(writer, (T[])oArray);
        }

        /// <summary>Writes a generic array value type.</summary>
        /// <param name="writer">The writer.</param>
        /// <param name="array"> The array.</param>
        public static void WriteGenericArrayValueType(TextWriter writer, T[] array)
        {
            if (array == null) return;
            writer.Write(JsWriter.ListStartChar);

            var ranOnce = false;
            var arrayLength = array.Length;
            for (var i = 0; i < arrayLength; i++)
            {
                JsWriter.WriteItemSeperatorIfRanOnce(writer, ref ranOnce);
                ElementWriteFn(writer, array[i]);
            }

            writer.Write(JsWriter.ListEndChar);
        }

        /// <summary>Writes a generic array multi dimension.</summary>
        /// <param name="writer"> The writer.</param>
        /// <param name="array">  The array.</param>
        /// <param name="rank">   The rank.</param>
        /// <param name="indices">The indices.</param>
        private static void WriteGenericArrayMultiDimension(TextWriter writer, Array array, int rank, int[] indices)
        {
            var ranOnce = false;
            writer.Write(JsWriter.ListStartChar);
            for (int i = 0; i < array.GetLength(rank); i++)
            {
                JsWriter.WriteItemSeperatorIfRanOnce(writer, ref ranOnce);
                indices[rank] = i;

                if (rank < (array.Rank - 1))
                    WriteGenericArrayMultiDimension(writer, array, rank + 1, indices);
                else
                    ElementWriteFn(writer, array.GetValue(indices));
            }
            writer.Write(JsWriter.ListEndChar);
        }

        /// <summary>Writes a generic array.</summary>
        /// <param name="writer">The writer.</param>
        /// <param name="array"> The array.</param>
        public static void WriteGenericArray(TextWriter writer, Array array)
        {
            WriteGenericArrayMultiDimension(writer, array, 0, new int[array.Rank]);
        }

        /// <summary>Writes an enumerable.</summary>
        /// <param name="writer">     The writer.</param>
        /// <param name="oEnumerable">The enumerable.</param>
        public static void WriteEnumerable(TextWriter writer, object oEnumerable)
        {
            WriteGenericEnumerable(writer, (IEnumerable<T>)oEnumerable);
        }

        /// <summary>Writes a generic enumerable.</summary>
        /// <param name="writer">    The writer.</param>
        /// <param name="enumerable">The enumerable.</param>
        public static void WriteGenericEnumerable(TextWriter writer, IEnumerable<T> enumerable)
        {
            if (enumerable == null) return;
            writer.Write(JsWriter.ListStartChar);

            var ranOnce = false;
            foreach (var value in enumerable)
            {
                JsWriter.WriteItemSeperatorIfRanOnce(writer, ref ranOnce);
                ElementWriteFn(writer, value);
            }

            writer.Write(JsWriter.ListEndChar);
        }

        /// <summary>Writes a generic enumerable value type.</summary>
        /// <param name="writer">    The writer.</param>
        /// <param name="enumerable">The enumerable.</param>
        public static void WriteGenericEnumerableValueType(TextWriter writer, IEnumerable<T> enumerable)
        {
            writer.Write(JsWriter.ListStartChar);

            var ranOnce = false;
            foreach (var value in enumerable)
            {
                JsWriter.WriteItemSeperatorIfRanOnce(writer, ref ranOnce);
                ElementWriteFn(writer, value);
            }

            writer.Write(JsWriter.ListEndChar);
        }
    }

    /// <summary>A write lists.</summary>
    internal static class WriteLists
    {
        /// <summary>Writes a list string.</summary>
        /// <param name="serializer">The serializer.</param>
        /// <param name="writer">    The writer.</param>
        /// <param name="list">      The list.</param>
        public static void WriteListString(ITypeSerializer serializer, TextWriter writer, object list)
        {
            WriteListString(serializer, writer, (List<string>)list);
        }

        /// <summary>Writes a list string.</summary>
        /// <param name="serializer">The serializer.</param>
        /// <param name="writer">    The writer.</param>
        /// <param name="list">      The list.</param>
        public static void WriteListString(ITypeSerializer serializer, TextWriter writer, List<string> list)
        {
            writer.Write(JsWriter.ListStartChar);

            var ranOnce = false;
            foreach (var x in list)
            {
                JsWriter.WriteItemSeperatorIfRanOnce(writer, ref ranOnce);
                serializer.WriteString(writer, x);
            }

            writer.Write(JsWriter.ListEndChar);
        }

        /// <summary>Writes an i list string.</summary>
        /// <param name="serializer">The serializer.</param>
        /// <param name="writer">    The writer.</param>
        /// <param name="list">      The list.</param>
        public static void WriteIListString(ITypeSerializer serializer, TextWriter writer, object list)
        {
            WriteIListString(serializer, writer, (IList<string>)list);
        }

        /// <summary>Writes an i list string.</summary>
        /// <param name="serializer">The serializer.</param>
        /// <param name="writer">    The writer.</param>
        /// <param name="list">      The list.</param>
        public static void WriteIListString(ITypeSerializer serializer, TextWriter writer, IList<string> list)
        {
            writer.Write(JsWriter.ListStartChar);

            var ranOnce = false;
            var listLength = list.Count;
            for (var i = 0; i < listLength; i++)
            {
                JsWriter.WriteItemSeperatorIfRanOnce(writer, ref ranOnce);
                serializer.WriteString(writer, list[i]);
            }

            writer.Write(JsWriter.ListEndChar);
        }

        /// <summary>Writes the bytes.</summary>
        /// <param name="serializer">The serializer.</param>
        /// <param name="writer">    The writer.</param>
        /// <param name="byteValue"> The byte value.</param>
        public static void WriteBytes(ITypeSerializer serializer, TextWriter writer, object byteValue)
        {
            if (byteValue == null) return;
            serializer.WriteBytes(writer, byteValue);
        }

        /// <summary>Writes a string array.</summary>
        /// <param name="serializer">The serializer.</param>
        /// <param name="writer">    The writer.</param>
        /// <param name="oList">     The list.</param>
        public static void WriteStringArray(ITypeSerializer serializer, TextWriter writer, object oList)
        {
            writer.Write(JsWriter.ListStartChar);

            var list = (string[])oList;
            var ranOnce = false;
            var listLength = list.Length;
            for (var i = 0; i < listLength; i++)
            {
                JsWriter.WriteItemSeperatorIfRanOnce(writer, ref ranOnce);
                serializer.WriteString(writer, list[i]);
            }

            writer.Write(JsWriter.ListEndChar);
        }
    }

    /// <summary>A write lists.</summary>
    /// <typeparam name="T">          Generic type parameter.</typeparam>
    /// <typeparam name="TSerializer">Type of the serializer.</typeparam>
    internal static class WriteLists<T, TSerializer>
        where TSerializer : ITypeSerializer
    {
        /// <summary>The cache function.</summary>
        private static readonly WriteObjectDelegate CacheFn;

        /// <summary>The serializer.</summary>
        private static readonly ITypeSerializer Serializer = JsWriter.GetTypeSerializer<TSerializer>();

        /// <summary>
        /// Initializes static members of the NServiceKit.Text.Common.WriteLists&lt;T, TSerializer&gt;
        /// class.
        /// </summary>
        static WriteLists()
        {
            CacheFn = GetWriteFn();
        }

        /// <summary>Gets the write.</summary>
        /// <value>The write.</value>
        public static WriteObjectDelegate Write
        {
            get { return CacheFn; }
        }

        /// <summary>Gets write function.</summary>
        /// <exception cref="ArgumentException">Thrown when one or more arguments have unsupported or
        /// illegal values.</exception>
        /// <returns>The write function.</returns>
        public static WriteObjectDelegate GetWriteFn()
        {
            var type = typeof(T);

            var listInterface = type.GetTypeWithGenericTypeDefinitionOf(typeof(IList<>));
            if (listInterface == null)
                throw new ArgumentException(string.Format("Type {0} is not of type IList<>", type.FullName));

            //optimized access for regularly used types
            if (type == typeof(List<string>))
                return (w, x) => WriteLists.WriteListString(Serializer, w, x);
            if (type == typeof(IList<string>))
                return (w, x) => WriteLists.WriteIListString(Serializer, w, x);

            if (type == typeof(List<int>))
                return WriteListsOfElements<int, TSerializer>.WriteListValueType;
            if (type == typeof(IList<int>))
                return WriteListsOfElements<int, TSerializer>.WriteIListValueType;

            if (type == typeof(List<long>))
                return WriteListsOfElements<long, TSerializer>.WriteListValueType;
            if (type == typeof(IList<long>))
                return WriteListsOfElements<long, TSerializer>.WriteIListValueType;

            var elementType = listInterface.GenericTypeArguments()[0];

            var isGenericList = typeof(T).IsGeneric()
                && typeof(T).GenericTypeDefinition() == typeof(List<>);

            if (elementType.IsValueType()
                && JsWriter.ShouldUseDefaultToStringMethod(elementType))
            {
                if (isGenericList)
                    return WriteListsOfElements<TSerializer>.GetWriteListValueType(elementType);

                return WriteListsOfElements<TSerializer>.GetWriteIListValueType(elementType);
            }

            return isGenericList
                ? WriteListsOfElements<TSerializer>.GetListWriteFn(elementType)
                : WriteListsOfElements<TSerializer>.GetIListWriteFn(elementType);
        }

    }
}