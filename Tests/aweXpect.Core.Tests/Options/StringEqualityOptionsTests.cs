using System.Collections.Generic;
using System.Text.RegularExpressions;
using aweXpect.Core.Helpers;
using aweXpect.Options;

namespace aweXpect.Core.Tests.Options;

public sealed partial class StringEqualityOptionsTests
{
	public sealed class Tests
	{
		[Fact]
		public async Task AreConsideredEqual_Null_ShouldReturnFalse()
		{
			StringEqualityOptions sut = new("expected");
			sut.IgnoringLeadingWhiteSpace();
			sut.IgnoringTrailingWhiteSpace();
			sut.IgnoringNewlineStyle();

			bool result = await sut.AreConsideredEqual(null, "foo");

			await That(result).IsFalse();
		}

		[Fact]
		public async Task AreConsideredEqual_WhenAComparerIsUsed_ShouldStillApplyTheWhiteSpaceOptions()
		{
			StringEqualityOptions sut = new("expected");
			sut.Using(StringComparer.Ordinal).IgnoringLeadingWhiteSpace();

			bool result = await sut.AreConsideredEqual("  foo", "foo");

			await That(result).IsTrue()
				.Because("both values are normalized before the comparer sees them, so neither option is dropped");
		}

		[Fact]
		public async Task AreConsideredEqual_WhenIndentationIsIgnored_ShouldEmptyWhiteSpaceOnlyLines()
		{
			StringEqualityOptions sut = new("expected");
			sut.IgnoringIndentation();

			bool result = await sut.AreConsideredEqual("foo\n    ", "foo\n");

			await That(result).IsTrue();
		}

		[Theory]
		[InlineData("foo", "foo")]
		[InlineData("    foo", "foo")]
		[InlineData("\tfoo", "foo")]
		[InlineData("class C\n{\n    Foo();\n}", "class C\n{\nFoo();\n}")]
		[InlineData("class C\r\n{\r\n    Foo();\r\n}", "class C\n{\nFoo();\n}")]
		[InlineData("a\n   \nb", "a\n\nb")]
		[InlineData("\n  foo", "\nfoo")]
		public async Task AreConsideredEqual_WhenIndentationIsIgnored_ShouldRemoveLeadingWhiteSpacePerLine(
			string actual, string expected)
		{
			StringEqualityOptions sut = new("expected");
			sut.IgnoringIndentation();

			bool result = await sut.AreConsideredEqual(actual, expected);

			await That(result).IsTrue();
		}

		[Fact]
		public async Task AreConsideredEqual_WhenIndentationIsIgnored_ShouldStillConsiderTrailingWhiteSpace()
		{
			StringEqualityOptions sut = new("expected");
			sut.IgnoringIndentation();

			bool result = await sut.AreConsideredEqual("foo  \nbar", "foo\nbar");

			await That(result).IsFalse();
		}

		[Theory]
		[InlineData("AsBlock")]
		[InlineData("AsPrefix")]
		[InlineData("AsSuffix")]
		[InlineData("Containing")]
		[InlineData("Exact")]
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

		[Theory]
		[InlineData("AsRegex", "f.*", "b.*")]
		[InlineData("AsWildcard", "f*", "b*")]
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

		[Theory]
		[InlineData(nameof(StringEqualityOptions.IgnoringIndentation))]
		[InlineData(nameof(StringEqualityOptions.IgnoringLeadingWhiteSpace))]
		[InlineData(nameof(StringEqualityOptions.IgnoringNewlineStyle))]
		[InlineData(nameof(StringEqualityOptions.IgnoringTrailingWhiteSpace))]
		public async Task AsBlock_WhenALineOptionIsSpecified_ShouldThrowInvalidOperationException(string option)
		{
			StringEqualityOptions sut = new("expected");
			Change(sut, option, false);

			void Act() => sut.AsBlock();

			await That(Act).Throws<InvalidOperationException>()
				.WithMessage($"AsBlock cannot be combined with {option}.")
				.Because("a block compares the lines on its own, and any explicit call counts as specified");
		}

		[Fact]
		public async Task AsBlock_WhenAnotherMatchTypeIsSpecified_ShouldThrowInvalidOperationException()
		{
			StringEqualityOptions sut = new("expected");
			sut.AsPrefix();

			void Act() => sut.AsBlock();

			await That(Act).Throws<InvalidOperationException>()
				.WithMessage("AsBlock cannot be combined with AsPrefix.");
		}

