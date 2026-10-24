using aweXpect.Core.Helpers;
using aweXpect.Results;

namespace aweXpect;

/// <summary>
///     The comparison operators of an <see cref="IIntPropertyResult{TResult}" />.
/// </summary>
public static class IntPropertyResultExtensions
{
	/// <summary>
	///     …is between the <paramref name="minimum" />…
	/// </summary>
	public static BetweenResult<TResult, int?> Between<TResult>(this IIntPropertyResult<TResult> result,
		int? minimum)
	{
		result.Validation?.Invoke(minimum, nameof(minimum));
		return new BetweenResult<TResult, int?>(maximum =>
		{
			result.Validation?.Invoke(maximum, nameof(maximum));
			ThrowHelper.ThrowIfMaximumIsBelowMinimum(minimum, maximum);
			return result.Compare(new PropertyComparison<int>(minimum,
				static (a, min, max) => a >= min && a <= max,
				"between ",
				isOrderedAgainstNull: minimum is null || maximum is null,
				hasMaximum: true, maximum: maximum));
		});
	}

	/// <summary>
	///     …is equal to the <paramref name="expected" /> value.
	/// </summary>
	public static TResult EqualTo<TResult>(this IIntPropertyResult<TResult> result,
		int? expected)
	{
		result.Validation?.Invoke(expected, nameof(expected));
		return result.Compare(new PropertyComparison<int>(expected,
			static (a, e, _) => a == e,
			"equal to "));
	}

	/// <summary>
	///     …is greater than the <paramref name="expected" /> value.
	/// </summary>
	public static TResult GreaterThan<TResult>(this IIntPropertyResult<TResult> result,
		int? expected)
	{
		result.Validation?.Invoke(expected, nameof(expected));
		return result.Compare(new PropertyComparison<int>(expected,
			static (a, e, _) => a > e,
			"greater than ",
			isOrderedAgainstNull: expected is null));
	}

	/// <summary>
	///     …is greater than or equal to the <paramref name="expected" /> value.
	/// </summary>
	public static TResult GreaterThanOrEqualTo<TResult>(this IIntPropertyResult<TResult> result,
		int? expected)
	{
		result.Validation?.Invoke(expected, nameof(expected));
		return result.Compare(new PropertyComparison<int>(expected,
			static (a, e, _) => a >= e,
			"greater than or equal to ",
			isOrderedAgainstNull: expected is null));
	}

	/// <summary>
	///     …is less than the <paramref name="expected" /> value.
	/// </summary>
	public static TResult LessThan<TResult>(this IIntPropertyResult<TResult> result,
		int? expected)
	{
		result.Validation?.Invoke(expected, nameof(expected));
		return result.Compare(new PropertyComparison<int>(expected,
			static (a, e, _) => a < e,
			"less than ",
			isOrderedAgainstNull: expected is null));
	}

	/// <summary>
	///     …is less than or equal to the <paramref name="expected" /> value.
	/// </summary>
	public static TResult LessThanOrEqualTo<TResult>(this IIntPropertyResult<TResult> result,
		int? expected)
	{
		result.Validation?.Invoke(expected, nameof(expected));
		return result.Compare(new PropertyComparison<int>(expected,
			static (a, e, _) => a <= e,
			"less than or equal to ",
			isOrderedAgainstNull: expected is null));
	}

	/// <summary>
	///     …is not between the <paramref name="minimum" />…
	/// </summary>
	public static BetweenResult<TResult, int?> NotBetween<TResult>(this IIntPropertyResult<TResult> result,
		int? minimum)
	{
		result.Validation?.Invoke(minimum, nameof(minimum));
		return new BetweenResult<TResult, int?>(maximum =>
		{
			result.Validation?.Invoke(maximum, nameof(maximum));
			ThrowHelper.ThrowIfMaximumIsBelowMinimum(minimum, maximum);
			return result.Compare(new PropertyComparison<int>(minimum,
				static (a, min, max) => !(a >= min && a <= max),
				"between ", true,
				minimum is null || maximum is null,
				true, maximum));
		});
	}

	/// <summary>
	///     …is not equal to the <paramref name="unexpected" /> value.
	/// </summary>
	public static TResult NotEqualTo<TResult>(this IIntPropertyResult<TResult> result,
		int? unexpected)
	{
		result.Validation?.Invoke(unexpected, nameof(unexpected));
		return result.Compare(new PropertyComparison<int>(unexpected,
			static (a, u, _) => a != u,
			"equal to ", true));
	}

	/// <summary>
	///     …is not greater than the <paramref name="unexpected" /> value.
	/// </summary>
	public static TResult NotGreaterThan<TResult>(this IIntPropertyResult<TResult> result,
		int? unexpected)
	{
		result.Validation?.Invoke(unexpected, nameof(unexpected));
		return result.Compare(new PropertyComparison<int>(unexpected,
			static (a, u, _) => !(a > u),
			"greater than ", true,
			unexpected is null));
	}

	/// <summary>
	///     …is not greater than or equal to the <paramref name="unexpected" /> value.
	/// </summary>
	public static TResult NotGreaterThanOrEqualTo<TResult>(this IIntPropertyResult<TResult> result,
		int? unexpected)
	{
		result.Validation?.Invoke(unexpected, nameof(unexpected));
		return result.Compare(new PropertyComparison<int>(unexpected,
			static (a, u, _) => !(a >= u),
			"greater than or equal to ", true,
			unexpected is null));
	}

	/// <summary>
	///     …is not less than the <paramref name="unexpected" /> value.
	/// </summary>
	public static TResult NotLessThan<TResult>(this IIntPropertyResult<TResult> result,
		int? unexpected)
	{
		result.Validation?.Invoke(unexpected, nameof(unexpected));
		return result.Compare(new PropertyComparison<int>(unexpected,
			static (a, u, _) => !(a < u),
			"less than ", true,
			unexpected is null));
	}

	/// <summary>
	///     …is not less than or equal to the <paramref name="unexpected" /> value.
	/// </summary>
	public static TResult NotLessThanOrEqualTo<TResult>(this IIntPropertyResult<TResult> result,
		int? unexpected)
	{
		result.Validation?.Invoke(unexpected, nameof(unexpected));
		return result.Compare(new PropertyComparison<int>(unexpected,
			static (a, u, _) => !(a <= u),
			"less than or equal to ", true,
			unexpected is null));
	}
}
