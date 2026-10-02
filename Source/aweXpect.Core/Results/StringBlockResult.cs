using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect.Results;

/// <summary>
///     The result for verifying that a string matches a block of lines.
/// </summary>
/// <remarks>
///     <seealso cref="StringBlockCountResult{TType,TThat}" />
/// </remarks>
public class StringBlockResult<TType, TThat>(
	ExpectationBuilder expectationBuilder,
	TThat returnValue,
	StringEqualityOptions options)
	: AndOrResult<TType, TThat, StringBlockResult<TType, TThat>>(expectationBuilder, returnValue),
		IOptionsProvider<StringEqualityOptions>
{
	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	StringEqualityOptions IOptionsProvider<StringEqualityOptions>.Options => options;

	/// <summary>
	///     Ignores casing when comparing the <see langword="string" />s.
	/// </summary>
	public StringBlockResult<TType, TThat> IgnoringCase(bool ignoreCase = true)
	{
		options.IgnoringCase(ignoreCase);
		return this;
	}

	/// <summary>
	///     Uses the provided <paramref name="comparer" /> for comparing <see langword="string" />s.
	/// </summary>
	public StringBlockResult<TType, TThat> Using(
		IEqualityComparer<string> comparer)
	{
		options.Using(comparer);
		return this;
	}
}
