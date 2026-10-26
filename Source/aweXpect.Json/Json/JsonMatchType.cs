using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Json.Options;

namespace aweXpect.Json;

internal sealed class JsonMatchType(JsonOptions options) : StringMatchType
{
	private static readonly MatchPhrase SharedPhrase =
		MatchPhrase.Being("JSON equivalent to").WithName(" as JSON").WithUnformattedExpected();

	private string? _validatedExpected;

	/// <inheritdoc />
	public override bool InspectsSubject => true;

	/// <inheritdoc />
	public override StringMatcher ForEvaluation(MatchEnvironment environment, StringMatchSettings settings)
		=> new JsonMatcher(options);

	/// <inheritdoc />
	public override MatchPhrase Phrase => SharedPhrase;

	/// <inheritdoc />
	/// <remarks>
	///     The last accepted value is remembered, because it is validated again for every item of a collection.
	/// </remarks>
	/// <exception cref="ArgumentNullException">The <paramref name="expected" /> value is <see langword="null" />.</exception>
	/// <exception cref="ArgumentException">The <paramref name="expected" /> value is no valid JSON.</exception>
	public override void ValidateExpected(string? expected, string parameterName)
	{
		if (expected is not null && string.Equals(expected, _validatedExpected, StringComparison.Ordinal))
		{
			return;
		}

		ParseExpected(expected, parameterName, options.DocumentOptions).Dispose();
		_validatedExpected = expected;
	}

	/// <summary>
	///     Parses the <paramref name="expected" /> JSON of an expectation, which the caller received as
	///     <paramref name="parameterName" />.
	/// </summary>
	/// <remarks>
	///     An expected value that is no JSON differs from every subject, so that a negated expectation could never fail.
	/// </remarks>
	internal static JsonDocument ParseExpected(string? expected, string parameterName,
		JsonDocumentOptions documentOptions)
	{
		if (expected is null)
		{
			throw Tracing.WriteException(new ArgumentNullException(parameterName,
				$"The '{parameterName}' JSON cannot be null."));
		}

		try
		{
			return JsonDocument.Parse(expected, documentOptions);
		}
		catch (JsonException e)
		{
			throw Tracing.WriteException(new ArgumentException(
				$"The '{parameterName}' JSON is invalid: {e.Message}", parameterName, e));
		}
	}

	/// <summary>
	///     Compares the JSON strings of one evaluation and keeps the members to ignore that matched no member.
	/// </summary>
	private sealed class JsonMatcher(JsonOptions options) : StringMatcher
	{
		private Dictionary<string, string?>? _unmatchedMembersToIgnore;

		/// <inheritdoc />
		/// <exception cref="ArgumentNullException">The <paramref name="expected" /> value is <see langword="null" />.</exception>
		/// <exception cref="ArgumentException">The <paramref name="expected" /> value is no valid JSON.</exception>
		public override async ValueTask<MatchResult> Match(string? actual, string? expected)
		{
			using JsonDocument expectedJson = ParseExpected(expected, nameof(expected), options.DocumentOptions);
			if (actual is null)
			{
				return MatchResult.NoMatch;
			}

			JsonDocument actualJson;
			try
			{
				actualJson = JsonDocument.Parse(actual, options.DocumentOptions);
			}
			catch (JsonException e)
			{
				return MatchResult.NotComparable(
					$"was {Formatter.Format(actual)}, which could not be parsed as JSON: {e.Message}", e);
			}

			using (actualJson)
			{
				JsonElementValidator.JsonComparisonResult comparisonResult = await JsonElementValidator.Compare(
					actualJson.RootElement,
					expectedJson.RootElement,
					options);
				_unmatchedMembersToIgnore = MembersToIgnoreTracker.Keep(_unmatchedMembersToIgnore,
					comparisonResult.GetUnmatchedMembersToIgnore());
				return comparisonResult.HasError
					? MatchResult.NoMatchBecause(new DifferencesExplanation(comparisonResult))
					: MatchResult.Match;
			}
		}

		/// <inheritdoc />
		public override string GetFailure(string it, ExpectationGrammars grammars, string? actual, string? expected,
			MatchResult result, StringDifferenceSettings? differenceSettings)
		{
			if (actual is null)
			{
				return $"{it}{grammars.SubjectVerb(it, " was ", " were ")}<null>";
			}

			return result.Explanation?.GetFailure(it, grammars, null) ?? "";
		}

		/// <summary>
		///     Adds the "JSON options" context with the members to ignore by name that matched no member in a failed
		///     comparison.
		/// </summary>
		/// <remarks>
		///     The items of a collection are compared one after the other with the same matcher, so the members to ignore
		///     of every failed comparison are kept, and a suggestion found for an item is not lost to a later item.
		/// </remarks>
		public override void AppendContexts(ResultContextCollector contexts)
			=> options.AddContext(contexts, _unmatchedMembersToIgnore);
	}

	/// <summary>
	///     The differences that a JSON comparison found.
	/// </summary>
	private sealed class DifferencesExplanation(JsonElementValidator.JsonComparisonResult comparisonResult)
		: MatchExplanation
	{
		/// <inheritdoc />
		public override string GetFailure(string it, ExpectationGrammars grammars, string? indentation)
		{
			string result = comparisonResult.ToString();
			return string.IsNullOrEmpty(result) ? "" : $"{it} differed as{result}";
		}
	}
}
