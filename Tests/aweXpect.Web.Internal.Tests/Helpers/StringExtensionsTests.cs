using aweXpect.Helpers;

namespace aweXpect.Web.Internal.Tests;

public sealed class StringExtensionsTests
{
	[Theory]
	[InlineData(null)]
	[InlineData("foo")]
	public async Task Indent_WhenIndentationIsEmpty_ShouldReturnInput(string? input)
	{
		string? result = input.Indent("");

		await That(result).IsEqualTo(input);
	}

	[Theory]
	[InlineData(null)]
	[InlineData("foo")]
	public async Task Indent_WhenIndentationIsNull_ShouldReturnInput(string? input)
	{
		string? result = input.Indent(null);

		await That(result).IsEqualTo(input);
	}

	[Fact]
	public async Task Indent_WhenNull_ShouldReturnNull()
	{
		string? input = null;

		string? result = input.Indent();

		await That(result).IsNull();
	}

	[Theory]
	[InlineData(true, """
	                    foo
	                    bar
	                  """)]
	[InlineData(false, """
	                   foo
	                     bar
	                   """)]
	public async Task Indent_WithIndentFirstLine_ShouldReturnExpectedOutput(bool indentFirstLine, string expectedOutput)
	{
		string input = """
		               foo
		               bar
		               """;

		string? result = input.Indent(indentFirstLine: indentFirstLine);

		await That(result).IsEqualTo(expectedOutput);
	}
}
