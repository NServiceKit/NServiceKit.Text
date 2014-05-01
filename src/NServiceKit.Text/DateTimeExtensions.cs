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
using System.Globalization;
using NServiceKit.Text.Common;

namespace NServiceKit.Text
{
    /// <summary>A fast, standards-based, serialization-issue free DateTime serailizer.</summary>
    public static class DateTimeExtensions
    {
        /// <summary>The unix epoch.</summary>
        public const long UnixEpoch = 621355968000000000L;

        /// <summary>The unix epoch date time UTC.</summary>
        private static readonly DateTime UnixEpochDateTimeUtc = new DateTime(UnixEpoch, DateTimeKind.Utc);

        /// <summary>The unix epoch date time unspecified.</summary>
        private static readonly DateTime UnixEpochDateTimeUnspecified = new DateTime(UnixEpoch, DateTimeKind.Unspecified);

        /// <summary>The minimum date time UTC.</summary>
        private static readonly DateTime MinDateTimeUtc = new DateTime(1, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>A DateTime extension method that converts a dateTime to an unix time.</summary>
        /// <param name="dateTime">The dateTime to act on.</param>
        /// <returns>dateTime as a long.</returns>
        public static long ToUnixTime(this DateTime dateTime)
        {
            return (dateTime.ToStableUniversalTime().Ticks - UnixEpoch) / TimeSpan.TicksPerSecond;
        }

        /// <summary>
        /// A double extension method that initializes this object from the given from unix time.
        /// </summary>
        /// <param name="unixTime">The unixTime to act on.</param>
        /// <returns>A DateTime.</returns>
        public static DateTime FromUnixTime(this double unixTime)
        {
            return UnixEpochDateTimeUtc + TimeSpan.FromSeconds(unixTime);
        }

        /// <summary>
        /// A DateTime extension method that converts a dateTime to an unix time milliseconds alternate.
        /// </summary>
        /// <param name="dateTime">The dateTime to act on.</param>
        /// <returns>dateTime as a long.</returns>
        public static long ToUnixTimeMsAlt(this DateTime dateTime)
        {
            return (dateTime.ToStableUniversalTime().Ticks - UnixEpoch) / TimeSpan.TicksPerMillisecond;
        }

        /// <summary>The local time zone.</summary>
        private static TimeZoneInfo LocalTimeZone = TimeZoneInfo.Local;

        /// <summary>
        /// A long extension method that converts the ticks to an unix time milliseconds.
        /// </summary>
        /// <param name="dateTime">The dateTime to act on.</param>
        /// <returns>ticks as a long.</returns>
        public static long ToUnixTimeMs(this DateTime dateTime)
        {
            var dtUtc = dateTime;
            if (dateTime.Kind != DateTimeKind.Utc)
            {
                dtUtc = dateTime.Kind == DateTimeKind.Unspecified && dateTime > DateTime.MinValue
                    ? DateTime.SpecifyKind(dateTime.Subtract(LocalTimeZone.GetUtcOffset(dateTime)), DateTimeKind.Utc)
                    : dateTime.ToStableUniversalTime();
            }

            return (long)(dtUtc.Subtract(UnixEpochDateTimeUtc)).TotalMilliseconds;
        }

        /// <summary>
        /// A long extension method that converts the ticks to an unix time milliseconds.
        /// </summary>
        /// <param name="ticks">The ticks to act on.</param>
        /// <returns>ticks as a long.</returns>
        public static long ToUnixTimeMs(this long ticks)
        {
            return (ticks - UnixEpoch) / TimeSpan.TicksPerMillisecond;
        }

        /// <summary>From unix time milliseconds.</summary>
        /// <param name="msSince1970">The milliseconds since 1970.</param>
        /// <returns>A DateTime.</returns>
        public static DateTime FromUnixTimeMs(this double msSince1970)
        {
            return UnixEpochDateTimeUtc + TimeSpan.FromMilliseconds(msSince1970);
        }

        /// <summary>From unix time milliseconds.</summary>
        /// <param name="msSince1970">The milliseconds since 1970.</param>
        /// <returns>A DateTime.</returns>
        public static DateTime FromUnixTimeMs(this long msSince1970)
        {
            return UnixEpochDateTimeUtc + TimeSpan.FromMilliseconds(msSince1970);
        }

        /// <summary>From unix time milliseconds.</summary>
        /// <param name="msSince1970">The milliseconds since 1970.</param>
        /// <param name="offset">     The offset to act on.</param>
        /// <returns>A DateTime.</returns>
        public static DateTime FromUnixTimeMs(this long msSince1970, TimeSpan offset)
        {
            return UnixEpochDateTimeUnspecified + TimeSpan.FromMilliseconds(msSince1970) + offset;
        }

        /// <summary>From unix time milliseconds.</summary>
        /// <param name="msSince1970">The milliseconds since 1970.</param>
        /// <param name="offset">     The offset to act on.</param>
        /// <returns>A DateTime.</returns>
        public static DateTime FromUnixTimeMs(this double msSince1970, TimeSpan offset)
        {
            return UnixEpochDateTimeUnspecified + TimeSpan.FromMilliseconds(msSince1970) + offset;
        }

        /// <summary>From unix time milliseconds.</summary>
        /// <param name="msSince1970">The milliseconds since 1970.</param>
        /// <returns>A DateTime.</returns>
        public static DateTime FromUnixTimeMs(string msSince1970)
        {
            long ms;
            if (long.TryParse(msSince1970, out ms)) return ms.FromUnixTimeMs();

            // Do we really need to support fractional unix time ms time strings??
            return double.Parse(msSince1970).FromUnixTimeMs();
        }

        /// <summary>From unix time milliseconds.</summary>
        /// <param name="msSince1970">The milliseconds since 1970.</param>
        /// <param name="offset">     The offset to act on.</param>
        /// <returns>A DateTime.</returns>
        public static DateTime FromUnixTimeMs(string msSince1970, TimeSpan offset)
        {
            long ms;
            if (long.TryParse(msSince1970, out ms)) return ms.FromUnixTimeMs(offset);

            // Do we really need to support fractional unix time ms time strings??
            return double.Parse(msSince1970).FromUnixTimeMs(offset);
        }

        /// <summary>A DateTime extension method that round to milliseconds.</summary>
        /// <param name="dateTime">The dateTime to act on.</param>
        /// <returns>A DateTime.</returns>
        public static DateTime RoundToMs(this DateTime dateTime)
        {
            return new DateTime((dateTime.Ticks / TimeSpan.TicksPerMillisecond) * TimeSpan.TicksPerMillisecond);
        }

        /// <summary>A DateTime extension method that round to second.</summary>
        /// <param name="dateTime">The dateTime to act on.</param>
        /// <returns>A DateTime.</returns>
        public static DateTime RoundToSecond(this DateTime dateTime)
        {
            return new DateTime((dateTime.Ticks / TimeSpan.TicksPerSecond) * TimeSpan.TicksPerSecond);
        }

        /// <summary>
        /// A DateTime extension method that converts a dateTime to a shortest XSD date time string.
        /// </summary>
        /// <param name="dateTime">The dateTime to act on.</param>
        /// <returns>dateTime as a string.</returns>
        public static string ToShortestXsdDateTimeString(this DateTime dateTime)
        {
            return DateTimeSerializer.ToShortestXsdDateTimeString(dateTime);
        }

        /// <summary>
        /// A string extension method that initializes this object from the given from shortest XSD date
        /// time string.
        /// </summary>
        /// <param name="xsdDateTime">The xsdDateTime to act on.</param>
        /// <returns>A DateTime.</returns>
        public static DateTime FromShortestXsdDateTimeString(this string xsdDateTime)
        {
            return DateTimeSerializer.ParseShortestXsdDateTime(xsdDateTime);
        }

        /// <summary>
        /// A DateTime extension method that query if 'dateTime' is equal to the second.
        /// </summary>
        /// <param name="dateTime">     The dateTime to act on.</param>
        /// <param name="otherDateTime">The other date time.</param>
        /// <returns>true if equal to the second, false if not.</returns>
        public static bool IsEqualToTheSecond(this DateTime dateTime, DateTime otherDateTime)
        {
            return dateTime.ToStableUniversalTime().RoundToSecond().Equals(otherDateTime.ToStableUniversalTime().RoundToSecond());
        }

        /// <summary>
        /// A TimeSpan extension method that converts this object to a time offset string.
        /// </summary>
        /// <param name="offset">   The offset to act on.</param>
        /// <param name="seperator">The seperator.</param>
        /// <returns>The given data converted to a string.</returns>
        public static string ToTimeOffsetString(this TimeSpan offset, string seperator = "")
        {
            var hours = Math.Abs(offset.Hours).ToString(CultureInfo.InvariantCulture);
            var minutes = Math.Abs(offset.Minutes).ToString(CultureInfo.InvariantCulture);
            return (offset < TimeSpan.Zero ? "-" : "+")
                + (hours.Length == 1 ? "0" + hours : hours)
                + seperator
                + (minutes.Length == 1 ? "0" + minutes : minutes);
        }

        /// <summary>
        /// A string extension method that initializes this object from the given from time offset string.
        /// </summary>
        /// <param name="offsetString">The offsetString to act on.</param>
        /// <returns>A TimeSpan.</returns>
        public static TimeSpan FromTimeOffsetString(this string offsetString)
        {
            if (!offsetString.Contains(":"))
                offsetString = offsetString.Insert(offsetString.Length - 2, ":");

            offsetString = offsetString.TrimStart('+');

            return TimeSpan.Parse(offsetString);
        }

        /// <summary>
        /// A DateTime extension method that converts a dateTime to a stable universal time.
        /// </summary>
        /// <param name="dateTime">The dateTime to act on.</param>
        /// <returns>dateTime as a DateTime.</returns>
        public static DateTime ToStableUniversalTime(this DateTime dateTime)
        {
            if (dateTime.Kind == DateTimeKind.Utc)
                return dateTime;
            if (dateTime == DateTime.MinValue)
                return MinDateTimeUtc;

#if SILVERLIGHT
			// Silverlight 3, 4 and 5 all work ok with DateTime.ToUniversalTime, but have no TimeZoneInfo.ConverTimeToUtc implementation.
			return dateTime.ToUniversalTime();
#else
            // .Net 2.0 - 3.5 has an issue with DateTime.ToUniversalTime, but works ok with TimeZoneInfo.ConvertTimeToUtc.
            // .Net 4.0+ does this under the hood anyway.
            return TimeZoneInfo.ConvertTimeToUtc(dateTime);
#endif
        }

        /// <summary>A DateTime extension method that format sortable date.</summary>
        /// <param name="from">from to act on.</param>
        /// <returns>The formatted sortable date.</returns>
        public static string FmtSortableDate(this DateTime from)
        {
            return from.ToString("yyyy-MM-dd");
        }

        /// <summary>A DateTime extension method that format sortable date time.</summary>
        /// <param name="from">from to act on.</param>
        /// <returns>The formatted sortable date time.</returns>
        public static string FmtSortableDateTime(this DateTime from)
        {
            return from.ToString("u");
        }

        /// <summary>A DateTime extension method that last monday.</summary>
        /// <param name="from">from to act on.</param>
        /// <returns>A DateTime.</returns>
        public static DateTime LastMonday(this DateTime from)
        {
            var mondayOfWeek = from.Date.AddDays(-(int)from.DayOfWeek + 1);
            return mondayOfWeek;
        }

        /// <summary>A DateTime extension method that starts of last month.</summary>
        /// <param name="from">from to act on.</param>
        /// <returns>A DateTime.</returns>
        public static DateTime StartOfLastMonth(this DateTime from)
        {
            return new DateTime(from.Date.Year, from.Date.Month, 1).AddMonths(-1);
        }

        /// <summary>A DateTime extension method that ends of last month.</summary>
        /// <param name="from">from to act on.</param>
        /// <returns>A DateTime.</returns>
        public static DateTime EndOfLastMonth(this DateTime from)
        {
            return new DateTime(from.Date.Year, from.Date.Month, 1).AddDays(-1);
        }
    }

}
