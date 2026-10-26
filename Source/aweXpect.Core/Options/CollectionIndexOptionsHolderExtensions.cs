using aweXpect.Core;

namespace aweXpect.Options;

/// <summary>
///     The option methods of <see cref="CollectionIndexOptions" /> on the <see cref="OptionsHolder{TOptions}" /> of an
///     expectation.
/// </summary>
public static class CollectionIndexOptionsHolderExtensions
{
	/// <inheritdoc cref="CollectionIndexOptions.AtIndex(int)" />
	public static OptionsHolder<CollectionIndexOptions> AtIndex(this OptionsHolder<CollectionIndexOptions> holder, int index)
	{
		holder.Value = holder.Value.AtIndex(index);
		return holder;
	}

	/// <inheritdoc cref="CollectionIndexOptions.AtIndexFromEnd(int)" />
	public static OptionsHolder<CollectionIndexOptions> AtIndexFromEnd(this OptionsHolder<CollectionIndexOptions> holder,
		int index)
	{
		holder.Value = holder.Value.AtIndexFromEnd(index);
		return holder;
	}

	/// <inheritdoc cref="CollectionIndexOptions.SetMatch(CollectionIndexOptions.IMatch, string)" />
	public static OptionsHolder<CollectionIndexOptions> SetMatch(this OptionsHolder<CollectionIndexOptions> holder,
		CollectionIndexOptions.IMatch match, string optionName)
	{
		holder.Value = holder.Value.SetMatch(match, optionName);
		return holder;
	}
}
