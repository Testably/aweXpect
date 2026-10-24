using System.Text.Json;
using System.Text.Json.Nodes;

namespace aweXpect.Json.Tests;

public sealed class NonNullableResultTests
{
	private const string Reason = "the awaited result is the verified subject, which cannot be null";

	[Test]
	public async Task JsonNodeIsArray_ShouldReturnNonNullableNode()
	{
		JsonNode? subject = new JsonArray(1);

		JsonNode result = await That(subject).IsArray();

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Test]
	public async Task JsonNodeIsArrayWithExpectations_ShouldReturnNonNullableNode()
	{
		JsonNode? subject = new JsonArray(1);

		JsonNode result = await That(subject).IsArray(a => a.At(0).Matching(1));

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Test]
	public async Task JsonNodeIsBoolean_ShouldReturnNonNullableNode()
	{
		JsonNode? subject = JsonValue.Create(true);

		JsonNode result = await That(subject).IsBoolean();

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Test]
	public async Task JsonNodeIsNumber_ShouldReturnNonNullableNode()
	{
		JsonNode? subject = JsonValue.Create(1);

		JsonNode result = await That(subject).IsNumber();

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Test]
	public async Task JsonNodeIsObject_ShouldReturnNonNullableNode()
	{
		JsonNode? subject = new JsonObject();

		JsonNode result = await That(subject).IsObject();

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Test]
	public async Task JsonNodeIsObjectWithExpectations_ShouldReturnNonNullableNode()
	{
		JsonNode? subject = new JsonObject { ["foo"] = new JsonArray(), };

		JsonNode result = await That(subject).IsObject(o => o.With("foo").AnArray());

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Test]
	public async Task JsonNodeIsString_ShouldReturnNonNullableNode()
	{
		JsonNode? subject = JsonValue.Create("foo");

		JsonNode result = await That(subject).IsString();

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Test]
	public async Task ObjectIsJsonSerializable_ShouldReturnNonNullableObject()
	{
		object? subject = new Poco { Value = 1, };

		object result = await That(subject).IsJsonSerializable();

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Test]
	public async Task ObjectIsJsonSerializableOfType_ShouldReturnNonNullableObject()
	{
		object? subject = new Poco { Value = 1, };

		object result = await That(subject).IsJsonSerializable<Poco>();

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Test]
	public async Task ObjectIsJsonSerializableOfTypeWithSerializerOptions_ShouldReturnNonNullableObject()
	{
		object? subject = new Poco { Value = 1, };

		object result = await That(subject).IsJsonSerializable<Poco>(new JsonSerializerOptions());

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Test]
	public async Task ObjectIsJsonSerializableWithSerializerOptions_ShouldReturnNonNullableObject()
	{
		object? subject = new Poco { Value = 1, };

		object result = await That(subject).IsJsonSerializable(new JsonSerializerOptions());

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Test]
	public async Task StringIsValidJsonMatching_ShouldReturnNonNullableString()
	{
		string? subject = "{\"foo\": 1}";

		string result = await That(subject).IsValidJsonMatching(new { foo = 1, });

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Test]
	public async Task StringIsValidJsonMatchingArray_ShouldReturnNonNullableString()
	{
		string? subject = "[1, 2]";

		string result = await That(subject).IsValidJsonMatching([1, 2,]);

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Test]
	public async Task StringIsValidJsonMatchingExactly_ShouldReturnNonNullableString()
	{
		string? subject = "{\"foo\": 1}";

		string result = await That(subject).IsValidJsonMatchingExactly(new { foo = 1, });

		await That(result).IsSameAs(subject).Because(Reason);
	}

	[Test]
	public async Task StringIsValidJsonMatchingExactlyArray_ShouldReturnNonNullableString()
	{
		string? subject = "[1, 2]";

		string result = await That(subject).IsValidJsonMatchingExactly([1, 2,]);

		await That(result).IsSameAs(subject).Because(Reason);
	}

	private sealed class Poco
	{
		public int Value { get; set; }
	}
}
