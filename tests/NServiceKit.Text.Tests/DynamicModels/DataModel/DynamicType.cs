using System;

namespace NServiceKit.Text.Tests.DynamicModels.DataModel
{
    /// <summary>A dynamic type.</summary>
	public class DynamicType
	{
        /// <summary>Gets or sets the name.</summary>
        /// <value>The name.</value>
		public string Name { get; set; }

        /// <summary>Gets or sets the type.</summary>
        /// <value>The type.</value>
		public Type Type { get; set; }

        /// <summary>Gets or sets the value.</summary>
        /// <value>The value.</value>
		public object Value { get; set; }

        /// <summary>Gets typed value.</summary>
        /// <returns>The typed value.</returns>
		public object GetTypedValue()
		{
			var strValue = this.Value as string;
			if (strValue != null)
			{
				var unescapedValue = strValue.FromCsvField();
				return TypeSerializer.DeserializeFromString(unescapedValue, this.Type);
			}
			return Value;
		}
	}
}