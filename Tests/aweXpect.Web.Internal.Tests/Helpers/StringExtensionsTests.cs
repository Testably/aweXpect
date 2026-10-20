using aweXpect.Helpers;

namespace aweXpect.Web.Internal.Tests;

public sealed class StringExtensionsTests
{
	[Test]
	[Arguments(null)]
	[Arguments("foo")]
	public async Task Indent_WhenIndentationIsEmpty_ShouldReturnInput(string? input)
	{
		string? result = input.Indent("");

		await That(result).IsEqualTo(input);
	}

	[Test]
	[Arguments(null)]
	[Arguments("foo")]
	public async Task Indent_WhenIndentationIsNull_ShouldReturnInput(string? input)
	{
		string? result = input.Indent(null);

		await That(result).IsEqualTo(input);
	}

	[Test]
	public async Task Indent_WhenNull_ShouldReturnNull()
	{
		string? input = null;

		string? result = input.Indent();

		await That(result).IsNull();
	}

	[Test]
	[Arguments(true, """
	                    foo
	                    bar
	                  """)]
	[Arguments(false, """
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
