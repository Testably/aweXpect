using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect;

/// <summary>
///     Extension methods for results that match a collection with <see cref="CollectionMatchOptions" />.
/// </summary>
/// <remarks>
///     Each option can be specified only once.
/// </remarks>
public static class CollectionMatchOptionsExtensions
{
	/// <summary>
	///     Ignores the order in the subject and expected values.
	/// </summary>
	public static TResult InAnyOrder<TResult>(this TResult result)
		where TResult : IOptionsProvider<CollectionMatchOptions>
	{
		result.Options.InAnyOrder();
		return result;
	}

	/// <summary>
	///     Ignores duplicates in both collections,
	///     according to the <paramref name="ignoreDuplicates" /> parameter.
	/// </summary>
	/// <remarks>
	///     Only which items occur matters, not how often: every expected item has to be matched by an item, and every
	///     item has to match an expected item, as far as the relation requires it, so <c>[1, 1, 2]</c> matches
	///     <c>[1, 2]</c>.
	/// </remarks>
	/// <exception cref="System.InvalidOperationException">The duplicates are already specified.</exception>
	public static TResult IgnoringDuplicates<TResult>(this TResult result, bool ignoreDuplicates = true)
		where TResult : IOptionsProvider<CollectionMatchOptions>
	{
		result.Options.IgnoringDuplicates(ignoreDuplicates);
		return result;
	}

	/// <summary>
	///     Ignores items that appear in between the matched items,
	///     according to the <paramref name="ignoreInterspersedItems" /> parameter.
	/// </summary>
	/// <exception cref="System.InvalidOperationException">
	///     The interspersed items are already specified, or the order is already ignored via <c>InAnyOrder()</c>.
	/// </exception>
	public static TResult IgnoringInterspersedItems<TResult>(this TResult result,
		bool ignoreInterspersedItems = true)
		where TResult : IOptionsProvider<CollectionMatchOptions>, ICollectionContainmentOptions
	{
		result.Options.IgnoringInterspersedItems(ignoreInterspersedItems);
		return result;
	}

	/// <summary>
	///     Verifies that the two collections differ by at least one additional item.
	/// </summary>
	/// <remarks>
	///     This means that the expected collection is a proper subset of the subject for <c>Contains</c> and a proper
	///     superset for <c>IsContainedIn</c>.
	/// </remarks>
	public static TResult Properly<TResult>(this TResult result)
		where TResult : IOptionsProvider<CollectionMatchOptions>, IProperContainmentOptions
	{
		result.Options.Properly();
		return result;
	}
}
