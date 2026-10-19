using System;
using System.Text.RegularExpressions;
using aweXpect.Core;

namespace aweXpect.Snapshots;

/// <summary>
///     Selects what a snapshot rule applies to: the members or values of an object, the lines of a string, or text in
///     the written snapshot.
/// </summary>
/// <remarks>
///     A rule is written as a selection, optional filters, then exactly one action, e.g.
///     <c>.For.Members().WithName("Id").Mask()</c>. A selection only offers the actions that fit it.<br />
///     <see cref="Members" /> and <see cref="Values" /> apply while an object is written, <see cref="Lines" /> to a
///     <see cref="string" /> subject, and the other selections to the written text of every subject.
/// </remarks>
/// <typeparam name="TReturn">
///     What an action returns: the result of the expectation, or the <see cref="SnapshotRule" /> for all snapshots.
/// </typeparam>
public sealed class SnapshotFor<TReturn>
{
	private readonly Func<SnapshotRule, TReturn> _add;

	internal SnapshotFor(Func<SnapshotRule, TReturn> add)
	{
		_add = add;
	}

	/// <summary>
	///     Selects the members of the objects in an object snapshot, at any depth, e.g. to leave one out.
	/// </summary>
	/// <remarks>
	///     The items of a collection, the values of a dictionary and the items of a tuple are not members, so that e.g.
	///     <c>.For.Members().OfType&lt;DateTime&gt;().Ignore()</c> does not write a <c>List&lt;DateTime&gt;</c> as an
	///     empty list; select them with <see cref="Values" />.<br />
	///     Only for snapshots of objects: on a <see cref="string" /> subject, which is compared as raw text, the action
	///     throws an <see cref="InvalidOperationException" />.
	/// </remarks>
	public SnapshotMembers<TReturn, object> Members()
		=> new(_add, ValueSelection.Members);

	/// <summary>
	///     Selects the values in an object snapshot: the members, and the items of tuples, collections and dictionaries.
	/// </summary>
	/// <remarks>
	///     Only for snapshots of objects: on a <see cref="string" /> subject, which is compared as raw text, the action
	///     throws an <see cref="InvalidOperationException" />.
	/// </remarks>
	public SnapshotValues<TReturn, object> Values()
		=> new(_add, ValueSelection.Values);

	/// <summary>
	///     Selects the lines of a <see cref="string" /> snapshot, e.g. a timestamp or a banner.
	/// </summary>
	/// <remarks>
	///     A line ends with <c>\r\n</c>, <c>\n</c> or <c>\r</c> and is checked without it. The lines are handled before
	///     the text rules, so that a line that is left out does not use up a numbered placeholder.<br />
	///     Only for snapshots of a <see cref="string" />: an object is written by the formatter, so on any other subject
	///     the action throws an <see cref="InvalidOperationException" />.
	/// </remarks>
	public SnapshotLines<TReturn> Lines()
		=> new(_add, []);

	/// <summary>
	///     Selects each occurrence of the <paramref name="text" /> in the written snapshot, e.g. an id that is known in the
	///     test.
	/// </summary>
	/// <remarks>
	///     The <paramref name="text" /> is compared ordinally, unless <c>.IgnoringCase()</c>, <c>.AsWildcard()</c> or
	///     <c>.AsRegex()</c> follow.
	/// </remarks>
	public SnapshotText<TReturn> Text(string text)
		=> new(_add, RuleArguments.ThrowIfNullOrEmpty(text, nameof(text)));

	/// <summary>
	///     Selects each match of the <paramref name="pattern" /> in the written snapshot, with its options and timeout,
	///     e.g. a static or generated <see cref="System.Text.RegularExpressions.Regex" />.
	/// </summary>
	public SnapshotPattern<TReturn> Regex(Regex pattern)
	{
		if (pattern is null)
		{
			throw Tracing.WriteException(new ArgumentNullException(nameof(pattern), "The 'pattern' cannot be null."));
		}

		return new SnapshotPattern<TReturn>(_add, replacement => TextMasking.Pattern(pattern, replacement));
	}

