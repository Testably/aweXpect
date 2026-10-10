using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
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
