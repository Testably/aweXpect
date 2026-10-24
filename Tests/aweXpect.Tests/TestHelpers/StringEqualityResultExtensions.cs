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
	///     Like <see cref="AsCaseFolded{TResult}(TResult)" />, but explains a failure with a "Case folding" context, as an
	///     extension would list its options.
	/// </remarks>
	public static TResult AsCaseFoldedWithContext<TResult>(this TResult result)
		where TResult : IOptionsProvider<StringEqualityOptions>
	{
		result.Options.SetMatchType(new CaseFoldedMatchType("invariant culture"), nameof(AsCaseFoldedWithContext));
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

	private sealed class CaseFoldedMatchType(string? context = null) : StringMatchType
	{
		public override StringMatcher ForEvaluation(MatchEnvironment environment, StringMatchSettings settings)
			=> new CaseFoldedMatcher(context);

		public override MatchPhrase Phrase { get; } =
			MatchPhrase.Being("case-folded equal to").WithName(" as case-folded");

		public override StringMatchSettingKinds SupportedSettings
			=> StringMatchSettingKinds.IgnoreCase | StringMatchSettingKinds.Comparer | StringMatchSettingKinds.WhiteSpace;

		private sealed class CaseFoldedMatcher(string? context) : StringMatcher
		{
			public override ValueTask<MatchResult> Match(string? actual, string? expected)
				=> new(MatchResult.From(string.Equals(actual?.ToUpperInvariant(), expected?.ToUpperInvariant(),
					StringComparison.Ordinal)));

			public override void AppendContexts(ResultContextCollector contexts)
			{
				if (context is not null)
				{
					contexts.Add(new ResultContext.Fixed("Case folding", context));
				}
			}
		}
	}

	private sealed class NumberMatchType : StringMatchType
	{
		private static readonly NumberMatcher Matcher = new();

		public override StringMatcher ForEvaluation(MatchEnvironment environment, StringMatchSettings settings)
			=> Matcher;

		public override MatchPhrase Phrase { get; } = MatchPhrase.Being("the number").WithName(" as number");

		public override void ValidateExpected(string? expected, string parameterName)
		{
			if (!TryParse(expected, out _))
			{
				throw new ArgumentException($"The value {Formatter.Format(expected)} is no number.", nameof(expected));
			}
		}

		private static bool TryParse(string? value, out int number)
			=> int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out number);

		private sealed class NumberMatcher : StringMatcher
		{
			public override ValueTask<MatchResult> Match(string? actual, string? expected)
			{
				if (actual is null)
				{
					return new ValueTask<MatchResult>(MatchResult.NoMatch);
				}

				if (!TryParse(actual, out int actualNumber))
				{
					return new ValueTask<MatchResult>(
						MatchResult.NotComparable($"was {Formatter.Format(actual)}, which is no number"));
				}

				return new ValueTask<MatchResult>(MatchResult.From(TryParse(expected, out int expectedNumber) &&
				                                                   actualNumber == expectedNumber));
			}
		}
	}
}