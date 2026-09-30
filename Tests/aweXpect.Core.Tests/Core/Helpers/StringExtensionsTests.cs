using aweXpect.Core.Helpers;

namespace aweXpect.Core.Tests.Core.Helpers;

public class StringExtensionsTests
{
	public sealed class DisplayWhitespace
	{
		[Theory]
		[InlineData("a\0b", @"a\0b")]
		[InlineData("\u0001\u001F", @"\u0001\u001F")]
		[InlineData("\u007F\u0085", @"\u007F\u0085")]
		[InlineData("a\u00A0b", @"a\u00A0b")]
		[InlineData("a\u200Bb", @"a\u200Bb")]
		[InlineData("\u00AD\u200D\u2060\uFEFF", @"\u00AD\u200D\u2060\uFEFF")]
		[InlineData("\u2007\u202F\u3000", @"\u2007\u202F\u3000")]
		[InlineData("\u2028\u2029", @"\u2028\u2029")]
		public async Task ShouldEscapeControlAndInvisibleCharacters(string input, string expected)
		{
			string result = input.DisplayWhitespace();

			await That(result).IsEqualTo(expected)
				.Because("characters that are invisible or look like a plain space must be told apart in a message");
		}

		[Fact]
		public async Task ShouldEscapeNewlines()
		{
			string input = "\r,\n;\t ";
			string expected = @"\r,\n;\t ";

			string result = input.DisplayWhitespace();

			await That(result).IsEqualTo(expected);
		}

		[Fact]
		public async Task ShouldKeepBackslashesQuotesAndVisibleCharacters()
		{
			string input = "C:\\temp \"a\" 'b' äß€😀";

			string result = input.DisplayWhitespace();

			await That(result).IsEqualTo(input)
				.Because("unquoted text like an exception message is not a literal, so a path keeps its backslashes");
		}

		[Fact]
		public async Task WhenNull_ShouldReturnNull()
		{
			string? input = null;

			string? result = input.DisplayWhitespace();

			await That(result).IsNull();
		}
	}

	public sealed class Escape
	{
		[Fact]
		public async Task ShouldEscapeBackslash()
		{
			string input = "a\\nb";
			string expected = @"a\\nb";

			string result = input.Escape();

			await That(result).IsEqualTo(expected)
				.Because("a backslash in the value must not be confused with an escaped newline");
		}

		[Fact]
		public async Task ShouldEscapeControlAndInvisibleCharacters()
		{
			string input = "\r\n\t\0\u00A0\u200B";
			string expected = @"\r\n\t\0\u00A0\u200B";

			string result = input.Escape();

			await That(result).IsEqualTo(expected);
		}

		[Theory]
		[InlineData('"', "a\"b'c", "a\\\"b'c")]
		[InlineData('\'', "a\"b'c", "a\"b\\'c")]
		public async Task ShouldOnlyEscapeTheGivenQuote(char quote, string input, string expected)
		{
			string result = input.Escape(quote);

			await That(result).IsEqualTo(expected)
				.Because("only the quote that encloses the value can end it early, like in a C# literal");
		}

		[Fact]
		public async Task WhenNothingToEscape_ShouldReturnSameInstance()
		{
			string input = "foo bar äß😀";

			string result = input.Escape();

			await That(result).IsSameAs(input);
		}

		[Fact]
		public async Task WhenNull_ShouldReturnNull()
		{
			string? input = null;

			string? result = input.Escape();

			await That(result).IsNull();
		}
	}

	public sealed class Indent
	{
		[Fact]
		public async Task WhenIndentationIsEmpty_ShouldReturnInput()
		{
			string input = "foo\nbar";

			string result = input.Indent("");

			await That(result).IsEqualTo(input);
		}

		[Fact]
		public async Task WhenIndentationIsNotEmpty_ShouldReturnIndentedInput()
		{
			string input = "foo\nbar";
			string expected = "   foo\n   bar";

			string result = input.Indent("   ");

			await That(result).IsEqualTo(expected);
		}

		[Fact]
		public async Task WhenIndentFirstLineIsFalse_ShouldOnlyIndentSubsequentLines()
		{
			string input = "foo\nbar";
			string expected = "foo\n   bar";

			string result = input.Indent("   ", false);

			await That(result).IsEqualTo(expected);
		}

		[Fact]
		public async Task WhenNull_ShouldReturnNull()
		{
			string? input = null;

			string? result = input.Indent();

			await That(result).IsNull();
		}
	}

