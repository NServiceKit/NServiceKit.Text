using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NServiceKit.Text.Tests.DynamicModels.DataModel
{
    /// <summary>A data container base.</summary>
	[Serializable]
	public abstract class DataContainerBase
	{
        /// <summary>Gets or sets the identifier.</summary>
        /// <value>The identifier.</value>
		public Guid Identifier { get; set; }

	}
}