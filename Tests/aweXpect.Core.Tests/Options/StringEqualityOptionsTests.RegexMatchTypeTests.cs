using System.Text.RegularExpressions;
using aweXpect.Options;

namespace aweXpect.Core.Tests.Options;

public sealed partial class StringEqualityOptionsTests
{
	public sealed class RegexMatchTypeTests
	{
		[Fact]
		public async Task AreConsideredEqual_WhenPatternDoesNotCompleteInTime_ShouldThrowArgumentException()
		{
			StringEqualityOptions sut = new();
			sut.AsRegex();

			async Task Act() => await sut.AreConsideredEqual(new string('a', 30) + "!", "(a+)+$");

			await That(Act).Throws<ArgumentException>()
				.WithMessage(
					"""The regex "(a+)+$" did not complete within 0:01. Simplify the pattern to avoid catastrophic backtracking.""")
				.AsPrefix().And
				.WithParamName("expected")
				.Because("the timeout must name the pattern instead of surfacing as a generic evaluation error");
		}

		[Fact]
		public async Task AreConsideredEqual_WhenPatternIsEmpty_ShouldThrowArgumentException()
		{
			StringEqualityOptions sut = new();
			sut.AsRegex();

			async Task Act() => await sut.AreConsideredEqual("foo", "");

			await That(Act).Throws<ArgumentException>()
				.WithMessage("The 'expected' regex pattern cannot be empty.").AsPrefix().And
				.WithParamName("expected")
				.Because("the pattern is also rejected when the match type was set before it");
		}

		[Fact]
		public async Task AreConsideredEqual_WhenPatternIsNull_ShouldThrowArgumentNullException()
		{
			StringEqualityOptions sut = new();
			sut.AsRegex();

			async Task Act() => await sut.AreConsideredEqual("foo", (string?)null);

			await That(Act).Throws<ArgumentNullException>()
				.WithParamName("expected").And
				.WithMessage("The 'expected' regex pattern cannot be null.").AsPrefix()
				.Because("the pattern is also rejected when the match type was set before it");
		}

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
		public async Task CountOccurrences_WhenPatternDoesNotCompleteInTime_ShouldThrowArgumentException()
		{
			StringEqualityOptions sut = new();
			sut.AsRegex();

			async Task Act() => await sut.CountOccurrences(new string('a', 30) + "!", "(a+)+$");

			await That(Act).Throws<ArgumentException>()
				.WithMessage(
					"""The regex "(a+)+$" did not complete within 0:01. Simplify the pattern to avoid catastrophic backtracking.""")
				.AsPrefix().And
				.WithParamName("expected")
				.Because("counting the occurrences runs the same pattern and must fail the same way");
		}

		[Fact]
		public async Task CountOccurrences_WhenPatternIsEmpty_ShouldThrowArgumentException()
		{
			StringEqualityOptions sut = new();
			sut.AsRegex();

			async Task Act() => await sut.CountOccurrences("foo", "");

			await That(Act).Throws<ArgumentException>()
				.WithMessage("The 'expected' regex pattern cannot be empty.").AsPrefix().And
				.WithParamName("expected")
				.Because("an empty pattern is also meaningless when the occurrences are counted");
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

		[Fact]
		public async Task CountOccurrences_WhenPatternIsNull_ShouldThrowArgumentNullException()
		{
			StringEqualityOptions sut = new();
			sut.AsRegex();

			async Task Act() => await sut.CountOccurrences("foo", null!);

			await That(Act).Throws<ArgumentNullException>()
				.WithParamName("expected").And
				.WithMessage("The 'expected' regex pattern cannot be null.").AsPrefix()
				.Because("a missing pattern is also meaningless when the occurrences are counted");
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
		public async Task WhenPatternIsNull_ShouldThrowArgumentNullException()
		{
			string sut = "foo";

			async Task Act()
				=> await That(sut).IsEqualTo(null).AsRegex();

			await That(Act).Throws<ArgumentNullException>()
				.WithParamName("expected").And
				.WithMessage("The 'expected' regex pattern cannot be null.").AsPrefix()
				.Because("a missing pattern cannot express any expectation");
		}

		[Fact]
		public async Task WhenSubjectAndPatternAreNull_ShouldThrowArgumentNullException()
		{
			string? sut = null;

			async Task Act()
				=> await That(sut).IsEqualTo(null).AsRegex();

			await That(Act).Throws<ArgumentNullException>()
				.WithParamName("expected").And
				.WithMessage("The 'expected' regex pattern cannot be null.").AsPrefix()
				.Because("a missing pattern is rejected before the subject is looked at, so that "
				         + "'is null' is never expressed through a pattern");
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
