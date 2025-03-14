using System.Text;
using System.Text.Json;
using aweXpect.Customization;
using static aweXpect.Formatting.Format;

namespace aweXpect.Json.Tests.Formatting;

public class JsonFormattingTests
{
	[Theory]
	[InlineData("null", "null")]
	[InlineData("true", "true")]
	[InlineData("false", "false")]
	[InlineData("[]", "[]")]
	[InlineData("12", "12")]
	[InlineData("14.5", "14.5")]
	[InlineData("\"foo\"", "\"foo\"")]
	[InlineData("{ \"bar\": 3 }", "{\"bar\":3}")]
	public async Task ShouldFormatExpectedly(string inputJson, string expectedOutput)
	{
		JsonElement jsonElement = FromString(inputJson);
		StringBuilder sb = new();

		string result1 = Formatter.Format(jsonElement);
		Formatter.Format(sb, jsonElement);
		string result2 = sb.ToString();

		await That(result1).IsEqualTo(expectedOutput);
		await That(result2).IsEqualTo(expectedOutput);
	}

	[Fact]
	public async Task ShouldNotInfluenceCustomizationOptions()
	{
		JsonElement jsonElement = FromString("{}");
		StringBuilder sb = new();
		JsonSerializerOptions optionsBefore = Customize.aweXpect.Json().DefaultJsonSerializerOptions.Get();
		bool writeIndentedBefore = optionsBefore.WriteIndented;

		_ = Formatter.Format(jsonElement, FormattingOptions.MultipleLines);

		JsonSerializerOptions optionsAfter = Customize.aweXpect.Json().DefaultJsonSerializerOptions.Get();
		bool writeIndentedAfter = optionsAfter.WriteIndented;

		await That(writeIndentedBefore).IsFalse();
		await That(writeIndentedAfter).IsFalse();
	}

	[Theory]
	[InlineData("[]", "[]")]
	[InlineData("{\"bar\":3}", """
	                           {
	                             "bar": 3
	                           }
	                           """)]
	public async Task WithMultiLine_ShouldSerializeIndented(string inputJson, string expectedOutput)
	{
		JsonElement jsonElement = FromString(inputJson);
		StringBuilder sb = new();

		string result1 = Formatter.Format(jsonElement, FormattingOptions.MultipleLines);
		Formatter.Format(sb, jsonElement, FormattingOptions.MultipleLines);
		string result2 = sb.ToString();

		await That(result1).IsEqualTo(expectedOutput);
		await That(result2).IsEqualTo(expectedOutput);
	}

	private static JsonElement FromString(string value)
	{
		using JsonDocument document = JsonDocument.Parse(value);
		return document.RootElement.Clone();
	}
}
