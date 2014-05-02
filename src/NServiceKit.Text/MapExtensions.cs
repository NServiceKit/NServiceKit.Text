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

using System.Collections.Generic;
using System.Text;
using NServiceKit.Text.Common;

namespace NServiceKit.Text
{
    /// <summary>A map extensions.</summary>
	public static class MapExtensions
	{
        /// <summary>A Dictionary&lt;K,V&gt; extension method that joins.</summary>
        /// <typeparam name="K">Generic type parameter.</typeparam>
        /// <typeparam name="V">Generic type parameter.</typeparam>
        /// <param name="values">The values to act on.</param>
        /// <returns>A string.</returns>
		public static string Join<K, V>(this Dictionary<K, V> values)
		{
			return Join(values, JsWriter.ItemSeperatorString, JsWriter.MapKeySeperatorString);
		}

        /// <summary>A Dictionary&lt;K,V&gt; extension method that joins.</summary>
        /// <typeparam name="K">Generic type parameter.</typeparam>
        /// <typeparam name="V">Generic type parameter.</typeparam>
        /// <param name="values">       The values to act on.</param>
        /// <param name="itemSeperator">The item seperator.</param>
        /// <param name="keySeperator"> The key seperator.</param>
        /// <returns>A string.</returns>
		public static string Join<K, V>(this Dictionary<K, V> values, string itemSeperator, string keySeperator)
		{
			var sb = new StringBuilder();
			foreach (var entry in values)
			{
				if (sb.Length > 0)
					sb.Append(itemSeperator);

				sb.Append(entry.Key).Append(keySeperator).Append(entry.Value);
			}
			return sb.ToString();
		}
	}
}