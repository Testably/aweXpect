using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect.Snapshots.Tests.TestHelpers;

internal static class YieldingStringMatch
{
	/// <remarks>
	///     A custom match type that compares exactly, but only after yielding, as an asynchronous extension would.
	/// </remarks>
	public static TResult AsYieldingMatch<TResult>(this TResult result)
		where TResult : IOptionsProvider<StringEqualityOptions>
	{
		result.Options.SetMatchType(new YieldingMatchType(), nameof(AsYieldingMatch));
		return result;
	}

	private sealed class YieldingMatchType : IStringMatchType
	{
		public bool InspectsSubject => false;

		public async ValueTask<StringMatchResult> AreConsideredEqual(string? actual, string? expected,
			bool ignoreCase, IEqualityComparer<string>? comparer)
		{
			await Task.Yield();
			return string.Equals(actual, expected, StringComparison.Ordinal);
		}

		public void AppendContexts(ResultContextCollector contexts)
		{
			// The comparison has no options to explain.
		}

		public string GetExpectation(string? expected, ExpectationGrammars grammars)
			=> $"{(grammars.HasFlag(ExpectationGrammars.Negated) ? "is not" : "is")} equal to \"{expected}\"";

		public string GetExtendedFailure(string it, string? actual, string? expected, bool ignoreCase,
			IEqualityComparer<string> comparer, StringDifferenceSettings? settings)
			=> $"{it} was \"{actual}\"";

		public string GetTypeString() => " after yielding";

		public string GetOptionString(bool ignoreCase, IEqualityComparer<string>? comparer) => "";

		public void ValidateOptions(bool ignoreCase, IEqualityComparer<string>? comparer)
		{
			// The comparison ignores the options, so every option is accepted.
		}

		public void ValidateExpected(string? expected)
		{
			// Every expected value can be compared.
		}
	}
}
