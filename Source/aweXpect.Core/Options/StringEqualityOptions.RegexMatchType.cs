using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Helpers;

namespace aweXpect.Options;

public partial class StringEqualityOptions
{
	/// <remarks>
	///     <see cref="RegexOptions.CultureInvariant" /> keeps the result independent of the current culture. The regex
	///     engine still applies its own case folding, which can differ from
	///     <see cref="System.StringComparison.OrdinalIgnoreCase" /> for a few characters, depending on the target
	///     framework.
	/// </remarks>
	private const RegexOptions IgnoreCaseOptions = RegexOptions.IgnoreCase | RegexOptions.CultureInvariant;

	private static readonly IStringMatchType RegexMatch = new RegexMatchType(RegexOptions.None);

	/// <summary>
	///     Interprets the expected <see langword="string" /> as <see cref="Regex" /> pattern.
	/// </summary>
	/// <exception cref="System.InvalidOperationException">
	///     A custom comparer is already set, which the regex engine cannot honour, or a match type is already specified.
	/// </exception>
	public StringEqualityOptions AsRegex()
	{
		if (_comparer is not null)
		{
			throw ComparerAndPatternConflict();
		}

		SetMatchType(RegexMatch, nameof(AsRegex));
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
	///     A custom comparer is already set, which the regex engine cannot honour, or a match type is already specified.
	/// </exception>
	/// <exception cref="System.ArgumentOutOfRangeException">
	///     The <paramref name="regexOptions" /> are not a valid combination of <see cref="RegexOptions" />.
	/// </exception>
	public StringEqualityOptions AsRegex(RegexOptions regexOptions)
	{
		if (_comparer is not null)
		{
			throw ComparerAndPatternConflict();
		}

		ThrowIfInvalidRegexOptions(regexOptions);
		SetMatchType(new RegexMatchType(regexOptions), nameof(AsRegex));
		return this;
	}

	/// <summary>
	///     Verifies that the <paramref name="regexOptions" /> are a combination that the regex engine accepts.
	/// </summary>
	/// <remarks>
	///     The regex engine decides which combinations it accepts, which differs between the target frameworks, so it is
	///     asked directly instead of failing only when the pattern is matched.
	/// </remarks>
	private static void ThrowIfInvalidRegexOptions(RegexOptions regexOptions)
	{
		try
		{
			_ = new Regex(string.Empty, regexOptions, RegexTimeout);
		}
		catch (ArgumentOutOfRangeException)
		{
			// ReSharper disable once LocalizableElement
			throw Tracing.WriteException(new ArgumentOutOfRangeException(nameof(regexOptions),
				$"The regex options '{regexOptions}' are not a valid combination."));
		}
	}

	private sealed class RegexMatchType(RegexOptions regexOptions) : IStringMatchType
	{
		/// <summary>
		///     The <see cref="RegexOptions" /> that the pattern is matched with.
		/// </summary>
		public RegexOptions Options { get; } = regexOptions;

		/// <summary>
		///     Parses the <paramref name="expected" /> pattern with the <see cref="Options" /> and the timeout.
		/// </summary>
		public Regex CreateRegex(string expected, bool ignoreCase)
		{
			RegexOptions options = Options;
			if (ignoreCase)
			{
				options |= IgnoreCaseOptions;
			}

			return new Regex(expected, options, RegexTimeout);
		}

		/// <summary>
		///     Counts the non-overlapping matches of the <paramref name="regex" /> in the <paramref name="actual" /> value.
		/// </summary>
		/// <remarks>
		///     Empty matches are not counted, because they do not cover anything in the <paramref name="actual" /> value,
		///     consistent with an empty expected value which never occurs.
		/// </remarks>
		public static int CountOccurrences(string actual, Regex regex)
		{
			int count = 0;
			foreach (Match match in regex.Matches(actual))
			{
				if (match.Length > 0)
				{
					count++;
				}
			}

			return count;
		}

		#region IMatchType Members

		/// <inheritdoc cref="IStringMatchType.InspectsSubject" />
		public bool InspectsSubject => true;

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
		public ValueTask<bool>
			AreConsideredEqual(string? actual, string? expected, bool ignoreCase,
				IEqualityComparer<string>? comparer)
		{
			if (actual is null || expected is null)
			{
				return new ValueTask<bool>(false);
			}

			return new ValueTask<bool>(CreateRegex(expected, ignoreCase).IsMatch(actual));
		}

		/// <inheritdoc cref="IStringMatchType.GetExpectation(string?, ExpectationGrammars)" />
		public string GetExpectation(string? expected, ExpectationGrammars grammars)
			=> (grammars.HasFlag(ExpectationGrammars.Active), grammars.HasFlag(ExpectationGrammars.Negated)) switch
			{
				(true, false) =>
					$"{grammars.Verb("matches", "match")} regex {Formatter.Format(expected.TruncateWithEllipsisOnWord(DefaultMaxLength))}",
				(false, false) =>
					$"matching regex {Formatter.Format(expected.TruncateWithEllipsisOnWord(DefaultMaxLength))}",
				(true, true) =>
					$"{grammars.Verb("does not match", "do not match")} regex {Formatter.Format(expected.TruncateWithEllipsisOnWord(DefaultMaxLength))}",
				(false, true) =>
					$"not matching regex {Formatter.Format(expected.TruncateWithEllipsisOnWord(DefaultMaxLength))}",
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