		[Fact]
		public async Task AsRegex_WhenAComparerIsUsed_ShouldThrowInvalidOperationException()
		{
			StringEqualityOptions sut = new("expected");
			sut.Using(StringComparer.Ordinal);

			void Act() => sut.AsRegex();

			await That(Act).Throws<InvalidOperationException>()
				.WithMessage("A custom comparer is not supported for regex or wildcard matching.");
		}

		[Fact]
		public async Task AsRegex_WithOptions_WhenAComparerIsUsed_ShouldThrowInvalidOperationException()
		{
			StringEqualityOptions sut = new("expected");
			sut.Using(StringComparer.Ordinal);

			void Act() => sut.AsRegex(RegexOptions.Multiline);

			await That(Act).Throws<InvalidOperationException>()
				.WithMessage("A custom comparer is not supported for regex or wildcard matching.");
		}

		[Fact]
		public async Task AsWildcard_WhenAComparerIsUsed_ShouldThrowInvalidOperationException()
		{
			StringEqualityOptions sut = new("expected");
			sut.Using(StringComparer.Ordinal);

			void Act() => sut.AsWildcard();

			await That(Act).Throws<InvalidOperationException>()
				.WithMessage("A custom comparer is not supported for regex or wildcard matching.");
		}

		[Fact]
		public async Task ComparesByOrdinalEquality_ByDefault_ShouldBeTrue()
		{
			StringEqualityOptions sut = new("expected");

			await That(sut.ComparesByOrdinalEquality).IsTrue();
		}

		[Theory]
		[InlineData(nameof(StringEqualityOptions.AsBlock))]
		[InlineData(nameof(StringEqualityOptions.AsPrefix))]
		[InlineData(nameof(StringEqualityOptions.AsRegex))]
		[InlineData(nameof(StringEqualityOptions.AsRegex) + "WithOptions")]
		[InlineData(nameof(StringEqualityOptions.AsSuffix))]
		[InlineData(nameof(StringEqualityOptions.AsWildcard))]
		[InlineData(nameof(StringEqualityOptions.Containing))]
		[InlineData(nameof(StringEqualityOptions.IgnoringCase))]
		[InlineData(nameof(StringEqualityOptions.IgnoringIndentation))]
		[InlineData(nameof(StringEqualityOptions.IgnoringLeadingWhiteSpace))]
		[InlineData(nameof(StringEqualityOptions.IgnoringNewlineStyle))]
		[InlineData(nameof(StringEqualityOptions.IgnoringTrailingWhiteSpace))]
		[InlineData(nameof(StringEqualityOptions.Using))]
		public async Task ComparesByOrdinalEquality_WhenTheComparisonIsChanged_ShouldBeFalse(string option)
		{
			StringEqualityOptions sut = new("expected");
			Change(sut, option, true);

			await That(sut.ComparesByOrdinalEquality).IsFalse();
		}

		[Theory]
		[InlineData(nameof(StringEqualityOptions.IgnoringCase))]
		[InlineData(nameof(StringEqualityOptions.IgnoringIndentation))]
		[InlineData(nameof(StringEqualityOptions.IgnoringLeadingWhiteSpace))]
		[InlineData(nameof(StringEqualityOptions.IgnoringNewlineStyle))]
		[InlineData(nameof(StringEqualityOptions.IgnoringTrailingWhiteSpace))]
		public async Task ComparesByOrdinalEquality_WhenTheOptionIsDisabled_ShouldBeTrue(string option)
		{
			StringEqualityOptions sut = new("expected");
			Change(sut, option, false);

			await That(sut.ComparesByOrdinalEquality).IsTrue();
		}

		[Theory]
		[InlineData("aaaa", "aa", false, 2)]
		[InlineData("aaa", "aa", false, 1)]
		[InlineData("abcABCabc", "abc", false, 2)]
		[InlineData("abcABCabc", "abc", true, 3)]
		[InlineData("x\U00010400y\U00010428", "\U00010428", false, 1)]
		[InlineData("ab", "abc", false, 0)]
		public async Task CountOccurrences_WhenComparedOrdinally_ShouldCountTheNonOverlappingOccurrences(
			string actual, string expected, bool ignoreCase, int expectedCount)
		{
			StringEqualityOptions sut = new("expected");
			sut.IgnoringCase(ignoreCase);

			int result = await sut.CountOccurrences(actual, expected);

			await That(result).IsEqualTo(expectedCount)
				.Because("the occurrences are searched instead of compared with a window at every position, which must count alike");
		}

