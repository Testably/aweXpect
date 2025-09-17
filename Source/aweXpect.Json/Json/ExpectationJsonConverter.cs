using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using aweXpect.Core;
using aweXpect.Equivalency;
using aweXpect.Results;

namespace aweXpect.Json;

internal class ExpectationJsonConverter : JsonConverter<Expectation>
{
	private const string Prefix = "Expectation:::";

	private readonly Dictionary<Guid, Expectation> _expectations = new();

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
			    Guid.TryParse(value[Prefix.Length..], out Guid guid) &&
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
