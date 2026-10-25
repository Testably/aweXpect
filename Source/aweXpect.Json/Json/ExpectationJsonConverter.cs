using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using System.Threading;
using aweXpect.Core;
using aweXpect.Core.EvaluationContext;
using aweXpect.Core.Extending;
using aweXpect.Equivalency;
using aweXpect.Helpers;
using aweXpect.Results;

namespace aweXpect.Json;

/// <summary>
///     Serializes the <c>It.Is…</c> expectations of an expected object, which are then evaluated as part of the evaluation
///     with the <paramref name="context" /> and the <paramref name="cancellationToken" />.
/// </summary>
internal class ExpectationJsonConverter(IEvaluationContext context, CancellationToken cancellationToken)
{
	private const string Prefix = "Expectation:::";

	/// <remarks>
	///     The serializer caches its expensive metadata per set of equal options, which a converter instance per call
	///     would defeat. So the converters in the options are shared and write into the converter of the serialization
	///     that currently runs on this thread, as the serialization is synchronous.
	/// </remarks>
	[ThreadStatic] private static ExpectationJsonConverter? _current;

	private static JsonSerializerOptions? _defaultOptions;

	private Dictionary<Guid, Expectation>? _expectations;

	public IEvaluationContext Context => context;

	public CancellationToken CancellationToken => cancellationToken;

	/// <summary>
	///     Parses the <paramref name="expected" /> value as JSON, in which this converter replaces the <c>It.Is…</c>
	///     expectations.
	/// </summary>
	/// <remarks>
	///     The expected value is usually an anonymous object, which only reflection can serialize, unless the
	///     <paramref name="serializerOptions" /> have a <see cref="JsonSerializerOptions.TypeInfoResolver" />.
	///     Without <paramref name="serializerOptions" />, it is serialized with the default options.
	/// </remarks>
	public JsonDocument ParseExpected(object? expected, JsonDocumentOptions documentOptions,
		JsonSerializerOptions? serializerOptions)
	{
		if (serializerOptions is null &&
		    TryParseWithoutSerializer(expected, documentOptions, out JsonDocument? document))
		{
			return document;
		}

		if (serializerOptions?.TypeInfoResolver is not null)
		{
			JsonSerializerOptions resolverOptions = new(serializerOptions);
			resolverOptions.Converters.Add(ExpectationConverter.Instance);
			resolverOptions.Converters.Add(ObjectConverter.Instance);
			return ParseWithResolver(expected, documentOptions, resolverOptions);
		}

		if (!ReflectionFallback.IsSupported)
		{
			throw Tracing.WriteException(
				ReflectionFallback.NotSupported("The members to serialize the expected value as JSON"));
		}

		JsonSerializerOptions options = serializerOptions is null
			? GetDefaultReflectionOptions(JsonSerializerOptions.Default)
			: CreateReflectionOptions(serializerOptions);
		byte[] json;
		ExpectationJsonConverter? previous = SetCurrent(this);
		try
		{
			json = JsonSerializer.SerializeToUtf8Bytes(expected, options);
		}
		finally
		{
			SetCurrent(previous);
		}

		return JsonDocument.Parse(json, documentOptions);
	}

	private static JsonSerializerOptions GetDefaultReflectionOptions(JsonSerializerOptions defaultOptions)
		=> _defaultOptions ??= CreateReflectionOptions(defaultOptions);

	private static JsonSerializerOptions CreateReflectionOptions(JsonSerializerOptions serializerOptions)
	{
		JsonSerializerOptions options = new(serializerOptions);
		options.Converters.Add(ExpectationConverter.Instance);
		return options;
	}

	/// <summary>
	///     Sets the <paramref name="converter" /> of the serialization on this thread and returns the previous one.
	/// </summary>
	private static ExpectationJsonConverter? SetCurrent(ExpectationJsonConverter? converter)
	{
		ExpectationJsonConverter? previous = _current;
		_current = converter;
		return previous;
	}

	private JsonDocument ParseWithResolver(object? expected, JsonDocumentOptions documentOptions,
		JsonSerializerOptions options)
	{
		using MemoryStream stream = new();
		ExpectationJsonConverter? previous = SetCurrent(this);
		try
		{
			using Utf8JsonWriter writer = new(stream);
			WriteWithResolver(writer, expected, options);
		}
		finally
		{
			SetCurrent(previous);
		}

		return JsonDocument.Parse(new ReadOnlyMemory<byte>(stream.GetBuffer(), 0, (int)stream.Length),
			documentOptions);
	}

