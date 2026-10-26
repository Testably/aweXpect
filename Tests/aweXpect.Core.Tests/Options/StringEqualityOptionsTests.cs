using System.Collections.Generic;
using System.Text.RegularExpressions;
using aweXpect.Core.Helpers;
using aweXpect.Core.Tests.TestHelpers;
using aweXpect.Options;

namespace aweXpect.Core.Tests.Options;

public sealed partial class StringEqualityOptionsTests
{
	public sealed class Tests
	{
		[Test]
		public async Task AreConsideredEqual_Null_ShouldReturnFalse()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.IgnoringLeadingWhiteSpace();
			sut = sut.IgnoringTrailingWhiteSpace();
			sut = sut.IgnoringNewlineStyle();

			bool result = await sut.AreConsideredEqual(null, "foo", "expected");

			await That(result).IsFalse();
		}

		[Test]
		public async Task AreConsideredEqual_WhenAComparerIsUsed_ShouldStillApplyTheWhiteSpaceOptions()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.Using(StringComparer.Ordinal).IgnoringLeadingWhiteSpace();

			bool result = await sut.AreConsideredEqual("  foo", "foo", "expected");

			await That(result).IsTrue()
				.Because("both values are normalized before the comparer sees them, so neither option is dropped");
		}

		[Test]
		[Arguments(false, "bar")]
		[Arguments(true, "bar")]
		[Arguments(false, 42)]
		[Arguments(true, 42)]
		public async Task AreConsideredEqual_WhenACustomMatchTypeCannotCompareTheSubject_ShouldThrowUserCodeException(
			bool completesLater, object expected)
		{
			FormatException cause = new("no number");
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.SetMatchType(new DelegatingMatchType(completesLater
				? async () =>
				{
					await Task.Yield();
					return MatchResult.NotComparable("is not comparable", cause);
				}
				: () => new ValueTask<MatchResult>(
					MatchResult.NotComparable("is not comparable", cause))), "AsCustom");

			async Task Act() => await ((IEqualityEvaluator<string?>)sut).AreConsideredEqual("foo", expected);

			await That(Act).Throws<UserCodeException>()
				.WithMessage("The code of the caller threw an exception while the expectation was evaluated.").And
				.Whose(e => e.Thrower, thrower => thrower.IsNull()).And
				.WithInner<NotComparableException>(inner => inner
					.HasMessage("is not comparable").And
					.Whose(e => e.InnerException, innerCause => innerCause.IsSameAs(cause)))
				.Because("the evaluation fails both ways for a subject that the match type cannot compare, in every consumer");
		}

		[Test]
		public async Task AreConsideredEqual_WhenACustomMatchTypeCompletesLater_ShouldAwaitIt()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.SetMatchType(new DelegatingMatchType(async () =>
			{
				await Task.Yield();
				return MatchResult.Match;
			}), "AsCustom");

			bool result = await sut.AreConsideredEqual("foo", "bar", "expected");

			await That(result).IsTrue();
		}

		[Test]
		[Arguments("bar", "bar")]
		[Arguments("bar  ", "bar")]
		[Arguments(null, null)]
		[Arguments(42, null)]
		public async Task AreConsideredEqual_WhenACustomMatchTypeRejectsTheExpectedValue_ShouldThrowAtTheCall(
			object? expected, string? validatedExpected)
		{
			ArgumentException exception = new("my rejection");
			ValidatingMatchType matchType = new(rejectExpected: _ => exception);
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.IgnoringTrailingWhiteSpace();
			sut = sut.SetMatchType(matchType, "AsCustom");

			void Act() => ((IEqualityEvaluator<string?>)sut).AreConsideredEqual("foo", expected).AsTask();

			ArgumentException thrownException = await That(Act).Throws<ArgumentException>()
				.Because("an unusable expected value must not only throw when the returned task is awaited");
			await That(thrownException).IsSameAs(exception);
			await That(matchType.ValidatedExpected).IsEqualTo([validatedExpected,]);
			await That(matchType.ComparedExpected).IsEmpty();
		}

		[Test]
		public async Task AreConsideredEqual_WhenACustomMatchTypeTimesOut_ShouldNameItsTimeout()
		{
			RegexMatchTimeoutException exception = new("foo", "bar", TimeSpan.FromSeconds(3));
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.SetMatchType(new DelegatingMatchType(() => throw exception), "AsCustom");

			async Task Act() => await sut.AreConsideredEqual("foo", "bar", "expected");

			await That(Act).Throws<ArgumentException>()
				.WithMessage(
					"""The pattern "bar" did not complete within 0:03. Simplify the pattern to avoid catastrophic backtracking.""")
				.AsPrefix()
				.Because("the failure must name the timeout that applied instead of the configured default");
		}

		[Test]
		[Arguments(false)]
		[Arguments(true)]
		public async Task AreConsideredEqual_WhenACustomMatchTypeTimesOut_ShouldThrowArgumentException(bool completesLater)
		{
			RegexMatchTimeoutException exception = new("foo", "bar", TimeSpan.FromSeconds(1));
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.SetMatchType(new DelegatingMatchType(completesLater
				? async () =>
				{
					await Task.Yield();
					throw exception;
				}
				: () => throw exception), "AsCustom");

			async Task Act() => await sut.AreConsideredEqual("foo", "bar", "expected");

			await That(Act).Throws<ArgumentException>()
				.WithMessage(
					"""The pattern "bar" did not complete within 0:01. Simplify the pattern to avoid catastrophic backtracking.""")
				.AsPrefix().And
				.WithParamName("expected").And
				.WithInner<RegexMatchTimeoutException>(inner => inner.IsSameAs(exception))
				.Because("a timeout of a custom match type must be reported like one of a pattern");
		}

		[Test]
		public async Task AreConsideredEqual_WhenIndentationIsIgnored_ShouldEmptyWhiteSpaceOnlyLines()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.IgnoringIndentation();

			bool result = await sut.AreConsideredEqual("foo\n    ", "foo\n", "expected");

			await That(result).IsTrue();
		}

		[Test]
		[Arguments("foo", "foo")]
		[Arguments("    foo", "foo")]
		[Arguments("\tfoo", "foo")]
		[Arguments("class C\n{\n    Foo();\n}", "class C\n{\nFoo();\n}")]
		[Arguments("class C\r\n{\r\n    Foo();\r\n}", "class C\n{\nFoo();\n}")]
		[Arguments("a\n   \nb", "a\n\nb")]
		[Arguments("\n  foo", "\nfoo")]
		public async Task AreConsideredEqual_WhenIndentationIsIgnored_ShouldRemoveLeadingWhiteSpacePerLine(
			string actual, string expected)
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.IgnoringIndentation();

			bool result = await sut.AreConsideredEqual(actual, expected, "expected");

			await That(result).IsTrue();
		}

		[Test]
		public async Task AreConsideredEqual_WhenIndentationIsIgnored_ShouldStillConsiderTrailingWhiteSpace()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.IgnoringIndentation();

			bool result = await sut.AreConsideredEqual("foo  \nbar", "foo\nbar", "expected");

			await That(result).IsFalse();
		}

		[Test]
		[Arguments("AsBlock")]
		[Arguments("AsPrefix")]
		[Arguments("AsSuffix")]
		[Arguments("Containing")]
		[Arguments("Exact")]
		public async Task AreConsideredEqual_WhenTheComparerThrows_ShouldNameTheComparer(string matchType)
		{
			InvalidOperationException exception = new("comparer failed");
			StringEqualityOptions sut = WithMatchType(matchType);
			sut = sut.Using(new ThrowingComparer(exception));

			async Task Act()
				=> await sut.AreConsideredEqual("foo", "foo", "expected");

			await That(Act).Throws<UserCodeException>()
				.WithMessage("The code of the caller threw an exception while the expectation was evaluated.").And
				.Whose(e => e.Thrower, thrower => thrower.IsEqualTo("the comparer")).And
				.Whose(e => e.InnerException, inner => inner.IsSameAs(exception))
				.Because("the failure has to blame the comparer for every match type that consults it");
		}

		[Test]
		[Arguments("AsRegex", "f.*", "b.*")]
		[Arguments("AsWildcard", "f*", "b*")]
		public async Task AreConsideredEqual_WhenThePatternOrTheCasingChanges_ShouldParseThePatternAgain(
			string matchType, string matchingPattern, string otherPattern)
		{
			StringEqualityOptions sut = WithMatchType(matchType);

			bool matchingResult = await sut.AreConsideredEqual("foo", matchingPattern, "expected");
			bool otherResult = await sut.AreConsideredEqual("foo", otherPattern, "expected");
			bool caseSensitiveResult = await sut.AreConsideredEqual("FOO", matchingPattern, "expected");
			sut = sut.IgnoringCase();
			bool ignoringCaseResult = await sut.AreConsideredEqual("FOO", matchingPattern, "expected");

			await That(matchingResult).IsTrue();
			await That(otherResult).IsFalse()
				.Because("the pattern that was parsed for the previous comparison must not be reused for another one");
			await That(caseSensitiveResult).IsFalse();
			await That(ignoringCaseResult).IsTrue()
				.Because("the casing is part of the parsed pattern");
		}

		[Test]
		[Arguments("Exact", "SELECT  *\r\n  FROM t", "SELECT * FROM t", true)]
		[Arguments("Exact", " a\tb\n", "a b", true)]
		[Arguments("Exact", "a  b", "a  b", true)]
		[Arguments("Exact", "", " \t ", true)]
		[Arguments("Exact", "ab", "a b", false)]
		[Arguments("Exact", "a b", "ab", false)]
		[Arguments("Exact", null, " ", false)]
		[Arguments("AsPrefix", "SELECT * FROM t", "SELECT  *", true)]
		[Arguments("AsPrefix", "  SELECT", "SELECT ", true)]
		[Arguments("AsPrefix", "SELECT\n*", "SELECT ", true)]
		[Arguments("AsPrefix", "SELECTED", "SELECT ", false)]
		[Arguments("AsSuffix", "x = 1", "  =  1", true)]
		[Arguments("AsSuffix", "=1 ", " =1", true)]
		[Arguments("AsSuffix", "x=1", " =1", false)]
		[Arguments("Containing", "a  b\n c", "b c", true)]
		[Arguments("Containing", "a\tbc", " b", true)]
		[Arguments("Containing", "abc", " b", false)]
		[Arguments("AsWildcard", "SELECT  *\n  FROM t", "SELECT * FROM *", true)]
		[Arguments("AsWildcard", "SELECT *FROM t", "SELECT * FROM *", false)]
		[Arguments("AsRegex", "a  \n b", "^a b$", true)]
		[Arguments("AsRegex", "a\nb", @"a\nb", false)]
		public async Task AreConsideredEqual_WhenWhiteSpaceIsNormalized_ShouldCompareTheCollapsedValues(
			string matchType, string? actual, string expected, bool isEqual)
		{
			StringEqualityOptions sut = WithMatchType(matchType);
			sut = sut.NormalizingWhiteSpace();

			bool result = await sut.AreConsideredEqual(actual, expected, "expected");

			await That(result).IsEqualTo(isEqual)
				.Because("every run of whitespace counts as a single space, and the whitespace of the expected value at an "
				         + "inner end has to match whitespace in the subject, unless it reaches the end of the subject");
		}

		[Test]
		[Arguments(nameof(StringEqualityOptions.IgnoringIndentation))]
		[Arguments(nameof(StringEqualityOptions.IgnoringLeadingWhiteSpace))]
		[Arguments(nameof(StringEqualityOptions.IgnoringNewlineStyle))]
		[Arguments(nameof(StringEqualityOptions.IgnoringTrailingWhiteSpace))]
		public async Task AreConsideredEqual_WhenWhiteSpaceIsNormalized_ShouldSupersedeTheOtherWhiteSpaceOptions(
			string option)
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = Change(sut, option, true);
			sut = sut.NormalizingWhiteSpace();

			bool isEqual = await sut.AreConsideredEqual("  a \r\n\t b  ", "a b", "expected");
			bool isEqualWithoutWhiteSpace = await sut.AreConsideredEqual("a  b", "ab", "expected");

			await That(isEqual).IsTrue();
			await That(isEqualWithoutWhiteSpace).IsFalse()
				.Because("the other whitespace options only remove whitespace that the normalization collapses anyway");
		}

		[Test]
		[Arguments("foo", "foo", true)]
		[Arguments("foo", "FOO", false)]
		[Arguments("foo", " foo", false)]
		[Arguments("foo", "", false)]
		[Arguments("", "", true)]
		[Arguments("foo", null, false)]
		[Arguments(null, "foo", false)]
		[Arguments(null, null, true)]
		public async Task AreConsideredEqual_WithoutOptions_ShouldCompareOrdinally(
			string? actual, string? expected, bool isEqual)
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			StringEqualityOptions withComparer = StringEqualityOptions.Default;
			withComparer = withComparer.Using(StringComparer.Ordinal);

			bool result = await sut.AreConsideredEqual(actual, expected, "expected");
			bool resultAsObject = await ((IEqualityEvaluator<string?>)sut).AreConsideredEqual<object?>(actual, expected);
			bool resultWithComparer = await withComparer.AreConsideredEqual(actual, expected, "expected");

			await That(result).IsEqualTo(isEqual);
			await That(resultAsObject).IsEqualTo(isEqual);
			await That(resultWithComparer).IsEqualTo(isEqual)
				.Because("the ordinal comparer decides the same without the options comparing ordinally themselves");
		}

		[Test]
		[Arguments("1", false)]
		[Arguments("", false)]
		[Arguments(null, true)]
		public async Task AreConsideredEqual_WithoutOptions_WhenExpectedIsNoString_ShouldCompareWithNull(
			string? actual, bool isEqual)
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			StringEqualityOptions withComparer = StringEqualityOptions.Default;
			withComparer = withComparer.Using(StringComparer.Ordinal);

			bool result = await ((IEqualityEvaluator<string?>)sut).AreConsideredEqual(actual, 1);
			bool resultWithComparer = await ((IEqualityEvaluator<string?>)withComparer).AreConsideredEqual(actual, 1);

			await That(result).IsEqualTo(isEqual);
			await That(resultWithComparer).IsEqualTo(isEqual);
		}

		[Test]
		[Arguments(nameof(StringEqualityOptions.IgnoringIndentation))]
		[Arguments(nameof(StringEqualityOptions.IgnoringLeadingWhiteSpace))]
		[Arguments(nameof(StringEqualityOptions.IgnoringNewlineStyle))]
		[Arguments(nameof(StringEqualityOptions.IgnoringTrailingWhiteSpace))]
		[Arguments(nameof(StringEqualityOptions.NormalizingWhiteSpace))]
		public async Task AsBlock_WhenALineOptionIsDisabled_ShouldNotThrow(string option)
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = Change(sut, option, false);

			void Act() => sut.AsBlock();

			await That(Act).DoesNotThrow()
				.Because("an option set to false is the same as not set");
		}

		[Test]
		[Arguments(nameof(StringEqualityOptions.IgnoringIndentation))]
		[Arguments(nameof(StringEqualityOptions.IgnoringLeadingWhiteSpace))]
		[Arguments(nameof(StringEqualityOptions.IgnoringNewlineStyle))]
		[Arguments(nameof(StringEqualityOptions.IgnoringTrailingWhiteSpace))]
		[Arguments(nameof(StringEqualityOptions.NormalizingWhiteSpace))]
		public async Task AsBlock_WhenALineOptionIsEnabled_ShouldThrowInvalidOperationException(string option)
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = Change(sut, option, true);

			void Act() => sut.AsBlock();

			await That(Act).Throws<InvalidOperationException>()
				.WithMessage($"{option} cannot be combined with AsBlock.")
				.Because("a block compares the lines on its own");
		}

		[Test]
		public async Task AsBlock_WhenAnotherMatchTypeIsSpecified_ShouldThrowInvalidOperationException()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.AsPrefix();

			void Act() => sut.AsBlock();

			await That(Act).Throws<InvalidOperationException>()
				.WithMessage("AsBlock cannot be combined with AsPrefix.");
		}

		[Test]
		public async Task AsRegex_WhenAComparerIsUsed_ShouldThrowInvalidOperationException()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.Using(StringComparer.Ordinal);

			void Act() => sut.AsRegex();

			await That(Act).Throws<InvalidOperationException>()
				.WithMessage("Using cannot be combined with AsRegex.");
		}

		[Test]
		public async Task AsRegex_WithOptions_WhenAComparerIsUsed_ShouldThrowInvalidOperationException()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.Using(StringComparer.Ordinal);

			void Act() => sut.AsRegex(RegexOptions.Multiline);

			await That(Act).Throws<InvalidOperationException>()
				.WithMessage("Using cannot be combined with AsRegex.");
		}

		[Test]
		public async Task AsWildcard_WhenAComparerIsUsed_ShouldThrowInvalidOperationException()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.Using(StringComparer.Ordinal);

			void Act() => sut.AsWildcard();

			await That(Act).Throws<InvalidOperationException>()
				.WithMessage("Using cannot be combined with AsWildcard.");
		}

		[Test]
		public async Task ComparesByOrdinalEquality_ByDefault_ShouldBeTrue()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;

			await That(sut.ComparesByOrdinalEquality).IsTrue();
		}

		[Test]
		[Arguments(nameof(StringEqualityOptions.AsBlock))]
		[Arguments(nameof(StringEqualityOptions.AsPrefix))]
		[Arguments(nameof(StringEqualityOptions.AsRegex))]
		[Arguments(nameof(StringEqualityOptions.AsRegex) + "WithOptions")]
		[Arguments(nameof(StringEqualityOptions.AsSuffix))]
		[Arguments(nameof(StringEqualityOptions.AsWildcard))]
		[Arguments(nameof(StringEqualityOptions.Containing))]
		[Arguments(nameof(StringEqualityOptions.IgnoringCase))]
		[Arguments(nameof(StringEqualityOptions.IgnoringIndentation))]
		[Arguments(nameof(StringEqualityOptions.IgnoringLeadingWhiteSpace))]
		[Arguments(nameof(StringEqualityOptions.IgnoringNewlineStyle))]
		[Arguments(nameof(StringEqualityOptions.IgnoringTrailingWhiteSpace))]
		[Arguments(nameof(StringEqualityOptions.NormalizingWhiteSpace))]
		[Arguments(nameof(StringEqualityOptions.Using))]
		public async Task ComparesByOrdinalEquality_WhenTheComparisonIsChanged_ShouldBeFalse(string option)
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = Change(sut, option, true);

			await That(sut.ComparesByOrdinalEquality).IsFalse();
		}

		[Test]
		[Arguments(nameof(StringEqualityOptions.IgnoringCase))]
		[Arguments(nameof(StringEqualityOptions.IgnoringIndentation))]
		[Arguments(nameof(StringEqualityOptions.IgnoringLeadingWhiteSpace))]
		[Arguments(nameof(StringEqualityOptions.IgnoringNewlineStyle))]
		[Arguments(nameof(StringEqualityOptions.IgnoringTrailingWhiteSpace))]
		[Arguments(nameof(StringEqualityOptions.NormalizingWhiteSpace))]
		public async Task ComparesByOrdinalEquality_WhenTheOptionIsDisabled_ShouldBeTrue(string option)
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = Change(sut, option, false);

			await That(sut.ComparesByOrdinalEquality).IsTrue();
		}

		[Test]
		public async Task CountOccurrences_WhenAComparerIsUsed_ShouldCompareEveryWindowWithIt()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.Using(StringComparer.OrdinalIgnoreCase);

			int result = await sut.CountOccurrences("xabAB", "ab", "expected");

			await That(result).IsEqualTo(2);
		}

		[Test]
		public async Task CountOccurrences_WhenACustomMatchTypeCannotCompareAWindow_ShouldThrowUserCodeException()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.SetMatchType(new DelegatingMatchType(() => new ValueTask<MatchResult>(
				MatchResult.NotComparable("is not comparable"))), "AsCustom");

			async Task Act() => await sut.CountOccurrences("foo", "f", "expected");

			await That(Act).Throws<UserCodeException>()
				.WithMessage("The code of the caller threw an exception while the expectation was evaluated.").And
				.WithInner<NotComparableException>(inner => inner
					.HasMessage("is not comparable").And
					.Whose(e => e.InnerException, cause => cause.IsNull()))
				.Because("every comparison by the match type fails both ways when it cannot compare its subject");
		}

		[Test]
		public async Task CountOccurrences_WhenACustomMatchTypeRejectsTheExpectedValue_ShouldThrowAtTheCall()
		{
			ArgumentException exception = new("my rejection");
			ValidatingMatchType matchType = new(rejectExpected: _ => exception);
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.IgnoringNewlineStyle();
			sut = sut.SetMatchType(matchType, "AsCustom");

			void Act() => sut.CountOccurrences("some text", "b\r\nar", "expected").AsTask();

			ArgumentException thrownException = await That(Act).Throws<ArgumentException>()
				.Because("an unusable expected value must not only throw when the returned task is awaited");
			await That(thrownException).IsSameAs(exception);
			await That(matchType.ValidatedExpected).IsEqualTo(["b\nar",]);
			await That(matchType.ComparedExpected).IsEmpty();
		}

		[Test]
		[Arguments("aaaa", "aa", false, 2)]
		[Arguments("aaa", "aa", false, 1)]
		[Arguments("abcABCabc", "abc", false, 2)]
		[Arguments("abcABCabc", "abc", true, 3)]
		[Arguments("x\U00010400y\U00010428", "\U00010428", false, 1)]
		[Arguments("ab", "abc", false, 0)]
		public async Task CountOccurrences_WhenComparedOrdinally_ShouldCountTheNonOverlappingOccurrences(
			string actual, string expected, bool ignoreCase, int expectedCount)
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.IgnoringCase(ignoreCase);

			int result = await sut.CountOccurrences(actual, expected, "expected");

			await That(result).IsEqualTo(expectedCount)
				.Because("the occurrences are searched instead of compared with a window at every position, which must count alike");
		}

		[Test]
		public async Task CountOccurrences_WhenExpectedIsLongerThanActual_ShouldStillApplyTheOptions()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.IgnoringTrailingWhiteSpace();

			int result = await sut.CountOccurrences("ab", "ab ", "expected");

			await That(result).IsEqualTo(1);
		}

		[Test]
		public async Task CountOccurrences_WhenExpectedIsPaddedWithWhiteSpace_ShouldOnlyIgnoreItAtTheEndOfTheSubject()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.IgnoringTrailingWhiteSpace();

			int result = await sut.CountOccurrences("abab", "ab  ", "expected");

			await That(result).IsEqualTo(1)
				.Because("the first 'ab' is followed by another character instead of whitespace");
		}

		[Test]
		[Arguments(" ")]
		[Arguments("   ")]
		[Arguments("\t")]
		public async Task CountOccurrences_WhenExpectedNormalizesToEmpty_ShouldThrowArgumentException(string expected)
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.IgnoringIndentation();

			async Task Act() => await sut.CountOccurrences("some text", expected, "expected");

			await That(Act).Throws<ArgumentException>()
				.WithMessage("The 'expected' string cannot be empty.").AsPrefix().And
				.WithParamName("expected")
				.Because("an empty needle never occurs, so a negated expectation could never fail");
		}

		[Test]
		public async Task CountOccurrences_WhenIndentationIsIgnored_ShouldFindNestedMultiLineSnippet()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.IgnoringIndentation();

			int result = await sut.CountOccurrences("class C\n{\n    Foo();\n    Bar();\n}", "Foo();\nBar();", "expected");

			await That(result).IsEqualTo(1);
		}

		[Test]
		public async Task CountOccurrences_WhenMatchingAsRegex_ShouldNotLimitTheWindowToThePatternLength()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.AsRegex();

			int result = await sut.CountOccurrences("ab", "^.*$", "expected");

			await That(result).IsEqualTo(1);
		}

		[Test]
		public async Task CountOccurrences_WhenMatchingAsRegex_ShouldNotNormalizeTheIndividualWindows()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.AsRegex().IgnoringIndentation();

			int result = await sut.CountOccurrences("x  abz", "^ab$", "expected");

			await That(result).IsEqualTo(0)
				.Because("the indentation is removed once from the whole string, not a second time from each occurrence");
		}

		[Test]
		[Arguments("forget", 0)]
		[Arguments("get", 1)]
		[Arguments("for get get", 2)]
		public async Task CountOccurrences_WhenMatchingAsRegex_ShouldOnlyIgnoreTheWhiteSpaceOfThePatternAtTheStartOfTheSubject(
			string actual, int expectedCount)
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.AsRegex().IgnoringLeadingWhiteSpace();

			int result = await sut.CountOccurrences(actual, " g.t", "expected");

			await That(result).IsEqualTo(expectedCount)
				.Because("the space of the pattern is only optional where the pattern reaches the start of the subject");
		}

		[Test]
		public async Task CountOccurrences_WhenMatchingAsWildcard_ShouldNotNormalizeTheIndividualWindows()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.AsWildcard().IgnoringIndentation();

			int result = await sut.CountOccurrences("a b", "?b", "expected");

			await That(result).IsEqualTo(1);
		}

		[Test]
		[Arguments("forget", 0)]
		[Arguments("get", 1)]
		[Arguments("get got", 2)]
		public async Task CountOccurrences_WhenMatchingAsWildcard_ShouldOnlyIgnoreTheWhiteSpaceOfThePatternAtTheStartOfTheSubject(
			string actual, int expectedCount)
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.AsWildcard().IgnoringLeadingWhiteSpace();

			int result = await sut.CountOccurrences(actual, " g?t", "expected");

			await That(result).IsEqualTo(expectedCount)
				.Because("the space of the pattern is only optional where the pattern reaches the start of the subject");
		}

		[Test]
		public async Task CountOccurrences_WhenOccurrencesAreIndentedDifferently_ShouldCountAll()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.IgnoringIndentation();

			int result = await sut.CountOccurrences("  a\n  b\nx\na\nb", "a\nb", "expected");

			await That(result).IsEqualTo(2);
		}

		[Test]
		[Arguments("forget", " get", 0)]
		[Arguments("get get", " get", 2)]
		[Arguments("  get", "\tget", 1)]
		[Arguments("ab", " ab ", 1)]
		[Arguments("a b", " ", 1)]
		[Arguments(" ab ", " ", 0)]
		public async Task CountOccurrences_WhenWhiteSpaceIsIgnored_ShouldOnlyIgnoreTheWhiteSpaceOfTheExpectedValueAtTheEdgesOfTheSubject(
			string actual, string expected, int expectedCount)
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.IgnoringLeadingWhiteSpace().IgnoringTrailingWhiteSpace();

			int result = await sut.CountOccurrences(actual, expected, "expected");

			await That(result).IsEqualTo(expectedCount)
				.Because("the whitespace of the expected value is only optional where it reaches an edge of the subject, "
				         + "and whitespace alone never occurs at an edge");
		}

		[Test]
		[Arguments("a  b\na b", "a b", 2)]
		[Arguments("a\tb", "a  b", 1)]
		[Arguments("x ab", " ab ", 1)]
		[Arguments("ab", "a b", 0)]
		[Arguments("a \n b", " ", 1)]
		[Arguments(" ab ", "\t", 0)]
		public async Task CountOccurrences_WhenWhiteSpaceIsNormalized_ShouldCountTheOccurrencesInTheCollapsedValue(
			string actual, string expected, int expectedCount)
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.NormalizingWhiteSpace();

			int result = await sut.CountOccurrences(actual, expected, "expected");

			await That(result).IsEqualTo(expectedCount)
				.Because("both values are collapsed once as a whole, and whitespace alone never occurs at an edge");
		}

		[Test]
		[Arguments(null, "abababab", "ab")]
		[Arguments("IgnoringCase", "abABabAB", "ab")]
		[Arguments("Using", "abababab", "ab")]
		[Arguments("AsRegex", "axayazaw", "a.")]
		[Arguments("AsWildcard", "axayazaw", "a?")]
		[Arguments("AsBlock", "a\na\na\na", "a")]
		public async Task CountOccurrences_WithMaximum_ShouldStopAtTheMaximum(string? option, string actual,
			string expected)
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			if (option is not null)
			{
				sut = Change(sut, option, true);
			}

			int result = await sut.CountOccurrences(actual, expected, 2, "expected");
			int total = await sut.CountOccurrences(actual, expected, "expected");

			await That(result).IsEqualTo(2)
				.Because("the search stops once the maximum number of occurrences is found");
			await That(total).IsEqualTo(4);
		}

		[Test]
		[Arguments(0)]
		[Arguments(-1)]
		public async Task CountOccurrences_WithMaximum_WhenNotPositive_ShouldThrowArgumentOutOfRangeException(int maximum)
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;

			void Act() => sut.CountOccurrences("abab", "ab", maximum, "expected").AsTask();

			await That(Act).Throws<ArgumentOutOfRangeException>()
				.WithParamName("maximum").And
				.WithMessage("The maximum must be greater than zero.").AsPrefix();
		}

		[Test]
		[Arguments(null, "abababab", "ab")]
		[Arguments("Using", "abababab", "ab")]
		[Arguments("AsRegex", "axayazaw", "a.")]
		[Arguments("AsBlock", "a\na\na\na", "a")]
		public async Task CountOccurrences_WithMaximum_WhenThereAreFewerOccurrences_ShouldCountAll(string? option,
			string actual, string expected)
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			if (option is not null)
			{
				sut = Change(sut, option, true);
			}

			int result = await sut.CountOccurrences(actual, expected, 10, "expected");

			await That(result).IsEqualTo(4);
		}

		[Test]
		[Arguments("AsBlock", ExpectationGrammars.Active, "matches \"foo\" as block")]
		[Arguments("AsBlock", ExpectationGrammars.Active | ExpectationGrammars.Plural, "match \"foo\" as block")]
		[Arguments("AsBlock", ExpectationGrammars.Active | ExpectationGrammars.Negated,
			"does not match \"foo\" as block")]
		[Arguments("AsBlock", ExpectationGrammars.Active | ExpectationGrammars.Plural | ExpectationGrammars.Negated,
			"do not match \"foo\" as block")]
		[Arguments("AsPrefix", ExpectationGrammars.Active, "starts with \"foo\"")]
		[Arguments("AsPrefix", ExpectationGrammars.Active | ExpectationGrammars.Plural, "start with \"foo\"")]
		[Arguments("AsPrefix", ExpectationGrammars.Active | ExpectationGrammars.Negated, "does not start with \"foo\"")]
		[Arguments("AsPrefix", ExpectationGrammars.Active | ExpectationGrammars.Plural | ExpectationGrammars.Negated,
			"do not start with \"foo\"")]
		[Arguments("AsRegex", ExpectationGrammars.Active, "matches regex \"foo\"")]
		[Arguments("AsRegex", ExpectationGrammars.Active | ExpectationGrammars.Plural, "match regex \"foo\"")]
		[Arguments("AsRegex", ExpectationGrammars.Active | ExpectationGrammars.Negated,
			"does not match regex \"foo\"")]
		[Arguments("AsRegex", ExpectationGrammars.Active | ExpectationGrammars.Plural | ExpectationGrammars.Negated,
			"do not match regex \"foo\"")]
		[Arguments("AsSuffix", ExpectationGrammars.Active, "ends with \"foo\"")]
		[Arguments("AsSuffix", ExpectationGrammars.Active | ExpectationGrammars.Plural, "end with \"foo\"")]
		[Arguments("AsSuffix", ExpectationGrammars.Active | ExpectationGrammars.Negated, "does not end with \"foo\"")]
		[Arguments("AsSuffix", ExpectationGrammars.Active | ExpectationGrammars.Plural | ExpectationGrammars.Negated,
			"do not end with \"foo\"")]
		[Arguments("AsWildcard", ExpectationGrammars.Active, "matches \"foo\"")]
		[Arguments("AsWildcard", ExpectationGrammars.Active | ExpectationGrammars.Plural, "match \"foo\"")]
		[Arguments("AsWildcard", ExpectationGrammars.Active | ExpectationGrammars.Negated, "does not match \"foo\"")]
		[Arguments("AsWildcard", ExpectationGrammars.Active | ExpectationGrammars.Plural | ExpectationGrammars.Negated,
			"do not match \"foo\"")]
		[Arguments("Containing", ExpectationGrammars.Active, "contains \"foo\"")]
		[Arguments("Containing", ExpectationGrammars.Active | ExpectationGrammars.Plural, "contain \"foo\"")]
		[Arguments("Containing", ExpectationGrammars.Active | ExpectationGrammars.Negated, "does not contain \"foo\"")]
		[Arguments("Containing", ExpectationGrammars.Active | ExpectationGrammars.Plural | ExpectationGrammars.Negated,
			"do not contain \"foo\"")]
		[Arguments("Exact", ExpectationGrammars.Active, "is equal to \"foo\"")]
		[Arguments("Exact", ExpectationGrammars.Active | ExpectationGrammars.Plural, "are equal to \"foo\"")]
		[Arguments("Exact", ExpectationGrammars.Active | ExpectationGrammars.Negated, "is not equal to \"foo\"")]
		[Arguments("Exact", ExpectationGrammars.Active | ExpectationGrammars.Plural | ExpectationGrammars.Negated,
			"are not equal to \"foo\"")]
		public async Task GetExpectation_ShouldAgreeWithTheNumberOfTheSubject(
			string matchType, ExpectationGrammars grammars, string expected)
		{
			StringEqualityOptions sut = WithMatchType(matchType);

			string result = sut.GetExpectation("foo", grammars);

			await That(result).IsEqualTo(expected);
		}

		[Test]
		[Arguments("AsBlock", "matching \"foo\" as block")]
		[Arguments("AsPrefix", "starting with \"foo\"")]
		[Arguments("AsRegex", "matching regex \"foo\"")]
		[Arguments("AsSuffix", "ending with \"foo\"")]
		[Arguments("AsWildcard", "matching \"foo\"")]
		[Arguments("Containing", "containing \"foo\"")]
		[Arguments("Exact", "equal to \"foo\"")]
		public async Task GetExpectation_WhenPassive_ShouldIgnoreTheNumberOfTheSubject(
			string matchType, string expected)
		{
			StringEqualityOptions sut = WithMatchType(matchType);

			string result = sut.GetExpectation("foo", ExpectationGrammars.Plural);

			await That(result).IsEqualTo(expected)
				.Because("a participle has no number that it could agree with");
		}

		[Test]
		public async Task GetExtendedFailure_Null_ShouldReturnItWasNull()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.IgnoringLeadingWhiteSpace();
			sut = sut.IgnoringTrailingWhiteSpace();
			sut = sut.IgnoringNewlineStyle();

			string result = sut.GetExtendedFailure("it", ExpectationGrammars.None, null, "foo", MatchResult.NoMatch);

			await That(result).IsEqualTo("it was <null>");
		}

		[Test]
		public async Task GetExtendedFailure_WhenExpectedIsNull_ShouldOnlyTrimTheActualValue()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.IgnoringLeadingWhiteSpace();
			sut = sut.IgnoringTrailingWhiteSpace();

			string result = sut.GetExtendedFailure("it", ExpectationGrammars.None, " foo ", null, MatchResult.NoMatch);

			await That(result).IsEqualTo("it was \"foo\"");
		}

		[Test]
		public async Task GetExtendedFailure_WhenACustomMatchTypeThrowsWithoutComparer_ShouldNotCatchTheException()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.SetMatchType(new CustomMatchType(), "AsCustom");

			void Act()
				=> sut.GetExtendedFailure("it", ExpectationGrammars.None, "foo", "bar", MatchResult.NoMatch);

			await That(Act).Throws<NotSupportedException>()
				.Because("without a comparer no code of the caller is called while the failure is explained");
		}

		[Test]
		public async Task GetExtendedFailure_WhenAPluralSubjectIsIt_ShouldKeepTheSingularVerb()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;

			string result = sut.GetExtendedFailure("it", ExpectationGrammars.Plural, "foo", "bar", MatchResult.NoMatch);

			await That(result).StartsWith("it was \"foo\"")
				.Because("the generic pronoun always takes the singular verb");
		}

		[Test]
		[Arguments("AsBlock")]
		[Arguments("AsCustom")]
		[Arguments("AsPrefix")]
		[Arguments("AsSuffix")]
		[Arguments("Containing")]
		[Arguments("Exact")]
		public async Task GetExtendedFailure_WhenAPluralSubjectIsNamed_ShouldUseThePluralVerb(string matchType)
		{
			StringEqualityOptions sut = matchType == "AsCustom"
				? StringEqualityOptions.Default.SetMatchType(new DefaultFailureMatchType(), "AsCustom")
				: WithMatchType(matchType);

			string result = sut.GetExtendedFailure("the titles", ExpectationGrammars.Plural, "foo", "bar",
				MatchResult.NoMatch);
			string negatedResult = sut.GetExtendedFailure("the titles",
				ExpectationGrammars.Plural | ExpectationGrammars.Negated, "foo", "bar", MatchResult.NoMatch);

			await That(result).StartsWith("the titles were \"foo\"")
				.Because("a custom match type uses the same verb as the built-in ones");
			await That(negatedResult).IsEqualTo("the titles were \"foo\"");
		}

		[Test]
		[Arguments("Exact", "1.2.3", "1.2.4")]
		[Arguments("AsPrefix", "1.2.3.4", "1.2.4")]
		[Arguments("AsSuffix", "0.1.2.3", "1.2.4")]
		public async Task GetExtendedFailure_WhenTheComparerThrowsForAPartOfTheValues_ShouldOmitTheDifference(
			string matchType, string actual, string expected)
		{
			StringEqualityOptions sut = WithMatchType(matchType);
			sut = sut.Using(new VersionStringComparer());

			bool isEqual = await sut.AreConsideredEqual(actual, expected, "expected");
			string result = sut.GetExtendedFailure("it", ExpectationGrammars.None, actual, expected, MatchResult.NoMatch);

			await That(isEqual).IsFalse();
			await That(result).IsEqualTo($"it was \"{actual}\"")
				.Because("the comparer answered for the values and is only called for parts of them to locate the difference");
		}

		[Test]
		public async Task GetExtendedFailure_WhenTheComparerThrowsForTrimmedValues_ShouldOmitTheDifference()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.Using(new ThrowingForComparer("1.2.3"));

			string result = sut.GetExtendedFailure("it", ExpectationGrammars.None, " 1.2.3", "1.2.4", MatchResult.NoMatch);

			await That(result).IsEqualTo("it was \" 1.2.3\"");
		}

		[Test]
		public async Task GetExtendedFailure_WhenIndentationAndLeadingWhiteSpaceAreIgnored_ShouldReportOriginalPosition()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.IgnoringIndentation().IgnoringLeadingWhiteSpace();

			string result = sut.GetExtendedFailure("it", ExpectationGrammars.None, "\n  foo", "bar", MatchResult.NoMatch);

			await That(result).Contains("differs on line 2 and column 3:");
		}

		[Test]
		public async Task GetExtendedFailure_WhenIndentationIsIgnored_ShouldReportColumnOfOriginalLine()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.IgnoringIndentation();

			string result = sut.GetExtendedFailure("it", ExpectationGrammars.None, "foo\n    baz", "foo\nbar", MatchResult.NoMatch);

			await That(result).Contains("differs on line 2 and column 7:");
		}

		[Test]
		public async Task GetExtendedFailure_WhenIndentationIsIgnoredAsPrefix_ShouldReportOriginalIndex()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.AsPrefix().IgnoringIndentation();

			string result = sut.GetExtendedFailure("it", ExpectationGrammars.None, "  foobar", "baz", MatchResult.NoMatch);

			await That(result).Contains("differs at index 2:");
		}

		[Test]
		public async Task GetExtendedFailure_WhenIndentationIsIgnoredAsSuffix_ShouldReportOriginalIndex()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.AsSuffix().IgnoringIndentation();

			string result = sut.GetExtendedFailure("it", ExpectationGrammars.None, "  foobar", "baz", MatchResult.NoMatch);

			await That(result).Contains("differs at index 7:");
		}

		[Test]
		public async Task GetExtendedFailure_WhenIndentationIsIgnoredOnSingleLine_ShouldReportOriginalIndex()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.IgnoringIndentation();

			string result = sut.GetExtendedFailure("it", ExpectationGrammars.None, "  foo", "bar", MatchResult.NoMatch);

			await That(result).Contains("differs at index 2:");
		}

		[Test]
		public async Task GetExtendedFailure_WhenLeadingWhiteSpaceIsIgnored_ShouldNotShiftColumnsOnLaterLines()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.IgnoringLeadingWhiteSpace();

			string result = sut.GetExtendedFailure("it", ExpectationGrammars.None, "  a\nbcd", "a\nbXd", MatchResult.NoMatch);

			await That(result).Contains("differs on line 2 and column 2:");
		}

		[Test]
		[Arguments(null)]
		[Arguments(nameof(StringEqualityOptions.IgnoringIndentation))]
		[Arguments(nameof(StringEqualityOptions.IgnoringLeadingWhiteSpace))]
		public async Task GetExtendedFailure_WhenWhiteSpaceIsNormalized_ShouldReportThePositionInTheNormalizedValues(
			string? option)
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			if (option is not null)
			{
				sut = Change(sut, option, true);
			}

			sut = sut.NormalizingWhiteSpace();

			string result = sut.GetExtendedFailure("it", ExpectationGrammars.None, "\n  a  b\n  c", "a b  d", MatchResult.NoMatch);

			await That(result).IsEqualTo("""
			                             it was "a b c", which differs at index 4:
			                                    ↓ (actual)
			                               "a b c"
			                               "a b d"
			                                    ↑ (expected)
			                             """).IgnoringNewlineStyle()
				.Because("collapsed whitespace cannot be mapped back to the original lines and columns, so the shown "
				         + "values and the position both refer to the normalized values");
		}

		[Test]
		public async Task GetExtendedMemberFailure_Negated_ShouldStateTheValueOfTheMember()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;

			string result = sut.GetExtendedMemberFailure("it", "message", ExpectationGrammars.Negated, "foo", "foo", MatchResult.NoMatch);

			await That(result).IsEqualTo("it had message \"foo\"");
		}

		[Test]
		public async Task GetExtendedMemberFailure_Null_ShouldStateThatTheMemberWasNull()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;

			string result = sut.GetExtendedMemberFailure("it", "message", ExpectationGrammars.None, null, "foo", MatchResult.NoMatch);

			await That(result).IsEqualTo("it had message <null>");
		}

		[Test]
		public async Task GetExtendedMemberFailure_WhenACustomMatchTypeFails_ShouldKeepItsFailure()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.SetMatchType(new DelegatingMatchType(() => new ValueTask<MatchResult>(MatchResult.NoMatch), "my custom failure"), "AsCustom");

			string result = sut.GetExtendedMemberFailure("it", "message", ExpectationGrammars.None, "foo", "bar", MatchResult.NoMatch);

			await That(result).IsEqualTo("my custom failure")
				.Because("only the failures of the built-in match types can be rephrased");
		}

		[Test]
		public async Task GetExtendedMemberFailure_WhenThePluralSubjectIsNamed_ShouldKeepTheMemberSingular()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;

			string result = sut.GetExtendedMemberFailure("the requests", "header value", ExpectationGrammars.Plural,
				"foo", "bar", MatchResult.NoMatch);

			await That(result).StartsWith("the requests had header value \"foo\", which differs at index 0:")
				.Because("the member holds a single value, also for a plural subject");
		}

		[Test]
		[Arguments("AsBlock", "bar", """
		                             it had message "foo", which differs at index 0:
		                                ↓ (actual)
		                               "foo"
		                               "bar"
		                                ↑ (expected)
		                             """)]
		[Arguments("AsPrefix", "bar", """
		                              it had message "foo", which differs at index 0:
		                                 ↓ (actual)
		                                "foo"
		                                "bar"
		                                 ↑ (expected prefix)
		                              """)]
		[Arguments("AsRegex", "b.r", """
		                             it had message "foo", which did not match:
		                               ↓ (actual)
		                               "foo"
		                               "b.r"
		                               ↑ (regex pattern)
		                             """)]
		[Arguments("AsSuffix", "bar", """
		                              it had message "foo", which differs at index 2:
		                                   ↓ (actual)
		                                "foo"
		                                "bar"
		                                   ↑ (expected suffix)
		                              """)]
		[Arguments("AsWildcard", "b?r", """
		                                it had message "foo", which did not match:
		                                  ↓ (actual)
		                                  "foo"
		                                  "b?r"
		                                  ↑ (wildcard pattern)
		                                """)]
		[Arguments("Containing", "bar", """
		                                it had message "foo"
		                                """)]
		[Arguments("Exact", "bar", """
		                           it had message "foo", which differs at index 0:
		                              ↓ (actual)
		                             "foo"
		                             "bar"
		                              ↑ (expected)
		                           """)]
		public async Task GetExtendedMemberFailure_WithABuiltInMatchType_ShouldStateTheValueOfTheMember(
			string matchType, string expected, string expectedFailure)
		{
			StringEqualityOptions sut = WithMatchType(matchType);

			string result = sut.GetExtendedMemberFailure("it", "message", ExpectationGrammars.None, "foo", expected, MatchResult.NoMatch);

			await That(result).IsEqualTo(expectedFailure).IgnoringNewlineStyle();
		}

		[Test]
		public async Task IgnoringCase_WhenACustomMatchTypeDoesNotSupportIt_ShouldThrowAndKeepTheOptions()
		{
			ValidatingMatchType matchType = new(StringMatchSettingKinds.None);
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.SetMatchType(matchType, "AsCustom");

			void Act() => sut.IgnoringCase();

			await That(Act).Throws<InvalidOperationException>()
				.WithMessage("IgnoringCase cannot be combined with AsCustom.");
			await That(sut.ToString()).IsEqualTo(" as custom");
			await That(() => sut.IgnoringCase(false)).DoesNotThrow()
				.Because("the rejected call must not count as specified, and a casing that is not ignored is supported");
		}

		[Test]
		[Arguments(false)]
		[Arguments(true)]
		public async Task IgnoringCase_WhenACustomMatchTypeIsSpecified_ShouldApplyASupportedCasing(bool ignoreCase)
		{
			ValidatingMatchType matchType = new(StringMatchSettingKinds.IgnoreCase);
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.SetMatchType(matchType, "AsCustom");

			sut = sut.IgnoringCase(ignoreCase);

			await That(sut.ToString()).IsEqualTo(ignoreCase ? " as custom ignoring case" : " as custom");
		}

		[Test]
		public async Task IgnoringCase_WhenAComparerIsUsed_ShouldReportTheComparerBeforeTheMatchType()
		{
			ValidatingMatchType matchType = new(StringMatchSettingKinds.Comparer);
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.SetMatchType(matchType, "AsCustom");
			sut = sut.Using(StringComparer.Ordinal);

			void Act() => sut.IgnoringCase();

			await That(Act).Throws<InvalidOperationException>()
				.WithMessage(
					"IgnoringCase cannot be combined with a custom comparer; use a case-insensitive comparer instead.")
				.Because("the options reject a combination that they cannot honour on their own first");
		}

		[Test]
		public async Task IgnoringCase_WhenAComparerIsUsed_ShouldThrowInvalidOperationException()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.Using(StringComparer.Ordinal);

			void Act() => sut.IgnoringCase();

			await That(Act).Throws<InvalidOperationException>()
				.WithMessage(
					"IgnoringCase cannot be combined with a custom comparer; use a case-insensitive comparer instead.");
		}

		[Test]
		public async Task IgnoringCase_WithFalse_WhenAComparerIsUsed_ShouldNotThrow()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.Using(StringComparer.Ordinal);

			void Act() => sut.IgnoringCase(false);

			await That(Act).DoesNotThrow()
				.Because("resetting the flag keeps the comparer as the only relevant option");
		}

		[Test]
		[Arguments(nameof(StringEqualityOptions.AsBlock), false)]
		[Arguments(nameof(StringEqualityOptions.AsBlock), true)]
		[Arguments(nameof(StringEqualityOptions.AsPrefix), false)]
		[Arguments(nameof(StringEqualityOptions.AsPrefix), true)]
		[Arguments(nameof(StringEqualityOptions.AsRegex), false)]
		[Arguments(nameof(StringEqualityOptions.AsRegex), true)]
		[Arguments(nameof(StringEqualityOptions.AsRegex) + "WithOptions", false)]
		[Arguments(nameof(StringEqualityOptions.AsRegex) + "WithOptions", true)]
		[Arguments(nameof(StringEqualityOptions.AsSuffix), false)]
		[Arguments(nameof(StringEqualityOptions.AsSuffix), true)]
		[Arguments(nameof(StringEqualityOptions.AsWildcard), false)]
		[Arguments(nameof(StringEqualityOptions.AsWildcard), true)]
		[Arguments(nameof(StringEqualityOptions.Containing), false)]
		[Arguments(nameof(StringEqualityOptions.Containing), true)]
		public async Task IgnoringCase_WithABuiltInMatchType_ShouldBeAcceptedInEitherOrder(
			string matchType, bool matchTypeFirst)
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;

			void Act()
			{
				if (matchTypeFirst)
				{
					sut = Change(sut, matchType, true);
					sut = sut.IgnoringCase();
				}
				else
				{
					sut = sut.IgnoringCase();
					sut = Change(sut, matchType, true);
				}
			}

			await That(Act).DoesNotThrow();
			await That(sut.ToString()).EndsWith(" ignoring case");
		}

		[Test]
		[Arguments(nameof(StringEqualityOptions.IgnoringIndentation))]
		[Arguments(nameof(StringEqualityOptions.IgnoringLeadingWhiteSpace))]
		[Arguments(nameof(StringEqualityOptions.IgnoringNewlineStyle))]
		[Arguments(nameof(StringEqualityOptions.IgnoringTrailingWhiteSpace))]
		[Arguments(nameof(StringEqualityOptions.NormalizingWhiteSpace))]
		public async Task IgnoringOption_WhenDisabledAndMatchingAsBlock_ShouldNotThrow(string option)
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.AsBlock();

			void Act() => Change(sut, option, false);

			await That(Act).DoesNotThrow()
				.Because("an option set to false is the same as not set");
		}

		[Test]
		[Arguments(nameof(StringEqualityOptions.IgnoringIndentation))]
		[Arguments(nameof(StringEqualityOptions.IgnoringLeadingWhiteSpace))]
		[Arguments(nameof(StringEqualityOptions.IgnoringNewlineStyle))]
		[Arguments(nameof(StringEqualityOptions.IgnoringTrailingWhiteSpace))]
		[Arguments(nameof(StringEqualityOptions.NormalizingWhiteSpace))]
		public async Task IgnoringOption_WhenMatchingAsBlock_ShouldThrowInvalidOperationException(string option)
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.AsBlock();

			void Act() => Change(sut, option, true);

			await That(Act).Throws<InvalidOperationException>()
				.WithMessage($"{option} cannot be combined with AsBlock.")
				.Because("a block compares the lines on its own");
		}

		[Test]
		[Arguments(nameof(StringEqualityOptions.IgnoringCase), true)]
		[Arguments(nameof(StringEqualityOptions.IgnoringCase), false)]
		[Arguments(nameof(StringEqualityOptions.IgnoringIndentation), true)]
		[Arguments(nameof(StringEqualityOptions.IgnoringIndentation), false)]
		[Arguments(nameof(StringEqualityOptions.IgnoringLeadingWhiteSpace), true)]
		[Arguments(nameof(StringEqualityOptions.IgnoringLeadingWhiteSpace), false)]
		[Arguments(nameof(StringEqualityOptions.IgnoringNewlineStyle), true)]
		[Arguments(nameof(StringEqualityOptions.IgnoringNewlineStyle), false)]
		[Arguments(nameof(StringEqualityOptions.IgnoringTrailingWhiteSpace), true)]
		[Arguments(nameof(StringEqualityOptions.IgnoringTrailingWhiteSpace), false)]
		[Arguments(nameof(StringEqualityOptions.NormalizingWhiteSpace), true)]
		[Arguments(nameof(StringEqualityOptions.NormalizingWhiteSpace), false)]
		public async Task IgnoringOption_WhenSpecifiedTwice_ShouldUseTheLastValue(string option, bool last)
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			StringEqualityOptions onlyLast = StringEqualityOptions.Default;
			onlyLast = Change(onlyLast, option, last);
			sut = Change(sut, option, !last);

			sut = Change(sut, option, last);

			await That(sut.ToString()).IsEqualTo(onlyLast.ToString());
		}

		[Test]
		public async Task SetMatchType_ShouldNotChangeTheOriginalOptions()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;

			StringEqualityOptions result = sut.SetMatchType(new CustomMatchType(), "AsCustom");

			await That(result).IsNotSameAs(sut);
			await That(sut.ComparesByOrdinalEquality).IsTrue();
		}

		[Test]
		[Arguments(nameof(StringEqualityOptions.AsPrefix))]
		[Arguments(nameof(StringEqualityOptions.AsRegex))]
		[Arguments(nameof(StringEqualityOptions.AsSuffix))]
		[Arguments(nameof(StringEqualityOptions.AsWildcard))]
		[Arguments(nameof(StringEqualityOptions.Containing))]
		public async Task SetMatchType_WhenAMatchTypeIsSpecified_ShouldThrowInvalidOperationException(string option)
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = Change(sut, option, true);

			void Act() => sut.SetMatchType(new CustomMatchType(), "AsCustom");

			await That(Act).Throws<InvalidOperationException>()
				.WithMessage($"AsCustom cannot be combined with {option}.");
		}

		[Test]
		[Arguments(nameof(StringEqualityOptions.AsPrefix))]
		[Arguments(nameof(StringEqualityOptions.AsRegex))]
		[Arguments(nameof(StringEqualityOptions.AsSuffix))]
		[Arguments(nameof(StringEqualityOptions.AsWildcard))]
		[Arguments(nameof(StringEqualityOptions.Containing))]
		public async Task SetMatchType_WhenAMatchTypeIsSpecified_ShouldNotCheckTheSettings(string option)
		{
			ValidatingMatchType matchType = new(StringMatchSettingKinds.None);
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = Change(sut, option, true);
			sut = sut.IgnoringCase();

			void Act() => sut.SetMatchType(matchType, "AsCustom");

			await That(Act).Throws<InvalidOperationException>()
				.WithMessage($"AsCustom cannot be combined with {option}.")
				.Because("the second match type is rejected before the settings that it does not support");
		}

		[Test]
		public async Task SetMatchType_WhenCaseIsIgnored_ShouldAcceptASupportedCasing()
		{
			ValidatingMatchType matchType = new(StringMatchSettingKinds.IgnoreCase);
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.IgnoringCase();

			sut = sut.SetMatchType(matchType, "AsCustom");

			await That(sut.ToString()).IsEqualTo(" as custom ignoring case");
		}

		[Test]
		public async Task SetMatchType_WhenAComparerIsUsed_ShouldAcceptASupportedComparer()
		{
			ValidatingMatchType matchType = new(StringMatchSettingKinds.Comparer);
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.Using(StringComparer.Ordinal);

			sut = sut.SetMatchType(matchType, "AsCustom");

			await That(sut.ToString()).IsEqualTo($" as custom using {Formatter.Format(StringComparer.Ordinal.GetType())}");
		}

		[Test]
		[Arguments(nameof(StringEqualityOptions.IgnoringCase))]
		[Arguments(nameof(StringEqualityOptions.IgnoringIndentation))]
		[Arguments(nameof(StringEqualityOptions.NormalizingWhiteSpace))]
		[Arguments(nameof(StringEqualityOptions.Using))]
		public async Task SetMatchType_WhenTheMatchTypeDoesNotSupportTheSpecifiedOption_ShouldThrowAndKeepTheOptions(
			string option)
		{
			ValidatingMatchType matchType = new(StringMatchSettingKinds.None);
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = Change(sut, option, true);
			string optionString = sut.ToString();

			void Act() => sut.SetMatchType(matchType, "AsCustom");

			await That(Act).Throws<InvalidOperationException>()
				.WithMessage($"{option} cannot be combined with AsCustom.");
			await That(sut.ToString()).IsEqualTo(optionString);
			await That(() => sut.SetMatchType(new ValidatingMatchType(), "AsOther")).DoesNotThrow()
				.Because("the rejected match type must not count as specified");
		}

		[Test]
		public async Task SetMatchType_WhenTheOptionsAreDisabled_ShouldNotThrow()
		{
			ValidatingMatchType matchType = new(StringMatchSettingKinds.None);
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.IgnoringCase(false).IgnoringIndentation(false).IgnoringNewlineStyle(false)
				.IgnoringLeadingWhiteSpace(false).IgnoringTrailingWhiteSpace(false).NormalizingWhiteSpace(false);

			void Act() => sut.SetMatchType(matchType, "AsCustom");

			await That(Act).DoesNotThrow()
				.Because("an option set to false is the same as not set");
		}

		[Test]
		public async Task SetMatchType_WhenTheSameMatchTypeOptionIsRepeated_ShouldCheckTheSettingsAgain()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.SetMatchType(new ValidatingMatchType(StringMatchSettingKinds.WhiteSpace), "AsCustom");
			sut = sut.IgnoringIndentation();

			void Act() => sut.SetMatchType(new ValidatingMatchType(StringMatchSettingKinds.None), "AsCustom");

			await That(Act).Throws<InvalidOperationException>()
				.WithMessage("IgnoringIndentation cannot be combined with AsCustom.");
		}

		[Test]
		public async Task SetMatchType_WhenTheSameMatchTypeOptionIsRepeated_ShouldReplaceTheMatchType()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.SetMatchType(new ValidatingMatchType(), "AsCustom");

			sut = sut.SetMatchType(new CustomMatchType(), "AsCustom");

			await That(() => sut.IgnoringCase()).Throws<InvalidOperationException>()
				.WithMessage("IgnoringCase cannot be combined with AsCustom.")
				.Because("the later match type without casing replaced the earlier one with casing");
		}

		[Test]
		[Arguments(new[] { "IgnoringTrailingWhiteSpace", "IgnoringIndentation", },
			"IgnoringIndentation and IgnoringTrailingWhiteSpace cannot be combined with AsCustom.")]
		[Arguments(new[] { "NormalizingWhiteSpace", "IgnoringLeadingWhiteSpace", "IgnoringCase", },
			"IgnoringCase, IgnoringLeadingWhiteSpace and NormalizingWhiteSpace cannot be combined with AsCustom.")]
		[Arguments(new[] { "Using", "IgnoringNewlineStyle", },
			"IgnoringNewlineStyle and Using cannot be combined with AsCustom.")]
		[Arguments(new[] { "IgnoringIndentation", "IgnoringNewlineStyle", "IgnoringLeadingWhiteSpace", "IgnoringTrailingWhiteSpace", "NormalizingWhiteSpace", "IgnoringCase", },
			"IgnoringCase, IgnoringIndentation, IgnoringNewlineStyle, IgnoringLeadingWhiteSpace, IgnoringTrailingWhiteSpace and NormalizingWhiteSpace cannot be combined with AsCustom.")]
		public async Task SetMatchType_WhenSeveralSpecifiedOptionsAreNotSupported_ShouldNameAllOfThemInDeclarationOrder(
			string[] options, string expectedMessage)
		{
			ValidatingMatchType matchType = new(StringMatchSettingKinds.None);
			StringEqualityOptions sut = StringEqualityOptions.Default;
			foreach (string option in options)
			{
				sut = Change(sut, option, true);
			}

			void Act() => sut.SetMatchType(matchType, "AsCustom");

			await That(Act).Throws<InvalidOperationException>()
				.WithMessage(expectedMessage);
		}

		[Test]
		[Arguments(nameof(StringEqualityOptions.AsPrefix), nameof(StringEqualityOptions.AsSuffix))]
		[Arguments(nameof(StringEqualityOptions.AsSuffix), nameof(StringEqualityOptions.AsPrefix))]
		public async Task SetMatchType_WithTwoDifferentMatchTypes_ShouldThrowInEitherOrder(string first, string second)
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = Change(sut, first, true);

			void Act() => Change(sut, second, true);

			await That(Act).Throws<InvalidOperationException>()
				.WithMessage($"{second} cannot be combined with {first}.");
		}

		[Test]
		public async Task ToString_WhenCaseAndIndentationIsIgnored_ShouldIncludeOptions()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.IgnoringCase().IgnoringIndentation();

			string result = sut.ToString();

			await That(result).IsEqualTo(" ignoring case and indentation");
		}

		[Test]
		public async Task ToString_WhenCaseAndNewlineStyleIsIgnored_ShouldIncludeOptions()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.IgnoringCase().IgnoringNewlineStyle();

			string result = sut.ToString();

			await That(result).IsEqualTo(" ignoring case and newline style");
		}

		[Test]
		public async Task ToString_WhenCaseAndWhiteSpaceAndNewlineStyleAndIndentationIsIgnored_ShouldIncludeOptions()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.IgnoringCase().IgnoringLeadingWhiteSpace()
				.IgnoringTrailingWhiteSpace()
				.IgnoringNewlineStyle()
				.IgnoringIndentation();

			string result = sut.ToString();

			await That(result).IsEqualTo(" ignoring case, whitespace, newline style and indentation");
		}

		[Test]
		public async Task ToString_WhenCaseAndWhiteSpaceAndNewlineStyleIsIgnored_ShouldIncludeOptions()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.IgnoringCase().IgnoringLeadingWhiteSpace()
				.IgnoringTrailingWhiteSpace()
				.IgnoringNewlineStyle();

			string result = sut.ToString();

			await That(result).IsEqualTo(" ignoring case, whitespace and newline style");
		}

		[Test]
		public async Task ToString_WhenIndentationAndNewlineStyleIsIgnored_ShouldIncludeOptions()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.IgnoringNewlineStyle().IgnoringIndentation();

			string result = sut.ToString();

			await That(result).IsEqualTo(" ignoring newline style and indentation");
		}

		[Test]
		public async Task ToString_WhenIndentationIsIgnored_ShouldIncludeOptions()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.IgnoringIndentation();

			string result = sut.ToString();

			await That(result).IsEqualTo(" ignoring indentation");
		}

		[Test]
		public async Task ToString_WhenIndentationIsIgnoredWithComparer_ShouldIncludeOptions()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.Using(StringComparer.Ordinal).IgnoringIndentation();

			string result = sut.ToString();

			// The comparer type is named differently on .NET Framework.
			await That(result).StartsWith(" using ").And.EndsWith(" ignoring indentation");
		}

		[Test]
		public async Task ToString_WhenWhiteSpaceAndNewlineStyleIsIgnored_ShouldIncludeOptions()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.IgnoringLeadingWhiteSpace()
				.IgnoringTrailingWhiteSpace()
				.IgnoringNewlineStyle();

			string result = sut.ToString();

			await That(result).IsEqualTo(" ignoring whitespace and newline style");
		}

		[Test]
		[Arguments(null, " normalizing whitespace")]
		[Arguments("AsRegex", " as regex normalizing whitespace")]
		[Arguments("IgnoringCase", " ignoring case and normalizing whitespace")]
		[Arguments("IgnoringNewlineStyle", " ignoring newline style and normalizing whitespace")]
		public async Task ToString_WhenWhiteSpaceIsNormalized_ShouldIncludeOptions(string? option, string expected)
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			if (option is not null)
			{
				sut = Change(sut, option, true);
			}

			sut = sut.NormalizingWhiteSpace();

			string result = sut.ToString();

			await That(result).IsEqualTo(expected);
		}

		[Test]
		public async Task Using_WhenACustomMatchTypeDoesNotSupportIt_ShouldThrowAndKeepTheOptions()
		{
			ValidatingMatchType matchType = new(StringMatchSettingKinds.None);
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.SetMatchType(matchType, "AsCustom");

			void Act() => sut.Using(StringComparer.Ordinal);

			await That(Act).Throws<InvalidOperationException>()
				.WithMessage("Using cannot be combined with AsCustom.");
			await That(sut.ToString()).IsEqualTo(" as custom");
			await That(Act).Throws<InvalidOperationException>()
				.WithMessage("Using cannot be combined with AsCustom.")
				.Because("the rejected comparer must not count as specified");
		}

		[Test]
		public async Task Using_WhenACustomMatchTypeIsSpecified_ShouldApplyASupportedComparer()
		{
			ValidatingMatchType matchType = new(StringMatchSettingKinds.IgnoreCase | StringMatchSettingKinds.Comparer);
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.SetMatchType(matchType, "AsCustom");
			sut = sut.IgnoringCase(false);

			sut = sut.Using(StringComparer.Ordinal);

			await That(sut.ToString()).IsEqualTo($" as custom using {Formatter.Format(StringComparer.Ordinal.GetType())}");
		}

		[Test]
		public async Task Using_WhenCaseIsIgnored_ShouldReportTheCasingBeforeTheMatchType()
		{
			ValidatingMatchType matchType = new(StringMatchSettingKinds.IgnoreCase);
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.SetMatchType(matchType, "AsCustom");
			sut = sut.IgnoringCase();

			void Act() => sut.Using(StringComparer.Ordinal);

			await That(Act).Throws<InvalidOperationException>()
				.WithMessage(
					"IgnoringCase cannot be combined with a custom comparer; use a case-insensitive comparer instead.")
				.Because("the options reject a combination that they cannot honour on their own first");
		}

		[Test]
		public async Task Using_WhenAComparerIsUsed_ShouldUseTheLastComparer()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.Using(StringComparer.Ordinal);

			sut = sut.Using(StringComparer.OrdinalIgnoreCase);

			await That(await sut.AreConsideredEqual("foo", "FOO", "expected")).IsTrue();
		}

		[Test]
		public async Task Using_WhenCaseIsExplicitlyNotIgnored_ShouldNotThrow()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.IgnoringCase(false);

			void Act() => sut.Using(StringComparer.Ordinal);

			await That(Act).DoesNotThrow()
				.Because("the reset flag does not compete with the comparer");
		}

		[Test]
		public async Task Using_WhenCaseIsIgnoredAndReset_ShouldNotThrow()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.IgnoringCase().IgnoringCase(false);

			void Act() => sut.Using(StringComparer.Ordinal);

			await That(Act).DoesNotThrow()
				.Because("the later IgnoringCase(false) replaces the earlier one, so the casing is not ignored");
		}

		[Test]
		public async Task Using_WhenCaseIsIgnored_ShouldThrowInvalidOperationException()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.IgnoringCase();

			void Act() => sut.Using(StringComparer.Ordinal);

			await That(Act).Throws<InvalidOperationException>()
				.WithMessage(
					"IgnoringCase cannot be combined with a custom comparer; use a case-insensitive comparer instead.");
		}

		[Test]
		public async Task Using_WhenMatchingAsRegex_ShouldThrowInvalidOperationException()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.AsRegex();

			void Act() => sut.Using(StringComparer.Ordinal);

			await That(Act).Throws<InvalidOperationException>()
				.WithMessage("Using cannot be combined with AsRegex.");
		}

		[Test]
		public async Task Using_WhenMatchingAsWildcard_ShouldThrowInvalidOperationException()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.AsWildcard();

			void Act() => sut.Using(StringComparer.Ordinal);

			await That(Act).Throws<InvalidOperationException>()
				.WithMessage("Using cannot be combined with AsWildcard.");
		}

		[Test]
		public async Task Using_WithNull_ShouldThrowArgumentNullException()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;

			void Act() => sut.Using(null!);

			await That(Act).Throws<ArgumentNullException>()
				.WithParamName("comparer").And
				.WithMessage("The 'comparer' cannot be null.").AsPrefix();
		}

		[Test]
		public async Task Using_WithNull_WhenAComparerIsUsed_ShouldThrowArgumentNullException()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.Using(StringComparer.Ordinal);

			void Act() => sut.Using(null!);

			await That(Act).Throws<ArgumentNullException>()
				.WithParamName("comparer").And
				.WithMessage("The 'comparer' cannot be null.").AsPrefix()
				.Because("a comparer cannot be reset to the default one");
		}

		[Test]
		public async Task ValidateExpected_ThroughTheEqualityOptions_ShouldRejectAnUnusableExpectedValue()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default.AsPrefix();

			void Act() => ValidateThroughTheEqualityOptions(sut, "", "expected");

			await That(Act).Throws<ArgumentException>()
				.WithParamName("expected").And
				.WithMessage("The 'expected' prefix cannot be empty.").AsPrefix()
				.Because("a collection expectation validates its expected items through the equality options");
		}

		[Test]
		public async Task ValidateExpected_WhenACustomMatchTypeRejectsTheExpectedValue_ShouldThrowItsException()
		{
			ArgumentException exception = new("my rejection");
			ValidatingMatchType matchType = new(rejectExpected: expected => expected == "foo" ? exception : null);
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.SetMatchType(matchType, "AsCustom");

			void Act() => sut.ValidateExpected("foo", "expected");

			ArgumentException thrownException = await That(Act).Throws<ArgumentException>();
			await That(thrownException).IsSameAs(exception);
			await That(() => sut.ValidateExpected("bar", "expected")).DoesNotThrow();
		}

		[Test]
		[Arguments("AsPrefix", null)]
		[Arguments("AsPrefix", "")]
		[Arguments("AsPrefix", "  ")]
		[Arguments("AsSuffix", null)]
		[Arguments("AsSuffix", "")]
		[Arguments("AsSuffix", "  ")]
		[Arguments("AsRegex", null)]
		[Arguments("AsRegex", "")]
		[Arguments("AsRegex", "[")]
		[Arguments("AsWildcard", null)]
		public async Task ValidateExpected_WhenExpectedIsUnusable_ShouldThrowTheSameExceptionAsTheComparison(
			string matchType, string? expected)
		{
			StringEqualityOptions sut = StringEqualityOptions.Default
				.IgnoringLeadingWhiteSpace().IgnoringTrailingWhiteSpace();
			sut = SetMatchType(sut, matchType);
			ArgumentException? comparisonException = null;
			try
			{
				await sut.AreConsideredEqual("foo", expected, "unexpected");
			}
			catch (ArgumentException exception)
			{
				comparisonException = exception;
			}

			void Act() => sut.ValidateExpected(expected, "unexpected");

			ArgumentException validationException = await That(Act).Throws<ArgumentException>()
				.WithParamName("unexpected");
			await That(validationException.GetType()).IsEqualTo(comparisonException?.GetType());
			await That(validationException.Message).IsEqualTo(comparisonException?.Message);
		}

		[Test]
		[Arguments("", null)]
		[Arguments("", "")]
		[Arguments("AsBlock", null)]
		[Arguments("AsBlock", "")]
		[Arguments("Containing", null)]
		[Arguments("AsPrefix", "f")]
		[Arguments("AsSuffix", "f")]
		[Arguments("AsRegex", "f.*")]
		[Arguments("AsWildcard", "")]
		[Arguments("AsWildcard", "f*")]
		public async Task ValidateExpected_WhenExpectedIsUsable_ShouldNotThrow(string matchType, string? expected)
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = SetMatchType(sut, matchType);

			void Act() => sut.ValidateExpected(expected, "expected");

			await That(Act).DoesNotThrow();
		}

		[Test]
		[Arguments("foo", "foo")]
		[Arguments("  foo\t", "foo")]
		[Arguments("f\r\noo", "f\noo")]
		[Arguments("", "")]
		[Arguments(null, null)]
		public async Task ValidateExpected_WithACustomMatchType_ShouldPassTheValueThatIsCompared(
			string? expected, string? normalizedExpected)
		{
			ValidatingMatchType matchType = new();
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.IgnoringLeadingWhiteSpace().IgnoringTrailingWhiteSpace().IgnoringNewlineStyle();
			sut = sut.SetMatchType(matchType, "AsCustom");

			sut.ValidateExpected(expected, "expected");
			await ((IEqualityEvaluator<string?>)sut).AreConsideredEqual("foo", expected);

			await That(matchType.ValidatedExpected).IsEqualTo([normalizedExpected, normalizedExpected,]);
			await That(matchType.ComparedExpected).IsEqualTo([normalizedExpected,]);
		}

		private static StringEqualityOptions SetMatchType(StringEqualityOptions options, string matchType)
		{
			switch (matchType)
			{
				case "AsBlock":
					return options.AsBlock();
				case "AsPrefix":
					return options.AsPrefix();
				case "AsRegex":
					return options.AsRegex();
				case "AsSuffix":
					return options.AsSuffix();
				case "AsWildcard":
					return options.AsWildcard();
				case "Containing":
					return options.Containing();
				default:
					return options;
			}
		}

		private static StringEqualityOptions Change(StringEqualityOptions options, string option, bool enable)
		{
			switch (option)
			{
				case "AsBlock":
					return options.AsBlock();
				case "AsPrefix":
					return options.AsPrefix();
				case "AsRegex":
					return options.AsRegex();
				case "AsRegexWithOptions":
					return options.AsRegex(RegexOptions.Multiline);
				case "AsSuffix":
					return options.AsSuffix();
				case "AsWildcard":
					return options.AsWildcard();
				case "Containing":
					return options.Containing();
				case "IgnoringCase":
					return options.IgnoringCase(enable);
				case "IgnoringIndentation":
					return options.IgnoringIndentation(enable);
				case "IgnoringLeadingWhiteSpace":
					return options.IgnoringLeadingWhiteSpace(enable);
				case "IgnoringNewlineStyle":
					return options.IgnoringNewlineStyle(enable);
				case "IgnoringTrailingWhiteSpace":
					return options.IgnoringTrailingWhiteSpace(enable);
				case "NormalizingWhiteSpace":
					return options.NormalizingWhiteSpace(enable);
				case "Using":
					return options.Using(StringComparer.Ordinal);
				default:
					throw new ArgumentOutOfRangeException(nameof(option), option, null);
			}
		}

		/// <remarks>
		///     The options are a type parameter constrained to the interface, so that the call goes through
		///     <see cref="IEqualityOptions{TSubject}.ValidateExpected(TSubject, string)" /> as in a collection expectation,
		///     while a cast to the interface would be redundant for the sealed options.
		/// </remarks>
		private static void ValidateThroughTheEqualityOptions<TOptions>(TOptions options, string? expected,
			string parameterName)
			where TOptions : IEqualityOptions<string?>
			=> options.ValidateExpected(expected, parameterName);

		/// <remarks>
		///     The exact match type is the default, so it is selected by naming no method at all.
		/// </remarks>
		private static StringEqualityOptions WithMatchType(string matchType)
		{
			StringEqualityOptions options = StringEqualityOptions.Default;
			switch (matchType)
			{
				case "AsBlock":
					return options.AsBlock();
				case "AsPrefix":
					return options.AsPrefix();
				case "AsRegex":
					return options.AsRegex();
				case "AsSuffix":
					return options.AsSuffix();
				case "AsWildcard":
					return options.AsWildcard();
				case "Containing":
					return options.Containing();
			}

			return options;
		}

		private sealed class CustomMatchType : StringMatchType
		{
			public override StringMatcher ForEvaluation(MatchEnvironment environment, StringMatchSettings settings)
				=> new Matcher();

			public override MatchPhrase Phrase => MatchPhrase.EqualTo;

			private sealed class Matcher : StringMatcher
			{
				public override ValueTask<MatchResult> Match(string? actual, string? expected)
					=> throw new NotSupportedException();

				public override string GetFailure(string it, ExpectationGrammars grammars, string? actual,
					string? expected, MatchResult result, StringDifferenceSettings? differenceSettings)
					=> throw new NotSupportedException();

				public override void AppendContexts(ResultContextCollector contexts)
					=> throw new NotSupportedException();
			}
		}

		private sealed class DefaultFailureMatchType : StringMatchType
		{
			public override StringMatcher ForEvaluation(MatchEnvironment environment, StringMatchSettings settings)
				=> new Matcher();

			public override MatchPhrase Phrase => MatchPhrase.EqualTo;

			private sealed class Matcher : StringMatcher
			{
				public override ValueTask<MatchResult> Match(string? actual, string? expected)
					=> new(MatchResult.From(actual == expected));
			}
		}

		private sealed class DelegatingMatchType(Func<ValueTask<MatchResult>> match, string failure = "")
			: StringMatchType
		{
			public override StringMatcher ForEvaluation(MatchEnvironment environment, StringMatchSettings settings)
				=> new Matcher(match, failure);

			public override MatchPhrase Phrase => MatchPhrase.EqualTo;

			private sealed class Matcher(Func<ValueTask<MatchResult>> match, string failure) : StringMatcher
			{
				public override ValueTask<MatchResult> Match(string? actual, string? expected)
					=> match();

				public override string GetFailure(string it, ExpectationGrammars grammars, string? actual,
					string? expected, MatchResult result, StringDifferenceSettings? differenceSettings)
					=> failure;

				public override void AppendContexts(ResultContextCollector contexts)
					=> throw new NotSupportedException();
			}
		}

		/// <remarks>
		///     Supports the <paramref name="supportedSettings" />, records what it is asked to validate and to compare, and
		///     rejects what the delegate returns an exception for.
		/// </remarks>
		private sealed class ValidatingMatchType(
			StringMatchSettingKinds supportedSettings = StringMatchSettingKinds.IgnoreCase |
			                                            StringMatchSettingKinds.Comparer |
			                                            StringMatchSettingKinds.WhiteSpace,
			Func<string?, Exception?>? rejectExpected = null)
			: StringMatchType
		{
			public List<string?> ValidatedExpected { get; } = [];

			public List<string?> ComparedExpected { get; } = [];

			public override StringMatcher ForEvaluation(MatchEnvironment environment, StringMatchSettings settings)
				=> new Matcher(ComparedExpected);

			public override MatchPhrase Phrase { get; } = MatchPhrase.EqualTo.WithName(" as custom");

			public override StringMatchSettingKinds SupportedSettings => supportedSettings;

			public override void ValidateExpected(string? expected, string parameterName)
			{
				ValidatedExpected.Add(expected);
				if (rejectExpected?.Invoke(expected) is { } exception)
				{
					throw exception;
				}
			}

			private sealed class Matcher(List<string?> comparedExpected) : StringMatcher
			{
				public override ValueTask<MatchResult> Match(string? actual, string? expected)
				{
					comparedExpected.Add(expected);
					return new ValueTask<MatchResult>(MatchResult.Match);
				}

				public override string GetFailure(string it, ExpectationGrammars grammars, string? actual,
					string? expected, MatchResult result, StringDifferenceSettings? differenceSettings)
					=> throw new NotSupportedException();

				public override void AppendContexts(ResultContextCollector contexts)
					=> throw new NotSupportedException();
			}
		}

		private sealed class ThrowingComparer(Exception exception) : IEqualityComparer<string>
		{
			public bool Equals(string? x, string? y) => throw exception;

			public int GetHashCode(string obj) => throw exception;
		}

		private sealed class ThrowingForComparer(string value) : IEqualityComparer<string>
		{
			public bool Equals(string? x, string? y)
				=> x == value || y == value
					? throw new NotSupportedException($"cannot compare '{x}' and '{y}'")
					: x == y;

			public int GetHashCode(string obj) => obj.GetHashCode();
		}
	}
}
