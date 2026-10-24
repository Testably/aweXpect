using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using aweXpect.Equivalency;
using aweXpect.Formatting;
using static aweXpect.Expect;
using static aweXpect.Formatting.Format;

namespace aweXpect.Json.Aot;

internal static partial class Checks
{
	private const string SwitchName = "aweXpect.ReflectionFallback.IsSupported";

	public static readonly Check[] All =
	[
		new("a valid JSON string passes",
			() => ShouldPass(async () => await That("{\"a\": [1, 2]}").IsValidJson())),
		new("an invalid JSON string fails and names the parse error",
			() => ShouldFail(async () => await That("{\"a\":").IsValidJson(), "is valid JSON", "could not be parsed")),
		new("an equivalent JSON string passes",
			() => ShouldPass(async () => await That("{\"a\":[1,2]}").IsEqualTo("{ \"a\": [1, 2] }").AsJson())),
		new("a differing JSON string fails and names the difference",
			() => ShouldFail(async () => await That("{\"a\":[1,2]}").IsEqualTo("{\"a\":[1,3]}").AsJson(),
				"$.a[1]")),
		new("an object with the expected structure passes",
			() => ShouldPass(async () => await That(Parse("{\"a\": [1, 2], \"b\": {}}"))
				.IsObject(o => o.With(2).Properties()
					.And.With("a").AnArray(a => a.With(2).Elements())
					.And.With("b").AnObject()))),
		new("an array with a differing length fails and names it",
			() => ShouldFail(async () => await That(Parse("[1, 2]")).IsArray(a => a.With(3).Elements()),
				"$ had 2 elements")),
		new("the elements of an array fail with their JSON representation",
			() => ShouldFail(async () => await That(Parse("[\"a\", null]")).IsArray().WhoseElements
					.All().Satisfy(e => e.ValueKind == JsonValueKind.String),
				"whose elements satisfy", "  \"a\",", "  null")),
		new("the properties of an object fail with their JSON representation",
			() => ShouldFail(async () => await That(Parse("{\"a\": \"x\", \"b\": [1, null]}")).IsObject().WhoseProperties
					.All().Satisfy(p => p.Value.ValueKind == JsonValueKind.String),
				"whose properties satisfy", "  \"a\":\"x\",", "  \"b\":[1,null]")),
		new("the root element of a valid JSON string can be verified",
			() => ShouldFail(async () => await That("[1]").IsValidJson(e => e.IsObject()),
				"is an object")),
		new("an element is formatted on a single line",
			() => ShouldEqual(Formatter.Format(Parse("{ \"a\": [1, \"\\u00e4\"] }")), "{\"a\":[1,\"\\u00E4\"]}")),
		new("an element is formatted with line breaks",
			() => ShouldEqual(Formatter.Format(Parse("{\"a\":1}"), FormattingOptions.MultipleLines),
				$"{{{Environment.NewLine}  \"a\": 1{Environment.NewLine}}}")),
		new("an element matching an expected object fails loudly",
			() => ShouldFailLoudly(async () => await That(Parse("{\"a\": [1, 2]}")).Matches(new
			{
				a = new[] { 1, 2, },
			}))),
		new("an element differing from an expected object fails loudly under negation",
			() => ShouldFailLoudly(async () => await That(Parse("{\"a\": 1}")).DoesNotComplyWith(it => it
				.MatchesExactly(new
				{
					a = 2,
				})))),
		new("a nullable element compared with an expected object fails loudly",
			() => ShouldFailLoudly(async () => await That((JsonElement?)Parse("{\"a\": 1}")).Matches(new
			{
				a = 1,
			}))),
		new("a JSON string compared with an expected object fails loudly",
			() => ShouldFailLoudly(async () => await That("{\"a\": 1}").IsValidJsonMatching(new
			{
				a = 1,
			}))),
		new("an element matching an expected node passes",
			() => ShouldPass(async () => await That(Parse("{\"a\": [1, 2]}"))
				.Matches(new JsonObject { ["a"] = new JsonArray(1, 2), }))),
		new("a property compared with an expected value passes",
			() => ShouldPass(async () => await That(Parse("{\"a\": 1}"))
				.IsObject(o => o.With("a").Matching(1)))),
		new("a property compared with an It.Is expectation fails and names it",
			() => ShouldFail(async () => await That(Parse("{\"a\": 1}"))
					.IsObject(o => o.With("a").Matching(It.Is<int>().That.IsGreaterThan(1))),
				"$.a was 1")),
		new("array elements compared with expected values pass",
			() => ShouldPass(async () => await That(Parse("[1, 2]"))
				.IsArray(a => a.WithElements(1, 2)))),
		new("a node with the expected structure passes",
			() => ShouldPass(async () => await That(JsonNode.Parse("{\"a\": [1, 2]}"))
				.IsObject(o => o.With("a").AnArray(a => a.With(2).Elements())))),
		new("a null node fails as the JSON null",
			() => ShouldFail(async () => await That((JsonNode?)null).IsObject(), "is an object", "but it was null")),
		new("the elements of a node array fail with their JSON representation",
			() => ShouldFail(async () => await That(new JsonArray("a", null)).IsArray().WhoseElements
					.All().Satisfy(e => e.ValueKind == JsonValueKind.Number),
				"whose elements satisfy", "  \"a\",", "  null")),
		new("the properties of a node object fail with their JSON representation",
			() => ShouldFail(async () => await That(new JsonObject { ["a"] = 1, }).IsObject().WhoseProperties
					.All().Satisfy(p => p.Value.ValueKind == JsonValueKind.String),
				"whose properties satisfy", "  \"a\":1")),
		new("a node matching an expected object fails loudly",
			() => ShouldFailLoudly(async () => await That(new JsonObject { ["a"] = 1, }).Matches(new
			{
				a = 1,
			}))),
		new("a serializable object fails loudly",
			() => ShouldFailLoudly(async () => await That((object)new Dto { Name = "a", })
				.IsJsonSerializable())),
		new("a serializable object fails loudly under negation",
			() => ShouldFailLoudly(async () => await That((object)new Dto { Name = "a", })
				.DoesNotComplyWith(it => it.IsJsonSerializable<Dto>()))),
		new("an object serialized with its source-generated type info passes",
			() => ShouldPass(async () => await That(new Dto { Name = "a", })
				.IsJsonSerializedAs("{\"Name\": \"a\"}", DtoContext.Default.Dto))),
		new("an object serialized differently with its source-generated type info fails and names the difference",
			() => ShouldFail(async () => await That(new Dto { Name = "a", })
					.IsJsonSerializedAs("{\"Name\": \"b\"}", DtoContext.Default.Dto),
				"$.Name", "Serialized:", "{\"Name\":\"a\"}")),
		new("an object serialized without type info fails loudly",
			() => ShouldFailLoudly(async () => await That(new Dto { Name = "a", })
				.IsJsonSerializedAs("{\"Name\": \"a\"}"))),
		new("an element matching an expected object with source-generated options passes",
			() => ShouldPass(async () => await That(Parse("{\"Name\": \"a\"}"))
				.Matches(new Dto { Name = "a", }, o => o.WithSerializerOptions(DtoContext.Default.Options)))),
		new("an It.Is expectation in an object member with source-generated options fails and names it",
			() => ShouldFail(async () => await That(Parse("{\"Title\": \"a\", \"Rating\": 1}"))
					.Matches(new Track { Title = "a", Rating = It.Is<int>().That.IsGreaterThan(1), },
						o => o.WithSerializerOptions(DtoContext.Default.Options)),
				"$.Rating was 1")),
		new("a property compared with a value of source-generated options passes",
			() => ShouldPass(async () => await That(Parse("{\"a\": {\"Name\": \"a\"}}"))
				.IsObject(o => o.With("a").Matching(new Dto { Name = "a", }),
					o => o.WithSerializerOptions(DtoContext.Default.Options)))),
		new("an expected value missing from the source-generated context names the fix",
			() => ShouldThrowNotSupported(async () => await That(Parse("{}"))
					.Matches(new Track { Rating = new Dto[0], }, o => o.WithSerializerOptions(DtoContext.Default.Options)),
				"Add [JsonSerializable(typeof(Checks.Dto[]))]")),
	];

