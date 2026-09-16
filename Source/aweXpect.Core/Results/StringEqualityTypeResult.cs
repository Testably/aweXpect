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
		options);

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
