using System;
using aweXpect.Core;
using aweXpect.Helpers;
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
	public static EnumValueResult<TEnum> HasValue<TEnum>(this IThat<TEnum> subject)
		where TEnum : struct, Enum
		=> new(subject, a => a.ToUnderlyingValue(), "value");

	/// <summary>
	///     Verifies that the underlying value of the subject is equal to the <paramref name="expected" /> value.
	/// </summary>
	public static AndOrResult<TEnum, IThat<TEnum>> HasValue<TEnum>(
		this IThat<TEnum> subject,
		long? expected)
		where TEnum : struct, Enum
		=> subject.HasValue().EqualTo(expected);

	/// <summary>
	///     Verifies that the underlying value of the subject is equal to the <paramref name="expected" /> value.
	/// </summary>
	public static AndOrResult<TEnum, IThat<TEnum>> HasValue<TEnum>(
		this IThat<TEnum> subject,
		ulong? expected)
		where TEnum : struct, Enum
		=> subject.HasValue().EqualTo(expected);
}
