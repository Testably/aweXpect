using System.Collections.Generic;
using aweXpect.Core.Helpers;
using aweXpect.Customization;

namespace aweXpect.Core.Tests.Core;

public class StringDifferenceTests
{
	[Theory]
	[InlineData(StringDifference.MatchType.Wildcard, 0)]
	[InlineData(StringDifference.MatchType.Regex, 0)]
	[InlineData(StringDifference.MatchType.Prefix, -1)]
	[InlineData(StringDifference.MatchType.Suffix, -1)]
	[InlineData(StringDifference.MatchType.Equality, -1)]
	public async Task ShouldCacheIndexOfFirstMismatch(StringDifference.MatchType matchType, int expectedIndex)
	{
		const string actual = "Foo";
		const string expected = "Foo";
		ExecuteOnceComparer comparer = new();

		StringDifference sut = new(actual, expected, comparer);

		await That(sut.IndexOfFirstMismatch(matchType)).IsEqualTo(expectedIndex);
		await That(sut.IndexOfFirstMismatch(matchType)).IsEqualTo(expectedIndex);
	}

	public sealed class WildcardOrRegexTests
	{
		[Theory]
		[InlineData(StringDifference.MatchType.Wildcard, "wildcard pattern")]
		[InlineData(StringDifference.MatchType.Regex, "regex pattern")]
		public async Task WhenActualValueIsNull_ShouldUsePatternName(StringDifference.MatchType matchType,
			string patternName)
		{
			const string? actual = null;
			const string expected = "This is a text";

			StringDifference sut = new(actual, expected,
				settings: new StringDifferenceSettings(0, 0).WithMatchType(matchType));

			await That(sut.ToString()).IsEqualTo(
				$"""
				 differs:
				   ↓ (actual)
				   <null>
				   "This is a text"
				   ↑ ({patternName})
				 """);
		}

		[Theory]
		[InlineData(StringDifference.MatchType.Wildcard, "wildcard pattern")]
		[InlineData(StringDifference.MatchType.Regex, "regex pattern")]
		public async Task WhenExpectedValueIsNull_ShouldUsePatternName(StringDifference.MatchType matchType,
			string patternName)
		{
			const string actual = "This is a text";
			const string? expected = null;

			StringDifference sut = new(actual, expected,
				settings: new StringDifferenceSettings(0, 0).WithMatchType(matchType));

			await That(sut.ToString()).IsEqualTo(
				$"""
				 differs:
				   ↓ (actual)
				   "This is a text"
				   <null>
				   ↑ ({patternName})
				 """);
		}

		[Theory]
		[InlineData(StringDifference.MatchType.Wildcard, "wildcard pattern")]
		[InlineData(StringDifference.MatchType.Regex, "regex pattern")]
		public async Task WhenMaximumStringLengthIsIncreased_ShouldShowLongerValues(
			StringDifference.MatchType matchType, string patternName)
		{
			string actual = new('a', 400);
			string expected = new('b', 400);
			StringDifference sut = new(actual, expected,
				settings: new StringDifferenceSettings(0, 0).WithMatchType(matchType));

			string result;
			using (Customize.aweXpect.Formatting().MaximumStringLength.Set(500))
			{
				result = sut.ToString();
			}

			await That(result).IsEqualTo(
					$"""
					 differs:
					   ↓ (actual)
					   "{actual}"
					   "{expected}"
					   ↑ ({patternName})
					 """)
				.Because("the configured maximum string length limits the shown values");
		}
	}

	public sealed class EqualityTests
	{
		[Fact]
		public async Task WhenActualValueIsLongerThanExpected_ShouldDifferAtIndexActualLength()
		{
			const string actual = "A text that is longer";
			const string expected = "A text";

			StringDifference sut = new(actual, expected);

			await That(sut.IndexOfFirstMismatch(StringDifference.MatchType.Equality)).IsEqualTo(6);
			await That(sut.ToString()).IsEqualTo(
				"""
				differs at index 6:
				         ↓ (actual)
				  "A text that is longer"
				  "A text"
				         ↑ (expected)
				""");
		}

		[Fact]
		public async Task WhenActualValueIsNull_ShouldDifferAtIndex0()
		{
			const string? actual = null;
			const string expected = "This is a text";

			StringDifference sut = new(actual, expected);

			await That(sut.IndexOfFirstMismatch(StringDifference.MatchType.Equality)).IsEqualTo(0);
			await That(sut.ToString()).IsEqualTo(
				"""
				differs:
				  ↓ (actual)
				  <null>
				  "This is a text"
				  ↑ (expected)
				""");
		}

