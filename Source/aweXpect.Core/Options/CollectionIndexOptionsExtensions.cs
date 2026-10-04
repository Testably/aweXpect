using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect;

/// <summary>
///     Extension methods for results that limit the index of an item with <see cref="CollectionIndexOptions" />.
/// </summary>
/// <remarks>
///     Only one index can be specified.
/// </remarks>
public static class CollectionIndexOptionsExtensions
{
	/// <summary>
	///     …at the given zero-based <paramref name="index" />.
	/// </summary>
	public static TResult AtIndex<TResult>(this TResult result, int index)
		where TResult : IOptionsProvider<CollectionIndexOptions>
	{
		result.Options.AtIndex(index);
		return result;
	}

	/// <summary>
	///     …at the given zero-based <paramref name="index" />, counted from the end of the collection.
	/// </summary>
	public static TResult AtIndexFromEnd<TResult>(this TResult result, int index)
		where TResult : IOptionsProvider<CollectionIndexOptions>
	{
		result.Options.AtIndexFromEnd(index);
		return result;
	}
}
