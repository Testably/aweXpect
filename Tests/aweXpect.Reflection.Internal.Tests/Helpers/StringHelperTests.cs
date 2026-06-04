using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection.Internal.Tests.Helpers;

public class StringHelperTests
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
	public async Task WhenLaterLineEndsWithButDoesNotStartWithCommonWhiteSpace_ShouldTrimCommonPrefix()
	{
		// line[1] = "  bar" => common white-space is the two leading spaces.
		// line[2] = "x  " starts with 'x' (not the common prefix) but ends with two spaces.
		// The common prefix must be truncated based on a leading-character comparison
		// (StartsWith), not a trailing one (EndsWith).
		string input = "foo\n  bar\nx  ";

		string result = input.TrimCommonWhiteSpace();

		await That(result).IsEqualTo("foo\n  bar\nx  ");
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
	public async Task WhenTwoLines_ShouldTrimSecondLineAndKeepOnOneLine()
	{
		string input = """
		               foo
		                	 bar
		               """;

		string result = input.TrimCommonWhiteSpace();

		await That(result).IsEqualTo("""
		                             foo bar
		                             """);
	}
}
