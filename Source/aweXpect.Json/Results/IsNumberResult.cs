using System;
using System.Text.Json;
using aweXpect.Core;
using aweXpect.Helpers;
using aweXpect.Results;

namespace aweXpect.Json.Results;

/// <summary>
///     The result for verifying that a <see cref="JsonElement" /> is a <see cref="JsonValueKind.Number" />, which
///     offers the operators of a <see langword="double" /> property, e.g. <c>GreaterThan(3)</c> or
///     <c>EqualTo(4.5).Within(0.1)</c>, to compare its value in the same expectation.
/// </summary>
/// <remarks>
///     Only one operator can be called on this result. It compares the number as <see langword="double" />, so a number
///     that does not fit into a <see langword="double" /> fails it. Its expected values and bounds cannot be
///     <see langword="null" />, as a JSON number never is.
///     <para />
///     <seealso cref="AndOrResult{TType,TThat}" />
/// </remarks>
public class IsNumberResult<TType, TThat> : AndOrResult<TType, TThat>,
	IDoublePropertyResult<AndOrResult<TType, TThat>, NumberToleranceResult<TType, TThat, double>>
{
	private static readonly Action<double?, string> ThrowIfNull
		= static (value, paramName) => ThrowHelper.ThrowIfNull(value, paramName);

	private readonly TThat _returnValue;
	private readonly JsonComparisonSlot<JsonNumberComparison> _slot;

	internal IsNumberResult(
		ExpectationBuilder expectationBuilder,
		TThat returnValue,
		JsonComparisonSlot<JsonNumberComparison> slot)
		: base(expectationBuilder, returnValue)
	{
		_returnValue = returnValue;
		_slot = slot;
	}

	/// <inheritdoc cref="IDoublePropertyResult{TResult, TToleranceResult}.Validation" />
	Action<double?, string>?
		IDoublePropertyResult<AndOrResult<TType, TThat>, NumberToleranceResult<TType, TThat, double>>.Validation
		=> ThrowIfNull;

	/// <inheritdoc cref="IDoublePropertyResult{TResult, TToleranceResult}.Compare(PropertyComparison{double})" />
	AndOrResult<TType, TThat>
		IDoublePropertyResult<AndOrResult<TType, TThat>, NumberToleranceResult<TType, TThat, double>>.Compare(
			PropertyComparison<double> comparison)
	{
		_slot.Set(new JsonNumberComparison(comparison));
		return new AndOrResult<TType, TThat>(ExpectationBuilder, _returnValue);
	}

	/// <inheritdoc
	///     cref="IDoublePropertyResult{TResult, TToleranceResult}.CompareEquality(PropertyEqualityComparison{double})" />
	NumberToleranceResult<TType, TThat, double>
		IDoublePropertyResult<AndOrResult<TType, TThat>, NumberToleranceResult<TType, TThat, double>>.CompareEquality(
			PropertyEqualityComparison<double> comparison)
	{
		_slot.Set(new JsonNumberComparison(comparison));
		return new NumberToleranceResult<TType, TThat, double>(ExpectationBuilder, _returnValue, comparison.Options);
	}
}
