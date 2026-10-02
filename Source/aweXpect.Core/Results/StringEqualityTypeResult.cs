using System.Text.RegularExpressions;
using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect.Results;

#pragma warning disable S110 // The result hierarchy is intentionally deep, so that each continuation inherits the complete vocabulary of its base
/// <summary>
///     Allows specifying the equality type for the string equality check.
/// </summary>
public class StringEqualityTypeResult<TType, TThat>(
	ExpectationBuilder expectationBuilder,
	TThat returnValue,
	StringEqualityOptions options)
	: StringEqualityTypeResult<TType, TThat,
		StringEqualityTypeResult<TType, TThat>>(
		expectationBuilder,
		returnValue,
		options)
{
	private readonly StringEqualityOptions _options = options;
	private readonly TThat _returnValue = returnValue;

	/// <summary>
	///     Interprets the expected <see langword="string" /> as a block of lines, which may be indented as a whole.
	/// </summary>
	/// <remarks>
	///     The block must consist of the same lines as the actual string. All its lines must share the same whitespace
	///     prefix in the actual string, so the relative indentation within the block is still compared.<br />
	///     A line that consists only of whitespace matches any line that consists only of whitespace.<br />
	///     The newline style is always ignored, and a single trailing line terminator does not start a new line,
	///     so <c>"a\nb\n"</c> has the same two lines as <c>"a\nb"</c>.
	/// </remarks>
	public StringBlockResult<TType, TThat> AsBlock()
	{
		_options.AsBlock();
		return new StringBlockResult<TType, TThat>(ExpectationBuilder, _returnValue, _options);
	}
}

/// <summary>
///     Allows specifying the equality type for the string equality check.
/// </summary>
public class StringEqualityTypeResult<TType, TThat, TSelf>(
	ExpectationBuilder expectationBuilder,
	TThat returnValue,
	StringEqualityOptions options)
	: StringEqualityResult<TType, TThat>(
		expectationBuilder,
		returnValue,
		options)
	where TSelf : StringEqualityTypeResult<TType, TThat, TSelf>
{
	private readonly StringEqualityOptions _options = options;

	/// <summary>
	///     Interprets the expected <see langword="string" /> as a prefix, so that the actual value starts with it.
	/// </summary>
	public TSelf AsPrefix()
	{
		_options.AsPrefix();
		return (TSelf)this;
	}

	/// <summary>
	///     Interprets the expected <see langword="string" /> as <see cref="Regex" /> pattern.
	/// </summary>
	public TSelf AsRegex()
	{
		_options.AsRegex();
		return (TSelf)this;
	}

	/// <summary>
	///     Interprets the expected <see langword="string" /> as <see cref="Regex" /> pattern,
	///     applying the given <paramref name="regexOptions" />.
	/// </summary>
	public TSelf AsRegex(RegexOptions regexOptions)
	{
		_options.AsRegex(regexOptions);
		return (TSelf)this;
	}

	/// <summary>
	///     Interprets the expected <see langword="string" /> as a suffix, so that the actual value ends with it.
	/// </summary>
	public TSelf AsSuffix()
	{
		_options.AsSuffix();
		return (TSelf)this;
	}

	/// <summary>
	///     Interprets the expected <see langword="string" /> as wildcard pattern.<br />
	///     Supports * to match zero or more characters and ? to match exactly one character.
	/// </summary>
	public TSelf AsWildcard()
	{
		_options.AsWildcard();
		return (TSelf)this;
	}
}
