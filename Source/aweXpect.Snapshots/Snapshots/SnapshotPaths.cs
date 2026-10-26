using System;

namespace aweXpect.Snapshots;

/// <summary>
///     The paths of the environment, see <see cref="SnapshotTextFor{TResult}.Paths" />, which are masked by default and can
///     only be kept.
/// </summary>
/// <typeparam name="TResult">What an action returns.</typeparam>
public sealed class SnapshotPaths<TResult>
{
	private readonly Func<SnapshotRule, TResult> _add;

	internal SnapshotPaths(Func<SnapshotRule, TResult> add)
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
	public TResult Keep()
		=> _add(SnapshotRule.KeepPaths);
}
