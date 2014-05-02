//
// https://github.com/NServiceKit/NServiceKit.Text
// NServiceKit.Text: .NET C# POCO JSON, JSV and CSV Text Serializers.
//
// Authors:
//	 Peter Townsend (townsend.pete@gmail.com)
//   Demis Bellot (demis.bellot@gmail.com)
//
// Copyright 2012 ServiceStack Ltd.
//
// Licensed under the same terms of ServiceStack: new BSD license.
//

using System;
using System.Collections.Generic;
using System.Text;
using NServiceKit.Text.Common;

namespace NServiceKit.Text
{
    /// <summary>A jsv formatter.</summary>
	public static class JsvFormatter
	{
        /// <summary>Formats.</summary>
        /// <param name="serializedText">The serialized text.</param>
        /// <returns>The formatted value.</returns>
		public static string Format(string serializedText)
		{
			if (string.IsNullOrEmpty(serializedText)) return null;

			var tabCount = 0;
			var sb = new StringBuilder();
			var firstKeySeparator = true;

			for (var i = 0; i < serializedText.Length; i++)
			{
				var current = serializedText[i];
				var previous = i - 1 >= 0 ? serializedText[i - 1] : 0;
				var next = i < serializedText.Length - 1 ? serializedText[i + 1] : 0;

				if (current == JsWriter.MapStartChar || current == JsWriter.ListStartChar)
				{
					if (previous == JsWriter.MapKeySeperator)
					{
						if (next == JsWriter.MapEndChar || next == JsWriter.ListEndChar)
						{
							sb.Append(current);
							sb.Append(serializedText[++i]); //eat next
							continue;
						}

						AppendTabLine(sb, tabCount);
					}

					sb.Append(current);
					AppendTabLine(sb, ++tabCount);
					firstKeySeparator = true;
					continue;
				}

				if (current == JsWriter.MapEndChar || current == JsWriter.ListEndChar)
				{
					AppendTabLine(sb, --tabCount);
					sb.Append(current);
					firstKeySeparator = true;
					continue;
				}

				if (current == JsWriter.ItemSeperator)
				{
					sb.Append(current);
					AppendTabLine(sb, tabCount);
					firstKeySeparator = true;
					continue;
				}

				sb.Append(current);

				if (current == JsWriter.MapKeySeperator && firstKeySeparator)
				{
					sb.Append(" ");
					firstKeySeparator = false;
				}
			}

			return sb.ToString();
		}

        /// <summary>Appends a tab line.</summary>
        /// <param name="sb">      The sb.</param>
        /// <param name="tabCount">Number of tabs.</param>
		private static void AppendTabLine(StringBuilder sb, int tabCount)
		{
			sb.AppendLine();

			if (tabCount > 0)
			{
				sb.Append(new string('\t', tabCount));
			}
		}
	}
}