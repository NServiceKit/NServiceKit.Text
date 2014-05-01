using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace NServiceKit.Text.Tests.DynamicModels.DataModel
{
    /// <summary>An object graph.</summary>
	[Serializable]
	public class ObjectGraph : ISerializable
	{
        /// <summary>Collection of internals.</summary>
		private readonly CustomCollection internalCollection;

        /// <summary>
        /// Initializes a new instance of the NServiceKit.Text.Tests.DynamicModels.DataModel.ObjectGraph
        /// class.
        /// </summary>
		public ObjectGraph()
		{
			internalCollection = new CustomCollection();
		}

        /// <summary>
        /// Initializes a new instance of the NServiceKit.Text.Tests.DynamicModels.DataModel.ObjectGraph
        /// class.
        /// </summary>
        /// <param name="info">   The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> to
        /// populate with data.</param>
        /// <param name="context">The destination (see
        /// <see cref="T:System.Runtime.Serialization.StreamingContext" />) for this serialization.</param>
		protected ObjectGraph(SerializationInfo info, StreamingContext context)
		{
			internalCollection = (CustomCollection)info.GetValue("col", typeof(CustomCollection));
			Data = (DataContainer)info.GetValue("data", typeof(DataContainer));
		}

        /// <summary>Gets a collection of mies.</summary>
        /// <value>A Collection of mies.</value>
		public CustomCollection MyCollection
		{
			get { return internalCollection; }
		}

        /// <summary>Gets or sets URI of the address.</summary>
        /// <value>The address URI.</value>
		public Uri AddressUri
		{
			get { return internalCollection.AddressUri; }
			set { internalCollection.AddressUri = value; }
		}

        /// <summary>Gets or sets the type of some.</summary>
        /// <value>The type of some.</value>
		public Type SomeType
		{
			get { return internalCollection.SomeType; }
			set { internalCollection.SomeType = value; }
		}

        /// <summary>Gets or sets the int value.</summary>
        /// <value>The int value.</value>
		public int IntValue
		{
			get { return internalCollection.IntValue; }
			set { internalCollection.IntValue = value; }
		}

        /// <summary>Gets or sets the data.</summary>
        /// <value>The data.</value>
		public DataContainer Data { get; set; }

		#region ISerializable Members
        /// <summary>
        /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo" /> with the data
        /// needed to serialize the target object.
        /// </summary>
        /// <param name="info">   The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> to
        /// populate with data.</param>
        /// <param name="context">The destination (see
        /// <see cref="T:System.Runtime.Serialization.StreamingContext" />) for this serialization.</param>
        /// ### <exception cref="T:System.Security.SecurityException">The caller does not have the required
        /// permission.</exception>
		[SecurityPermission(SecurityAction.LinkDemand, SerializationFormatter = true)]
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("col", internalCollection);
			info.AddValue("data", Data);
		}

		#endregion
	}
}