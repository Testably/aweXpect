using System.Linq;
using System.Text.RegularExpressions;
using aweXpect.Options;

namespace aweXpect.Core.Tests.Options;

public sealed partial class StringEqualityOptionsTests
{
	public sealed class SharedValueTests
	{
		[Test]
		public async Task AsPrefix_AfterIgnoringCase_ShouldReturnTheSameOptionsAsInTheOtherOrder()
		{
			StringEqualityOptions first = StringEqualityOptions.Default.IgnoringCase().AsPrefix();
			StringEqualityOptions second = StringEqualityOptions.Default.AsPrefix().IgnoringCase();

			await That(first).IsSameAs(second);
		}

		[Test]
		public async Task AsRegex_WithOptions_ShouldReturnNewOptionsEachTime()
		{
			StringEqualityOptions first = StringEqualityOptions.Default.AsRegex(RegexOptions.Multiline);
			StringEqualityOptions second = StringEqualityOptions.Default.AsRegex(RegexOptions.Multiline);

			await That(first).IsNotSameAs(second)
				.Because("a match type with arguments is not shared");
		}

		[Test]
		public async Task Default_ShouldCompareByOrdinalEquality()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;

			await That(sut.ComparesByOrdinalEquality).IsTrue();
			await That(sut.ToString()).IsEmpty();
		}

		[Test]
		public async Task IgnoringCase_ShouldReturnTheSameOptionsEachTime()
		{
			StringEqualityOptions first = StringEqualityOptions.Default.IgnoringCase();
			StringEqualityOptions second = StringEqualityOptions.Default.IgnoringCase();

			await That(first).IsSameAs(second);
		}

		[Test]
		public async Task IgnoringCase_WhenDisabledAgain_ShouldReturnTheDefault()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default.IgnoringCase().IgnoringCase(false);

			await That(sut).IsSameAs(StringEqualityOptions.Default);
		}

		[Test]
		public async Task IgnoringCase_WithFalse_WhenAComparerIsUsed_ShouldReturnTheSameOptions()
		{
			StringEqualityOptions withComparer = StringEqualityOptions.Default.Using(StringComparer.Ordinal);

			StringEqualityOptions result = withComparer.IgnoringCase(false);

			await That(result).IsSameAs(withComparer);
		}

		[Test]
		public async Task IgnoringTrailingWhiteSpace_WhenAComparerIsUsed_ShouldNotChangeTheOriginalOptions()
		{
			StringEqualityOptions withComparer = StringEqualityOptions.Default.Using(StringComparer.Ordinal);

			StringEqualityOptions result = withComparer.IgnoringTrailingWhiteSpace();

			await That(result).IsNotSameAs(withComparer);
			await That(withComparer.ToString()).DoesNotContain("trailing whitespace");
			await That(result.ToString()).EndsWith("ignoring trailing whitespace");
		}

		[Test]
		public async Task SharedOptions_WhenComparedOnSeveralThreads_ShouldEachUseTheirOwnPattern()
		{
			StringEqualityOptions wildcard = StringEqualityOptions.Default.AsWildcard();
			StringEqualityOptions regex = StringEqualityOptions.Default.AsRegex();

			int[] mismatches = await Task.WhenAll(Enumerable.Range(0, 8).Select(thread => Task.Run(async () =>
			{
				int count = 0;
				for (int i = 0; i < 1000; i++)
				{
					string actual = $"{thread}-{i}";
					if (!await wildcard.AreConsideredEqual(actual, $"{thread}-*", "expected") ||
					    !await regex.AreConsideredEqual(actual, $"^{thread}-", "expected") ||
					    await wildcard.AreConsideredEqual(actual, $"{thread + 1}-*", "expected"))
					{
						count++;
					}
				}

				return count;
			})));

			await That(mismatches.Sum()).IsEqualTo(0)
				.Because("shared options are never written after they are published, so a thread cannot see the pattern of another");
		}

		[Test]
		public async Task SharedOptions_WhenCreatedOnSeveralThreads_ShouldBeTheSameOptions()
		{
			StringEqualityOptions[] results = await Task.WhenAll(Enumerable.Range(0, 16).Select(_ => Task.Run(()
				=> StringEqualityOptions.Default.AsSuffix().IgnoringCase().IgnoringNewlineStyle()
					.NormalizingWhiteSpace())));

			await That(results.Distinct().Count()).IsEqualTo(1)
				.Because("an options value created in a race is dropped, so every thread gets the published one");
		}

		[Test]
		public async Task Using_ShouldReturnNewOptionsEachTime()
		{
			StringEqualityOptions first = StringEqualityOptions.Default.Using(StringComparer.Ordinal);
			StringEqualityOptions second = StringEqualityOptions.Default.Using(StringComparer.Ordinal);

			await That(first).IsNotSameAs(second)
				.Because("options with a comparer are not shared");
			await That(StringEqualityOptions.Default.ComparesByOrdinalEquality).IsTrue();
		}
	}
}