		[Fact]
		public async Task CountOccurrences_WhenExpectedIsLongerThanActual_ShouldStillApplyTheOptions()
		{
			StringEqualityOptions sut = new("expected");
			sut.IgnoringTrailingWhiteSpace();

			int result = await sut.CountOccurrences("ab", "ab ");

			await That(result).IsEqualTo(1);
		}

		[Fact]
		public async Task CountOccurrences_WhenExpectedIsPaddedWithWhiteSpace_ShouldOnlyIgnoreItAtTheEndOfTheSubject()
		{
			StringEqualityOptions sut = new("expected");
			sut.IgnoringTrailingWhiteSpace();

			int result = await sut.CountOccurrences("abab", "ab  ");

			await That(result).IsEqualTo(1)
				.Because("the first 'ab' is followed by another character instead of whitespace");
		}

		[Theory]
		[InlineData(" ")]
		[InlineData("   ")]
		[InlineData("\t")]
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

		[Fact]
		public async Task CountOccurrences_WhenIndentationIsIgnored_ShouldFindNestedMultiLineSnippet()
		{
			StringEqualityOptions sut = new("expected");
			sut.IgnoringIndentation();

			int result = await sut.CountOccurrences("class C\n{\n    Foo();\n    Bar();\n}", "Foo();\nBar();");

			await That(result).IsEqualTo(1);
		}

		[Fact]
		public async Task CountOccurrences_WhenMatchingAsRegex_ShouldNotLimitTheWindowToThePatternLength()
		{
			StringEqualityOptions sut = new("expected");
			sut.AsRegex();

			int result = await sut.CountOccurrences("ab", "^.*$");

			await That(result).IsEqualTo(1);
		}

		[Fact]
		public async Task CountOccurrences_WhenMatchingAsRegex_ShouldNotNormalizeTheIndividualWindows()
		{
			StringEqualityOptions sut = new("expected");
			sut.AsRegex().IgnoringIndentation();

			int result = await sut.CountOccurrences("x  abz", "^ab$");

			await That(result).IsEqualTo(0)
				.Because("the indentation is removed once from the whole string, not a second time from each occurrence");
		}

		[Theory]
		[InlineData("forget", 0)]
		[InlineData("get", 1)]
		[InlineData("for get get", 2)]
		public async Task CountOccurrences_WhenMatchingAsRegex_ShouldOnlyIgnoreTheWhiteSpaceOfThePatternAtTheStartOfTheSubject(
			string actual, int expectedCount)
		{
			StringEqualityOptions sut = new("expected");
			sut.AsRegex().IgnoringLeadingWhiteSpace();

			int result = await sut.CountOccurrences(actual, " g.t");

			await That(result).IsEqualTo(expectedCount)
				.Because("the space of the pattern is only optional where the pattern reaches the start of the subject");
		}

		[Fact]
		public async Task CountOccurrences_WhenMatchingAsWildcard_ShouldNotNormalizeTheIndividualWindows()
		{
			StringEqualityOptions sut = new("expected");
			sut.AsWildcard().IgnoringIndentation();

			int result = await sut.CountOccurrences("a b", "?b");

			await That(result).IsEqualTo(1);
		}

		[Theory]
		[InlineData("forget", 0)]
		[InlineData("get", 1)]
		[InlineData("get got", 2)]
		public async Task CountOccurrences_WhenMatchingAsWildcard_ShouldOnlyIgnoreTheWhiteSpaceOfThePatternAtTheStartOfTheSubject(
			string actual, int expectedCount)
		{
			StringEqualityOptions sut = new("expected");
			sut.AsWildcard().IgnoringLeadingWhiteSpace();

			int result = await sut.CountOccurrences(actual, " g?t");

			await That(result).IsEqualTo(expectedCount)
				.Because("the space of the pattern is only optional where the pattern reaches the start of the subject");
		}

		[Fact]
		public async Task CountOccurrences_WhenOccurrencesAreIndentedDifferently_ShouldCountAll()
		{
			StringEqualityOptions sut = new("expected");
			sut.IgnoringIndentation();

			int result = await sut.CountOccurrences("  a\n  b\nx\na\nb", "a\nb");

			await That(result).IsEqualTo(2);
		}

