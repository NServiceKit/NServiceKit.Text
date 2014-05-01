using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using NServiceKit.Text.Common;
using NServiceKit.Text.Jsv;
using NServiceKit.Text.Reflection;

namespace NServiceKit.Text
{
    /// <summary>A CSV serializer.</summary>
	public class CsvSerializer
	{
        /// <summary>The UTF 8 encoding without bom.</summary>
		private static readonly UTF8Encoding UTF8EncodingWithoutBom = new UTF8Encoding(false);

        /// <summary>The write function cache.</summary>
		private static Dictionary<Type, WriteObjectDelegate> WriteFnCache = new Dictionary<Type, WriteObjectDelegate>();

        /// <summary>Gets write function.</summary>
        /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
        /// <param name="type">The type.</param>
        /// <returns>The write function.</returns>
		internal static WriteObjectDelegate GetWriteFn(Type type)
		{
			try
			{
				WriteObjectDelegate writeFn;
                if (WriteFnCache.TryGetValue(type, out writeFn)) return writeFn;

                var genericType = typeof(CsvSerializer<>).MakeGenericType(type);
                var mi = genericType.GetPublicStaticMethod("WriteFn");
                var writeFactoryFn = (Func<WriteObjectDelegate>)mi.MakeDelegate(
                    typeof(Func<WriteObjectDelegate>));

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

        /// <summary>Serialize to CSV.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="records">The records.</param>
        /// <returns>A string.</returns>
		public static string SerializeToCsv<T>(IEnumerable<T> records)
		{
			var sb = new StringBuilder();
			using (var writer = new StringWriter(sb, CultureInfo.InvariantCulture))
			{
				writer.WriteCsv(records);
				return sb.ToString();
			}
		}

        /// <summary>Serialize to string.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="value">The value.</param>
        /// <returns>A string.</returns>
		public static string SerializeToString<T>(T value)
		{
			if (value == null) return null;
			if (typeof(T) == typeof(string)) return value as string;

			var sb = new StringBuilder();
			using (var writer = new StringWriter(sb, CultureInfo.InvariantCulture))
			{
				CsvSerializer<T>.WriteObject(writer, value);
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
			CsvSerializer<T>.WriteObject(writer, value);
		}

        /// <summary>Serialize to stream.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="value"> The value.</param>
        /// <param name="stream">The stream.</param>
		public static void SerializeToStream<T>(T value, Stream stream)
		{
			if (value == null) return;
		    var writer = new StreamWriter(stream, UTF8EncodingWithoutBom);
			CsvSerializer<T>.WriteObject(writer, value);
            writer.Flush();
		}

        /// <summary>Serialize to stream.</summary>
        /// <param name="obj">   The object.</param>
        /// <param name="stream">The stream.</param>
		public static void SerializeToStream(object obj, Stream stream)
		{
			if (obj == null) return;
		    var writer = new StreamWriter(stream, UTF8EncodingWithoutBom);
            var writeFn = GetWriteFn(obj.GetType());
            writeFn(writer, obj);
            writer.Flush();
        }

        /// <summary>Deserialize from stream.</summary>
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="stream">The stream.</param>
        /// <returns>A T.</returns>
		public static T DeserializeFromStream<T>(Stream stream)
		{
            throw new NotImplementedException();
		}

        /// <summary>Deserialize from stream.</summary>
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        /// <param name="type">  The type.</param>
        /// <param name="stream">The stream.</param>
        /// <returns>An object.</returns>
		public static object DeserializeFromStream(Type type, Stream stream)
		{
            throw new NotImplementedException();
		}

        /// <summary>Writes a late bound object.</summary>
        /// <param name="writer">The writer.</param>
        /// <param name="value"> The value.</param>
		public static void WriteLateBoundObject(TextWriter writer, object value)
		{
			if (value == null) return;
			var writeFn = GetWriteFn(value.GetType());
			writeFn(writer, value);
		}
	}

    /// <summary>A CSV serializer.</summary>
    /// <typeparam name="T">Generic type parameter.</typeparam>
	internal static class CsvSerializer<T>
	{
        /// <summary>The cache function.</summary>
		private static readonly WriteObjectDelegate CacheFn;

        /// <summary>Writes the function.</summary>
        /// <returns>A WriteObjectDelegate.</returns>
		public static WriteObjectDelegate WriteFn()
		{
			return CacheFn;
		}

        /// <summary>The ignore response status.</summary>
		private const string IgnoreResponseStatus = "ResponseStatus";

        /// <summary>The value getter.</summary>
		private static Func<object, object> valueGetter = null;

        /// <summary>The write element function.</summary>
		private static WriteObjectDelegate writeElementFn = null;

        /// <summary>Gets write function.</summary>
        /// <returns>The write function.</returns>
		private static WriteObjectDelegate GetWriteFn()
		{
			PropertyInfo firstCandidate = null;
			Type bestCandidateEnumerableType = null;
			PropertyInfo bestCandidate = null;

            if (typeof(T).IsValueType())
            {
				return JsvWriter<T>.WriteObject;
			}

			//If type is an enumerable property itself write that
			bestCandidateEnumerableType = typeof(T).GetTypeWithGenericTypeDefinitionOf(typeof(IEnumerable<>));
			if (bestCandidateEnumerableType != null)
			{
                var elementType = bestCandidateEnumerableType.GenericTypeArguments()[0];
                writeElementFn = CreateWriteFn(elementType);

				return WriteEnumerableType;
			}

			//Look for best candidate property if DTO
			if (typeof(T).IsDto() || typeof(T).HasAttr<CsvAttribute>())
			{
				var properties = TypeConfig<T>.Properties;
				foreach (var propertyInfo in properties)
				{
					if (propertyInfo.Name == IgnoreResponseStatus) continue;

					if (propertyInfo.PropertyType == typeof(string)
                        || propertyInfo.PropertyType.IsValueType()
                        || propertyInfo.PropertyType == typeof(byte[])) continue;

					if (firstCandidate == null)
					{
						firstCandidate = propertyInfo;
					}

					var enumProperty = propertyInfo.PropertyType
						.GetTypeWithGenericTypeDefinitionOf(typeof(IEnumerable<>));

					if (enumProperty != null)
					{
						bestCandidateEnumerableType = enumProperty;
						bestCandidate = propertyInfo;
						break;
					}
				}
			}

			//If is not DTO or no candidates exist, write self
			var noCandidatesExist = bestCandidate == null && firstCandidate == null;
			if (noCandidatesExist)
			{
				return WriteSelf;
			}

			//If is DTO and has an enumerable property serialize that
			if (bestCandidateEnumerableType != null)
			{
				valueGetter = bestCandidate.GetValueGetter(typeof(T));
                var elementType = bestCandidateEnumerableType.GenericTypeArguments()[0];
                writeElementFn = CreateWriteFn(elementType);

				return WriteEnumerableProperty;
			}

			//If is DTO and has non-enumerable, reference type property serialize that
			valueGetter = firstCandidate.GetValueGetter(typeof(T));
			writeElementFn = CreateWriteRowFn(firstCandidate.PropertyType);

			return WriteNonEnumerableType;
		}

        /// <summary>Creates write function.</summary>
        /// <param name="elementType">Type of the element.</param>
        /// <returns>The new write function.</returns>
		private static WriteObjectDelegate CreateWriteFn(Type elementType)
		{
			return CreateCsvWriterFn(elementType, "WriteObject");
		}

        /// <summary>Creates write row function.</summary>
        /// <param name="elementType">Type of the element.</param>
        /// <returns>The new write row function.</returns>
		private static WriteObjectDelegate CreateWriteRowFn(Type elementType)
		{
			return CreateCsvWriterFn(elementType, "WriteObjectRow");
		}

        /// <summary>Creates CSV writer function.</summary>
        /// <param name="elementType">Type of the element.</param>
        /// <param name="methodName"> Name of the method.</param>
        /// <returns>The new CSV writer function.</returns>
		private static WriteObjectDelegate CreateCsvWriterFn(Type elementType, string methodName)
		{
			var genericType = typeof(CsvWriter<>).MakeGenericType(elementType);
            var mi = genericType.GetPublicStaticMethod(methodName);
            var writeFn = (WriteObjectDelegate)mi.MakeDelegate(typeof(WriteObjectDelegate));
            return writeFn;
        }

        /// <summary>Writes an enumerable type.</summary>
        /// <param name="writer">The writer.</param>
        /// <param name="obj">   The object.</param>
		public static void WriteEnumerableType(TextWriter writer, object obj)
		{
			writeElementFn(writer, obj);
		}

        /// <summary>Writes a self.</summary>
        /// <param name="writer">The writer.</param>
        /// <param name="obj">   The object.</param>
		public static void WriteSelf(TextWriter writer, object obj)
		{
			CsvWriter<T>.WriteRow(writer, (T)obj);
		}

        /// <summary>Writes an enumerable property.</summary>
        /// <param name="writer">The writer.</param>
        /// <param name="obj">   The object.</param>
		public static void WriteEnumerableProperty(TextWriter writer, object obj)
		{
			if (obj == null) return; //AOT

			var enumerableProperty = valueGetter(obj);
			writeElementFn(writer, enumerableProperty);
		}

        /// <summary>Writes a non enumerable type.</summary>
        /// <param name="writer">The writer.</param>
        /// <param name="obj">   The object.</param>
		public static void WriteNonEnumerableType(TextWriter writer, object obj)
		{
			var nonEnumerableType = valueGetter(obj);
			writeElementFn(writer, nonEnumerableType);
		}

        /// <summary>
        /// Initializes static members of the NServiceKit.Text.CsvSerializer&lt;T&gt; class.
        /// </summary>
		static CsvSerializer()
		{
			if (typeof(T) == typeof(object))
			{
				CacheFn = CsvSerializer.WriteLateBoundObject;
			}
			else
			{
				CacheFn = GetWriteFn();
			}
		}

        /// <summary>Writes an object.</summary>
        /// <param name="writer">The writer.</param>
        /// <param name="value"> The value.</param>
		public static void WriteObject(TextWriter writer, object value)
		{
			CacheFn(writer, value);
		}
	}
}