	public sealed class PrependAOrAn
	{
		[Theory]
		[InlineData("", "a ")]
		[InlineData("apple", "an apple")]
		[InlineData("bee", "a bee")]
		[InlineData("Exception", "an Exception")]
		[InlineData("NotSupportedException", "a NotSupportedException")]
		[InlineData("ArgumentException", "an ArgumentException")]
		[InlineData("HashSet<int>", "a HashSet<int>")]
		[InlineData("Hero", "a Hero")]
		[InlineData("HResultException", "an HResultException")]
		[InlineData("IOException", "an IOException")]
		[InlineData("SMTPException", "an SMTPException")]
		[InlineData("X509Exception", "an X509Exception")]
		[InlineData("TException", "a TException")]
		[InlineData("UInt32", "a UInt32")]
		[InlineData("User", "a User")]
		[InlineData("UriFormatException", "a UriFormatException")]
		[InlineData("UnauthorizedAccessException", "an UnauthorizedAccessException")]
		[InlineData("Update", "an Update")]
		[InlineData("H", "an H")]
		[InlineData("U", "a U")]
		public async Task ShouldReturnExpectedResult(string input, string expected)
		{
			string result = input.PrependAOrAn();

			await That(result).IsEqualTo(expected);
		}
	}

	public sealed class RemoveNewlineStyle
	{
		[Fact]
		public async Task ShouldReplaceNewlinesWithSlashN()
		{
			string input = "\ra\r\nb\nc";
			string expected = "\na\nb\nc";

			string result = input.RemoveNewlineStyle();

			await That(result).IsEqualTo(expected);
		}

		[Fact]
		public async Task WhenNull_ShouldReturnNull()
		{
			string? input = null;

			string? result = input.RemoveNewlineStyle();

			await That(result).IsNull();
		}
	}

	public sealed class SubstringUntilFirst
	{
		[Fact]
		public async Task WhenFirstCharacter_ShouldReturnEmptyString()
		{
			string input = "a,b,c";

			string result = input.SubstringUntilFirst('a');

			await That(result).IsEqualTo("");
		}


		[Fact]
		public async Task WhenNotPresent_ShouldReturnString()
		{
			string input = "foo";

			string result = input.SubstringUntilFirst('X');

			await That(result).IsEqualTo(input);
		}

		[Fact]
		public async Task WhenPresent_ShouldReturnSubstringUntilFirstOccurrence()
		{
			string input = "a,b,c";

			string result = input.SubstringUntilFirst(',');

			await That(result).IsEqualTo("a");
		}
	}

	public sealed class TruncateWithEllipsis
	{
		[Fact]
		public async Task WhenCutWouldSplitALineBreak_ShouldCutBeforeTheLineBreak()
		{
			string input = "abcd\r\nefgh";

			string result = input.TruncateWithEllipsis(5);

			await That(result).IsEqualTo("abcd…")
				.Because("a lone \\r would suggest that the value contains no \\n");
		}

		[Fact]
		public async Task WhenCutWouldSplitASurrogatePair_ShouldCutBeforeThePair()
		{
			string input = "abcd\U0001F600efgh";

			string result = input.TruncateWithEllipsis(5);

			await That(result).IsEqualTo("abcd…")
				.Because("a lone high surrogate is not a valid character");
		}

		[Fact]
		public async Task WhenLonger_ShouldTruncateWithEllipsis()
		{
			string input = "12345678910";
			string expected = "1234567891…";

			string result = input.TruncateWithEllipsis(10);

			await That(result).IsEqualTo(expected);
		}


		[Fact]
		public async Task WhenNull_ShouldReturnNull()
		{
			string? input = null;

			string? result = input.TruncateWithEllipsis(10);

			await That(result).IsNull();
		}

		[Fact]
		public async Task WhenShorter_ShouldReturnInput()
		{
			string input = "1234567890";

			string result = input.TruncateWithEllipsis(10);

			await That(result).IsEqualTo(input);
		}
	}

	public sealed class TruncateWithEllipsisOnWord
	{
		[Fact]
		public async Task WhenCutWouldSplitALineBreak_ShouldCutBeforeTheLineBreak()
		{
			string input = $"{new string('a', 29)}\r\nb";

			string result = input.TruncateWithEllipsisOnWord(30);

			await That(result).IsEqualTo($"{new string('a', 29)}…")
				.Because("a lone \\r would suggest that the value contains no \\n");
		}

		[Fact]
		public async Task WhenCutWouldSplitASurrogatePair_ShouldCutBeforeThePair()
		{
			string input = $"{new string('a', 29)}\U0001F600b";

			string result = input.TruncateWithEllipsisOnWord(30);

			await That(result).IsEqualTo($"{new string('a', 29)}…")
				.Because("a lone high surrogate is not a valid character");
		}