		[Theory]
		[InlineData("forget", " get", 0)]
		[InlineData("get get", " get", 2)]
		[InlineData("  get", "\tget", 1)]
		[InlineData("ab", " ab ", 1)]
		[InlineData("a b", " ", 1)]
		[InlineData(" ab ", " ", 0)]
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

		[Theory]
		[InlineData("AsBlock", ExpectationGrammars.Active, "matches \"foo\" as block")]
		[InlineData("AsBlock", ExpectationGrammars.Active | ExpectationGrammars.Plural, "match \"foo\" as block")]
		[InlineData("AsBlock", ExpectationGrammars.Active | ExpectationGrammars.Negated,
			"does not match \"foo\" as block")]
		[InlineData("AsBlock", ExpectationGrammars.Active | ExpectationGrammars.Plural | ExpectationGrammars.Negated,
			"do not match \"foo\" as block")]
		[InlineData("AsPrefix", ExpectationGrammars.Active, "starts with \"foo\"")]
		[InlineData("AsPrefix", ExpectationGrammars.Active | ExpectationGrammars.Plural, "start with \"foo\"")]
		[InlineData("AsPrefix", ExpectationGrammars.Active | ExpectationGrammars.Negated, "does not start with \"foo\"")]
		[InlineData("AsPrefix", ExpectationGrammars.Active | ExpectationGrammars.Plural | ExpectationGrammars.Negated,
			"do not start with \"foo\"")]
		[InlineData("AsRegex", ExpectationGrammars.Active, "matches regex \"foo\"")]
		[InlineData("AsRegex", ExpectationGrammars.Active | ExpectationGrammars.Plural, "match regex \"foo\"")]
		[InlineData("AsRegex", ExpectationGrammars.Active | ExpectationGrammars.Negated,
			"does not match regex \"foo\"")]
		[InlineData("AsRegex", ExpectationGrammars.Active | ExpectationGrammars.Plural | ExpectationGrammars.Negated,
			"do not match regex \"foo\"")]
		[InlineData("AsSuffix", ExpectationGrammars.Active, "ends with \"foo\"")]
		[InlineData("AsSuffix", ExpectationGrammars.Active | ExpectationGrammars.Plural, "end with \"foo\"")]
		[InlineData("AsSuffix", ExpectationGrammars.Active | ExpectationGrammars.Negated, "does not end with \"foo\"")]
		[InlineData("AsSuffix", ExpectationGrammars.Active | ExpectationGrammars.Plural | ExpectationGrammars.Negated,
			"do not end with \"foo\"")]
		[InlineData("AsWildcard", ExpectationGrammars.Active, "matches \"foo\"")]
		[InlineData("AsWildcard", ExpectationGrammars.Active | ExpectationGrammars.Plural, "match \"foo\"")]
		[InlineData("AsWildcard", ExpectationGrammars.Active | ExpectationGrammars.Negated, "does not match \"foo\"")]
		[InlineData("AsWildcard", ExpectationGrammars.Active | ExpectationGrammars.Plural | ExpectationGrammars.Negated,
			"do not match \"foo\"")]
		[InlineData("Containing", ExpectationGrammars.Active, "contains \"foo\"")]
		[InlineData("Containing", ExpectationGrammars.Active | ExpectationGrammars.Plural, "contain \"foo\"")]
		[InlineData("Containing", ExpectationGrammars.Active | ExpectationGrammars.Negated, "does not contain \"foo\"")]
		[InlineData("Containing", ExpectationGrammars.Active | ExpectationGrammars.Plural | ExpectationGrammars.Negated,
			"do not contain \"foo\"")]
		[InlineData("Exact", ExpectationGrammars.Active, "is equal to \"foo\"")]
		[InlineData("Exact", ExpectationGrammars.Active | ExpectationGrammars.Plural, "are equal to \"foo\"")]
		[InlineData("Exact", ExpectationGrammars.Active | ExpectationGrammars.Negated, "is not equal to \"foo\"")]
		[InlineData("Exact", ExpectationGrammars.Active | ExpectationGrammars.Plural | ExpectationGrammars.Negated,
			"are not equal to \"foo\"")]
		public async Task GetExpectation_ShouldAgreeWithTheNumberOfTheSubject(
			string matchType, ExpectationGrammars grammars, string expected)
		{
			StringEqualityOptions sut = WithMatchType(matchType);

			string result = sut.GetExpectation("foo", grammars);

			await That(result).IsEqualTo(expected);
		}

