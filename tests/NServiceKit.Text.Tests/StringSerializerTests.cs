#if !MONO && !MONOTOUCH
using System;
using System.Collections.Generic;
using Northwind.Common.ComplexModel;
using Northwind.Common.DataModel;
using NUnit.Framework;

namespace NServiceKit.Text.Tests
{
    /// <summary>A string serializer tests.</summary>
	[TestFixture]
	public class StringSerializerTests
		: TestBase
	{
        /// <summary>Tests fixture set up.</summary>
		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			NorthwindData.LoadData(false);
		}

        /// <summary>Can convert customer order list dto.</summary>
		[Test]
		public void Can_convert_CustomerOrderListDto()
		{
			var dto = DtoFactory.CustomerOrderListDto;

			Serialize(dto);
		}

        /// <summary>Can convert to customer order list dto.</summary>
		[Test]
		public void Can_convert_to_CustomerOrderListDto()
		{
			var dto = DtoFactory.CustomerOrderListDto;

			Serialize(dto);
		}

        /// <summary>Can convert to customers.</summary>
		[Test]
		public void Can_convert_to_Customers()
		{
			var dto = NorthwindData.Customers;

			Serialize(dto);
		}

        /// <summary>Can convert to orders.</summary>
		[Test]
		public void Can_convert_to_Orders()
		{
			NorthwindData.LoadData(false);
			var dto = NorthwindData.Orders;

			Serialize(dto);
		}

	}
}

#endif
