using System;
using System.Globalization;
using aweXpect.Core;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatNullableEnum
{
	/// <summary>
	///     Verifies that the underlying value of the subject…
	/// </summary>
	[GuaranteesNotNull]
	public static PropertyResult.Long<TEnum?> HasValue<TEnum>(this IThat<TEnum?> subject)
		where TEnum : struct, Enum
		=> new(subject, a => a is null ? null : Convert.ToInt64(a.Value, CultureInfo.InvariantCulture), "value");

	/// <summary>
	///     Verifies that the underlying value of the subject is equal to the <paramref name="expected" /> value.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<TEnum?, IThat<TEnum?>> HasValue<TEnum>(
		this IThat<TEnum?> subject,
		long? expected)
		where TEnum : struct, Enum
		=> subject.HasValue().EqualTo(expected);
}
