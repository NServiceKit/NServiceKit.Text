using System;

namespace NServiceKit.Text.Tests.DynamicModels.DataModel
{
    /// <summary>A strict type.</summary>
	public class StrictType
	{
        /// <summary>Gets or sets the name.</summary>
        /// <value>The name.</value>
		public string Name { get; set; }

        /// <summary>Gets or sets the type.</summary>
        /// <value>The type.</value>
		public Type Type { get; set; }

        /// <summary>Gets or sets the value.</summary>
        /// <value>The value.</value>
		public CustomCollectionDto Value { get; set; }
	}
}