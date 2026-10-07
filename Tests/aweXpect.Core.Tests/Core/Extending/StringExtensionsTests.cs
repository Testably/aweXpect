using aweXpect.Core.Extending;

namespace aweXpect.Core.Tests.Core.Extending;

public class StringExtensionsTests
{
	public sealed class Indent
	{
		[Test]
		public async Task WhenIndentationIsEmpty_ShouldReturnInput()
		{
			string input = "foo\nbar";

			string result = input.Indent("");

			await That(result).IsEqualTo(input);
		}

		[Test]
		public async Task WhenIndentationIsNotEmpty_ShouldReturnIndentedInput()
		{
			string input = "foo\nbar";
			string expected = "   foo\n   bar";

			string result = input.Indent("   ");

			await That(result).IsEqualTo(expected);
		}

		[Test]
		public async Task WhenIndentationIsNull_ShouldReturnInput()
		{
			string input = "foo\nbar";

			string result = input.Indent(null);

			await That(result).IsEqualTo(input)
				.Because("the indentation of AppendExpectation or AppendResult can be passed on as it is");
		}

		[Test]
		public async Task WhenIndentFirstLineIsFalse_ShouldOnlyIndentSubsequentLines()
		{
			string input = "foo\nbar";
			string expected = "foo\n   bar";

			string result = input.Indent("   ", false);

			await That(result).IsEqualTo(expected);
		}

		[Test]
		public async Task WhenLinesAreSeparatedByCarriageReturnAndLineFeed_ShouldKeepLineEndings()
		{
			string input = "foo\r\n\r\nbar";

			string result = input.Indent();

			await That(result).IsEqualTo("  foo\r\n  \r\n  bar")
				.Because("the line endings must be kept and empty lines are indented as well");
		}

		[Test]
		public async Task WhenNull_ShouldReturnNull()
		{
			string? input = null;

			string? result = input.Indent();

			await That(result).IsNull();
		}

		[Test]
		public async Task WithoutIndentation_ShouldIndentAllLinesByTwoSpaces()
		{
			string input = "foo\nbar";

			string result = input.Indent();

			await That(result).IsEqualTo("  foo\n  bar");
		}
	}

	public sealed class PrependAOrAn
	{
		[Test]
		[Arguments("", "a ")]
		[Arguments("apple", "an apple")]
		[Arguments("bee", "a bee")]
		[Arguments("Exception", "an Exception")]
		[Arguments("NotSupportedException", "a NotSupportedException")]
		[Arguments("ArgumentException", "an ArgumentException")]
		[Arguments("HashSet<int>", "a HashSet<int>")]
		[Arguments("Hero", "a Hero")]
		[Arguments("HResultException", "an HResultException")]
		[Arguments("IOException", "an IOException")]
		[Arguments("SMTPException", "an SMTPException")]
		[Arguments("X509Exception", "an X509Exception")]
		[Arguments("TException", "a TException")]
		[Arguments("UInt32", "a UInt32")]
		[Arguments("User", "a User")]
		[Arguments("UriFormatException", "a UriFormatException")]
		[Arguments("UnauthorizedAccessException", "an UnauthorizedAccessException")]
		[Arguments("Update", "an Update")]
		[Arguments("H", "an H")]
		[Arguments("U", "a U")]
		[Arguments("Int32", "an Int32")]
		[Arguments("Object", "an Object")]
		[Arguments("orange", "an orange")]
		[Arguments("uint", "an uint")]
		[Arguments("ulong", "a ulong")]
		[Arguments("xylophone", "a xylophone")]
		[Arguments("1st", "a 1st")]
		public async Task ShouldReturnExpectedResult(string input, string expected)
		{
			string result = input.PrependAOrAn();

			await That(result).IsEqualTo(expected);
		}

		[Test]
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
		[Test]
		public async Task WhenAllLaterLinesAreBlank_ShouldReturnUnchangedInput()
		{
			string input = "foo\n  \n";

			string result = input.TrimCommonWhiteSpace();

			await That(result).IsEqualTo(input)
				.Because("blank lines don't limit the common whitespace, so there is none to remove");
		}

		[Test]
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

		[Test]
		[Arguments("\n")]
		[Arguments("\r\n")]
		public async Task WhenBlankLinesAreShorterThanCommonWhiteSpace_ShouldIgnoreThem(string newLine)
		{
			string input = $"foo{newLine}{newLine}    bar{newLine}  {newLine}      baz";

			string result = input.TrimCommonWhiteSpace();

			await That(result).IsEqualTo($"foo{newLine}{newLine}bar{newLine}{newLine}  baz")
				.Because("blank lines must neither limit the common whitespace nor break the trimming");
		}

		[Test]
		public async Task WhenEmpty_ShouldReturnEmptyString()
		{
			string input = string.Empty;

			string result = input.TrimCommonWhiteSpace();

			await That(result).IsEmpty();
		}

		[Test]
		[Arguments("\n")]
		[Arguments("\r\n")]
		public async Task WhenLinesAreSeparatedBy_ShouldTrimAndKeepLineEndings(string newLine)
		{
			string input = $"foo{newLine}    bar{newLine}      baz";

			string result = input.TrimCommonWhiteSpace();

			await That(result).IsEqualTo($"foo{newLine}bar{newLine}  baz")
				.Because("the trimming must not depend on the line endings of the source or the operating system");
		}

		[Test]
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

		[Test]
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

		[Test]
		public async Task WhenNull_ShouldThrowArgumentNullException()
		{
			string value = null!;

			void Act() => value.TrimCommonWhiteSpace();

			await That(Act).Throws<ArgumentNullException>()
				.WithParamName("value").And
				.WithMessage("The 'value' cannot be null.").AsPrefix();
		}

		[Test]
		public async Task WhenOnlyHasOneLine_ShouldReturnLine()
		{
			string input = "foo";

			string result = input.TrimCommonWhiteSpace();

			await That(result).IsEqualTo(input);
		}

		[Test]
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
