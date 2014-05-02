using System;
using System.Linq;
using System.Collections.Generic;
using NServiceKit.Text.Common;
using System.Reflection;

namespace NServiceKit.Text
{
    /// <summary>A CSV configuration.</summary>
    public static class CsvConfig
    {
        /// <summary>Initializes static members of the NServiceKit.Text.CsvConfig class.</summary>
		static CsvConfig()
		{
            Reset();
		}

        /// <summary>The ts item seperator string.</summary>
		[ThreadStatic]
		private static string tsItemSeperatorString;

        /// <summary>The item seperator string.</summary>
		private static string sItemSeperatorString;

        /// <summary>Gets or sets the item seperator string.</summary>
        /// <value>The item seperator string.</value>
		public static string ItemSeperatorString
		{
			get
			{
				return tsItemSeperatorString ?? sItemSeperatorString ?? JsWriter.ItemSeperatorString;
			}
			set
			{
				tsItemSeperatorString = value;
				if (sItemSeperatorString == null) sItemSeperatorString = value;
                ResetEscapeStrings();
			}
		}

        /// <summary>The ts item delimiter string.</summary>
		[ThreadStatic]
		private static string tsItemDelimiterString;

        /// <summary>The item delimiter string.</summary>
		private static string sItemDelimiterString;

        /// <summary>Gets or sets the item delimiter string.</summary>
        /// <value>The item delimiter string.</value>
		public static string ItemDelimiterString
		{
			get
			{
				return tsItemDelimiterString ?? sItemDelimiterString ?? JsWriter.QuoteString;
			}
			set
			{
				tsItemDelimiterString = value;
				if (sItemDelimiterString == null) sItemDelimiterString = value;
			    EscapedItemDelimiterString = value + value;
			    ResetEscapeStrings();
			}
		}

        /// <summary>The default escaped item delimiter string.</summary>
        private const string DefaultEscapedItemDelimiterString = JsWriter.QuoteString + JsWriter.QuoteString;

        /// <summary>The ts escaped item delimiter string.</summary>
        [ThreadStatic]
		private static string tsEscapedItemDelimiterString;

        /// <summary>The escaped item delimiter string.</summary>
		private static string sEscapedItemDelimiterString;

        /// <summary>Gets or sets the escaped item delimiter string.</summary>
        /// <value>The escaped item delimiter string.</value>
		internal static string EscapedItemDelimiterString
		{
			get
			{
				return tsEscapedItemDelimiterString ?? sEscapedItemDelimiterString ?? DefaultEscapedItemDelimiterString;
			}
			set
			{
				tsEscapedItemDelimiterString = value;
				if (sEscapedItemDelimiterString == null) sEscapedItemDelimiterString = value;
			}
		}

        /// <summary>The default escape strings.</summary>
        private static readonly string[] defaultEscapeStrings = GetEscapeStrings();

        /// <summary>The ts escape strings.</summary>
		[ThreadStatic]
		private static string[] tsEscapeStrings;

        /// <summary>The escape strings.</summary>
		private static string[] sEscapeStrings;

        /// <summary>Gets the escape strings.</summary>
        /// <value>The escape strings.</value>
		public static string[] EscapeStrings
		{
			get
			{
				return tsEscapeStrings ?? sEscapeStrings ?? defaultEscapeStrings;
			}
            private set
            {
				tsEscapeStrings = value;
				if (sEscapeStrings == null) sEscapeStrings = value;
            }
		}

        /// <summary>Gets escape strings.</summary>
        /// <returns>An array of string.</returns>
        private static string[] GetEscapeStrings()
        {
            return new[] {ItemDelimiterString, ItemSeperatorString, RowSeparatorString, "\r", "\n"};
        }

        /// <summary>Resets the escape strings.</summary>
        private static void ResetEscapeStrings()
        {
            EscapeStrings = GetEscapeStrings();
        }

        /// <summary>The ts row separator string.</summary>
		[ThreadStatic]
		private static string tsRowSeparatorString;

        /// <summary>The row separator string.</summary>
		private static string sRowSeparatorString;

        /// <summary>Gets or sets the row separator string.</summary>
        /// <value>The row separator string.</value>
		public static string RowSeparatorString
		{
			get
			{
			    return tsRowSeparatorString ?? sRowSeparatorString ?? Environment.NewLine;
			}
		    set
			{
				tsRowSeparatorString = value;
				if (sRowSeparatorString == null) sRowSeparatorString = value;
                ResetEscapeStrings();
			}
		}

        /// <summary>Resets this object.</summary>
		public static void Reset()
		{
			tsItemSeperatorString = sItemSeperatorString = null;
			tsItemDelimiterString = sItemDelimiterString = null;
			tsEscapedItemDelimiterString = sEscapedItemDelimiterString = null;
			tsRowSeparatorString = sRowSeparatorString = null;
		    tsEscapeStrings = sEscapeStrings = null;
		}

    }

    /// <summary>A CSV configuration.</summary>
    /// <typeparam name="T">Generic type parameter.</typeparam>
	public static class CsvConfig<T>
	{
        /// <summary>Gets or sets a value indicating whether the omit headers.</summary>
        /// <value>true if omit headers, false if not.</value>
		public static bool OmitHeaders { get; set; }

        /// <summary>The custom headers map.</summary>
		private static Dictionary<string, string> customHeadersMap;

        /// <summary>Gets or sets the custom headers map.</summary>
        /// <value>The custom headers map.</value>
		public static Dictionary<string, string> CustomHeadersMap
		{
			get
			{
				return customHeadersMap;
			}
			set
			{
				customHeadersMap = value;
				if (value == null) return;
				CsvWriter<T>.ConfigureCustomHeaders(customHeadersMap);
			}
		}

        /// <summary>Sets the custom headers.</summary>
        /// <exception cref="ArgumentException">Thrown when one or more arguments have unsupported or
        /// illegal values.</exception>
        /// <value>The custom headers.</value>
		public static object CustomHeaders
		{
			set
			{
				if (value == null) return;
                if (value.GetType().IsValueType())
                    throw new ArgumentException("CustomHeaders is a ValueType");

                var propertyInfos = value.GetType().GetPropertyInfos();
                if (propertyInfos.Length == 0) return;

				customHeadersMap = new Dictionary<string, string>();
				foreach (var pi in propertyInfos)
				{
                    var getMethod = pi.GetMethodInfo();
                    if (getMethod == null) continue;

					var oValue = getMethod.Invoke(value, new object[0]);
					if (oValue == null) continue;
					customHeadersMap[pi.Name] = oValue.ToString();
				}
				CsvWriter<T>.ConfigureCustomHeaders(customHeadersMap);
			}
		}

        /// <summary>Resets this object.</summary>
		public static void Reset()
		{
			OmitHeaders = false;
			CsvWriter<T>.Reset();
		}
	}
}