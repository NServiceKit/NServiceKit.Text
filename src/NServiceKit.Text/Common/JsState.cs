using System;
using System.Collections.Generic;

namespace NServiceKit.Text.Common
{
    /// <summary>The js state.</summary>
    internal static class JsState
    {
        /// <summary>Exposing field for perf.</summary>
        [ThreadStatic]
        internal static int WritingKeyCount = 0;

        /// <summary>The is writing value.</summary>
        [ThreadStatic]
        internal static bool IsWritingValue = false;

        /// <summary>The is writing dynamic.</summary>
        [ThreadStatic]
        internal static bool IsWritingDynamic = false;

        /// <summary>The query string mode.</summary>
        [ThreadStatic]
        internal static bool QueryStringMode = false;

        /// <summary>The depth.</summary>
        [ThreadStatic]
        internal static int Depth = 0;
    }
}