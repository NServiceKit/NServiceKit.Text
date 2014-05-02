using Northwind.Common.DataModel;
using NUnit.Framework;
using System;

namespace NServiceKit.Text.Tests
{
    /// <summary>A CSV stream tests.</summary>
    [TestFixture]
    public class CsvStreamTests
    {
        /// <summary>Logs.</summary>
        /// <param name="fmt"> Describes the format to use.</param>
        /// <param name="args">A variable-length parameters list containing arguments.</param>
        protected void Log(string fmt, params object[] args)
        {
            // Console.WriteLine("{0}", String.Format(fmt, args).Trim());
        }

        /// <summary>Tear down.</summary>
        [TearDown]
        public void TearDown()
        {
            CsvConfig.Reset();
        }

        /// <summary>Can create CSV from customers.</summary>
        [Test]
        public void Can_create_csv_from_Customers()
        {
            NorthwindData.LoadData(false);
            var csv = CsvSerializer.SerializeToCsv(NorthwindData.Customers);
            Log(csv);
            Assert.That(csv, Is.Not.Null);
        }

        /// <summary>Can create CSV from customers pipe separator.</summary>
        [Test]
        public void Can_create_csv_from_Customers_pipe_separator()
        {
            CsvConfig.ItemSeperatorString = "|";
            NorthwindData.LoadData(false);
            var csv = CsvSerializer.SerializeToCsv(NorthwindData.Customers);
            Log(csv);
            Assert.That(csv, Is.Not.Null);
        }

        /// <summary>Can create CSV from customers pipe delimiter.</summary>
        [Test]
        public void Can_create_csv_from_Customers_pipe_delimiter()
        {
            CsvConfig.ItemDelimiterString = "|";
            NorthwindData.LoadData(false);
            var csv = CsvSerializer.SerializeToCsv(NorthwindData.Customers);
            Log(csv);
            Assert.That(csv, Is.Not.Null);
        }

        /// <summary>Can create CSV from customers pipe row separator.</summary>
        [Test]
        public void Can_create_csv_from_Customers_pipe_row_separator()
        {
            CsvConfig.RowSeparatorString = "|";
            NorthwindData.LoadData(false);
            var csv = CsvSerializer.SerializeToCsv(NorthwindData.Customers);
            Log(csv);
            Assert.That(csv, Is.Not.Null);
        }

        /// <summary>Can create CSV from categories.</summary>
        [Test]
        public void Can_create_csv_from_Categories()
        {
            NorthwindData.LoadData(false);
            var category = NorthwindFactory.Category(1, "between \"quotes\" here", "with, comma", null);
            var categories = new[] { category, category };
            var csv = CsvSerializer.SerializeToCsv(categories);
            Log(csv);
            Assert.That(csv, Is.EqualTo(
                "Id,CategoryName,Description,Picture"
                + Environment.NewLine
                + "1,\"between \"\"quotes\"\" here\",\"with, comma\","
                + Environment.NewLine
                + "1,\"between \"\"quotes\"\" here\",\"with, comma\","
                + Environment.NewLine
            ));
        }

        /// <summary>Can create CSV from categories pipe separator.</summary>
        [Test]
        public void Can_create_csv_from_Categories_pipe_separator()
        {
            CsvConfig.ItemSeperatorString = "|";
            NorthwindData.LoadData(false);
            var category = NorthwindFactory.Category(1, "between \"quotes\" here", "with, comma", null);
            var categories = new[] { category, category };
            var csv = CsvSerializer.SerializeToCsv(categories);
            Log(csv);
            Assert.That(csv, Is.EqualTo(
                "Id|CategoryName|Description|Picture"
                + Environment.NewLine
                + "1|\"between \"\"quotes\"\" here\"|with, comma|"
                + Environment.NewLine
                + "1|\"between \"\"quotes\"\" here\"|with, comma|"
                + Environment.NewLine
            ));
        }

