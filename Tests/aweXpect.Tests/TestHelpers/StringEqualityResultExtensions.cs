using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect.Tests;

public static class StringEqualityResultExtensions
{
	/// <remarks>
	///     The result offers no <c>AsRegex()</c>, so the match type is switched through the options, as an extension
	///     would do.
	/// </remarks>
	public static TResult AsRegexThroughOptions<TResult>(this TResult result)
		where TResult : IOptionsProvider<StringEqualityOptions>
	{
		result.Options.AsRegex();
		return result;
	}

	/// <remarks>
	///     A custom match type that compares the subject as a value, as an extension would set it.
	/// </remarks>
	public static TResult AsCaseFolded<TResult>(this TResult result)
		where TResult : IOptionsProvider<StringEqualityOptions>
	{
		result.Options.SetMatchType(new CaseFoldedMatchType(), nameof(AsCaseFolded));
		return result;
	}

	private sealed class CaseFoldedMatchType : IStringMatchType
	{
		public bool InspectsSubject => false;

		public ValueTask<bool> AreConsideredEqual(string? actual, string? expected, bool ignoreCase,
			IEqualityComparer<string>? comparer)
			=> new(string.Equals(actual?.ToUpperInvariant(), expected?.ToUpperInvariant(), StringComparison.Ordinal));

		public string GetExpectation(string? expected, ExpectationGrammars grammars)
			=> $"{(grammars.HasFlag(ExpectationGrammars.Negated) ? "is not" : "is")} case-folded equal to {Formatter.Format(expected)}";

		public string GetExtendedFailure(string it, string? actual, string? expected, bool ignoreCase,
			IEqualityComparer<string> comparer, StringDifferenceSettings? settings)
			=> $"{it} was {Formatter.Format(actual)}";

		public string GetTypeString() => " as case-folded";

		public string GetOptionString(bool ignoreCase, IEqualityComparer<string>? comparer) => "";
	}
}
