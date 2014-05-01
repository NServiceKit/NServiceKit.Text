using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using NUnit.Framework;
using NServiceKit.Text.Tests.Support;

namespace NServiceKit.Text.Tests.JsonTests
{
    /// <summary>A throw on deserialize error test.</summary>
	[TestFixture]
	public class ThrowOnDeserializeErrorTest
	{
        /// <summary>Throws on protected setter.</summary>
        [Test]
        [ExpectedException(typeof(SerializationException), ExpectedMessage = "Failed to set property 'idBadProt' with 'abc'")]
        public void Throws_on_protected_setter()
        {
            JsConfig.Reset();
            JsConfig.ThrowOnDeserializationError = true;

            string json = @"{""idBadProt"":""abc"", ""idGood"":""2"" }";
            JsonSerializer.DeserializeFromString(json, typeof(TestDto));
        }

        /// <summary>Throws on incorrect type.</summary>
		[Test]
        [ExpectedException(typeof(SerializationException), ExpectedMessage = "Failed to set property 'idBad' with 'abc'")]
		public void Throws_on_incorrect_type()
		{
			JsConfig.Reset();
			JsConfig.ThrowOnDeserializationError = true;

			string json = @"{""idBad"":""abc"", ""idGood"":""2"" }";
    		JsonSerializer.DeserializeFromString(json, typeof(TestDto));
		}

        /// <summary>Throws on incorrect type with data set.</summary>
		[Test]
		public void Throws_on_incorrect_type_with_data_set()
		{
			JsConfig.Reset();
			JsConfig.ThrowOnDeserializationError = true;

            try {
			    string json = @"{""idBad"":""abc"", ""idGood"":""2"" }";
    		    JsonSerializer.DeserializeFromString(json, typeof(TestDto));
                Assert.Fail("Exception should have been thrown.");
            } catch (SerializationException ex) {
                Assert.That(ex.Data, Is.Not.Null);
                Assert.That(ex.Data["propertyName"], Is.EqualTo("idBad"));
                Assert.That(ex.Data["propertyValueString"], Is.EqualTo("abc"));
                Assert.That(ex.Data["propertyType"], Is.EqualTo(typeof(int)));
            }
		}

        /// <summary>Tests does not throw.</summary>
		[Test]
		public void TestDoesNotThrow()
		{
			JsConfig.Reset();
			JsConfig.ThrowOnDeserializationError = false;
			string json = @"{""idBad"":""abc"", ""idGood"":""2"" }";
			JsonSerializer.DeserializeFromString(json, typeof(TestDto));
		}

        /// <summary>Tests reset.</summary>
		[Test]
		public void TestReset()
		{
			JsConfig.Reset();
			Assert.IsFalse(JsConfig.ThrowOnDeserializationError);
			JsConfig.ThrowOnDeserializationError = true;
			Assert.IsTrue(JsConfig.ThrowOnDeserializationError);
			JsConfig.Reset();
			Assert.IsFalse(JsConfig.ThrowOnDeserializationError);
		}

        /// <summary>A test dto.</summary>
		[DataContract]
		class TestDto
		{
            /// <summary>Gets the identifier of the prot.</summary>
            /// <value>The identifier of the prot.</value>
            [DataMember(Name = "idBadProt")]
            public int protId { get; protected set; }

            /// <summary>Gets or sets the identifier good.</summary>
            /// <value>The identifier good.</value>
            [DataMember(Name = "idGood")]
			public int IdGood { get; set; }

            /// <summary>Gets or sets the identifier bad.</summary>
            /// <value>The identifier bad.</value>
			[DataMember(Name = "idBad")]
			public int IdBad { get; set; }
        }
	}
}
