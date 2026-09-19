using aweXpect.Options;

namespace aweXpect.Core.Tests.Options;

public sealed partial class StringEqualityOptionsTests
{
	public sealed class WildcardMatchTypeTests
	{
		[Fact]
		public async Task AsWildcard_ShouldReturnSameInstance()
		{
			StringEqualityOptions sut = new();

			StringEqualityOptions result = sut.AsWildcard();

			await That(result).IsSameAs(sut);
		}

		[Theory]
		[InlineData("axxb", "a*b", 1)]
		[InlineData("axb ayb", "a?b", 2)]
		[InlineData("axxb ayb", "a*b", 1)]
		[InlineData("a\nxb", "a*b", 1)]
		public async Task CountOccurrences_ShouldCountTheMatchesOfThePattern(string actual, string expected,
			int expectedCount)
		{
			StringEqualityOptions sut = new();
			sut.AsWildcard();

			int result = await sut.CountOccurrences(actual, expected);

			await That(result).IsEqualTo(expectedCount)
				.Because("the match can be longer or shorter than the pattern and '*' is matched greedily");
		}

		[Theory]
		[InlineData("a\nb a\nb", "a?b", 2)]
		[InlineData("a\r\nb", "a??b", 1)]
		[InlineData("a\r\nb", "a?b", 0)]
		[InlineData("a\nxb\nayb", "a*b", 1)]
		public async Task CountOccurrences_ShouldTreatNewlinesLikeAnyOtherCharacter(string actual, string expected,
			int expectedCount)
		{
			StringEqualityOptions sut = new();
			sut.AsWildcard();

			int result = await sut.CountOccurrences(actual, expected);

			await That(result).IsEqualTo(expectedCount)
				.Because("a newline is a character, so both '*' and '?' have to match it");
		}

		[Fact]
		public async Task CountOccurrences_WhenCaseIsIgnored_ShouldIgnoreCase()
		{
			StringEqualityOptions sut = new();
			sut.AsWildcard().IgnoringCase();

			int result = await sut.CountOccurrences("AxB ayb", "a?b");

			await That(result).IsEqualTo(2);
		}

		[Theory]
		[InlineData("abc", 1)]
		[InlineData("", 0)]
		public async Task CountOccurrences_WhenPatternMatchesTheEmptyString_ShouldIgnoreEmptyMatches(string actual,
			int expectedCount)
		{
			StringEqualityOptions sut = new();
			sut.AsWildcard();

			int result = await sut.CountOccurrences(actual, "*");

			await That(result).IsEqualTo(expectedCount)
				.Because("an empty match does not cover any occurrence, just as an empty expected value never occurs");
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public async Task ShouldCompareCaseSensitive(bool ignoreCase)
		{
			string sut = "foo\nbar";

			async Task Act()
				=> await That(sut).IsEqualTo("FOO\nBAR").AsWildcard().IgnoringCase(ignoreCase);

			await That(Act).Throws<XunitException>().OnlyIf(!ignoreCase)
				.WithMessage("""
				             Expected that sut
				             matches "FOO\nBAR",
				             but it did not match:
				               ↓ (actual)
				               "foo\nbar"
				               "FOO\nBAR"
				               ↑ (wildcard pattern)
				             """).IgnoringNewlineStyle();
		}

		[Fact]
		public async Task ShouldDisplayActualAndPatternUnderneathEachOther()
		{
			string sut = "foo";

			async Task Act()
				=> await That(sut).IsEqualTo("bar").AsWildcard();

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that sut
				             matches "bar",
				             but it did not match:
				               ↓ (actual)
				               "foo"
				               "bar"
				               ↑ (wildcard pattern)
				             """);
		}

		[Fact]
		public async Task ShouldReplaceNewlines()
		{
			string sut = "foo\nbar";

			async Task Act()
				=> await That(sut).IsEqualTo("\tsomething\r\nelse").AsWildcard();

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that sut
				             matches "\tsomething\r\nelse",
				             but it did not match:
				               ↓ (actual)
				               "foo\nbar"
				               "\tsomething\r\nelse"
				               ↑ (wildcard pattern)
				             """).IgnoringNewlineStyle();
		}

		[Fact]
		public async Task ShouldSupportPassiveGrammaticalVoice()
		{
			Exception exception = new("foo");

			async Task Act()
				=> await That(() => Task.FromException(exception)).Throws().WithMessage("bar")
					.AsWildcard();

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that () => Task.FromException(exception)
				             throws an exception with Message matching "bar",
				             but it did not match:
				               ↓ (actual)
				               "foo"
				               "bar"
				               ↑ (wildcard pattern)

				             Message:
				             foo
				             """);
		}

		[Fact]
		public async Task WhenPatternIsNull_ShouldFail()
		{
			string sut = "foo";

			async Task Act()
				=> await That(sut).IsEqualTo(null).AsWildcard();

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that sut
				             matches <null>,
				             but could not compare the <null> wildcard pattern with "foo"
				             """);
		}

		[Fact]
		public async Task WhenSubjectAndPatternAreNull_ShouldFail()
		{
			string? sut = null;

			async Task Act()
				=> await That(sut).IsEqualTo(null).AsWildcard();

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that sut
				             matches <null>,
				             but it was <null>
				             """);
		}

		[Fact]
		public async Task WhenSubjectIsNull_ShouldFail()
		{
			string? sut = null;

			async Task Act()
				=> await That(sut).IsEqualTo("*").AsWildcard();

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that sut
				             matches "*",
				             but it was <null>
				             """);
		}
	}
}
