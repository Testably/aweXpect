using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
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
		new("a valid JSON string fails the negation and names the subject",
			() => ShouldFail(async () => await That("[1, 2]").IsNotValidJson(), "is not valid JSON",
				"but it was \"[1, 2]\"")),
		new("an equivalent JSON string passes",
			() => ShouldPass(async () => await That("{\"a\":[1,2]}").IsEqualTo("{ \"a\": [1, 2] }").AsJson())),
		new("a differing JSON string fails and names the difference",
			() => ShouldFail(async () => await That("{\"a\":[1,2]}").IsEqualTo("{\"a\":[1,3]}").AsJson(),
				"$.a[1]")),
		new("an object with the expected structure passes",
			() => ShouldPass(async () => await That(Parse("{\"a\": [1, 2], \"b\": {}}"))
				.HasProperty("a", a => a.IsArray().WhoseElements.HasCount(2))
				.And.HasProperty("b", b => b.IsObject())
				.And.DoesNotHaveProperty("c"))),
		new("a missing property fails and names its path",
			() => ShouldFail(async () => await That(Parse("{\"a\": {}}")).HasValueAt("a.b", b => b.IsString()),
				"property $.a.b did not exist")),
		new("a failed lookup shows the indented JSON it ran on",
			() => ShouldFail(async () => await That(Parse("{\"a\": {\"c\": 1}}")).HasProperty("a", a => a.HasProperty("b")),
				"property $.a.b did not exist", "Actual ($.a):", "  \"c\": 1")),
		new("a number is read as the requested type",
			() => ShouldPass(async () => await That(Parse("{\"a\": 9.5}"))
				.HasProperty("a", a => a.IsNumber<decimal>(n => n.IsEqualTo(9.5m)))
				.And.HasProperty("a", a => a.IsNumber(n => n.IsGreaterThan(9))))),
		new("a number that does not fit the requested type fails and names it",
			() => ShouldFail(async () => await That(Parse("[1.5]")).HasElementAt(0, e => e.IsNumber<int>()),
				"$[0] was 1.5, which is not an int")),
		new("values compared with expected values pass",
			() => ShouldPass(async () => await That(Parse("{\"a\": \"Let It Be\", \"b\": 9.5, \"c\": true}"))
				.HasProperty("a", a => a.IsString("let").AsPrefix().IgnoringCase())
				.And.HasProperty("b", b => b.IsNumber(9.4m).Within(0.1m))
				.And.HasProperty("c", c => c.IsBoolean(true)))),
		new("values compared with operators pass",
			() => ShouldPass(async () => await That(Parse("{\"a\": \"Let It Be\", \"b\": 9.5}"))
				.HasProperty("a", a => a.IsString().StartingWith("let").IgnoringCase())
				.And.HasProperty("b", b => b.IsNumber().EqualTo(9.4).Within(0.1)))),
		new("a value compared with an operator fails in one sentence",
			() => ShouldFail(async () => await That(Parse("{\"a\": 3}")).HasProperty("a", a => a.IsNumber().GreaterThan(4)),
				"has property \"a\" which is a number greater than 4.0", "$.a was 3")),
		new("a number compared with an expected value of another type fails and names it",
			() => ShouldFail(async () => await That(Parse("[1.5]")).HasElementAt(0, e => e.IsNumber(2)),
				"$[0] was 1.5, which is not an int")),
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
		new("an element matching expected JSON text passes",
			() => ShouldPass(async () => await That(Parse("{\"a\": [1, 2]}")).Matches("{\"a\": [1, 2]}"))),
		new("a JSON string matching expected JSON text passes",
			() => ShouldPass(async () => await That("{\"a\": 1}").IsValidJsonMatchingExactly("{\"a\": 1}"))),
		new("an element matching an expected node passes",
			() => ShouldPass(async () => await That(Parse("{\"a\": [1, 2]}"))
				.Matches(new JsonObject { ["a"] = new JsonArray(1, 2), }))),
		new("a property compared with an expected value passes",
			() => ShouldPass(async () => await That(Parse("{\"a\": 1}"))
				.HasProperty("a", a => a.Matches(1)))),
		new("a property compared with an It.Is expectation fails and names it",
			() => ShouldFail(async () => await That(Parse("{\"a\": 1}"))
					.HasProperty("a", a => a.Matches(It.Is<int>().That.IsGreaterThan(1))),
				"$.a was 1")),
		new("array elements compared with expected values pass",
			() => ShouldPass(async () => await That(Parse("[1, 2]"))
				.Matches(new[] { 1, 2, }))),
		new("a node with the expected structure passes",
			() => ShouldPass(async () => await That(JsonNode.Parse("{\"a\": [1, 2]}"))
				.HasProperty("a", a => a.HasElementAt(1, e => e.IsNumber<int>(n => n.IsEqualTo(2)))))),
		new("a null node fails as the JSON null",
			() => ShouldFail(async () => await That((JsonNode?)null).IsObject(), "is an object", "but it was null")),
		new("the elements of a node array fail with their JSON representation",
			() => ShouldFail(async () => await That(new JsonArray("a", null)).IsArray().WhoseElements
					.All().Satisfy(e => e?.GetValueKind() == JsonValueKind.Number),
				"whose elements satisfy", "  \"a\",", "  <null>")),
		new("the properties of a node object fail with their JSON representation",
			() => ShouldFail(async () => await That(new JsonObject { ["a"] = 1, }).IsObject().WhoseProperties
					.All().Satisfy(p => p.Value?.GetValueKind() == JsonValueKind.String),
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
			() => ShouldFailLoudly(async () => await That(new Dto { Name = "a", })
				.DoesNotComplyWith(it => it.IsJsonSerializable()))),
		new("an object round-tripped with its source-generated type info passes",
			() => ShouldPass(async () => await That(new Dto { Name = "a", })
				.IsJsonSerializable(DtoContext.Default.Dto))),
		new("an object losing a member in a round trip with its source-generated type info fails and names it",
			() => ShouldFail(async () =>
				{
					JsonTypeInfo<DtoWithIgnoredMember> typeInfo = DtoContext.Default.DtoWithIgnoredMember;
					await That(new DtoWithIgnoredMember { Name = "a", Secret = "b", }).IsJsonSerializable(typeInfo);
				},
				"is serializable as JSON", "Property Secret differed")),
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
				.HasProperty("a", a => a.Matches(new Dto { Name = "a", },
					o => o.WithSerializerOptions(DtoContext.Default.Options))))),
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

	internal sealed class Dto
	{
		public string? Name { get; set; }
	}

	internal sealed class DtoWithIgnoredMember
	{
		public string? Name { get; set; }
		[JsonIgnore] public string? Secret { get; set; }
	}

	private sealed class Track
	{
		public string? Title { get; set; }
		public object? Rating { get; set; }
	}

	[JsonSerializable(typeof(Dto))]
	[JsonSerializable(typeof(DtoWithIgnoredMember))]
	[JsonSerializable(typeof(Track))]
	[JsonSerializable(typeof(int))]
	private sealed partial class DtoContext : JsonSerializerContext;
}
