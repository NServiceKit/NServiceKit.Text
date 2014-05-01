using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NServiceKit.Text.Tests.DynamicModels.DataModel
{
    /// <summary>A custom collection item.</summary>
	[Serializable]
	public class CustomCollectionItem
	{
        /// <summary>
        /// Initializes a new instance of the
        /// NServiceKit.Text.Tests.DynamicModels.DataModel.CustomCollectionItem class.
        /// </summary>
		public CustomCollectionItem()
		{}

        /// <summary>
        /// Initializes a new instance of the
        /// NServiceKit.Text.Tests.DynamicModels.DataModel.CustomCollectionItem class.
        /// </summary>
        /// <param name="name"> The name.</param>
        /// <param name="value">The value.</param>
		public CustomCollectionItem(string name, object value)
		{
			Name = name;
			Value = value;
		}

        /// <summary>Gets or sets the name.</summary>
        /// <value>The name.</value>
		public string Name { get; set; }

        /// <summary>Gets or sets the value.</summary>
        /// <value>The value.</value>
		public object Value { get; set; }

        /// <summary>
        /// Returns a <see cref="T:System.String" /> that represents the current
        /// <see cref="T:System.Object" />.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
        /// </returns>
		public override string ToString()
		{
			return string.Concat("Name = '", Name, "' Value = '", Value, "'");
		}
	}
}