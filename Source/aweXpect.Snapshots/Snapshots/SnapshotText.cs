using System;
using System.Text.RegularExpressions;

namespace aweXpect.Snapshots;

/// <summary>
///     The occurrences of a text in the written snapshot, see <see cref="SnapshotFor{TResult}.Text" />, which also
///     allows setting how the text is matched.
/// </summary>
/// <remarks>
///     The text is compared ordinally, unless <see cref="IgnoringCase" />, <see cref="AsWildcard" /> or
///     <see cref="AsRegex" /> follow. A pattern uses the timeout of
///     <c>Customize.aweXpect.Settings().DefaultRegexTimeout</c>, and a pattern that times out fails the expectation.
///     Each option returns a new selection, so a stored selection can be continued more than once and never changes a
///     rule created from it.
/// </remarks>
/// <typeparam name="TResult">What an action returns.</typeparam>
public sealed class SnapshotText<TResult> : SnapshotPattern<TResult>
{
	private readonly Func<SnapshotRule, TResult> _add;
	private readonly bool _ignoreCase;
	private readonly Kind _kind;
	private readonly string _text;

	internal SnapshotText(Func<SnapshotRule, TResult> add, string text)
		: this(add, text, false, Kind.Literal)
	{
	}

	private SnapshotText(Func<SnapshotRule, TResult> add, string text, bool ignoreCase, Kind kind)
		: base(add, null)
	{
		_add = add;
		_text = text;
		_ignoreCase = ignoreCase;
		_kind = kind;
	}

	/// <summary>
	///     Interprets the text as a regex pattern, whose matches are replaced with the substitutions of
	///     <see cref="Regex.Replace(string, string)" />, e.g. <c>$1</c> for the first group.
	/// </summary>
	/// <remarks>
	///     An invalid pattern throws an <see cref="ArgumentException" /> at the action. For options or a timeout of its
	///     own, use <see cref="SnapshotFor{TResult}.Regex" /> instead.
	/// </remarks>
	public SnapshotText<TResult> AsRegex()
		=> new(_add, _text, _ignoreCase, Kind.Regex);

	/// <summary>
	///     Interprets the text as a wildcard pattern, where <c>*</c> matches zero or more characters and <c>?</c> exactly
	///     one character, within a line.
	/// </summary>
	/// <remarks>
	///     Unlike in aweXpect's string comparison, a wildcard does not match a line break, so that <c>*</c> cannot mask
	///     the rest of the snapshot.
	/// </remarks>
	public SnapshotText<TResult> AsWildcard()
		=> new(_add, _text, _ignoreCase, Kind.Wildcard);

	/// <summary>
	///     Ignores the case of the text when <paramref name="ignoreCase" /> is <see langword="true" />.
	/// </summary>
	public SnapshotText<TResult> IgnoringCase(bool ignoreCase = true)
		=> new(_add, _text, ignoreCase, _kind);

	private protected override TextMasking CreateMasking(string template)
	{
		string text = _text;
		RegexOptions options = _ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None;
		return _kind switch
		{
			Kind.Regex => TextMasking.Pattern(text, options, template, true, nameof(text)),
			Kind.Wildcard => TextMasking.Pattern(
				Regex.Escape(text).Replace(@"\*", @"[^\r\n]*").Replace(@"\?", @"(?>[\uD800-\uDBFF][\uDC00-\uDFFF]|[^\r\n])"),
				options, template, false, nameof(text)),
			_ when _ignoreCase => TextMasking.Pattern(Regex.Escape(text), options, template, false, nameof(text)),
			_ => TextMasking.Literal(text, template),
		};
	}

	private enum Kind
	{
		Literal,
		Wildcard,
		Regex,
	}
}
