using System;
using aweXpect.Core;
using aweXpect.Core.Helpers;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect;

/// <summary>
///     The comparison operators of an <see cref="IDoublePropertyResult{TResult, TToleranceResult}" />.
/// </summary>
public static class DoublePropertyResultExtensions
{
	/// <summary>
	///     …is between the <paramref name="minimum" />…
	/// </summary>
	/// <remarks>
	///     A <c>NaN</c> <paramref name="minimum" /> or maximum throws an <see cref="ArgumentOutOfRangeException" />.
	/// </remarks>
	public static BetweenResult<TResult, double?> Between<TResult, TToleranceResult>(
		this IDoublePropertyResult<TResult, TToleranceResult> result, double? minimum)
	{
		ThrowIfNaN(minimum, "minimum", nameof(minimum));
		result.Validation?.Invoke(minimum, nameof(minimum));
		return new BetweenResult<TResult, double?>(maximum =>
		{
			ThrowIfNaN(maximum, "maximum", nameof(maximum));
			result.Validation?.Invoke(maximum, nameof(maximum));
			ThrowHelper.ThrowIfMaximumIsBelowMinimum(minimum, maximum);
			return result.Compare(new PropertyComparison<double>(minimum,
				static (a, min, max) => a >= min && a <= max,
				"between ",
				isOrderedAgainstNull: minimum is null || maximum is null,
				hasMaximum: true, maximum: maximum));
		});
	}

	/// <summary>
	///     …is equal to the <paramref name="expected" /> value.
	/// </summary>
	/// <remarks>
	///     A tolerance is specified on the result, e.g. <c>Within(0.1)</c>. <c>NaN</c> is only equal to <c>NaN</c>.
	/// </remarks>
	public static TToleranceResult EqualTo<TResult, TToleranceResult>(
		this IDoublePropertyResult<TResult, TToleranceResult> result, double? expected)
	{
		result.Validation?.Invoke(expected, nameof(expected));
		return result.CompareEquality(new PropertyEqualityComparison<double>(expected,
			new OptionsHolder<NumberTolerance<double>>(new NumberTolerance<double>(NumberDifferences.Double)), false));
	}

	/// <summary>
	///     …is greater than the <paramref name="expected" /> value.
	/// </summary>
	/// <remarks>
	///     A <c>NaN</c> <paramref name="expected" /> value throws an <see cref="ArgumentOutOfRangeException" />.
	/// </remarks>
	public static TResult GreaterThan<TResult, TToleranceResult>(
		this IDoublePropertyResult<TResult, TToleranceResult> result, double? expected)
	{
		ThrowIfNaN(expected, "expected value", nameof(expected));
		result.Validation?.Invoke(expected, nameof(expected));
		return result.Compare(new PropertyComparison<double>(expected,
			static (a, e, _) => a > e,
			"greater than ",
			isOrderedAgainstNull: expected is null));
	}

	/// <summary>
	///     …is greater than or equal to the <paramref name="expected" /> value.
	/// </summary>
	/// <remarks>
	///     A <c>NaN</c> <paramref name="expected" /> value throws an <see cref="ArgumentOutOfRangeException" />.
	/// </remarks>
	public static TResult GreaterThanOrEqualTo<TResult, TToleranceResult>(
		this IDoublePropertyResult<TResult, TToleranceResult> result, double? expected)
	{
		ThrowIfNaN(expected, "expected value", nameof(expected));
		result.Validation?.Invoke(expected, nameof(expected));
		return result.Compare(new PropertyComparison<double>(expected,
			static (a, e, _) => a >= e,
			"greater than or equal to ",
			isOrderedAgainstNull: expected is null));
	}

	/// <summary>
	///     …is less than the <paramref name="expected" /> value.
	/// </summary>
	/// <remarks>
	///     A <c>NaN</c> <paramref name="expected" /> value throws an <see cref="ArgumentOutOfRangeException" />.
	/// </remarks>
	public static TResult LessThan<TResult, TToleranceResult>(
		this IDoublePropertyResult<TResult, TToleranceResult> result, double? expected)
	{
		ThrowIfNaN(expected, "expected value", nameof(expected));
		result.Validation?.Invoke(expected, nameof(expected));
		return result.Compare(new PropertyComparison<double>(expected,
			static (a, e, _) => a < e,
			"less than ",
			isOrderedAgainstNull: expected is null));
	}

	/// <summary>
	///     …is less than or equal to the <paramref name="expected" /> value.
	/// </summary>
	/// <remarks>
	///     A <c>NaN</c> <paramref name="expected" /> value throws an <see cref="ArgumentOutOfRangeException" />.
	/// </remarks>
	public static TResult LessThanOrEqualTo<TResult, TToleranceResult>(
		this IDoublePropertyResult<TResult, TToleranceResult> result, double? expected)
	{
		ThrowIfNaN(expected, "expected value", nameof(expected));
		result.Validation?.Invoke(expected, nameof(expected));
		return result.Compare(new PropertyComparison<double>(expected,
			static (a, e, _) => a <= e,
			"less than or equal to ",
			isOrderedAgainstNull: expected is null));
	}