	/// <remarks>
	///     Resolves the type info of the runtime type like the serializer does for an <see langword="object" />, but writes
	///     <c>It.Is…</c> expectations itself, because a source-generated resolver knows none of their types.
	/// </remarks>
	private void WriteWithResolver(Utf8JsonWriter writer, object? value, JsonSerializerOptions options)
	{
		switch (value)
		{
			case null:
				writer.WriteNullValue();
				return;
			case Expectation expectation:
				WriteExpectation(writer, expectation);
				return;
		}

		Type type = value.GetType();
		if (type == typeof(object))
		{
			writer.WriteStartObject();
			writer.WriteEndObject();
			return;
		}

		if (!options.TryGetTypeInfo(type, out JsonTypeInfo? typeInfo))
		{
			throw Tracing.WriteException(new NotSupportedException(
				$"The expected value cannot be serialized as JSON, because the TypeInfoResolver of the serializer options provides no metadata for {Formatter.Format(type)}. Add [JsonSerializable(typeof({Formatter.Format(type)}))] to the JsonSerializerContext."));
		}

		JsonSerializer.Serialize(writer, value, typeInfo);
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

	private void WriteExpectation(Utf8JsonWriter writer, Expectation value)
	{
		Guid guid = Guid.NewGuid();
		(_expectations ??= new Dictionary<Guid, Expectation>())[guid] = value;
		writer.WriteStringValue(Prefix + guid);
	}

	/// <summary>
	///     Replaces the placeholders of the <c>It.Is…</c> expectations in the <paramref name="formattedValue" /> with
	///     their descriptions.
	/// </summary>
	/// <remarks>
	///     An expectation describes its constraints only once it was evaluated, otherwise just its type.
	/// </remarks>
	public string DescribeExpectations(string formattedValue)
	{
		if (_expectations is null || formattedValue.IndexOf(Prefix, StringComparison.Ordinal) < 0)
		{
			return formattedValue;
		}

		StringBuilder sb = new(formattedValue);
		foreach (KeyValuePair<Guid, Expectation> expectation in _expectations)
		{
			sb.Replace("\"" + Prefix + expectation.Key + "\"", expectation.Value.ToString());
		}

		return sb.ToString();
	}

	public bool TryGetExpectation(JsonElement element,
		[NotNullWhen(true)] out EquivalencyExpectationBuilder? equivalencyExpectationBuilder)
	{
		if (_expectations is not null && element.ValueKind == JsonValueKind.String)
		{
			string? value = element.GetString();
			if (value?.StartsWith(Prefix, StringComparison.Ordinal) == true &&
#if NET8_0_OR_GREATER
			    Guid.TryParse(value.AsSpan(Prefix.Length), out Guid guid) &&
#else
			    Guid.TryParse(value.Substring(Prefix.Length), out Guid guid) &&
#endif
			    _expectations.TryGetValue(guid, out Expectation? expectation) &&
			    EquivalencyExpectations.TryGetBuilder(expectation, out equivalencyExpectationBuilder))
			{
				return true;
			}
		}

		equivalencyExpectationBuilder = null;
		return false;
	}

	/// <summary>
	///     Writes the values of <see langword="object" /> members like the serializer, but finds the <c>It.Is…</c>
	///     expectations among them, which a source-generated resolver cannot resolve.
	/// </summary>
	private sealed class ObjectConverter : JsonConverter<object>
	{
		public static readonly ObjectConverter Instance = new();

		public override bool CanConvert(Type typeToConvert)
			=> typeToConvert == typeof(object);

		public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
			=> JsonElement.ParseValue(ref reader);

		public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
			=> _current!.WriteWithResolver(writer, value, options);
	}

	/// <summary>
	///     Writes the <c>It.Is…</c> expectations as placeholders, which the comparison replaces with the expectation.
	/// </summary>
	private sealed class ExpectationConverter : JsonConverter<Expectation>
	{
		public static readonly ExpectationConverter Instance = new();

		public override bool CanConvert(Type typeToConvert)
			=> typeof(Expectation).IsAssignableFrom(typeToConvert);

		public override Expectation? Read(ref Utf8JsonReader reader, Type typeToConvert,
			JsonSerializerOptions options)
		{
			Guid guid = Guid.Parse(reader.GetString()!);
			if (_current?._expectations?.TryGetValue(guid, out Expectation? expectation) == true)
			{
				return expectation;
			}

			return default;
		}

		public override void Write(Utf8JsonWriter writer, Expectation value, JsonSerializerOptions options)
			=> _current!.WriteExpectation(writer, value);
	}
}
