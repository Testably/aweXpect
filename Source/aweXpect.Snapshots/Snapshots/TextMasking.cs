using System;
using System.Text.RegularExpressions;
using aweXpect.Core;
using aweXpect.Customization;

namespace aweXpect.Snapshots;

/// <summary>
///     Replaces text in the written subject, e.g. an id, a <see cref="Guid" /> or a path of the environment, with a
///     mask.
/// </summary>
/// <remarks>
///     The mask is a template: <c>{0}</c> in it is replaced with the number of the matched text from the
///     <see cref="SnapshotNumbering" /> of the subject, so that the same text keeps the same number. Only a regex
///     pattern of the user applies the substitutions of <see cref="Regex.Replace(string, string)" />, e.g. <c>$1</c>,
///     before the number; any other mask is inserted literally.
/// </remarks>
internal sealed class TextMasking
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
	private static readonly Regex GuidPattern = new(
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
	private static readonly Regex DatePattern = new(
		"(?<![0-9])[0-9]{4}-(?:0[1-9]|1[0-2])-(?:0[1-9]|[12][0-9]|3[01])" +
		"(?:T(?:[01][0-9]|2[0-3]):[0-5][0-9](?::[0-5][0-9](?:\\.[0-9]{1,7})?)?(?:Z|[+-](?:[01][0-9]|2[0-3]):[0-5][0-9])?)?" +
		"(?![0-9])",
		RegexOptions.None, RegexTimeout);

	private readonly bool _ignoresCase;
	private readonly string? _literal;
	private readonly string _template;
	private readonly bool _usesDefaultTimeout;
	private readonly bool _usesSubstitutions;
	private Regex? _regex;

	private TextMasking(string literal, string template)
	{
		_literal = literal;
		_template = template;
	}

	private TextMasking(Regex regex, string template, bool usesDefaultTimeout, bool usesSubstitutions,
		bool ignoresCase = false)
	{
		_regex = regex;
		_template = template;
		_usesDefaultTimeout = usesDefaultTimeout;
		_usesSubstitutions = usesSubstitutions;
		_ignoresCase = ignoresCase;
	}

	/// <summary>
	///     Whether the masking replaces a value of the environment, which is applied before the other text maskings.
	/// </summary>
	public bool IsEnvironment { get; private init; }

	/// <summary>
	///     Replaces each occurrence of the <paramref name="value" />, compared ordinally, with the
	///     <paramref name="template" />.
	/// </summary>
	public static TextMasking Literal(string value, string template)
		=> new(value, template);

	/// <summary>
	///     Replaces each match of the <paramref name="pattern" />, which is parsed with the <paramref name="options" />
	///     and the default regex timeout, with the <paramref name="template" />.
	/// </summary>
	/// <remarks>
	///     An invalid <paramref name="pattern" /> throws an <see cref="ArgumentException" /> for the
	///     <paramref name="parameterName" />, where the rule is written. With <see cref="RegexOptions.IgnoreCase" />, the
	///     matches that only differ in case share a number.
	/// </remarks>
	public static TextMasking Pattern(string pattern, RegexOptions options, string template,
		bool usesSubstitutions, string parameterName)
	{
		Regex regex;
		try
		{
			regex = new Regex(pattern, options | RegexOptions.CultureInvariant, RegexTimeout);
		}
		catch (ArgumentException exception)
		{
			throw Tracing.WriteException(new ArgumentException(
				$"The '{parameterName}' is not a valid regex pattern: {exception.Message}", parameterName, exception));
		}

		return new TextMasking(regex, template, true, usesSubstitutions,
			(options & RegexOptions.IgnoreCase) != 0);
	}

	/// <summary>
	///     Replaces each match of the <paramref name="regex" />, with its own options and timeout, with the
	///     <paramref name="template" />, which can use substitutions.
	/// </summary>
	public static TextMasking Pattern(Regex regex, string template)
		=> new(regex, template, false, true);

	/// <summary>
	///     Replaces each <see cref="Guid" /> with the <paramref name="template" />, by default <c>{Guid_{0}}</c>.
	/// </summary>
	/// <remarks>
	///     The key of the number is in lower case, so that the same <see cref="Guid" /> in upper and lower case, or as a
	///     member, keeps the same number.
	/// </remarks>
	public static TextMasking Guids(string? template)
		=> new(GuidPattern, template ?? SnapshotNumbering.GetTemplate("Guid"), true, false, true);

	/// <summary>
	///     Replaces each ISO 8601 date with the <paramref name="template" />, by default <c>{Date_{0}}</c>.
	/// </summary>
	public static TextMasking Dates(string? template)
		=> new(DatePattern, template ?? SnapshotNumbering.GetTemplate("Date"), true, false);

	/// <summary>
	///     Replaces the name of this machine as a whole word, ignoring the case, with <c>{MachineName}</c> or the
	///     <paramref name="template" />.
	/// </summary>
	public static TextMasking MachineName(string? template)
		=> Word(Environment.MachineName, RegexOptions.IgnoreCase, template ?? "{MachineName}");

	/// <summary>
	///     Replaces the name of the current user as a whole word, ignoring the case like the paths of the environment,
	///     with <c>{UserName}</c> or the <paramref name="template" />.
	/// </summary>
	public static TextMasking UserName(string? template)
		=> Word(Environment.UserName, EnvironmentMaskings.PathOptions, template ?? "{UserName}");

	/// <summary>
	///     Replaces the <paramref name="path" /> of the environment with the <paramref name="placeholder" />, see
	///     <see cref="EnvironmentMaskings" />.
	/// </summary>
	public static TextMasking ForPath(string path, string placeholder)
		=> Environmental(EnvironmentMaskings.GetPathPattern(path), EnvironmentMaskings.PathOptions, placeholder);

	/// <summary>
	///     Applies the masking to the <paramref name="text" />.
	/// </summary>
	/// <remarks>
	///     The numbers come from the <paramref name="numbering" /> of the subject, so that they only depend on the subject
	///     itself and are shared with the other masks of the same template. A literal that does not occur uses up no
	///     number.
	/// </remarks>
	public string Apply(string text, SnapshotNumbering numbering)
	{
		Regex? regex = GetRegex();
		if (regex is null)
		{
			return text.IndexOf(_literal!, StringComparison.Ordinal) < 0
				? text
				: text.Replace(_literal!, numbering.Apply(_template, _literal!));
		}

		return regex.Replace(text, match => numbering.Apply(
			_usesSubstitutions ? match.Result(_template) : _template,
			_ignoresCase ? match.Value.ToLowerInvariant() : match.Value));
	}

	/// <summary>
	///     Returns the regex, with the current <c>Customize.aweXpect.Settings().DefaultRegexTimeout</c> unless it came
	///     with its own timeout from the caller.
	/// </summary>
	/// <remarks>
	///     The timeout of a <see cref="Regex" /> is fixed when it is created, so the regex is only created again when the
	///     setting differs from the last one, e.g. within a scope that sets it. A concurrent evaluation with another
	///     setting uses its own regex and only replaces the remembered one.
	/// </remarks>
	private Regex? GetRegex()
	{
		Regex? regex = _regex;
		if (regex is null || !_usesDefaultTimeout)
		{
			return regex;
		}

		TimeSpan timeout = Customize.aweXpect.Settings().DefaultRegexTimeout.Get();
		if (regex.MatchTimeout != timeout)
		{
			regex = new Regex(regex.ToString(), regex.Options, timeout);
			_regex = regex;
		}

		return regex;
	}

	/// <remarks>
	///     An empty <paramref name="word" /> matches nothing.
	/// </remarks>
	private static TextMasking Word(string word, RegexOptions options, string template)
		=> Environmental(word.Length == 0 ? "(?!)" : $@"(?<!\w){Regex.Escape(word)}(?!\w)", options, template);

	private static TextMasking Environmental(string pattern, RegexOptions options, string template)
		=> new(new Regex(pattern, options | RegexOptions.CultureInvariant, RegexTimeout), template, true, false,
			(options & RegexOptions.IgnoreCase) != 0)
		{
			IsEnvironment = true,
		};
}
