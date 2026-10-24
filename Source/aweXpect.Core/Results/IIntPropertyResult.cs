using System;

namespace aweXpect.Results;

/// <summary>
///     A result that offers the comparison operators of an <see langword="int" /> property, e.g. <c>EqualTo(…)</c> or
///     <c>Between(…).And(…)</c>, which continue with <typeparamref name="TResult" />.
/// </summary>
/// <remarks>
///     The operators are extension methods in <see cref="IntPropertyResultExtensions" />, so that every result that
///     implements this interface offers the same operators with the same names, argument checks and expectation texts.
///     Implement the interface explicitly, so that its members do not show up next to the operators.
/// </remarks>
public interface IIntPropertyResult<out TResult>
{
	/// <summary>
	///     Validates the arguments of the operators: receives the value and the name of its parameter, e.g.
	///     <c>expected</c> or <c>minimum</c>, and throws for an invalid value.
	/// </summary>
	/// <remarks>
	///     Return <see langword="null" /> when the operators accept any value.
	/// </remarks>
	Action<int?, string>? Validation { get; }

	/// <summary>
	///     Adds the <paramref name="comparison" /> of the property and returns the result to continue with.
	/// </summary>
	TResult Compare(PropertyComparison<int> comparison);
}
