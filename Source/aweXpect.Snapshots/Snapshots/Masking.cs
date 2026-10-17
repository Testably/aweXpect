using System;
using System.Text.RegularExpressions;
using aweXpect.Core;

namespace aweXpect.Snapshots;

/// <summary>
///     Replaces a value that changes from run to run, e.g. an id or a timestamp, before the subject is compared with
///     its snapshot.
/// </summary>
/// <remarks>
///     A literal masking replaces the text ordinally and inserts the replacement literally; a regex masking uses the
///     substitutions of <see cref="Regex.Replace(string, string)" />, e.g. <c>$1</c>.
/// </remarks>
internal sealed class Masking
{
	/// <summary>
	///     The timeout of a pattern from <see cref="AsRegex" />, the same as for the regex matching of aweXpect.
	/// </summary>
	private static readonly TimeSpan RegexTimeout = TimeSpan.FromSeconds(1);

	private readonly string _replacement;
	private readonly string _value;
	private Regex? _regex;

	public Masking(string value, string replacement)
	{
		_value = value;
		_replacement = replacement;
	}

	public Masking(Regex regex, string replacement)
	{
		_value = regex.ToString();
		_replacement = replacement;
		_regex = regex;
	}

	/// <summary>
	///     Interprets the value as a regex pattern.
	/// </summary>
	/// <remarks>
	///     The pattern is parsed here, so that an invalid one is reported where it is written instead of when the
	///     expectation is evaluated. The message names the <c>value</c> of <c>.Masking(…)</c>, as <c>.AsRegex()</c> has
	///     no parameter of its own.
	/// </remarks>
	public void AsRegex()
	{
		try
		{
			_regex = new Regex(_value, RegexOptions.None, RegexTimeout);
		}
		catch (ArgumentException exception)
		{
			throw Tracing.WriteException(new ArgumentException(
				$"The 'value' is not a valid regex pattern: {exception.Message}", exception));
		}
	}

	public string Apply(string text)
		=> _regex is null
			? text.Replace(_value, _replacement)
			: _regex.Replace(text, _replacement);
}
