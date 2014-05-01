using System;
using NUnit.Framework;

namespace NServiceKit.Text.Tests
{
    /// <summary>A system time tests.</summary>
	[TestFixture]
	public class SystemTimeTests
	{
        /// <summary>
        /// When set system time resolver then should get correct system time UTC now.
        /// </summary>
		[Test]
		public void When_set_SystemTimeResolver_Then_should_get_correct_SystemTime_UtcNow()
		{
			var dateTime = new DateTime(2011, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			SystemTime.UtcDateTimeResolver = () => dateTime;
			Assert.AreEqual(dateTime.ToUniversalTime(), SystemTime.UtcNow);
		}

        /// <summary>
        /// When set UTC date time resolver then should get correct system time now.
        /// </summary>
		[Test]
		public void When_set_UtcDateTimeResolver_Then_should_get_correct_SystemTime_Now()
		{
            var dateTime = new DateTime(2011, 1, 1, 0, 0, 0, DateTimeKind.Local);
			SystemTime.UtcDateTimeResolver = () => dateTime;
			Assert.AreEqual(dateTime, SystemTime.Now);
		}

        /// <summary>
        /// When set UTC date time resolver to null and then should get correct system time now.
        /// </summary>
		[Test]
		public void When_set_UtcDateTimeResolver_to_null_and_Then_should_get_correct_SystemTime_Now()
		{
			SystemTime.UtcDateTimeResolver = null;
			Assert.True(DateTime.UtcNow.IsEqualToTheSecond(SystemTime.UtcNow));
		}

        /// <summary>
        /// When set UTC date time resolver to null then should get correct system time now.
        /// </summary>
		[Test]
		public void When_set_UtcDateTimeResolver_to_null_Then_should_get_correct_SystemTime_Now()
		{
			SystemTime.UtcDateTimeResolver = null;
			Assert.True(DateTime.Now.IsEqualToTheSecond(SystemTime.Now));
		}

        /// <summary>Tear down.</summary>
		[TearDown]
		public void TearDown()
		{
			SystemTime.UtcDateTimeResolver = null;
		}
	}
}
