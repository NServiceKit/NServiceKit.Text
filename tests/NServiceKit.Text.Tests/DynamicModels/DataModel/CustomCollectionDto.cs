using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace NServiceKit.Text.Tests.DynamicModels.DataModel
{
    /// <summary>A custom collection dto.</summary>
	public class CustomCollectionDto
	{
        /// <summary>Gets or sets the exception.</summary>
        /// <value>The exception.</value>
		public Exception Exception { get; set; }

        /// <summary>Gets or sets details of the exception.</summary>
        /// <value>The custom exception.</value>
		public CustomException CustomException { get; set; }

        /// <summary>Gets or sets URI of the address.</summary>
        /// <value>The address URI.</value>
		public Uri AddressUri { get; set; }

        /// <summary>Gets or sets the type of some.</summary>
        /// <value>The type of some.</value>
		public Type SomeType { get; set; }

        /// <summary>Gets or sets the int value.</summary>
        /// <value>The int value.</value>
		public int IntValue { get; set; }
	}
}