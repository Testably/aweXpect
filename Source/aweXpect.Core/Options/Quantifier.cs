using System;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Helpers;

namespace aweXpect.Options;

/// <summary>
///     Quantifies an occurrence.
/// </summary>
/// <remarks>
///     An immutable value: the option methods return a new quantifier, and <see langword="default" /> means
///     <c>at least once</c>, like <see cref="Default" />.<br />
///     The quantifier carries no negation, as several evaluations can share it; a negated expectation passes its
///     negation to <see cref="Check(int, bool, bool)" />, <see cref="IsNever" /> and <see cref="ToString(bool)" />.
/// </remarks>
[ImmutableOptions]
public readonly struct Quantifier
{
	private static readonly string?[] OptionNames =
	[
		null,
		nameof(AtLeast),
		nameof(AtMost),
		nameof(Between),
		nameof(LessThan),
		nameof(MoreThan),
		nameof(Exactly),
		nameof(QuantifierExtensions.Never),
		nameof(QuantifierExtensions.Once),
		nameof(QuantifierExtensions.Twice),
	];

	private readonly CountBounds _bounds;
	private readonly QuantifierOption _option;

	private Quantifier(CountBounds bounds, QuantifierOption option)
	{
		_bounds = bounds;
		_option = option;
	}

	/// <summary>
	///     The quantifier of an unspecified occurrence: at least once.
	/// </summary>
	public static Quantifier Default => default;

	/// <summary>
	///     The amount at which <see cref="Check(int, bool, bool)" /> becomes determinable without knowing whether more
	///     occurrences follow.
	/// </summary>
	/// <remarks>
	///     Callers that observe occurrences over time can stop as soon as this many occurred; below it they have to
	///     wait for their timeout to expire, because only then is the amount final.
	/// </remarks>
	public int DeterminableAmount => Bounds.DeterminableAmount;

	/// <summary>
	///     The bounds, which are <c>at least once</c> as long as no option is specified, so that
	///     <see langword="default" /> is a valid quantifier.
	/// </summary>
	private CountBounds Bounds => _option == QuantifierOption.None ? CountBounds.Default : _bounds;

	/// <summary>
	///     Returns <see langword="true" /> if the quantifier, or its negation when <paramref name="isNegated" />, is only
	///     met when it never occurs.
	/// </summary>
	public bool IsNever(bool isNegated)
		=> isNegated
			? Bounds.Complement() is { IsNever: true, }
			: Bounds.IsNever;

	/// <summary>
	///     Verifies that it occurs at least <paramref name="minimum" /> times.
	/// </summary>
	/// <exception cref="ArgumentOutOfRangeException">The <paramref name="minimum" /> is negative.</exception>
	/// <exception cref="InvalidOperationException">Another quantifier is already specified.</exception>
	public Quantifier AtLeast(int minimum)
		=> With(CountBounds.AtLeast(minimum), QuantifierOption.AtLeast);

	/// <summary>
	///     Verifies that it occurs at most <paramref name="maximum" /> times.
	/// </summary>
	/// <exception cref="ArgumentOutOfRangeException">The <paramref name="maximum" /> is negative.</exception>
	/// <exception cref="InvalidOperationException">Another quantifier is already specified.</exception>
	public Quantifier AtMost(int maximum)
		=> With(CountBounds.AtMost(maximum), QuantifierOption.AtMost);

	/// <summary>
	///     Verifies that it occurs between <paramref name="minimum" /> and <paramref name="maximum" /> times.
	/// </summary>
	/// <exception cref="ArgumentOutOfRangeException">
	///     The <paramref name="minimum" /> or the <paramref name="maximum" /> is negative, or the
	///     <paramref name="maximum" /> is less than the <paramref name="minimum" />.
	/// </exception>
	/// <exception cref="InvalidOperationException">Another quantifier is already specified.</exception>
	public Quantifier Between(int minimum, int maximum)
		=> With(CountBounds.Between(minimum, maximum), QuantifierOption.Between);

	/// <summary>
	///     Verifies that it occurs fewer than <paramref name="maximum" /> times.
	/// </summary>
	/// <exception cref="ArgumentOutOfRangeException">The <paramref name="maximum" /> is not positive.</exception>
	/// <exception cref="InvalidOperationException">Another quantifier is already specified.</exception>
	public Quantifier LessThan(int maximum)
		=> With(CountBounds.LessThan(maximum), QuantifierOption.LessThan);

	/// <summary>
	///     Verifies that it occurs more than <paramref name="minimum" /> times.
	/// </summary>
	/// <exception cref="ArgumentOutOfRangeException">
	///     The <paramref name="minimum" /> is negative or <see cref="int.MaxValue" />.
	/// </exception>
	/// <exception cref="InvalidOperationException">Another quantifier is already specified.</exception>
	public Quantifier MoreThan(int minimum)
		=> With(CountBounds.MoreThan(minimum), QuantifierOption.MoreThan);

	/// <summary>
	///     Verifies that it never occurs.
	/// </summary>
	/// <exception cref="InvalidOperationException">Another quantifier is already specified.</exception>
	public Quantifier Never()
		=> Exactly(0, QuantifierOption.Never);

	/// <summary>
	///     Verifies the amount against the conditions, or against their negation when <paramref name="isNegated" />.
	/// </summary>
	/// <remarks>
	///     Returns <see langword="true" /> when the <paramref name="amount" /> meets the conditions and no larger amount
	///     can exceed them, <see langword="false" /> when it exceeds them and <see langword="null" /> as long as a larger
	///     amount could still meet or exceed them. When <paramref name="isNegated" />, <see langword="true" /> and
	///     <see langword="false" /> are swapped.<br />
	///     At the <paramref name="isLast" /> amount no larger amount follows, so <see langword="null" /> means that the
	///     amount is too low. It is returned regardless of <paramref name="isNegated" />: the conditions are not met, so
	///     their negation is.
	/// </remarks>
	public bool? Check(int amount, bool isLast, bool isNegated = false)
		=> Bounds.Check(amount, isLast) is { } isMet ? isMet != isNegated : null;

	/// <summary>
	///     Verifies that it occurs exactly <paramref name="expected" /> times.
	/// </summary>
	/// <exception cref="ArgumentOutOfRangeException">The <paramref name="expected" /> is negative.</exception>
	/// <exception cref="InvalidOperationException">Another quantifier is already specified.</exception>
	public Quantifier Exactly(int expected)
		=> Exactly(expected, QuantifierOption.Exactly);

	/// <summary>
	///     Verifies that it occurs exactly <paramref name="expected" /> times, as specified by the <paramref name="option" />.
	/// </summary>
	internal Quantifier Exactly(int expected, QuantifierOption option)
		=> With(CountBounds.Exactly(expected), option);

	/// <summary>
	///     Returns a quantifier with the <paramref name="bounds" /> of the <paramref name="option" />, which replaces the
	///     same option, but rejects another one, because it would silently replace the bounds of the first one.
	/// </summary>
	private Quantifier With(CountBounds bounds, QuantifierOption option)
	{
		ThrowHelper.ThrowIfCompetingOption(OptionNames[(int)_option], OptionNames[(int)option]!);
		return new Quantifier(bounds, option);
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
		Bounds.AppendTimes(stringBuilder, isNegated);
		return stringBuilder.ToString();
	}
}
