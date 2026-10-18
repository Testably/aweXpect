using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using aweXpect.Equivalency;

namespace aweXpect.Json.Tests;

public sealed class ExpectedValueTests
{
	[Test]
	public async Task CollectionWithJsonConverterAttribute_ShouldBeSerializedWithTheConverter()
	{
		JsonElement subject = Parse("\"converted\"");

		async Task Act()
			=> await That(subject).MatchesExactly(new ConvertedCollection());

		await That(Act).DoesNotThrow()
			.Because("the serializer lets the converter replace the collection");
	}

	[Test]
	public async Task EmptyDictionary_ShouldBeSerializedAsObject()
	{
		JsonElement subject = Parse("{}");

		async Task Act()
			=> await That(subject).MatchesExactly(new Dictionary<string, int>());

		await That(Act).DoesNotThrow()
			.Because("the serializer writes a dictionary as an object, even without entries");
	}

	[Test]
	public async Task Enum_ShouldBeSerializedAsNumber()
	{
		JsonElement subject = Parse("[1]");

		async Task Act()
			=> await That(subject).MatchesExactly(new[] { Genre.Rock, });

		await That(Act).DoesNotThrow();
	}

	[Test]
	public async Task Expectation_InAnArray_ShouldBeEvaluated()
	{
		JsonElement subject = Parse("[1, 5]");

		async Task Act()
			=> await That(subject).MatchesExactly(new object[] { 1, It.Is<int>().That.IsGreaterThan(5), });

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             matches new object[] { 1, It.Is<int>().That.IsGreaterThan(5), } exactly,
			             but it differed as $[1] was 5
			             """);
	}

	[Test]
	[Arguments(double.NaN)]
	[Arguments(double.PositiveInfinity)]
	[Arguments(double.NegativeInfinity)]
	public async Task NonFiniteDouble_ShouldThrowLikeTheSerializer(double value)
	{
		JsonElement subject = Parse("0");

		void Serialize()
			=> JsonSerializer.Serialize(value);

		async Task Act()
			=> await That(subject).Matches(value);

		await That(Serialize).Throws<ArgumentException>()
			.WithMessage(NonFiniteNumberMessage);
		await That(Act).Throws<ArgumentException>()
			.WithMessage(NonFiniteNumberMessage);
	}

	[Test]
	[Arguments(float.NaN)]
	[Arguments(float.PositiveInfinity)]
	[Arguments(float.NegativeInfinity)]
	public async Task NonFiniteFloat_ShouldThrowLikeTheSerializer(float value)
	{
		JsonElement subject = Parse("0");

		void Serialize()
			=> JsonSerializer.Serialize(value);

		async Task Act()
			=> await That(subject).Matches(value);

		await That(Serialize).Throws<ArgumentException>()
			.WithMessage(NonFiniteNumberMessage);
		await That(Act).Throws<ArgumentException>()
			.WithMessage(NonFiniteNumberMessage);
	}

	[Test]
	[MethodDataSource(nameof(SupportedValues))]
	public async Task SupportedValue_ShouldBeWrittenLikeTheSerializer(ExpectedValue expectedValue)
	{
		object? value = expectedValue.Value;
		JsonElement serialized = JsonSerializer.SerializeToElement(value);
		JsonElement other = Parse("{\"other\": 0}");

		string? expectedMessage = await FailureMessage(other, serialized);
		string? message = await FailureMessage(other, value);
		string? messageForSerialized = await FailureMessage(serialized, value);

		await That(message).IsEqualTo(expectedMessage)
			.Because("the value must be written like the serializer writes it");
		await That(messageForSerialized).IsNull();
	}

	[Test]
	public async Task ValueWithTypeInfo_ShouldBeSerializedWithTheDefaultOptions()
	{
		JsonTypeInfo<Dto> typeInfo = (JsonTypeInfo<Dto>)new JsonSerializerOptions
		{
			PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
			TypeInfoResolver = new DefaultJsonTypeInfoResolver(),
		}.GetTypeInfo(typeof(Dto));
		JsonElement subject = Parse("{\"Name\": \"a\"}");

		async Task Act()
			=> await That(subject).MatchesExactly(JsonValue.Create(new Dto { Name = "a", }, typeInfo));

		await That(Act).DoesNotThrow()
			.Because("the serializer resolves the wrapped value anew from the default options");
	}

	private const string NonFiniteNumberMessage =
		".NET number values such as positive and negative infinity cannot be written as valid JSON. To make it work when using 'JsonSerializer', consider specifying 'JsonNumberHandling.AllowNamedFloatingPointLiterals' (see https://learn.microsoft.com/dotnet/api/system.text.json.serialization.jsonnumberhandling).";

	public static IEnumerable<Func<ExpectedValue>> SupportedValues()
	{
		yield return () => new ExpectedValue(null);
		yield return () => new ExpectedValue("foo");
		yield return () => new ExpectedValue("\"\\\n<>&'" + (char)0xE4 + (char)0x2028);
		yield return () => new ExpectedValue('a');
		yield return () => new ExpectedValue((char)0xE4);
		yield return () => new ExpectedValue(true);
		yield return () => new ExpectedValue(false);
		yield return () => new ExpectedValue(byte.MaxValue);
		yield return () => new ExpectedValue(sbyte.MinValue);
		yield return () => new ExpectedValue(short.MinValue);
		yield return () => new ExpectedValue(ushort.MaxValue);
		yield return () => new ExpectedValue(int.MinValue);
		yield return () => new ExpectedValue(uint.MaxValue);
		yield return () => new ExpectedValue(long.MinValue);
		yield return () => new ExpectedValue(ulong.MaxValue);
		yield return () => new ExpectedValue(0.1f);
		yield return () => new ExpectedValue(float.MaxValue);
		yield return () => new ExpectedValue(-0.0f);
		yield return () => new ExpectedValue(0.1);
		yield return () => new ExpectedValue(42.1);
		yield return () => new ExpectedValue(double.MaxValue);
		yield return () => new ExpectedValue(double.Epsilon);
		yield return () => new ExpectedValue(1e20);
		yield return () => new ExpectedValue(1.10m);
		yield return () => new ExpectedValue(decimal.MinValue);
		yield return () => new ExpectedValue(Parse("{\"a\": [1, 2.50, \"b\", null, true]}"));
		yield return () => new ExpectedValue(JsonDocument.Parse("[1, {\"a\": 2}]"));
		yield return () => new ExpectedValue(JsonNode.Parse("{\"a\": [1, 2.50, \"b\", null, true]}"));
		yield return () => new ExpectedValue(JsonSerializer.Deserialize<JsonNode>("1.50"));
		yield return () => new ExpectedValue(new JsonObject { ["a"] = 1, ["b"] = null, ["c"] = new JsonArray(1.5m, "x", 'y', false), });
		yield return () => new ExpectedValue(JsonValue.Create(Parse("\"a\"")));
		yield return () => new ExpectedValue(new[] { 1, 2, });
		yield return () => new ExpectedValue(Array.Empty<string>());
		yield return () => new ExpectedValue(new object?[] { 1, "a", null, new[] { true, }, new List<double> { 0.1, }, });
		yield return () => new ExpectedValue(Enumerable.Range(1, 3));
		yield return () => new ExpectedValue(new List<JsonNode?> { 1, null, new JsonObject(), });
	}

	private static async Task<string?> FailureMessage(JsonElement subject, object? expected)
		=> (await Catch.ExceptionAsync(async () => await That(subject).MatchesExactly(expected)))?.Message;

	private static JsonElement Parse(string json)
	{
		using JsonDocument document = JsonDocument.Parse(json);
		return document.RootElement.Clone();
	}

	[JsonConverter(typeof(ConvertedCollectionConverter))]
	private sealed class ConvertedCollection : List<int>;

	private sealed class ConvertedCollectionConverter : JsonConverter<ConvertedCollection>
	{
		public override ConvertedCollection Read(ref Utf8JsonReader reader, Type typeToConvert,
			JsonSerializerOptions options)
			=> throw new NotSupportedException();

		public override void Write(Utf8JsonWriter writer, ConvertedCollection value, JsonSerializerOptions options)
			=> writer.WriteStringValue("converted");
	}

	public sealed class ExpectedValue(object? value)
	{
		public object? Value { get; } = value;

		public override string ToString()
			=> Value?.GetType().Name ?? "null";
	}

	private sealed class Dto
	{
		public string? Name { get; set; }
	}

	private enum Genre
	{
		Pop = 0,
		Rock = 1,
	}
}
