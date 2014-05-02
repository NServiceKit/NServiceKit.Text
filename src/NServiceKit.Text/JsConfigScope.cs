using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace NServiceKit.Text
{
    /// <summary>The js configuration scope.</summary>
    public sealed class JsConfigScope : IDisposable
    {
        /// <summary>true if disposed.</summary>
        bool disposed;

        /// <summary>The parent.</summary>
        JsConfigScope parent;

        /// <summary>The head.</summary>
        [ThreadStatic]
        private static JsConfigScope head;

        /// <summary>Initializes a new instance of the NServiceKit.Text.JsConfigScope class.</summary>
        internal JsConfigScope()
        {
#if !SILVERLIGHT
            Thread.BeginThreadAffinity();
#endif
            parent = head;
            head = this;
        }

        /// <summary>Gets the current.</summary>
        /// <value>The current.</value>
        internal static JsConfigScope Current
        {
            get
            {
                return head;
            }
        }

        /// <summary>Dispose current.</summary>
        public static void DisposeCurrent()
        {
            if (head != null)
            {
                head.Dispose();
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
        /// resources.
        /// </summary>
        public void Dispose()
        {
            if (!disposed)
            {
                disposed = true;

                Debug.Assert(this == head, "Disposed out of order.");

                head = parent;
#if !SILVERLIGHT
                Thread.EndThreadAffinity();
#endif
            }
        }

        /// <summary>Gets or sets a dictionary of convert object types into strings.</summary>
        /// <value>A Dictionary of convert object types into strings.</value>
        public bool? ConvertObjectTypesIntoStringDictionary { get; set; }

        /// <summary>Gets or sets the try to parse primitive type values.</summary>
        /// <value>The try to parse primitive type values.</value>
        public bool? TryToParsePrimitiveTypeValues { get; set; }

        /// <summary>Gets or sets the type of the try to parse numeric.</summary>
        /// <value>The type of the try to parse numeric.</value>
		public bool? TryToParseNumericType { get; set; }

        /// <summary>Gets or sets the include null values.</summary>
        /// <value>The include null values.</value>
        public bool? IncludeNullValues { get; set; }

        /// <summary>Gets or sets the treat enum as integer.</summary>
        /// <value>The treat enum as integer.</value>
        public bool? TreatEnumAsInteger { get; set; }

        /// <summary>Gets or sets information describing the exclude type.</summary>
        /// <value>Information describing the exclude type.</value>
        public bool? ExcludeTypeInfo { get; set; }

        /// <summary>Gets or sets information describing the include type.</summary>
        /// <value>Information describing the include type.</value>
        public bool? IncludeTypeInfo { get; set; }

        /// <summary>Gets or sets the type attribute.</summary>
        /// <value>The type attribute.</value>
        public string TypeAttr { get; set; }

        /// <summary>Gets or sets the JSON type attribute in object.</summary>
        /// <value>The JSON type attribute in object.</value>
        internal string JsonTypeAttrInObject { get; set; }

        /// <summary>Gets or sets the jsv type attribute in object.</summary>
        /// <value>The jsv type attribute in object.</value>
        internal string JsvTypeAttrInObject { get; set; }

        /// <summary>Gets or sets the type writer.</summary>
        /// <value>The type writer.</value>
        public Func<Type, string> TypeWriter { get; set; }

        /// <summary>Gets or sets the type finder.</summary>
        /// <value>The type finder.</value>
        public Func<string, Type> TypeFinder { get; set; }

        /// <summary>Gets or sets the date handler.</summary>
        /// <value>The date handler.</value>
        public JsonDateHandler? DateHandler { get; set; }

        /// <summary>Gets or sets the time span handler.</summary>
        /// <value>The time span handler.</value>
        public JsonTimeSpanHandler? TimeSpanHandler { get; set; }

        /// <summary>Gets or sets a list of names of the emit camel cases.</summary>
        /// <value>A list of names of the emit camel cases.</value>
        public bool? EmitCamelCaseNames { get; set; }

        /// <summary>Gets or sets a list of names of the emit lowercase underscores.</summary>
        /// <value>A list of names of the emit lowercase underscores.</value>
        public bool? EmitLowercaseUnderscoreNames { get; set; }

        /// <summary>Gets or sets the throw on deserialization error.</summary>
        /// <value>The throw on deserialization error.</value>
        public bool? ThrowOnDeserializationError { get; set; }

        /// <summary>Gets or sets the always use UTC.</summary>
        /// <value>The always use UTC.</value>
        public bool? AlwaysUseUtc { get; set; }

        /// <summary>Gets or sets the assume UTC.</summary>
        /// <value>The assume UTC.</value>
        public bool? AssumeUtc { get; set; }

        /// <summary>Gets or sets the append UTC offset.</summary>
        /// <value>The append UTC offset.</value>
        public bool? AppendUtcOffset { get; set; }

        /// <summary>Gets or sets the escape unicode.</summary>
        /// <value>The escape unicode.</value>
        public bool? EscapeUnicode { get; set; }

        /// <summary>Gets or sets the prefer interfaces.</summary>
        /// <value>The prefer interfaces.</value>
        public bool? PreferInterfaces { get; set; }

        /// <summary>Gets or sets the include public fields.</summary>
        /// <value>The include public fields.</value>
        public bool? IncludePublicFields { get; set; }

        /// <summary>Gets or sets the depth of the maximum.</summary>
        /// <value>The depth of the maximum.</value>
        public int? MaxDepth { get; set; }

        /// <summary>Gets or sets the model factory.</summary>
        /// <value>The model factory.</value>
        public EmptyCtorFactoryDelegate ModelFactory { get; set; }

        /// <summary>Gets or sets the exclude property references.</summary>
        /// <value>The exclude property references.</value>
        public string[] ExcludePropertyReferences { get; set; }
    }
}