	private static JsonElement Parse(string json)
	{
		using JsonDocument document = JsonDocument.Parse(json);
		return document.RootElement.Clone();
	}

	private static Task<string?> ShouldEqual(string actual, string expected)
		=> Task.FromResult(actual == expected ? null : $"was {actual} instead of {expected}");

	private static async Task<string?> ShouldPass(Func<Task> act)
	{
		try
		{
			await act();
			return null;
		}
		catch (Exception exception)
		{
			return $"threw {exception.GetType().FullName}: {exception.Message}";
		}
	}

	/// <summary>
	///     Expects the failure exception whose message contains every part.
	/// </summary>
	private static async Task<string?> ShouldFail(Func<Task> act, params string[] parts)
	{
		try
		{
			await act();
		}
		catch (FailException exception)
		{
			string? missing = Array.Find(parts, part => !exception.Message.Contains(part, StringComparison.Ordinal));
			return missing is null ? null : $"message lacks \"{missing}\": {exception.Message}";
		}
		catch (Exception exception)
		{
			return $"threw {exception.GetType().FullName} instead of {typeof(FailException).FullName}: {exception.Message}";
		}

		return "did not throw";
	}

	/// <summary>
	///     Expects the error naming the switch, whatever the outcome with reflection would be.
	/// </summary>
	/// <remarks>
	///     Both runs have the reflection fallback switched off, because the project is published with Native AOT, so a
	///     pass or an ordinary failure means that reflection-based serialization was reached or its error swallowed.
	/// </remarks>
	private static async Task<string?> ShouldFailLoudly(Func<Task> act)
	{
		try
		{
			await act();
		}
		catch (NotSupportedException exception) when (exception.Message.Contains(SwitchName, StringComparison.Ordinal))
		{
			return null;
		}
		catch (Exception exception)
		{
			return $"threw {exception.GetType().FullName} without naming the switch: {exception.Message}";
		}

		return "did not throw";
	}

	/// <summary>
	///     Expects the <see cref="NotSupportedException" /> whose message contains the <paramref name="part" />.
	/// </summary>
	private static async Task<string?> ShouldThrowNotSupported(Func<Task> act, string part)
	{
		try
		{
			await act();
		}
		catch (NotSupportedException exception) when (exception.Message.Contains(part, StringComparison.Ordinal))
		{
			return null;
		}
		catch (Exception exception)
		{
			return $"threw {exception.GetType().FullName} without \"{part}\": {exception.Message}";
		}

		return "did not throw";
	}

	private sealed class Dto
	{
		public string? Name { get; set; }
	}

	private sealed class Track
	{
		public string? Title { get; set; }
		public object? Rating { get; set; }
	}

	[JsonSerializable(typeof(Dto))]
	[JsonSerializable(typeof(Track))]
	[JsonSerializable(typeof(int))]
	private sealed partial class DtoContext : JsonSerializerContext;
}
