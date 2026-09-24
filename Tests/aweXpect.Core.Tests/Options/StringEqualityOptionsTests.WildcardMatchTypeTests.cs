using aweXpect.Options;

namespace aweXpect.Core.Tests.Options;

public sealed partial class StringEqualityOptionsTests
{
	public sealed class WildcardMatchTypeTests
	{
		/// <remarks>
		///     Each <c>*a</c> group can consume any number of the leading <c>a</c>s, and the trailing <c>b</c> never
		///     matches, so the engine has to try every split before it can give up.
		/// </remarks>
		private const string CatastrophicPattern = "*a*a*a*a*a*a*a*a*a*ab";

		[Fact]
		public async Task AreConsideredEqual_WhenExpectedIsNotAString_ShouldThrowArgumentNullException()
		{
			StringEqualityOptions sut = new();
			sut.AsWildcard();

			async Task Act() => await sut.AreConsideredEqual("42", 42);

			await That(Act).Throws<ArgumentNullException>()
				.WithParamName("expected").And
				.WithMessage("The 'expected' wildcard pattern cannot be null.").AsPrefix()
				.Because("a value that is not a string cannot be a pattern, so it is rejected like a missing one");
		}

		[Fact]
		public async Task AreConsideredEqual_WhenPatternDoesNotCompleteInTime_ShouldThrowArgumentException()
		{
			StringEqualityOptions sut = new();
			sut.AsWildcard();

			async Task Act() => await sut.AreConsideredEqual(new string('a', 100), CatastrophicPattern);

			await That(Act).Throws<ArgumentException>()
				.WithMessage(
					$"""The wildcard pattern "{CatastrophicPattern}" did not complete within 0:01. Simplify the pattern to avoid catastrophic backtracking.""")
				.AsPrefix().And
				.WithParamName("expected")
				.Because("the timeout must name the wildcard pattern the user wrote, not the translated regex");
		}

		[Theory]
		[InlineData("", true)]
		[InlineData("foo", false)]
		public async Task AreConsideredEqual_WhenPatternIsEmpty_ShouldMatchOnlyTheEmptyValue(string actual,
			bool expectMatch)
		{
			StringEqualityOptions sut = new();
			sut.AsWildcard();

			bool result = await sut.AreConsideredEqual(actual, "");

			await That(result).IsEqualTo(expectMatch)
				.Because("an empty wildcard pattern has the well-defined meaning of the empty string");
		}

		[Fact]
		public async Task AreConsideredEqual_WhenPatternIsNull_ShouldThrowArgumentNullException()
		{
			StringEqualityOptions sut = new();
			sut.AsWildcard();

			async Task Act() => await sut.AreConsideredEqual("foo", (string?)null);

			await That(Act).Throws<ArgumentNullException>()
				.WithParamName("expected").And
				.WithMessage("The 'expected' wildcard pattern cannot be null.").AsPrefix()
				.Because("the pattern is also rejected when the match type was set before it");
		}

		[Fact]
		public async Task AreConsideredEqual_WhenPatternIsNull_ShouldThrowBeforeTheTaskIsAwaited()
		{
			StringEqualityOptions sut = new();
			sut.AsWildcard();

			void Act() => _ = sut.AreConsideredEqual("foo", (string?)null);

			await That(Act).Throws<ArgumentNullException>()
				.WithParamName("expected").And
				.WithMessage("The 'expected' wildcard pattern cannot be null.").AsPrefix()
				.Because("an unusable pattern must throw at the call instead of inside the returned task");
		}

		[Fact]
		public async Task AreConsideredEqual_WithParameterName_WhenPatternIsNull_ShouldNameIt()
		{
			StringEqualityOptions sut = new("unexpected");
			sut.AsWildcard();

			async Task Act() => await sut.AreConsideredEqual("foo", (string?)null);

			await That(Act).Throws<ArgumentNullException>()
				.WithParamName("unexpected").And
				.WithMessage("The 'unexpected' wildcard pattern cannot be null.").AsPrefix()
				.Because("a negated expectation receives the pattern as 'unexpected'");
		}

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

		[Fact]
		public async Task CountOccurrences_WhenPatternDoesNotCompleteInTime_ShouldThrowArgumentException()
		{
			StringEqualityOptions sut = new();
			sut.AsWildcard();

			async Task Act() => await sut.CountOccurrences(new string('a', 100), CatastrophicPattern);

			await That(Act).Throws<ArgumentException>()
				.WithMessage(
					$"""The wildcard pattern "{CatastrophicPattern}" did not complete within 0:01. Simplify the pattern to avoid catastrophic backtracking.""")
				.AsPrefix().And
				.WithParamName("expected")
				.Because("counting the occurrences runs the same pattern and must fail the same way");
		}

		[Fact]
		public async Task CountOccurrences_WhenPatternIsEmpty_ShouldReturnZero()
		{
			StringEqualityOptions sut = new();
			sut.AsWildcard();

			int result = await sut.CountOccurrences("foo", "");

			await That(result).IsEqualTo(0)
				.Because("an empty expected value never occurs, but it is still a valid wildcard pattern");
		}

		[Fact]
		public async Task CountOccurrences_WhenPatternIsNull_ShouldThrowArgumentNullException()
		{
			StringEqualityOptions sut = new();
			sut.AsWildcard();

			async Task Act() => await sut.CountOccurrences("foo", null!);

			await That(Act).Throws<ArgumentNullException>()
				.WithParamName("expected").And
				.WithMessage("The 'expected' wildcard pattern cannot be null.").AsPrefix()
				.Because("a missing pattern is also meaningless when the occurrences are counted");
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
		[InlineData(false, "matches \"foo\"")]
		[InlineData(true, "matches \"foo\" ignoring case")]
		public async Task GetExpectation_ShouldRenderTheIgnoreCaseOption(bool ignoreCase, string expected)
		{
			StringEqualityOptions sut = new();
			sut.AsWildcard().IgnoringCase(ignoreCase);

			string result = sut.GetExpectation("foo", ExpectationGrammars.Active);

			await That(result).IsEqualTo(expected)
				.Because("an option that decides the outcome must not be invisible in the expectation");
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
				             but Message did not match:
				               ↓ (actual)
				               "foo"
				               "bar"
				               ↑ (wildcard pattern)

				             Message:
				             foo
				             """);
		}

		[Fact]
		public async Task WhenPatternIsNull_ShouldThrowArgumentNullException()
		{
			string sut = "foo";

			async Task Act()
				=> await That(sut).IsEqualTo(null).AsWildcard();

			await That(Act).Throws<ArgumentNullException>()
				.WithParamName("expected").And
				.WithMessage("The 'expected' wildcard pattern cannot be null.").AsPrefix()
				.Because("a missing pattern cannot express any expectation");
		}

		[Fact]
		public async Task WhenSubjectAndPatternAreNull_ShouldThrowArgumentNullException()
		{
			string? sut = null;

			async Task Act()
				=> await That(sut).IsEqualTo(null).AsWildcard();

			await That(Act).Throws<ArgumentNullException>()
				.WithParamName("expected").And
				.WithMessage("The 'expected' wildcard pattern cannot be null.").AsPrefix()
				.Because("a missing pattern is rejected before the subject is looked at, so that "
				         + "'is null' is never expressed through a pattern");
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
