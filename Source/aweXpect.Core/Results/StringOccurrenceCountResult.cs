using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect.Results;

/// <summary>
///     The result for verifying how often a <see langword="string" /> occurs within a <see langword="string" />, which
///     in addition to the options of a <see cref="StringCountResult{TType,TThat}" /> allows interpreting the expected
///     <see langword="string" /> as a pattern, e.g. via
///     <see cref="StringEqualityOptionsExtensions.AsWildcard{TResult}(TResult)" />.
/// </summary>
/// <remarks>
///     An occurrence can be anywhere within the <see langword="string" />, so the expected <see langword="string" />
///     cannot be interpreted as a prefix or suffix.
/// </remarks>
public class StringOccurrenceCountResult<TType, TThat>(
	ExpectationBuilder expectationBuilder,
	TThat returnValue,
	Quantifier quantifier,
	StringEqualityOptions options)
	: AndOrResult<TType, TThat, StringOccurrenceCountResult<TType, TThat>>(expectationBuilder, returnValue),
		IOptionsProvider<Quantifier>,
		IOptionsProvider<StringEqualityOptions>,
		IStringPatternMatchTypeOptions
{
	private readonly TThat _returnValue = returnValue;

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	Quantifier IOptionsProvider<Quantifier>.Options => quantifier;

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	StringEqualityOptions IOptionsProvider<StringEqualityOptions>.Options => options;

	/// <summary>
	///     Interprets the expected <see langword="string" /> as a block of lines, which may be indented as a whole.
	/// </summary>
	/// <remarks>
	///     The block must start and end at line boundaries. All its lines must share the same whitespace prefix in the
	///     actual string, so the relative indentation within the block is still compared.<br />
	///     A line that consists only of whitespace matches any line that consists only of whitespace.<br />
	///     The newline style is always ignored, and a single trailing line terminator does not start a new line,
	///     so <c>"a\nb\n"</c> has the same two lines as <c>"a\nb"</c>.
	/// </remarks>
	/// <exception cref="System.InvalidOperationException">
	///     A match type or an option that changes the lines, e.g. the indentation, is already specified.
	/// </exception>
	public StringBlockCountResult<TType, TThat> AsBlock()
	{
		options.AsBlock();
		return new StringBlockCountResult<TType, TThat>(ExpectationBuilder, _returnValue, quantifier, options);
	}
}
