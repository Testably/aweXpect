using aweXpect.Core.Helpers;

namespace aweXpect.Core.Tests.Core.Helpers;

public class StringExtensionsTests
{
	public sealed class DisplayWhitespace
	{
		[Fact]
		public async Task ShouldEscapeNewlines()
		{
			string input = "\r,\n;\t ";
			string expected = @"\r,\n;\t ";

			string result = input.DisplayWhitespace();

			await That(result).IsEqualTo(expected);
		}

		[Fact]
		public async Task WhenNull_ShouldReturnNull()
		{
			string? input = null;

			string? result = input.DisplayWhitespace();

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

	public sealed class ToSingleLine
	{
		[Fact]
		public async Task ShouldEscapeNewlines()
		{
			string input = "\r,\n;\t ";
			string expected = "\\r,\\n;\t ";

			string result = input.ToSingleLine();

			await That(result).IsEqualTo(expected);
		}

		[Fact]
		public async Task WhenNull_ShouldReturnNull()
		{
			string? input = null;

			string? result = input.ToSingleLine();

			await That(result).IsNull();
		}
	}

	public sealed class TruncateWithEllipsis
	{
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

		[Fact]
		public async Task WhenEmpty_ShouldReturnEmptyString()
		{
			string input = string.Empty;

			string result = input.TrimCommonWhiteSpace();

			await That(result).IsEmpty();
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
