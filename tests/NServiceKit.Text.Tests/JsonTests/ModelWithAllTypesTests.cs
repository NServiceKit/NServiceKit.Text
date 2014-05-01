using System;
using System.Collections.Generic;
using NUnit.Framework;
using NServiceKit.Text.Tests.DynamicModels;

namespace NServiceKit.Text.Tests.JsonTests
{
    /// <summary>A model with all types tests.</summary>
	[TestFixture]
	public class ModelWithAllTypesTests
	{
        /// <summary>Can serialize.</summary>
		[Test]
		public void Can_Serialize()
		{
			var model = ModelWithAllTypes.Create(1);
			var s = JsonSerializer.SerializeToString(model);

			Console.WriteLine(s);
		}

        /// <summary>Can serialize list.</summary>
		[Test]
		public void Can_Serialize_list()
		{
			var model = new List<ModelWithAllTypes>
           	{
				ModelWithAllTypes.Create(1),
				ModelWithAllTypes.Create(2)
           	};
			var s = JsonSerializer.SerializeToString(model);

			Console.WriteLine(s);
		}

        /// <summary>Can serialize map.</summary>
		[Test]
		public void Can_Serialize_map()
		{
			var model = new Dictionary<string, ModelWithAllTypes>
           	{
				{"A", ModelWithAllTypes.Create(1)},
				{"B", ModelWithAllTypes.Create(2)},
           	};
			var s = JsonSerializer.SerializeToString(model);

			Console.WriteLine(s);
		}


	}
}