		[Fact]
		public async Task WhenActualValueIsShorterThanExpected_ShouldDifferAtIndexExpectedLength()
		{
			const string actual = "A text";
			const string expected = "A text that is longer";

			StringDifference sut = new(actual, expected);

			await That(sut.IndexOfFirstMismatch(StringDifference.MatchType.Equality)).IsEqualTo(6);
			await That(sut.ToString()).IsEqualTo(
				"""
				differs at index 6:
				         ↓ (actual)
				  "A text"
				  "A text that is longer"
				         ↑ (expected)
				""");
		}

		[Fact]
		public async Task WhenExpectedValueIsNull_ShouldDifferAtIndex0()
		{
			const string actual = "This is a text";
			const string? expected = null;

			StringDifference sut = new(actual, expected);

			await That(sut.IndexOfFirstMismatch(StringDifference.MatchType.Equality)).IsEqualTo(0);
			await That(sut.ToString()).IsEqualTo(
				"""
				differs:
				  ↓ (actual)
				  "This is a text"
				  <null>
				  ↑ (expected)
				""");
		}

		[Fact]
		public async Task WhenFirstMismatchIsBelow11Characters_ShouldIncludeCompleteTextBeforeFirstMismatch()
		{
			const string actual = "This is a long text";
			const string expected = "This is a text that differs at index 10";

			StringDifference sut = new(actual, expected);

			await That(sut.IndexOfFirstMismatch(StringDifference.MatchType.Equality)).IsEqualTo(10);
			await That(sut.ToString()).IsEqualTo(
				"""
				differs at index 10:
				             ↓ (actual)
				  "This is a long text"
				  "This is a text that differs at index 10"
				             ↑ (expected)
				""");
		}

		[Fact]
		public async Task WhenIgnoringTrailingColumns_ShouldOnlyShiftTheFirstComparedLine()
		{
			StringDifference firstLine = new("abc\nd", "aXc\nd", null, new StringDifferenceSettings(0, 2));
			StringDifference laterLine = new("a\nbcd", "a\nbXd", null, new StringDifferenceSettings(0, 2));

			await That(firstLine.ToString()).IsEqualTo(
				"""
				differs on line 1 and column 4:
				    ↓ (actual)
				  "abc\nd"
				  "aXc\nd"
				    ↑ (expected)
				""");
			await That(laterLine.ToString()).IsEqualTo(
				"""
				differs on line 2 and column 2:
				       ↓ (actual)
				  "a\nbcd"
				  "a\nbXd"
				       ↑ (expected)
				""");
		}

		[Fact]
		public async Task WhenLongTextDiffers_ShouldCalculateIndexOfFirstMismatch()
		{
			const string actual = "this is a long text that differs in between two words";
			const string expected = "this is a long text which differs in between two words";

			StringDifference sut = new(actual, expected);

			await That(sut.IndexOfFirstMismatch(StringDifference.MatchType.Equality)).IsEqualTo(20);
			await That(sut.ToString()).IsEqualTo(
				"""
				differs at index 20:
				                   ↓ (actual)
				  "…is a long text that differs in between two words"
				  "…is a long text which differs in between two words"
				                   ↑ (expected)
				""");
		}

		[Fact]
		public async Task WhenMinimumNumberOfCharactersAfterStringDifferenceIsSmall_ShouldStillShowTheMismatch()
		{
			StringDifference sut = new("abcdefghijklmnopqrstuvwxyzX", "abcdefghijklmnopqrstuvwxyzY");

			string result;
			using (Customize.aweXpect.Formatting().MinimumNumberOfCharactersAfterStringDifference.Set(5))
			{
				result = sut.ToString();
			}

			await That(result).IsEqualTo(
					"""
					differs at index 26:
					              ↓ (actual)
					  "…qrstuvwxyzX"
					  "…qrstuvwxyzY"
					              ↑ (expected)
					""")
				.Because("the arrows must point at a visible mismatching character");
		}

		[Fact]
		public async Task WhenMismatchIsFarFromTheStart_ShouldShowTheMinimumNumberOfCharactersAfterTheMismatch()
		{
			StringDifference sut = new(
				$"{new string('a', 100)}X{new string('b', 100)}",
				$"{new string('a', 100)}Y{new string('b', 100)}");

			string result = sut.ToString();

			await That(result).IsEqualTo(
					$"""
					 differs at index 100:
					               ↓ (actual)
					   "…aaaaaaaaaaX{new string('b', 49)}…"
					   "…aaaaaaaaaaY{new string('b', 49)}…"
					               ↑ (expected)
					 """)
				.Because("at least 45 characters must follow the mismatch by default");
		}

