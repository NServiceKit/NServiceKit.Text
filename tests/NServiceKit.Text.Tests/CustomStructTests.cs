using System;
using System.Collections.Generic;
using NUnit.Framework;
#if !MONOTOUCH
using NServiceKit.Common.Extensions;
#endif

namespace NServiceKit.Text.Tests
{
    /// <summary>A user stat.</summary>
	public struct UserStat
	{
        /// <summary>Gets or sets the identifier of the user.</summary>
        /// <value>The identifier of the user.</value>
		public Guid UserId { get; set; }

        /// <summary>Gets or sets the times recommended.</summary>
        /// <value>The times recommended.</value>
		public int TimesRecommended { get; set; }

        /// <summary>Gets or sets the times purchased.</summary>
        /// <value>The times purchased.</value>
		public int TimesPurchased { get; set; }

        /// <summary>Gets or sets the times flowed.</summary>
        /// <value>The times flowed.</value>
		public int TimesFlowed { get; set; }

        /// <summary>Gets or sets the times previewed.</summary>
        /// <value>The times previewed.</value>
		public int TimesPreviewed { get; set; }

        /// <summary>Gets weighted value.</summary>
        /// <returns>The weighted value.</returns>
		public int GetWeightedValue()
		{
			return (this.TimesRecommended * 10)
				   + (this.TimesPurchased * 3)
				   + (this.TimesFlowed * 2)
				   + this.TimesPreviewed;
		}

        /// <summary>Adds userStat.</summary>
        /// <param name="userStat">The user stat to add.</param>
		public void Add(UserStat userStat)
		{
			this.TimesRecommended += userStat.TimesRecommended;
			this.TimesFlowed += userStat.TimesFlowed;
			this.TimesPreviewed += userStat.TimesPreviewed;
			this.TimesPurchased += userStat.TimesPurchased;
		}

        /// <summary>Parses.</summary>
        /// <exception cref="ArgumentException">Thrown when one or more arguments have unsupported or
        /// illegal values.</exception>
        /// <param name="userStatString">The user stat string.</param>
        /// <returns>An UserStat.</returns>
		public static UserStat Parse(string userStatString)
		{
			var parts = userStatString.Split(':');
			if (parts.Length != 6)
				throw new ArgumentException("userStatString must have 6 parts");

			var i = 0;
			var userStat = new UserStat {
				UserId = new Guid(parts[i++]),
				TimesRecommended = int.Parse(parts[i++]),
				TimesPurchased = int.Parse(parts[i++]),
				TimesFlowed = int.Parse(parts[i++]),
				TimesPreviewed = int.Parse(parts[i++]),
			};
			return userStat;
		}

        /// <summary>Returns the fully qualified type name of this instance.</summary>
        /// <returns>A <see cref="T:System.String" /> containing a fully qualified type name.</returns>
		public override string ToString()
		{
			return string.Format("{0}:{1}:{2}:{3}:{4}:{5}",
				this.UserId.ToString("n"),
				TimesRecommended, 
				TimesPurchased, 
				TimesRecommended, 
				TimesPreviewed,
				GetWeightedValue());
		}
	}

    /// <summary>A custom structure tests.</summary>
	[TestFixture]
	public class CustomStructTests
		: TestBase
	{
        /// <summary>Creates user stat.</summary>
        /// <param name="userId">Identifier for the user.</param>
        /// <param name="score"> The score.</param>
        /// <returns>The new user stat.</returns>
		private static UserStat CreateUserStat(Guid userId, int score)
		{
			return new UserStat {
				UserId = userId,
				TimesRecommended = score,
				TimesPurchased = score,
				TimesFlowed = score,
				TimesPreviewed = score
			};
		}

        /// <summary>Can serialize empty user stat.</summary>
		[Test]
		public void Can_serialize_empty_UserStat()
		{
			var userStat = new UserStat();
			var dtoStr = TypeSerializer.SerializeToString(userStat);

			Assert.That(dtoStr, Is.EqualTo("\"00000000000000000000000000000000:0:0:0:0:0\""));

			SerializeAndCompare(userStat);
		}

        /// <summary>Can serialize user stat.</summary>
		[Test]
		public void Can_serialize_UserStat()
		{
			var userId = new Guid("96d7a49f7a0f46918661217995c5e4cc");
			var userStat = CreateUserStat(userId, 1);
			var dtoStr = TypeSerializer.SerializeToString(userStat);

			Assert.That(dtoStr, Is.EqualTo("\"96d7a49f7a0f46918661217995c5e4cc:1:1:1:1:16\""));

			SerializeAndCompare(userStat);
		}
#if !MONOTOUCH
        /// <summary>Can serialize user statistics list.</summary>
		[Test]
		public void Can_serialize_UserStats_list()
		{
			var guidValues =  new[] {
          		new Guid("6203A3AF-1738-4CDF-A3AD-0F578AD198F0"), 
          		new Guid("C7C87DF5-4821-400D-B9F7-D8EEE23C5842"), 
          		new Guid("33EB45D4-21A0-41CC-A07D-43BFAB4B3E92"), 
          		new Guid("ED041F82-572A-41CB-90D3-E227786BE9EB"), 
          		new Guid("D703F00C-613A-44A9-AC2B-C46ED0F23D3C"), 
          	};

			var userStats = 5.Times(i => CreateUserStat(guidValues[i], i));
			var dtoStr = TypeSerializer.SerializeToString(userStats);

			Assert.That(dtoStr, Is.EqualTo(
				"[\"6203a3af17384cdfa3ad0f578ad198f0:0:0:0:0:0\",\"c7c87df54821400db9f7d8eee23c5842:1:1:1:1:16\",\"33eb45d421a041cca07d43bfab4b3e92:2:2:2:2:32\",\"ed041f82572a41cb90d3e227786be9eb:3:3:3:3:48\",\"d703f00c613a44a9ac2bc46ed0f23d3c:4:4:4:4:64\"]"));

			SerializeAndCompare(userStats);
		}
#endif
	}
}