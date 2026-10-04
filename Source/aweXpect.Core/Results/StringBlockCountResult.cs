using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect.Results;

/// <summary>
///     The result for verifying that a string contains a block of lines a specified number of times.
/// </summary>
/// <remarks>
///     A block compares the lines on its own, so of the string options only the casing and a comparer can be
///     specified.
/// </remarks>
public class StringBlockCountResult<TType, TThat>(
	ExpectationBuilder expectationBuilder,
	TThat returnValue,
	Quantifier quantifier,
	StringEqualityOptions options)
	: AndOrResult<TType, TThat, StringBlockCountResult<TType, TThat>>(expectationBuilder, returnValue),
		IOptionsProvider<Quantifier>
{
	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	Quantifier IOptionsProvider<Quantifier>.Options => quantifier;

	/// <summary>
	///     Ignores casing when comparing the <see langword="string" />s.
	/// </summary>
	public StringBlockCountResult<TType, TThat> IgnoringCase(bool ignoreCase = true)
	{
		options.IgnoringCase(ignoreCase);
		return this;
	}

	/// <summary>
	///     Uses the provided <paramref name="comparer" /> for comparing <see langword="string" />s.
	/// </summary>
	public StringBlockCountResult<TType, TThat> Using(
		IEqualityComparer<string> comparer)
	{
		options.Using(comparer);
		return this;
	}
}
