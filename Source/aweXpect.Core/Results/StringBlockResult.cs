using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect.Results;

/// <summary>
///     The result for verifying that a string matches a block of lines.
/// </summary>
/// <remarks>
///     A block compares the lines on its own, so of the string options only the casing and a comparer can be
///     specified.
/// </remarks>
public class StringBlockResult<TType, TThat>(
	ExpectationBuilder expectationBuilder,
	TThat returnValue,
	StringEqualityOptions options)
	: AndOrResult<TType, TThat, StringBlockResult<TType, TThat>>(expectationBuilder, returnValue)
{
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