	/// <summary>
	///     Selects each <see cref="Guid" /> in the written snapshot, in the format <c>"D"</c>
	///     (<c>3f2504e0-4f89-11d3-9a0c-0305e82c3301</c>) in upper or lower case.
	/// </summary>
	/// <remarks>
	///     A <see cref="Guid" /> inside braces or parentheses keeps them. The format <c>"N"</c> without hyphens is not
	///     matched, as it cannot be told apart from a hash; select it with <see cref="Regex" /> if needed.<br />
	///     <c>.Mask()</c> numbers the placeholders, <c>{Guid_1}</c>, <c>{Guid_2}</c> and so on, where the same
	///     <see cref="Guid" /> keeps the same number, also when it was masked as a value, e.g. by
	///     <c>.For.Values().OfType&lt;Guid&gt;().Mask()</c>.
	/// </remarks>
	public SnapshotKnownText<TReturn> Guids()
		=> new(_add, TextMasking.Guids);

	/// <summary>
	///     Selects each date and date with time in ISO 8601 in the written snapshot.
	/// </summary>
	/// <remarks>
	///     It matches a date (<c>2026-10-17</c>) and a date with a time (<c>2026-10-17T09:30</c>), with optional seconds,
	///     a fraction of up to seven digits and an offset or <c>Z</c>, as the round-trip format <c>"o"</c> writes a
	///     <see cref="DateTime" />, <see cref="DateTimeOffset" /> or <c>DateOnly</c>. Culture-specific formats, a space
	///     instead of the <c>T</c> and a time on its own are not matched; select them with <see cref="Regex" /> if
	///     needed.<br />
	///     <c>.Mask()</c> numbers the placeholders, <c>{Date_1}</c>, <c>{Date_2}</c> and so on, where the same text keeps
	///     the same number.
	/// </remarks>
	public SnapshotKnownText<TReturn> Dates()
		=> new(_add, TextMasking.Dates);

	/// <summary>
	///     Selects the name of this machine, <see cref="Environment.MachineName" />, in the written snapshot.
	/// </summary>
	/// <remarks>
	///     The name only matches as a whole word, ignoring the case, as host names are case-insensitive. <c>.Mask()</c>
	///     replaces it with <c>{MachineName}</c>. Like the paths of the environment, it is masked before the other text
	///     rules, so that they see the placeholder.
	/// </remarks>
	public SnapshotKnownText<TReturn> MachineName()
		=> new(_add, TextMasking.MachineName);

	/// <summary>
	///     Selects the name of the current user, <see cref="Environment.UserName" />, in the written snapshot.
	/// </summary>
	/// <remarks>
	///     The name only matches as a whole word, ignoring the case on Windows and macOS like the paths of the
	///     environment. <c>.Mask()</c> replaces it with <c>{UserName}</c>. Like the paths, it is masked before the other
	///     text rules, so that they see the placeholder; a path in the user profile therefore becomes
	///     <c>{UserProfile}</c>, unless the paths are kept.
	/// </remarks>
	public SnapshotKnownText<TReturn> UserName()
		=> new(_add, TextMasking.UserName);

	/// <summary>
	///     Selects the paths of the environment, which are masked by default, so that they can be kept.
	/// </summary>
	/// <remarks>
	///     By default, the directory of the test project is replaced with <c>{ProjectDirectory}</c>, the source root with
	///     <c>{SourceRoot}</c>, <see cref="AppContext.BaseDirectory" /> with <c>{OutputDirectory}</c>,
	///     <see cref="System.IO.Path.GetTempPath()" /> with <c>{TempPath}</c> and the profile of the user with
	///     <c>{UserProfile}</c>, so that a snapshot does not depend on the machine.<br />
	///     The longest path is replaced first, as they are nested in each other. On Windows, a path matches with <c>\</c>
	///     or <c>/</c> as separator, also with escaped backslashes (<c>\\</c>) as in a string of an object snapshot, and
	///     elsewhere with <c>/</c>. It only matches as whole segments, so <c>C:\Music\Album</c> does not match inside
	///     <c>C:\Music\Albums</c>; the rest of the path keeps its separators. The case is ignored on Windows and macOS. A
	///     root of the file system, such as <c>C:\</c> or <c>/</c>, is never masked. The paths are replaced before the
	///     other text rules, so that they see the placeholders.
	/// </remarks>
	public SnapshotPaths<TReturn> Paths()
		=> new(_add);
}
