using System.Text.RegularExpressions;

namespace aweXpect.Tests;

public sealed partial class ThatString
{
	public partial class Contains
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenActualIsNull_ShouldFail()
			{
				string? subject = null;

				async Task Act()
					=> await That(subject).Contains("foo");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains "foo" at least once,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenExpectedIsEmpty_ShouldThrowArgumentException()
			{
				string subject = "some text";
				string expected = "";

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).Throws<ArgumentException>()
					.WithMessage("The 'expected' string cannot be empty.").AsPrefix().And
					.WithParamName("expected");
			}

			[Fact]
			public async Task WhenExpectedIsNull_ShouldThrowArgumentNullException()
			{
				string subject = "some text";
				string? expected = null;

				async Task Act()
					=> await That(subject).Contains(expected!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' value cannot be null.").AsPrefix();
			}

			[Fact]
			public async Task WhenExpectedStringIsContained_ShouldSucceed()
			{
				string subject = "some text";
				string expected = "me";

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenExpectedStringIsNotContained_ShouldFail()
			{
				string subject = "some text";
				string expected = "not";

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains "not" at least once,
					             but it did not contain "not" in "some text"
					             """);
			}
		}

		public sealed class AsBlockTests
		{
			[Fact]
			public async Task ShouldIncludeSettingInExpectationText()
			{
				string subject = "foo";
				string expected = "bar";

				async Task Act()
					=> await That(subject).Contains(expected).AsBlock();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains "bar" as block at least once,
					             but it did not contain "bar" in "foo"
					             """);
			}

			[Fact]
			public async Task WhenBlockIsIndentedAsAWhole_ShouldSucceed()
			{
				string subject = """
				                 public class Foo
				                 {
				                     public int Bar
				                     {
				                         get;
				                     }
				                 }
				                 """;
				string expected = """
				                  public int Bar
				                  {
				                      get;
				                  }
				                  """;

				async Task Act()
					=> await That(subject).Contains(expected).AsBlock();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenBlockOccursWithDifferentIndentations_ShouldCountAllOccurrences()
			{
				string subject = "  a\n  b\nx\na\nb";
				string expected = "a\nb";

				async Task Act()
					=> await That(subject).Contains(expected).AsBlock().Twice();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenCombinedWithIgnoringCase_ShouldIgnoreCase()
			{
				string subject = "  FOO\n  bar";
				string expected = "foo\nBAR";

				async Task Act()
					=> await That(subject).Contains(expected).AsBlock().IgnoringCase();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenExpectedEndsWithNewline_ShouldIgnoreTheTrailingLineTerminator()
			{
				string subject = "x\n  a\n  b";
				string expected = "a\nb\n";

				async Task Act()
					=> await That(subject).Contains(expected).AsBlock();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenExpectedMatchesMidLine_ShouldFail()
			{
				string subject = "public int Foo;";
				string expected = "int Foo";

				async Task Act()
					=> await That(subject).Contains(expected).AsBlock();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains "int Foo" as block at least once,
					             but it did not contain "int Foo" in "public int Foo;"
					             """);
			}

			[Fact]
			public async Task WhenLinesAreIndentedDifferently_ShouldFail()
			{
				string subject = "    a\nb";
				string expected = "a\nb";

				async Task Act()
					=> await That(subject).Contains(expected).AsBlock();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              contains "a\nb" as block at least once,
					              but it did not contain "a\nb" in "    a\nb"
					              """);
			}

			[Fact]
			public async Task WhenSubjectUsesADifferentNewlineStyle_ShouldSucceed()
			{
				string subject = "  foo\r\n  bar";
				string expected = "foo\nbar";

				async Task Act()
					=> await That(subject).Contains(expected).AsBlock();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenUsedWithDoesNotContain_ShouldFailWhenBlockIsContained()
			{
				string subject = "  a\n  b";
				string expected = "a\nb";

				async Task Act()
					=> await That(subject).DoesNotContain(expected).AsBlock();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              does not contain "a\nb" as block,
					              but it contained "a\nb" once in "  a\n  b"
					              """);
			}
		}

		public sealed class AsPrefixTests
		{
			[Fact]
			public async Task WhenExpectedIsEmptyAfterTheIndentationIsIgnored_ShouldThrowArgumentException()
			{
				string subject = "some text";
				string expected = " ";

				async Task Act()
					=> await That(subject).Contains(expected).AsPrefix().IgnoringIndentation();

				await That(Act).Throws<ArgumentException>()
					.WithMessage("The 'expected' prefix cannot be empty.").AsPrefix().And
					.WithParamName("expected")
					.Because("the counted prefix is the normalized one, which every subject starts with");
			}
		}

		public sealed class AsRegexTests
		{
			[Fact]
			public async Task ShouldIncludeSettingInExpectationText()
			{
				string subject = "foo";
				string expected = "b.r";

				async Task Act()
					=> await That(subject).Contains(expected).AsRegex();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains "b.r" as regex at least once,
					             but it did not contain "b.r" in "foo"
					             """);
			}

			[Theory]
			[InlineData("IxI", 2)]
			[InlineData("İxİ", 0)]
			public async Task WhenCombinedWithIgnoringCase_ShouldCountOccurrencesIndependentOfTheCurrentCulture(
				string subject, int expectedCount)
			{
				using CultureOverride _ = new("tr-TR");

				async Task Act()
					=> await That(subject).Contains("i").AsRegex().IgnoringCase().Exactly(expectedCount);

				await That(Act).DoesNotThrow()
					.Because("the dotted and dotless Turkish 'I' must not change how often the pattern occurs");
			}

			[Fact]
			public async Task WhenCombinedWithIgnoringCase_ShouldIgnoreCase()
			{
				string subject = "AXXXB";
				string expected = "a.*b";

				async Task Act()
					=> await That(subject).Contains(expected).AsRegex().IgnoringCase();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMatchIsLongerThanThePattern_ShouldSucceed()
			{
				string subject = "axxxb";
				string expected = "a.*b";

				async Task Act()
					=> await That(subject).Contains(expected).AsRegex();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenOptionsContainMultiline_ShouldCountTheMatchesPerLine()
			{
				string subject = "a\nb\nb";
				string expected = "^b";

				async Task Act()
					=> await That(subject).Contains(expected).AsRegex(RegexOptions.Multiline).Twice();

				await That(Act).DoesNotThrow()
					.Because("the given options also apply when counting the occurrences");
			}

			[Fact]
			public async Task WhenPatternIsAnchoredToALineOfTheSubject_ShouldNotCountIt()
			{
				string subject = "a\nb";
				string expected = "^b";

				async Task Act()
					=> await That(subject).Contains(expected).AsRegex().Never();

				await That(Act).DoesNotThrow()
					.Because("'^' binds to the start of the complete subject, which does not start with 'b'");
			}

			[Fact]
			public async Task WhenPatternIsEmptyAfterTheIndentationIsIgnored_ShouldThrowArgumentException()
			{
				string subject = "some text";
				string expected = " ";

				async Task Act()
					=> await That(subject).Contains(expected).AsRegex().IgnoringIndentation();

				await That(Act).Throws<ArgumentException>()
					.WithMessage("The 'expected' regex pattern cannot be empty.").AsPrefix().And
					.WithParamName("expected")
					.Because("the pattern that is counted is the normalized one, which matches every subject");
			}

			[Fact]
			public async Task WhenPatternMatchesTheEmptyString_ShouldNotCountEmptyMatches()
			{
				string subject = "bbb";
				string expected = "a*";

				async Task Act()
					=> await That(subject).Contains(expected).AsRegex();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains "a*" as regex at least once,
					             but it did not contain "a*" in "bbb"
					             """)
					.Because("an empty match does not cover any occurrence");
			}

			[Fact]
			public async Task WhenUsedWithAtLeast_ShouldCountAllMatches()
			{
				string subject = "abcabc";
				string expected = "[a-c]{3}";

				async Task Act()
					=> await That(subject).Contains(expected).AsRegex().AtLeast(2);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenUsedWithAtMost_ShouldCountAllMatches()
			{
				string subject = "abcabc";
				string expected = "[a-c]{3}";

				async Task Act()
					=> await That(subject).Contains(expected).AsRegex().AtMost(2);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenUsedWithExactly_ShouldCountAGreedyMatchOnce()
			{
				string subject = "aaaa";
				string expected = "a+";

				async Task Act()
					=> await That(subject).Contains(expected).AsRegex().Exactly(1);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenUsedWithExactly_ShouldCountNonOverlappingMatches()
			{
				string subject = "abcabc";
				string expected = "[a-c]{3}";

				async Task Act()
					=> await That(subject).Contains(expected).AsRegex().Exactly(2);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenUsedWithNever_ShouldSucceedWhenPatternDoesNotMatch()
			{
				string subject = "xyz";
				string expected = "a.*b";

				async Task Act()
					=> await That(subject).Contains(expected).AsRegex().Never();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenUsedWithOnce_ShouldCountTheSingleMatch()
			{
				string subject = "axxxb";
				string expected = "a.*b";

				async Task Act()
					=> await That(subject).Contains(expected).AsRegex().Once();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenUsedWithTwice_ShouldFailWithTheRealCount()
			{
				string subject = "axxxb";
				string expected = "a.*b";

				async Task Act()
					=> await That(subject).Contains(expected).AsRegex().Twice();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains "a.*b" as regex exactly twice,
					             but it contained "a.*b" once in "axxxb"
					             """);
			}
		}

		public sealed class AsWildcardTests
		{
			[Fact]
			public async Task ShouldIncludeSettingInExpectationText()
			{
				string subject = "foo";
				string expected = "b?r";

				async Task Act()
					=> await That(subject).Contains(expected).AsWildcard();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains "b?r" as wildcard at least once,
					             but it did not contain "b?r" in "foo"
					             """);
			}

			[Fact]
			public async Task WhenAsteriskSpansNewlines_ShouldCountTheGreedyMatchOnce()
			{
				string subject = "a\nxb\nayb";
				string expected = "a*b";

				async Task Act()
					=> await That(subject).Contains(expected).AsWildcard().Once();

				await That(Act).DoesNotThrow()
					.Because("'*' matches across newlines and is matched greedily");
			}

			[Theory]
			[InlineData("IxI", 2)]
			[InlineData("İxİ", 0)]
			public async Task WhenCombinedWithIgnoringCase_ShouldCountOccurrencesIndependentOfTheCurrentCulture(
				string subject, int expectedCount)
			{
				using CultureOverride _ = new("tr-TR");

				async Task Act()
					=> await That(subject).Contains("i").AsWildcard().IgnoringCase().Exactly(expectedCount);

				await That(Act).DoesNotThrow()
					.Because("the dotted and dotless Turkish 'I' must not change how often the pattern occurs");
			}

			[Fact]
			public async Task WhenCombinedWithIgnoringCase_ShouldIgnoreCase()
			{
				string subject = "AXXB";
				string expected = "a*b";

				async Task Act()
					=> await That(subject).Contains(expected).AsWildcard().IgnoringCase();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMatchIsLongerThanThePattern_ShouldSucceed()
			{
				string subject = "axxb";
				string expected = "a*b";

				async Task Act()
					=> await That(subject).Contains(expected).AsWildcard();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenPatternMatchesTheEmptyString_ShouldNotCountEmptyMatches()
			{
				string subject = "";
				string expected = "*";

				async Task Act()
					=> await That(subject).Contains(expected).AsWildcard();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains "*" as wildcard at least once,
					             but it did not contain "*" in ""
					             """)
					.Because("an empty match does not cover any occurrence");
			}

			[Fact]
			public async Task WhenQuestionMarkMatchesANewline_ShouldCountAllMatches()
			{
				string subject = "a\nb a\nb";
				string expected = "a?b";

				async Task Act()
					=> await That(subject).Contains(expected).AsWildcard().Twice();

				await That(Act).DoesNotThrow()
					.Because("'?' matches the newline of each occurrence");
			}

			[Fact]
			public async Task WhenSubjectHasMoreLinesThanThePattern_ShouldStillCountTheOccurrence()
			{
				string subject = "xyz\nabc\nqqq";
				string expected = "abc";

				async Task Act()
					=> await That(subject).Contains(expected).AsWildcard().Once();

				await That(Act).DoesNotThrow()
					.Because("counting searches for the pattern anywhere, unlike the anchored equality check");
			}

			[Fact]
			public async Task WhenUsedWithOnce_ShouldCountAGreedyMatchOnce()
			{
				string subject = "axxb ayb";
				string expected = "a*b";

				async Task Act()
					=> await That(subject).Contains(expected).AsWildcard().Once();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenUsedWithTwice_ShouldCountAllMatches()
			{
				string subject = "axb ayb";
				string expected = "a?b";

				async Task Act()
					=> await That(subject).Contains(expected).AsWildcard().Twice();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class IgnoringCaseTests
		{
			[Fact]
			public async Task ShouldIncludeSettingInExpectationText()
			{
				string subject =
					"In this text in between the word an investigator should find the word 'IN' multiple times.";
				string expected = "in";

				async Task Act()
					=> await That(subject).Contains(expected).AtLeast(7).IgnoringCase();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains "in" ignoring case at least 7 times,
					             but it contained "in" 5 times in "In this text in between the word an investigator should find the word 'IN' multiple times."
					             """);
			}

			[Fact]
			public async Task
				WhenExpectedStringOccursEnoughTimesCaseInsensitive_ShouldSucceed()
			{
				string subject =
					"In this text in between the word an investigator should find the word 'IN' multiple times.";
				string expected = "in";

				async Task Act()
					=> await That(subject).Contains(expected).AtLeast(5).IgnoringCase();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class IgnoringIndentationTests
		{
			[Fact]
			public async Task ShouldIncludeSettingInExpectationText()
			{
				string subject = "foo";
				string expected = "bar";

				async Task Act()
					=> await That(subject).Contains(expected).IgnoringIndentation();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains "bar" ignoring indentation at least once,
					             but it did not contain "bar" in "foo"
					             """);
			}

			[Fact]
			public async Task WhenCombinedWithIgnoringCase_ShouldIgnoreBoth()
			{
				string subject = "foo\n    BAR";
				string expected = "foo\nbar";

				async Task Act()
					=> await That(subject).Contains(expected).IgnoringIndentation().IgnoringCase();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSnippetIsIndentedDifferently_ShouldSucceed()
			{
				string subject = """
				                 public class Foo
				                 {
				                     public int Bar
				                     {
				                         get;
				                     }
				                 }
				                 """;
				string expected = """
				                  public int Bar
				                  {
				                      get;
				                  }
				                  """;

				async Task Act()
					=> await That(subject).Contains(expected).IgnoringIndentation();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSnippetOccursWithDifferentIndentations_ShouldCountAllOccurrences()
			{
				string subject = "  a\n  b\nx\na\nb";
				string expected = "a\nb";

				async Task Act()
					=> await That(subject).Contains(expected).Twice().IgnoringIndentation();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectUsesADifferentNewlineStyle_ShouldSucceed()
			{
				string subject = "foo\r\n    bar";
				string expected = "foo\nbar";

				async Task Act()
					=> await That(subject).Contains(expected).IgnoringIndentation();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class IgnoringNewlineStyleTests
		{
			[Fact]
			public async Task WhenSubjectUsesADifferentNewlineStyle_ShouldFindAllOccurrences()
			{
				string subject = "x\r\na\r\nb\r\ny\r\na\r\nb";
				string expected = "a\nb";

				async Task Act()
					=> await That(subject).Contains(expected).Twice().IgnoringNewlineStyle();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class UsingTests
		{
			[Fact]
			public async Task
				WhenExpectedStringOccursEnoughTimesForTheComparer_ShouldSucceed()
			{
				string subject =
					"In this text in between the word an investigator should find the word 'IN' multiple times.";
				string expected = "in";

				async Task Act()
					=> await That(subject).Contains(expected).Exactly(4)
						.Using(new IgnoreCaseForVocalsComparer());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task
				WhenExpectedStringOccursIncorrectTimesForTheComparer_ShouldIncludeComparerInMessage()
			{
				string subject =
					"In this text in between the word an investigator should find the word 'IN' multiple times.";
				string expected = "in";

				async Task Act()
					=> await That(subject).Contains(expected).Exactly(5)
						.Using(new IgnoreCaseForVocalsComparer());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains "in" using IgnoreCaseForVocalsComparer exactly 5 times,
					             but it contained "in" 4 times in "In this text in between the word an investigator should find the word 'IN' multiple times."
					             """);
			}
		}
	}
}
