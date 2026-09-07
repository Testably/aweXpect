using System.Text.RegularExpressions;
using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect.Results;

/// <summary>
///     Allows specifying the equality type for the string equality check.
/// </summary>
public class StringEqualityTypeCountResult<TType, TThat>(
	ExpectationBuilder expectationBuilder,
	TThat returnValue,
	Quantifier quantifier,
	StringEqualityOptions options)
	: StringCountResult<TType, TThat>(expectationBuilder, returnValue, quantifier, options)
{
	private readonly ExpectationBuilder _expectationBuilder = expectationBuilder;
	private readonly StringEqualityOptions _options = options;
	private readonly Quantifier _quantifier = quantifier;
	private readonly TThat _returnValue = returnValue;

	/// <summary>
	///     Interprets the expected <see langword="string" /> as a block of lines, which may be indented as a whole.
	/// </summary>
	/// <remarks>
	///     The block must start and end at line boundaries. All its lines must share the same white-space prefix in the
	///     actual string, so the relative indentation within the block is still compared.<br />
	///     As the lines are compared individually, the indentation and the newline style can no longer be ignored.
	/// </remarks>
	public StringBlockCountResult<TType, TThat> AsBlock()
	{
		_options.AsBlock();
		return new StringBlockCountResult<TType, TThat>(_expectationBuilder, _returnValue, _quantifier, _options);
	}

	/// <summary>
	///     Interprets the expected <see langword="string" /> to be exactly equal.
	/// </summary>
	public StringCountResult<TType, TThat> Exactly()
	{
		_options.Exactly();
		return this;
	}

	/// <summary>
	///     Interprets the expected <see langword="string" /> as a prefix, so that the actual value starts with it.
	/// </summary>
	public StringCountResult<TType, TThat> AsPrefix()
	{
		_options.AsPrefix();
		return this;
	}

	/// <summary>
	///     Interprets the expected <see langword="string" /> as <see cref="Regex" /> pattern.
	/// </summary>
	public StringCountResult<TType, TThat> AsRegex()
	{
		_options.AsRegex();
		return this;
	}

	/// <summary>
	///     Interprets the expected <see langword="string" /> as a suffix, so that the actual value ends with it.
	/// </summary>
	public StringCountResult<TType, TThat> AsSuffix()
	{
		_options.AsSuffix();
		return this;
	}

	/// <summary>
	///     Interprets the expected <see langword="string" /> as wildcard pattern.<br />
	///     Supports * to match zero or more characters and ? to match exactly one character.
	/// </summary>
	public StringCountResult<TType, TThat> AsWildcard()
	{
		_options.AsWildcard();
		return this;
	}
}
