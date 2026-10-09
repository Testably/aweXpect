using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using aweXpect.Core;

namespace aweXpect.Json;

internal sealed class JsonMatchType(JsonOptions options) : IStringMatchType
{
	private JsonElementValidator.JsonComparisonResult? _comparisonResult;
	private string? _validatedExpected;

	/// <inheritdoc cref="IStringMatchType.InspectsSubject" />
	public bool InspectsSubject => true;

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
		if (actual is null)
		{
			return $"{it} was <null>";
		}

		string? result = _comparisonResult?.ToString();
		if (!string.IsNullOrEmpty(result))
		{
			return $"{it} differed as{result}";
		}

		return "";
	}

	/// <inheritdoc cref="IStringMatchType.AreConsideredEqual(string?, string?, bool, IEqualityComparer{string})" />
	/// <remarks>
	///     The state of the previous comparison is reset first, because the same match type compares every value the
	///     expectation is evaluated for.
	/// </remarks>
	/// <exception cref="ArgumentNullException">The <paramref name="expected" /> value is <see langword="null" />.</exception>
	/// <exception cref="ArgumentException">The <paramref name="expected" /> value is no valid JSON.</exception>
	public async ValueTask<StringMatchResult>
		AreConsideredEqual(
			string? actual,
			string? expected,
			bool ignoreCase,
			IEqualityComparer<string>? comparer)
	{
		_comparisonResult = null;
		using JsonDocument expectedJson = ParseExpected(expected, options.DocumentOptions);
		if (actual is null)
		{
			return false;
		}

		JsonDocument actualJson;
		try
		{
			actualJson = JsonDocument.Parse(actual, options.DocumentOptions);
		}
		catch (JsonException e)
		{
			return StringMatchResult.NotComparable(
				$"it was {Formatter.Format(actual)}, which could not be parsed as JSON: {e.Message}", e);
		}

		using (actualJson)
		{
			_comparisonResult = await JsonElementValidator.Compare(
				actualJson.RootElement,
				expectedJson.RootElement,
				options);
			return !_comparisonResult.HasError;
		}
	}

	/// <inheritdoc cref="IStringMatchType.GetExpectation(string?, ExpectationGrammars)" />
	public string GetExpectation(string? expected, ExpectationGrammars grammars)
		=> (grammars.HasFlag(ExpectationGrammars.Active), grammars.HasFlag(ExpectationGrammars.Negated)) switch
		{
			(true, false) => $"{grammars.Verb("is", "are")} JSON equivalent to {expected}",
			(false, false) => $"JSON equivalent to {expected}",
			(true, true) => $"{grammars.Verb("is not", "are not")} JSON equivalent to {expected}",
			(false, true) => $"not JSON equivalent to {expected}",
		};

	/// <inheritdoc cref="IStringMatchType.GetTypeString()" />
	public string GetTypeString()
		=> " as JSON";

	/// <inheritdoc cref="IStringMatchType.GetOptionString(bool, IEqualityComparer{string})" />
	/// <remarks>
	///     The casing and a comparer are rejected when they are specified, so there is no option to describe.
	/// </remarks>
	public string GetOptionString(bool ignoreCase, IEqualityComparer<string>? comparer)
		=> "";

	/// <inheritdoc cref="IStringMatchType.ValidateOptions(bool, IEqualityComparer{string})" />
	/// <exception cref="InvalidOperationException">
	///     The casing is ignored or a <paramref name="comparer" /> is specified, which a JSON comparison cannot honour.
	/// </exception>
	public void ValidateOptions(bool ignoreCase, IEqualityComparer<string>? comparer)
	{
		if (ignoreCase)
		{
			throw Tracing.WriteException(
				new InvalidOperationException("IgnoringCase cannot be combined with AsJson."));
		}

		if (comparer is not null)
		{
			throw Tracing.WriteException(
				new InvalidOperationException("Using cannot be combined with AsJson."));
		}
	}

	/// <inheritdoc cref="IStringMatchType.ValidateExpected(string?)" />
	/// <remarks>
	///     The last accepted value is remembered, because it is validated again for every item of a collection.
	/// </remarks>
	/// <exception cref="ArgumentNullException">The <paramref name="expected" /> value is <see langword="null" />.</exception>
	/// <exception cref="ArgumentException">The <paramref name="expected" /> value is no valid JSON.</exception>
	public void ValidateExpected(string? expected)
	{
		if (expected is not null && string.Equals(expected, _validatedExpected, StringComparison.Ordinal))
		{
			return;
		}

		ParseExpected(expected, options.DocumentOptions).Dispose();
		_validatedExpected = expected;
	}

	/// <remarks>
	///     An expected value that is no JSON differs from every subject, so that a negated expectation could never fail.
	/// </remarks>
	private static JsonDocument ParseExpected(string? expected, JsonDocumentOptions documentOptions)
	{
		if (expected is null)
		{
			throw Tracing.WriteException(new ArgumentNullException(nameof(expected), "The expected JSON cannot be null."));
		}

		try
		{
			return JsonDocument.Parse(expected, documentOptions);
		}
		catch (JsonException e)
		{
			throw Tracing.WriteException(new ArgumentException($"The expected JSON is invalid: {e.Message}", e));
		}
	}
}