        /// <summary>Can create CSV from categories pipe delimiter.</summary>
        [Test]
        public void Can_create_csv_from_Categories_pipe_delimiter()
        {
            CsvConfig.ItemDelimiterString = "|";
            NorthwindData.LoadData(false);
            var category = NorthwindFactory.Category(1, "between \"quotes\" here", "with, comma", null);
            var categories = new[] { category, category };
            var csv = CsvSerializer.SerializeToCsv(categories);
            Log(csv);
            Assert.That(csv, Is.EqualTo(
                "Id,CategoryName,Description,Picture"
                + Environment.NewLine
                + "1,between \"quotes\" here,|with, comma|,"
                + Environment.NewLine
                + "1,between \"quotes\" here,|with, comma|,"
                + Environment.NewLine
            ));
        }

        /// <summary>Can create CSV from categories long delimiter.</summary>
        [Test]
        public void Can_create_csv_from_Categories_long_delimiter()
        {
            CsvConfig.ItemDelimiterString = "~^~";
            NorthwindData.LoadData(false);
            var category = NorthwindFactory.Category(1, "between \"quotes\" here", "with, comma", null);
            var categories = new[] { category, category };
            var csv = CsvSerializer.SerializeToCsv(categories);
            Log(csv);
            Assert.That(csv, Is.EqualTo(
                "Id,CategoryName,Description,Picture"
                + Environment.NewLine
                + "1,between \"quotes\" here,~^~with, comma~^~,"
                + Environment.NewLine
                + "1,between \"quotes\" here,~^~with, comma~^~,"
                + Environment.NewLine
            ));
        }

        /// <summary>Can generate CSV with invalid characters.</summary>
        [Test]
        public void Can_generate_csv_with_invalid_chars()
        {
            var fields = new[] { "1", "2", "3\"", "4", "5\"five,six\"", "7,7.1", "\"7,7.1\"", "8" };
            var csv = CsvSerializer.SerializeToCsv(fields);
            Log(csv);
            Assert.That(csv, Is.EqualTo(
                "1,2,\"3\"\"\",4,\"5\"\"five,six\"\"\",\"7,7.1\",\"\"\"7,7.1\"\"\",8"
                + Environment.NewLine
            ));
        }

        /// <summary>Can generate CSV with invalid characters pipe delimiter.</summary>
        [Test]
        public void Can_generate_csv_with_invalid_chars_pipe_delimiter()
        {
            CsvConfig.ItemDelimiterString = "|";
            var fields = new[] { "1", "2", "3\"", "4", "5\"five,six\"", "7,7.1", "\"7,7.1\"", "8" };
            var csv = CsvSerializer.SerializeToCsv(fields);
            Log(csv);
            Assert.That(csv, Is.EqualTo(
                "1,2,3\",4,|5\"five,six\"|,|7,7.1|,|\"7,7.1\"|,8"
                + Environment.NewLine
            ));
        }

        /// <summary>Can generate CSV with invalid characters pipe separator.</summary>
        [Test]
        public void Can_generate_csv_with_invalid_chars_pipe_separator()
        {
            CsvConfig.ItemSeperatorString = "|";
            var fields = new[] { "1", "2", "3\"", "4", "5\"five,six\"", "7,7.1", "\"7,7.1\"", "8" };
            var csv = CsvSerializer.SerializeToCsv(fields);
            Log(csv);
            Assert.That(csv, Is.EqualTo(
                "1|2|\"3\"\"\"|4|\"5\"\"five,six\"\"\"|7,7.1|\"\"\"7,7.1\"\"\"|8"
                + Environment.NewLine
            ));
        }

        /// <summary>Can convert to CSV field.</summary>
        [Test]
        public void Can_convert_to_csv_field()
        {
            Assert.That("1".ToCsvField(), Is.EqualTo("1"));
            Assert.That("3\"".ToCsvField(), Is.EqualTo("\"3\"\"\""));
            Assert.That("5\"five,six\"".ToCsvField(), Is.EqualTo("\"5\"\"five,six\"\"\""));
            Assert.That("7,7.1".ToCsvField(), Is.EqualTo("\"7,7.1\""));
            Assert.That("\"7,7.1\"".ToCsvField(), Is.EqualTo("\"\"\"7,7.1\"\"\""));
        }

