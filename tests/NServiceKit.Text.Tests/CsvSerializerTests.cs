using Northwind.Common.DataModel;
using NServiceKit.Text.Tests.Support;
using NUnit.Framework;

namespace NServiceKit.Text.Tests
{
    /// <summary>A CSV serializer tests.</summary>
    [TestFixture]
    public class CsvSerializerTests
    {
        /// <summary>
        /// Initializes static members of the NServiceKit.Text.Tests.CsvSerializerTests class.
        /// </summary>
        static CsvSerializerTests()
        {
            NorthwindData.LoadData(false);
        }

        /// <summary>true this object to the given stream.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="data">The data.</param>
        public void Serialize<T>(T data)
        {
            //TODO: implement serializer and test properly
            var csv = CsvSerializer.SerializeToString(data);
            Assert.IsNotNullOrEmpty(csv);
        }

        /// <summary>Can serialize movie.</summary>
        [Test]
        public void Can_Serialize_Movie()
        {
            Serialize(MoviesData.Movies[0]);
        }

        /// <summary>Can serialize movies.</summary>
        [Test]
        public void Can_Serialize_Movies()
        {
            Serialize(MoviesData.Movies);
        }

        /// <summary>Can serialize movie response dto.</summary>
        [Test]
        public void Can_Serialize_MovieResponse_Dto()
        {
            Serialize(new MovieResponse { Movie = MoviesData.Movies[0] });
        }

        /// <summary>Can serialize movies response dto.</summary>
        [Test]
        public void Can_Serialize_MoviesResponse_Dto()
        {
            Serialize(new MoviesResponse { Movies = MoviesData.Movies });
        }

        /// <summary>Can serialize movies response 2 dto.</summary>
        [Test]
        public void Can_Serialize_MoviesResponse2_Dto()
        {
            Serialize(new MoviesResponse2 { Movies = MoviesData.Movies });
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