		[Fact]
		public async Task WhenNoLeadingWordBoundaryExistsBetween5And15Characters_ShouldFallbackTo10Characters()
		{
			const string actual =
				"This text '_contains' a long word between 5 and 15 characters before the first mismatch";
			const string expected =
				"This text '_contains' a loNg word between 5 and 15 characters before the first mismatch";
			StringDifference sut = new(actual, expected);

			string result = sut.ToString();

			await That(result).IsEqualTo(
				"""
				differs at index 26:
				              ↓ (actual)
				  "…ains' a long word between 5 and 15 characters before the first…"
				  "…ains' a loNg word between 5 and 15 characters before the first…"
				              ↑ (expected)
				""");
		}

		[Fact]
		public async Task WhenNoTrailingWordBoundaryExistsBetween45And60CharactersAfterTheMismatch_ShouldFallbackTo50Characters()
		{
			const string actual = "This text contains lot of words and is used for testing the WordBoundaryAlgorithm";
			const string expected =
				"This text is used to verify when  between  45 and60characters_no_word_boundary exists";
			StringDifference sut = new(actual, expected);

			string result = sut.ToString();

			await That(result).IsEqualTo(
				"""
				differs at index 10:
				             ↓ (actual)
				  "This text contains lot of words and is used for testing the…"
				  "This text is used to verify when  between  45 and60character…"
				             ↑ (expected)
				""");
		}

		[Fact]
		public async Task WhenStringContainsEscapedCharacters_ShouldPositionArrowsCorrectly()
		{
			const string actual = "a\\b\"c\u00A0dX";
			const string expected = "a\\b\"c\u00A0dY";

			StringDifference sut = new(actual, expected);

			await That(sut.IndexOfFirstMismatch(StringDifference.MatchType.Equality)).IsEqualTo(7);
			await That(sut.ToString()).IsEqualTo(
				"""
				differs at index 7:
				                 ↓ (actual)
				  "a\\b\"c\u00A0dX"
				  "a\\b\"c\u00A0dY"
				                 ↑ (expected)
				""").Because("the arrows must point at the mismatch in the escaped text");
		}

		[Fact]
		public async Task WhenStringContainsWhitespace_ShouldPositionArrowsCorrectly()
		{
			const string actual = "foo\r\tbar\nBAZ";
			const string expected = "foo\r\tbar\nbaz";

			StringDifference sut = new(actual, expected);

			await That(sut.IndexOfFirstMismatch(StringDifference.MatchType.Equality)).IsEqualTo(9);
			await That(sut.ToString()).IsEqualTo(
				"""
				differs on line 2 and column 1:
				               ↓ (actual)
				  "foo\r\tbar\nBAZ"
				  "foo\r\tbar\nbaz"
				               ↑ (expected)
				""");
		}

		[Fact]
		public async Task WhenStringsDifferInCaseOnly_ShouldDefaultToCaseSensitiveComparison()
		{
			const string actual = "this IS a text that only differs in casing";
			const string expected = "this is a text that only differs in casing";

			StringDifference sut = new(actual, expected);

			await That(sut.IndexOfFirstMismatch(StringDifference.MatchType.Equality)).IsEqualTo(5);
			await That(sut.ToString()).IsEqualTo(
				"""
				differs at index 5:
				        ↓ (actual)
				  "this IS a text that only differs in casing"
				  "this is a text that only differs in casing"
				        ↑ (expected)
				""");
		}

		[Fact]
		public async Task WhenStringsDifferInCaseOnly_WhenUsingAComparer_ShouldCompareSubstringsWithComparer()
		{
			const string actual = "this IS a text that only differs in casing";
			const string expected = "this is a text that only differs in casing";
			StringComparer comparer = StringComparer.OrdinalIgnoreCase;

			StringDifference sut = new(actual, expected, comparer);

			await That(sut.IndexOfFirstMismatch(StringDifference.MatchType.Equality)).IsEqualTo(-1);
			await That(sut.ToString()).IsEqualTo("differs");
		}

