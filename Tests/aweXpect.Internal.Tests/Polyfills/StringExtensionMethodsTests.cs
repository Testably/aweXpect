#if NETFRAMEWORK
using aweXpect.Polyfills;

namespace aweXpect.Internal.Tests.Polyfills;

public sealed class StringExtensionMethodsTests
{
	public sealed class ContainsCharTests
	{
		[Test]
		[Arguments(StringComparison.Ordinal, false)]
		[Arguments(StringComparison.OrdinalIgnoreCase, true)]
		public async Task ShouldHonourComparisonType(StringComparison comparisonType, bool expected)
		{
			bool result = StringExtensionMethods.Contains("aBc", 'b', comparisonType);

			await That(result).IsEqualTo(expected);
		}
	}

	public sealed class ContainsStringTests
	{
		[Test]
		[Arguments(StringComparison.Ordinal, false)]
		[Arguments(StringComparison.OrdinalIgnoreCase, true)]
		public async Task ShouldHonourComparisonType(StringComparison comparisonType, bool expected)
		{
			bool result = StringExtensionMethods.Contains("aBc", "bC", comparisonType);

			await That(result).IsEqualTo(expected);
		}
	}

	public sealed class EndsWithTests
	{
		[Test]
		public async Task ShouldCompareOrdinally()
		{
			bool result = StringExtensionMethods.EndsWith("abc", '­');

			await That(result).IsFalse().Because("a culture-sensitive comparison ignores the soft hyphen");
		}

		[Test]
		public async Task WhenLastCharacterMatches_ShouldReturnTrue()
		{
			bool result = StringExtensionMethods.EndsWith("abc", 'c');

			await That(result).IsTrue();
		}
	}

	public sealed class ReplaceTests
	{
		[Test]
		[Arguments(StringComparison.Ordinal, "aBcx")]
		[Arguments(StringComparison.OrdinalIgnoreCase, "axcx")]
		public async Task ShouldHonourComparisonType(StringComparison comparisonType, string expected)
		{
			string result = StringExtensionMethods.Replace("aBcb", "b", "x", comparisonType);

			await That(result).IsEqualTo(expected);
		}

		[Test]
		public async Task WhenNewValueIsNull_ShouldRemoveOccurrences()
		{
			string result = StringExtensionMethods.Replace("aBcb", "b", null, StringComparison.OrdinalIgnoreCase);

			await That(result).IsEqualTo("ac");
		}
	}

	public sealed class StartsWithTests
	{
		[Test]
		public async Task ShouldCompareOrdinally()
		{
			bool result = StringExtensionMethods.StartsWith("abc", '­');

			await That(result).IsFalse().Because("a culture-sensitive comparison ignores the soft hyphen");
		}

		[Test]
		public async Task WhenFirstCharacterMatches_ShouldReturnTrue()
		{
			bool result = StringExtensionMethods.StartsWith("abc", 'a');

			await That(result).IsTrue();
		}
	}
}
#endif
