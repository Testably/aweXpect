using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Helpers;
using aweXpect.Customization;

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

	private const int MaximumParsedRegexes = 256;

	private static readonly StringMatchType RegexMatch = new RegexMatchType(RegexOptions.None);

	/// <summary>
	///     The regexes parsed by any expectation, as each expectation has options of its own, and parsing a pattern
	///     costs far more than matching it.
	/// </summary>
	/// <remarks>
	///     The culture is part of the key, because it affects a pattern that ignores the casing without
	///     <see cref="RegexOptions.CultureInvariant" />, and so is the timeout, because a regex keeps the one it was
	///     parsed with. The cache is cleared when it is full, so that generated patterns cannot grow it without limit.
	/// </remarks>
	private static readonly
		ConcurrentDictionary<(string Pattern, RegexOptions Options, string Culture, TimeSpan Timeout), Regex>
		ParsedRegexes = new();

	/// <summary>
	///     Interprets the expected <see langword="string" /> as <see cref="Regex" /> pattern.
	/// </summary>
	/// <exception cref="System.InvalidOperationException">
	///     A custom comparer is already set, which the regex engine cannot honour, or another match type is already specified.
	/// </exception>
	public StringEqualityOptions AsRegex()
	{
		return WithMatchType(RegexMatch, nameof(AsRegex), RegexMatchTypeIndex);
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
	///     A custom comparer is already set, which the regex engine cannot honour, or another match type is already specified.
	/// </exception>
	/// <exception cref="System.ArgumentOutOfRangeException">
	///     The <paramref name="regexOptions" /> are not a valid combination of <see cref="RegexOptions" />.
	/// </exception>
	public StringEqualityOptions AsRegex(RegexOptions regexOptions)
	{
		ThrowIfInvalidRegexOptions(regexOptions);
		return WithMatchType(new RegexMatchType(regexOptions), nameof(AsRegex), PrivateMatchTypeIndex);
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
			_ = GetOrParseRegex(string.Empty, regexOptions);
		}
		catch (ArgumentOutOfRangeException)
		{
			// ReSharper disable once LocalizableElement
			throw Tracing.WriteException(new ArgumentOutOfRangeException(nameof(regexOptions),
				$"The regex options '{regexOptions}' are not a valid combination."));
		}
	}

	/// <remarks>
	///     The timeout is read whenever a regex is needed, so that a value set for a scope applies to the expectations
	///     that are evaluated within it.
	/// </remarks>
	private static TimeSpan GetRegexTimeout()
		=> Customize.aweXpect.Settings().DefaultRegexTimeout.Get();

	private static Regex GetOrParseRegex(string pattern, RegexOptions options)
	{
		TimeSpan timeout = GetRegexTimeout();
		(string, RegexOptions, string, TimeSpan) key = (pattern, options, CultureInfo.CurrentCulture.Name, timeout);
		if (ParsedRegexes.TryGetValue(key, out Regex? regex))
		{
			return regex;
		}

		regex = new Regex(pattern, options, timeout);
		if (ParsedRegexes.Count >= MaximumParsedRegexes)
		{
			ParsedRegexes.Clear();
		}

		ParsedRegexes[key] = regex;
		return regex;
	}

	private sealed class RegexMatchType(RegexOptions regexOptions) : StringMatchType
	{
		private static readonly MatchPhrase SharedPhrase =
			MatchPhrase.Verb("matches regex", "match regex", "matching regex")
				.WithName(" as regex")
				.WithPattern("regex", "regex pattern");

		/// <summary>
		///     The last pattern that was accepted, because it is validated again for every item of a collection.
		/// </summary>
		private string? _validatedPattern;

		/// <inheritdoc />
		/// <remarks>
		///     A regex may match any part of the subject, like a substring.
		/// </remarks>
		public override StringMatchAnchors Anchors => StringMatchAnchors.None;

		/// <inheritdoc />
		public override bool InspectsSubject => true;

		/// <inheritdoc />
		public override MatchPhrase Phrase => SharedPhrase;

		/// <inheritdoc />
		/// <remarks>
		///     A pattern is matched by the regex engine, which has no way to consult a comparer.
		/// </remarks>
		public override StringMatchSettingKinds SupportedSettings
			=> StringMatchSettingKinds.IgnoreCase | StringMatchSettingKinds.WhiteSpace;

		/// <inheritdoc />
		public override StringMatcher ForEvaluation(MatchEnvironment environment, StringMatchSettings settings)
			=> new RegexMatcher(this, settings.IgnoreCase);

		/// <inheritdoc />
		/// <remarks>
		///     An empty regex pattern matches every value, so that the expectation would say nothing about the subject.
		/// </remarks>
		public override void ValidateExpected(string? expected, string parameterName)
		{
			if (expected is null || string.Equals(expected, _validatedPattern, StringComparison.Ordinal))
			{
				return;
			}

			if (expected.Length == 0)
			{
				throw CreateEmptyPatternException(parameterName, "regex pattern");
			}

			try
			{
				_ = CreateRegex(expected, false);
			}
			catch (ArgumentException exception) when (exception is not ArgumentOutOfRangeException)
			{
				// ReSharper disable once LocalizableElement
				throw Tracing.WriteException(new ArgumentException(
					$"The '{parameterName}' regex pattern is invalid: {exception.Message}", parameterName, exception));
			}

			_validatedPattern = expected;
		}

		/// <summary>
		///     Parses the <paramref name="expected" /> pattern with the regex options and the timeout.
		/// </summary>
		private Regex CreateRegex(string expected, bool ignoreCase)
		{
			RegexOptions options = regexOptions;
			if (ignoreCase)
			{
				options |= IgnoreCaseOptions;
			}

			return GetOrParseRegex(expected, options);
		}

		/// <summary>
		///     Counts the non-overlapping matches of the <paramref name="regex" /> in the <paramref name="actual" /> value,
		///     up to the <paramref name="maximum" />.
		/// </summary>
		/// <remarks>
		///     Empty matches are not counted, because they do not cover anything in the <paramref name="actual" /> value,
		///     consistent with an empty expected value which never occurs. The matches are found one by one, so that no
		///     match is searched after the <paramref name="maximum" />.
		/// </remarks>
		public static int CountMatches(string actual, Regex regex, int maximum)
		{
			int count = 0;
			for (Match match = regex.Match(actual); match.Success && count < maximum; match = match.NextMatch())
			{
				if (match.Length > 0)
				{
					count++;
				}
			}

			return count;
		}

		/// <summary>
		///     Matches the patterns of one evaluation, in which a collection compares every item with the same pattern, so
		///     it keeps the regex that was parsed last.
		/// </summary>
		private sealed class RegexMatcher(RegexMatchType matchType, bool ignoreCase) : StringMatcher
		{
			/// <remarks>
			///     The regex knows its pattern, so that one field is enough for the matcher of shared options, which is used
			///     by several threads.
			/// </remarks>
			private Regex? _regex;

			/// <inheritdoc />
			public override ValueTask<MatchResult> Match(string? actual, string? expected)
			{
				if (actual is null || expected is null)
				{
					return new ValueTask<MatchResult>(MatchResult.NoMatch);
				}

				return new ValueTask<MatchResult>(MatchResult.From(GetRegex(expected).IsMatch(actual)));
			}

			/// <inheritdoc />
			public override string GetFailure(string it, ExpectationGrammars grammars, string? actual,
				string? expected, MatchResult result, StringDifferenceSettings? differenceSettings)
			{
				if (expected is null)
				{
					return $"could not compare the <null> regex with {Formatter.Format(actual)}";
				}

				StringDifference stringDifference = new(actual, expected,
					ignoreCase ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal,
					differenceSettings.WithMatchType(StringDifference.MatchType.Regex));
				return $"{it} did not match{stringDifference.ToString("")}";
			}

			/// <inheritdoc />
			/// <remarks>
			///     A pattern can match a different number of characters than it is long, so its occurrences are counted
			///     by the regex engine.
			/// </remarks>
			public override int? CountOccurrences(string actual, string expected, int maximum)
				=> CountMatches(actual, GetRegex(expected), maximum);

			private Regex GetRegex(string expected)
			{
				Regex? regex = _regex;
				if (regex is null || !string.Equals(regex.ToString(), expected, StringComparison.Ordinal) ||
				    regex.MatchTimeout != GetRegexTimeout())
				{
					regex = matchType.CreateRegex(expected, ignoreCase);
					_regex = regex;
				}

				return regex;
			}
		}
	}
}