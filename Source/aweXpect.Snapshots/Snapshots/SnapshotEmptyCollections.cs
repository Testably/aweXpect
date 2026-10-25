using System;

namespace aweXpect.Snapshots;

/// <summary>
///     The members whose value is an empty collection, see
///     <see cref="SnapshotMembers{TResult, TValue}.WhichAreEmptyCollections" />, which can only be left out.
/// </summary>
/// <typeparam name="TResult">What an action returns.</typeparam>
public sealed class SnapshotEmptyCollections<TResult>
{
	private readonly Func<SnapshotRule, TResult> _add;
	private readonly ValueSelection _selection;

	internal SnapshotEmptyCollections(Func<SnapshotRule, TResult> add, ValueSelection selection)
	{
		_add = add;
		_selection = selection;
	}

	/// <summary>
	///     Leaves out each selected member together with its name, e.g. <c>BonusTracks = []</c>.
	/// </summary>
	public TResult Ignore()
	{
		_selection.Validate();
		return _add(new SnapshotRule(ValueRule.Ignore(_selection)));
	}
}
