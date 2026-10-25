using System;

namespace aweXpect.Results;

/// <summary>
///     A result that offers the equality operators of a <see langword="string" /> property, <c>EqualTo(…)</c> and
///     <c>NotEqualTo(…)</c>, which continue with <typeparamref name="TResult" />.
/// </summary>
/// <remarks>
///     The operators are extension methods in <see cref="StringPropertyResultExtensions" />, so that every result that
///     implements this interface offers the same operators with the same names, argument checks and expectation texts.
///     Implement the interface explicitly, so that its members do not show up next to the operators.
///     <para />
///     <typeparamref name="TResult" /> usually offers the string options and the match types, e.g.
///     <see cref="StringEqualityTypeResult{TType, TThat}" />.
/// </remarks>
public interface IStringEqualityPropertyResult<out TResult>
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