		[Theory]
		[InlineData("foo", "bar", 0)]
		[InlineData("foo", "false", 1)]
		[InlineData("bar", "ban", 2)]
		[InlineData("foobar", "foo-", 3)]
		public async Task WhenTextDiffers_ShouldCalculateIndexOfFirstMismatch(
			string actual, string expected, int expectedIndex)
		{
			StringDifference sut = new(actual, expected);

			await That(sut.IndexOfFirstMismatch(StringDifference.MatchType.Equality)).IsEqualTo(expectedIndex);
		}

		[Fact]
		public async Task WhenTextHasLineFeedsOnly_ShouldIncludeLineAndColumnNumbers()
		{
			StringDifference sut = new(
				"first line\nsecond line\nthird line with X here\nlast line",
				"first line\nsecond line\nthird line with Y here\nlast line");

			await That(sut.ToString()).IsEqualTo(
				"""
				differs on line 3 and column 17:
				              ↓ (actual)
				  "…line with X here\nlast line"
				  "…line with Y here\nlast line"
				              ↑ (expected)
				""");
		}

		[Fact]
		public async Task WhenTextHasMultipleLines_ShouldIncludeLineAndColumnNumbers()
		{
			int expectedIndex = 100 + (3 * Environment.NewLine.Length);
			string nl = Environment.NewLine.DisplayWhitespace();

			string actual = """
			                @startuml
			                Alice -> Bob : Authentication Request
			                Bob --> Alice : Authentication Response
			                Alice -> Bob : Another authentication Request
			                Alice <-- Bob : Another authentication Response
			                @enduml
			                """;

			string expected = """
			                  @startuml
			                  Alice -> Bob : Authentication Request
			                  Bob --> Alice : Authentication Response
			                  Alice -> Bob : Invalid authentication Request
			                  Alice <-- Bob : Another authentication Response
			                  @enduml
			                  """;

			StringDifference sut = new(actual, expected);

			await That(sut.IndexOfFirstMismatch(StringDifference.MatchType.Equality)).IsEqualTo(expectedIndex);
			await That(sut.ToString()).IsEqualTo(
				$"""
				 differs on line 4 and column 16:
				              ↓ (actual)
				   "…-> Bob : Another authentication Request{nl}Alice <-- Bob : Another…"
				   "…-> Bob : Invalid authentication Request{nl}Alice <-- Bob : Another…"
				              ↑ (expected)
				 """);
		}

		[Fact]
		public async Task WhenTextIsSame_ShouldSetIndexOfFirstMismatchToNegativeOne()
		{
			const string actual = "this is a text that does not differ";

			StringDifference sut = new(actual, actual);

			await That(sut.IndexOfFirstMismatch(StringDifference.MatchType.Equality)).IsEqualTo(-1);
			await That(sut.ToString()).IsEqualTo("differs");
		}

		[Fact]
		public async Task WhenUsingACultureAwareComparer_ShouldCompareWithTheCulture()
		{
			StringDifference sut = new(
				"Prefix with an accented cafe\u0301 and MORE text X here",
				"prefix with an accented café and more text Y here",
				StringComparer.InvariantCultureIgnoreCase);

			await That(sut.ToString()).IsEqualTo(
				"""
				differs at index 27:
				                   ↓ (actual)
				  "…an accented cafe\u0301 and MORE text X here"
				  "…an accented café and more text Y here"
				                   ↑ (expected)
				""");
		}

		[Fact]
		public async Task WhenUsingACustomComparer_ShouldCompareWithTheComparer()
		{
			StringDifference sut = new(
				"some TEXT that differs at X here",
				"some text that differs at Y here",
				new UpperCaseComparer());

			await That(sut.ToString()).IsEqualTo(
				"""
				differs at index 26:
				               ↓ (actual)
				  "…differs at X here"
				  "…differs at Y here"
				               ↑ (expected)
				""");
		}

		[Fact]
		public async Task WhenUsingTheCurrentCultureComparer_ShouldCompareWithTheCurrentCulture()
		{
			StringDifference sut = new(
				"some TEXT that differs at X here",
				"some text that differs at Y here",
				StringComparer.CurrentCultureIgnoreCase);

			await That(sut.ToString()).IsEqualTo(
				"""
				differs at index 26:
				               ↓ (actual)
				  "…differs at X here"
				  "…differs at Y here"
				               ↑ (expected)
				""");
		}

		[Fact]
		public async Task WhenWindowContainsADecomposedCharacter_ShouldEscapeTheCombiningMark()
		{
			StringDifference sut = new(
				"some text with a cafe\u0301 and X here",
				"some text with a cafe\u0301 and Y here");

			await That(sut.ToString()).IsEqualTo(
				"""
				differs at index 27:
				                     ↓ (actual)
				  "…a cafe\u0301 and X here"
				  "…a cafe\u0301 and Y here"
				                     ↑ (expected)
				""");
		}

