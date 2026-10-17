using System;
using System.Globalization;
using System.Text.RegularExpressions;
using aweXpect.Core;
using aweXpect.Customization;
using aweXpect.Formatting;
using static aweXpect.Formatting.Format;

namespace aweXpect.Snapshots;

/// <summary>
///     Replaces a value that changes from run to run, e.g. an id or a timestamp, before the subject is compared with
///     its snapshot.
/// </summary>
/// <remarks>
///     A text masking replaces the text ordinally and inserts the replacement literally; a regex masking uses the
///     substitutions of <see cref="System.Text.RegularExpressions.Regex.Replace(string, string)" />, e.g. <c>$1</c>.
///     <see cref="Guids" /> and <see cref="Dates" /> number their placeholders, so that the same value keeps the same
///     placeholder within one subject.<br />
///     <see cref="Member(string)" /> and <see cref="Type{T}()" /> replace values nested in an object snapshot while it is
///     written, so they apply before the maskings of the text, and only to snapshots of objects; the first one that
///     matches a value wins. Numbered placeholders of the same name share their numbers across all maskings.<br />
///     Set maskings for all snapshots with <c>Customize.aweXpect.Snapshots().Maskings</c>; they are applied after the
///     maskings of the expectation.
/// </remarks>
public sealed class SnapshotMasking
{
	/// <summary>
	///     The timeout that the built-in patterns and a pattern passed as a string are parsed with, which is replaced by
	///     <c>Customize.aweXpect.Settings().DefaultRegexTimeout</c> when the masking is applied.
	/// </summary>
	private static readonly TimeSpan RegexTimeout = TimeSpan.FromSeconds(1);

	/// <summary>
	///     A <see cref="Guid" /> in the format <c>"D"</c>, also inside braces or parentheses, that is not part of a longer
	///     run of hexadecimal digits.
	/// </summary>
	/// <remarks>
	///     The format <c>"N"</c> (32 digits without hyphens) is not matched, as it cannot be told apart from a hash such
	///     as MD5. Every part has a fixed length, so the pattern cannot backtrack catastrophically.
	/// </remarks>
	private static readonly System.Text.RegularExpressions.Regex GuidPattern = new(
		"(?<![0-9A-Fa-f])[0-9A-Fa-f]{8}-[0-9A-Fa-f]{4}-[0-9A-Fa-f]{4}-[0-9A-Fa-f]{4}-[0-9A-Fa-f]{12}(?![0-9A-Fa-f])",
		RegexOptions.None, RegexTimeout);

	/// <summary>
	///     An ISO 8601 date (<c>2026-10-17</c>) or date and time (<c>2026-10-17T09:30</c>), with optional seconds, a
	///     fraction of up to seven digits and an offset or <c>Z</c>, as the round-trip format <c>"o"</c> writes a
	///     <see cref="DateTime" /> or <see cref="DateTimeOffset" />.
	/// </summary>
	/// <remarks>
	///     Months, days, hours, minutes and offsets are restricted to their ranges, so that e.g. a version number is not
	///     taken for a date. Every optional part has a bounded length, so the pattern cannot backtrack catastrophically.
	/// </remarks>
	private static readonly System.Text.RegularExpressions.Regex DatePattern = new(
		"(?<![0-9])[0-9]{4}-(?:0[1-9]|1[0-2])-(?:0[1-9]|[12][0-9]|3[01])" +
		"(?:T(?:[01][0-9]|2[0-3]):[0-5][0-9](?::[0-5][0-9](?:\\.[0-9]{1,7})?)?(?:Z|[+-](?:[01][0-9]|2[0-3]):[0-5][0-9])?)?" +
		"(?![0-9])",
		RegexOptions.None, RegexTimeout);

	private readonly bool _ignoresCase;
	private readonly string? _placeholder;
	private readonly string? _replacement;
	private readonly SnapshotValueRule? _rule;
	private readonly bool _usesDefaultTimeout;
	private readonly string _value;
	private System.Text.RegularExpressions.Regex? _regex;

