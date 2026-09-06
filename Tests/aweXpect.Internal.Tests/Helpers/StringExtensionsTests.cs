using System.Linq;
using aweXpect.Helpers;

namespace aweXpect.Internal.Tests.Helpers;

public sealed class StringExtensionsTests
{
	public sealed class GetLineCountTests
	{
		[Theory]
		[InlineData(null, 0)]
		[InlineData("", 0)]
		[InlineData("a", 1)]
		[InlineData("\n", 1)]
		[InlineData("\r", 1)]
		[InlineData("\r\n", 1)]
		[InlineData("a\n", 1)]
		[InlineData("a\r", 1)]
		[InlineData("a\r\n", 1)]
		[InlineData("a\nb", 2)]
		[InlineData("a\rb", 2)]
		[InlineData("a\r\nb", 2)]
		[InlineData("a\n\n", 2)]
		[InlineData("a\n\r", 2)]
		[InlineData("\r\n\r\n", 2)]
		[InlineData("a\nb\n", 2)]
		[InlineData("a\r\nb\r\n", 2)]
		[InlineData("a\nb\rc\r\nd", 4)]
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
		[Fact]
		public async Task WhenIndentationIsNotEmpty_ShouldReturnIndentedInput()
		{
			string input = "foo\nbar";
			string expected = "   foo\n   bar";

			string result = input.Indent("   ");

			await That(result).IsEqualTo(expected);
		}

		[Theory]
		[InlineData("")]
		[InlineData(null)]
		public async Task WhenIndentationIsNullOrEmpty_ShouldReturnInput(string? indentation)
		{
			string input = "foo\nbar";

			string result = input.Indent(indentation);

			await That(result).IsEqualTo(input);
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
		public async Task WhenInputIsNull_ShouldReturnNull()
		{
			string? input = null;

			string? result = input.Indent();

			await That(result).IsNull();
		}
	}

	public sealed class PrependAOrAnTests
	{
		[Theory]
		[InlineData("", "a ")]
		[InlineData("apple", "an apple")]
		[InlineData("bee", "a bee")]
		[InlineData("Exception", "an Exception")]
		[InlineData("NotSupportedException", "a NotSupportedException")]
		public async Task ShouldReturnExpectedValue(string input, string expected)
		{
			string result = input.PrependAOrAn();

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

		[Fact]
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
