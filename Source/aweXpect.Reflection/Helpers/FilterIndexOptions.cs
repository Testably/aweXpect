using System;
using aweXpect.Options;

namespace aweXpect.Reflection.Helpers;

/// <summary>
///     The <see cref="CollectionIndexOptions" /> of a filter, which remember how their index was specified, so that an
///     index option can create a new filter with its own options, while Core still rejects a second index.
/// </summary>
internal sealed class FilterIndexOptions : CollectionIndexOptions
{
	private readonly Action<CollectionIndexOptions> _setIndex;

	private FilterIndexOptions(Action<CollectionIndexOptions> setIndex)
	{
		setIndex(this);
		_setIndex = setIndex;
	}

	/// <summary>
	///     Returns new options with the index of the <paramref name="options" /> and the one specified by
	///     <paramref name="setIndex" />, while the <paramref name="options" /> stay unchanged.
	/// </summary>
	/// <remarks>
	///     Options of another type are expected to have no index, as filters only specify an index through this method.
	/// </remarks>
	public static CollectionIndexOptions With(CollectionIndexOptions options, Action<CollectionIndexOptions> setIndex)
		=> new FilterIndexOptions(options is FilterIndexOptions previous
			? o =>
			{
				previous._setIndex(o);
				setIndex(o);
			}
			: setIndex);
}
