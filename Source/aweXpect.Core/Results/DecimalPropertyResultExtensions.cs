using aweXpect.Core.Helpers;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect;

/// <summary>
///     The comparison operators of an <see cref="IDecimalPropertyResult{TResult, TToleranceResult}" />.
/// </summary>
public static class DecimalPropertyResultExtensions
{
	/// <summary>
	///     …is between the <paramref name="minimum" />…
	/// </summary>
	public static BetweenResult<TResult, decimal?> Between<TResult, TToleranceResult>(
		this IDecimalPropertyResult<TResult, TToleranceResult> result, decimal? minimum)
	{
		result.Validation?.Invoke(minimum, nameof(minimum));
		return new BetweenResult<TResult, decimal?>(maximum =>
		{
			result.Validation?.Invoke(maximum, nameof(maximum));
			ThrowHelper.ThrowIfMaximumIsBelowMinimum(minimum, maximum);
			return result.Compare(new PropertyComparison<decimal>(minimum,
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
	///     A tolerance is specified on the result, e.g. <c>Within(0.1m)</c>.
	/// </remarks>
	public static TToleranceResult EqualTo<TResult, TToleranceResult>(
		this IDecimalPropertyResult<TResult, TToleranceResult> result, decimal? expected)
	{
		result.Validation?.Invoke(expected, nameof(expected));
		return result.CompareEquality(new PropertyEqualityComparison<decimal>(expected,
			new NumberTolerance<decimal>(NumberDifferences.Decimal), false));
	}

	/// <summary>
	///     …is greater than the <paramref name="expected" /> value.
	/// </summary>
	public static TResult GreaterThan<TResult, TToleranceResult>(
		this IDecimalPropertyResult<TResult, TToleranceResult> result, decimal? expected)
	{
		result.Validation?.Invoke(expected, nameof(expected));
		return result.Compare(new PropertyComparison<decimal>(expected,
			static (a, e, _) => a > e,
			"greater than ",
			isOrderedAgainstNull: expected is null));
	}

	/// <summary>
	///     …is greater than or equal to the <paramref name="expected" /> value.
	/// </summary>
	public static TResult GreaterThanOrEqualTo<TResult, TToleranceResult>(
		this IDecimalPropertyResult<TResult, TToleranceResult> result, decimal? expected)
	{
		result.Validation?.Invoke(expected, nameof(expected));
		return result.Compare(new PropertyComparison<decimal>(expected,
			static (a, e, _) => a >= e,
			"greater than or equal to ",
			isOrderedAgainstNull: expected is null));
	}

	/// <summary>
	///     …is less than the <paramref name="expected" /> value.
	/// </summary>
	public static TResult LessThan<TResult, TToleranceResult>(
		this IDecimalPropertyResult<TResult, TToleranceResult> result, decimal? expected)
	{
		result.Validation?.Invoke(expected, nameof(expected));
		return result.Compare(new PropertyComparison<decimal>(expected,
			static (a, e, _) => a < e,
			"less than ",
			isOrderedAgainstNull: expected is null));
	}

	/// <summary>
	///     …is less than or equal to the <paramref name="expected" /> value.
	/// </summary>
	public static TResult LessThanOrEqualTo<TResult, TToleranceResult>(
		this IDecimalPropertyResult<TResult, TToleranceResult> result, decimal? expected)
	{
		result.Validation?.Invoke(expected, nameof(expected));
		return result.Compare(new PropertyComparison<decimal>(expected,
			static (a, e, _) => a <= e,
			"less than or equal to ",
			isOrderedAgainstNull: expected is null));
	}

	/// <summary>
	///     …is not between the <paramref name="minimum" />…
	/// </summary>
	public static BetweenResult<TResult, decimal?> NotBetween<TResult, TToleranceResult>(
		this IDecimalPropertyResult<TResult, TToleranceResult> result, decimal? minimum)
	{
		result.Validation?.Invoke(minimum, nameof(minimum));
		return new BetweenResult<TResult, decimal?>(maximum =>
		{
			result.Validation?.Invoke(maximum, nameof(maximum));
			ThrowHelper.ThrowIfMaximumIsBelowMinimum(minimum, maximum);
			return result.Compare(new PropertyComparison<decimal>(minimum,
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
	///     A tolerance is specified on the result, e.g. <c>Within(0.1m)</c>.
	/// </remarks>
	public static TToleranceResult NotEqualTo<TResult, TToleranceResult>(
		this IDecimalPropertyResult<TResult, TToleranceResult> result, decimal? unexpected)
	{
		result.Validation?.Invoke(unexpected, nameof(unexpected));
		return result.CompareEquality(new PropertyEqualityComparison<decimal>(unexpected,
			new NumberTolerance<decimal>(NumberDifferences.Decimal), true));
	}

	/// <summary>
	///     …is not greater than the <paramref name="unexpected" /> value.
	/// </summary>
	public static TResult NotGreaterThan<TResult, TToleranceResult>(
		this IDecimalPropertyResult<TResult, TToleranceResult> result, decimal? unexpected)
	{
		result.Validation?.Invoke(unexpected, nameof(unexpected));
		return result.Compare(new PropertyComparison<decimal>(unexpected,
			static (a, u, _) => !(a > u),
			"greater than ", true,
			unexpected is null));
	}

	/// <summary>
	///     …is not greater than or equal to the <paramref name="unexpected" /> value.
	/// </summary>
	public static TResult NotGreaterThanOrEqualTo<TResult, TToleranceResult>(
		this IDecimalPropertyResult<TResult, TToleranceResult> result, decimal? unexpected)
	{
		result.Validation?.Invoke(unexpected, nameof(unexpected));
		return result.Compare(new PropertyComparison<decimal>(unexpected,
			static (a, u, _) => !(a >= u),
			"greater than or equal to ", true,
			unexpected is null));
	}

	/// <summary>
	///     …is not less than the <paramref name="unexpected" /> value.
	/// </summary>
	public static TResult NotLessThan<TResult, TToleranceResult>(
		this IDecimalPropertyResult<TResult, TToleranceResult> result, decimal? unexpected)
	{
		result.Validation?.Invoke(unexpected, nameof(unexpected));
		return result.Compare(new PropertyComparison<decimal>(unexpected,
			static (a, u, _) => !(a < u),
			"less than ", true,
			unexpected is null));
	}

	/// <summary>
	///     …is not less than or equal to the <paramref name="unexpected" /> value.
	/// </summary>
	public static TResult NotLessThanOrEqualTo<TResult, TToleranceResult>(
		this IDecimalPropertyResult<TResult, TToleranceResult> result, decimal? unexpected)
	{
		result.Validation?.Invoke(unexpected, nameof(unexpected));
		return result.Compare(new PropertyComparison<decimal>(unexpected,
			static (a, u, _) => !(a <= u),
			"less than or equal to ", true,
			unexpected is null));
	}
}
