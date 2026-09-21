using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Helpers;

namespace aweXpect.Options;

public partial class StringEqualityOptions
{
	/// <remarks>
	///     <see cref="RegexOptions.CultureInvariant" /> keeps the result independent of the current culture, so that
	///     ignoring the casing means the same for a pattern as for all other match types.
	/// </remarks>
	private const RegexOptions IgnoreCaseOptions = RegexOptions.IgnoreCase | RegexOptions.CultureInvariant;

	private static readonly IStringMatchType RegexMatch = new RegexMatchType(RegexOptions.None);

	/// <summary>
	///     Interprets the expected <see langword="string" /> as <see cref="Regex" /> pattern.
	/// </summary>
	/// <exception cref="System.InvalidOperationException">
	///     A custom comparer is already set, which the regex engine cannot honour.
	/// </exception>
	public StringEqualityOptions AsRegex()
	{
		if (_comparer is not null)
		{
			throw ComparerAndPatternConflict();
		}

		_matchType = RegexMatch;
		return this;
	}

	/// <summary>
	///     Interprets the expected <see langword="string" /> as <see cref="Regex" /> pattern,
	///     applying the given <paramref name="regexOptions" />.
	/// </summary>
	/// <remarks>
	///     <see cref="RegexOptions.IgnoreCase" /> and <see cref="RegexOptions.CultureInvariant" /> are added when the
	///     casing is ignored via <see cref="IgnoringCase(bool)" />.
	/// </remarks>
	/// <exception cref="System.InvalidOperationException">
	///     A custom comparer is already set, which the regex engine cannot honour.
	/// </exception>
	public StringEqualityOptions AsRegex(RegexOptions regexOptions)
	{
		if (_comparer is not null)
		{
			throw ComparerAndPatternConflict();
		}

		_matchType = new RegexMatchType(regexOptions);
		return this;
	}

	private sealed class RegexMatchType(RegexOptions regexOptions) : IStringMatchType
	{
		/// <summary>
		///     The <see cref="RegexOptions" /> that the pattern is matched with.
		/// </summary>
		public RegexOptions Options { get; } = regexOptions;

		/// <summary>
		///     Counts the non-overlapping matches of the <paramref name="expected" /> pattern in the
		///     <paramref name="actual" /> value.
		/// </summary>
		/// <remarks>
		///     Empty matches are not counted, because they do not cover anything in the <paramref name="actual" /> value,
		///     consistent with an empty expected value which never occurs.
		/// </remarks>
		public static int CountOccurrences(string actual, string expected, bool ignoreCase,
			RegexOptions additionalOptions = RegexOptions.None)
		{
			RegexOptions options = additionalOptions;
			if (ignoreCase)
			{
				options |= IgnoreCaseOptions;
			}

			int count = 0;
			foreach (Match match in Regex.Matches(actual, expected, options, RegexTimeout))
			{
				if (match.Length > 0)
				{
					count++;
				}
			}

			return count;
		}

		#region IMatchType Members

		/// <inheritdoc
		///     cref="IStringMatchType.GetExtendedFailure(string, string?, string?, bool, IEqualityComparer{string}, StringDifferenceSettings?)" />
		public string GetExtendedFailure(string it, string? actual, string? expected,
			bool ignoreCase,
			IEqualityComparer<string> comparer,
			StringDifferenceSettings? settings)
		{
			if (expected is null)
			{
				return $"could not compare the <null> regex with {Formatter.Format(actual)}";
			}

			StringDifference stringDifference = new(actual, expected, comparer,
				settings.WithMatchType(StringDifference.MatchType.Regex));
			return $"{it} did not match{stringDifference.ToString("")}";
		}

		/// <inheritdoc cref="IStringMatchType.AreConsideredEqual(string?, string?, bool, IEqualityComparer{string})" />
#if NET8_0_OR_GREATER
		public ValueTask<bool>
#else
		public Task<bool>
#endif
		AreConsideredEqual(string? actual, string? expected, bool ignoreCase,
			IEqualityComparer<string>? comparer)
		{
			if (actual is null || expected is null)
			{
#if NET8_0_OR_GREATER
				return ValueTask.FromResult(false);
#else
				return Task.FromResult(false);
#endif
			}

			RegexOptions options = Options;
			if (ignoreCase)
			{
				options |= IgnoreCaseOptions;
			}

#if NET8_0_OR_GREATER
			return ValueTask.FromResult(Regex.IsMatch(actual, expected, options, RegexTimeout));
#else
			return Task.FromResult(Regex.IsMatch(actual, expected, options, RegexTimeout));
#endif
		}

		/// <inheritdoc cref="IStringMatchType.GetExpectation(string?, ExpectationGrammars)" />
		public string GetExpectation(string? expected, ExpectationGrammars grammars)
			=> (grammars.HasFlag(ExpectationGrammars.Active), grammars.HasFlag(ExpectationGrammars.Negated)) switch
			{
				(true, false) =>
					$"{grammars.Verb("matches", "match")} regex {Formatter.Format(expected.TruncateWithEllipsisOnWord(DefaultMaxLength).ToSingleLine())}",
				(false, false) =>
					$"matching regex {Formatter.Format(expected.TruncateWithEllipsisOnWord(DefaultMaxLength).ToSingleLine())}",
				(true, true) =>
					$"{grammars.Verb("does not match", "do not match")} regex {Formatter.Format(expected.TruncateWithEllipsisOnWord(DefaultMaxLength).ToSingleLine())}",
				(false, true) =>
					$"not matching regex {Formatter.Format(expected.TruncateWithEllipsisOnWord(DefaultMaxLength).ToSingleLine())}",
			};

		/// <inheritdoc cref="IStringMatchType.GetTypeString()" />
		public string GetTypeString()
			=> " as regex";

		/// <inheritdoc cref="IStringMatchType.GetOptionString(bool, IEqualityComparer{string})" />
		/// <remarks>
		///     A <paramref name="comparer" /> is rejected for a pattern, so only the casing can be ignored here.
		/// </remarks>
		public string GetOptionString(bool ignoreCase, IEqualityComparer<string>? comparer)
			=> ignoreCase ? " ignoring case" : "";

		#endregion
	}
}
