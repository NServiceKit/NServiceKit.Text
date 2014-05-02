using System;
using NUnit.Framework;

namespace NServiceKit.Text.Tests
{
    /// <summary>A date time extensions tests.</summary>
    [TestFixture]
    public class DateTimeExtensionsTests
    {
        /// <summary>Tests last monday.</summary>
        [TestCase]
        public void LastMondayTest()
        {
            var monday = new DateTime(2013, 04, 15);

            var lastMonday = DateTimeExtensions.LastMonday(monday);

            Assert.AreEqual(monday, lastMonday);
        } 
    }
}