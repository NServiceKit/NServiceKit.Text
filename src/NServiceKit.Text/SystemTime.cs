﻿//
// https://github.com/NServiceKit/NServiceKit.Text
// NServiceKit.Text: .NET C# POCO JSON, JSV and CSV Text Serializers.
//
// Authors:
//   Demis Bellot (demis.bellot@gmail.com)
//   Damian Hickey (dhickey@gmail.com)
//
// Copyright 2012 ServiceStack Ltd.
//
// Licensed under the same terms of ServiceStack: new BSD license.
//

using System;

namespace NServiceKit.Text
{
    /// <summary>A system time.</summary>
	public static class SystemTime
	{
        /// <summary>The UTC date time resolver.</summary>
		public static Func<DateTime> UtcDateTimeResolver;

        /// <summary>Gets the Date/Time of the now.</summary>
        /// <value>The now.</value>
		public static DateTime Now
		{
			get
			{
				var temp = UtcDateTimeResolver;
				return temp == null ? DateTime.Now : temp().ToLocalTime();
			}
		}

        /// <summary>Gets the Date/Time of the UTC now.</summary>
        /// <value>The UTC now.</value>
		public static DateTime UtcNow
		{
			get
			{
				var temp = UtcDateTimeResolver;
				return temp == null ? DateTime.UtcNow : temp();
			}
		}
	}
}