	private SnapshotMasking(string value, string replacement, System.Text.RegularExpressions.Regex? regex,
		bool usesDefaultTimeout)
	{
		_value = value;
		_replacement = replacement;
		_regex = regex;
		_usesDefaultTimeout = usesDefaultTimeout;
	}

	private SnapshotMasking(System.Text.RegularExpressions.Regex regex, string placeholder, bool ignoresCase)
		: this(regex.ToString(), "", regex, true)
	{
		_placeholder = placeholder;
		_ignoresCase = ignoresCase;
	}

	private SnapshotMasking(SnapshotValueRule rule, string? replacement)
	{
		_value = "";
		_rule = rule;
		_replacement = replacement;
	}

	/// <summary>
	///     Whether the masking replaces values nested in an object snapshot while it is written, instead of the text.
	/// </summary>
	internal bool IsStructural => _rule is not null;

	/// <summary>
	///     Replaces each occurrence of the <paramref name="value" /> with the <paramref name="replacement" />, e.g. an id
	///     that is known in the test.
	/// </summary>
	/// <remarks>
	///     An empty <paramref name="replacement" /> removes the <paramref name="value" />.
	/// </remarks>
	public static SnapshotMasking Text(string value, string replacement)
	{
		if (value is null)
		{
			throw Tracing.WriteException(new ArgumentNullException(nameof(value), "The 'value' cannot be null."));
		}

		if (value.Length == 0)
		{
			throw Tracing.WriteException(new ArgumentException("The 'value' cannot be empty.", nameof(value)));
		}

		return new SnapshotMasking(value, ThrowIfNull(replacement), null, false);
	}

	/// <summary>
	///     Replaces each match of the regex <paramref name="pattern" /> with the <paramref name="replacement" />, which can
	///     use the substitutions of <see cref="System.Text.RegularExpressions.Regex.Replace(string, string)" />, e.g.
	///     <c>$1</c>.
	/// </summary>
	/// <remarks>
	///     The pattern has no options, which can be set inline (e.g. <c>(?i)</c>), and the timeout of
	///     <c>Customize.aweXpect.Settings().DefaultRegexTimeout</c> when the masking is applied; for another timeout, pass
	///     a <see cref="System.Text.RegularExpressions.Regex" /> instead. An invalid pattern throws here, where it is
	///     written, instead of when the expectation is evaluated. An empty <paramref name="replacement" /> removes the
	///     matches.
	/// </remarks>
	public static SnapshotMasking Regex(string pattern, string replacement)
	{
		if (pattern is null)
		{
			throw Tracing.WriteException(new ArgumentNullException(nameof(pattern), "The 'pattern' cannot be null."));
		}

		if (pattern.Length == 0)
		{
			throw Tracing.WriteException(new ArgumentException("The 'pattern' cannot be empty.", nameof(pattern)));
		}

		System.Text.RegularExpressions.Regex regex;
		try
		{
			regex = new System.Text.RegularExpressions.Regex(pattern, RegexOptions.None, RegexTimeout);
		}
		catch (ArgumentException exception)
		{
			throw Tracing.WriteException(new ArgumentException(
				$"The 'pattern' is not a valid regex pattern: {exception.Message}", nameof(pattern), exception));
		}

		return new SnapshotMasking(pattern, ThrowIfNull(replacement), regex, true);
	}

	/// <summary>
	///     Replaces each match of the <paramref name="pattern" /> with the <paramref name="replacement" />, which can use
	///     the substitutions of <see cref="System.Text.RegularExpressions.Regex.Replace(string, string)" />, e.g.
	///     <c>$1</c>.
	/// </summary>
	/// <remarks>
	///     The options and the timeout of the <paramref name="pattern" /> apply. An empty <paramref name="replacement" />
	///     removes the matches.
	/// </remarks>
	public static SnapshotMasking Regex(System.Text.RegularExpressions.Regex pattern, string replacement)
	{
		if (pattern is null)
		{
			throw Tracing.WriteException(new ArgumentNullException(nameof(pattern), "The 'pattern' cannot be null."));
		}

		return new SnapshotMasking(pattern.ToString(), ThrowIfNull(replacement), pattern, false);
	}

