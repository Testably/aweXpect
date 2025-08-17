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
