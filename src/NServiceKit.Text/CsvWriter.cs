using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Reflection;
using NServiceKit.Text.Common;
using NServiceKit.Text.Reflection;
#if WINDOWS_PHONE && !WP8
using NServiceKit.Text.WP;
#endif

namespace NServiceKit.Text
{
    /// <summary>A CSV dictionary writer.</summary>
    internal class CsvDictionaryWriter
    {
        /// <summary>Writes a row.</summary>
        /// <param name="writer">The writer.</param>
        /// <param name="row">   The row.</param>
        public static void WriteRow(TextWriter writer, IEnumerable<string> row)
        {
            var ranOnce = false;
            foreach (var field in row)
            {
                CsvWriter.WriteItemSeperatorIfRanOnce(writer, ref ranOnce);

                writer.Write(field.ToCsvField());
            }
            writer.Write(CsvConfig.RowSeparatorString);
        }

        /// <summary>Writes an object row.</summary>
        /// <param name="writer">The writer.</param>
        /// <param name="row">   The row.</param>
        public static void WriteObjectRow(TextWriter writer, IEnumerable<object> row)
        {
            var ranOnce = false;
            foreach (var field in row)
            {
                JsWriter.WriteItemSeperatorIfRanOnce(writer, ref ranOnce);

                writer.Write(field.ToCsvField());
            }
            writer.WriteLine();
        }

        /// <summary>Writes.</summary>
        /// <param name="writer"> The writer.</param>
        /// <param name="records">The records.</param>
        public static void Write(TextWriter writer, IEnumerable<Dictionary<string, object>> records)
        {
            if (records == null) return; //AOT

            var requireHeaders = !CsvConfig<Dictionary<string, object>>.OmitHeaders;
            foreach (var record in records)
            {
                if (requireHeaders)
                {
                    WriteRow(writer, record.Keys);
                    requireHeaders = false;
                }
                WriteObjectRow(writer, record.Values);
            }
        }

        /// <summary>Writes.</summary>
        /// <param name="writer"> The writer.</param>
        /// <param name="records">The records.</param>
        public static void Write(TextWriter writer, IEnumerable<Dictionary<string, string>> records)
        {
            if (records == null) return; //AOT

            var allKeys = new HashSet<string>();
            var cachedRecords = new List<IDictionary<string, string>>();

            foreach (var record in records)
            {
                foreach (var key in record.Keys)
                {
                    if (!allKeys.Contains(key))
                    {
                        allKeys.Add(key);
                    }
                }
                cachedRecords.Add(record);
            }

            var headers = allKeys.OrderBy(key => key).ToList();
            if (!CsvConfig<Dictionary<string, string>>.OmitHeaders)
            {
                WriteRow(writer, headers);
            }
            foreach (var cachedRecord in cachedRecords)
            {
                var fullRecord = headers.ConvertAll(header => 
                    cachedRecord.ContainsKey(header) ? cachedRecord[header] : null);
                WriteRow(writer, fullRecord);
            }
        }
    }

    /// <summary>A CSV writer.</summary>
    public static class CsvWriter
    {
        /// <summary>Query if 'value' has any escape characters.</summary>
        /// <param name="value">The value.</param>
        /// <returns>true if any escape characters, false if not.</returns>
        public static bool HasAnyEscapeChars(string value)
        {
            return CsvConfig.EscapeStrings.Any(value.Contains);
        }

        /// <summary>Writes an item seperator if ran once.</summary>
        /// <param name="writer"> The writer.</param>
        /// <param name="ranOnce">The ran once.</param>
        internal static void WriteItemSeperatorIfRanOnce(TextWriter writer, ref bool ranOnce)
        {
            if (ranOnce)
                writer.Write(CsvConfig.ItemSeperatorString);
            else
                ranOnce = true;
        }
    }

    /// <summary>A CSV writer.</summary>
    /// <typeparam name="T">Generic type parameter.</typeparam>
    internal class CsvWriter<T>
    {
        /// <summary>The delimiter character.</summary>
        public const char DelimiterChar = ',';

        /// <summary>Gets or sets the headers.</summary>
        /// <value>The headers.</value>
        public static List<string> Headers { get; set; }

        /// <summary>The property getters.</summary>
        internal static List<Func<T, object>> PropertyGetters;

        /// <summary>The optimized writer.</summary>
        private static readonly WriteObjectDelegate OptimizedWriter;

        /// <summary>
        /// Initializes static members of the NServiceKit.Text.CsvWriter&lt;T&gt; class.
        /// </summary>
        static CsvWriter()
        {
            if (typeof(T) == typeof(string))
            {
                OptimizedWriter = (w, o) => WriteRow(w, (IEnumerable<string>)o);
                return;
            }

            Reset();
        }

        /// <summary>Resets this object.</summary>
        internal static void Reset()
        {
            Headers = new List<string>();

            PropertyGetters = new List<Func<T, object>>();
            var isDataContract = typeof(T).IsDto();
            foreach (var propertyInfo in TypeConfig<T>.Properties)
            {
                if (!propertyInfo.CanRead || propertyInfo.GetMethodInfo() == null) continue;
                if (!TypeSerializer.CanCreateFromString(propertyInfo.PropertyType)) continue;

                PropertyGetters.Add(propertyInfo.GetValueGetter<T>());
                var propertyName = propertyInfo.Name;
                if (isDataContract)
                {
                    var dcsDataMember = propertyInfo.GetDataMember();
                    if (dcsDataMember != null && dcsDataMember.Name != null)
                    {
                        propertyName = dcsDataMember.Name;
                    }
                }
                Headers.Add(propertyName);
            }
        }