	/// <summary>
	///     Replaces each <see cref="Guid" /> with a numbered placeholder, <c>{Guid_1}</c>, <c>{Guid_2}</c> and so on, where
	///     the same <see cref="Guid" /> keeps the same number.
	/// </summary>
	/// <remarks>
	///     A <see cref="Guid" /> is matched in the format <c>"D"</c> (<c>3f2504e0-4f89-11d3-9a0c-0305e82c3301</c>) in
	///     upper or lower case, also inside braces or parentheses, which are kept. The format <c>"N"</c> without hyphens
	///     is not matched, as it cannot be told apart from a hash; mask it with <see cref="Regex(string, string)" /> if needed.<br />
	///     The numbers are counted per subject in the order of appearance, after the maskings that come before.
	/// </remarks>
	public static SnapshotMasking Guids()
		=> new(GuidPattern, "Guid", true);

	/// <summary>
	///     Replaces each date and date with time in ISO 8601 with a numbered placeholder, <c>{Date_1}</c>, <c>{Date_2}</c>
	///     and so on, where the same text keeps the same number.
	/// </summary>
	/// <remarks>
	///     It matches a date (<c>2026-10-17</c>) and a date with a time (<c>2026-10-17T09:30</c>), with optional seconds,
	///     a fraction of up to seven digits and an offset or <c>Z</c>, as the round-trip format <c>"o"</c> writes a
	///     <see cref="DateTime" />, <see cref="DateTimeOffset" /> or <c>DateOnly</c>. Culture-specific formats, a space
	///     instead of the <c>T</c> and a time on its own are not matched; mask them with <see cref="Regex(string, string)" /> if needed.
	///     <br />
	///     The numbers are counted per subject in the order of appearance, after the maskings that come before.
	/// </remarks>
	public static SnapshotMasking Dates()
		=> new(DatePattern, "Date", false);

	/// <summary>
	///     Replaces the value of each member whose path ends with the <paramref name="memberName" /> in an object snapshot
	///     with a numbered placeholder, e.g. <c>{Id_1}</c>, where the same value keeps the same number.
	/// </summary>
	/// <remarks>
	///     The <paramref name="memberName" /> matches whole segments of the path, ignoring the case, at any depth, like an
	///     ignored member of an equivalency comparison: <c>Id</c> matches <c>Id</c>, <c>Artist.Id</c> and
	///     <c>Tracks[0].Id</c>, but not <c>TrackId</c>. The placeholder is named after the last segment, e.g. <c>Id</c> for
	///     <c>Artist.Id</c>, and shares its numbers with the other maskings of that name. A <see langword="null" />
	///     value is not masked.
	/// </remarks>
	public static SnapshotMasking Member(string memberName)
		=> new(SnapshotValueRule.ForMember(memberName), null);

	/// <summary>
	///     Replaces the value of each member whose path ends with the <paramref name="memberName" /> in an object snapshot
	///     with the <paramref name="replacement" />.
	/// </summary>
	/// <remarks>
	///     The <paramref name="memberName" /> matches like in <see cref="Member(string)" />. The
	///     <paramref name="replacement" /> is written as it is, without quotes. A <see langword="null" /> value is not
	///     masked.
	/// </remarks>
	public static SnapshotMasking Member(string memberName, string replacement)
		=> new(SnapshotValueRule.ForMember(memberName), ThrowIfNull(replacement));

	/// <summary>
	///     Replaces each value of the type <typeparamref name="T" /> in an object snapshot with a numbered placeholder
	///     named after the type, e.g. <c>{Guid_1}</c>, where the same value keeps the same number.
	/// </summary>
	/// <remarks>
	///     A value matches by its declared or its runtime type, including <see cref="Nullable{T}" /> and derived types.
	///     The placeholder is named after the type without its generic arity and shares its numbers with the other
	///     maskings of that name, e.g. <c>Type&lt;Guid&gt;()</c> with <see cref="Guids" />. A <see langword="null" />
	///     value is not masked.
	/// </remarks>
	public static SnapshotMasking Type<T>()
		=> new(SnapshotValueRule.ForType(typeof(T)), null);

