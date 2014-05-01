using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace NServiceKit.Text.Tests.DynamicModels.DataModel
{
    /// <summary>Collection of customs.</summary>
	[Serializable]
	public class CustomCollection : Collection<CustomCollectionItem>
	{
        /// <summary>Searches for the first item index.</summary>
        /// <param name="name">The name.</param>
        /// <returns>The found item index.</returns>
		public int FindItemIndex(string name)
		{
			return IndexOf((from item in this
							where item.Name == name
							select item).FirstOrDefault());
		}

        /// <summary>Removes all described by name.</summary>
        /// <param name="name">The name.</param>
		public void RemoveAll(string name)
		{
			while (true)
			{
				var idx = FindItemIndex(name);
				if (idx < 0)
					break;

				RemoveAt(idx);
			}
		}

        /// <summary>Gets or sets URI of the address.</summary>
        /// <value>The address URI.</value>
		public Uri AddressUri
		{
			get
			{
				var idx = FindItemIndex("AddressUri");
				//Cater for value containing a real value or a serialized string value
				//Using 'FromCsvField()' because 'Value' may have escaped chars
				return idx < 0 ? null : 
					(
						this[idx].Value is string
						? new Uri(((string)this[idx].Value).FromCsvField())
						: this[idx].Value as Uri
					);
			}
			set
			{
				RemoveAll("AddressUri");
				Add(new CustomCollectionItem("AddressUri", value));
			}
		}

        /// <summary>Gets or sets the type of some.</summary>
        /// <value>The type of some.</value>
		public Type SomeType
		{
			get
			{
				var idx = FindItemIndex("SomeType");
				//Cater for value containing a real value or a serialized string value
				//Using 'FromCsvField()' because 'Value' may have escaped chars
				return idx < 0 ? null : 
					(
						this[idx].Value is string
                        ? AssemblyUtils.FindType(((string)this[idx].Value).FromCsvField())
						: this[idx].Value as Type
					);
			}
			set
			{
				RemoveAll("SomeType");
				Add(new CustomCollectionItem("SomeType", value));
			}
		}

        /// <summary>Gets or sets the int value.</summary>
        /// <value>The int value.</value>
		public int IntValue
		{
			get
			{
				var idx = FindItemIndex("IntValue");
				//Cater for value containing a real value or a serialized string value
				return idx < 0 ? -1 : 
					(
						this[idx].Value is string
						? int.Parse((string)this[idx].Value)
						: (int)this[idx].Value 
					);
			}
			set
			{
				RemoveAll("IntValue");
				Add(new CustomCollectionItem("IntValue", value));
			}
		}
	}
}