
using System;

namespace NServiceKit.Text
{
    /// <summary>An environment.</summary>
	public static class Env
	{
        /// <summary>Initializes static members of the NServiceKit.Text.Env class.</summary>
		static Env()
		{
		    string platformName = null;

#if NETFX_CORE
            IsWinRT = true;
            platformName = "WinRT";
#else
            var platform = (int)Environment.OSVersion.Platform;
			IsUnix = (platform == 4) || (platform == 6) || (platform == 128);
		    platformName = Environment.OSVersion.Platform.ToString();
#endif

            IsMono = AssemblyUtils.FindType("Mono.Runtime") != null;

            IsMonoTouch = AssemblyUtils.FindType("MonoTouch.Foundation.NSObject") != null;

            IsWinRT = AssemblyUtils.FindType("Windows.ApplicationModel") != null;

			SupportsExpressions = SupportsEmit = !IsMonoTouch;

            ServerUserAgent = "NServiceKit/" +
                NServiceKitVersion + " "
                + platformName
                + (IsMono ? "/Mono" : "/.NET")
                + (IsMonoTouch ? " MonoTouch" : "")
                + (IsWinRT ? ".NET WinRT" : "");
		}

        /// <summary>The service kit version.</summary>
		public static decimal NServiceKitVersion = 3.960m;

        /// <summary>Gets or sets a value indicating whether this object is unix.</summary>
        /// <value>true if this object is unix, false if not.</value>
		public static bool IsUnix { get; set; }

        /// <summary>Gets or sets a value indicating whether this object is mono.</summary>
        /// <value>true if this object is mono, false if not.</value>
		public static bool IsMono { get; set; }

        /// <summary>Gets or sets a value indicating whether this object is mono touch.</summary>
        /// <value>true if this object is mono touch, false if not.</value>
		public static bool IsMonoTouch { get; set; }

        /// <summary>Gets or sets a value indicating whether this object is window right.</summary>
        /// <value>true if this object is window right, false if not.</value>
		public static bool IsWinRT { get; set; }

        /// <summary>Gets or sets a value indicating whether the supports expressions.</summary>
        /// <value>true if supports expressions, false if not.</value>
		public static bool SupportsExpressions { get; set; }

        /// <summary>Gets or sets a value indicating whether the supports emit.</summary>
        /// <value>true if supports emit, false if not.</value>
		public static bool SupportsEmit { get; set; }

        /// <summary>Gets or sets the server user agent.</summary>
        /// <value>The server user agent.</value>
		public static string ServerUserAgent { get; set; }
	}
}