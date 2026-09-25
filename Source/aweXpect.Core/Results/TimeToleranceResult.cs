using System;
using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect.Results;

/// <summary>
///     The result of an expectation with an underlying value of type <typeparamref name="TType" />.
///     <para />
///     In addition to the combinations from <see cref="AndOrResult{TType,TThat}" />, allows specifying a
///     tolerance.
/// </summary>
public class TimeToleranceResult<TType, TThat>(
	ExpectationBuilder expectationBuilder,
	TThat returnValue,
	TimeTolerance options)
	: TimeToleranceResult<TType, TThat,
		TimeToleranceResult<TType, TThat>>(
		expectationBuilder,
		returnValue,
		options);

/// <summary>
///     The result of an expectation with an underlying value of type <typeparamref name="TType" />.
///     <para />
///     In addition to the combinations from <see cref="AndOrResult{TType,TThat}" />, allows specifying a
///     tolerance.
/// </summary>
public class TimeToleranceResult<TType, TThat, TSelf>(
	ExpectationBuilder expectationBuilder,
	TThat returnValue,
	TimeTolerance options)
	: AndOrResult<TType, TThat, TSelf>(expectationBuilder, returnValue),
		IOptionsProvider<TimeTolerance>
	where TSelf : TimeToleranceResult<TType, TThat, TSelf>
{
	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	TimeTolerance IOptionsProvider<TimeTolerance>.Options => options;

	/// <summary>
	///     Specifies a <paramref name="tolerance" /> to apply on the time comparison.
	/// </summary>
	/// <remarks>
	///     The tolerance relaxes the bounds of equality, ordering and range expectations, and a subject exactly the
	///     tolerance away matches wherever the bound itself matches, for example on <c>IsEqualTo</c> or
	///     <c>IsOnOrAfter</c>, but not on <c>IsAfter</c>. A negated expectation, such as <c>IsNotAfter</c>,
	///     <c>IsNotBetween</c> or <c>IsNotEqualTo</c>, is the exact inverse of its positive form, so the tolerance
	///     narrows it and it fails within the tolerance of the bound. On a <c>DateOnly</c>,
	///     a <paramref name="tolerance" /> that is not a whole number of days throws an
	///     <see cref="ArgumentOutOfRangeException" />.
	/// </remarks>
	public TSelf Within(TimeSpan tolerance)
	{
		options.SetTolerance(tolerance);
		return (TSelf)this;
	}
}
