using System;
using System.Text.RegularExpressions;

namespace aweXpect.Snapshots;

/// <summary>
///     The occurrences of a text in the written snapshot, see <see cref="SnapshotTextFor{TResult}.Text" />, which also
///     allows setting how the text is matched.
/// </summary>
/// <remarks>
///     The text is compared ordinally, unless <see cref="IgnoringCase" />, <see cref="AsWildcard" /> or
///     <see cref="AsRegex" /> follow. A pattern uses the timeout of
///     <c>Customize.aweXpect.Settings().DefaultRegexTimeout</c>, and a pattern that times out fails the expectation.
/// </remarks>
/// <typeparam name="TResult">What an action returns.</typeparam>
public sealed class SnapshotText<TResult> : SnapshotPattern<TResult>
{
	private readonly string _text;
	private bool _ignoreCase;
	private Kind _kind;

	internal SnapshotText(Func<SnapshotRule, TResult> add, string text)
		: base(add, null)
	{
		_text = text;
	}

	/// <summary>
	///     Interprets the text as a regex pattern, whose matches are replaced with the substitutions of
	///     <see cref="Regex.Replace(string, string)" />, e.g. <c>$1</c> for the first group.
	/// </summary>
	/// <remarks>
	///     An invalid pattern throws an <see cref="ArgumentException" /> at the action. For options or a timeout of its
	///     own, use <see cref="SnapshotTextFor{TResult}.Regex" /> instead.
	/// </remarks>
	public SnapshotText<TResult> AsRegex()
	{
		_kind = Kind.Regex;
		return this;
	}

	/// <summary>
	///     Interprets the text as a wildcard pattern, where <c>*</c> matches zero or more characters and <c>?</c> exactly
	///     one character, within a line.
	/// </summary>
	/// <remarks>
	///     Unlike in aweXpect's string comparison, a wildcard does not match a line break, so that <c>*</c> cannot mask
	///     the rest of the snapshot.
	/// </remarks>
	public SnapshotText<TResult> AsWildcard()
	{
		_kind = Kind.Wildcard;
		return this;
	}

	/// <summary>
	///     Ignores the case of the text when <paramref name="ignoreCase" /> is <see langword="true" />.
	/// </summary>
	public SnapshotText<TResult> IgnoringCase(bool ignoreCase = true)
	{
		_ignoreCase = ignoreCase;
		return this;
	}

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
