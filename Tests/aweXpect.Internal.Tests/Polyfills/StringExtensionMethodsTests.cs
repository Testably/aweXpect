#if NETFRAMEWORK
using aweXpect.Polyfills;

namespace aweXpect.Internal.Tests.Polyfills;

public sealed class StringExtensionMethodsTests
{
	public sealed class ContainsCharTests
	{
		[Theory]
		[InlineData(StringComparison.Ordinal, false)]
		[InlineData(StringComparison.OrdinalIgnoreCase, true)]
		public async Task ShouldHonourComparisonType(StringComparison comparisonType, bool expected)
		{
			bool result = StringExtensionMethods.Contains("aBc", 'b', comparisonType);

			await That(result).IsEqualTo(expected);
		}
	}

	public sealed class ContainsStringTests
	{
		[Theory]
		[InlineData(StringComparison.Ordinal, false)]
		[InlineData(StringComparison.OrdinalIgnoreCase, true)]
		public async Task ShouldHonourComparisonType(StringComparison comparisonType, bool expected)
		{
			bool result = StringExtensionMethods.Contains("aBc", "bC", comparisonType);

			await That(result).IsEqualTo(expected);
		}
	}

	public sealed class EndsWithTests
	{
		[Fact]
		public async Task ShouldCompareOrdinally()
		{
			bool result = StringExtensionMethods.EndsWith("abc", '­');

			await That(result).IsFalse().Because("a culture-sensitive comparison ignores the soft hyphen");
		}

		[Fact]
		public async Task WhenLastCharacterMatches_ShouldReturnTrue()
		{
			bool result = StringExtensionMethods.EndsWith("abc", 'c');

			await That(result).IsTrue();
		}
	}

	public sealed class ReplaceTests
	{
		[Theory]
		[InlineData(StringComparison.Ordinal, "aBcx")]
		[InlineData(StringComparison.OrdinalIgnoreCase, "axcx")]
		public async Task ShouldHonourComparisonType(StringComparison comparisonType, string expected)
		{
			string result = StringExtensionMethods.Replace("aBcb", "b", "x", comparisonType);

			await That(result).IsEqualTo(expected);
		}

		[Fact]
		public async Task WhenNewValueIsNull_ShouldRemoveOccurrences()
		{
			string result = StringExtensionMethods.Replace("aBcb", "b", null, StringComparison.OrdinalIgnoreCase);

			await That(result).IsEqualTo("ac");
		}
	}

	public sealed class StartsWithTests
	{
		[Fact]
		public async Task ShouldCompareOrdinally()
		{
			bool result = StringExtensionMethods.StartsWith("abc", '­');

			await That(result).IsFalse().Because("a culture-sensitive comparison ignores the soft hyphen");
		}

		[Fact]
		public async Task WhenFirstCharacterMatches_ShouldReturnTrue()
		{
			bool result = StringExtensionMethods.StartsWith("abc", 'a');

			await That(result).IsTrue();
		}
	}
}
#endif
