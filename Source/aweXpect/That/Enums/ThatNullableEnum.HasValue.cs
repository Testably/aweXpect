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
	/// <remarks>
	///     Unlike <see cref="Nullable{T}.HasValue" /> this does not ask whether the subject is set: the comparisons
	///     apply to the underlying numeric value of the enum, and a <see langword="null" /> subject fails every one
	///     of them. Use <c>IsNotNull()</c> to verify only that the subject has a value.
	/// </remarks>
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
