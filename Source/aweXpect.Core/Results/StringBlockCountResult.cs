using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect.Results;

/// <summary>
///     The result for verifying that a string contains a block of lines a specified number of times.
/// </summary>
/// <remarks>
///     <seealso cref="CountResult{TType,TThat,TSelf}" />
/// </remarks>
public class StringBlockCountResult<TType, TThat>(
	ExpectationBuilder expectationBuilder,
	TThat returnValue,
	Quantifier quantifier,
	StringEqualityOptions options)
	: CountResult<TType, TThat, StringBlockCountResult<TType, TThat>>(expectationBuilder, returnValue, quantifier),
		IOptionsProvider<StringEqualityOptions>
{
	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	StringEqualityOptions IOptionsProvider<StringEqualityOptions>.Options => options;

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
		options.UsingComparer(comparer);
		return this;
	}
}
