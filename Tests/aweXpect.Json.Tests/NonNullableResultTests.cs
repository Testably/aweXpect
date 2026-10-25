using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization.Metadata;

namespace aweXpect.Json.Tests;

public sealed class NonNullableResultTests
{
	[Test]
	public async Task JsonNodeDoesNotHaveProperty_ShouldReturnNonNullableNode()
	{
		JsonNode? subject = new JsonObject();

		JsonNode result = await That(subject).DoesNotHaveProperty("foo");

		await That(result).IsSameAs(subject);
	}

	[Test]
	public async Task JsonNodeHasElementAt_ShouldReturnNonNullableNode()
	{
		JsonNode? subject = new JsonArray(1);

		JsonNode result = await That(subject).HasElementAt(0, e => e.IsNumber());

		await That(result).IsSameAs(subject);
	}

	[Test]
	public async Task JsonNodeHasProperty_ShouldReturnNonNullableNode()
	{
		JsonNode? subject = new JsonObject { ["foo"] = new JsonArray(), };

		JsonNode result = await That(subject).HasProperty("foo");

		await That(result).IsSameAs(subject);
	}

	[Test]
	public async Task JsonNodeHasPropertyWithExpectations_ShouldReturnNonNullableNode()
	{
		JsonNode? subject = new JsonObject { ["foo"] = new JsonArray(), };

		JsonNode result = await That(subject).HasProperty("foo", p => p.IsArray());

		await That(result).IsSameAs(subject);
	}

	[Test]
	public async Task JsonNodeHasValueAt_ShouldReturnNonNullableNode()
	{
		JsonNode? subject = new JsonObject { ["foo"] = new JsonArray(1), };

		JsonNode result = await That(subject).HasValueAt("foo[0]", v => v.IsNumber());

		await That(result).IsSameAs(subject);
	}

	[Test]
	public async Task JsonNodeIsArray_ShouldReturnNonNullableNode()
	{
		JsonNode? subject = new JsonArray(1);

		JsonNode result = await That(subject).IsArray();

		await That(result).IsSameAs(subject);
	}

	[Test]
	public async Task JsonNodeIsBoolean_ShouldReturnNonNullableNode()
	{
		JsonNode? subject = JsonValue.Create(true);

		JsonNode result = await That(subject).IsBoolean();

		await That(result).IsSameAs(subject);
	}

	[Test]
	public async Task JsonNodeIsBooleanWithExpectations_ShouldReturnNonNullableNode()
	{
		JsonNode? subject = JsonValue.Create(true);

		JsonNode result = await That(subject).IsBoolean(b => b.IsTrue());

		await That(result).IsSameAs(subject);
	}

	[Test]
	public async Task JsonNodeIsBooleanWithValue_ShouldReturnNonNullableNode()
	{
		JsonNode? subject = JsonValue.Create(true);

		JsonNode result = await That(subject).IsBoolean(true);

		await That(result).IsSameAs(subject);
	}

	[Test]
	public async Task JsonNodeIsNumber_ShouldReturnNonNullableNode()
	{
		JsonNode? subject = JsonValue.Create(1);

		JsonNode result = await That(subject).IsNumber();

		await That(result).IsSameAs(subject);
	}

	[Test]
	public async Task JsonNodeIsNumberOfType_ShouldReturnNonNullableNode()
	{
		JsonNode? subject = JsonValue.Create(1);

		JsonNode result = await That(subject).IsNumber<int>();

		await That(result).IsSameAs(subject);
	}

	[Test]
	public async Task JsonNodeIsNumberOfTypeWithExpectations_ShouldReturnNonNullableNode()
	{
		JsonNode? subject = JsonValue.Create(1);

		JsonNode result = await That(subject).IsNumber<int>(n => n.IsEqualTo(1));

		await That(result).IsSameAs(subject);
	}

	[Test]
	public async Task JsonNodeIsNumberWithExpectations_ShouldReturnNonNullableNode()
	{
		JsonNode? subject = JsonValue.Create(1);

		JsonNode result = await That(subject).IsNumber(n => n.IsEqualTo(1));

		await That(result).IsSameAs(subject);
	}

