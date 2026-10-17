using aweXpect.Formatting;

namespace aweXpect.Snapshots;

/// <summary>
///     Leaves values out of an object snapshot, e.g. a member that changes from run to run and is irrelevant for the
///     test.
/// </summary>
/// <remarks>
///     A value that is left out disappears together with its name. Ignoring wins over masking.<br />
///     Set ignorings for all snapshots with <c>Customize.aweXpect.Snapshots().Ignorings</c>; they are applied after the
///     ignorings of the expectation. They only apply to snapshots of objects, as a <see cref="string" /> is compared as
///     raw text.
/// </remarks>
public sealed class SnapshotIgnoring
{
	private readonly bool _isMembersOnly;
	private readonly SnapshotValueRule _rule;

	private SnapshotIgnoring(SnapshotValueRule rule, bool isMembersOnly)
	{
		_rule = rule;
		_isMembersOnly = isMembersOnly;
	}

	/// <summary>
	///     Leaves out each member whose path ends with the <paramref name="memberName" />.
	/// </summary>
	/// <remarks>
	///     The <paramref name="memberName" /> matches whole segments of the path, ignoring the case, at any depth, like an
	///     ignored member of an equivalency comparison: <c>Id</c> matches <c>Id</c>, <c>Artist.Id</c> and
	///     <c>Tracks[0].Id</c>, but not <c>TrackId</c>; <c>Artist.Id</c> only matches the <c>Id</c> of an <c>Artist</c>.
	/// </remarks>
	public static SnapshotIgnoring Member(string memberName)
		=> new(SnapshotValueRule.ForMember(memberName), false);

	/// <summary>
	///     Leaves out each member of the type <typeparamref name="T" />.
	/// </summary>
	/// <remarks>
	///     A member matches by its declared or its runtime type, including <see cref="System.Nullable{T}" /> and derived
	///     types, e.g. <c>Type&lt;DateTime&gt;()</c> also leaves out a <c>DateTime?</c> member and a
	///     <see cref="System.DateTime" /> in an <see cref="object" /> member.<br />
	///     The items of a collection, the values of a dictionary and the items of a tuple are kept, so that a
	///     <c>List&lt;DateTime&gt;</c> is not written as an empty list; mask them with <c>SnapshotMasking.Type&lt;T&gt;()</c>
	///     instead.
	/// </remarks>
	public static SnapshotIgnoring Member<T>()
		=> new(SnapshotValueRule.ForType(typeof(T)), true);

	internal bool Matches(FormattedValue value)
		=> (!_isMembersOnly || value.Kind == FormattedValueKind.Member) && _rule.Matches(value);
}
