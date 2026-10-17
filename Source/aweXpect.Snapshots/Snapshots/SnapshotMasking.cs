using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using aweXpect.Core;

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
///     Set maskings for all snapshots with <c>Customize.aweXpect.Snapshots().Maskings</c>; they are applied after the
///     maskings of the expectation.
/// </remarks>
public sealed class SnapshotMasking
{
	/// <summary>
	///     The timeout of the built-in patterns and of a pattern from <c>.AsRegex()</c>, the same as for the regex matching
	///     of aweXpect.
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

	private readonly string? _placeholder;
	private readonly StringComparer _placeholderComparer = StringComparer.Ordinal;
	private readonly System.Text.RegularExpressions.Regex? _regex;
	private readonly string _replacement;
	private readonly string _value;

	private SnapshotMasking(string value, string replacement, System.Text.RegularExpressions.Regex? regex)
	{
		_value = value;
		_replacement = replacement;
		_regex = regex;
	}

	private SnapshotMasking(System.Text.RegularExpressions.Regex regex, string placeholder, StringComparer comparer)
		: this(regex.ToString(), "", regex)
	{
		_placeholder = placeholder;
		_placeholderComparer = comparer;
	}

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

		return new SnapshotMasking(value, ThrowIfNull(replacement), null);
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

		return new SnapshotMasking(pattern.ToString(), ThrowIfNull(replacement), pattern);
	}

	/// <summary>
	///     Replaces each <see cref="Guid" /> with a numbered placeholder, <c>Guid_1</c>, <c>Guid_2</c> and so on, where the
	///     same <see cref="Guid" /> keeps the same number.
	/// </summary>
	/// <remarks>
	///     A <see cref="Guid" /> is matched in the format <c>"D"</c> (<c>3f2504e0-4f89-11d3-9a0c-0305e82c3301</c>) in
	///     upper or lower case, also inside braces or parentheses, which are kept. The format <c>"N"</c> without hyphens
	///     is not matched, as it cannot be told apart from a hash; mask it with <see cref="Regex" /> if needed.<br />
	///     The numbers are counted per subject in the order of appearance, after the maskings that come before.
	/// </remarks>
	public static SnapshotMasking Guids()
		=> new(GuidPattern, "Guid", StringComparer.OrdinalIgnoreCase);

	/// <summary>
	///     Replaces each date and date with time in ISO 8601 with a numbered placeholder, <c>Date_1</c>, <c>Date_2</c> and
	///     so on, where the same text keeps the same number.
	/// </summary>
	/// <remarks>
	///     It matches a date (<c>2026-10-17</c>) and a date with a time (<c>2026-10-17T09:30</c>), with optional seconds,
	///     a fraction of up to seven digits and an offset or <c>Z</c>, as the round-trip format <c>"o"</c> writes a
	///     <see cref="DateTime" />, <see cref="DateTimeOffset" /> or <c>DateOnly</c>. Culture-specific formats, a space
	///     instead of the <c>T</c> and a time on its own are not matched; mask them with <see cref="Regex" /> if needed.
	///     <br />
	///     The numbers are counted per subject in the order of appearance, after the maskings that come before.
	/// </remarks>
	public static SnapshotMasking Dates()
		=> new(DatePattern, "Date", StringComparer.Ordinal);

	/// <summary>
	///     Interprets the value of a text masking as a regex pattern.
	/// </summary>
	/// <remarks>
	///     The pattern is parsed here, so that an invalid one is reported where it is written instead of when the
	///     expectation is evaluated. The message names the <c>value</c> of <c>.Masking(…)</c>, as <c>.AsRegex()</c> has
	///     no parameter of its own.
	/// </remarks>
	internal SnapshotMasking AsRegex()
	{
		try
		{
			return new SnapshotMasking(_value, _replacement,
				new System.Text.RegularExpressions.Regex(_value, RegexOptions.None, RegexTimeout));
		}
		catch (ArgumentException exception)
		{
			throw Tracing.WriteException(new ArgumentException(
				$"The 'value' is not a valid regex pattern: {exception.Message}", exception));
		}
	}

	/// <summary>
	///     Applies the masking to the <paramref name="text" />.
	/// </summary>
	/// <remarks>
	///     The numbers of the placeholders start at 1 for each <paramref name="text" />, so that they only depend on the
	///     subject itself.
	/// </remarks>
	internal string Apply(string text)
	{
		if (_regex is null)
		{
			return text.Replace(_value, _replacement);
		}

		if (_placeholder is null)
		{
			return _regex.Replace(text, _replacement);
		}

		Dictionary<string, string> placeholders = new(_placeholderComparer);
		return _regex.Replace(text, match =>
		{
			if (!placeholders.TryGetValue(match.Value, out string? placeholder))
			{
				placeholder = $"{_placeholder}_{placeholders.Count + 1}";
				placeholders.Add(match.Value, placeholder);
			}

			return placeholder;
		});
	}

	private static string ThrowIfNull(string replacement)
		=> replacement ?? throw Tracing.WriteException(
			new ArgumentNullException(nameof(replacement), "The 'replacement' cannot be null."));
}