        /// <summary>Can convert to CSV field pipe separator.</summary>
        [Test]
        public void Can_convert_to_csv_field_pipe_separator()
        {
            CsvConfig.ItemSeperatorString = "|";
            Assert.That("1".ToCsvField(), Is.EqualTo("1"));
            Assert.That("3\"".ToCsvField(), Is.EqualTo("\"3\"\"\""));
            Assert.That("5\"five,six\"".ToCsvField(), Is.EqualTo("\"5\"\"five,six\"\"\""));
            Assert.That("7,7.1".ToCsvField(), Is.EqualTo("7,7.1"));
            Assert.That("\"7,7.1\"".ToCsvField(), Is.EqualTo("\"\"\"7,7.1\"\"\""));
        }

        /// <summary>Can convert to CSV field pipe delimiter.</summary>
        [Test]
        public void Can_convert_to_csv_field_pipe_delimiter()
        {
            CsvConfig.ItemDelimiterString = "|";
            Assert.That("1".ToCsvField(), Is.EqualTo("1"));
            Assert.That("3\"".ToCsvField(), Is.EqualTo("3\""));
            Assert.That("5\"five,six\"".ToCsvField(), Is.EqualTo("|5\"five,six\"|"));
            Assert.That("7,7.1".ToCsvField(), Is.EqualTo("|7,7.1|"));
            Assert.That("\"7,7.1\"".ToCsvField(), Is.EqualTo("|\"7,7.1\"|"));
        }

        /// <summary>Can convert from CSV field.</summary>
        [Test]
        public void Can_convert_from_csv_field()
        {
            Assert.That("1".FromCsvField(), Is.EqualTo("1"));
            Assert.That("\"3\"\"\"".FromCsvField(), Is.EqualTo("3\""));
            Assert.That("\"5\"\"five,six\"\"\"".FromCsvField(), Is.EqualTo("5\"five,six\""));
            Assert.That("\"7,7.1\"".FromCsvField(), Is.EqualTo("7,7.1"));
            Assert.That("\"\"\"7,7.1\"\"\"".FromCsvField(), Is.EqualTo("\"7,7.1\""));
        }

        /// <summary>Can convert from CSV field pipe separator.</summary>
        [Test]
        public void Can_convert_from_csv_field_pipe_separator()
        {
            CsvConfig.ItemSeperatorString = "|";
            Assert.That("1".FromCsvField(), Is.EqualTo("1"));
            Assert.That("\"3\"\"\"".FromCsvField(), Is.EqualTo("3\""));
            Assert.That("\"5\"\"five,six\"\"\"".FromCsvField(), Is.EqualTo("5\"five,six\""));
            Assert.That("\"7,7.1\"".FromCsvField(), Is.EqualTo("7,7.1"));
            Assert.That("7,7.1".FromCsvField(), Is.EqualTo("7,7.1"));
            Assert.That("\"\"\"7,7.1\"\"\"".FromCsvField(), Is.EqualTo("\"7,7.1\""));
        }

        /// <summary>Can convert from CSV field pipe delimiter.</summary>
        [Test]
        public void Can_convert_from_csv_field_pipe_delimiter()
        {
            CsvConfig.ItemDelimiterString = "|";
            Assert.That("1".FromCsvField(), Is.EqualTo("1"));
            Assert.That("3\"".FromCsvField(), Is.EqualTo("3\""));
            Assert.That("|5\"five,six\"|".FromCsvField(), Is.EqualTo("5\"five,six\""));
            Assert.That("|7,7.1|".FromCsvField(), Is.EqualTo("7,7.1"));
            Assert.That("|\"7,7.1\"|".FromCsvField(), Is.EqualTo("\"7,7.1\""));
        }

    }
}