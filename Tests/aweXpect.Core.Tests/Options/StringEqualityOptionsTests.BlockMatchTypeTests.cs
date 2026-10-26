using System.Collections.Generic;
using System.Linq;
using aweXpect.Options;

namespace aweXpect.Core.Tests.Options;

public sealed partial class StringEqualityOptionsTests
{
	public sealed class BlockMatchTypeTests
	{
		[Test]
		public async Task AreConsideredEqual_BothNull_ShouldReturnTrue()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.AsBlock();

			bool result = await sut.AreConsideredEqual(null, null, "expected");

			await That(result).IsTrue();
		}

		[Test]
		[Arguments(null, "foo")]
		[Arguments("foo", null)]
		public async Task AreConsideredEqual_OneNull_ShouldReturnFalse(string? actual, string? expected)
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.AsBlock();

			bool result = await sut.AreConsideredEqual(actual, expected, "expected");

			await That(result).IsFalse();
		}

		[Test]
		[Arguments("a\nb\n", "a\nb", true)]
		[Arguments("a\nb", "a\nb\r\n", true)]
		[Arguments("a\nb\n\n", "a\nb", false)]
		[Arguments("", "", true)]
		[Arguments("", "\n", false)]
		public async Task AreConsideredEqual_ShouldIgnoreASingleTrailingLineTerminator(string actual,
			string expected, bool expectedResult)
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.AsBlock();

			bool result = await sut.AreConsideredEqual(actual, expected, "expected");

			await That(result).IsEqualTo(expectedResult);
		}

		[Test]
		public async Task AreConsideredEqual_WhenActualHasAdditionalLines_ShouldReturnFalse()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.AsBlock();

			bool result = await sut.AreConsideredEqual("class C\n{\n    Foo();\n}", "{\n    Foo();\n}", "expected");

			await That(result).IsFalse();
		}

		[Test]
		public async Task AreConsideredEqual_WhenBlockIsIndentedAsAWhole_ShouldReturnTrue()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.AsBlock();

			bool result = await sut.AreConsideredEqual("    {\n        Foo();\n    }", "{\n    Foo();\n}", "expected");

			await That(result).IsTrue();
		}

		[Test]
		public async Task AsBlock_ShouldNotChangeTheOriginalOptions()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;

			StringEqualityOptions result = sut.AsBlock();

			await That(result).IsNotSameAs(sut);
			await That(sut.ComparesByOrdinalEquality).IsTrue();
		}

		[Test]
		public async Task CountOccurrences_ShouldCountNonOverlappingOccurrences()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.AsBlock();

			int result = await sut.CountOccurrences("a\nb\na\nb\na", "a\nb\na", "expected");

			await That(result).IsEqualTo(1);
		}

		[Test]
		[Arguments("a\nb", "a\nb\n", 1)]
		[Arguments("a\nb\n", "a\nb", 1)]
		[Arguments("x\na\nb", "a\nb\n", 1)]
		[Arguments("a\nb\n\n", "a\nb", 1)]
		[Arguments("a\nb\n  ", "a\nb\n", 1)]
		[Arguments("a\nb", "a\nb\n\n", 0)]
		[Arguments("a\nb\n\n", "a\nb\n\n", 1)]
		[Arguments("", "\n", 0)]
		public async Task CountOccurrences_ShouldIgnoreASingleTrailingLineTerminator(string actual,
			string expected, int expectedCount)
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.AsBlock();

			int result = await sut.CountOccurrences(actual, expected, "expected");

			await That(result).IsEqualTo(expectedCount);
		}

		[Test]
		public async Task CountOccurrences_ShouldNotMatchMidLine()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.AsBlock();

			int result = await sut.CountOccurrences("public int Foo;", "int Foo", "expected");

			await That(result).IsEqualTo(0);
		}

		[Test]
		[Arguments("public int Foo\n{\n    get;\n}", 1)]
		[Arguments("    public int Foo\n    {\n        get;\n    }", 1)]
		[Arguments("\tpublic int Foo\n\t{\n\t    get;\n\t}", 1)]
		[Arguments("class C\n{\n    public int Foo\n    {\n        get;\n    }\n}", 1)]
		[Arguments("    public int Foo\r\n    {\r\n        get;\r\n    }", 1)]
		[Arguments("public int Foo\n{\nget;\n}", 0)]
		[Arguments("public int Foo\n{\n        get;\n}", 0)]
		[Arguments("    public int Foo\n{\n    get;\n}", 0)]
		[Arguments("// public int Foo\n// {\n//     get;\n// }", 0)]
		[Arguments("public int Foo\n{\n    get; set;\n}", 0)]
		[Arguments("public int Foo\n{\n    get;", 0)]
		public async Task CountOccurrences_ShouldRequireTheSameWhiteSpacePrefixOnAllLines(string actual,
			int expectedCount)
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.AsBlock();

			int result = await sut.CountOccurrences(actual, "public int Foo\n{\n    get;\n}", "expected");

			await That(result).IsEqualTo(expectedCount);
		}

		[Test]
		public async Task CountOccurrences_WhenActualLineEndsWithExpectedLine_ShouldNotMatch()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.AsBlock();

			int result = await sut.CountOccurrences("public int Foo;", "int Foo;", "expected");

			await That(result).IsEqualTo(0);
		}

		[Test]
		[Arguments("a\n\nb")]
		[Arguments("  a\n\n  b")]
		[Arguments("  a\n   \n  b")]
		[Arguments("  a\n\t\n  b")]
		public async Task CountOccurrences_WhenBlockContainsBlankLine_ShouldMatchAnyWhiteSpaceOnlyLine(
			string actual)
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.AsBlock();

			int result = await sut.CountOccurrences(actual, "a\n\nb", "expected");

			await That(result).IsEqualTo(1);
		}

		[Test]
		public async Task CountOccurrences_WhenBlockContainsBlankLine_ShouldNotMatchNonBlankLine()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.AsBlock();

			int result = await sut.CountOccurrences("a\nx\nb", "a\n\nb", "expected");

			await That(result).IsEqualTo(0);
		}

		[Test]
		public async Task CountOccurrences_WhenCaseIsIgnored_ShouldIgnoreCase()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.AsBlock().IgnoringCase();

			int result = await sut.CountOccurrences("  FOO\n  bar", "foo\nBAR", "expected");

			await That(result).IsEqualTo(1);
		}

		[Test]
		public async Task CountOccurrences_WhenComparerIsUsed_ShouldUseComparer()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.AsBlock().Using(StringComparer.OrdinalIgnoreCase);

			int result = await sut.CountOccurrences("  FOO\n  bar", "foo\nBAR", "expected");

			await That(result).IsEqualTo(1);
		}

		[Test]
		public async Task CountOccurrences_WhenOccurrencesAreIndentedDifferently_ShouldCountAll()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.AsBlock();

			int result = await sut.CountOccurrences("\ta\n\tb\nx\n  a\n  b\na\nb", "a\nb", "expected");

			await That(result).IsEqualTo(3);
		}

		[Test]
		public async Task GetExpectation_ShouldIncludeAsBlock()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.AsBlock();

			string result = sut.GetExpectation("foo", ExpectationGrammars.Active);

			await That(result).IsEqualTo("matches \"foo\" as block");
		}

		[Test]
		public async Task GetExpectation_WhenNegatedAndPassive_ShouldSayNotMatching()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.AsBlock();

			string result = sut.GetExpectation("foo", ExpectationGrammars.Negated);

			await That(result).IsEqualTo("not matching \"foo\" as block");
		}

		[Test]
		public async Task GetExtendedFailure_Null_ShouldReturnItWasNull()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.AsBlock();

			string result = sut.GetExtendedFailure("it", ExpectationGrammars.None, null, "foo", MatchResult.NoMatch);

			await That(result).IsEqualTo("it was <null>");
		}

		[Test]
		public async Task GetExtendedFailure_WhenABlankLineIsExpected_ShouldNameTheLineThatIsNotBlank()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.AsBlock();

			string result = sut.GetExtendedFailure("it", ExpectationGrammars.None, "a\n  x\nb", "a\n\nb", MatchResult.NoMatch);

			await That(result).IsEqualTo("it was \"a\\n  x\\nb\", which has \"  x\" on line 2 instead of a blank line");
		}

		[Test]
		public async Task GetExtendedFailure_WhenALineDiffers_ShouldShowTheDifferenceOnThatLine()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.AsBlock();

			string result = sut.GetExtendedFailure("it", ExpectationGrammars.None, "  a\n  bxd\n  c", "a\nbyd\nc", MatchResult.NoMatch);

			await That(result).IsEqualTo("""
			                             it was "  a\n  bxd\n  c", which differs on line 2 and column 4:
			                                        ↓ (actual)
			                               "  a\n  bxd\n  c"
			                               "  a\n  byd\n  c"
			                                        ↑ (expected)
			                             """);
		}

		[Test]
		public async Task GetExtendedFailure_WhenALineDiffersAndTheLineCountDiffers_ShouldShowTheDifferenceOnThatLine()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.AsBlock();

			string result = sut.GetExtendedFailure("it", ExpectationGrammars.None, "a\nx\nb", "a\nb", MatchResult.NoMatch);

			await That(result).IsEqualTo("""
			                             it was "a\nx\nb", which differs on line 2 and column 1:
			                                   ↓ (actual)
			                               "a\nx\nb"
			                               "a\nb"
			                                   ↑ (expected)
			                             """)
				.Because("the first line that does not match also locates the additional line");
		}

		[Test]
		[Arguments("    a\n  b", "a\nb", "it was \"    a\\n  b\", which has line 2 indented by \"  \" instead of \"    \"")]
		[Arguments("\ta\n    b", "a\nb", "it was \"\\ta\\n    b\", which has line 2 indented by \"    \" instead of \"\\t\"")]
		[Arguments("  a\n  b", "a\n  b", "it was \"  a\\n  b\", which has line 2 indented by \"  \" instead of \"    \"")]
		[Arguments(" a\nb", "  a\nb", "it was \" a\\nb\", which has line 1 indented by \" \" instead of \"  \"")]
		public async Task GetExtendedFailure_WhenALineIsIndentedDifferently_ShouldNameItsIndentation(string actual,
			string expected, string expectedMessage)
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.AsBlock();

			string result = sut.GetExtendedFailure("it", ExpectationGrammars.None, actual, expected, MatchResult.NoMatch);

			await That(result).IsEqualTo(expectedMessage);
		}

		[Test]
		public async Task GetExtendedFailure_WhenCaseIsIgnored_ShouldOnlyShowOtherDifferences()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.AsBlock().IgnoringCase();

			string result = sut.GetExtendedFailure("it", ExpectationGrammars.None, "  FOO\n  bar\n  BAZ", "foo\nBAR\nbax", MatchResult.NoMatch);

			await That(result).IsEqualTo("""
			                             it was "  FOO\n  bar\n  BAZ", which differs on line 3 and column 5:
			                                                  ↓ (actual)
			                               "… FOO\n  bar\n  BAZ"
			                               "… FOO\n  bar\n  bax"
			                                                  ↑ (expected)
			                             """);
		}

		[Test]
		public async Task GetExtendedFailure_WhenComparerIsUsed_ShouldUseTheComparer()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.AsBlock().Using(new IgnoreCaseComparer());

			string result = sut.GetExtendedFailure("it", ExpectationGrammars.None, "  FOO\nBAR", "foo\nbar", MatchResult.NoMatch);

			await That(result).IsEqualTo("it was \"  FOO\\nBAR\", which has line 2 indented by \"\" instead of \"  \"");
		}

		[Test]
		public async Task GetExtendedFailure_WhenComparerThrows_ShouldReturnTheSubject()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.AsBlock().Using(new ThrowingComparer(new NotSupportedException("foo")));

			string result = sut.GetExtendedFailure("it", ExpectationGrammars.None, "a\nb", "a\nc", MatchResult.NoMatch);

			await That(result).IsEqualTo("it was \"a\\nb\"");
		}

		[Test]
		public async Task GetExtendedFailure_WhenComparerThrowsForTheWholeText_ShouldNameTheLine()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.AsBlock().Using(new ThrowingForMultipleLinesComparer());

			string result = sut.GetExtendedFailure("it", ExpectationGrammars.None, "a\nx", "a\nb", MatchResult.NoMatch);

			await That(result).IsEqualTo("it was \"a\\nx\", which differs on line 2")
				.Because("the difference is located in the whole text, which the comparer cannot compare");
		}

		[Test]
		public async Task GetExtendedFailure_WhenExpectedIsEmpty_ShouldNameTheNumberOfLines()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.AsBlock();

			string result = sut.GetExtendedFailure("it", ExpectationGrammars.None, "a", "", MatchResult.NoMatch);

			await That(result).IsEqualTo("it was \"a\", which has 1 line instead of 0");
		}

		[Test]
		public async Task GetExtendedFailure_WhenSubjectHasAdditionalLines_ShouldNameTheNumberOfLines()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.AsBlock();

			string result = sut.GetExtendedFailure("it", ExpectationGrammars.None, "foo\nbar", "foo", MatchResult.NoMatch);

			await That(result).IsEqualTo("it was \"foo\\nbar\", which has 2 lines instead of 1");
		}

		[Test]
		public async Task GetExtendedFailure_WhenSubjectHasFewerLines_ShouldNameTheNumberOfLines()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.AsBlock();

			string result = sut.GetExtendedFailure("it", ExpectationGrammars.None, "a\nb\n", "a\nb\nc", MatchResult.NoMatch);

			await That(result).IsEqualTo("it was \"a\\nb\\n\", which has 2 lines instead of 3")
				.Because("a single trailing line terminator does not start a new line");
		}

		[Test]
		public async Task GetExtendedFailure_WhenSubjectIsASingleLine_ShouldShowTheIndex()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.AsBlock();

			string result = sut.GetExtendedFailure("it", ExpectationGrammars.None, "fob", "foo", MatchResult.NoMatch);

			await That(result).IsEqualTo("""
			                             it was "fob", which differs at index 2:
			                                  ↓ (actual)
			                               "fob"
			                               "foo"
			                                  ↑ (expected)
			                             """);
		}

		[Test]
		public async Task GetExtendedFailure_WhenSubjectIsEmpty_ShouldReturnTheSubject()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.AsBlock();

			string result = sut.GetExtendedFailure("it", ExpectationGrammars.None, "", "a", MatchResult.NoMatch);

			await That(result).IsEqualTo("it was \"\"");
		}

		[Test]
		[Arguments("a\r\nb\r\nx")]
		[Arguments("a\rb\rx")]
		[Arguments("a\nb\r\nx\r\n")]
		public async Task GetExtendedFailure_WhenSubjectUsesOtherLineTerminators_ShouldNumberTheLinesOfTheSubject(
			string actual)
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.AsBlock();

			string result = sut.GetExtendedFailure("it", ExpectationGrammars.None, actual, "a\nb\nc", MatchResult.NoMatch);

			await That(result).StartsWith($"it was {Formatter.Format(actual)}, which differs on line 3 and column 1:");
		}

		[Test]
		public async Task GetExtendedFailure_WhenTheBlockStartsWithBlankLines_ShouldNumberTheLinesOfTheSubject()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.AsBlock();

			string result = sut.GetExtendedFailure("it", ExpectationGrammars.None, "\n \n  a\n  x", "\n\na\nb", MatchResult.NoMatch);

			await That(result).StartsWith("it was \"\\n \\n  a\\n  x\", which differs on line 4 and column 3:");
		}

		[Test]
		public async Task GetOccurrenceDetails_WhenABlankLineIsExpected_ShouldNameTheLineThatIsNotBlank()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.AsBlock();

			string result = sut.GetOccurrenceDetails("x\na\ny\nb", "a\n\nb", 0);

			await That(result).IsEqualTo(", and the closest match at lines 2-4 has \"y\" on line 3 instead of a blank line");
		}

		[Test]
		public async Task GetOccurrenceDetails_WhenALineOfTheClosestMatchDiffers_ShouldShowTheDifference()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.AsBlock();

			string result = sut.GetOccurrenceDetails("class C\n{\n    int x;\n    int y;\n}",
				"{\n    int x;\n    int z;\n}", 0);

			await That(result).IsEqualTo("""
			                             , and the closest match at lines 2-5 differs on line 4 and column 9:
			                                                 ↓ (actual)
			                               "…int x;\n    int y;\n}"
			                               "…int x;\n    int z;\n}"
			                                                 ↑ (expected)
			                             """);
		}

		[Test]
		public async Task GetOccurrenceDetails_WhenBlockConsistsOfBlankLines_ShouldReturnEmpty()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.AsBlock();

			string result = sut.GetOccurrenceDetails("a\n b", "\n \n", 0);

			await That(result).IsEmpty()
				.Because("a blank line matches any blank line, so finding its text inside of lines explains nothing");
		}

		[Test]
		public async Task GetOccurrenceDetails_WhenBlockIsLongerThanTheSubjectAndALineDiffers_ShouldShowTheDifference()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.AsBlock();

			string result = sut.GetOccurrenceDetails("{\n  a;\n}", "{\n  a;\n  b;\n}", 0);

			await That(result).StartsWith(", which differs on line 3 and column 1:");
		}

		[Test]
		[Arguments("  a\n  b", "a\nb\nc", ", which has 2 lines instead of at least 3")]
		[Arguments("a", "a\nb", ", which has 1 line instead of at least 2")]
		[Arguments("  a\nb", "a\nb\nc", ", which has line 2 indented by \"\" instead of \"  \"")]
		[Arguments("a\nb", "\nb\nc", ", which has \"a\" on line 1 instead of a blank line")]
		public async Task GetOccurrenceDetails_WhenBlockIsLongerThanTheSubject_ShouldCompareFromTheFirstLine(
			string actual, string expected, string expectedDetails)
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.AsBlock();

			string result = sut.GetOccurrenceDetails(actual, expected, 0);

			await That(result).IsEqualTo(expectedDetails);
		}

		[Test]
		public async Task GetOccurrenceDetails_WhenCountIsPositiveWithoutAnOccurrence_ShouldReturnEmpty()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.AsBlock();

			string result = sut.GetOccurrenceDetails("x\ny", "a", 1);

			await That(result).IsEmpty();
		}

		[Test]
		[Arguments("x\n  a\n  b", "a\nb", 1, " at lines 2-3")]
		[Arguments("a\nx\na", "a", 1, " at line 1")]
		[Arguments("a\nx\na", "a", 2, " at lines 1 and 3")]
		[Arguments("a\nb\na\nb\na\nb", "a\nb", 3, " at lines 1-2, 3-4 and 5-6")]
		[Arguments("a\r\nb\r\n\r\n  a\n  b", "a\nb\n", 2, " at lines 1-2 and 4-5")]
		public async Task GetOccurrenceDetails_WhenBlockOccurs_ShouldNameItsLines(string actual, string expected,
			int count, string expectedDetails)
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.AsBlock();

			string result = sut.GetOccurrenceDetails(actual, expected, count);

			await That(result).IsEqualTo(expectedDetails);
		}

		[Test]
		[Arguments("a\nb\na\nb", "a\nb", 1, " at lines 1-2")]
		[Arguments("a\na\na\na\na\na\na\na\na\na\na\na", "a", 12, " at lines 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, \u2026")]
		[Arguments("a\na\na\na\na\na\na\na\na\na", "a", 10, " at lines 1, 2, 3, 4, 5, 6, 7, 8, 9 and 10")]
		public async Task GetOccurrenceDetails_WhenBlockOccursMoreOften_ShouldOnlyNameTheCountedOccurrences(
			string actual, string expected, int count, string expectedDetails)
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.AsBlock();

			string result = sut.GetOccurrenceDetails(actual, expected, count);

			await That(result).IsEqualTo(expectedDetails)
				.Because("at most 10 occurrences are listed");
		}

		[Test]
		[Arguments("public int Foo;", "int Foo", ", only as part of line 1")]
		[Arguments("x\nint a\nb;", "a\nb", ", only as part of lines 2-3")]
		[Arguments("x\nint a\nb;", "a\nb\n", ", only as part of lines 2-3")]
		public async Task GetOccurrenceDetails_WhenBlockOccursOnlyInsideOfLines_ShouldNameTheLines(string actual,
			string expected, string expectedDetails)
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.AsBlock();

			string result = sut.GetOccurrenceDetails(actual, expected, 0);

			await That(result).IsEqualTo(expectedDetails);
		}

		[Test]
		[Arguments("  FOO\nbar", "foo\nBAR",
			", and the closest match at lines 1-2 has line 2 indented by \"\" instead of \"  \"")]
		[Arguments("public INT FOO;", "int foo", ", only as part of line 1")]
		public async Task GetOccurrenceDetails_WhenCaseIsIgnored_ShouldIgnoreCase(string actual, string expected,
			string expectedDetails)
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.AsBlock().IgnoringCase();

			string result = sut.GetOccurrenceDetails(actual, expected, 0);

			await That(result).IsEqualTo(expectedDetails);
		}

		[Test]
		public async Task GetOccurrenceDetails_WhenComparerThrows_ShouldReturnEmpty()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.AsBlock().Using(new ThrowingComparer(new NotSupportedException("foo")));

			string result = sut.GetOccurrenceDetails("    a\nb", "a\nb", 0);

			await That(result).IsEmpty();
		}

		[Test]
		public async Task GetOccurrenceDetails_WhenCultureAwareComparerIsUsed_ShouldSearchInsideOfLines()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.AsBlock().Using(StringComparer.InvariantCultureIgnoreCase);

			string result = sut.GetOccurrenceDetails("public INT FOO;", "int foo", 0);

			await That(result).IsEqualTo(", only as part of line 1");
		}

		[Test]
		public async Task GetOccurrenceDetails_WhenCustomComparerIsUsed_ShouldNotSearchInsideOfLines()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.AsBlock().Using(new IgnoreCaseComparer());

			string result = sut.GetOccurrenceDetails("public INT FOO;", "int foo", 0);

			await That(result).IsEmpty()
				.Because("a custom comparer can only compare whole values");
		}

		[Test]
		public async Task GetOccurrenceDetails_WhenCustomComparerIsUsed_ShouldUseItForTheClosestMatch()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.AsBlock().Using(new IgnoreCaseComparer());

			string result = sut.GetOccurrenceDetails("x\n  FOO\nbar", "foo\nBAR", 0);

			await That(result)
				.IsEqualTo(", and the closest match at lines 2-3 has line 3 indented by \"\" instead of \"  \"");
		}

		[Test]
		public async Task GetOccurrenceDetails_WhenExpectedIsEmpty_ShouldReturnEmpty()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.AsBlock();

			string result = sut.GetOccurrenceDetails("a", "", 0);

			await That(result).IsEmpty();
		}

		[Test]
		public async Task GetOccurrenceDetails_WhenIndentationAndInsideOfLinesBothApply_ShouldPreferTheIndentation()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.AsBlock();

			string result = sut.GetOccurrenceDetails("xa\nby\n  a\nb", "a\nb", 0);

			await That(result).IsEqualTo(", and the closest match at lines 3-4 has line 4 indented by \"\" instead of \"  \"");
		}

		[Test]
		public async Task GetOccurrenceDetails_WhenInsideOfLinesAndAClosestMatchBothApply_ShouldPreferInsideOfLines()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.AsBlock();

			string result = sut.GetOccurrenceDetails("a\nc\nxa\nby", "a\nb", 0);

			await That(result).IsEqualTo(", only as part of lines 3-4");
		}

		[Test]
		public async Task GetOccurrenceDetails_WhenNoLineMatches_ShouldReturnEmpty()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.AsBlock();

			string result = sut.GetOccurrenceDetails("x\ny\nz", "a\nb", 0);

			await That(result).IsEmpty();
		}

		[Test]
		public async Task GetOccurrenceDetails_WhenNotMatchedAsBlock_ShouldReturnEmpty()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;

			string result = sut.GetOccurrenceDetails("    a\nb", "a\nb", 0);

			await That(result).IsEmpty();
		}

		[Test]
		public async Task GetOccurrenceDetails_WhenOnlyBlankLinesMatch_ShouldReturnEmpty()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.AsBlock();

			string result = sut.GetOccurrenceDetails("x\n\ny", "a\n\nb", 0);

			await That(result).IsEmpty()
				.Because("a blank line matches any blank line, so it does not make a match close");
		}

		[Test]
		[Arguments("    a\nb", "a\nb",
			", and the closest match at lines 1-2 has line 2 indented by \"\" instead of \"    \"")]
		[Arguments("\ta\n    b", "a\nb",
			", and the closest match at lines 1-2 has line 2 indented by \"    \" instead of \"\\t\"")]
		[Arguments("x\r\n    a\r\nb", "a\nb",
			", and the closest match at lines 2-3 has line 3 indented by \"\" instead of \"    \"")]
		[Arguments(" a\n b", "  a\nb",
			", and the closest match at lines 1-2 has line 1 indented by \" \" instead of \"  \"")]
		public async Task GetOccurrenceDetails_WhenOnlyTheIndentationDiffers_ShouldNameTheIndentation(string actual,
			string expected, string expectedDetails)
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.AsBlock();

			string result = sut.GetOccurrenceDetails(actual, expected, 0);

			await That(result).IsEqualTo(expectedDetails);
		}

		[Test]
		public async Task GetOccurrenceDetails_WhenSeveralMatchesAreEquallyClose_ShouldPickTheFirst()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.AsBlock();

			string result = sut.GetOccurrenceDetails("x\na\ny\na\nz", "a\nb", 0);

			await That(result).StartsWith(", and the closest match at lines 2-3 differs on line 3 and column 1:");
		}

		[Test]
		public async Task GetOccurrenceDetails_WhenSubjectIsEmpty_ShouldReturnEmpty()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.AsBlock();

			string result = sut.GetOccurrenceDetails("", "a", 0);

			await That(result).IsEmpty();
		}

		[Test]
		[Arguments(49_998, false)]
		[Arguments(49_999, true)]
		public async Task GetOccurrenceDetails_WhenSubjectIsTooLong_ShouldNotSearchForAClosestMatch(
			int additionalLines, bool expectEmpty)
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.AsBlock();
			string actual = "a\nc" + string.Concat(Enumerable.Repeat("\nx", additionalLines));

			string result = sut.GetOccurrenceDetails(actual, "a\nb", 0);

			if (expectEmpty)
			{
				await That(result).IsEmpty()
					.Because("at most 100000 lines of the subject times the lines of the block are compared");
			}
			else
			{
				await That(result).StartsWith(", and the closest match at lines 1-2 differs on line 2 and column 1:");
			}
		}

		[Test]
		[Arguments(49_998, ", and the closest match at lines 1-2 has line 2 indented by \"\" instead of \"    \"")]
		[Arguments(49_999, ", only as part of lines 1-2")]
		public async Task GetOccurrenceDetails_WhenSubjectIsTooLong_ShouldStillSearchInsideOfLines(
			int additionalLines, string expectedDetails)
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.AsBlock();
			string actual = "    a\nb" + string.Concat(Enumerable.Repeat("\nx", additionalLines));

			string result = sut.GetOccurrenceDetails(actual, "a\nb", 0);

			await That(result).IsEqualTo(expectedDetails)
				.Because("only the comparison of every window with the block is limited");
		}

		[Test]
		public async Task ToString_ShouldReturnAsBlock()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.AsBlock();

			string result = sut.ToString();

			await That(result).IsEqualTo(" as block");
		}

		[Test]
		public async Task ToString_WhenCaseIsIgnored_ShouldReturnAsBlockIgnoringCase()
		{
			StringEqualityOptions sut = StringEqualityOptions.Default;
			sut = sut.AsBlock().IgnoringCase();

			string result = sut.ToString();

			await That(result).IsEqualTo(" as block ignoring case");
		}

		private sealed class IgnoreCaseComparer : IEqualityComparer<string>
		{
			public bool Equals(string? x, string? y) => StringComparer.OrdinalIgnoreCase.Equals(x, y);

			public int GetHashCode(string obj) => StringComparer.OrdinalIgnoreCase.GetHashCode(obj);
		}

		private sealed class ThrowingComparer(Exception exception) : IEqualityComparer<string>
		{
			public bool Equals(string? x, string? y) => throw exception;

			public int GetHashCode(string obj) => throw exception;
		}

		private sealed class ThrowingForMultipleLinesComparer : IEqualityComparer<string>
		{
			public bool Equals(string? x, string? y)
				=> x?.IndexOf('\n') >= 0 || y?.IndexOf('\n') >= 0
					? throw new NotSupportedException("only single lines are supported")
					: x == y;

			public int GetHashCode(string obj) => obj.GetHashCode();
		}
	}
}
