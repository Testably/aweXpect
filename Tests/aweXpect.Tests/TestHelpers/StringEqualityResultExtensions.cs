using System.Collections.Generic;
using System.Globalization;
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

	/// <remarks>
	///     A custom match type that can neither use every expected value, nor honour every option, nor compare every
	///     subject, as an extension would set it.
	/// </remarks>
	public static TResult AsNumber<TResult>(this TResult result)
		where TResult : IOptionsProvider<StringEqualityOptions>
	{
		result.Options.SetMatchType(new NumberMatchType(), nameof(AsNumber));
		return result;
	}

	private sealed class CaseFoldedMatchType : IStringMatchType
	{
		public bool InspectsSubject => false;

		public ValueTask<StringMatchResult> AreConsideredEqual(string? actual, string? expected, bool ignoreCase,
			IEqualityComparer<string>? comparer)
			=> new(string.Equals(actual?.ToUpperInvariant(), expected?.ToUpperInvariant(), StringComparison.Ordinal));

		public string GetExpectation(string? expected, ExpectationGrammars grammars)
			=> $"{(grammars.HasFlag(ExpectationGrammars.Negated) ? "is not" : "is")} case-folded equal to {Formatter.Format(expected)}";

		public string GetExtendedFailure(string it, string? actual, string? expected, bool ignoreCase,
			IEqualityComparer<string> comparer, StringDifferenceSettings? settings)
			=> $"{it} was {Formatter.Format(actual)}";

		public string GetTypeString() => " as case-folded";

		public string GetOptionString(bool ignoreCase, IEqualityComparer<string>? comparer) => "";

		public void ValidateOptions(bool ignoreCase, IEqualityComparer<string>? comparer)
		{
			// The comparison folds the case on its own, so every option is accepted.
		}

		public void ValidateExpected(string? expected)
		{
			// Every expected value can be folded.
		}
	}

	private sealed class NumberMatchType : IStringMatchType
	{
		public bool InspectsSubject => false;

		public ValueTask<StringMatchResult> AreConsideredEqual(string? actual, string? expected, bool ignoreCase,
			IEqualityComparer<string>? comparer)
		{
			if (actual is null)
			{
				return new ValueTask<StringMatchResult>(false);
			}

			if (!TryParse(actual, out int actualNumber))
			{
				return new ValueTask<StringMatchResult>(
					StringMatchResult.NotComparable($"it was {Formatter.Format(actual)}, which is no number"));
			}

			return new ValueTask<StringMatchResult>(TryParse(expected, out int expectedNumber) &&
			                                        actualNumber == expectedNumber);
		}

		public string GetExpectation(string? expected, ExpectationGrammars grammars)
			=> $"{(grammars.HasFlag(ExpectationGrammars.Negated) ? "is not" : "is")} the number {Formatter.Format(expected)}";

		public string GetExtendedFailure(string it, string? actual, string? expected, bool ignoreCase,
			IEqualityComparer<string> comparer, StringDifferenceSettings? settings)
			=> $"{it} was {Formatter.Format(actual)}";

		public string GetTypeString() => " as number";

		public string GetOptionString(bool ignoreCase, IEqualityComparer<string>? comparer) => "";

		public void ValidateOptions(bool ignoreCase, IEqualityComparer<string>? comparer)
		{
			if (ignoreCase)
			{
				throw new InvalidOperationException("IgnoringCase cannot be combined with AsNumber.");
			}

			if (comparer is not null)
			{
				throw new InvalidOperationException("A custom comparer is not supported for AsNumber.");
			}
		}

		public void ValidateExpected(string? expected)
		{
			if (!TryParse(expected, out _))
			{
				throw new ArgumentException($"The value {Formatter.Format(expected)} is no number.", nameof(expected));
			}
		}

		private static bool TryParse(string? value, out int number)
			=> int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out number);
	}
}
