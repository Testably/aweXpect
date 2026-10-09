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
			StringEqualityOptions sut = new("expected");
			sut.IgnoringLeadingWhiteSpace();
			sut.IgnoringTrailingWhiteSpace();
			sut.IgnoringNewlineStyle();

			bool result = await sut.AreConsideredEqual(null, "foo");

			await That(result).IsFalse();
		}

		[Test]
		public async Task AreConsideredEqual_WhenAComparerIsUsed_ShouldStillApplyTheWhiteSpaceOptions()
		{
			StringEqualityOptions sut = new("expected");
			sut.Using(StringComparer.Ordinal).IgnoringLeadingWhiteSpace();

			bool result = await sut.AreConsideredEqual("  foo", "foo");

			await That(result).IsTrue()
				.Because("both values are normalized before the comparer sees them, so neither option is dropped");
		}

		[Test]
		public async Task AreConsideredEqual_WhenACustomMatchTypeCompletesLater_ShouldAwaitIt()
		{
			StringEqualityOptions sut = new("expected");
			sut.SetMatchType(new DelegatingMatchType(async () =>
			{
				await Task.Yield();
				return true;
			}), "AsCustom");

			bool result = await sut.AreConsideredEqual("foo", "bar");

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
			StringEqualityOptions sut = new("expected");
			sut.IgnoringTrailingWhiteSpace();
			sut.SetMatchType(matchType, "AsCustom");

			void Act() => _ = sut.AreConsideredEqual("foo", expected);

			ArgumentException thrownException = await That(Act).Throws<ArgumentException>()
				.Because("an unusable expected value must not only throw when the returned task is awaited");
			await That(thrownException).IsSameAs(exception);
			await That(matchType.ValidatedExpected).IsEqualTo([validatedExpected,]);
			await That(matchType.ComparedExpected).IsEmpty();
		}

		[Test]
		[Arguments(false)]
		[Arguments(true)]
		public async Task AreConsideredEqual_WhenACustomMatchTypeTimesOut_ShouldThrowArgumentException(bool completesLater)
		{
			RegexMatchTimeoutException exception = new("foo", "bar", TimeSpan.FromSeconds(1));
			StringEqualityOptions sut = new("expected");
			sut.SetMatchType(new DelegatingMatchType(completesLater
				? async () =>
				{
					await Task.Yield();
					throw exception;
				}
				: () => throw exception), "AsCustom");

			async Task Act() => await sut.AreConsideredEqual("foo", "bar");

			await That(Act).Throws<ArgumentException>()
				.WithMessage(
					"""The wildcard pattern "bar" did not complete within 0:01. Simplify the pattern to avoid catastrophic backtracking.""")
				.AsPrefix().And
				.WithParamName("expected").And
				.WithInner<RegexMatchTimeoutException>(inner => inner.IsSameAs(exception))
				.Because("a timeout of a custom match type must be reported like one of a pattern");
		}

		[Test]
		public async Task AreConsideredEqual_WhenIndentationIsIgnored_ShouldEmptyWhiteSpaceOnlyLines()
		{
			StringEqualityOptions sut = new("expected");
			sut.IgnoringIndentation();

			bool result = await sut.AreConsideredEqual("foo\n    ", "foo\n");

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
			StringEqualityOptions sut = new("expected");
			sut.IgnoringIndentation();

			bool result = await sut.AreConsideredEqual(actual, expected);

			await That(result).IsTrue();
		}

		[Test]
		public async Task AreConsideredEqual_WhenIndentationIsIgnored_ShouldStillConsiderTrailingWhiteSpace()
		{
			StringEqualityOptions sut = new("expected");
			sut.IgnoringIndentation();

			bool result = await sut.AreConsideredEqual("foo  \nbar", "foo\nbar");

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
			sut.Using(new ThrowingComparer(exception));

			async Task Act()
				=> await sut.AreConsideredEqual("foo", "foo");

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

			bool matchingResult = await sut.AreConsideredEqual("foo", matchingPattern);
			bool otherResult = await sut.AreConsideredEqual("foo", otherPattern);
			bool caseSensitiveResult = await sut.AreConsideredEqual("FOO", matchingPattern);
			sut.IgnoringCase();
			bool ignoringCaseResult = await sut.AreConsideredEqual("FOO", matchingPattern);

			await That(matchingResult).IsTrue();
			await That(otherResult).IsFalse()
				.Because("the pattern that was parsed for the previous comparison must not be reused for another one");
			await That(caseSensitiveResult).IsFalse();
			await That(ignoringCaseResult).IsTrue()
				.Because("the casing is part of the parsed pattern");
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
			StringEqualityOptions sut = new("expected");
			StringEqualityOptions withComparer = new("expected");
			withComparer.Using(StringComparer.Ordinal);

			bool result = await sut.AreConsideredEqual(actual, expected);
			bool resultAsObject = await sut.AreConsideredEqual<object?>(actual, expected);
			bool resultWithComparer = await withComparer.AreConsideredEqual(actual, expected);

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
			StringEqualityOptions sut = new("expected");
			StringEqualityOptions withComparer = new("expected");
			withComparer.Using(StringComparer.Ordinal);

			bool result = await sut.AreConsideredEqual(actual, 1);
			bool resultWithComparer = await withComparer.AreConsideredEqual(actual, 1);

			await That(result).IsEqualTo(isEqual);
			await That(resultWithComparer).IsEqualTo(isEqual);
		}

		[Test]
		[Arguments(nameof(StringEqualityOptions.IgnoringIndentation))]
		[Arguments(nameof(StringEqualityOptions.IgnoringLeadingWhiteSpace))]
		[Arguments(nameof(StringEqualityOptions.IgnoringNewlineStyle))]
		[Arguments(nameof(StringEqualityOptions.IgnoringTrailingWhiteSpace))]
		public async Task AsBlock_WhenALineOptionIsSpecified_ShouldThrowInvalidOperationException(string option)
		{
			StringEqualityOptions sut = new("expected");
			Change(sut, option, false);

			void Act() => sut.AsBlock();

			await That(Act).Throws<InvalidOperationException>()
				.WithMessage($"AsBlock cannot be combined with {option}.")
				.Because("a block compares the lines on its own, and any explicit call counts as specified");
		}

		[Test]
		public async Task AsBlock_WhenAnotherMatchTypeIsSpecified_ShouldThrowInvalidOperationException()
		{
			StringEqualityOptions sut = new("expected");
			sut.AsPrefix();

			void Act() => sut.AsBlock();

			await That(Act).Throws<InvalidOperationException>()
				.WithMessage("AsBlock cannot be combined with AsPrefix.");
		}

		[Test]
		public async Task AsRegex_WhenAComparerIsUsed_ShouldThrowInvalidOperationException()
		{
			StringEqualityOptions sut = new("expected");
			sut.Using(StringComparer.Ordinal);

			void Act() => sut.AsRegex();

			await That(Act).Throws<InvalidOperationException>()
				.WithMessage("A custom comparer is not supported for regex or wildcard matching.");
		}

		[Test]
		public async Task AsRegex_WithOptions_WhenAComparerIsUsed_ShouldThrowInvalidOperationException()
		{
			StringEqualityOptions sut = new("expected");
			sut.Using(StringComparer.Ordinal);

			void Act() => sut.AsRegex(RegexOptions.Multiline);

			await That(Act).Throws<InvalidOperationException>()
				.WithMessage("A custom comparer is not supported for regex or wildcard matching.");
		}

		[Test]
		public async Task AsWildcard_WhenAComparerIsUsed_ShouldThrowInvalidOperationException()
		{
			StringEqualityOptions sut = new("expected");
			sut.Using(StringComparer.Ordinal);

			void Act() => sut.AsWildcard();

			await That(Act).Throws<InvalidOperationException>()
				.WithMessage("A custom comparer is not supported for regex or wildcard matching.");
		}

		[Test]
		public async Task ComparesByOrdinalEquality_ByDefault_ShouldBeTrue()
		{
			StringEqualityOptions sut = new("expected");

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
		[Arguments(nameof(StringEqualityOptions.Using))]
		public async Task ComparesByOrdinalEquality_WhenTheComparisonIsChanged_ShouldBeFalse(string option)
		{
			StringEqualityOptions sut = new("expected");
			Change(sut, option, true);

			await That(sut.ComparesByOrdinalEquality).IsFalse();
		}

		[Test]
		[Arguments(nameof(StringEqualityOptions.IgnoringCase))]
		[Arguments(nameof(StringEqualityOptions.IgnoringIndentation))]
		[Arguments(nameof(StringEqualityOptions.IgnoringLeadingWhiteSpace))]
		[Arguments(nameof(StringEqualityOptions.IgnoringNewlineStyle))]
		[Arguments(nameof(StringEqualityOptions.IgnoringTrailingWhiteSpace))]
		public async Task ComparesByOrdinalEquality_WhenTheOptionIsDisabled_ShouldBeTrue(string option)
		{
			StringEqualityOptions sut = new("expected");
			Change(sut, option, false);

			await That(sut.ComparesByOrdinalEquality).IsTrue();
		}

		[Test]
		public async Task CountOccurrences_WhenAComparerIsUsed_ShouldCompareEveryWindowWithIt()
		{
			StringEqualityOptions sut = new("expected");
			sut.Using(StringComparer.OrdinalIgnoreCase);

			int result = await sut.CountOccurrences("xabAB", "ab");

			await That(result).IsEqualTo(2);
		}

		[Test]
		public async Task CountOccurrences_WhenACustomMatchTypeRejectsTheExpectedValue_ShouldThrowAtTheCall()
		{
			ArgumentException exception = new("my rejection");
			ValidatingMatchType matchType = new(rejectExpected: _ => exception);
			StringEqualityOptions sut = new("expected");
			sut.IgnoringNewlineStyle();
			sut.SetMatchType(matchType, "AsCustom");

			void Act() => _ = sut.CountOccurrences("some text", "b\r\nar");

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
			StringEqualityOptions sut = new("expected");
			sut.IgnoringCase(ignoreCase);

			int result = await sut.CountOccurrences(actual, expected);

			await That(result).IsEqualTo(expectedCount)
				.Because("the occurrences are searched instead of compared with a window at every position, which must count alike");
		}

		[Test]
		public async Task CountOccurrences_WhenExpectedIsLongerThanActual_ShouldStillApplyTheOptions()
		{
			StringEqualityOptions sut = new("expected");
			sut.IgnoringTrailingWhiteSpace();

			int result = await sut.CountOccurrences("ab", "ab ");

			await That(result).IsEqualTo(1);
		}

		[Test]
		public async Task CountOccurrences_WhenExpectedIsPaddedWithWhiteSpace_ShouldOnlyIgnoreItAtTheEndOfTheSubject()
		{
			StringEqualityOptions sut = new("expected");
			sut.IgnoringTrailingWhiteSpace();

			int result = await sut.CountOccurrences("abab", "ab  ");

			await That(result).IsEqualTo(1)
				.Because("the first 'ab' is followed by another character instead of whitespace");
		}

		[Test]
		[Arguments(" ")]
		[Arguments("   ")]
		[Arguments("\t")]
		public async Task CountOccurrences_WhenExpectedNormalizesToEmpty_ShouldThrowArgumentException(string expected)
		{
			StringEqualityOptions sut = new("expected");
			sut.IgnoringIndentation();

			async Task Act() => await sut.CountOccurrences("some text", expected);

			await That(Act).Throws<ArgumentException>()
				.WithMessage("The 'expected' string cannot be empty.").AsPrefix().And
				.WithParamName("expected")
				.Because("an empty needle never occurs, so a negated expectation could never fail");
		}

		[Test]
		public async Task CountOccurrences_WhenIndentationIsIgnored_ShouldFindNestedMultiLineSnippet()
		{
			StringEqualityOptions sut = new("expected");
			sut.IgnoringIndentation();

			int result = await sut.CountOccurrences("class C\n{\n    Foo();\n    Bar();\n}", "Foo();\nBar();");

			await That(result).IsEqualTo(1);
		}

		[Test]
		public async Task CountOccurrences_WhenMatchingAsRegex_ShouldNotLimitTheWindowToThePatternLength()
		{
			StringEqualityOptions sut = new("expected");
			sut.AsRegex();

			int result = await sut.CountOccurrences("ab", "^.*$");

			await That(result).IsEqualTo(1);
		}

		[Test]
		public async Task CountOccurrences_WhenMatchingAsRegex_ShouldNotNormalizeTheIndividualWindows()
		{
			StringEqualityOptions sut = new("expected");
			sut.AsRegex().IgnoringIndentation();

			int result = await sut.CountOccurrences("x  abz", "^ab$");

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
			StringEqualityOptions sut = new("expected");
			sut.AsRegex().IgnoringLeadingWhiteSpace();

			int result = await sut.CountOccurrences(actual, " g.t");

			await That(result).IsEqualTo(expectedCount)
				.Because("the space of the pattern is only optional where the pattern reaches the start of the subject");
		}

		[Test]
		public async Task CountOccurrences_WhenMatchingAsWildcard_ShouldNotNormalizeTheIndividualWindows()
		{
			StringEqualityOptions sut = new("expected");
			sut.AsWildcard().IgnoringIndentation();

			int result = await sut.CountOccurrences("a b", "?b");

			await That(result).IsEqualTo(1);
		}

		[Test]
		[Arguments("forget", 0)]
		[Arguments("get", 1)]
		[Arguments("get got", 2)]
		public async Task CountOccurrences_WhenMatchingAsWildcard_ShouldOnlyIgnoreTheWhiteSpaceOfThePatternAtTheStartOfTheSubject(
			string actual, int expectedCount)
		{
			StringEqualityOptions sut = new("expected");
			sut.AsWildcard().IgnoringLeadingWhiteSpace();

			int result = await sut.CountOccurrences(actual, " g?t");

			await That(result).IsEqualTo(expectedCount)
				.Because("the space of the pattern is only optional where the pattern reaches the start of the subject");
		}

		[Test]
		public async Task CountOccurrences_WhenOccurrencesAreIndentedDifferently_ShouldCountAll()
		{
			StringEqualityOptions sut = new("expected");
			sut.IgnoringIndentation();

			int result = await sut.CountOccurrences("  a\n  b\nx\na\nb", "a\nb");

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
			StringEqualityOptions sut = new("expected");
			sut.IgnoringLeadingWhiteSpace().IgnoringTrailingWhiteSpace();

			int result = await sut.CountOccurrences(actual, expected);

			await That(result).IsEqualTo(expectedCount)
				.Because("the whitespace of the expected value is only optional where it reaches an edge of the subject, "
				         + "and whitespace alone never occurs at an edge");
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
			StringEqualityOptions sut = new("expected");
			sut.IgnoringLeadingWhiteSpace();
			sut.IgnoringTrailingWhiteSpace();
			sut.IgnoringNewlineStyle();

			string result = sut.GetExtendedFailure("it", ExpectationGrammars.None, null, "foo");

			await That(result).IsEqualTo("it was <null>");
		}

		[Test]
		public async Task GetExtendedFailure_WhenExpectedIsNull_ShouldOnlyTrimTheActualValue()
		{
			StringEqualityOptions sut = new("expected");
			sut.IgnoringLeadingWhiteSpace();
			sut.IgnoringTrailingWhiteSpace();

			string result = sut.GetExtendedFailure("it", ExpectationGrammars.None, " foo ", null);

			await That(result).IsEqualTo("it was \"foo\"");
		}

		[Test]
		public async Task GetExtendedFailure_WhenACustomMatchTypeThrowsWithoutComparer_ShouldNotCatchTheException()
		{
			StringEqualityOptions sut = new("expected");
			sut.SetMatchType(new CustomMatchType(), "AsCustom");

			void Act()
				=> sut.GetExtendedFailure("it", ExpectationGrammars.None, "foo", "bar");

			await That(Act).Throws<NotSupportedException>()
				.Because("without a comparer no code of the caller is called while the failure is explained");
		}

		[Test]
		[Arguments("Exact", "1.2.3", "1.2.4")]
		[Arguments("AsPrefix", "1.2.3.4", "1.2.4")]
		[Arguments("AsSuffix", "0.1.2.3", "1.2.4")]
		public async Task GetExtendedFailure_WhenTheComparerThrowsForAPartOfTheValues_ShouldOmitTheDifference(
			string matchType, string actual, string expected)
		{
			StringEqualityOptions sut = WithMatchType(matchType);
			sut.Using(new VersionStringComparer());

			bool isEqual = await sut.AreConsideredEqual(actual, expected);
			string result = sut.GetExtendedFailure("it", ExpectationGrammars.None, actual, expected);

			await That(isEqual).IsFalse();
			await That(result).IsEqualTo($"it was \"{actual}\"")
				.Because("the comparer answered for the values and is only called for parts of them to locate the difference");
		}

		[Test]
		public async Task GetExtendedFailure_WhenTheComparerThrowsForTrimmedValues_ShouldOmitTheDifference()
		{
			StringEqualityOptions sut = new("expected");
			sut.Using(new ThrowingForComparer("1.2.3"));

			string result = sut.GetExtendedFailure("it", ExpectationGrammars.None, " 1.2.3", "1.2.4");

			await That(result).IsEqualTo("it was \" 1.2.3\"");
		}

		[Test]
		public async Task GetExtendedFailure_WhenIndentationAndLeadingWhiteSpaceAreIgnored_ShouldReportOriginalPosition()
		{
			StringEqualityOptions sut = new("expected");
			sut.IgnoringIndentation().IgnoringLeadingWhiteSpace();

			string result = sut.GetExtendedFailure("it", ExpectationGrammars.None, "\n  foo", "bar");

			await That(result).Contains("differs on line 2 and column 3:");
		}

		[Test]
		public async Task GetExtendedFailure_WhenIndentationIsIgnored_ShouldReportColumnOfOriginalLine()
		{
			StringEqualityOptions sut = new("expected");
			sut.IgnoringIndentation();

			string result = sut.GetExtendedFailure("it", ExpectationGrammars.None, "foo\n    baz", "foo\nbar");

			await That(result).Contains("differs on line 2 and column 7:");
		}

		[Test]
		public async Task GetExtendedFailure_WhenIndentationIsIgnoredAsPrefix_ShouldReportOriginalIndex()
		{
			StringEqualityOptions sut = new("expected");
			sut.AsPrefix().IgnoringIndentation();

			string result = sut.GetExtendedFailure("it", ExpectationGrammars.None, "  foobar", "baz");

			await That(result).Contains("differs at index 2:");
		}

		[Test]
		public async Task GetExtendedFailure_WhenIndentationIsIgnoredAsSuffix_ShouldReportOriginalIndex()
		{
			StringEqualityOptions sut = new("expected");
			sut.AsSuffix().IgnoringIndentation();

			string result = sut.GetExtendedFailure("it", ExpectationGrammars.None, "  foobar", "baz");

			await That(result).Contains("differs at index 7:");
		}

		[Test]
		public async Task GetExtendedFailure_WhenIndentationIsIgnoredOnSingleLine_ShouldReportOriginalIndex()
		{
			StringEqualityOptions sut = new("expected");
			sut.IgnoringIndentation();

			string result = sut.GetExtendedFailure("it", ExpectationGrammars.None, "  foo", "bar");

			await That(result).Contains("differs at index 2:");
		}

		[Test]
		public async Task GetExtendedFailure_WhenLeadingWhiteSpaceIsIgnored_ShouldNotShiftColumnsOnLaterLines()
		{
			StringEqualityOptions sut = new("expected");
			sut.IgnoringLeadingWhiteSpace();

			string result = sut.GetExtendedFailure("it", ExpectationGrammars.None, "  a\nbcd", "a\nbXd");

			await That(result).Contains("differs on line 2 and column 2:");
		}

		[Test]
		public async Task GetExtendedMemberFailure_WhenACustomMatchTypeFails_ShouldKeepItsFailure()
		{
			StringEqualityOptions sut = new("expected");
			sut.SetMatchType(new DelegatingMatchType(() => new ValueTask<bool>(false), "my custom failure"), "AsCustom");

			string result = sut.GetExtendedMemberFailure("it", "message", ExpectationGrammars.None, "foo", "bar");

			await That(result).IsEqualTo("my custom failure")
				.Because("only the failures of the built-in match types can be rephrased");
		}

		[Test]
		[Arguments(false)]
		[Arguments(true)]
		public async Task IgnoringCase_WhenACustomMatchTypeIsSpecified_ShouldLetItValidateTheNewCasing(bool ignoreCase)
		{
			ValidatingMatchType matchType = new();
			StringEqualityOptions sut = new("expected");
			sut.SetMatchType(matchType, "AsCustom");

			sut.IgnoringCase(ignoreCase);

			await That(matchType.ValidatedOptions).IsEqualTo([(false, null), (ignoreCase, null),]);
			await That(sut.ToString()).IsEqualTo(ignoreCase ? " as custom ignoring case" : " as custom");
		}

		[Test]
		public async Task IgnoringCase_WhenACustomMatchTypeRejectsIt_ShouldThrowItsExceptionAndKeepTheOptions()
		{
			InvalidOperationException exception = new("my rejection");
			ValidatingMatchType matchType = new((ignoreCase, _) => ignoreCase ? exception : null);
			StringEqualityOptions sut = new("expected");
			sut.SetMatchType(matchType, "AsCustom");

			void Act() => sut.IgnoringCase();

			InvalidOperationException thrownException = await That(Act).Throws<InvalidOperationException>();
			await That(thrownException).IsSameAs(exception);
			await That(sut.ToString()).IsEqualTo(" as custom");
			await That(() => sut.IgnoringCase(false)).DoesNotThrow()
				.Because("the rejected call must not count as specified");
		}

		[Test]
		public async Task IgnoringCase_WhenAComparerIsUsed_ShouldNotAskACustomMatchType()
		{
			ValidatingMatchType matchType = new((ignoreCase, _) => ignoreCase ? new NotSupportedException() : null);
			StringEqualityOptions sut = new("expected");
			sut.SetMatchType(matchType, "AsCustom");
			sut.Using(StringComparer.Ordinal);

			void Act() => sut.IgnoringCase();

			await That(Act).Throws<InvalidOperationException>()
				.WithMessage(
					"IgnoringCase cannot be combined with a custom comparer; use a case-insensitive comparer instead.")
				.Because("a match type is only asked for a combination that the options accept on their own");
			await That(matchType.ValidatedOptions).IsEqualTo([(false, null), (false, StringComparer.Ordinal),]);
		}

		[Test]
		public async Task IgnoringCase_WhenAComparerIsUsed_ShouldThrowInvalidOperationException()
		{
			StringEqualityOptions sut = new("expected");
			sut.Using(StringComparer.Ordinal);

			void Act() => sut.IgnoringCase();

			await That(Act).Throws<InvalidOperationException>()
				.WithMessage(
					"IgnoringCase cannot be combined with a custom comparer; use a case-insensitive comparer instead.");
		}

		[Test]
		public async Task IgnoringCase_WithFalse_WhenAComparerIsUsed_ShouldNotThrow()
		{
			StringEqualityOptions sut = new("expected");
			sut.Using(StringComparer.Ordinal);

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
			StringEqualityOptions sut = new("expected");

			void Act()
			{
				if (matchTypeFirst)
				{
					Change(sut, matchType, true);
					sut.IgnoringCase();
				}
				else
				{
					sut.IgnoringCase();
					Change(sut, matchType, true);
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
		public async Task IgnoringOption_WhenMatchingAsBlock_ShouldThrowInvalidOperationException(string option)
		{
			StringEqualityOptions sut = new("expected");
			sut.AsBlock();

			void Act() => Change(sut, option, true);

			await That(Act).Throws<InvalidOperationException>()
				.WithMessage($"{option} cannot be combined with AsBlock.")
				.Because("a block compares the lines on its own");
		}

		[Test]
		[Arguments(nameof(StringEqualityOptions.IgnoringCase))]
		[Arguments(nameof(StringEqualityOptions.IgnoringIndentation))]
		[Arguments(nameof(StringEqualityOptions.IgnoringLeadingWhiteSpace))]
		[Arguments(nameof(StringEqualityOptions.IgnoringNewlineStyle))]
		[Arguments(nameof(StringEqualityOptions.IgnoringTrailingWhiteSpace))]
		public async Task IgnoringOption_WhenSpecifiedTwice_ShouldThrowInvalidOperationException(string option)
		{
			StringEqualityOptions sut = new("expected");
			Change(sut, option, false);

			void Act() => Change(sut, option, true);

			await That(Act).Throws<InvalidOperationException>()
				.WithMessage($"{option} cannot be specified more than once.")
				.Because("any explicit call counts as specified, also with false");
		}

		[Test]
		[Arguments(nameof(StringEqualityOptions.AsPrefix))]
		[Arguments(nameof(StringEqualityOptions.AsRegex))]
		[Arguments(nameof(StringEqualityOptions.AsSuffix))]
		[Arguments(nameof(StringEqualityOptions.AsWildcard))]
		[Arguments(nameof(StringEqualityOptions.Containing))]
		public async Task SetMatchType_WhenAMatchTypeIsSpecified_ShouldThrowInvalidOperationException(string option)
		{
			StringEqualityOptions sut = new("expected");
			Change(sut, option, true);

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
		public async Task SetMatchType_WhenAMatchTypeIsSpecified_ShouldNotAskTheNewMatchType(string option)
		{
			ValidatingMatchType matchType = new((_, _) => new NotSupportedException());
			StringEqualityOptions sut = new("expected");
			Change(sut, option, true);

			void Act() => sut.SetMatchType(matchType, "AsCustom");

			await That(Act).Throws<InvalidOperationException>()
				.WithMessage($"AsCustom cannot be combined with {option}.");
			await That(matchType.ValidatedOptions).IsEmpty();
		}

		[Test]
		public async Task SetMatchType_WhenCaseIsIgnored_ShouldLetTheMatchTypeValidateIt()
		{
			ValidatingMatchType matchType = new();
			StringEqualityOptions sut = new("expected");
			sut.IgnoringCase();

			sut.SetMatchType(matchType, "AsCustom");

			await That(matchType.ValidatedOptions).IsEqualTo([(true, null),]);
			await That(sut.ToString()).IsEqualTo(" as custom ignoring case");
		}

		[Test]
		public async Task SetMatchType_WhenAComparerIsUsed_ShouldLetTheMatchTypeValidateIt()
		{
			ValidatingMatchType matchType = new();
			StringEqualityOptions sut = new("expected");
			sut.Using(StringComparer.Ordinal);

			sut.SetMatchType(matchType, "AsCustom");

			await That(matchType.ValidatedOptions).IsEqualTo([(false, StringComparer.Ordinal),]);
			await That(sut.ToString()).IsEqualTo(" as custom using a comparer");
		}

		[Test]
		[Arguments(nameof(StringEqualityOptions.IgnoringCase))]
		[Arguments(nameof(StringEqualityOptions.Using))]
		public async Task SetMatchType_WhenTheMatchTypeRejectsTheSpecifiedOptions_ShouldThrowItsExceptionAndKeepTheOptions(
			string option)
		{
			InvalidOperationException exception = new("my rejection");
			ValidatingMatchType matchType = new((_, _) => exception);
			StringEqualityOptions sut = new("expected");
			Change(sut, option, true);
			string optionString = sut.ToString();

			void Act() => sut.SetMatchType(matchType, "AsCustom");

			InvalidOperationException thrownException = await That(Act).Throws<InvalidOperationException>();
			await That(thrownException).IsSameAs(exception);
			await That(sut.ToString()).IsEqualTo(optionString);
			await That(() => sut.SetMatchType(new ValidatingMatchType(), "AsOther")).DoesNotThrow()
				.Because("the rejected match type must not count as specified");
		}

		[Test]
		public async Task ToString_WhenCaseAndIndentationIsIgnored_ShouldIncludeOptions()
		{
			StringEqualityOptions sut = new("expected");
			sut.IgnoringCase().IgnoringIndentation();

			string result = sut.ToString();

			await That(result).IsEqualTo(" ignoring case and indentation");
		}

		[Test]
		public async Task ToString_WhenCaseAndNewlineStyleIsIgnored_ShouldIncludeOptions()
		{
			StringEqualityOptions sut = new("expected");
			sut.IgnoringCase().IgnoringNewlineStyle();

			string result = sut.ToString();

			await That(result).IsEqualTo(" ignoring case and newline style");
		}

		[Test]
		public async Task ToString_WhenCaseAndWhiteSpaceAndNewlineStyleAndIndentationIsIgnored_ShouldIncludeOptions()
		{
			StringEqualityOptions sut = new("expected");
			sut.IgnoringCase().IgnoringLeadingWhiteSpace()
				.IgnoringTrailingWhiteSpace()
				.IgnoringNewlineStyle()
				.IgnoringIndentation();

			string result = sut.ToString();

			await That(result).IsEqualTo(" ignoring case, whitespace, newline style and indentation");
		}

		[Test]
		public async Task ToString_WhenCaseAndWhiteSpaceAndNewlineStyleIsIgnored_ShouldIncludeOptions()
		{
			StringEqualityOptions sut = new("expected");
			sut.IgnoringCase().IgnoringLeadingWhiteSpace()
				.IgnoringTrailingWhiteSpace()
				.IgnoringNewlineStyle();

			string result = sut.ToString();

			await That(result).IsEqualTo(" ignoring case, whitespace and newline style");
		}

		[Test]
		public async Task ToString_WhenIndentationAndNewlineStyleIsIgnored_ShouldIncludeOptions()
		{
			StringEqualityOptions sut = new("expected");
			sut.IgnoringNewlineStyle().IgnoringIndentation();

			string result = sut.ToString();

			await That(result).IsEqualTo(" ignoring newline style and indentation");
		}

		[Test]
		public async Task ToString_WhenIndentationIsIgnored_ShouldIncludeOptions()
		{
			StringEqualityOptions sut = new("expected");
			sut.IgnoringIndentation();

			string result = sut.ToString();

			await That(result).IsEqualTo(" ignoring indentation");
		}

		[Test]
		public async Task ToString_WhenIndentationIsIgnoredWithComparer_ShouldIncludeOptions()
		{
			StringEqualityOptions sut = new("expected");
			sut.Using(StringComparer.Ordinal).IgnoringIndentation();

			string result = sut.ToString();

			// The comparer type is named differently on .NET Framework.
			await That(result).StartsWith(" using ").And.EndsWith(" ignoring indentation");
		}

		[Test]
		public async Task ToString_WhenWhiteSpaceAndNewlineStyleIsIgnored_ShouldIncludeOptions()
		{
			StringEqualityOptions sut = new("expected");
			sut.IgnoringLeadingWhiteSpace()
				.IgnoringTrailingWhiteSpace()
				.IgnoringNewlineStyle();

			string result = sut.ToString();

			await That(result).IsEqualTo(" ignoring whitespace and newline style");
		}

		[Test]
		public async Task Using_WhenACustomMatchTypeIsSpecified_ShouldLetItValidateTheNewComparer()
		{
			ValidatingMatchType matchType = new();
			StringEqualityOptions sut = new("expected");
			sut.SetMatchType(matchType, "AsCustom");
			sut.IgnoringCase(false);

			sut.Using(StringComparer.Ordinal);

			await That(matchType.ValidatedOptions)
				.IsEqualTo([(false, null), (false, null), (false, StringComparer.Ordinal),]);
			await That(sut.ToString()).IsEqualTo(" as custom using a comparer");
		}

		[Test]
		public async Task Using_WhenACustomMatchTypeRejectsIt_ShouldThrowItsExceptionAndKeepTheOptions()
		{
			InvalidOperationException exception = new("my rejection");
			ValidatingMatchType matchType = new((_, comparer)
				=> ReferenceEquals(comparer, StringComparer.Ordinal) ? exception : null);
			StringEqualityOptions sut = new("expected");
			sut.SetMatchType(matchType, "AsCustom");

			void Act() => sut.Using(StringComparer.Ordinal);

			InvalidOperationException thrownException = await That(Act).Throws<InvalidOperationException>();
			await That(thrownException).IsSameAs(exception);
			await That(sut.ToString()).IsEqualTo(" as custom");
			await That(() => sut.Using(StringComparer.OrdinalIgnoreCase)).DoesNotThrow()
				.Because("the rejected comparer must not count as specified");
		}

		[Test]
		public async Task Using_WhenCaseIsIgnored_ShouldNotAskACustomMatchType()
		{
			ValidatingMatchType matchType = new((_, comparer) => comparer is null ? null : new NotSupportedException());
			StringEqualityOptions sut = new("expected");
			sut.SetMatchType(matchType, "AsCustom");
			sut.IgnoringCase();

			void Act() => sut.Using(StringComparer.Ordinal);

			await That(Act).Throws<InvalidOperationException>()
				.WithMessage(
					"IgnoringCase cannot be combined with a custom comparer; use a case-insensitive comparer instead.")
				.Because("a match type is only asked for a combination that the options accept on their own");
			await That(matchType.ValidatedOptions).IsEqualTo([(false, null), (true, null),]);
		}

		[Test]
		public async Task Using_WhenAComparerIsUsed_ShouldThrowInvalidOperationException()
		{
			StringEqualityOptions sut = new("expected");
			sut.Using(StringComparer.Ordinal);

			void Act() => sut.Using(StringComparer.OrdinalIgnoreCase);

			await That(Act).Throws<InvalidOperationException>()
				.WithMessage("Using cannot be specified more than once.")
				.Because("the second comparer would silently replace the first one");
		}

		[Test]
		public async Task Using_WhenCaseIsExplicitlyNotIgnored_ShouldNotThrow()
		{
			StringEqualityOptions sut = new("expected");
			sut.IgnoringCase(false);

			void Act() => sut.Using(StringComparer.Ordinal);

			await That(Act).DoesNotThrow()
				.Because("the reset flag does not compete with the comparer");
		}

		[Test]
		public async Task Using_WhenCaseIsIgnored_ShouldThrowInvalidOperationException()
		{
			StringEqualityOptions sut = new("expected");
			sut.IgnoringCase();

			void Act() => sut.Using(StringComparer.Ordinal);

			await That(Act).Throws<InvalidOperationException>()
				.WithMessage(
					"IgnoringCase cannot be combined with a custom comparer; use a case-insensitive comparer instead.");
		}

		[Test]
		public async Task Using_WhenMatchingAsRegex_ShouldThrowInvalidOperationException()
		{
			StringEqualityOptions sut = new("expected");
			sut.AsRegex();

			void Act() => sut.Using(StringComparer.Ordinal);

			await That(Act).Throws<InvalidOperationException>()
				.WithMessage("A custom comparer is not supported for regex or wildcard matching.");
		}

		[Test]
		public async Task Using_WhenMatchingAsWildcard_ShouldThrowInvalidOperationException()
		{
			StringEqualityOptions sut = new("expected");
			sut.AsWildcard();

			void Act() => sut.Using(StringComparer.Ordinal);

			await That(Act).Throws<InvalidOperationException>()
				.WithMessage("A custom comparer is not supported for regex or wildcard matching.");
		}

		[Test]
		public async Task Using_WithNull_ShouldThrowArgumentNullException()
		{
			StringEqualityOptions sut = new("expected");

			void Act() => sut.Using(null!);

			await That(Act).Throws<ArgumentNullException>()
				.WithParamName("comparer").And
				.WithMessage("The 'comparer' cannot be null.").AsPrefix();
		}

		[Test]
		public async Task Using_WithNull_WhenAComparerIsUsed_ShouldThrowArgumentNullException()
		{
			StringEqualityOptions sut = new("expected");
			sut.Using(StringComparer.Ordinal);

			void Act() => sut.Using(null!);

			await That(Act).Throws<ArgumentNullException>()
				.WithParamName("comparer").And
				.WithMessage("The 'comparer' cannot be null.").AsPrefix()
				.Because("a comparer cannot be reset to the default one");
		}

		[Test]
		public async Task ValidateExpected_WhenACustomMatchTypeRejectsTheExpectedValue_ShouldThrowItsException()
		{
			ArgumentException exception = new("my rejection");
			ValidatingMatchType matchType = new(rejectExpected: expected => expected == "foo" ? exception : null);
			StringEqualityOptions sut = new("expected");
			sut.SetMatchType(matchType, "AsCustom");

			void Act() => sut.ValidateExpected("foo");

			ArgumentException thrownException = await That(Act).Throws<ArgumentException>();
			await That(thrownException).IsSameAs(exception);
			await That(() => sut.ValidateExpected("bar")).DoesNotThrow();
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
			StringEqualityOptions sut = new StringEqualityOptions("unexpected")
				.IgnoringLeadingWhiteSpace().IgnoringTrailingWhiteSpace();
			SetMatchType(sut, matchType);
			ArgumentException? comparisonException = null;
			try
			{
				await sut.AreConsideredEqual("foo", expected);
			}
			catch (ArgumentException exception)
			{
				comparisonException = exception;
			}

			void Act() => sut.ValidateExpected(expected);

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
			StringEqualityOptions sut = new("expected");
			SetMatchType(sut, matchType);

			void Act() => sut.ValidateExpected(expected);

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
			StringEqualityOptions sut = new("expected");
			sut.IgnoringLeadingWhiteSpace().IgnoringTrailingWhiteSpace().IgnoringNewlineStyle();
			sut.SetMatchType(matchType, "AsCustom");

			sut.ValidateExpected(expected);
			await sut.AreConsideredEqual("foo", expected);

			await That(matchType.ValidatedExpected).IsEqualTo([normalizedExpected, normalizedExpected,]);
			await That(matchType.ComparedExpected).IsEqualTo([normalizedExpected,]);
		}

		private static void SetMatchType(StringEqualityOptions options, string matchType)
		{
			switch (matchType)
			{
				case "AsBlock":
					options.AsBlock();
					break;
				case "AsPrefix":
					options.AsPrefix();
					break;
				case "AsRegex":
					options.AsRegex();
					break;
				case "AsSuffix":
					options.AsSuffix();
					break;
				case "AsWildcard":
					options.AsWildcard();
					break;
				case "Containing":
					options.Containing();
					break;
			}
		}

		private static void Change(StringEqualityOptions options, string option, bool enable)
		{
			switch (option)
			{
				case "AsBlock":
					options.AsBlock();
					break;
				case "AsPrefix":
					options.AsPrefix();
					break;
				case "AsRegex":
					options.AsRegex();
					break;
				case "AsRegexWithOptions":
					options.AsRegex(RegexOptions.Multiline);
					break;
				case "AsSuffix":
					options.AsSuffix();
					break;
				case "AsWildcard":
					options.AsWildcard();
					break;
				case "Containing":
					options.Containing();
					break;
				case "IgnoringCase":
					options.IgnoringCase(enable);
					break;
				case "IgnoringIndentation":
					options.IgnoringIndentation(enable);
					break;
				case "IgnoringLeadingWhiteSpace":
					options.IgnoringLeadingWhiteSpace(enable);
					break;
				case "IgnoringNewlineStyle":
					options.IgnoringNewlineStyle(enable);
					break;
				case "IgnoringTrailingWhiteSpace":
					options.IgnoringTrailingWhiteSpace(enable);
					break;
				case "Using":
					options.Using(StringComparer.Ordinal);
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(option), option, null);
			}
		}

		/// <remarks>
		///     The exact match type is the default, so it is selected by naming no method at all.
		/// </remarks>
		private static StringEqualityOptions WithMatchType(string matchType)
		{
			StringEqualityOptions options = new("expected");
			switch (matchType)
			{
				case "AsBlock":
					options.AsBlock();
					break;
				case "AsPrefix":
					options.AsPrefix();
					break;
				case "AsRegex":
					options.AsRegex();
					break;
				case "AsSuffix":
					options.AsSuffix();
					break;
				case "AsWildcard":
					options.AsWildcard();
					break;
				case "Containing":
					options.Containing();
					break;
			}

			return options;
		}

		private sealed class CustomMatchType : IStringMatchType
		{
			public bool InspectsSubject => false;

			public ValueTask<bool> AreConsideredEqual(string? actual, string? expected, bool ignoreCase,
				IEqualityComparer<string>? comparer)
				=> throw new NotSupportedException();

			public string GetExpectation(string? expected, ExpectationGrammars grammars)
				=> throw new NotSupportedException();

			public string GetExtendedFailure(string it, string? actual, string? expected, bool ignoreCase,
				IEqualityComparer<string> comparer, StringDifferenceSettings? settings)
				=> throw new NotSupportedException();

			public string GetTypeString() => throw new NotSupportedException();

			public string GetOptionString(bool ignoreCase, IEqualityComparer<string>? comparer)
				=> throw new NotSupportedException();

			public void ValidateOptions(bool ignoreCase, IEqualityComparer<string>? comparer)
			{
				// Every option is accepted.
			}

			public void ValidateExpected(string? expected)
			{
				// Every expected value is accepted.
			}
		}

		private sealed class DelegatingMatchType(Func<ValueTask<bool>> areConsideredEqual, string failure = "")
			: IStringMatchType
		{
			public bool InspectsSubject => false;

			public ValueTask<bool> AreConsideredEqual(string? actual, string? expected, bool ignoreCase,
				IEqualityComparer<string>? comparer)
				=> areConsideredEqual();

			public string GetExpectation(string? expected, ExpectationGrammars grammars)
				=> throw new NotSupportedException();

			public string GetExtendedFailure(string it, string? actual, string? expected, bool ignoreCase,
				IEqualityComparer<string> comparer, StringDifferenceSettings? settings)
				=> failure;

			public string GetTypeString() => throw new NotSupportedException();

			public string GetOptionString(bool ignoreCase, IEqualityComparer<string>? comparer)
				=> throw new NotSupportedException();

			public void ValidateOptions(bool ignoreCase, IEqualityComparer<string>? comparer)
			{
				// Every option is accepted.
			}

			public void ValidateExpected(string? expected)
			{
				// Every expected value is accepted.
			}
		}

		/// <remarks>
		///     Records what it is asked to validate and to compare, and rejects what the delegates return an exception
		///     for.
		/// </remarks>
		private sealed class ValidatingMatchType(
			Func<bool, IEqualityComparer<string>?, Exception?>? rejectOptions = null,
			Func<string?, Exception?>? rejectExpected = null)
			: IStringMatchType
		{
			public List<(bool IgnoreCase, IEqualityComparer<string>? Comparer)> ValidatedOptions { get; } = [];

			public List<string?> ValidatedExpected { get; } = [];

			public List<string?> ComparedExpected { get; } = [];

			public bool InspectsSubject => false;

			public ValueTask<bool> AreConsideredEqual(string? actual, string? expected, bool ignoreCase,
				IEqualityComparer<string>? comparer)
			{
				ComparedExpected.Add(expected);
				return new ValueTask<bool>(true);
			}

			public string GetExpectation(string? expected, ExpectationGrammars grammars)
				=> throw new NotSupportedException();

			public string GetExtendedFailure(string it, string? actual, string? expected, bool ignoreCase,
				IEqualityComparer<string> comparer, StringDifferenceSettings? settings)
				=> throw new NotSupportedException();

			public string GetTypeString() => " as custom";

			public string GetOptionString(bool ignoreCase, IEqualityComparer<string>? comparer)
				=> (ignoreCase ? " ignoring case" : "") + (comparer is null ? "" : " using a comparer");

			public void ValidateOptions(bool ignoreCase, IEqualityComparer<string>? comparer)
			{
				ValidatedOptions.Add((ignoreCase, comparer));
				if (rejectOptions?.Invoke(ignoreCase, comparer) is { } exception)
				{
					throw exception;
				}
			}

			public void ValidateExpected(string? expected)
			{
				ValidatedExpected.Add(expected);
				if (rejectExpected?.Invoke(expected) is { } exception)
				{
					throw exception;
				}
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
