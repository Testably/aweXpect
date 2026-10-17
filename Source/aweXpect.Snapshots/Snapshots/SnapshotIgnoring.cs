using System;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Formatting;

namespace aweXpect.Snapshots;

/// <summary>
///     Leaves values out of a snapshot, e.g. a member of an object or a line of a string that changes from run to run
///     and is irrelevant for the test.
/// </summary>
/// <remarks>
///     A member that is left out disappears together with its name. Ignoring wins over masking.<br />
///     Set ignorings for all snapshots with <c>Customize.aweXpect.Snapshots().Ignorings</c>; they are applied together
///     with the ignorings of the expectation. Member and type rules only apply to snapshots of objects, as a
///     <see cref="string" /> is compared as raw text, and line rules only to snapshots of a <see cref="string" />.
/// </remarks>
public class SnapshotIgnoring
{
	private readonly bool _isMembersOnly;
	private readonly Func<string, bool>? _linePredicate;
	private readonly SnapshotValueRule? _rule;

	private SnapshotIgnoring(SnapshotValueRule rule, bool isMembersOnly)
	{
		_rule = rule;
		_isMembersOnly = isMembersOnly;
	}

	private SnapshotIgnoring(Func<string, bool> linePredicate)
	{
		_linePredicate = linePredicate;
	}

	/// <summary>
	///     Initializes a line rule that decides about each line in <see cref="MatchesLine(string)" />.
	/// </summary>
	private protected SnapshotIgnoring()
	{
	}

	/// <summary>
	///     Whether the ignoring removes lines from a snapshot of a <see cref="string" />, instead of values nested in an
	///     object snapshot.
	/// </summary>
	internal bool IsLineRule => _rule is null;

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

	/// <summary>
	///     Leaves out each line of a <see cref="string" /> snapshot for which the <paramref name="predicate" /> returns
	///     <see langword="true" />, e.g. a timestamp or a banner.
	/// </summary>
	/// <remarks>
	///     The <paramref name="predicate" /> gets each line without its line break (<c>\r\n</c>, <c>\n</c> or <c>\r</c>). A
	///     removed line takes its line break with it, and removing the last line also removes the line break before it;
	///     the other lines keep theirs.<br />
	///     The lines are removed before the maskings are applied, so that they don't use up numbered placeholders.
	/// </remarks>
	public static SnapshotIgnoring Lines(Func<string, bool> predicate)
	{
		if (predicate is null)
		{
			throw Tracing.WriteException(new ArgumentNullException(nameof(predicate),
				"The 'predicate' cannot be null."));
		}

		return new SnapshotIgnoring(predicate);
	}

	/// <summary>
	///     Leaves out each line of a <see cref="string" /> snapshot that contains the <paramref name="text" />, e.g.
	///     <c>"elapsed"</c>.
	/// </summary>
	/// <remarks>
	///     The <paramref name="text" /> can be anywhere in the line and is compared ordinally, like in aweXpect's
	///     <c>Contains</c>. The options that follow apply to this line rule only, not to the comparison with the snapshot:
	///     <c>.IgnoringCase()</c> ignores the case of the <paramref name="text" />, and <c>.AsWildcard()</c> or
	///     <c>.AsRegex()</c> interpret it as a pattern, with the timeout of
	///     <c>Customize.aweXpect.Settings().DefaultRegexTimeout</c>. An invalid pattern throws an
	///     <see cref="ArgumentException" /> when the expectation is evaluated, and a pattern that times out fails it.<br />
	///     Lines are removed like by <see cref="Lines(Func{string, bool})" />.
	/// </remarks>
	public static SnapshotIgnoringLinesContaining LinesContaining(string text)
	{
		if (text is null)
		{
			throw Tracing.WriteException(new ArgumentNullException(nameof(text), "The 'text' cannot be null."));
		}

		if (text.Length == 0)
		{
			throw Tracing.WriteException(new ArgumentException("The 'text' cannot be empty.", nameof(text)));
		}

		return new SnapshotIgnoringLinesContaining(text);
	}

	internal bool Matches(FormattedValue value)
		=> _rule is not null && (!_isMembersOnly || value.Kind == FormattedValueKind.Member) && _rule.Matches(value);

	/// <summary>
	///     Throws when the line rule cannot be applied, e.g. for an invalid regex pattern, also when no line is checked.
	/// </summary>
	internal virtual void ValidateLineRule()
	{
	}

	/// <summary>
	///     Whether the line rule removes the <paramref name="line" />, which has no line break.
	/// </summary>
	internal virtual ValueTask<bool> MatchesLine(string line)
		=> new(_linePredicate is not null && _linePredicate(line));
}
