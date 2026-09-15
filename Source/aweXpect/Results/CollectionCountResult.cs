using System;
using aweXpect.Options;

namespace aweXpect.Results;

/// <summary>
///     The result for counting items in a collection.
/// </summary>
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
	///     Verifies that the collection has more than <paramref name="minimum" /> items.
	/// </summary>
	public TReturn GreaterThan(int minimum)
		=> factory(EnumerableQuantifier.MoreThan(minimum), false);

	/// <summary>
	///     Verifies that the collection has at least <paramref name="minimum" /> items.
	/// </summary>
	public TReturn GreaterThanOrEqualTo(int minimum)
		=> factory(EnumerableQuantifier.AtLeast(minimum), false);

	/// <summary>
	///     Verifies that the collection has less than <paramref name="maximum" /> items.
	/// </summary>
	public TReturn LessThan(int maximum)
		=> factory(EnumerableQuantifier.LessThan(maximum), false);

	/// <summary>
	///     Verifies that the collection has at most <paramref name="maximum" /> items.
	/// </summary>
	public TReturn LessThanOrEqualTo(int maximum)
		=> factory(EnumerableQuantifier.AtMost(maximum), false);

	/// <summary>
	///     Verifies that the collection has between <paramref name="minimum" />…
	/// </summary>
	public BetweenResult<TReturn> Between(int minimum)
		=> new(maximum => factory(EnumerableQuantifier.Between(minimum, maximum), false));
}
