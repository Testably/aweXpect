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
	///     The result text of the failure, which refers to the compared string as "it", e.g.
	///     <c>it could not be parsed as JSON: …</c>.
	/// </param>
	/// <param name="cause">The exception that explains the <paramref name="reason" />, reported as the failure cause.</param>
	/// <exception cref="ArgumentNullException">The <paramref name="reason" /> is <see langword="null" />.</exception>
	public static StringMatchResult NotComparable(string reason, Exception? cause = null)
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
