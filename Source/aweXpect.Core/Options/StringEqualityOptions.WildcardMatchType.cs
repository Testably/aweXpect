using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Helpers;

namespace aweXpect.Options;

public partial class StringEqualityOptions
{
	private static readonly IStringMatchType WildcardMatch = new WildcardMatchType();

	/// <summary>
	///     Interprets the expected <see langword="string" /> as wildcard pattern.<br />
	///     Supports * to match zero or more characters and ? to match exactly one character.
	/// </summary>
	public StringEqualityOptions AsWildcard()
	{
		_matchType = WildcardMatch;
		return this;
	}

	private sealed class WildcardMatchType : IStringMatchType
	{
		/// <summary>
		///     Counts the non-overlapping matches of the <paramref name="expected" /> wildcard pattern in the
		///     <paramref name="actual" /> value.
		/// </summary>
		public static int CountOccurrences(string actual, string expected, bool ignoreCase)
			=> RegexMatchType.CountOccurrences(actual, WildcardToUnanchoredRegularExpression(expected), ignoreCase);

		private static string WildcardToRegularExpression(string value)
			=> $"^{WildcardToUnanchoredRegularExpression(value)}$";

		private static string WildcardToUnanchoredRegularExpression(string value)
			=> Regex.Escape(value)
				.Replace("\\?", ".")
				.Replace("\\*", "(.|\\n)*");

		#region IStringMatchType Members

		/// <inheritdoc
		///     cref="IStringMatchType.GetExtendedFailure(string, string?, string?, bool, IEqualityComparer{string}, StringDifferenceSettings?)" />
		public string GetExtendedFailure(string it, string? actual, string? expected,
			bool ignoreCase,
			IEqualityComparer<string> comparer,
			StringDifferenceSettings? settings)
		{
			if (expected is null)
			{
				return $"could not compare the <null> wildcard pattern with {Formatter.Format(actual)}";
			}

			StringDifference stringDifference = new(actual, expected, comparer,
				settings.WithMatchType(StringDifference.MatchType.Wildcard));
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

			RegexOptions options = ignoreCase
				? RegexOptions.Multiline | RegexOptions.IgnoreCase
				: RegexOptions.Multiline;

#if NET8_0_OR_GREATER
			return ValueTask.FromResult(Regex.IsMatch(actual, WildcardToRegularExpression(expected), options,
				RegexTimeout));
#else
			return Task.FromResult(Regex.IsMatch(actual, WildcardToRegularExpression(expected), options,
				RegexTimeout));
#endif
		}

		/// <inheritdoc cref="IStringMatchType.GetExpectation(string?, ExpectationGrammars)" />
		public string GetExpectation(string? expected, ExpectationGrammars grammars)
			=> (grammars.HasFlag(ExpectationGrammars.Active), grammars.HasFlag(ExpectationGrammars.Negated)) switch
			{
				(true, false) =>
					$"matches {Formatter.Format(expected.TruncateWithEllipsisOnWord(DefaultMaxLength).ToSingleLine())}",
				(false, false) =>
					$"matching {Formatter.Format(expected.TruncateWithEllipsisOnWord(DefaultMaxLength).ToSingleLine())}",
				(true, true) =>
					$"does not match {Formatter.Format(expected.TruncateWithEllipsisOnWord(DefaultMaxLength).ToSingleLine())}",
				(false, true) =>
					$"not matching {Formatter.Format(expected.TruncateWithEllipsisOnWord(DefaultMaxLength).ToSingleLine())}",
			};

		/// <inheritdoc cref="IStringMatchType.GetTypeString()" />
		public string GetTypeString()
			=> " as wildcard";

		/// <inheritdoc cref="IStringMatchType.GetOptionString(bool, IEqualityComparer{string})" />
		public string GetOptionString(bool ignoreCase, IEqualityComparer<string>? comparer)
			=> "";

		#endregion
	}
}
