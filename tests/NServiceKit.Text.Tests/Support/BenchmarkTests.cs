using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NServiceKit.Common.Extensions;
using NServiceKit.Common.Tests;
using NServiceKit.Text.Tests.JsonTests;

namespace NServiceKit.Text.Tests.Support
{
    /// <summary>A benchmark tests.</summary>
	[Ignore]
	[TestFixture]
	public class BenchmarkTests
		: PerfTestBase
	{
        /// <summary>Tests string parsing.</summary>
		[Test]
		public void Test_string_parsing()
		{
			const int stringSampleSize = 1024 * 10;
			var testString = CreateRandomString(stringSampleSize);
			var copyTo = new char[stringSampleSize];

			CompareMultipleRuns(
				"As char array",
				() => {
					var asChars = testString.ToCharArray();
					for (var i = 0; i < stringSampleSize; i++)
					{
						copyTo[i] = asChars[i];
					}
				},
				"As string",
				() => {
					for (var i = 0; i < stringSampleSize; i++)
					{
						copyTo[i] = testString[i];
					}
				});
		}

        /// <summary>Creates random string.</summary>
        /// <param name="size">The size.</param>
        /// <returns>The new random string.</returns>
		public string CreateRandomString(int size)
		{
			var randString = new char[size];
			for (var i = 0; i < size; i++)
			{
				randString[i] = (char)((i % 10) + '0');
			}
			return new string(randString);
		}

        /// <summary>The escape characters.</summary>
		static readonly char[] EscapeChars = new char[]
		{
			'"', '\n', '\r', '\t', '"', '\\', '\f', '\b',
		};

        /// <summary>The jsv escape characters.</summary>
		public static readonly char[] JsvEscapeChars = new[]
    	{
    		'"', ',', '{', '}', '[', ']',
    	};

        /// <summary>The length from largest character.</summary>
		private const int LengthFromLargestChar = '\\' + 1;

        /// <summary>The has escape characters.</summary>
		private static readonly bool[] HasEscapeChars = new bool[LengthFromLargestChar];

        /// <summary>Print escape characters.</summary>
		[Test]
		public void PrintEscapeChars()
		{
			JsvEscapeChars.ToList().OrderBy(x => (int)x).ForEach(x => Console.WriteLine(x + ": " + (int)x));
		}

        /// <summary>Measure index of escape characters.</summary>
		[Test]
		public void MeasureIndexOfEscapeChars()
		{
			foreach (var escapeChar in EscapeChars)
			{
				HasEscapeChars[escapeChar] = true;
			}

			var value = CreateRandomString(100);
			var len = value.Length;
			var hasEscapeChars = false;

			CompareMultipleRuns(
				"With bool flags",
				() => {
					for (var i = 0; i < len; i++)
					{
						var c = value[i];
						if (c >= LengthFromLargestChar || !HasEscapeChars[c]) continue;
						hasEscapeChars = true;
						break;
					}
				},
				"With IndexOfAny",
				() => {
					hasEscapeChars = value.IndexOfAny(EscapeChars) != -1;
				});

			Console.WriteLine(hasEscapeChars);
		}

        /// <summary>A runtime type.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
		public class RuntimeType<T>
		{
            /// <summary>The type.</summary>
			private static Type type = typeof(T);

            /// <summary>Tests variable type.</summary>
            /// <returns>true if the test passes, false if the test fails.</returns>
			internal static bool TestVarType()
			{
				return type == typeof(byte) || type == typeof(byte?)
						|| type == typeof(short) || type == typeof(short?)
						|| type == typeof(ushort) || type == typeof(ushort?)
						|| type == typeof(int) || type == typeof(int?)
						|| type == typeof(uint) || type == typeof(uint?)
						|| type == typeof(long) || type == typeof(long?)
						|| type == typeof(ulong) || type == typeof(ulong?)
						|| type == typeof(bool) || type == typeof(bool?)
						|| type != typeof(DateTime)
						|| type != typeof(DateTime?)
						|| type != typeof(Guid)
						|| type != typeof(Guid?)
						|| type != typeof(float) || type != typeof(float?)
						|| type != typeof(double) || type != typeof(double?)
						|| type != typeof(decimal) || type != typeof(decimal?);
			}

            /// <summary>Tests generic type.</summary>
            /// <returns>true if the test passes, false if the test fails.</returns>
			internal static bool TestGenericType()
			{
				return typeof(T) == typeof(byte) || typeof(T) == typeof(byte?)
						|| typeof(T) == typeof(short) || typeof(T) == typeof(short?)
						|| typeof(T) == typeof(ushort) || typeof(T) == typeof(ushort?)
						|| typeof(T) == typeof(int) || typeof(T) == typeof(int?)
						|| typeof(T) == typeof(uint) || typeof(T) == typeof(uint?)
						|| typeof(T) == typeof(long) || typeof(T) == typeof(long?)
						|| typeof(T) == typeof(ulong) || typeof(T) == typeof(ulong?)
						|| typeof(T) == typeof(bool) || typeof(T) == typeof(bool?)
						|| typeof(T) != typeof(DateTime)
						|| typeof(T) != typeof(DateTime?)
						|| typeof(T) != typeof(Guid)
						|| typeof(T) != typeof(Guid?)
						|| typeof(T) != typeof(float) || typeof(T) != typeof(float?)
						|| typeof(T) != typeof(double) || typeof(T) != typeof(double?)
						|| typeof(T) != typeof(decimal) || typeof(T) != typeof(decimal?);
			}
		}

        /// <summary>Tests variable or generic type.</summary>
		[Test]
		public void TestVarOrGenericType()
		{
			var matchingTypesCount = 0;

			CompareMultipleRuns(
				"With var type",
				() => {
					if (RuntimeType<BenchmarkTests>.TestVarType())
					{
						matchingTypesCount++;
					}
				},
				"With generic type",
				() => {
					if (RuntimeType<BenchmarkTests>.TestGenericType())
					{
						matchingTypesCount++;
					}
				});

			Console.WriteLine(matchingTypesCount);
		}

        /// <summary>Tests for list enumeration.</summary>
		[Test]
		public void Test_for_list_enumeration()
		{
			List<Cat> list = 20.Times(x => new Cat { Name = "Cat" });
			var results = 0;
			var listLength = list.Count;

			CompareMultipleRuns(
				"With foreach",
				() => {
					foreach (var cat in list)
					{
						results++;
					}
				},
				"With for",
				() => {
					for (var i = 0; i < listLength; i++)
					{
						var cat = list[i];
						results++;
					}
				});

			Console.WriteLine(results);
		}

        /// <summary>Tests for ilist enumeration.</summary>
		[Test]
		public void Test_for_Ilist_enumeration()
		{
			IList<Cat> list = 20.Times(x => new Cat { Name = "Cat" });
			var results = 0;
			var listLength = list.Count;
			CompareMultipleRuns(
				"With foreach",
				() => {
					foreach (var cat in list)
					{
						results++;
					}
				},
				"With for",
				() => {
					for (var i = 0; i < listLength; i++)
					{
						var cat = list[i];
						results++;
					}
				});

			Console.WriteLine(results);
		}


	}
}