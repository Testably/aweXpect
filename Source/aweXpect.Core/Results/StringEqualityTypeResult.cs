using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect.Results;

/// <summary>
///     The result of a <see langword="string" /> equality check, which in addition to the options on the
///     <see cref="StringEqualityOptions" /> allows specifying the match type, e.g. via
///     <see cref="StringEqualityOptionsExtensions.AsWildcard{TResult}(TResult)" />.
/// </summary>
public class StringEqualityTypeResult<TType, TThat>(
	ExpectationBuilder expectationBuilder,
	TThat returnValue,
	StringEqualityOptions options)
	: AndOrResult<TType, TThat, StringEqualityTypeResult<TType, TThat>>(expectationBuilder, returnValue),
		IOptionsProvider<StringEqualityOptions>,
		IStringMatchTypeOptions
{
	private readonly TThat _returnValue = returnValue;

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	StringEqualityOptions IOptionsProvider<StringEqualityOptions>.Options => options;

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
	/// <exception cref="System.InvalidOperationException">
	///     A match type or an option that changes the lines, e.g. the indentation, is already specified.
	/// </exception>
	public StringBlockResult<TType, TThat> AsBlock()
	{
		options.AsBlock();
		return new StringBlockResult<TType, TThat>(ExpectationBuilder, _returnValue, options);
	}
}
