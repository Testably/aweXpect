using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading;
using aweXpect.Core;
using aweXpect.Core.EvaluationContext;
using aweXpect.Equivalency;
using aweXpect.Helpers;
using aweXpect.Results;

namespace aweXpect.Json;

/// <summary>
///     Serializes the <c>It.Is…</c> expectations of an expected object, which are then evaluated as part of the evaluation
///     with the <paramref name="context" /> and the <paramref name="cancellationToken" />.
/// </summary>
internal class ExpectationJsonConverter(IEvaluationContext context, CancellationToken cancellationToken)
	: JsonConverter<Expectation>
{
	private const string Prefix = "Expectation:::";

	private readonly Dictionary<Guid, Expectation> _expectations = new();

	public IEvaluationContext Context => context;

	public CancellationToken CancellationToken => cancellationToken;

	/// <summary>
	///     Parses the <paramref name="expected" /> value as JSON, in which this converter replaces the <c>It.Is…</c>
	///     expectations.
	/// </summary>
	/// <remarks>
	///     The expected value is usually an anonymous object, which only reflection can serialize.
	/// </remarks>
	public JsonDocument ParseExpected(object? expected, JsonDocumentOptions documentOptions)
	{
		if (TryParseWithoutSerializer(expected, documentOptions, out JsonDocument? document))
		{
			return document;
		}

		if (!ReflectionFallback.IsSupported)
		{
			throw Tracing.WriteException(ReflectionFallbackHelpers.SerializationNotSupported("The expected value"));
		}

#pragma warning disable CA1869
		JsonSerializerOptions serializerOptions = new(JsonSerializerOptions.Default);
#pragma warning restore CA1869
		serializerOptions.Converters.Add(this);
		return JsonDocument.Parse(JsonSerializer.Serialize(expected, serializerOptions), documentOptions);
	}

	/// <summary>
	///     Writes the <paramref name="expected" /> value without the serializer, when it consists only of values whose
	///     JSON does not depend on the serializer, so that it needs no reflection.
	/// </summary>
	private bool TryParseWithoutSerializer(object? expected, JsonDocumentOptions documentOptions,
		[NotNullWhen(true)] out JsonDocument? document)
	{
		using MemoryStream stream = new();
		using (Utf8JsonWriter writer = new(stream))
		{
			if (!TryWrite(writer, expected))
			{
				document = null;
				return false;
			}
		}

		document = JsonDocument.Parse(new ReadOnlyMemory<byte>(stream.GetBuffer(), 0, (int)stream.Length),
			documentOptions);
		return true;
	}

	/// <remarks>
	///     Writes like <see cref="JsonSerializer" /> with the default options. A <see cref="JsonValue" /> is only written
	///     when it wraps a supported value, because the serializer resolves the JSON of any other value anew from its
	///     options. Enums are not supported, because they can carry a <see cref="JsonConverterAttribute" />.
	/// </remarks>
	private bool TryWrite(Utf8JsonWriter writer, object? value)
	{
		switch (value)
		{
			case null:
				writer.WriteNullValue();
				return true;
			case string stringValue:
				writer.WriteStringValue(stringValue);
				return true;
			case char charValue:
				writer.WriteStringValue(charValue.ToString());
				return true;
			case bool boolValue:
				writer.WriteBooleanValue(boolValue);
				return true;
			case byte byteValue:
				writer.WriteNumberValue(byteValue);
				return true;
			case sbyte sbyteValue:
				writer.WriteNumberValue(sbyteValue);
				return true;
			case short shortValue:
				writer.WriteNumberValue(shortValue);
				return true;
			case ushort ushortValue:
				writer.WriteNumberValue(ushortValue);
				return true;
			case int intValue:
				writer.WriteNumberValue(intValue);
				return true;
			case uint uintValue:
				writer.WriteNumberValue(uintValue);
				return true;
			case long longValue:
				writer.WriteNumberValue(longValue);
				return true;
			case ulong ulongValue:
				writer.WriteNumberValue(ulongValue);
				return true;
			case float floatValue:
				writer.WriteNumberValue(floatValue);
				return true;
			case double doubleValue:
				writer.WriteNumberValue(doubleValue);
				return true;
			case decimal decimalValue:
				writer.WriteNumberValue(decimalValue);
				return true;
			case JsonElement element:
				element.WriteTo(writer);
				return true;
			case JsonDocument jsonDocument:
				jsonDocument.WriteTo(writer);
				return true;
			case JsonObject jsonObject:
				return TryWriteObject(writer, jsonObject);
			case JsonValue jsonValue when jsonValue.TryGetValue(out JsonElement element):
				element.WriteTo(writer);
				return true;
			case JsonValue jsonValue:
				return jsonValue.TryGetValue(out object? wrappedValue) && TryWrite(writer, wrappedValue);
			case Expectation expectation:
				WriteExpectation(writer, expectation);
				return true;
			case IEnumerable items when IsSerializedAsArray(items):
				return TryWriteArray(writer, items);
			default:
				return false;
		}
	}

	/// <remarks>
	///     The serializer writes a dictionary as an object, does not support multidimensional arrays and lets a
	///     <see cref="JsonConverterAttribute" /> replace the collection.
	/// </remarks>
	private static bool IsSerializedAsArray(IEnumerable items)
		=> items is not IDictionary and not Array { Rank: > 1, } &&
		   !Attribute.IsDefined(items.GetType(), typeof(JsonConverterAttribute));

	private bool TryWriteArray(Utf8JsonWriter writer, IEnumerable items)
	{
		writer.WriteStartArray();
		foreach (object? item in items)
		{
			if (!TryWrite(writer, item))
			{
				return false;
			}
		}

		writer.WriteEndArray();
		return true;
	}

	private bool TryWriteObject(Utf8JsonWriter writer, JsonObject jsonObject)
	{
		writer.WriteStartObject();
		foreach (KeyValuePair<string, JsonNode?> property in jsonObject)
		{
			writer.WritePropertyName(property.Key);
			if (!TryWrite(writer, property.Value))
			{
				return false;
			}
		}

		writer.WriteEndObject();
		return true;
	}

	public override bool CanConvert(Type typeToConvert)
		=> typeof(Expectation).IsAssignableFrom(typeToConvert);

	public override Expectation? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		Guid guid = Guid.Parse(reader.GetString()!);
		if (_expectations.TryGetValue(guid, out Expectation? expectation))
		{
			return expectation;
		}

		return default;
	}

	public override void Write(Utf8JsonWriter writer, Expectation value, JsonSerializerOptions options)
		=> WriteExpectation(writer, value);

	private void WriteExpectation(Utf8JsonWriter writer, Expectation value)
	{
		Guid guid = Guid.NewGuid();
		_expectations[guid] = value;
		writer.WriteStringValue(Prefix + guid);
	}

	public bool TryGetExpectation(JsonElement element,
		[NotNullWhen(true)] out EquivalencyExpectationBuilder? equivalencyExpectationBuilder)
	{
		if (element.ValueKind == JsonValueKind.String)
		{
			string? value = element.GetString();
			if (value?.StartsWith(Prefix) == true &&
#if NET8_0_OR_GREATER
			    Guid.TryParse(value.AsSpan(Prefix.Length), out Guid guid) &&
#else
			    Guid.TryParse(value.Substring(Prefix.Length), out Guid guid) &&
#endif
			    _expectations.TryGetValue(guid, out Expectation? expectation) &&
			    expectation is IOptionsProvider<ExpectationBuilder>
			    {
				    Options: EquivalencyExpectationBuilder builder,
			    })
			{
				equivalencyExpectationBuilder = builder;
				return true;
			}
		}

		equivalencyExpectationBuilder = null;
		return false;
	}
}
