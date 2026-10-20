using System.Text;
using System.Text.Json;
using aweXpect.Customization;
using static aweXpect.Formatting.Format;

namespace aweXpect.Json.Tests.Formatting;

public class JsonFormattingTests
{
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
			=> await That(subject).DoesNotComplyWith(it => it.IsObject(o => o.With("foo").Matching(1)));

		using (Customize.aweXpect.Json().DefaultJsonSerializerOptions.Set(new JsonSerializerOptions()))
		{
			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is not an object or $.foo does not match 1,
				             but it was

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
				             is not an object or $.foo does not match 1,
				             but it was

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
