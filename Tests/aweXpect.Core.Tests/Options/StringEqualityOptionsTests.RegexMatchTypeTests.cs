using System.Text.RegularExpressions;
using aweXpect.Options;

namespace aweXpect.Core.Tests.Options;

public sealed partial class StringEqualityOptionsTests
{
	public sealed class RegexMatchTypeTests
	{
		[Test]
		public async Task AreConsideredEqual_WhenExpectedIsNotAString_ShouldThrowArgumentNullException()
		{
			StringEqualityOptions sut = new("expected");
			sut.AsRegex();

			async Task Act() => await sut.AreConsideredEqual("42", 42);

			await That(Act).Throws<ArgumentNullException>()
				.WithParamName("expected").And
				.WithMessage("The 'expected' regex pattern cannot be null.").AsPrefix()
				.Because("a value that is not a string cannot be a pattern, so it is rejected like a missing one");
		}

		[Test]
		public async Task AreConsideredEqual_WhenPatternDoesNotCompleteInTime_ShouldThrowArgumentException()
		{
			StringEqualityOptions sut = new("expected");
			sut.AsRegex();

			async Task Act() => await sut.AreConsideredEqual(new string('a', 30) + "!", "(a+)+$");

			await That(Act).Throws<ArgumentException>()
				.WithMessage(
					"""The regex "(a+)+$" did not complete within 0:01. Simplify the pattern to avoid catastrophic backtracking.""")
				.AsPrefix().And
				.WithParamName("expected")
				.Because("the timeout must name the pattern instead of surfacing as a generic evaluation error");
		}

		[Test]
		public async Task AreConsideredEqual_WhenPatternDoesNotCompleteInTime_ShouldThrowOnlyWhenAwaited()
		{
			StringEqualityOptions sut = new("expected");
			sut.AsRegex();
			ValueTask<bool> task = sut.AreConsideredEqual(new string('a', 30) + "!", "(a+)+$");

			async Task Act() => await task;

			await That(Act).Throws<ArgumentException>()
				.WithMessage("""The regex "(a+)+$" did not complete within 0:01.*""").AsWildcard()
				.Because("only an unusable pattern is rejected at the call, the matching itself still fails the task");
		}

		[Test]
		public async Task AreConsideredEqual_WhenPatternIsEmpty_ShouldThrowArgumentException()
		{
			StringEqualityOptions sut = new("expected");
			sut.AsRegex();

			async Task Act() => await sut.AreConsideredEqual("foo", "");

			await That(Act).Throws<ArgumentException>()
				.WithMessage("The 'expected' regex pattern cannot be empty.").AsPrefix().And
				.WithParamName("expected")
				.Because("the pattern is also rejected when the match type was set before it");
		}

		[Test]
		public async Task AreConsideredEqual_WhenPatternIsEmpty_ShouldThrowBeforeTheTaskIsAwaited()
		{
			StringEqualityOptions sut = new("expected");
			sut.AsRegex();

#if NET8_0_OR_GREATER
			void Act() => _ = sut.AreConsideredEqual("foo", "").AsTask();
#else
			void Act() => _ = sut.AreConsideredEqual("foo", "");
#endif

			await That(Act).Throws<ArgumentException>()
				.WithMessage("The 'expected' regex pattern cannot be empty.").AsPrefix().And
				.WithParamName("expected")
				.Because("an unusable pattern must throw at the call instead of inside the returned task");
		}

