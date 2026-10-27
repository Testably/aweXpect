using System;

namespace aweXpect.Snapshots;

/// <summary>
///     Selects what a rule for all snapshots applies to: the members or values of an object, the lines of a string, or
///     text in the written snapshot.
/// </summary>
/// <remarks>
///     A rule is written as a selection, optional filters, then exactly one action, e.g.
///     <c>SnapshotRule.For.Members().WithName("Id").Mask()</c>. A selection only offers the actions that fit it.<br />
///     <see cref="Members" /> and <see cref="Values" /> apply while an object is written, <see cref="Lines" /> to a
///     <see cref="string" /> subject, and the other selections to the written text of every subject. A rule that does
///     not apply to a subject is ignored there.
/// </remarks>
/// <typeparam name="TResult">What an action returns, the <see cref="SnapshotRule" />.</typeparam>
public sealed class SnapshotFor<TResult> : SnapshotTextFor<TResult>
{
	internal SnapshotFor(Func<SnapshotRule, TResult> add) : base(add)
	{
	}

	/// <summary>
	///     Selects the members of the objects in an object snapshot, at any depth, e.g. to leave one out.
	/// </summary>
	/// <remarks>
	///     The items of a collection, the values of a dictionary and the items of a tuple are not members; select them
	///     with <see cref="Values" />. It is ignored for a snapshot of a <see cref="string" />, which is compared as raw
	///     text.
	/// </remarks>
	public SnapshotMembers<TResult, object> Members()
		=> new(Add, ValueSelection.Members);

	/// <summary>
	///     Selects the values in an object snapshot: the members, and the items of tuples, collections and dictionaries.
	/// </summary>
	/// <remarks>
	///     It is ignored for a snapshot of a <see cref="string" />, which is compared as raw text.
	/// </remarks>
	public SnapshotValues<TResult, object> Values()
		=> new(Add, ValueSelection.Values);

	/// <summary>
	///     Selects the lines of a <see cref="string" /> snapshot, e.g. a timestamp or a banner.
	/// </summary>
	/// <remarks>
	///     A line ends with <c>\r\n</c>, <c>\n</c> or <c>\r</c> and is checked without it. The lines are handled before
	///     the text rules, so that a line that is left out does not use up a numbered placeholder.<br />
	///     It is ignored for a snapshot of an object, which is written by the formatter.
	/// </remarks>
	public SnapshotLines<TResult> Lines()
		=> new(Add, []);
}