		[Fact]
		public async Task WhenWindowEndsInsideLineBreak_ShouldIncludeTheWholeLineBreak()
		{
			StringDifference sut = new(
				$"abcX{new string('d', 48)}\r\ntail",
				$"abcY{new string('d', 48)}\r\ntail");

			await That(sut.ToString()).IsEqualTo(
					$"""
					 differs on line 1 and column 4:
					       ↓ (actual)
					   "abcX{new string('d', 48)}\r\n…"
					   "abcY{new string('d', 48)}\r\n…"
					       ↑ (expected)
					 """)
				.Because("the window must not end between the \\r and the \\n of a line break");
		}

		[Fact]
		public async Task WhenWindowEndsInsideSurrogatePair_ShouldIncludeTheWholePair()
		{
			StringDifference sut = new(
				$"abcX{new string('d', 48)}😀tail",
				$"abcY{new string('d', 48)}😀tail");

			await That(sut.ToString()).IsEqualTo(
					$"""
					 differs at index 3:
					       ↓ (actual)
					   "abcX{new string('d', 48)}😀…"
					   "abcY{new string('d', 48)}😀…"
					       ↑ (expected)
					 """)
				.Because("the window must not end between the two halves of a surrogate pair");
		}

		[Fact]
		public async Task WhenWindowStartsInsideLineBreak_ShouldIncludeTheWholeLineBreak()
		{
			StringDifference sut = new(
				$"{new string('a', 20)}\r\nbbbbbbbbbX",
				$"{new string('a', 20)}\r\nbbbbbbbbbY");

			await That(sut.ToString()).IsEqualTo(
					"""
					differs on line 2 and column 10:
					                 ↓ (actual)
					  "…\r\nbbbbbbbbbX"
					  "…\r\nbbbbbbbbbY"
					                 ↑ (expected)
					""")
				.Because("the window must not start between the \\r and the \\n of a line break");
		}

		[Fact]
		public async Task WhenWindowStartsInsideSurrogatePair_ShouldIncludeTheWholePair()
		{
			StringDifference sut = new(
				$"{new string('a', 20)}😀bbbbbbbbbX",
				$"{new string('a', 20)}😀bbbbbbbbbY");

			await That(sut.ToString()).IsEqualTo(
					"""
					differs at index 31:
					               ↓ (actual)
					  "…😀bbbbbbbbbX"
					  "…😀bbbbbbbbbY"
					               ↑ (expected)
					""")
				.Because("the window must not start between the two halves of a surrogate pair");
		}
	}

	public sealed class SuffixTests
	{
		[Fact]
		public async Task WhenActualValueIsNull_ShouldDifferAtIndex0()
		{
			const string? actual = null;
			const string expected = "This is a text";

			StringDifference sut = new(actual, expected, null, Settings);

			await That(sut.IndexOfFirstMismatch(StringDifference.MatchType.Suffix)).IsEqualTo(0);
			await That(sut.ToString()).IsEqualTo(
				"""
				differs:
				  ↓ (actual)
				  <null>
				  "This is a text"
				  ↑ (expected suffix)
				""");
		}

		[Fact]
		public async Task WhenActualValueIsShorterThanExpected_ShouldDifferAtIndexMinusOne()
		{
			const string actual = "that is longer";
			const string expected = "A text that is longer";

			StringDifference sut = new(actual, expected, null, Settings);

			await That(sut.IndexOfFirstMismatch(StringDifference.MatchType.Suffix)).IsEqualTo(-1);
			await That(sut.ToString()).IsEqualTo(
				"""
				is shorter than the expected length of 21 and misses the prefix:
				  "A text "
				""");
		}

		[Fact]
		public async Task WhenActualValueIsShorterThanExpected_ShouldEscapeAndTruncateTheMissingPrefix()
		{
			const string actual = "tail";
			const string expected = "\"a\\b\ncdefghijk tail";

			StringDifference sut = new(actual, expected, null, Settings);

			using (Customize.aweXpect.Formatting().MaximumStringLength.Set(10))
			{
				await That(sut.ToString()).IsEqualTo(
					"""
					is shorter than the expected length of 19 and misses the prefix:
					  "\"a\\b\ncdefg…"
					""").Because(
					"the missing prefix is shown like the other values in a message, whose limit counts the characters of the value and not their escapes");
			}
		}

