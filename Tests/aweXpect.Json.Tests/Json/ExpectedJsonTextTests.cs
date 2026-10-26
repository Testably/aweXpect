using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;
using aweXpect.Core;
using aweXpect.Equivalency;

namespace aweXpect.Json.Tests;

public sealed class ExpectedJsonTextTests
{
	[Test]
	public async Task AnonymousObject_ShouldMatchItsStringsAsJsonStrings()
	{
		JsonElement subject = Parse("""{"title": "Let It Be"}""");

		async Task Act()
			=> await That(subject).Matches(new
			{
				title = "Let It Be",
			});

		await That(Act).DoesNotThrow()
			.Because("a string inside an expected object is a JSON string value");
	}

	[Test]
	public async Task Array_ShouldMatchTheItems()
	{
		JsonElement subject = Parse("""["Let It Be", "Help!"]""");

		async Task Act()
			=> await That(subject).Matches(new[] { "Let It Be", "Help!", });

		await That(Act).DoesNotThrow()
			.Because("an array still binds to the overload for collections");
	}

	[Test]
	public async Task Expectation_ShouldMatchAJsonString()
	{
		JsonElement subject = Parse("\"Let It Be\"");

		async Task Act()
			=> await That(subject).Matches(It.Is<string>().That.IsEqualTo("Help!"));

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             matches It.Is<string>().That.IsEqualTo("Help!"),
			             but it differed as $ was "Let It Be", which differs at index 0:
			                ↓ (actual)
			               "Let It Be"
			               "Help!"
			                ↑ (expected)

			             JSON options:
			              - ignore additional properties
			             """);
	}

	[Test]
	[MethodDataSource(nameof(InvalidJsonTextCalls))]
	public async Task InvalidJson_ShouldThrowWhenTheExpectationIsWritten(JsonTextCall call)
	{
		await That(call.Write).Throws<ArgumentException>()
			.WithParamName("expected").And
			.WithMessage("The 'expected' JSON is invalid: 'L' is an invalid start of a value. LineNumber: 0 | BytePositionInLine: 0.").AsPrefix()
			.Because("invalid JSON is rejected before the expectation is awaited, so that a negation cannot pass");
	}

	[Test]
	public async Task JsonScalar_ShouldBeParsed()
	{
		JsonElement subject = Parse("\"42\"");

		async Task Act()
			=> await That(subject).Matches("42");

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             matches "42",
			             but it differed as $ was string "42" instead of 42

			             JSON options:
			              - ignore additional properties
			             """)
			.Because("the expected string is parsed as the JSON number 42");
	}

	[Test]
	[MethodDataSource(nameof(MatchingJsonTextCalls))]
	public async Task JsonText_ShouldBeParsed(JsonTextCall call)
	{
		async Task Act()
			=> await ThatAll(call.Write());

		await That(Act).DoesNotThrow();
	}

	[Test]
	public async Task JsonText_ShouldBeParsedWithTheDocumentOptions()
	{
		JsonElement subject = Parse("""{"title": "Let It Be"}""");

		async Task Act()
			=> await That(subject).Matches("""/* album */ {"title": "Let It Be"}""",
				o => o.WithDocumentOptions(d => d with
				{
					CommentHandling = JsonCommentHandling.Skip,
				}));

		await That(Act).DoesNotThrow();
	}

	[Test]
	public async Task NegatedJsonText_OnJsonElement_ShouldFailWhenMatching()
	{
		JsonElement subject = Parse("""{"title": "Let It Be"}""");
		string expected = """{"title": "Let It Be"}""";

		async Task Act()
			=> await That(subject).DoesNotComplyWith(it => it.Matches(expected));

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             does not match expected,
			             but it did

			             Actual:
			             {
			               "title": "Let It Be"
			             }

			             JSON options:
			              - ignore additional properties
			             """);
	}

	[Test]
	public async Task NegatedJsonText_OnJsonElement_ShouldSucceedWhenDifferent()
	{
		JsonElement subject = Parse("""{"title": "Let It Be"}""");

		async Task Act()
			=> await That(subject).DoesNotComplyWith(it => it.Matches("""{"title": "Help!"}"""));

		await That(Act).DoesNotThrow();
	}

	[Test]
	public async Task NegatedJsonText_OnString_ShouldFailWhenMatching()
	{
		string subject = """{"title": "Let It Be"}""";
		string expected = """{"title": "Let It Be"}""";

		async Task Act()
			=> await That(subject).DoesNotComplyWith(it => it.IsValidJsonMatching(expected));

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             is not valid JSON which matches expected,
			             but it was "{\"title\": \"Let It Be\"}"

			             JSON options:
			              - ignore additional properties
			             """);
	}

	[Test]
	public async Task NullString_ShouldMatchJsonNull()
	{
		JsonElement subject = Parse("null");
		string? expected = null;

		async Task Act()
			=> await That(subject).Matches(expected);

		await That(Act).DoesNotThrow()
			.Because("a null string is the JSON null, like a null object");
	}

	[Test]
	public async Task ObjectHoldingAString_ShouldMatchAJsonString()
	{
		JsonElement subject = Parse("\"Let It Be\"");
		object expected = "Let It Be";

		async Task Act()
			=> await That(subject).Matches(expected);

		await That(Act).DoesNotThrow()
			.Because("a string typed as object binds to the overload for values");
	}

	[Test]
	public async Task StringVariable_ShouldBeParsed()
	{
		JsonElement subject = Parse("""{"title": "Let It Be"}""");
		string expected = """{"title": "Let It Be"}""";

		async Task Act()
			=> await That(subject).Matches(expected);

		await That(Act).DoesNotThrow();
	}

	public static IEnumerable<Func<JsonTextCall>> InvalidJsonTextCalls()
		=> JsonTextCalls("Let It Be");

	public static IEnumerable<Func<JsonTextCall>> MatchingJsonTextCalls()
		=> JsonTextCalls("""{"title": "Let It Be"}""");

	private static IEnumerable<Func<JsonTextCall>> JsonTextCalls(string json)
	{
		const string subject = """{"title": "Let It Be", "year": 1970}""";
		yield return () => new JsonTextCall("JsonElement.Matches",
			() => That(Parse(subject)).Matches(json));
		yield return () => new JsonTextCall("JsonElement.MatchesExactly",
			() => That(Parse(subject)).MatchesExactly(json, o => o.IgnoringAdditionalProperties()));
		yield return () => new JsonTextCall("JsonElement?.Matches",
			() => That((JsonElement?)Parse(subject)).Matches(json));
		yield return () => new JsonTextCall("JsonElement?.MatchesExactly",
			() => That((JsonElement?)Parse(subject)).MatchesExactly(json, o => o.IgnoringAdditionalProperties()));
		yield return () => new JsonTextCall("JsonNode.Matches",
			() => That(JsonNode.Parse(subject)).Matches(json));
		yield return () => new JsonTextCall("JsonNode.MatchesExactly",
			() => That(JsonNode.Parse(subject)).MatchesExactly(json, o => o.IgnoringAdditionalProperties()));
		yield return () => new JsonTextCall("string.IsValidJsonMatching",
			() => That(subject).IsValidJsonMatching(json));
		yield return () => new JsonTextCall("string.IsValidJsonMatchingExactly",
			() => That(subject).IsValidJsonMatchingExactly(json, o => o.IgnoringAdditionalProperties()));
	}

	private static JsonElement Parse(string json)
	{
		using JsonDocument document = JsonDocument.Parse(json);
		return document.RootElement.Clone();
	}

	public sealed class JsonTextCall(string name, Func<Expectation> write)
	{
		public Func<Expectation> Write { get; } = write;

		public override string ToString()
			=> name;
	}
}
