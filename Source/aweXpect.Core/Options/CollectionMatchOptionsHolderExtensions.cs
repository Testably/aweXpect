using aweXpect.Core;

namespace aweXpect.Options;

/// <summary>
///     The option methods of <see cref="CollectionMatchOptions" /> on the <see cref="OptionsHolder{TOptions}" /> of an
///     expectation.
/// </summary>
public static class CollectionMatchOptionsHolderExtensions
{
	/// <inheritdoc cref="CollectionMatchOptions.IgnoringDuplicates(bool)" />
	public static OptionsHolder<CollectionMatchOptions> IgnoringDuplicates(this OptionsHolder<CollectionMatchOptions> holder,
		bool ignoreDuplicates = true)
	{
		holder.Value = holder.Value.IgnoringDuplicates(ignoreDuplicates);
		return holder;
	}

	/// <inheritdoc cref="CollectionMatchOptions.IgnoringInterspersedItems(bool)" />
	public static OptionsHolder<CollectionMatchOptions> IgnoringInterspersedItems(
		this OptionsHolder<CollectionMatchOptions> holder, bool ignoreInterspersedItems = true)
	{
		holder.Value = holder.Value.IgnoringInterspersedItems(ignoreInterspersedItems);
		return holder;
	}

	/// <inheritdoc cref="CollectionMatchOptions.InAnyOrder()" />
	public static OptionsHolder<CollectionMatchOptions> InAnyOrder(this OptionsHolder<CollectionMatchOptions> holder)
	{
		holder.Value = holder.Value.InAnyOrder();
		return holder;
	}

	/// <inheritdoc cref="CollectionMatchOptions.Properly()" />
	public static OptionsHolder<CollectionMatchOptions> Properly(this OptionsHolder<CollectionMatchOptions> holder)
	{
		holder.Value = holder.Value.Properly();
		return holder;
	}
}