		[Fact]
		public async Task WhenExpectedValueIsNull_ShouldDifferAtIndex0()
		{
			const string actual = "This is a text";
			const string? expected = null;

			StringDifference sut = new(actual, expected, null, Settings);

			await That(sut.IndexOfFirstMismatch(StringDifference.MatchType.Suffix)).IsEqualTo(0);
			await That(sut.ToString()).IsEqualTo(
				"""
				differs:
				  ↓ (actual)
				  "This is a text"
				  <null>
				  ↑ (expected suffix)
				""");
		}

		[Fact]
		public async Task WhenFirstMismatchIsBelow14Characters_ShouldIncludeCompleteTextBeforeFirstMismatch()
		{
			const string actual = "This is a long text";
			const string expected = "This is a text";

			StringDifference sut = new(actual, expected, null, Settings);

			await That(sut.IndexOfFirstMismatch(StringDifference.MatchType.Suffix)).IsEqualTo(13);
			await That(sut.ToString()).IsEqualTo(
				"""
				differs before index 13:
				                ↓ (actual)
				  "This is a long text"
				       "This is a text"
				                ↑ (expected suffix)
				""");
		}

		[Fact]
		public async Task WhenLongTextDiffers_ShouldCalculateIndexOfFirstMismatch()
		{
			const string actual = "this is a long text that differs in between two words";
			const string expected = "this is a long text which differs in between two words";

			StringDifference sut = new(actual, expected, null, Settings);

			await That(sut.IndexOfFirstMismatch(StringDifference.MatchType.Suffix)).IsEqualTo(23);
			await That(sut.ToString()).IsEqualTo(
				"""
				differs before index 23:
				                           ↓ (actual)
				   "this is a long text that differs in between two…"
				  "this is a long text which differs in between two…"
				                           ↑ (expected suffix)
				""");
		}

		[Fact]
		public async Task WhenMinimumNumberOfCharactersAfterStringDifferenceIsSmall_ShouldShowTheDifference()
		{
			const string actual = "abcdefghijkX";
			const string expected = "aY";
			string result;

			using (Customize.aweXpect.Formatting().MinimumNumberOfCharactersAfterStringDifference.Set(5))
			{
				StringDifference sut = new(actual, expected, null, Settings);
				result = sut.ToString();
			}

			await That(result).IsEqualTo(
				"""
				differs before index 11:
				              ↓ (actual)
				  "…bcdefghijkX"
				            "aY"
				              ↑ (expected suffix)
				""");
		}

		[Fact]
		public async Task WhenNoLeadingWordBoundaryExistsBetween45And60Characters_ShouldFallbackTo50Characters()
		{
			const string actual = "This text contains lot of words and is used for testing the WordBoundaryAlgorithm";
			const string expected =
				"This text contains lot of words and is used for testing the end of the WordBoundaryAlgorithm";
			StringDifference sut = new(actual, expected, null, Settings);

			string result = sut.ToString();

			await That(result).IsEqualTo(
				"""
				differs before index 54:
				                                                                ↓ (actual)
				             "…text contains lot of words and is used for testing the WordBound…"
				  "…text contains lot of words and is used for testing the end of the WordBound…"
				                                                                ↑ (expected suffix)
				""");
		}

		[Fact]
		public async Task WhenNoTrailingWordBoundaryExistsBetween5And15Characters_ShouldFallbackTo14Characters()
		{
			const string actual =
				"This text '_contains' a long word between 5 and 15 characters after tHe last mismatch";
			const string expected =
				"This text '_contains' a long word between 5 and 15 characters after the last mismatch";
			StringDifference sut = new(actual, expected, null, Settings);

			string result = sut.ToString();

			await That(result).IsEqualTo(
				"""
				differs before index 69:
				                                                               ↓ (actual)
				  "…'_contains' a long word between 5 and 15 characters after tHe last mismatc…"
				  "…'_contains' a long word between 5 and 15 characters after the last mismatc…"
				                                                               ↑ (expected suffix)
				""");
		}

		[Fact]
		public async Task WhenStringContainsEscapedCharacters_ShouldPositionArrowsCorrectly()
		{
			const string actual = "a\\b\"c\u00A0dXe";
			const string expected = "a\\b\"c\u00A0dYe";

			StringDifference sut = new(actual, expected, null, Settings);

			await That(sut.IndexOfFirstMismatch(StringDifference.MatchType.Suffix)).IsEqualTo(7);
			await That(sut.ToString()).IsEqualTo(
				"""
				differs before index 7:
				                 ↓ (actual)
				  "a\\b\"c\u00A0dXe"
				  "a\\b\"c\u00A0dYe"
				                 ↑ (expected suffix)
				""").Because("the arrows must point at the mismatch in the escaped text");
		}

