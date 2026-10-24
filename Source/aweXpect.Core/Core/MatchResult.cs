using System;
using aweXpect.Core.Helpers;

namespace aweXpect.Core;

/// <summary>
///     The result of comparing two values with a <see cref="StringMatcher" /> or an <see cref="ObjectMatcher" />.
/// </summary>
/// <remarks>
///     A match and a plain mismatch allocate nothing. Only a mismatch that has to remember why it failed carries a
///     <see cref="MatchExplanation" />, which writes the failure text only when a failure message is created.<br />
///     The <see langword="default" /> value is a mismatch, so that an uninitialized result never passes.
/// </remarks>
public readonly struct MatchResult
{
	private readonly object? _detail;

	private MatchResult(MatchOutcome outcome, string? notComparableReason, object? detail)
	{
		Outcome = outcome;
		NotComparableReason = notComparableReason;
		_detail = detail;
	}

	/// <summary>
	///     The outcome of the comparison.
	/// </summary>
	public MatchOutcome Outcome { get; }

	/// <summary>
	///     Whether the values match.
	/// </summary>
	public bool IsMatch => Outcome == MatchOutcome.Match;

	/// <summary>
	///     Why the subject could not be compared, for <see cref="MatchOutcome.NotComparable" />, otherwise
	///     <see langword="null" />.
	/// </summary>
	public string? NotComparableReason { get; }

	/// <summary>
	///     The exception that explains the <see cref="NotComparableReason" />, if any.
	/// </summary>
	public Exception? Cause => _detail as Exception;

	/// <summary>
	///     The explanation of a mismatch, if the matcher provided one.
	/// </summary>
	public MatchExplanation? Explanation => _detail as MatchExplanation;

	/// <summary>
	///     The values match.
	/// </summary>
	public static MatchResult Match => new(MatchOutcome.Match, null, null);

	/// <summary>
	///     The values do not match, and the matcher explains the failure from the values alone.
	/// </summary>
	public static MatchResult NoMatch => default;

	/// <summary>
	///     A match when <paramref name="isMatch" /> is <see langword="true" />, otherwise a plain mismatch.
	/// </summary>
	public static MatchResult From(bool isMatch)
		=> isMatch ? Match : NoMatch;

	/// <summary>
	///     The values do not match, for the reason that the <paramref name="explanation" /> describes.
	/// </summary>
	/// <exception cref="ArgumentNullException">The <paramref name="explanation" /> is <see langword="null" />.</exception>
	public static MatchResult NoMatchBecause(MatchExplanation explanation)
	{
		explanation.ThrowIfNull();
		return new MatchResult(MatchOutcome.NoMatch, null, explanation);
	}

	/// <summary>
	///     The subject cannot be compared at all, e.g. a string that is no valid JSON, so that the expectation fails in
	///     both polarities with the <paramref name="reason" /> as the result, instead of the negated expectation
	///     succeeding.
	/// </summary>
	/// <param name="reason">
	///     Why the subject could not be compared, which follows "it" in the failure message, e.g.
	///     <c>was "foo", which could not be parsed as JSON: …</c> in
	///     <c>but it was "foo", which could not be parsed as JSON: …</c>.
	/// </param>
	/// <param name="cause">The exception that explains the <paramref name="reason" />, reported as the failure cause.</param>
	/// <remarks>
	///     Instead of "it", the <paramref name="reason" /> follows the name of the compared value in the failure
	///     message, e.g. the member name inside <c>Whose</c>, or "an item" (in a dictionary "a value") when the
	///     expectation is on a collection. Name the compared value in the reason, as "an item" does not say which one.
	/// </remarks>
	/// <exception cref="ArgumentNullException">The <paramref name="reason" /> is <see langword="null" />.</exception>
	public static MatchResult NotComparable(string reason, Exception? cause = null)
	{
		reason.ThrowIfNull();
		return new MatchResult(MatchOutcome.NotComparable, reason, cause);
	}
}
