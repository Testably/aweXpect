using System;
using aweXpect.Options;

namespace aweXpect.Results;

/// <summary>
///     The result for counting items in a collection.
/// </summary>
/// <param name="factory">
///     Creates the result from the <see cref="EnumerableQuantifier" /> to apply and a flag that is
///     <see langword="true" /> when the expectation is negated, as for <see cref="NotEqualTo(int)" />.
/// </param>
public class CollectionCountResult<TReturn>(Func<EnumerableQuantifier, bool, TReturn> factory)
{
	/// <summary>
	///     Verifies that the collection has exactly <paramref name="expected" /> items.
	/// </summary>
	public TReturn EqualTo(int expected)
		=> factory(EnumerableQuantifier.Exactly(expected), false);

	/// <summary>
	///     Verifies that the collection does not have exactly <paramref name="unexpected" /> items.
	/// </summary>
	public TReturn NotEqualTo(int unexpected)
		=> factory(EnumerableQuantifier.Exactly(unexpected), true);

	/// <summary>
	///     Verifies that the collection has more than <paramref name="expected" /> items.
	/// </summary>
	public TReturn GreaterThan(int expected)
		=> factory(EnumerableQuantifier.MoreThan(expected), false);

	/// <summary>
	///     Verifies that the collection does not have more than <paramref name="expected" /> items.
	/// </summary>
	public TReturn NotGreaterThan(int expected)
		=> factory(EnumerableQuantifier.MoreThan(expected), true);

	/// <summary>
	///     Verifies that the collection has at least <paramref name="expected" /> items.
	/// </summary>
	public TReturn GreaterThanOrEqualTo(int expected)
		=> factory(EnumerableQuantifier.AtLeast(expected), false);

	/// <summary>
	///     Verifies that the collection does not have at least <paramref name="expected" /> items.
	/// </summary>
	public TReturn NotGreaterThanOrEqualTo(int expected)
		=> factory(EnumerableQuantifier.AtLeast(expected), true);

	/// <summary>
	///     Verifies that the collection has fewer than <paramref name="expected" /> items.
	/// </summary>
	public TReturn LessThan(int expected)
		=> factory(EnumerableQuantifier.LessThan(expected), false);

	/// <summary>
	///     Verifies that the collection does not have fewer than <paramref name="expected" /> items.
	/// </summary>
	public TReturn NotLessThan(int expected)
		=> factory(EnumerableQuantifier.LessThan(expected), true);

	/// <summary>
	///     Verifies that the collection has at most <paramref name="expected" /> items.
	/// </summary>
	public TReturn LessThanOrEqualTo(int expected)
		=> factory(EnumerableQuantifier.AtMost(expected), false);

	/// <summary>
	///     Verifies that the collection does not have at most <paramref name="expected" /> items.
	/// </summary>
	public TReturn NotLessThanOrEqualTo(int expected)
		=> factory(EnumerableQuantifier.AtMost(expected), true);

	/// <summary>
	///     Verifies that the collection has between <paramref name="minimum" />…
	/// </summary>
	public BetweenResult<TReturn> Between(int minimum)
		=> new(maximum => factory(EnumerableQuantifier.Between(minimum, maximum), false));

	/// <summary>
	///     Verifies that the collection does not have between <paramref name="minimum" />…
	/// </summary>
	public BetweenResult<TReturn> NotBetween(int minimum)
		=> new(maximum => factory(EnumerableQuantifier.Between(minimum, maximum), true));
}
