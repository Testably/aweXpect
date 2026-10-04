using aweXpect.Core;
using aweXpect.Helpers;
using aweXpect.Options;

namespace aweXpect;

/// <summary>
///     Equality options for the items of a collection that tell whether their match type is still the default one.
/// </summary>
internal sealed class ItemEqualityOptions<TItem> : ObjectEqualityOptions<TItem>, IHasDefaultMatchType
{
	private readonly IObjectMatchType _defaultMatchType;

	public ItemEqualityOptions()
	{
		_defaultMatchType = MatchType;
	}

	public bool HasDefaultMatchType => ReferenceEquals(MatchType, _defaultMatchType);
}
