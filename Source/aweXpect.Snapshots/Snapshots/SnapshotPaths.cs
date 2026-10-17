using System;

namespace aweXpect.Snapshots;

/// <summary>
///     The paths of the environment, see <see cref="SnapshotFor{TReturn}.Paths" />, which are masked by default and can
///     only be kept.
/// </summary>
/// <typeparam name="TReturn">What an action returns.</typeparam>
public sealed class SnapshotPaths<TReturn>
{
	private readonly Func<SnapshotRule, TReturn> _add;

	internal SnapshotPaths(Func<SnapshotRule, TReturn> add)
	{
		_add = add;
	}

	/// <summary>
	///     Keeps the paths of the environment as they are, instead of masking them.
	/// </summary>
	/// <remarks>
	///     On an expectation, it keeps the paths in its snapshot; in <c>Customize.aweXpect.Snapshots().Rules</c>, in every
	///     snapshot. An expectation cannot mask the paths again while a rule for all snapshots keeps them; set the rules
	///     without it in a scope instead.
	/// </remarks>
	public TReturn Keep()
		=> _add(SnapshotRule.KeepPaths);
}