		[Theory]
		[InlineData("AsBlock", "matching \"foo\" as block")]
		[InlineData("AsPrefix", "starting with \"foo\"")]
		[InlineData("AsRegex", "matching regex \"foo\"")]
		[InlineData("AsSuffix", "ending with \"foo\"")]
		[InlineData("AsWildcard", "matching \"foo\"")]
		[InlineData("Containing", "containing \"foo\"")]
		[InlineData("Exact", "equal to \"foo\"")]
		public async Task GetExpectation_WhenPassive_ShouldIgnoreTheNumberOfTheSubject(
			string matchType, string expected)
		{
			StringEqualityOptions sut = WithMatchType(matchType);

			string result = sut.GetExpectation("foo", ExpectationGrammars.Plural);

			await That(result).IsEqualTo(expected)
				.Because("a participle has no number that it could agree with");
		}

		[Fact]
		public async Task GetExtendedFailure_Null_ShouldReturnItWasNull()
		{
			StringEqualityOptions sut = new("expected");
			sut.IgnoringLeadingWhiteSpace();
			sut.IgnoringTrailingWhiteSpace();
			sut.IgnoringNewlineStyle();

			string result = sut.GetExtendedFailure("it", ExpectationGrammars.None, null, "foo");

			await That(result).IsEqualTo("it was <null>");
		}

		[Fact]
		public async Task GetExtendedFailure_WhenIndentationAndLeadingWhiteSpaceAreIgnored_ShouldReportOriginalPosition()
		{
			StringEqualityOptions sut = new("expected");
			sut.IgnoringIndentation().IgnoringLeadingWhiteSpace();

			string result = sut.GetExtendedFailure("it", ExpectationGrammars.None, "\n  foo", "bar");

			await That(result).Contains("differs on line 2 and column 3:");
		}

		[Fact]
		public async Task GetExtendedFailure_WhenIndentationIsIgnored_ShouldReportColumnOfOriginalLine()
		{
			StringEqualityOptions sut = new("expected");
			sut.IgnoringIndentation();

			string result = sut.GetExtendedFailure("it", ExpectationGrammars.None, "foo\n    baz", "foo\nbar");

			await That(result).Contains("differs on line 2 and column 7:");
		}

		[Fact]
		public async Task GetExtendedFailure_WhenIndentationIsIgnoredAsPrefix_ShouldReportOriginalIndex()
		{
			StringEqualityOptions sut = new("expected");
			sut.AsPrefix().IgnoringIndentation();

			string result = sut.GetExtendedFailure("it", ExpectationGrammars.None, "  foobar", "baz");

			await That(result).Contains("differs at index 2:");
		}

		[Fact]
		public async Task GetExtendedFailure_WhenIndentationIsIgnoredAsSuffix_ShouldReportOriginalIndex()
		{
			StringEqualityOptions sut = new("expected");
			sut.AsSuffix().IgnoringIndentation();

			string result = sut.GetExtendedFailure("it", ExpectationGrammars.None, "  foobar", "baz");

			await That(result).Contains("differs at index 7:");
		}

		[Fact]
		public async Task GetExtendedFailure_WhenIndentationIsIgnoredOnSingleLine_ShouldReportOriginalIndex()
		{
			StringEqualityOptions sut = new("expected");
			sut.IgnoringIndentation();

			string result = sut.GetExtendedFailure("it", ExpectationGrammars.None, "  foo", "bar");

			await That(result).Contains("differs at index 2:");
		}

		[Fact]
		public async Task GetExtendedFailure_WhenLeadingWhiteSpaceIsIgnored_ShouldNotShiftColumnsOnLaterLines()
		{
			StringEqualityOptions sut = new("expected");
			sut.IgnoringLeadingWhiteSpace();

			string result = sut.GetExtendedFailure("it", ExpectationGrammars.None, "  a\nbcd", "a\nbXd");

			await That(result).Contains("differs on line 2 and column 2:");
		}

