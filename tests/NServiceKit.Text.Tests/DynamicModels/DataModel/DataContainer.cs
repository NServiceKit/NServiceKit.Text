using System;
using System.Collections.Generic;

namespace NServiceKit.Text.Tests.DynamicModels.DataModel
{
    /// <summary>A data container.</summary>
	[Serializable]
	public sealed class DataContainer : DataContainerBase
	{
        /// <summary>Gets or sets a list of types.</summary>
        /// <value>A List of types.</value>
		public IEnumerable<Type> TypeList { get; set; }

        /// <summary>Gets or sets the exception.</summary>
        /// <value>The exception.</value>
		public Exception Exception { get; set; }

        /// <summary>Gets or sets the object.</summary>
        /// <value>The object.</value>
		public object Object { get; set; }

        /// <summary>Gets or sets the type.</summary>
        /// <value>The type.</value>
		public Type Type { get; set; }
		//public IEnumerable<object> ObjectList { get; set; }
	}
}