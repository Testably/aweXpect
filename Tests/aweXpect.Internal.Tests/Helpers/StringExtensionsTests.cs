using System.Linq;
using aweXpect.Helpers;

namespace aweXpect.Internal.Tests.Helpers;

public sealed class StringExtensionsTests
{
	public sealed class GetLineCountTests
	{
		[Test]
		[Arguments(null, 0)]
		[Arguments("", 0)]
		[Arguments("a", 1)]
		[Arguments("\n", 1)]
		[Arguments("\r", 1)]
		[Arguments("\r\n", 1)]
		[Arguments("a\n", 1)]
		[Arguments("a\r", 1)]
		[Arguments("a\r\n", 1)]
		[Arguments("a\nb", 2)]
		[Arguments("a\rb", 2)]
		[Arguments("a\r\nb", 2)]
		[Arguments("a\n\n", 2)]
		[Arguments("a\n\r", 2)]
		[Arguments("\r\n\r\n", 2)]
		[Arguments("a\nb\n", 2)]
		[Arguments("a\r\nb\r\n", 2)]
		[Arguments("a\nb\rc\r\nd", 4)]
		public async Task ShouldReturnExpectedLineCount(string? value, int expected)
		{
			int result = value.GetLineCount();

			await That(result).IsEqualTo(expected);
			// GetLineCount duplicates the line semantics of GetLines, so both must agree.
			await That(result).IsEqualTo(value.GetLines().Count());
		}
	}

	public sealed class IndentTests
	{
		[Test]
		public async Task WhenIndentationIsNotEmpty_ShouldReturnIndentedInput()
		{
			string input = "foo\nbar";
			string expected = "   foo\n   bar";

			string result = input.Indent("   ");

			await That(result).IsEqualTo(expected);
		}

		[Test]
		[Arguments("")]
		[Arguments(null)]
		public async Task WhenIndentationIsNullOrEmpty_ShouldReturnInput(string? indentation)
		{
			string input = "foo\nbar";

			string result = input.Indent(indentation);

			await That(result).IsEqualTo(input);
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
		public async Task WhenInputIsNull_ShouldReturnNull()
		{
			string? input = null;

			string? result = input.Indent();

			await That(result).IsNull();
		}
	}

	public sealed class PrependAOrAnTests
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
		public async Task ShouldReturnExpectedValue(string input, string expected)
		{
			string result = input.PrependAOrAn();

			await That(result).IsEqualTo(expected);
		}
	}

	public sealed class TrimCommonWhiteSpace
	{
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
		public async Task WhenLinesHaveSomeCommonWhiteSpace1_ShouldTrim()
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
		public async Task WhenLinesHaveSomeCommonWhiteSpace2_ShouldTrim()
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
