using System;

namespace aweXpect.Snapshots;

/// <summary>
///     Selects what a rule of a <see cref="string" /> snapshot applies to: its lines, or text in it.
/// </summary>
/// <remarks>
///     A rule is written as a selection, optional filters, then exactly one action, e.g.
///     <c>.For.Lines().Containing("elapsed").Ignore()</c>. A selection only offers the actions that fit it.
/// </remarks>
/// <typeparam name="TResult">What an action returns, the result of the expectation.</typeparam>
public sealed class StringSnapshotFor<TResult> : SnapshotTextFor<TResult>
{
	internal StringSnapshotFor(Func<SnapshotRule, TResult> add) : base(add)
	{
	}

	/// <summary>
	///     Selects the lines of a <see cref="string" /> snapshot, e.g. a timestamp or a banner.
	/// </summary>
	/// <remarks>
	///     A line ends with <c>\r\n</c>, <c>\n</c> or <c>\r</c> and is checked without it. The lines are handled before
	///     the text rules, so that a line that is left out does not use up a numbered placeholder.
	/// </remarks>
	public SnapshotLines<TResult> Lines()
		=> new(Add, []);
}
