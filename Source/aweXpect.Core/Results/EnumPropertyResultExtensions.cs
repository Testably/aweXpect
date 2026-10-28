using System;
using aweXpect.Results;

namespace aweXpect;

/// <summary>
///     The equality operators of an <see cref="IEnumPropertyResult{TResult, TEnum}" />.
/// </summary>
public static class EnumPropertyResultExtensions
{
	/// <summary>
	///     …is equal to the <paramref name="expected" /> value.
	/// </summary>
	public static TResult EqualTo<TResult, TEnum>(this IEnumPropertyResult<TResult, TEnum> result,
		TEnum? expected)
		where TEnum : struct, Enum
		=> result.Compare(new PropertyComparison<TEnum>(expected,
			static (a, e, _) => Nullable.Equals(a, e),
			"equal to "));

	/// <summary>
	///     …is not equal to the <paramref name="unexpected" /> value.
	/// </summary>
	public static TResult NotEqualTo<TResult, TEnum>(this IEnumPropertyResult<TResult, TEnum> result,
		TEnum? unexpected)
		where TEnum : struct, Enum
		=> result.Compare(new PropertyComparison<TEnum>(unexpected,
			static (a, u, _) => !Nullable.Equals(a, u),
			"equal to ", true));
}
