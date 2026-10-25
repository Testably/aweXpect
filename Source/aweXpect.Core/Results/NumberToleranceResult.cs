using System;
#if NET8_0_OR_GREATER
using System.Numerics;
#endif
using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect.Results;

/// <summary>
///     The result of an expectation with an underlying value of type <typeparamref name="TType" />.
///     <para />
///     In addition to the combinations from <see cref="AndOrResult{TNumber,TThat}" />, allows specifying a
///     tolerance via <see cref="ToleranceExtensions" />.
/// </summary>
public class NumberToleranceResult<TType, TThat>(
	ExpectationBuilder expectationBuilder,
	TThat returnValue,
	IOptionsHolderSource<NumberTolerance<TType>> options)
	: AndOrResult<TType, TThat, NumberToleranceResult<TType, TThat>>(expectationBuilder, returnValue),
		INumberToleranceResult<NumberToleranceResult<TType, TThat>, TType>
#if NET8_0_OR_GREATER
	where TType : struct, INumber<TType>
#else
	where TType : struct, IComparable<TType>
#endif
{
	private readonly TThat _returnValue = returnValue;

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	OptionsHolder<NumberTolerance<TType>> IOptionsProvider<NumberTolerance<TType>>.Options => options.Holder;

	/// <inheritdoc cref="ToleranceExtensions.Within{TSelf, TNumber}(INumberToleranceResult{TSelf, TNumber}, TNumber)" />
	public WithinResult Within(TType tolerance)
	{
		options.Holder.Within(tolerance);
		return new WithinResult(ExpectationBuilder, _returnValue, options);
	}

	/// <summary>
	///     The result directly after <see cref="NumberToleranceResult{TType,TThat}.Within" />, whose tolerance can still be made relative to the
	///     expected value, e.g. via <c>Percent()</c>.
	/// </summary>
	public sealed class WithinResult(
		ExpectationBuilder expectationBuilder,
		TThat returnValue,
		IOptionsHolderSource<NumberTolerance<TType>> options)
		: NumberToleranceResult<TType, TThat>(expectationBuilder, returnValue, options),
			IWithinToleranceResult<NumberToleranceResult<TType, TThat>, TType>
	{
		/// <inheritdoc cref="IWithinToleranceResult{TResult,TTolerance}.SetRelativeTolerance(Func{TTolerance, TTolerance, TTolerance}, string)" />
		NumberToleranceResult<TType, TThat> IWithinToleranceResult<NumberToleranceResult<TType, TThat>, TType>.SetRelativeTolerance(
			Func<TType, TType, TType> absoluteTolerance, string unit)
		{
			OptionsHolder<NumberTolerance<TType>> holder = ((IOptionsProvider<NumberTolerance<TType>>)this).Options;
			holder.Value = holder.Value.SetRelativeTolerance(absoluteTolerance, unit);
			return this;
		}
	}
}

/// <summary>
///     The result of an expectation with an underlying value of type <typeparamref name="TType" /> that compares a
///     <typeparamref name="TNumber" /> of it, e.g. a number in a document.
///     <para />
///     In addition to the combinations from <see cref="AndOrResult{TType,TThat}" />, allows specifying a
///     tolerance via <see cref="ToleranceExtensions" />.
/// </summary>
public class NumberToleranceResult<TType, TThat, TNumber>(
	ExpectationBuilder expectationBuilder,
	TThat returnValue,
	IOptionsHolderSource<NumberTolerance<TNumber>> options)
	: AndOrResult<TType, TThat, NumberToleranceResult<TType, TThat, TNumber>>(expectationBuilder, returnValue),
		INumberToleranceResult<NumberToleranceResult<TType, TThat, TNumber>, TNumber>
#if NET8_0_OR_GREATER
	where TNumber : struct, INumber<TNumber>
#else
	where TNumber : struct, IComparable<TNumber>
#endif
{
	private readonly TThat _returnValue = returnValue;

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	OptionsHolder<NumberTolerance<TNumber>> IOptionsProvider<NumberTolerance<TNumber>>.Options => options.Holder;

	/// <inheritdoc cref="ToleranceExtensions.Within{TSelf, TNumber}(INumberToleranceResult{TSelf, TNumber}, TNumber)" />
	public WithinResult Within(TNumber tolerance)
	{
		options.Holder.Within(tolerance);
		return new WithinResult(ExpectationBuilder, _returnValue, options);
	}

	/// <summary>
	///     The result directly after <see cref="NumberToleranceResult{TType,TThat,TNumber}.Within" />, whose tolerance
	///     can still be made relative to the expected value, e.g. via <c>Percent()</c>.
	/// </summary>
	public sealed class WithinResult(
		ExpectationBuilder expectationBuilder,
		TThat returnValue,
		IOptionsHolderSource<NumberTolerance<TNumber>> options)
		: NumberToleranceResult<TType, TThat, TNumber>(expectationBuilder, returnValue, options),
			IWithinToleranceResult<NumberToleranceResult<TType, TThat, TNumber>, TNumber>
	{
		/// <inheritdoc cref="IWithinToleranceResult{TResult,TTolerance}.SetRelativeTolerance(Func{TTolerance, TTolerance, TTolerance}, string)" />
		NumberToleranceResult<TType, TThat, TNumber>
			IWithinToleranceResult<NumberToleranceResult<TType, TThat, TNumber>, TNumber>.SetRelativeTolerance(
				Func<TNumber, TNumber, TNumber> absoluteTolerance, string unit)
		{
			OptionsHolder<NumberTolerance<TNumber>> holder = ((IOptionsProvider<NumberTolerance<TNumber>>)this).Options;
			holder.Value = holder.Value.SetRelativeTolerance(absoluteTolerance, unit);
			return this;
		}
	}
}
