using System.Text.RegularExpressions;
using aweXpect.Options;

namespace aweXpect.Core.Tests.Options;

public sealed partial class StringEqualityOptionsTests
{
	public sealed class RegexMatchTypeTests
	{
		[Fact]
		public async Task AsRegex_ShouldReturnSameInstance()
		{
			StringEqualityOptions sut = new();

			StringEqualityOptions result = sut.AsRegex();

			await That(result).IsSameAs(sut);
		}

		[Fact]
		public async Task AsRegex_WithOptions_ShouldReturnSameInstance()
		{
			StringEqualityOptions sut = new();

			StringEqualityOptions result = sut.AsRegex(RegexOptions.Multiline);

			await That(result).IsSameAs(sut);
		}

		[Theory]
		[InlineData("axxxb", "a.*b", 1)]
		[InlineData("abcabc", "[a-c]{3}", 2)]
		[InlineData("aaaa", "a+", 1)]
		[InlineData("a1b a22b", "a\\d+b", 2)]
		[InlineData("aXa", "a", 2)]
		[InlineData("aaaa", "aa", 2)]
		public async Task CountOccurrences_ShouldCountTheMatchesOfThePattern(string actual, string expected,
			int expectedCount)
		{
			StringEqualityOptions sut = new();
			sut.AsRegex();

			int result = await sut.CountOccurrences(actual, expected);

			await That(result).IsEqualTo(expectedCount)
				.Because("the match can be longer or shorter than the pattern");
		}

		[Fact]
		public async Task CountOccurrences_WhenCaseIsIgnored_ShouldIgnoreCase()
		{
			StringEqualityOptions sut = new();
			sut.AsRegex().IgnoringCase();

			int result = await sut.CountOccurrences("AxB ayb", "a.b");

			await That(result).IsEqualTo(2);
		}

		[Theory]
		[InlineData(RegexOptions.None, 1)]
		[InlineData(RegexOptions.Multiline, 2)]
		public async Task CountOccurrences_WhenOptionsAreGiven_ShouldApplyThem(RegexOptions regexOptions,
			int expectedCount)
		{
			StringEqualityOptions sut = new();
			sut.AsRegex(regexOptions);

			int result = await sut.CountOccurrences("b\nb", "^b");

			await That(result).IsEqualTo(expectedCount)
				.Because("without the multiline option '^' only binds to the start of the complete value");
		}

		[Fact]
		public async Task CountOccurrences_WhenPatternIsInvalid_ShouldThrowArgumentException()
		{
			StringEqualityOptions sut = new();
			sut.AsRegex();

			async Task Act() => await sut.CountOccurrences("foo", "[");

			await That(Act).Throws<ArgumentException>()
				.WithMessage("*[*").AsWildcard()
				.Because("an invalid pattern must still fail immediately, but the message is localized");
		}

		[Theory]
		[InlineData("bbb", 0)]
		[InlineData("abab", 2)]
		public async Task CountOccurrences_WhenPatternMatchesTheEmptyString_ShouldIgnoreEmptyMatches(string actual,
			int expectedCount)
		{
			StringEqualityOptions sut = new();
			sut.AsRegex();

			int result = await sut.CountOccurrences(actual, "a*");

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
				=> await That(sut).IsEqualTo("FOO\nBAR").AsRegex().IgnoringCase(ignoreCase);

			await That(Act).Throws<XunitException>().OnlyIf(!ignoreCase)
				.WithMessage("""
				             Expected that sut
				             matches regex "FOO\nBAR",
				             but it did not match:
				               ↓ (actual)
				               "foo\nbar"
				               "FOO\nBAR"
				               ↑ (regex pattern)
				             """).IgnoringNewlineStyle();
		}

		[Fact]
		public async Task ShouldDisplayActualAndPatternUnderneathEachOther()
		{
			string sut = "foo";

			async Task Act()
				=> await That(sut).IsEqualTo("bar").AsRegex();

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that sut
				             matches regex "bar",
				             but it did not match:
				               ↓ (actual)
				               "foo"
				               "bar"
				               ↑ (regex pattern)
				             """);
		}

		[Fact]
		public async Task ShouldReplaceNewlines()
		{
			string sut = "foo\nbar";

			async Task Act()
				=> await That(sut).IsEqualTo("\tsomething\r\nelse").AsRegex();

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that sut
				             matches regex "\tsomething\r\nelse",
				             but it did not match:
				               ↓ (actual)
				               "foo\nbar"
				               "\tsomething\r\nelse"
				               ↑ (regex pattern)
				             """).IgnoringNewlineStyle();
		}

		[Fact]
		public async Task ShouldSupportPassiveGrammaticalVoice()
		{
			Exception exception = new("foo");

			async Task Act()
				=> await That(() => Task.FromException(exception)).Throws().WithMessage("bar")
					.AsRegex();

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that () => Task.FromException(exception)
				             throws an exception with Message matching regex "bar",
				             but it did not match:
				               ↓ (actual)
				               "foo"
				               "bar"
				               ↑ (regex pattern)

				             Message:
				             foo
				             """);
		}

		[Fact]
		public async Task WhenIgnoringCase_ShouldCompareCaseInsensitive()
		{
			string sut = "foo";

			async Task Act()
				=> await That(sut).IsEqualTo("FOO").AsRegex().IgnoringCase();

			await That(Act).DoesNotThrow();
		}

		[Fact]
		public async Task WhenPatternIsNull_ShouldFail()
		{
			string sut = "foo";

			async Task Act()
				=> await That(sut).IsEqualTo(null).AsRegex();

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that sut
				             matches regex <null>,
				             but could not compare the <null> regex with "foo"
				             """);
		}

		[Fact]
		public async Task WhenSubjectAndPatternAreNull_ShouldFail()
		{
			string? sut = null;

			async Task Act()
				=> await That(sut).IsEqualTo(null).AsRegex();

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that sut
				             matches regex <null>,
				             but it was <null>
				             """);
		}

		[Fact]
		public async Task WhenSubjectIsNull_ShouldFail()
		{
			string? sut = null;

			async Task Act()
				=> await That(sut).IsEqualTo(".*").AsRegex();

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that sut
				             matches regex ".*",
				             but it was <null>
				             """);
		}
	}
}
