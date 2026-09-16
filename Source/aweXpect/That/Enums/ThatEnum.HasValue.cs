using System;
using System.Globalization;
using aweXpect.Core;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatEnum
{
	/// <summary>
	///     Verifies that the underlying value of the subject…
	/// </summary>
	/// <remarks>
	///     The comparisons apply to the underlying numeric value of the enum, so <c>HasValue().EqualTo(1)</c>
	///     passes for the member declared as <c>= 1</c>.
	/// </remarks>
	public static PropertyResult.Long<TEnum> HasValue<TEnum>(this IThat<TEnum> subject)
		where TEnum : struct, Enum
		=> new(subject, a => Convert.ToInt64(a, CultureInfo.InvariantCulture), "value");

	/// <summary>
	///     Verifies that the underlying value of the subject is equal to the <paramref name="expected" /> value.
	/// </summary>
	public static AndOrResult<TEnum, IThat<TEnum>> HasValue<TEnum>(
		this IThat<TEnum> subject,
		long? expected)
		where TEnum : struct, Enum
		=> subject.HasValue().EqualTo(expected);
}
