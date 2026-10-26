using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect;

/// <summary>
///     Extension methods for results that limit the index of an item with <see cref="CollectionIndexOptions" />.
/// </summary>
/// <remarks>
///     Only one index can be specified.<br />
///     A result that implements <see cref="ICopyOnOption{TOptions, TSelf}" /> stays unchanged: it returns a new one
///     with the new index.
/// </remarks>
public static class CollectionIndexOptionsExtensions
{
	/// <summary>
	///     …at the given zero-based <paramref name="index" />.
	/// </summary>
	public static TSelf AtIndex<TSelf>(this TSelf result, int index)
		where TSelf : IOptionsProvider<CollectionIndexOptions>
	{
		if (result is ICopyOnOption<CollectionIndexOptions, TSelf> copyOnOption)
		{
			return copyOnOption.WithOptions(result.Options.Value.AtIndex(index));
		}

		result.Options.AtIndex(index);
		return result;
	}

	/// <summary>
	///     …at the given zero-based <paramref name="index" />, counted from the end of the collection.
	/// </summary>
	public static TSelf AtIndexFromEnd<TSelf>(this TSelf result, int index)
		where TSelf : IOptionsProvider<CollectionIndexOptions>
	{
		if (result is ICopyOnOption<CollectionIndexOptions, TSelf> copyOnOption)
		{
			return copyOnOption.WithOptions(result.Options.Value.AtIndexFromEnd(index));
		}

		result.Options.AtIndexFromEnd(index);
		return result;
	}
}
