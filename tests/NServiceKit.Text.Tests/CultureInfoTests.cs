using NUnit.Framework;
using System;
using System.Globalization;
using System.Threading;

namespace NServiceKit.Text.Tests
{
    /// <summary>A culture information tests.</summary>
    [TestFixture]
    public class CultureInfoTests
        : TestBase
    {
        /// <summary>A point.</summary>
        public class Point
        {
            /// <summary>Gets or sets the latitude.</summary>
            /// <value>The latitude.</value>
            public double Latitude { get; set; }

            /// <summary>Gets or sets the longitude.</summary>
            /// <value>The longitude.</value>
            public double Longitude { get; set; }

            /// <summary>
            /// Determines whether the specified <see cref="T:System.Object" /> is equal to the current
            /// <see cref="T:System.Object" />.
            /// </summary>
            /// <param name="other">The point to compare to this object.</param>
            /// <returns>
            /// true if the specified <see cref="T:System.Object" /> is equal to the current
            /// <see cref="T:System.Object" />; otherwise, false.
            /// </returns>
            public bool Equals(Point other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return other.Latitude == Latitude && other.Longitude == Longitude;
            }

            /// <summary>
            /// Determines whether the specified <see cref="T:System.Object" /> is equal to the current
            /// <see cref="T:System.Object" />.
            /// </summary>
            /// <param name="obj">The <see cref="T:System.Object" /> to compare with the current
            /// <see cref="T:System.Object" />.</param>
            /// <returns>
            /// true if the specified <see cref="T:System.Object" /> is equal to the current
            /// <see cref="T:System.Object" />; otherwise, false.
            /// </returns>
            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != typeof(Point)) return false;
                return Equals((Point)obj);
            }

            /// <summary>Serves as a hash function for a particular type.</summary>
            /// <returns>A hash code for the current <see cref="T:System.Object" />.</returns>
            public override int GetHashCode()
            {
                unchecked
                {
                    return (Latitude.GetHashCode() * 397) ^ Longitude.GetHashCode();
                }
            }
        }

        /// <summary>The previous culture.</summary>
        private CultureInfo previousCulture = CultureInfo.InvariantCulture;

        /// <summary>Tests fixture set up.</summary>
        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            previousCulture = Thread.CurrentThread.CurrentCulture;
            //Thread.CurrentThread.CurrentCulture = new CultureInfo("pt-BR");
            Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("fr-FR");
        }

        /// <summary>Tests fixture tear down.</summary>
        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            Thread.CurrentThread.CurrentCulture = previousCulture;
        }

        /// <summary>Can deserialize type with doubles in different culture.</summary>
        [Test]
        public void Can_deserialize_type_with_doubles_in_different_culture()
        {
            var point = new Point { Latitude = -23.5707, Longitude = -46.57239 };
            SerializeAndCompare(point);
        }

        /// <summary>Can deserialize type with single in different culture.</summary>
        [Test]
        public void Can_deserialize_type_with_Single_in_different_culture()
        {
            Single single = (float)1.123;
            var txt = TypeSerializer.SerializeToString(single);

            Assert.IsNotNullOrEmpty(txt);
        }

        /// <summary>Serializes doubles using invariant culture.</summary>
        [Test]
        public void Serializes_doubles_using_InvariantCulture()
        {
            //Used in RedisClient
            var doubleUtf8 = 66121.202.ToUtf8Bytes();
            var doubleStr = doubleUtf8.FromUtf8Bytes();
            Assert.That(doubleStr, Is.EqualTo("66121.202"));
        }

        /// <summary>Serializes long double without e notation.</summary>
        [Test]
        public void Serializes_long_double_without_E_notation()
        {
            //Used in RedisClient
            var doubleUtf8 = 1234567890123456d.ToUtf8Bytes();
            var doubleStr = doubleUtf8.FromUtf8Bytes();
            Assert.That(doubleStr, Is.EqualTo("1234567890123456"));
        }

    }
}