	[Test]
	public async Task JsonNodeIsNumberWithValue_ShouldReturnNonNullableNode()
	{
		JsonNode? subject = JsonValue.Create(1);

		JsonNode result = await That(subject).IsNumber(1).Within(0);

		await That(result).IsSameAs(subject);
	}

	[Test]
	public async Task JsonNodeIsObject_ShouldReturnNonNullableNode()
	{
		JsonNode? subject = new JsonObject();

		JsonNode result = await That(subject).IsObject();

		await That(result).IsSameAs(subject);
	}

	[Test]
	public async Task JsonNodeIsString_ShouldReturnNonNullableNode()
	{
		JsonNode? subject = JsonValue.Create("foo");

		JsonNode result = await That(subject).IsString();

		await That(result).IsSameAs(subject);
	}

	[Test]
	public async Task JsonNodeIsStringWithExpectations_ShouldReturnNonNullableNode()
	{
		JsonNode? subject = JsonValue.Create("foo");

		JsonNode result = await That(subject).IsString(s => s.IsEqualTo("foo"));

		await That(result).IsSameAs(subject);
	}

	[Test]
	public async Task JsonNodeIsStringWithValue_ShouldReturnNonNullableNode()
	{
		JsonNode? subject = JsonValue.Create("foo");

		JsonNode result = await That(subject).IsString("foo");

		await That(result).IsSameAs(subject);
	}

	[Test]
	public async Task ObjectIsJsonSerializable_ShouldReturnNonNullableObject()
	{
		object? subject = new Poco { Value = 1, };

		object result = await That(subject).IsJsonSerializable();

		await That(result).IsSameAs(subject);
	}

	[Test]
	public async Task ObjectIsJsonSerializableOfTypedSubject_ShouldReturnNonNullableSubjectType()
	{
		Poco? subject = new() { Value = 1, };

		Poco result = await That(subject).IsJsonSerializable();

		await That(result).IsSameAs(subject);
	}

	[Test]
	public async Task ObjectIsJsonSerializableOfTypedSubjectWithSerializerOptions_ShouldReturnNonNullableSubjectType()
	{
		Poco? subject = new() { Value = 1, };

		Poco result = await That(subject).IsJsonSerializable(new JsonSerializerOptions());

		await That(result).IsSameAs(subject);
	}

	[Test]
	public async Task ObjectIsJsonSerializableWithSerializerOptions_ShouldReturnNonNullableObject()
	{
		object? subject = new Poco { Value = 1, };

		object result = await That(subject).IsJsonSerializable(new JsonSerializerOptions());

		await That(result).IsSameAs(subject);
	}

	[Test]
	public async Task ObjectIsJsonSerializableWithTypeInfo_ShouldReturnNonNullableSubjectType()
	{
		Poco? subject = new() { Value = 1, };
		JsonSerializerOptions options = new()
		{
			TypeInfoResolver = new DefaultJsonTypeInfoResolver(),
		};

		Poco result = await That(subject).IsJsonSerializable((JsonTypeInfo<Poco>)options.GetTypeInfo(typeof(Poco)));

		await That(result).IsSameAs(subject);
	}

	[Test]
	public async Task StringIsValidJsonMatching_ShouldReturnNonNullableString()
	{
		string? subject = "{\"foo\": 1}";

		string result = await That(subject).IsValidJsonMatching(new { foo = 1, });

		await That(result).IsSameAs(subject);
	}

	[Test]
	public async Task StringIsValidJsonMatchingArray_ShouldReturnNonNullableString()
	{
		string? subject = "[1, 2]";

		string result = await That(subject).IsValidJsonMatching([1, 2,]);

		await That(result).IsSameAs(subject);
	}

	[Test]
	public async Task StringIsValidJsonMatchingExactly_ShouldReturnNonNullableString()
	{
		string? subject = "{\"foo\": 1}";

		string result = await That(subject).IsValidJsonMatchingExactly(new { foo = 1, });

		await That(result).IsSameAs(subject);
	}

	[Test]
	public async Task StringIsValidJsonMatchingExactlyArray_ShouldReturnNonNullableString()
	{
		string? subject = "[1, 2]";

		string result = await That(subject).IsValidJsonMatchingExactly([1, 2,]);

		await That(result).IsSameAs(subject);
	}

	private sealed class Poco
	{
		public int Value { get; set; }
	}
}
