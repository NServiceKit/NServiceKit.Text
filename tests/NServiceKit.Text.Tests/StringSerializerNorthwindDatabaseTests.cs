using System;
using System.Collections.Generic;
using Northwind.Common.DataModel;
using NUnit.Framework;

namespace NServiceKit.Text.Tests
{
    /// <summary>A string serializer northwind database tests.</summary>
	[TestFixture]
	public class StringSerializerNorthwindDatabaseTests
		: TestBase
	{
        /// <summary>Tests fixture set up.</summary>
		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			NorthwindData.LoadData(false);
		}

        /// <summary>Serialize category.</summary>
		[Test]
		public void serialize_Category()
		{
			Serialize(NorthwindData.Categories[0]);
		}

        /// <summary>Serialize categories.</summary>
		[Test]
		public void serialize_Categories()
		{
			Serialize(NorthwindData.Categories);
		}

        /// <summary>Serialize customer.</summary>
		[Test]
		public void serialize_Customer()
		{
			Serialize(NorthwindData.Customers[0]);
		}

        /// <summary>Serialize customers.</summary>
		[Test]
		public void serialize_Customers()
		{
			Serialize(NorthwindData.Customers);
		}

        /// <summary>Serialize employee.</summary>
		[Test]
		public void serialize_Employee()
		{
			Serialize(NorthwindData.Employees[0]);
		}

        /// <summary>Serialize employees.</summary>
		[Test]
		public void serialize_Employees()
		{
			Serialize(NorthwindData.Employees);
		}

        /// <summary>Serialize employee territory.</summary>
		[Test]
		public void serialize_EmployeeTerritory()
		{
			Serialize(NorthwindData.EmployeeTerritories[0]);
		}

        /// <summary>Serialize employee territories.</summary>
		[Test]
		public void serialize_EmployeeTerritories()
		{
			Serialize(NorthwindData.EmployeeTerritories);
		}

        /// <summary>Serialize order detail.</summary>
		[Test]
		public void serialize_OrderDetail()
		{
			Serialize(NorthwindData.OrderDetails[0]);
		}

        /// <summary>Serialize order details.</summary>
		[Test]
		public void serialize_OrderDetails()
		{
			Serialize(NorthwindData.OrderDetails);
		}

        /// <summary>Serialize order.</summary>
		[Test]
		public void serialize_Order()
		{
			Serialize(NorthwindData.Orders[0]);
		}

        /// <summary>Serialize orders.</summary>
		[Test]
		public void serialize_Orders()
		{
			Serialize(NorthwindData.Orders);
		}

        /// <summary>Serialize product.</summary>
		[Test]
		public void serialize_Product()
		{
			Serialize(NorthwindData.Products[0]);
		}

        /// <summary>Serialize products.</summary>
		[Test]
		public void serialize_Products()
		{
			Serialize(NorthwindData.Products);
		}

        /// <summary>Serialize region.</summary>
		[Test]
		public void serialize_Region()
		{
			Serialize(NorthwindData.Regions[0]);
		}

        /// <summary>Serialize regions.</summary>
		[Test]
		public void serialize_Regions()
		{
			Serialize(NorthwindData.Regions);
		}

        /// <summary>Serialize shipper.</summary>
		[Test]
		public void serialize_Shipper()
		{
			Serialize(NorthwindData.Shippers[0]);
		}

        /// <summary>Serialize shippers.</summary>
		[Test]
		public void serialize_Shippers()
		{
			Serialize(NorthwindData.Shippers);
		}

        /// <summary>Serialize supplier.</summary>
		[Test]
		public void serialize_Supplier()
		{
			Serialize(NorthwindData.Suppliers[0]);
		}

        /// <summary>Serialize suppliers.</summary>
		[Test]
		public void serialize_Suppliers()
		{
			Serialize(NorthwindData.Suppliers);
		}

        /// <summary>Serialize territory.</summary>
		[Test]
		public void serialize_Territory()
		{
			Serialize(NorthwindData.Territories[0]);
		}

        /// <summary>Serialize territories.</summary>
		[Test]
		public void serialize_Territories()
		{
			Serialize(NorthwindData.Territories);
		}

	}
}