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
using System.Linq;
using System.Text;
using NServiceKit.Text.Common;

namespace NServiceKit.Text
{
    /// <summary>A list extensions.</summary>
	public static class ListExtensions
	{
        /// <summary>An IEnumerable&lt;T&gt; extension method that joins.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="values">The values to act on.</param>
        /// <returns>A string.</returns>
		public static string Join<T>(this IEnumerable<T> values)
		{
			return Join(values, JsWriter.ItemSeperatorString);
		}

        /// <summary>An IEnumerable&lt;T&gt; extension method that joins.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="values">   The values to act on.</param>
        /// <param name="seperator">The seperator.</param>
        /// <returns>A string.</returns>
		public static string Join<T>(this IEnumerable<T> values, string seperator)
		{
			var sb = new StringBuilder();
			foreach (var value in values)
			{
				if (sb.Length > 0)
					sb.Append(seperator);
				sb.Append(value);
			}
			return sb.ToString();
		}

        /// <summary>A List&lt;T&gt; extension method that queries if a null or is empty.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="list">The list to act on.</param>
        /// <returns>true if a null or is t>, false if not.</returns>
		public static bool IsNullOrEmpty<T>(this List<T> list)
		{
			return list == null || list.Count == 0;
		}

        /// <summary>TODO: make it work.</summary>
        /// <typeparam name="TFrom">Type of from.</typeparam>
        /// <param name="list">     The list to act on.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>
        /// An enumerator that allows foreach to be used to process safe where in this collection.
        /// </returns>
		public static IEnumerable<TFrom> SafeWhere<TFrom>(this List<TFrom> list, Func<TFrom, bool> predicate)
		{
			return list.Where(predicate);
		}

        /// <summary>A List&lt;T&gt; extension method that nullable count.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="list">The list to act on.</param>
        /// <returns>An int.</returns>
		public static int NullableCount<T>(this List<T> list)
		{
			return list == null ? 0 : list.Count;
		}

        /// <summary>A List&lt;T&gt; extension method that adds if not exists to 'item'.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="list">The list to act on.</param>
        /// <param name="item">The item.</param>
		public static void AddIfNotExists<T>(this List<T> list, T item)
		{
			if (!list.Contains(item))
				list.Add(item);
		}
	}
}