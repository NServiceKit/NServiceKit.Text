﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NServiceKit.Text.Tests.DynamicModels.DataModel
{
	[Serializable]
	public class CustomCollectionItem
	{
		public CustomCollectionItem()
		{}

		public CustomCollectionItem(string name, object value)
		{
			Name = name;
			Value = value;
		}

		public string Name { get; set; }
		public object Value { get; set; }

		public override string ToString()
		{
			return string.Concat("Name = '", Name, "' Value = '", Value, "'");
		}
	}
}