		[Fact]
		public async Task IgnoringCase_WhenAComparerIsUsed_ShouldThrowInvalidOperationException()
		{
			StringEqualityOptions sut = new("expected");
			sut.Using(StringComparer.Ordinal);

			void Act() => sut.IgnoringCase();

			await That(Act).Throws<InvalidOperationException>()
				.WithMessage(
					"IgnoringCase cannot be combined with a custom comparer; use a case-insensitive comparer instead.");
		}

		[Fact]
		public async Task IgnoringCase_WithFalse_WhenAComparerIsUsed_ShouldNotThrow()
		{
			StringEqualityOptions sut = new("expected");
			sut.Using(StringComparer.Ordinal);

			void Act() => sut.IgnoringCase(false);

			await That(Act).DoesNotThrow()
				.Because("resetting the flag keeps the comparer as the only relevant option");
		}

		[Theory]
		[InlineData(nameof(StringEqualityOptions.IgnoringIndentation))]
		[InlineData(nameof(StringEqualityOptions.IgnoringLeadingWhiteSpace))]
		[InlineData(nameof(StringEqualityOptions.IgnoringNewlineStyle))]
		[InlineData(nameof(StringEqualityOptions.IgnoringTrailingWhiteSpace))]
		public async Task IgnoringOption_WhenMatchingAsBlock_ShouldThrowInvalidOperationException(string option)
		{
			StringEqualityOptions sut = new("expected");
			sut.AsBlock();

			void Act() => Change(sut, option, true);

			await That(Act).Throws<InvalidOperationException>()
				.WithMessage($"{option} cannot be combined with AsBlock.")
				.Because("a block compares the lines on its own");
		}

		[Theory]
		[InlineData(nameof(StringEqualityOptions.IgnoringCase))]
		[InlineData(nameof(StringEqualityOptions.IgnoringIndentation))]
		[InlineData(nameof(StringEqualityOptions.IgnoringLeadingWhiteSpace))]
		[InlineData(nameof(StringEqualityOptions.IgnoringNewlineStyle))]
		[InlineData(nameof(StringEqualityOptions.IgnoringTrailingWhiteSpace))]
		public async Task IgnoringOption_WhenSpecifiedTwice_ShouldThrowInvalidOperationException(string option)
		{
			StringEqualityOptions sut = new("expected");
			Change(sut, option, false);

			void Act() => Change(sut, option, true);

			await That(Act).Throws<InvalidOperationException>()
				.WithMessage($"{option} cannot be specified more than once.")
				.Because("any explicit call counts as specified, also with false");
		}

		[Theory]
		[InlineData(nameof(StringEqualityOptions.AsPrefix))]
		[InlineData(nameof(StringEqualityOptions.AsRegex))]
		[InlineData(nameof(StringEqualityOptions.AsSuffix))]
		[InlineData(nameof(StringEqualityOptions.AsWildcard))]
		[InlineData(nameof(StringEqualityOptions.Containing))]
		public async Task SetMatchType_WhenAMatchTypeIsSpecified_ShouldThrowInvalidOperationException(string option)
		{
			StringEqualityOptions sut = new("expected");
			Change(sut, option, true);

			void Act() => sut.SetMatchType(new CustomMatchType(), "AsCustom");

			await That(Act).Throws<InvalidOperationException>()
				.WithMessage($"AsCustom cannot be combined with {option}.");
		}

		[Fact]
		public async Task ToString_WhenCaseAndIndentationIsIgnored_ShouldIncludeOptions()
		{
			StringEqualityOptions sut = new("expected");
			sut.IgnoringCase().IgnoringIndentation();

			string result = sut.ToString();

			await That(result).IsEqualTo(" ignoring case and indentation");
		}

		[Fact]
		public async Task ToString_WhenCaseAndNewlineStyleIsIgnored_ShouldIncludeOptions()
		{
			StringEqualityOptions sut = new("expected");
			sut.IgnoringCase().IgnoringNewlineStyle();

			string result = sut.ToString();

			await That(result).IsEqualTo(" ignoring case and newline style");
		}

		[Fact]
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

		[Fact]
		public async Task ToString_WhenCaseAndWhiteSpaceAndNewlineStyleIsIgnored_ShouldIncludeOptions()
		{
			StringEqualityOptions sut = new("expected");
			sut.IgnoringCase().IgnoringLeadingWhiteSpace()
				.IgnoringTrailingWhiteSpace()
				.IgnoringNewlineStyle();

			string result = sut.ToString();

			await That(result).IsEqualTo(" ignoring case, whitespace and newline style");
		}

