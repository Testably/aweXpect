using System;

namespace aweXpect.Results;

/// <summary>
///     A result that offers the comparison operators of a <see langword="decimal" /> property: <c>EqualTo(…)</c> and
///     <c>NotEqualTo(…)</c>, which continue with <typeparamref name="TToleranceResult" />, and the ordering operators,
///     e.g. <c>GreaterThan(…)</c> or <c>Between(…).And(…)</c>, which continue with <typeparamref name="TResult" />.
/// </summary>
/// <remarks>
///     The operators are extension methods in <see cref="DecimalPropertyResultExtensions" />, so that every result that
///     implements this interface offers the same operators with the same names, argument checks and expectation texts.
///     Implement the interface explicitly, so that its members do not show up next to the operators.
///     <para />
///     <typeparamref name="TToleranceResult" /> usually offers a tolerance, e.g.
///     <see cref="NumberToleranceResult{TType, TThat, TNumber}" />.
/// </remarks>
public interface IDecimalPropertyResult<out TResult, out TToleranceResult>
{
	/// <summary>
	///     Validates the arguments of the operators: receives the value and the name of its parameter, e.g.
	///     <c>expected</c> or <c>minimum</c>, and throws for an invalid value.
	/// </summary>
	/// <remarks>
	///     Return <see langword="null" /> when the operators accept any value.
	/// </remarks>
	Action<decimal?, string>? Validation { get; }

	/// <summary>
	///     Adds the ordering <paramref name="comparison" /> of the property and returns the result to continue with.
	/// </summary>
	TResult Compare(PropertyComparison<decimal> comparison);

	/// <summary>
	///     Adds the equality <paramref name="comparison" /> of the property and returns the result to continue with,
	///     which can specify the tolerance of the comparison.
	/// </summary>
	TToleranceResult CompareEquality(PropertyEqualityComparison<decimal> comparison);
}
