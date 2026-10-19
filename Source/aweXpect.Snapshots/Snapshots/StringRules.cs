using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using aweXpect.Options;

namespace aweXpect.Snapshots;

/// <summary>
///     Matches the text of a rule with aweXpect's <see cref="StringEqualityOptions" />, so that options like
///     <c>.IgnoringCase()</c>, <c>.AsWildcard()</c> or <c>.AsRegex()</c> mean the same as in aweXpect.
/// </summary>
/// <remarks>
///     A pattern that times out is reported with the <see cref="RegexMatchTimeoutException" /> for the text of the rule,
///     like the timeout of a masking, instead of the <see cref="ArgumentException" /> of aweXpect.
/// </remarks>
internal static class StringRules
{
	/// <summary>
	///     Whether the <paramref name="actual" /> text equals the <paramref name="expected" /> text, or matches it as a
	///     prefix, a suffix or a pattern.
	/// </summary>
	/// <remarks>
	///     The comparison only completes asynchronously for a custom comparer that does, so it is waited for, as a value
	///     nested in an object snapshot is decided while it is written.
	/// </remarks>
	public static bool IsMatch(StringEqualityOptions options, string actual, string expected)
	{
		try
		{
			return options.AreConsideredEqual(actual, expected).AsTask().GetAwaiter().GetResult();
		}
		catch (ArgumentException exception) when (exception.InnerException is RegexMatchTimeoutException timeout)
		{
			throw new RegexMatchTimeoutException(actual, expected, timeout.MatchTimeout);
		}
	}

	/// <summary>
	///     Whether the <paramref name="actual" /> text contains the <paramref name="expected" /> text or a match of it as a
	///     pattern.
	/// </summary>
	public static async ValueTask<bool> Contains(StringEqualityOptions options, string actual, string expected)
	{
		try
		{
			return await options.CountOccurrences(actual, expected, 1) > 0;
		}
		catch (ArgumentException exception) when (exception.InnerException is RegexMatchTimeoutException timeout)
		{
			throw new RegexMatchTimeoutException(actual, expected, timeout.MatchTimeout);
		}
	}
}
