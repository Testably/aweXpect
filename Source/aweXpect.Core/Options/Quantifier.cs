using System;
using System.Text;
using aweXpect.Core.Helpers;

namespace aweXpect.Options;

/// <summary>
///     Quantifies an occurrence.
/// </summary>
/// <remarks>
///     The quantifier carries no negation, as several evaluations can share it; a negated expectation passes its
///     negation to <see cref="Check(int, bool, bool)" />, <see cref="IsNever" /> and <see cref="ToString(bool)" />.
/// </remarks>
public class Quantifier
{
	private CountBounds _bounds = CountBounds.Default;
	private string? _specifiedBy;

	/// <summary>
	///     The amount at which <see cref="Check(int, bool, bool)" /> becomes determinable without knowing whether more
	///     occurrences follow.
	/// </summary>
	/// <remarks>
	///     Callers that observe occurrences over time can stop as soon as this many occurred; below it they have to
	///     wait for their timeout to expire, because only then is the amount final.
	/// </remarks>
	public int DeterminableAmount => _bounds.DeterminableAmount;

	/// <summary>
	///     Returns <see langword="true" /> if the quantifier, or its negation when <paramref name="isNegated" />, is only
	///     met when it never occurs.
	/// </summary>
	public bool IsNever(bool isNegated)
		=> isNegated
			? _bounds.Complement() is { IsNever: true, }
			: _bounds.IsNever;

	/// <summary>
	///     A quantifier for exactly zero items.
	/// </summary>
	public static Quantifier Never()
	{
		Quantifier quantifier = new();
		quantifier.Exactly(0);
		return quantifier;
	}

	/// <summary>
	///     Verifies that it occurs at least <paramref name="minimum" /> times.
	/// </summary>
	/// <exception cref="ArgumentOutOfRangeException">The <paramref name="minimum" /> is negative.</exception>
	/// <exception cref="InvalidOperationException">The quantifier is already specified.</exception>
	public void AtLeast(int minimum)
		=> Set(CountBounds.AtLeast(minimum), nameof(AtLeast));

	/// <summary>
	///     Verifies that it occurs at most <paramref name="maximum" /> times.
	/// </summary>
	/// <exception cref="ArgumentOutOfRangeException">The <paramref name="maximum" /> is negative.</exception>
	/// <exception cref="InvalidOperationException">The quantifier is already specified.</exception>
	public void AtMost(int maximum)
		=> Set(CountBounds.AtMost(maximum), nameof(AtMost));

	/// <summary>
	///     Verifies that it occurs between <paramref name="minimum" /> and <paramref name="maximum" /> times.
	/// </summary>
	/// <exception cref="ArgumentOutOfRangeException">
	///     The <paramref name="minimum" /> or the <paramref name="maximum" /> is negative, or the
	///     <paramref name="maximum" /> is less than the <paramref name="minimum" />.
	/// </exception>
	/// <exception cref="InvalidOperationException">The quantifier is already specified.</exception>
	public void Between(int minimum, int maximum)
		=> Set(CountBounds.Between(minimum, maximum), nameof(Between));

	/// <summary>
	///     Verifies that it occurs fewer than <paramref name="maximum" /> times.
	/// </summary>
	/// <exception cref="ArgumentOutOfRangeException">The <paramref name="maximum" /> is not positive.</exception>
	/// <exception cref="InvalidOperationException">The quantifier is already specified.</exception>
	public void LessThan(int maximum)
		=> Set(CountBounds.LessThan(maximum), nameof(LessThan));

	/// <summary>
	///     Verifies that it occurs more than <paramref name="minimum" /> times.
	/// </summary>
	/// <exception cref="ArgumentOutOfRangeException">
	///     The <paramref name="minimum" /> is negative or <see cref="int.MaxValue" />.
	/// </exception>
	/// <exception cref="InvalidOperationException">The quantifier is already specified.</exception>
	public void MoreThan(int minimum)
		=> Set(CountBounds.MoreThan(minimum), nameof(MoreThan));

	/// <summary>
	///     Verifies the amount against the conditions, or against their negation when <paramref name="isNegated" />.
	/// </summary>
	/// <remarks>
	///     Returns <see langword="true" /> when the condition is satisfied,
	///     <see langword="false" /> when the condition is not satisfied
	///     and <see langword="null" /> when the condition could still be satisfied
	///     with a larger <paramref name="amount" />.
	/// </remarks>
	public bool? Check(int amount, bool isLast, bool isNegated = false)
		=> _bounds.Check(amount, isLast) is { } isMet ? isMet != isNegated : null;

	/// <summary>
	///     Verifies that it occurs exactly <paramref name="expected" /> times.
	/// </summary>
	/// <exception cref="ArgumentOutOfRangeException">The <paramref name="expected" /> is negative.</exception>
	/// <exception cref="InvalidOperationException">The quantifier is already specified.</exception>
	public void Exactly(int expected)
		=> Exactly(expected, nameof(Exactly));

	/// <summary>
	///     Verifies that it occurs exactly <paramref name="expected" /> times, as specified by the <paramref name="option" />.
	/// </summary>
	internal void Exactly(int expected, string option)
		=> Set(CountBounds.Exactly(expected), option);

	/// <summary>
	///     Rejects a second quantifier, because it would silently replace the bounds of the first one.
	/// </summary>
	private void Set(CountBounds bounds, string option)
	{
		ThrowHelper.ThrowIfOptionIsAlreadySpecified(_specifiedBy, option);
		_specifiedBy = option;
		_bounds = bounds;
	}

	/// <inheritdoc />
	public override string ToString()
		=> ToString(false);

	/// <summary>
	///     Returns the text of the quantifier, or of its negation when <paramref name="isNegated" />, e.g.
	///     <c>at least twice</c> or <c>never</c>.
	/// </summary>
	public string ToString(bool isNegated)
	{
		StringBuilder stringBuilder = new();
		_bounds.AppendTimes(stringBuilder, isNegated);
		return stringBuilder.ToString();
	}
}