        /// <summary>Configure custom headers.</summary>
        /// <param name="customHeadersMap">The custom headers map.</param>
        internal static void ConfigureCustomHeaders(Dictionary<string, string> customHeadersMap)
        {
            Reset();

            for (var i = Headers.Count - 1; i >= 0; i--)
            {
                var oldHeader = Headers[i];
                string newHeaderValue;
                if (!customHeadersMap.TryGetValue(oldHeader, out newHeaderValue))
                {
                    Headers.RemoveAt(i);
                    PropertyGetters.RemoveAt(i);
                }
                else
                {
                    Headers[i] = newHeaderValue.EncodeJsv();
                }
            }
        }

        /// <summary>Gets single row.</summary>
        /// <param name="records">   The records.</param>
        /// <param name="recordType">Type of the record.</param>
        /// <returns>The single row.</returns>
        private static List<string> GetSingleRow(IEnumerable<T> records, Type recordType)
        {
            var row = new List<string>();
            foreach (var value in records)
            {
                var strValue = recordType == typeof(string)
                   ? value as string
                   : TypeSerializer.SerializeToString(value);

                row.Add(strValue);
            }
            return row;
        }

        /// <summary>Gets the rows.</summary>
        /// <param name="records">The records.</param>
        /// <returns>The rows.</returns>
        public static List<List<string>> GetRows(IEnumerable<T> records)
        {
            var rows = new List<List<string>>();

            if (records == null) return rows;

            if (typeof(T).IsValueType() || typeof(T) == typeof(string))
            {
                rows.Add(GetSingleRow(records, typeof(T)));
                return rows;
            }

            foreach (var record in records)
            {
                var row = new List<string>();
                foreach (var propertyGetter in PropertyGetters)
                {
                    var value = propertyGetter(record) ?? "";

                    var strValue = value.GetType() == typeof(string)
                        ? (string)value
                        : TypeSerializer.SerializeToString(value);

                    row.Add(strValue);
                }
                rows.Add(row);
            }

            return rows;
        }

        /// <summary>Writes an object.</summary>
        /// <param name="writer"> The writer.</param>
        /// <param name="records">The records.</param>
        public static void WriteObject(TextWriter writer, object records)
        {
            Write(writer, (IEnumerable<T>)records);
        }

        /// <summary>Writes an object row.</summary>
        /// <param name="writer">The writer.</param>
        /// <param name="record">The record.</param>
        public static void WriteObjectRow(TextWriter writer, object record)
        {
            WriteRow(writer, (T)record);
        }

        /// <summary>Writes.</summary>
        /// <param name="writer"> The writer.</param>
        /// <param name="records">The records.</param>
        public static void Write(TextWriter writer, IEnumerable<T> records)
        {
            if (writer == null) return; //AOT

            if (typeof(T) == typeof(Dictionary<string, string>))
            {
                CsvDictionaryWriter.Write(writer, (IEnumerable<Dictionary<string, string>>)records);
                return;
            }

            if (typeof(T) == typeof(Dictionary<string, object>))
            {
                CsvDictionaryWriter.Write(writer, (IEnumerable<Dictionary<string, object>>)records);
                return;
            }

            if (OptimizedWriter != null)
            {
                OptimizedWriter(writer, records);
                return;
            }

            if (!CsvConfig<T>.OmitHeaders && Headers.Count > 0)
            {
                var ranOnce = false;
                foreach (var header in Headers)
                {
                    CsvWriter.WriteItemSeperatorIfRanOnce(writer, ref ranOnce);

                    writer.Write(header);
                }
                writer.Write(CsvConfig.RowSeparatorString);
            }

            if (records == null) return;

            if (typeof(T).IsValueType() || typeof(T) == typeof(string))
            {
                var singleRow = GetSingleRow(records, typeof(T));
                WriteRow(writer, singleRow);
                return;
            }

            var row = new string[Headers.Count];
            foreach (var record in records)
            {
                for (var i = 0; i < PropertyGetters.Count; i++)
                {
                    var propertyGetter = PropertyGetters[i];
                    var value = propertyGetter(record) ?? "";

                    var strValue = value.GetType() == typeof(string)
                       ? (string)value
                       : TypeSerializer.SerializeToString(value);

                    row[i] = strValue;
                }
                WriteRow(writer, row);
            }
        }

        /// <summary>Writes a row.</summary>
        /// <param name="writer">The writer.</param>
        /// <param name="row">   The row.</param>
        public static void WriteRow(TextWriter writer, T row)
        {
            if (writer == null) return; //AOT

            Write(writer, new[] { row });
        }

        /// <summary>Writes a row.</summary>
        /// <param name="writer">The writer.</param>
        /// <param name="row">   The row.</param>
        public static void WriteRow(TextWriter writer, IEnumerable<string> row)
        {
            var ranOnce = false;
            foreach (var field in row)
            {
                CsvWriter.WriteItemSeperatorIfRanOnce(writer, ref ranOnce);

                writer.Write(field.ToCsvField());
            }
            writer.WriteLine();
        }

        /// <summary>Writes.</summary>
        /// <param name="writer">The writer.</param>
        /// <param name="rows">  The rows.</param>
        public static void Write(TextWriter writer, IEnumerable<List<string>> rows)
        {
            if (Headers.Count > 0)
            {
                var ranOnce = false;
                foreach (var header in Headers)
                {
                    CsvWriter.WriteItemSeperatorIfRanOnce(writer, ref ranOnce);

                    writer.Write(header);
                }
                writer.Write(CsvConfig.RowSeparatorString);
            }

            foreach (var row in rows)
            {
                WriteRow(writer, row);
            }
        }
    }

}