using System.Collections.Generic;
using aweXpect.Customization;
using aweXpect.Options;
#if NETFRAMEWORK
using aweXpect.Core.Polyfills;
#endif

namespace aweXpect.Core.Tests.Options;

public sealed partial class StringEqualityOptionsTests
{
	public sealed class ExactMatchTypeTests
	{
		[Test]
		public async Task GetExpectation_WhenExpectedIsTruncated_ShouldNotSplitAnEscapeSequence()
		{
			StringEqualityOptions sut = new("expected");

			string result;
			using (Customize.aweXpect.Formatting().MaximumStringLength.Set(5))
			{
				result = sut.GetExpectation("abcd\nefgh", ExpectationGrammars.None);
			}

			await That(result).IsEqualTo("""
			                             equal to "abcd\n…"
			                             """)
				.Because("the limit applies to the characters of the expected value, not to their escaped form");
		}

		[Test]
		public async Task GetExtendedFailure_WhenExpectedIsEmptyAndSubjectIsOnlyWhitespace_ShouldNameTheWhitespace()
		{
			StringEqualityOptions sut = new("expected");

			string result = sut.GetExtendedFailure("it", ExpectationGrammars.None, "  ", "", MatchResult.NoMatch);

			await That(result).IsEqualTo("""
			                             it was "  ", which has unexpected whitespace ("  " at the beginning)
			                             """);
		}

		[Test]
		public async Task GetExtendedFailure_WhenSubjectIsEmptyAndExpectedIsOnlyWhitespace_ShouldNameTheWhitespace()
		{
			StringEqualityOptions sut = new("expected");

			string result = sut.GetExtendedFailure("it", ExpectationGrammars.None, "", "  ", MatchResult.NoMatch);

			await That(result).IsEqualTo("""
			                             it was "", which misses some whitespace ("  " at the beginning)
			                             """);
		}

		[Test]
		public async Task GetExtendedFailure_WhenWhitespaceIsTruncated_ShouldNotSplitAnEscapeSequence()
		{
			StringEqualityOptions sut = new("expected");

			string result;
			using (Customize.aweXpect.Formatting().MaximumStringLength.Set(4))
			{
				result = sut.GetExtendedFailure("it", ExpectationGrammars.None, "foo \n\n\n\n", "foo", MatchResult.NoMatch);
			}

			await That(result).IsEqualTo("""
			                             it was "foo …", which has unexpected whitespace (" \n\n\n…" at the end)
			                             """)
				.Because("the limit applies to the whitespace characters, not to their escaped form");
		}

		[Test]
		[Arguments(false)]
		[Arguments(true)]
		public async Task ShouldCompareCaseSensitive(bool ignoreCase)
		{
			string sut = "foo\nbar";

			async Task Act()
				=> await That(sut).IsEqualTo("FOO\nBAR").IgnoringCase(ignoreCase);

			await That(Act).Throws<FailException>().OnlyIf(!ignoreCase)
				.WithMessage("""
				             Expected that sut
				             is equal to "FOO\nBAR",
				             but it was "foo\nbar", which differs on line 1 and column 1:
				                ↓ (actual)
				               "foo\nbar"
				               "FOO\nBAR"
				                ↑ (expected)
				             """).IgnoringNewlineStyle();
		}

		[Test]
		public async Task ShouldDisplayActualAndPatternUnderneathEachOther()
		{
			string sut = "foo";

			async Task Act()
				=> await That(sut).IsEqualTo("bar");

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that sut
				             is equal to "bar",
				             but it was "foo", which differs at index 0:
				                ↓ (actual)
				               "foo"
				               "bar"
				                ↑ (expected)
				             """);
		}

		[Test]
		public async Task ShouldReplaceNewlines()
		{
			string sut = "foo\nbar";

			async Task Act()
				=> await That(sut).IsEqualTo("\tsomething\r\nelse");

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that sut
				             is equal to "\tsomething\r\nelse",
				             but it was "foo\nbar", which differs on line 1 and column 1:
				                ↓ (actual)
				               "foo\nbar"
				               "\tsomething\r\nelse"
				                ↑ (expected)
				             """).IgnoringNewlineStyle();
		}

