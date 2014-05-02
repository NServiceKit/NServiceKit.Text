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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using NServiceKit.Text.Common;

namespace NServiceKit.Text.Jsv
{
    /// <summary>A jsv writer.</summary>
	internal static class JsvWriter
	{
        /// <summary>The instance.</summary>
		public static readonly JsWriter<JsvTypeSerializer> Instance = new JsWriter<JsvTypeSerializer>();

        /// <summary>The write function cache.</summary>
        private static Dictionary<Type, WriteObjectDelegate> WriteFnCache = new Dictionary<Type, WriteObjectDelegate>();

        /// <summary>Removes the cache function described by forType.</summary>
        /// <param name="forType">Type of for.</param>
        internal static void RemoveCacheFn(Type forType)
        {
            Dictionary<Type, WriteObjectDelegate> snapshot, newCache;
            do
            {
                snapshot = WriteFnCache;
                newCache = new Dictionary<Type, WriteObjectDelegate>(WriteFnCache);
                newCache.Remove(forType);
                
            } while (!ReferenceEquals(
                Interlocked.CompareExchange(ref WriteFnCache, newCache, snapshot), snapshot));
        }

        /// <summary>Gets write function.</summary>
        /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
        /// <param name="type">The type.</param>
        /// <returns>The write function.</returns>
		public static WriteObjectDelegate GetWriteFn(Type type)
		{
			try
			{
                WriteObjectDelegate writeFn;
                if (WriteFnCache.TryGetValue(type, out writeFn)) return writeFn;

                var genericType = typeof(JsvWriter<>).MakeGenericType(type);
                var mi = genericType.GetPublicStaticMethod("WriteFn");
                var writeFactoryFn = (Func<WriteObjectDelegate>)mi.MakeDelegate(typeof(Func<WriteObjectDelegate>));

                writeFn = writeFactoryFn();

                Dictionary<Type, WriteObjectDelegate> snapshot, newCache;
                do
                {
                    snapshot = WriteFnCache;
                    newCache = new Dictionary<Type, WriteObjectDelegate>(WriteFnCache);
                    newCache[type] = writeFn;

                } while (!ReferenceEquals(
                    Interlocked.CompareExchange(ref WriteFnCache, newCache, snapshot), snapshot));

                return writeFn;
			}
			catch (Exception ex)
			{
				Tracer.Instance.WriteError(ex);
				throw;
			}
		}

        /// <summary>Writes a late bound object.</summary>
        /// <param name="writer">The writer.</param>
        /// <param name="value"> The value.</param>
		public static void WriteLateBoundObject(TextWriter writer, object value)
		{
			if (value == null) return;
			var type = value.GetType();
			var writeFn = type == typeof(object)
                ? WriteType<object, JsvTypeSerializer>.WriteObjectType
				: GetWriteFn(type);

			var prevState = JsState.IsWritingDynamic;
			JsState.IsWritingDynamic = true;
			writeFn(writer, value);
			JsState.IsWritingDynamic = prevState;
		}

        /// <summary>Gets value type to string method.</summary>
        /// <param name="type">The type.</param>
        /// <returns>The value type to string method.</returns>
		public static WriteObjectDelegate GetValueTypeToStringMethod(Type type)
		{
			return Instance.GetValueTypeToStringMethod(type);
		}
	}

    /// <summary>Implement the serializer using a more static approach.</summary>
    /// <typeparam name="T">.</typeparam>
	internal static class JsvWriter<T>
	{
        /// <summary>The cache function.</summary>
		private static WriteObjectDelegate CacheFn;

        /// <summary>Resets this object.</summary>
        public static void Reset()
        {
            JsvWriter.RemoveCacheFn(typeof(T));
            
            CacheFn = typeof(T) == typeof(object) 
                ? JsvWriter.WriteLateBoundObject 
                : JsvWriter.Instance.GetWriteFn<T>();
        }

        /// <summary>Writes the function.</summary>
        /// <returns>A WriteObjectDelegate.</returns>
		public static WriteObjectDelegate WriteFn()
		{
			return CacheFn ?? WriteObject;
		}

        /// <summary>
        /// Initializes static members of the NServiceKit.Text.Jsv.JsvWriter&lt;T&gt; class.
        /// </summary>
		static JsvWriter()
		{
		    CacheFn = typeof(T) == typeof(object) 
                ? JsvWriter.WriteLateBoundObject 
                : JsvWriter.Instance.GetWriteFn<T>();
		}

        /// <summary>Writes an object.</summary>
        /// <param name="writer">The writer.</param>
        /// <param name="value"> The value.</param>
        public static void WriteObject(TextWriter writer, object value)
        {
#if MONOTOUCH
			if (writer == null) return;
#endif
            try
            {
                if (++JsState.Depth > JsConfig.MaxDepth)
                    return;

                CacheFn(writer, value);
            }
            finally 
            {
                JsState.Depth--;
            }
        }

        /// <summary>Writes a root object.</summary>
        /// <param name="writer">The writer.</param>
        /// <param name="value"> The value.</param>
        public static void WriteRootObject(TextWriter writer, object value)
        {
#if MONOTOUCH
			if (writer == null) return;
#endif
            JsState.Depth = 0;
            CacheFn(writer, value);
        }

    }
}