		[Fact]
		public async Task WhenStringContainsWhitespace_ShouldPositionArrowsCorrectly()
		{
			const string actual = "foo\rbAr\nbaz";
			const string expected = "foo\rbar\nbaz";

			StringDifference sut = new(actual, expected, null, Settings);

			await That(sut.IndexOfFirstMismatch(StringDifference.MatchType.Equality)).IsEqualTo(5);
			await That(sut.ToString()).IsEqualTo(
				"""
				differs on line 1 and column 6:
				         ↓ (actual)
				  "foo\rbAr\nbaz"
				  "foo\rbar\nbaz"
				         ↑ (expected suffix)
				""");
		}

		[Fact]
		public async Task WhenStringsDifferInCaseOnly_ShouldDefaultToCaseSensitiveComparison()
		{
			const string actual = "this IS a text that only differs in casing";
			const string expected = "this is a text that only differs in casing";

			StringDifference sut = new(actual, expected, null, Settings);

			await That(sut.IndexOfFirstMismatch(StringDifference.MatchType.Equality)).IsEqualTo(5);
			await That(sut.ToString()).IsEqualTo(
				"""
				differs before index 5:
				        ↓ (actual)
				  "this IS a text that only…"
				  "this is a text that only…"
				        ↑ (expected suffix)
				""");
		}

		[Fact]
		public async Task WhenStringsDifferInCaseOnly_WhenUsingAComparer_ShouldCompareSubstringsWithComparer()
		{
			const string actual = "this IS a text that only differs in casing";
			const string expected = "this is a text that only differs in casing";
			StringComparer comparer = StringComparer.OrdinalIgnoreCase;

			StringDifference sut = new(actual, expected, comparer, Settings);

			await That(sut.IndexOfFirstMismatch(StringDifference.MatchType.Suffix)).IsEqualTo(-1);
			await That(sut.ToString()).IsEqualTo("differs");
		}

		[Theory]
		[InlineData("foo", "bar", 2)]
		[InlineData("foo", "bro", 1)]
		[InlineData("bar", "var", 0)]
		[InlineData("foobar", "bazbar", 2)]
		public async Task WhenTextDiffers_ShouldCalculateIndexOfFirstMismatch(
			string actual, string expected, int expectedIndex)
		{
			StringDifference sut = new(actual, expected, null, Settings);

			await That(sut.IndexOfFirstMismatch(StringDifference.MatchType.Suffix)).IsEqualTo(expectedIndex);
		}

		[Fact]
		public async Task WhenTextHasMultipleLines_ShouldIncludeLineAndColumnNumbers()
		{
			int expectedIndex = 106 + (3 * Environment.NewLine.Length);
			string nl = Environment.NewLine.DisplayWhitespace();
			string nb = new(' ', nl.Length); // newline blanks

			string actual = """
			                @startuml
			                Alice -> Bob : Authentication Request
			                Bob --> Alice : Authentication Response
			                Alice -> Bob : Another authentication Request
			                Alice <-- Bob : Another authentication Response
			                @enduml
			                """;

			string expected = """
			                  @startuml
			                  Alice -> Bob : Authentication Request
			                  Bob --> Alice : Authentication Response
			                  Alice -> Bob : Invalid authentication Request
			                  Alice <-- Bob : Another authentication Response
			                  @enduml
			                  """;

			StringDifference sut = new(actual, expected, null, Settings);

			await That(sut.IndexOfFirstMismatch(StringDifference.MatchType.Suffix)).IsEqualTo(expectedIndex);
			await That(sut.ToString()).IsEqualTo(
				$"""
				 differs on line 4 and column 22:
				                                        {nb}                     ↓ (actual)
				   "…--> Alice : Authentication Response{nl}Alice -> Bob : Another authentication…"
				   "…--> Alice : Authentication Response{nl}Alice -> Bob : Invalid authentication…"
				                                        {nb}                     ↑ (expected suffix)
				 """);
		}