		[Test]
		public async Task ShouldSupportPassiveGrammaticalVoice()
		{
			Exception exception = new("foo");

			async Task Act()
				=> await That(() => Task.FromException(exception)).Throws().WithMessage("bar");

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that () => Task.FromException(exception)
				             throws an exception with message equal to "bar",
				             but it had message "foo", which differs at index 0:
				                ↓ (actual)
				               "foo"
				               "bar"
				                ↑ (expected)

				             Message:
				             foo
				             """);
		}

		[Test]
		public async Task WhenComparerThrowsForTheNormalizedValues_ShouldNotHintAtIgnoringNewlineStyle()
		{
			string sut = "a\r\nb";

			async Task Act()
				=> await That(sut).IsEqualTo("a\rb").Using(new CarriageReturnComparer());

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that sut
				             is equal to "a\rb" using StringEqualityOptionsTests.ExactMatchTypeTests.CarriageReturnComparer,
				             but it was "a\r\nb", which differs on line 1 and column 3:
				                   ↓ (actual)
				               "a\r\nb"
				               "a\rb"
				                   ↑ (expected)
				             """)
				.Because("a comparer that throws for the normalized values must neither hide the difference nor get the hint");
		}

		[Test]
		public async Task WhenExpectedIsNull_ShouldFail()
		{
			string sut = "foo";

			async Task Act()
				=> await That(sut).IsEqualTo(null);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that sut
				             is equal to <null>,
				             but it was "foo"
				             """);
		}

		[Test]
		public async Task WhenIgnoringCase_ShouldCompareCaseInsensitive()
		{
			string sut = "foo";

			async Task Act()
				=> await That(sut).IsEqualTo("FOO").IgnoringCase();

			await That(Act).DoesNotThrow();
		}

		[Test]
		public async Task WhenIgnoringNewlineStyle_ShouldNotHintAtIt()
		{
			string sut = "foo\r\nbar";

			async Task Act()
				=> await That(sut).IsEqualTo("foo\nbaz").IgnoringNewlineStyle();

			await That(Act).Throws<FailException>()
				.WithMessage($"""
				              Expected that sut
				              is equal to "foo\nbaz" ignoring newline style,
				              but it was "foo\nbar", which differs on line 2 and column 3:
				                        ↓ (actual)
				                "foo\nbar"
				                "foo\nbaz"
				                        ↑ (expected)

				              Actual:
				              {sut}
				              """);
		}

		[Test]
		public async Task WhenNegated_ShouldNotHintAtIgnoringNewlineStyle()
		{
			string sut = "foo\r\nbar";

			async Task Act()
				=> await That(sut).IsNotEqualTo("foo\r\nbar");

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that sut
				             is not equal to "foo\r\nbar",
				             but it was "foo\r\nbar"
				             """)
				.Because("a negated expectation only fails for equal values");
		}

		[Test]
		public async Task WhenOnlyTheNewlineStyleDiffers_AndCaseIsIgnored_ShouldHintAtIgnoringNewlineStyle()
		{
			string sut = "FOO\r\nbar";

			async Task Act()
				=> await That(sut).IsEqualTo("foo\nBAR").IgnoringCase();

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that sut
				             is equal to "foo\nBAR" ignoring case,
				             but it was "FOO\r\nbar", which differs on line 1 and column 4:
				                   ↓ (actual)
				               "FOO\r\nbar"
				               "foo\nBAR"
				                   ↑ (expected)
				             (but the values match when ignoring the newline style)
				             """);
		}

		[Test]
		public async Task WhenOnlyTheNewlineStyleDiffers_AndComparerIsUsed_ShouldHintAtIgnoringNewlineStyle()
		{
			string sut = "FOO\r\nbar";

			async Task Act()
				=> await That(sut).IsEqualTo("foo\nBAR").Using(new CaseInsensitiveComparer());

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that sut
				             is equal to "foo\nBAR" using StringEqualityOptionsTests.ExactMatchTypeTests.CaseInsensitiveComparer,
				             but it was "FOO\r\nbar", which differs on line 1 and column 4:
				                   ↓ (actual)
				               "FOO\r\nbar"
				               "foo\nBAR"
				                   ↑ (expected)
				             (but the values match when ignoring the newline style)
				             """);
		}

		[Test]
		public async Task WhenOnlyTheNewlineStyleDiffers_AndSubjectIsShorter_ShouldHintAtIgnoringNewlineStyle()
		{
			string sut = "foo\r";

			async Task Act()
				=> await That(sut).IsEqualTo("foo\r\n");

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that sut
				             is equal to "foo\r\n",
				             but it was "foo\r" with a length of 4, which is shorter than the expected length of 5 and misses:
				               "\n"
				             (but the values match when ignoring the newline style)
				             """)
				.Because("a lone \\r is a newline of its own");
		}

		[Test]
		public async Task WhenOnlyTheNewlineStyleDiffers_AndTrailingWhiteSpaceIsIgnored_ShouldHintAtIgnoringNewlineStyle()
		{
			string sut = "foo\r\nbar\r\n";

			async Task Act()
				=> await That(sut).IsEqualTo("foo\nbar").IgnoringTrailingWhiteSpace();

			await That(Act).Throws<FailException>()
				.WithMessage($"""
				              Expected that sut
				              is equal to "foo\nbar" ignoring trailing whitespace,
				              but it was "foo\r\nbar", which differs on line 1 and column 4:
				                    ↓ (actual)
				                "foo\r\nbar"
				                "foo\nbar"
				                    ↑ (expected)
				              (but the values match when ignoring the newline style)

				              Actual:
				              {sut}
				              """);
		}

		[Test]
		public async Task WhenOnlyTheNewlineStyleDiffers_InTheExceptionMessage_ShouldHintAtIgnoringNewlineStyle()
		{
			Exception exception = new("foo\r\nbar");

			async Task Act()
				=> await That(() => Task.FromException(exception)).Throws().WithMessage("foo\nbar");

			await That(Act).Throws<FailException>()
				.WithMessage($"""
				              Expected that () => Task.FromException(exception)
				              throws an exception with message equal to "foo\nbar",
				              but it had message "foo\r\nbar", which differs on line 1 and column 4:
				                    ↓ (actual)
				                "foo\r\nbar"
				                "foo\nbar"
				                    ↑ (expected)
				              (but the values match when ignoring the newline style)

				              Message:
				              {exception.Message}
				              """);
		}

		[Test]
		public async Task WhenOnlyTheNewlineStyleDiffers_WithCrInSubject_ShouldHintAtIgnoringNewlineStyle()
		{
			string sut = "foo\rbar";

			async Task Act()
				=> await That(sut).IsEqualTo("foo\nbar");

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that sut
				             is equal to "foo\nbar",
				             but it was "foo\rbar", which differs at index 3:
				                   ↓ (actual)
				               "foo\rbar"
				               "foo\nbar"
				                   ↑ (expected)
				             (but the values match when ignoring the newline style)
				             """);
		}

		[Test]
		public async Task WhenOnlyTheNewlineStyleDiffers_WithCrLfInExpected_ShouldHintAtIgnoringNewlineStyle()
		{
			string sut = "foo\nbar";

			async Task Act()
				=> await That(sut).IsEqualTo("foo\r\nbar");

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that sut
				             is equal to "foo\r\nbar",
				             but it was "foo\nbar", which differs on line 1 and column 4:
				                   ↓ (actual)
				               "foo\nbar"
				               "foo\r\nbar"
				                   ↑ (expected)
				             (but the values match when ignoring the newline style)
				             """);
		}

		[Test]
		public async Task WhenOnlyTheNewlineStyleDiffers_WithCrLfInSubject_ShouldHintAtIgnoringNewlineStyle()
		{
			string sut = "foo\r\nbar";

			async Task Act()
				=> await That(sut).IsEqualTo("foo\nbar");

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that sut
				             is equal to "foo\nbar",
				             but it was "foo\r\nbar", which differs on line 1 and column 4:
				                   ↓ (actual)
				               "foo\r\nbar"
				               "foo\nbar"
				                   ↑ (expected)
				             (but the values match when ignoring the newline style)
				             """);
		}

		[Test]
		public async Task WhenSubjectAndExpectedAreNull_ShouldSucceed()
		{
			string? sut = null;

			async Task Act()
				=> await That(sut).IsEqualTo(null);

			await That(Act).DoesNotThrow();
		}

		[Test]
		public async Task WhenSubjectIsLonger_ShouldEscapeTheSuperfluousText()
		{
			string sut = "foo\r\nbar";

			async Task Act()
				=> await That(sut).IsEqualTo("foo");

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that sut
				             is equal to "foo",
				             but it was "foo\r\nbar" with a length of 8, which is longer than the expected length of 3 and has superfluous:
				               "\r\nbar"
				             """).IgnoringNewlineStyle()
				.Because("the superfluous text is escaped like the other values in the message");
		}

		[Test]
		public async Task WhenSubjectIsNull_AndExpectedHasANewline_ShouldNotHintAtIgnoringNewlineStyle()
		{
			string? sut = null;

			async Task Act()
				=> await That(sut).IsEqualTo("foo\r\nbar");

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that sut
				             is equal to "foo\r\nbar",
				             but it was <null>
				             """);
		}

		[Test]
		public async Task WhenSubjectIsNull_ShouldFail()
		{
			string? sut = null;

			async Task Act()
				=> await That(sut).IsEqualTo("");

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that sut
				             is equal to "",
				             but it was <null>
				             """);
		}

		[Test]
		public async Task WhenSubjectIsShorter_ShouldEscapeTheMissingText()
		{
			string sut = "foo";

			async Task Act()
				=> await That(sut).IsEqualTo("foo\n\"bar\"");

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that sut
				             is equal to "foo\n\"bar\"",
				             but it was "foo" with a length of 3, which is shorter than the expected length of 9 and misses:
				               "\n\"bar\""
				             """).IgnoringNewlineStyle()
				.Because("the missing text is escaped like the other values in the message");
		}

		[Test]
		public async Task WhenTheCaseAlsoDiffers_ShouldNotHintAtIgnoringNewlineStyle()
		{
			string sut = "FOO\r\nbar";

			async Task Act()
				=> await That(sut).IsEqualTo("foo\nbar");

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that sut
				             is equal to "foo\nbar",
				             but it was "FOO\r\nbar", which differs on line 1 and column 1:
				                ↓ (actual)
				               "FOO\r\nbar"
				               "foo\nbar"
				                ↑ (expected)
				             """);
		}

		[Test]
		public async Task WhenTheValuesAlsoDifferOtherwise_ShouldNotHintAtIgnoringNewlineStyle()
		{
			string sut = "foo\r\nbar";

			async Task Act()
				=> await That(sut).IsEqualTo("foo\nbaz");

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that sut
				             is equal to "foo\nbaz",
				             but it was "foo\r\nbar", which differs on line 1 and column 4:
				                   ↓ (actual)
				               "foo\r\nbar"
				               "foo\nbaz"
				                   ↑ (expected)
				             """);
		}

		private sealed class CarriageReturnComparer : IEqualityComparer<string>
		{
			public bool Equals(string? x, string? y)
				=> x!.Contains('\r', StringComparison.Ordinal) && y!.Contains('\r', StringComparison.Ordinal)
					? string.Equals(x, y, StringComparison.Ordinal)
					: throw new InvalidOperationException("Only values with a carriage return are supported.");

			public int GetHashCode(string obj)
				=> obj.GetHashCode();
		}

		private sealed class CaseInsensitiveComparer : IEqualityComparer<string>
		{
			public bool Equals(string? x, string? y)
				=> string.Equals(x, y, StringComparison.OrdinalIgnoreCase);

			public int GetHashCode(string obj)
				=> StringComparer.OrdinalIgnoreCase.GetHashCode(obj);
		}
	}
}
