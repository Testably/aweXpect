using System;

namespace aweXpect.Results;

/// <summary>
///     A result that offers the operators of a <see langword="string" /> property that look for a part of it,
///     <c>StartingWith(…)</c>, <c>EndingWith(…)</c> and their negations as well as <c>NotContaining(…)</c>, which
///     continue with <typeparamref name="TResult" />.
/// </summary>
/// <remarks>
///     The operators are extension methods in <see cref="StringPropertyResultExtensions" />, so that every result that
///     implements this interface offers the same operators with the same names, argument checks and expectation texts.
///     Implement the interface explicitly, so that its members do not show up next to the operators.
///     <para />
///     The operators already set the match type, so <typeparamref name="TResult" /> usually offers only the string
///     options, e.g. <see cref="StringEqualityResult{TType, TThat}" />.
/// </remarks>
public interface IStringAffixPropertyResult<out TResult>
{
	/// <summary>
	///     Validates the arguments of the operators: receives the value and the name of its parameter, e.g.
	///     <c>expected</c> or <c>unexpected</c>, and throws for an invalid value.
	/// </summary>
	/// <remarks>
	///     Return <see langword="null" /> when the operators accept any value.
	/// </remarks>
	Action<string?, string>? Validation { get; }

	/// <summary>
	///     Adds the <paramref name="comparison" /> of the property and returns the result to continue with.
	/// </summary>
	TResult Compare(StringPropertyComparison comparison);
}
