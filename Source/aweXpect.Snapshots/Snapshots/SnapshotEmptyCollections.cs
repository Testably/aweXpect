using System;

namespace aweXpect.Snapshots;

/// <summary>
///     The members whose value is an empty collection, see
///     <see cref="SnapshotMembers{TReturn, TValue}.WhichAreEmptyCollections" />, which can only be left out.
/// </summary>
/// <typeparam name="TReturn">What an action returns.</typeparam>
public sealed class SnapshotEmptyCollections<TReturn>
{
	private readonly Func<SnapshotRule, TReturn> _add;
	private readonly ValueSelection _selection;

	internal SnapshotEmptyCollections(Func<SnapshotRule, TReturn> add, ValueSelection selection)
	{
		_add = add;
		_selection = selection;
	}

	/// <summary>
	///     Leaves out each selected member together with its name, e.g. <c>BonusTracks = []</c>.
	/// </summary>
	public TReturn Ignore()
	{
		_selection.Validate();
		return _add(new SnapshotRule(ValueRule.Ignore(_selection)));
	}
}
