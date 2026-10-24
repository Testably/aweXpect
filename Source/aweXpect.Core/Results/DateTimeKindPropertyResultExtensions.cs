using System;
using aweXpect.Results;

namespace aweXpect;

/// <summary>
///     The equality operators of an <see cref="IDateTimeKindPropertyResult{TResult}" />.
/// </summary>
public static class DateTimeKindPropertyResultExtensions
{
	/// <summary>
	///     …is equal to the <paramref name="expected" /> value.
	/// </summary>
	public static TResult EqualTo<TResult>(this IDateTimeKindPropertyResult<TResult> result,
		DateTimeKind? expected)
		=> result.Compare(new PropertyComparison<DateTimeKind>(expected,
			static (a, e, _) => a == e,
			"equal to "));

	/// <summary>
	///     …is not equal to the <paramref name="unexpected" /> value.
	/// </summary>
	public static TResult NotEqualTo<TResult>(this IDateTimeKindPropertyResult<TResult> result,
		DateTimeKind? unexpected)
		=> result.Compare(new PropertyComparison<DateTimeKind>(unexpected,
			static (a, u, _) => a != u,
			"equal to ", true));
}
