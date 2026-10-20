using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection.Internal.Tests.Helpers;

public class StringHelperTests
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

		string result = input.TrimExpression();

		await That(result).IsEqualTo(input);
	}

	[Test]
	public async Task WhenEmpty_ShouldReturnEmptyString()
	{
		string input = string.Empty;

		string result = input.TrimExpression();

		await That(result).IsEmpty();
	}

	[Test]
	public async Task WhenLaterLineEndsWithButDoesNotStartWithCommonWhiteSpace_ShouldTrimCommonPrefix()
	{
		// line[1] = "  bar" => common white-space is the two leading spaces.
		// line[2] = "x  " starts with 'x' (not the common prefix) but ends with two spaces.
		// The common prefix must be truncated based on a leading-character comparison
		// (StartsWith), not a trailing one (EndsWith).
		string input = "foo\n  bar\nx  ";

		string result = input.TrimExpression();

		await That(result).IsEqualTo("foo\n  bar\nx  ");
	}

	[Test]
	public async Task WhenLaterLineIsBlank_ShouldIgnoreItForCommonWhiteSpace()
	{
		string input = "foo\n    bar\n\n    baz";

		string result = input.TrimExpression();

		await That(result).IsEqualTo("foo\nbar\n\nbaz")
			.Because("blank lines are often trimmed by editors and must not prevent trimming the other lines");
	}

	[Test]
	public async Task WhenLinesHaveDifferentWhiteSpace_ShouldKeepAllWhiteSpace()
	{
		string input = """
		               foo
		                   bar
		               	baz
		               """;

		string result = input.TrimExpression();

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

		string result = input.TrimExpression();

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

		string result = input.TrimExpression();

		await That(result).IsEqualTo(input);
	}

	[Test]
	public async Task WhenTwoLines_ShouldTrimSecondLineAndKeepOnOneLine()
	{
		string input = """
		               foo
		                	 bar
		               """;

		string result = input.TrimExpression();

		await That(result).IsEqualTo("""
		                             foo bar
		                             """);
	}
}
