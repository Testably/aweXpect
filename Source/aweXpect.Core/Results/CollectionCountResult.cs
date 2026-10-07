using System;
using aweXpect.Core.Helpers;
using aweXpect.Options;

namespace aweXpect.Results;

/// <summary>
///     The result for counting items in a collection.
/// </summary>
/// <param name="factory">
///     Creates the result from the <see cref="EnumerableQuantifier" /> to apply and a flag that is
///     <see langword="true" /> when the expectation is negated, as for <see cref="NotEqualTo(int?)" />.
/// </param>
/// <remarks>
///     A <see langword="null" /> count never matches, because a count is never <see langword="null" />. Compared
///     for order, e.g. with <see cref="GreaterThan(int?)" />, it fails also when negated, because nothing can be
///     ordered against <see langword="null" />.
/// </remarks>
public class CollectionCountResult<TReturn>(Func<EnumerableQuantifier, bool, TReturn> factory)
{
	/// <summary>
	///     Verifies that the collection has exactly <paramref name="expected" /> items.
	/// </summary>
	public TReturn EqualTo(int? expected)
		=> factory(EnumerableQuantifier.Exactly(expected), false);

	/// <summary>
	///     Verifies that the collection does not have exactly <paramref name="unexpected" /> items.
	/// </summary>
	public TReturn NotEqualTo(int? unexpected)
		=> factory(EnumerableQuantifier.Exactly(unexpected), true);

	/// <summary>
	///     Verifies that the collection has more than <paramref name="expected" /> items.
	/// </summary>
	public TReturn GreaterThan(int? expected)
		=> factory(Ordered(expected, EnumerableQuantifier.MoreThan, "more than"), false);

	/// <summary>
	///     Verifies that the collection does not have more than <paramref name="expected" /> items.
	/// </summary>
	public TReturn NotGreaterThan(int? expected)
		=> factory(Ordered(expected, EnumerableQuantifier.MoreThan, "more than"), true);

	/// <summary>
	///     Verifies that the collection has at least <paramref name="expected" /> items.
	/// </summary>
	public TReturn GreaterThanOrEqualTo(int? expected)
		=> factory(Ordered(expected, EnumerableQuantifier.AtLeast, "at least"), false);

	/// <summary>
	///     Verifies that the collection does not have at least <paramref name="expected" /> items.
	/// </summary>
	public TReturn NotGreaterThanOrEqualTo(int? expected)
		=> factory(Ordered(expected, EnumerableQuantifier.AtLeast, "at least"), true);

	/// <summary>
	///     Verifies that the collection has fewer than <paramref name="expected" /> items.
	/// </summary>
	public TReturn LessThan(int? expected)
		=> factory(Ordered(expected, EnumerableQuantifier.LessThan, "fewer than"), false);

	/// <summary>
	///     Verifies that the collection does not have fewer than <paramref name="expected" /> items.
	/// </summary>
	public TReturn NotLessThan(int? expected)
		=> factory(Ordered(expected, EnumerableQuantifier.LessThan, "fewer than"), true);

	/// <summary>
	///     Verifies that the collection has at most <paramref name="expected" /> items.
	/// </summary>
	public TReturn LessThanOrEqualTo(int? expected)
		=> factory(Ordered(expected, EnumerableQuantifier.AtMost, "at most"), false);

	/// <summary>
	///     Verifies that the collection does not have at most <paramref name="expected" /> items.
	/// </summary>
	public TReturn NotLessThanOrEqualTo(int? expected)
		=> factory(Ordered(expected, EnumerableQuantifier.AtMost, "at most"), true);

	/// <summary>
	///     Verifies that the collection has between <paramref name="minimum" />…
	/// </summary>
	public BetweenResult<TReturn, int?> Between(int? minimum)
	{
		ThrowHelper.ThrowIfCountIsNegative(minimum);
		return new BetweenResult<TReturn, int?>(maximum => factory(Range(minimum, maximum), false));
	}

	/// <summary>
	///     Verifies that the collection does not have between <paramref name="minimum" />…
	/// </summary>
	public BetweenResult<TReturn, int?> NotBetween(int? minimum)
	{
		ThrowHelper.ThrowIfCountIsNegative(minimum);
		return new BetweenResult<TReturn, int?>(maximum => factory(Range(minimum, maximum), true));
	}

	private static EnumerableQuantifier Ordered(int? expected, Func<int, EnumerableQuantifier> quantifier,
		string text)
		=> expected is null
			? EnumerableQuantifier.OrderedAgainstNull($"{text} <null>")
			: quantifier(expected.Value);

	private static EnumerableQuantifier Range(int? minimum, int? maximum)
	{
		ThrowHelper.ThrowIfCountIsNegative(maximum);
		return minimum is null || maximum is null
			? EnumerableQuantifier.OrderedAgainstNull(
				$"between {minimum?.ToString() ?? "<null>"} and {maximum?.ToString() ?? "<null>"}")
			: EnumerableQuantifier.Between(minimum.Value, maximum.Value);
	}
}