	/// <summary>
	///     Replaces each value of the type <typeparamref name="T" /> in an object snapshot with the
	///     <paramref name="replacement" />.
	/// </summary>
	/// <remarks>
	///     A value matches like in <see cref="Type{T}()" />. The <paramref name="replacement" /> is written as it is,
	///     without quotes. A <see langword="null" /> value is not masked.
	/// </remarks>
	public static SnapshotMasking Type<T>(string replacement)
		=> new(SnapshotValueRule.ForType(typeof(T)), ThrowIfNull(replacement));

	/// <summary>
	///     Applies the masking to the <paramref name="text" />, unless it is a structural masking.
	/// </summary>
	/// <remarks>
	///     The numbered placeholders come from the <paramref name="numbering" /> of the subject, so that they only depend
	///     on the subject itself and are shared with the other maskings of the same name.
	/// </remarks>
	internal string Apply(string text, SnapshotNumbering numbering)
	{
		if (_rule is not null)
		{
			return text;
		}

		System.Text.RegularExpressions.Regex? regex = GetRegex();
		if (regex is null)
		{
			return text.Replace(_value, _replacement!);
		}

		if (_placeholder is null)
		{
			return regex.Replace(text, _replacement!);
		}

		return regex.Replace(text, match => numbering.GetPlaceholder(_placeholder,
			_ignoresCase ? match.Value.ToLowerInvariant() : match.Value));
	}

	/// <summary>
	///     Returns the replacement of the <paramref name="value" /> nested in an object snapshot, or
	///     <see langword="null" /> when this structural masking does not apply to it.
	/// </summary>
	/// <remarks>
	///     A <see langword="null" /> value, or one that cannot be read, is not masked, so that it stays visible; the
	///     formatter writes the exception of a value that cannot be read. The key of a numbered placeholder is the text of
	///     the value, in lower case for a <see cref="Guid" /> like in <see cref="Guids" />, so that both share a number.
	/// </remarks>
	internal SnapshotPlaceholder? GetReplacement(FormattedValue value, SnapshotNumbering numbering)
	{
		if (_rule is null || !_rule.Matches(value))
		{
			return null;
		}

		object? actual;
		try
		{
			actual = value.GetValue();
		}
		catch (Exception)
		{
			return null;
		}

		if (actual is null)
		{
			return null;
		}

		return new SnapshotPlaceholder(_replacement ?? numbering.GetPlaceholder(_rule.PlaceholderName, actual switch
		{
			string text => text,
			IFormattable formattable => formattable.ToString(null, CultureInfo.InvariantCulture),
			_ => Formatter.Format(actual, FormattingOptions.Snapshot),
		}));
	}

	/// <summary>
	///     Returns the regex, with the current <c>Customize.aweXpect.Settings().DefaultRegexTimeout</c> unless it came
	///     with its own timeout from the caller.
	/// </summary>
	/// <remarks>
	///     The timeout of a <see cref="System.Text.RegularExpressions.Regex" /> is fixed when it is created, so the regex
	///     is only created again when the setting differs from the last one, e.g. within a scope that sets it. A
	///     concurrent evaluation with another setting uses its own regex and only replaces the remembered one.
	/// </remarks>
	private System.Text.RegularExpressions.Regex? GetRegex()
	{
		System.Text.RegularExpressions.Regex? regex = _regex;
		if (regex is null || !_usesDefaultTimeout)
		{
			return regex;
		}

		TimeSpan timeout = Customize.aweXpect.Settings().DefaultRegexTimeout.Get();
		if (regex.MatchTimeout != timeout)
		{
			regex = new System.Text.RegularExpressions.Regex(regex.ToString(), regex.Options, timeout);
			_regex = regex;
		}

		return regex;
	}

	private static string ThrowIfNull(string replacement)
		=> replacement ?? throw Tracing.WriteException(
			new ArgumentNullException(nameof(replacement), "The 'replacement' cannot be null."));
}
