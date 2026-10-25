using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using aweXpect.Customization;
using static aweXpect.Formatting.Format;

namespace aweXpect.Json.Tests.Formatting;

public class JsonFormattingTests
{
	[Test]
	public async Task CoreExpectationOnJsonElements_ShouldNotDependOnPreviousJsonExpectations()
	{
		List<JsonElement> subject = [FromString("\"Help!\""),];

		async Task Act()
			=> await That(subject).HasCount(2);

		await That(FromString("[1]")).IsArray().WhoseElements.HasCount(1);
		await That(FromString("{\"title\": \"Help!\"}")).IsObject().WhoseProperties.HasCount(1);
		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             has exactly 2 items,
			             but it had only 1 item

			             Collection:
			             [
			               Help!
			             ]
			             """)
			.Because("Json formats its types only within its own expectations, whatever ran before");
	}

	[Test]
	public async Task InCombination_ShouldFormatJsonTypesOfTheJsonExpectation()
	{
		JsonElement subject = FromString("[\"Help!\", \"Abbey Road\"]");

		async Task Act()
			=> await ThatAll(
				That(subject).IsArray().WhoseElements.HasCount(3),
				That(new[] { 1, 2, }).HasCount(2));

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected all of the following to succeed:
			              [01] Expected that subject is an array whose elements have exactly 3 items
			              [02] Expected that new[] { 1, 2, } has exactly 2 items
			             but
			              [01] elements had only 2 items

			             [01] Collection (elements):
			             [
			               "Help!",
			               "Abbey Road"
			             ]
			             """)
			.Because("a combination formats with the formatters of its expectations");
	}

	[Test]
	public async Task InNestedJsonExpectation_ShouldFormatJsonTypesInTheWholeExpectation()
	{
		JsonElement[] subject = [FromString("[\"Help!\", \"Abbey Road\"]"),];

		async Task Act()
			=> await That(subject).All().ComplyWith(album => album.IsArray().WhoseElements.HasCount(3));

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             is an array whose elements have exactly 3 items for all items,
			             but none of 1 were

			             Not matching items:
			             [
			               ["Help!","Abbey Road"]
			             ]

			             Collection:
			             [
			               ["Help!","Abbey Road"]
			             ]

			             Collection (item [0].elements):
			             [
			               "Help!",
			               "Abbey Road"
			             ]
			             """)
			.Because("the formatters of a nested Json expectation apply to the whole expectation it is nested in");
	}

	[Test]
	public async Task OutsideOfJsonExpectations_ShouldKeepTheDefaultFormatting()
	{
		JsonElement element = FromString("\"Abbey Road\"");
		JsonElement document = FromString("{\"title\": \"Abbey Road\"}");
		JsonProperty property = document.EnumerateObject().First();

		await That(document).IsObject().WhoseProperties.HasCount(1);
		await That(FromString("[1]")).IsArray().WhoseElements.HasCount(1);
		string formatted = Formatter.Format(new object[] { element, property, });

		await That(formatted).IsEqualTo($"[{element}, {property}]")
			.Because("the Json expectations must not change how other code formats these types");
	}

	[Test]
	[Arguments("null", "null")]
	[Arguments("true", "true")]
	[Arguments("false", "false")]
	[Arguments("[]", "[]")]
	[Arguments("12", "12")]
	[Arguments("14.5", "14.5")]
	[Arguments("\"foo\"", "\"foo\"")]
	[Arguments("{ \"bar\": 3 }", "{\"bar\":3}")]
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

	[Test]
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

	[Test]
	public async Task WithCustomizedOptions_ShouldBeUsableInMultipleExpectations()
	{
		JsonElement subject = FromString("{\"foo\": 1}");

		async Task Act()
			=> await That(subject).DoesNotComplyWith(it => it.Matches(new { foo = 1, }));

		using (Customize.aweXpect.Json().DefaultJsonSerializerOptions.Set(new JsonSerializerOptions()))
		{
			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             does not match new { foo = 1, },
				             but it did

				             Actual:
				             {
				               "foo": 1
				             }

				             JSON options:
				              - ignore additional properties
				             """);
			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             does not match new { foo = 1, },
				             but it did

				             Actual:
				             {
				               "foo": 1
				             }

				             JSON options:
				              - ignore additional properties
				             """)
				.Because("options that were already used for serialization are read-only and must still format");
		}
	}

	[Test]
	[Arguments(false)]
	[Arguments(true)]
	public async Task WithCustomizedOptions_ShouldNotChangeWriteIndented(bool writeIndented)
	{
		JsonElement jsonElement = FromString("{\"bar\":3}");
		JsonSerializerOptions serializerOptions = new()
		{
			WriteIndented = writeIndented,
		};
		StringBuilder singleLine = new();
		StringBuilder multipleLines = new();

		using (Customize.aweXpect.Json().DefaultJsonSerializerOptions.Set(serializerOptions))
		{
			_ = Formatter.Format(jsonElement);
			_ = Formatter.Format(jsonElement, FormattingOptions.MultipleLines);
			Formatter.Format(singleLine, jsonElement);
			Formatter.Format(multipleLines, jsonElement, FormattingOptions.MultipleLines);
		}

		await That(serializerOptions.WriteIndented).IsEqualTo(writeIndented)
			.Because("formatting must not mutate the customized options");
		await That(singleLine.ToString()).IsEqualTo("{\"bar\":3}");
		await That(multipleLines.ToString()).IsEqualTo("""
		                                              {
		                                                "bar": 3
		                                              }
		                                              """);
	}

	[Test]
	[Arguments("[]", "[]")]
	[Arguments("{\"bar\":3}", """
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