		[Test]
		[Arguments("foo")]
		[Arguments(null)]
		public async Task AreConsideredEqual_WhenPatternIsInvalid_ShouldThrowArgumentException(string? actual)
		{
			StringEqualityOptions sut = new("expected");
			sut.AsRegex();

			async Task Act() => await sut.AreConsideredEqual(actual, "a(");

			await That(Act).Throws<ArgumentException>()
				.WithMessage($"The 'expected' regex pattern is invalid: {GetParseError("a(")}").AsPrefix().And
				.WithParamName("expected").And
				.WithInner<ArgumentException>(inner => inner.HasMessage(GetParseError("a(")))
				.Because("a broken pattern is rejected before the subject is looked at");
		}

		[Test]
		public async Task AreConsideredEqual_WhenPatternIsNull_ShouldThrowArgumentNullException()
		{
			StringEqualityOptions sut = new("expected");
			sut.AsRegex();

			async Task Act() => await sut.AreConsideredEqual("foo", (string?)null);

			await That(Act).Throws<ArgumentNullException>()
				.WithParamName("expected").And
				.WithMessage("The 'expected' regex pattern cannot be null.").AsPrefix()
				.Because("the pattern is also rejected when the match type was set before it");
		}

		[Test]
		public async Task AreConsideredEqual_WhenPatternIsNull_ShouldThrowBeforeTheTaskIsAwaited()
		{
			StringEqualityOptions sut = new("expected");
			sut.AsRegex();

#if NET8_0_OR_GREATER
			void Act() => _ = sut.AreConsideredEqual("foo", (string?)null).AsTask();
#else
			void Act() => _ = sut.AreConsideredEqual("foo", (string?)null);
#endif

			await That(Act).Throws<ArgumentNullException>()
				.WithParamName("expected").And
				.WithMessage("The 'expected' regex pattern cannot be null.").AsPrefix()
				.Because("an unusable pattern must throw at the call instead of inside the returned task");
		}

		[Test]
		[Arguments("forget", false)]
		[Arguments("get it", true)]
		[Arguments("it got ", true)]
		[Arguments("for get", true)]
		public async Task AreConsideredEqual_WhenWhiteSpaceIsIgnored_ShouldOnlyIgnoreTheWhiteSpaceOfThePatternAtTheEdgesOfTheSubject(
			string actual, bool expectMatch)
		{
			StringEqualityOptions sut = new("expected");
			sut.AsRegex().IgnoringLeadingWhiteSpace().IgnoringTrailingWhiteSpace();

			bool result = await sut.AreConsideredEqual(actual, " g.t ");

			await That(result).IsEqualTo(expectMatch)
				.Because("the regex may match any part of the subject, so its whitespace is only optional at the edges of the subject");
		}

		[Test]
		public async Task AreConsideredEqual_WithParameterName_WhenPatternDoesNotCompleteInTime_ShouldNameIt()
		{
			StringEqualityOptions sut = new("unexpected");
			sut.AsRegex();

			async Task Act() => await sut.AreConsideredEqual(new string('a', 30) + "!", "(a+)+$");

			await That(Act).Throws<ArgumentException>()
				.WithMessage("""The regex "(a+)+$" did not complete within 0:01.*""").AsWildcard().And
				.WithParamName("unexpected")
				.Because("a negated expectation receives the pattern as 'unexpected'");
		}

		[Test]
		public async Task AreConsideredEqual_WithParameterName_WhenPatternIsEmpty_ShouldNameIt()
		{
			StringEqualityOptions sut = new("unexpected");
			sut.AsRegex();

			async Task Act() => await sut.AreConsideredEqual("foo", "");

			await That(Act).Throws<ArgumentException>()
				.WithMessage("The 'unexpected' regex pattern cannot be empty.").AsPrefix().And
				.WithParamName("unexpected")
				.Because("a negated expectation receives the pattern as 'unexpected'");
		}

		[Test]
		public async Task AreConsideredEqual_WithParameterName_WhenPatternIsInvalid_ShouldNameIt()
		{
			StringEqualityOptions sut = new("unexpected");
			sut.AsRegex();

			async Task Act() => await sut.AreConsideredEqual("foo", "a(");

			await That(Act).Throws<ArgumentException>()
				.WithMessage($"The 'unexpected' regex pattern is invalid: {GetParseError("a(")}").AsPrefix().And
				.WithParamName("unexpected")
				.Because("a negated expectation receives the pattern as 'unexpected'");
		}

		[Test]
		public async Task AreConsideredEqual_WithParameterName_WhenPatternIsNull_ShouldNameIt()
		{
			StringEqualityOptions sut = new("unexpected");
			sut.AsRegex();

			async Task Act() => await sut.AreConsideredEqual("foo", (string?)null);

			await That(Act).Throws<ArgumentNullException>()
				.WithParamName("unexpected").And
				.WithMessage("The 'unexpected' regex pattern cannot be null.").AsPrefix()
				.Because("a negated expectation receives the pattern as 'unexpected'");
		}

		[Test]
		public async Task AsRegex_ShouldReturnSameInstance()
		{
			StringEqualityOptions sut = new("expected");

			StringEqualityOptions result = sut.AsRegex();

			await That(result).IsSameAs(sut);
		}

		[Test]
		[Arguments((RegexOptions)0x4000_0000)]
		[Arguments(RegexOptions.ECMAScript | RegexOptions.Singleline)]
		public async Task AsRegex_WithInvalidOptions_ShouldThrowAtTheCall(RegexOptions regexOptions)
		{
			StringEqualityOptions sut = new("expected");

			void Act() => sut.AsRegex(regexOptions);

			await That(Act).Throws<ArgumentOutOfRangeException>()
				.WithMessage($"The regex options '{regexOptions}' are not a valid combination.").AsPrefix().And
				.WithParamName("regexOptions")
				.Because("invalid options used to fail only when the pattern was matched");
		}

		[Test]
		public async Task AsRegex_WithOptions_ShouldReturnSameInstance()
		{
			StringEqualityOptions sut = new("expected");

			StringEqualityOptions result = sut.AsRegex(RegexOptions.Multiline);

			await That(result).IsSameAs(sut);
		}

		[Test]
		[Arguments("axxxb", "a.*b", 1)]
		[Arguments("abcabc", "[a-c]{3}", 2)]
		[Arguments("aaaa", "a+", 1)]
		[Arguments("a1b a22b", "a\\d+b", 2)]
		[Arguments("aXa", "a", 2)]
		[Arguments("aaaa", "aa", 2)]
		public async Task CountOccurrences_ShouldCountTheMatchesOfThePattern(string actual, string expected,
			int expectedCount)
		{
			StringEqualityOptions sut = new("expected");
			sut.AsRegex();

			int result = await sut.CountOccurrences(actual, expected);

			await That(result).IsEqualTo(expectedCount)
				.Because("the match can be longer or shorter than the pattern");
		}

		[Test]
		public async Task CountOccurrences_WhenCaseIsIgnored_ShouldIgnoreCase()
		{
			StringEqualityOptions sut = new("expected");
			sut.AsRegex().IgnoringCase();

			int result = await sut.CountOccurrences("AxB ayb", "a.b");

			await That(result).IsEqualTo(2);
		}

		[Test]
		[Arguments(RegexOptions.None, 1)]
		[Arguments(RegexOptions.Multiline, 2)]
		public async Task CountOccurrences_WhenOptionsAreGiven_ShouldApplyThem(RegexOptions regexOptions,
			int expectedCount)
		{
			StringEqualityOptions sut = new("expected");
			sut.AsRegex(regexOptions);

			int result = await sut.CountOccurrences("b\nb", "^b");

			await That(result).IsEqualTo(expectedCount)
				.Because("without the multiline option '^' only binds to the start of the complete value");
		}

		[Test]
		public async Task CountOccurrences_WhenPatternDoesNotCompleteInTime_ShouldThrowArgumentException()
		{
			StringEqualityOptions sut = new("expected");
			sut.AsRegex();

			async Task Act() => await sut.CountOccurrences(new string('a', 30) + "!", "(a+)+$");

			await That(Act).Throws<ArgumentException>()
				.WithMessage(
					"""The regex "(a+)+$" did not complete within 0:01. Simplify the pattern to avoid catastrophic backtracking.""")
				.AsPrefix().And
				.WithParamName("expected")
				.Because("counting the occurrences runs the same pattern and must fail the same way");
		}

		[Test]
		public async Task CountOccurrences_WhenPatternIsEmpty_ShouldThrowArgumentException()
		{
			StringEqualityOptions sut = new("expected");
			sut.AsRegex();

			async Task Act() => await sut.CountOccurrences("foo", "");

			await That(Act).Throws<ArgumentException>()
				.WithMessage("The 'expected' regex pattern cannot be empty.").AsPrefix().And
				.WithParamName("expected")
				.Because("an empty pattern is also meaningless when the occurrences are counted");
		}

		[Test]
		public async Task CountOccurrences_WhenPatternIsInvalid_ShouldThrowArgumentException()
		{
			StringEqualityOptions sut = new("expected");
			sut.AsRegex();

			async Task Act() => await sut.CountOccurrences("foo", "[");

			await That(Act).Throws<ArgumentException>()
				.WithMessage($"The 'expected' regex pattern is invalid: {GetParseError("[")}").AsPrefix().And
				.WithParamName("expected").And
				.WithInner<ArgumentException>(inner => inner.HasMessage(GetParseError("[")))
				.Because("counting the occurrences parses the same pattern and must fail the same way");
		}

		[Test]
		public async Task CountOccurrences_WhenPatternIsNull_ShouldThrowArgumentNullException()
		{
			StringEqualityOptions sut = new("expected");
			sut.AsRegex();

			async Task Act() => await sut.CountOccurrences("foo", null!);

			await That(Act).Throws<ArgumentNullException>()
				.WithParamName("expected").And
				.WithMessage("The 'expected' regex pattern cannot be null.").AsPrefix()
				.Because("a missing pattern is also meaningless when the occurrences are counted");
		}

		[Test]
		[Arguments("bbb", 0)]
		[Arguments("abab", 2)]
		public async Task CountOccurrences_WhenPatternMatchesTheEmptyString_ShouldIgnoreEmptyMatches(string actual,
			int expectedCount)
		{
			StringEqualityOptions sut = new("expected");
			sut.AsRegex();

			int result = await sut.CountOccurrences(actual, "a*");

			await That(result).IsEqualTo(expectedCount)
				.Because("an empty match does not cover any occurrence, just as an empty expected value never occurs");
		}

		[Test]
		[Arguments(false, "matches regex \"foo\"")]
		[Arguments(true, "matches regex \"foo\" ignoring case")]
		public async Task GetExpectation_ShouldRenderTheIgnoreCaseOption(bool ignoreCase, string expected)
		{
			StringEqualityOptions sut = new("expected");
			sut.AsRegex().IgnoringCase(ignoreCase);

			string result = sut.GetExpectation("foo", ExpectationGrammars.Active);

			await That(result).IsEqualTo(expected)
				.Because("an option that decides the outcome must not be invisible in the expectation");
		}

		[Test]
		public async Task GetExpectation_WhenNegatedAndPassive_ShouldSayNotMatching()
		{
			StringEqualityOptions sut = new("expected");
			sut.AsRegex();

			string result = sut.GetExpectation("foo", ExpectationGrammars.Negated);

			await That(result).IsEqualTo("not matching regex \"foo\"");
		}

		[Test]
		public async Task GetExtendedFailure_WhenPatternIsNull_ShouldStateThatItCouldNotCompare()
		{
			StringEqualityOptions sut = new("expected");
			sut.AsRegex();

			string result = sut.GetExtendedFailure("it", ExpectationGrammars.None, "foo", null);

			await That(result).IsEqualTo("could not compare the <null> regex with \"foo\"");
		}

		[Test]
		[Arguments(false)]
		[Arguments(true)]
		public async Task ShouldCompareCaseSensitive(bool ignoreCase)
		{
			string sut = "foo\nbar";

			async Task Act()
				=> await That(sut).IsEqualTo("FOO\nBAR").AsRegex().IgnoringCase(ignoreCase);

			await That(Act).Throws<FailException>().OnlyIf(!ignoreCase)
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

		[Test]
		public async Task ShouldDisplayActualAndPatternUnderneathEachOther()
		{
			string sut = "foo";

			async Task Act()
				=> await That(sut).IsEqualTo("bar").AsRegex();

			await That(Act).Throws<FailException>()
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

		[Test]
		public async Task ShouldReplaceNewlines()
		{
			string sut = "foo\nbar";

			async Task Act()
				=> await That(sut).IsEqualTo("\tsomething\r\nelse").AsRegex();

			await That(Act).Throws<FailException>()
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

		[Test]
		public async Task ShouldSupportPassiveGrammaticalVoice()
		{
			Exception exception = new("foo");

			async Task Act()
				=> await That(() => Task.FromException(exception)).Throws().WithMessage("bar")
					.AsRegex();

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that () => Task.FromException(exception)
				             throws an exception with message matching regex "bar",
				             but it had message "foo", which did not match:
				               ↓ (actual)
				               "foo"
				               "bar"
				               ↑ (regex pattern)

				             Message:
				             foo
				             """);
		}

		[Test]
		public async Task ToString_ShouldNameTheMatchType()
		{
			StringEqualityOptions sut = new("expected");
			sut.AsRegex();

			string result = sut.ToString();

			await That(result).IsEqualTo(" as regex");
		}

		[Test]
		public async Task WhenIgnoringCase_ShouldCompareCaseInsensitive()
		{
			string sut = "foo";

			async Task Act()
				=> await That(sut).IsEqualTo("FOO").AsRegex().IgnoringCase();

			await That(Act).DoesNotThrow();
		}

		[Test]
		public async Task WhenPatternIsInvalid_ShouldThrowArgumentException()
		{
			string sut = "foo";

			async Task Act()
				=> await That(sut).IsEqualTo("a(").AsRegex();

			await That(Act).Throws<ArgumentException>()
				.WithMessage($"The 'expected' regex pattern is invalid: {GetParseError("a(")}").AsPrefix().And
				.WithParamName("expected").And
				.WithInner<ArgumentException>(inner => inner.HasMessage(GetParseError("a(")));
		}

		[Test]
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

		[Test]
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

		[Test]
		public async Task WhenSubjectIsNull_ShouldFail()
		{
			string? sut = null;

			async Task Act()
				=> await That(sut).IsEqualTo(".*").AsRegex();

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that sut
				             matches regex ".*",
				             but it was <null>
				             """);
		}

		[Test]
		public async Task WhenSubjectIsNullAndPatternIsInvalid_ShouldThrowArgumentException()
		{
			string? sut = null;

			async Task Act()
				=> await That(sut).IsNotEqualTo("a(").AsRegex();

			await That(Act).Throws<ArgumentException>()
				.WithMessage($"The 'unexpected' regex pattern is invalid: {GetParseError("a(")}").AsPrefix().And
				.WithParamName("unexpected").And
				.WithInner<ArgumentException>(inner => inner.HasMessage(GetParseError("a(")))
				.Because("a broken pattern must not go unnoticed only because the subject is null");
		}

		/// <remarks>
		///     The parse error is localized and differs between the target frameworks.
		/// </remarks>
		private static string GetParseError(string pattern)
		{
			try
			{
				_ = new Regex(pattern);
			}
			catch (ArgumentException exception)
			{
				return exception.Message;
			}

			throw new InvalidOperationException($"The pattern '{pattern}' is valid.");
		}
	}
}