		[Fact]
		public async Task WhenLongerWithoutWordBoundary_ShouldTruncateOnWordWithEllipsis()
		{
			string input = "some word boundary";
			string expected = "some word…";

			string result = input.TruncateWithEllipsisOnWord(11);

			await That(result).IsEqualTo(expected);
		}

		[Fact]
		public async Task WhenLongerWithoutWordBoundary_ShouldTruncateWithEllipsis()
		{
			string input = "12345678910";
			string expected = "1234567891…";

			string result = input.TruncateWithEllipsisOnWord(10);

			await That(result).IsEqualTo(expected);
		}

		[Fact]
		public async Task WhenNull_ShouldReturnNull()
		{
			string? input = null;

			string? result = input.TruncateWithEllipsisOnWord(10);

			await That(result).IsNull();
		}

		[Fact]
		public async Task WhenShorter_ShouldReturnInput()
		{
			string input = "1234567890";

			string result = input.TruncateWithEllipsisOnWord(10);

			await That(result).IsEqualTo(input);
		}

		[Theory]
		[InlineData("another word-boundary", "another wo…")]
		[InlineData("_another word-boundary", "_another…")]
		public async Task WhenWordBoundaryIsBelow80Percent_ShouldTruncateWithEllipsis(
			string input, string expected)
		{
			string result = input.TruncateWithEllipsisOnWord(10);

			await That(result).IsEqualTo(expected);
		}
	}

	public sealed class TrimCommonWhiteSpace
	{
		[Fact]
		public async Task WhenAnyLaterLineHasNoWhiteSpace_ShouldReturnUnchangedInput()
		{
			string input = """
			               foo
			                   bar
			               baz
			                  bay
			               """;

			string result = input.TrimCommonWhiteSpace();

			await That(result).IsEqualTo(input);
		}

		[Theory]
		[InlineData("\n")]
		[InlineData("\r\n")]
		public async Task WhenBlankLinesAreShorterThanCommonWhiteSpace_ShouldIgnoreThem(string newLine)
		{
			string input = $"foo{newLine}{newLine}    bar{newLine}  {newLine}      baz";

			string result = input.TrimCommonWhiteSpace();

			await That(result).IsEqualTo($"foo{newLine}{newLine}bar{newLine}{newLine}  baz")
				.Because("blank lines must neither limit the common whitespace nor break the trimming");
		}

		[Fact]
		public async Task WhenEmpty_ShouldReturnEmptyString()
		{
			string input = string.Empty;

			string result = input.TrimCommonWhiteSpace();

			await That(result).IsEmpty();
		}

		[Theory]
		[InlineData("\n")]
		[InlineData("\r\n")]
		public async Task WhenLinesAreSeparatedBy_ShouldTrimAndKeepLineEndings(string newLine)
		{
			string input = $"foo{newLine}    bar{newLine}      baz";

			string result = input.TrimCommonWhiteSpace();

			await That(result).IsEqualTo($"foo{newLine}bar{newLine}  baz")
				.Because("the trimming must not depend on the line endings of the source or the operating system");
		}

		[Fact]
		public async Task WhenLinesHaveDifferentWhiteSpace_ShouldKeepAllWhiteSpace()
		{
			string input = """
			               foo
			                   bar
			               	baz
			               """;

			string result = input.TrimCommonWhiteSpace();

			await That(result).IsEqualTo("""
			                             foo
			                                 bar
			                             	baz
			                             """);
		}

		[Fact]
		public async Task WhenLinesHaveSomeCommonWhiteSpace_ShouldTrim()
		{
			string input = """
			               foo
			                   bar
			                 baz
			                  bay
			               """;

			string result = input.TrimCommonWhiteSpace();

			await That(result).IsEqualTo("""
			                             foo
			                               bar
			                             baz
			                              bay
			                             """);
		}

		[Fact]
		public async Task WhenOnlyHasOneLine_ShouldReturnLine()
		{
			string input = "foo";

			string result = input.TrimCommonWhiteSpace();

			await That(result).IsEqualTo(input);
		}

		[Fact]
		public async Task WhenTwoLines_ShouldTrimSecondLine()
		{
			string input = """
			               foo
			                	 bar
			               """;

			string result = input.TrimCommonWhiteSpace();

			await That(result).IsEqualTo("""
			                             foo
			                             bar
			                             """);
		}
	}
}