		[Fact]
		public async Task WhenTextIsSame_ShouldSetIndexOfFirstMismatchToNegativeOne()
		{
			const string actual = "this is a text that does not differ";

			StringDifference sut = new(actual, actual, null, Settings);

			await That(sut.IndexOfFirstMismatch(StringDifference.MatchType.Suffix)).IsEqualTo(-1);
			await That(sut.ToString()).IsEqualTo("differs");
		}

		[Fact]
		public async Task WhenUsingACultureAwareComparer_ShouldCompareWithTheCulture()
		{
			StringDifference sut = new(
				"Prefix with an accented cafe\u0301 and MORE text X here",
				"prefix with an accented café and more text Y here",
				StringComparer.InvariantCultureIgnoreCase,
				Settings);

			await That(sut.ToString()).IsEqualTo(
				"""
				differs before index 44:
				                                                    ↓ (actual)
				  "Prefix with an accented cafe\u0301 and MORE text X here"
				        "prefix with an accented café and more text Y here"
				                                                    ↑ (expected suffix)
				""");
		}

		[Fact]
		public async Task WhenWindowEndsInsideLineBreak_ShouldIncludeTheWholeLineBreak()
		{
			StringDifference sut = new("abXccccccccccccc\r\ndd", "abYccccccccccccc\r\ndd", null, Settings);

			await That(sut.ToString()).IsEqualTo(
					"""
					differs on line 1 and column 3:
					     ↓ (actual)
					  "abXccccccccccccc\r\n…"
					  "abYccccccccccccc\r\n…"
					     ↑ (expected suffix)
					""")
				.Because("the window must not end between the \\r and the \\n of a line break");
		}

		[Fact]
		public async Task WhenWindowEndsInsideSurrogatePair_ShouldIncludeTheWholePair()
		{
			StringDifference sut = new("abXccccccccccccc😀dd", "abYccccccccccccc😀dd", null, Settings);

			await That(sut.ToString()).IsEqualTo(
					"""
					differs before index 2:
					     ↓ (actual)
					  "abXccccccccccccc😀…"
					  "abYccccccccccccc😀…"
					     ↑ (expected suffix)
					""")
				.Because("the window must not end between the two halves of a surrogate pair");
		}

		[Fact]
		public async Task WhenWindowStartsInsideLineBreak_ShouldIncludeTheWholeLineBreak()
		{
			StringDifference sut = new("x\r\naaaaaaaaaXtail", "x\r\naaaaaaaaaYtail", null, Settings);
			string result;

			using (Customize.aweXpect.Formatting().MinimumNumberOfCharactersAfterStringDifference.Set(5))
			{
				result = sut.ToString();
			}

			await That(result).IsEqualTo(
					"""
					differs on line 2 and column 10:
					                 ↓ (actual)
					  "…\r\naaaaaaaaaXtail"
					  "…\r\naaaaaaaaaYtail"
					                 ↑ (expected suffix)
					""")
				.Because("the window must not start between the \\r and the \\n of a line break");
		}

		[Fact]
		public async Task WhenWindowStartsInsideSurrogatePair_ShouldIncludeTheWholePair()
		{
			StringDifference sut = new("x😀aaaaaaaaaXtail", "x😀aaaaaaaaaYtail", null, Settings);
			string result;

			using (Customize.aweXpect.Formatting().MinimumNumberOfCharactersAfterStringDifference.Set(5))
			{
				result = sut.ToString();
			}

			await That(result).IsEqualTo(
					"""
					differs before index 12:
					               ↓ (actual)
					  "…😀aaaaaaaaaXtail"
					  "…😀aaaaaaaaaYtail"
					               ↑ (expected suffix)
					""")
				.Because("the window must not start between the two halves of a surrogate pair");
		}

		private static readonly StringDifferenceSettings Settings = new(0, 0)
		{
			MatchType = StringDifference.MatchType.Suffix,
		};
	}

	private sealed class ExecuteOnceComparer : IEqualityComparer<string>
	{
		private bool _wasExecuted;

		public bool Equals(string? x, string? y)
		{
			if (!_wasExecuted)
			{
				_wasExecuted = true;
				return string.Equals(x, y);
			}

			throw new NotSupportedException("Comparer was executed more than once");
		}

		public int GetHashCode(string obj) => obj.GetHashCode();
	}

	private sealed class UpperCaseComparer : IEqualityComparer<string>
	{
		public bool Equals(string? x, string? y)
			=> string.Equals(x?.ToUpperInvariant(), y?.ToUpperInvariant(), StringComparison.Ordinal);

		public int GetHashCode(string obj) => obj.ToUpperInvariant().GetHashCode();
	}
}
