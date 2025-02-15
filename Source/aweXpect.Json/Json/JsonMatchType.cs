#if NET8_0_OR_GREATER
using System.Collections.Generic;
using System.Text.Json;
using aweXpect.Core;

namespace aweXpect.Json;

internal sealed class JsonMatchType(JsonOptions options) : IStringMatchType
{
	private JsonElementValidator.JsonComparisonResult? _comparisonResult;
	private string? _deserializationError;

	/// <inheritdoc
	///     cref="IStringMatchType.GetExtendedFailure(string, string?, string?, bool, IEqualityComparer{string}, StringDifferenceSettings?)" />
	public string GetExtendedFailure(
		string it,
		string? actual,
		string? expected,
		bool ignoreCase,
		IEqualityComparer<string> comparer,
		StringDifferenceSettings? settings)
	{
		if (_deserializationError != null)
		{
			return _deserializationError;
		}

		string? result = _comparisonResult?.ToString();
		if (!string.IsNullOrEmpty(result))
		{
			return $"{it} differed as{result}";
		}

		return "";
	}

	/// <inheritdoc cref="IStringMatchType.AreConsideredEqual(string?, string?, bool, IEqualityComparer{string})" />
	public bool AreConsideredEqual(
		string? actual,
		string? expected,
		bool ignoreCase,
		IEqualityComparer<string> comparer)
	{
		if (actual == null && expected == null)
		{
			return true;
		}

		if (actual == null || expected == null)
		{
			return false;
		}

		try
		{
			using JsonDocument expectedJson = JsonDocument.Parse(expected, options.DocumentOptions);
			try
			{
				using JsonDocument actualJson = JsonDocument.Parse(actual, options.DocumentOptions);

				_comparisonResult = JsonElementValidator.Compare(
					actualJson.RootElement,
					expectedJson.RootElement,
					options);
				return !_comparisonResult.HasError;
			}
			catch (JsonException e)
			{
				_deserializationError = "could not parse subject: " + e.Message;
			}
		}
		catch (JsonException e)
		{
			_deserializationError = "could not parse expected: " + e.Message;
		}

		return false;
	}

	/// <inheritdoc cref="IStringMatchType.GetExpectation(string?, ExpectationGrammars)" />
	public string GetExpectation(string? expected, ExpectationGrammars grammar)
		=> $"is JSON equivalent to {expected}";

	/// <inheritdoc cref="IStringMatchType.GetTypeString()" />
	public string GetTypeString()
		=> " as JSON";

	/// <inheritdoc cref="IStringMatchType.GetOptionString(bool, IEqualityComparer{string})" />
	public string GetOptionString(bool ignoreCase, IEqualityComparer<string>? comparer)
		=> "";
}
#endif