		[Fact]
		public async Task ToString_WhenIndentationAndNewlineStyleIsIgnored_ShouldIncludeOptions()
		{
			StringEqualityOptions sut = new("expected");
			sut.IgnoringNewlineStyle().IgnoringIndentation();

			string result = sut.ToString();

			await That(result).IsEqualTo(" ignoring newline style and indentation");
		}

		[Fact]
		public async Task ToString_WhenIndentationIsIgnored_ShouldIncludeOptions()
		{
			StringEqualityOptions sut = new("expected");
			sut.IgnoringIndentation();

			string result = sut.ToString();

			await That(result).IsEqualTo(" ignoring indentation");
		}

		[Fact]
		public async Task ToString_WhenIndentationIsIgnoredWithComparer_ShouldIncludeOptions()
		{
			StringEqualityOptions sut = new("expected");
			sut.Using(StringComparer.Ordinal).IgnoringIndentation();

			string result = sut.ToString();

			// The comparer type is named differently on .NET Framework.
			await That(result).StartsWith(" using ").And.EndsWith(" ignoring indentation");
		}

		[Fact]
		public async Task ToString_WhenWhiteSpaceAndNewlineStyleIsIgnored_ShouldIncludeOptions()
		{
			StringEqualityOptions sut = new("expected");
			sut.IgnoringLeadingWhiteSpace()
				.IgnoringTrailingWhiteSpace()
				.IgnoringNewlineStyle();

			string result = sut.ToString();

			await That(result).IsEqualTo(" ignoring whitespace and newline style");
		}

		[Fact]
		public async Task Using_WhenAComparerIsUsed_ShouldThrowInvalidOperationException()
		{
			StringEqualityOptions sut = new("expected");
			sut.Using(StringComparer.Ordinal);

			void Act() => sut.Using(StringComparer.OrdinalIgnoreCase);

			await That(Act).Throws<InvalidOperationException>()
				.WithMessage("Using cannot be specified more than once.")
				.Because("the second comparer would silently replace the first one");
		}

		[Fact]
		public async Task Using_WhenCaseIsExplicitlyNotIgnored_ShouldNotThrow()
		{
			StringEqualityOptions sut = new("expected");
			sut.IgnoringCase(false);

			void Act() => sut.Using(StringComparer.Ordinal);

			await That(Act).DoesNotThrow()
				.Because("the reset flag does not compete with the comparer");
		}

		[Fact]
		public async Task Using_WhenCaseIsIgnored_ShouldThrowInvalidOperationException()
		{
			StringEqualityOptions sut = new("expected");
			sut.IgnoringCase();

			void Act() => sut.Using(StringComparer.Ordinal);

			await That(Act).Throws<InvalidOperationException>()
				.WithMessage(
					"IgnoringCase cannot be combined with a custom comparer; use a case-insensitive comparer instead.");
		}

		[Fact]
		public async Task Using_WhenMatchingAsRegex_ShouldThrowInvalidOperationException()
		{
			StringEqualityOptions sut = new("expected");
			sut.AsRegex();

			void Act() => sut.Using(StringComparer.Ordinal);

			await That(Act).Throws<InvalidOperationException>()
				.WithMessage("A custom comparer is not supported for regex or wildcard matching.");
		}

		[Fact]
		public async Task Using_WhenMatchingAsWildcard_ShouldThrowInvalidOperationException()
		{
			StringEqualityOptions sut = new("expected");
			sut.AsWildcard();

			void Act() => sut.Using(StringComparer.Ordinal);

			await That(Act).Throws<InvalidOperationException>()
				.WithMessage("A custom comparer is not supported for regex or wildcard matching.");
		}

		[Fact]
		public async Task Using_WithNull_ShouldThrowArgumentNullException()
		{
			StringEqualityOptions sut = new("expected");

			void Act() => sut.Using(null!);

			await That(Act).Throws<ArgumentNullException>()
				.WithParamName("comparer").And
				.WithMessage("The 'comparer' cannot be null.").AsPrefix();
		}

		[Fact]
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

		private sealed class ThrowingComparer(Exception exception) : IEqualityComparer<string>
		{
			public bool Equals(string? x, string? y) => throw exception;

			public int GetHashCode(string obj) => throw exception;
		}
	}
}
