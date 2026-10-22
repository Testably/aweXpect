using System;
using aweXpect.Core.Helpers;

namespace aweXpect.Core;

/// <summary>
///     The result of comparing two strings with an <see cref="IStringMatchType" />: whether they are considered equal,
///     or why the subject could not be compared at all.
/// </summary>
/// <remarks>
///     A <see langword="bool" /> converts implicitly, so that a match type which can compare every subject returns
///     <see langword="true" /> or <see langword="false" />.
/// </remarks>
public readonly struct StringMatchResult
{
	private StringMatchResult(bool isEqual, string? notComparableReason, Exception? cause)
	{
		IsEqual = isEqual;
		NotComparableReason = notComparableReason;
		Cause = cause;
	}

	/// <summary>
	///     Whether the two strings are considered equal.
	/// </summary>
	public bool IsEqual { get; }

	/// <summary>
	///     Why the subject could not be compared, or <see langword="null" /> when it was compared.
	/// </summary>
	internal string? NotComparableReason { get; }

	/// <summary>
	///     The exception that explains the <see cref="NotComparableReason" />, if any.
	/// </summary>
	internal Exception? Cause { get; }

	/// <summary>
	///     The subject could not be compared at all, e.g. a string that is no valid JSON, so that the expectation fails
	///     in both polarities with the <paramref name="reason" /> as the result, instead of the negated expectation
	///     succeeding.
	/// </summary>
	/// <param name="reason">
	///     Why the subject could not be compared, which follows "it" in the failure message, e.g.
	///     <c>was "foo", which could not be parsed as JSON: …</c> in
	///     <c>but it was "foo", which could not be parsed as JSON: …</c>.
	/// </param>
	/// <param name="cause">The exception that explains the <paramref name="reason" />, reported as the failure cause.</param>
	/// <remarks>
	///     Instead of "it", the <paramref name="reason" /> follows the name of the compared string in the failure
	///     message, e.g. the member name inside <c>Whose</c>, or "an item" (in a dictionary "a value") when the
	///     expectation is on a collection. Name the compared value in the reason, as "an item" does not say which one.
	/// </remarks>
	/// <exception cref="ArgumentNullException">The <paramref name="reason" /> is <see langword="null" />.</exception>
	public static StringMatchResult FailureBothWays(string reason, Exception? cause = null)
	{
		reason.ThrowIfNull();
		return new StringMatchResult(false, reason, cause);
	}

	/// <summary>
	///     The two strings are considered equal when <paramref name="isEqual" /> is <see langword="true" />, and different
	///     otherwise.
	/// </summary>
	public static implicit operator StringMatchResult(bool isEqual)
		=> new(isEqual, null, null);
}