	/// <summary>
	///     …is not between the <paramref name="minimum" />…
	/// </summary>
	/// <remarks>
	///     A <c>NaN</c> value of the property is not between any bounds, so it satisfies this expectation. A <c>NaN</c>
	///     <paramref name="minimum" /> or maximum throws an <see cref="ArgumentOutOfRangeException" />.
	/// </remarks>
	public static BetweenResult<TResult, double?> NotBetween<TResult, TToleranceResult>(
		this IDoublePropertyResult<TResult, TToleranceResult> result, double? minimum)
	{
		ThrowIfNaN(minimum, "minimum", nameof(minimum));
		result.Validation?.Invoke(minimum, nameof(minimum));
		return new BetweenResult<TResult, double?>(maximum =>
		{
			ThrowIfNaN(maximum, "maximum", nameof(maximum));
			result.Validation?.Invoke(maximum, nameof(maximum));
			ThrowHelper.ThrowIfMaximumIsBelowMinimum(minimum, maximum);
			return result.Compare(new PropertyComparison<double>(minimum,
				static (a, min, max) => !(a >= min && a <= max),
				"between ", true,
				minimum is null || maximum is null,
				true, maximum));
		});
	}

	/// <summary>
	///     …is not equal to the <paramref name="unexpected" /> value.
	/// </summary>
	/// <remarks>
	///     A tolerance is specified on the result, e.g. <c>Within(0.1)</c>. <c>NaN</c> is only equal to <c>NaN</c>.
	/// </remarks>
	public static TToleranceResult NotEqualTo<TResult, TToleranceResult>(
		this IDoublePropertyResult<TResult, TToleranceResult> result, double? unexpected)
	{
		result.Validation?.Invoke(unexpected, nameof(unexpected));
		return result.CompareEquality(new PropertyEqualityComparison<double>(unexpected,
			new OptionsHolder<NumberTolerance<double>>(new NumberTolerance<double>(NumberDifferences.Double)), true));
	}

	/// <summary>
	///     …is not greater than the <paramref name="unexpected" /> value.
	/// </summary>
	/// <remarks>
	///     A <c>NaN</c> value of the property is not greater than any value, so it satisfies this expectation. A
	///     <c>NaN</c> <paramref name="unexpected" /> value throws an <see cref="ArgumentOutOfRangeException" />.
	/// </remarks>
	public static TResult NotGreaterThan<TResult, TToleranceResult>(
		this IDoublePropertyResult<TResult, TToleranceResult> result, double? unexpected)
	{
		ThrowIfNaN(unexpected, "unexpected value", nameof(unexpected));
		result.Validation?.Invoke(unexpected, nameof(unexpected));
		return result.Compare(new PropertyComparison<double>(unexpected,
			static (a, u, _) => !(a > u),
			"greater than ", true,
			unexpected is null));
	}

	/// <summary>
	///     …is not greater than or equal to the <paramref name="unexpected" /> value.
	/// </summary>
	/// <remarks>
	///     A <c>NaN</c> value of the property is not greater than or equal to any value, so it satisfies this
	///     expectation. A <c>NaN</c> <paramref name="unexpected" /> value throws an
	///     <see cref="ArgumentOutOfRangeException" />.
	/// </remarks>
	public static TResult NotGreaterThanOrEqualTo<TResult, TToleranceResult>(
		this IDoublePropertyResult<TResult, TToleranceResult> result, double? unexpected)
	{
		ThrowIfNaN(unexpected, "unexpected value", nameof(unexpected));
		result.Validation?.Invoke(unexpected, nameof(unexpected));
		return result.Compare(new PropertyComparison<double>(unexpected,
			static (a, u, _) => !(a >= u),
			"greater than or equal to ", true,
			unexpected is null));
	}

	/// <summary>
	///     …is not less than the <paramref name="unexpected" /> value.
	/// </summary>
	/// <remarks>
	///     A <c>NaN</c> value of the property is not less than any value, so it satisfies this expectation. A
	///     <c>NaN</c> <paramref name="unexpected" /> value throws an <see cref="ArgumentOutOfRangeException" />.
	/// </remarks>
	public static TResult NotLessThan<TResult, TToleranceResult>(
		this IDoublePropertyResult<TResult, TToleranceResult> result, double? unexpected)
	{
		ThrowIfNaN(unexpected, "unexpected value", nameof(unexpected));
		result.Validation?.Invoke(unexpected, nameof(unexpected));
		return result.Compare(new PropertyComparison<double>(unexpected,
			static (a, u, _) => !(a < u),
			"less than ", true,
			unexpected is null));
	}

	/// <summary>
	///     …is not less than or equal to the <paramref name="unexpected" /> value.
	/// </summary>
	/// <remarks>
	///     A <c>NaN</c> value of the property is not less than or equal to any value, so it satisfies this expectation.
	///     A <c>NaN</c> <paramref name="unexpected" /> value throws an <see cref="ArgumentOutOfRangeException" />.
	/// </remarks>
	public static TResult NotLessThanOrEqualTo<TResult, TToleranceResult>(
		this IDoublePropertyResult<TResult, TToleranceResult> result, double? unexpected)
	{
		ThrowIfNaN(unexpected, "unexpected value", nameof(unexpected));
		result.Validation?.Invoke(unexpected, nameof(unexpected));
		return result.Compare(new PropertyComparison<double>(unexpected,
			static (a, u, _) => !(a <= u),
			"less than or equal to ", true,
			unexpected is null));
	}

	/// <summary>
	///     Rejects a <c>NaN</c> bound, which nothing can be ordered against.
	/// </summary>
	private static void ThrowIfNaN(double? value, string description, string paramName)
	{
		if (value is not null && double.IsNaN(value.Value))
		{
			// ReSharper disable once LocalizableElement
			throw Tracing.WriteException(
				new ArgumentOutOfRangeException(paramName, $"The {description} must not be NaN."));
		}
	}
}
