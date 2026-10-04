using aweXpect.Core.Extending;

namespace aweXpect.Core.Tests.Core.Extending;

public class StringExtensionsTests
{
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
		public async Task WhenIndentationIsNull_ShouldReturnInput()
		{
			string input = "foo\nbar";

			string result = input.Indent(null);

			await That(result).IsEqualTo(input)
				.Because("the indentation of AppendExpectation or AppendResult can be passed on as it is");
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
		public async Task WhenLinesAreSeparatedByCarriageReturnAndLineFeed_ShouldKeepLineEndings()
		{
			string input = "foo\r\n\r\nbar";

			string result = input.Indent("  ");

			await That(result).IsEqualTo("  foo\r\n  \r\n  bar")
				.Because("the line endings must be kept and empty lines are indented as well");
		}

		[Fact]
		public async Task WhenNull_ShouldReturnNull()
		{
			string? input = null;

			string? result = input.Indent();

			await That(result).IsNull();
		}

		[Fact]
		public async Task WithoutIndentation_ShouldIndentAllLinesByTwoSpaces()
		{
			string input = "foo\nbar";

			string result = input.Indent();

			await That(result).IsEqualTo("  foo\n  bar");
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

		[Fact]
		public async Task WhenNull_ShouldThrowArgumentNullException()
		{
			string value = null!;

			void Act() => value.PrependAOrAn();

			await That(Act).Throws<ArgumentNullException>()
				.WithParamName("value").And
				.WithMessage("The 'value' cannot be null.").AsPrefix();
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
		public async Task WhenNull_ShouldThrowArgumentNullException()
		{
			string value = null!;

			void Act() => value.TrimCommonWhiteSpace();

			await That(Act).Throws<ArgumentNullException>()
				.WithParamName("value").And
				.WithMessage("The 'value' cannot be null.").AsPrefix();
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
