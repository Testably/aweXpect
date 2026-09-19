using System;
using System.Globalization;

namespace aweXpect.Helpers;

internal static class EnumHelpers
{
	/// <summary>
	///     Returns the underlying numeric value of the <paramref name="value" />.
	/// </summary>
	/// <remarks>
	///     A <see langword="ulong" />-backed member above <see cref="long.MaxValue" /> overflows a
	///     <see langword="long" />, so the value is read through its own backing type; a <see langword="decimal" />
	///     then holds every backing type exactly.
	/// </remarks>
	public static decimal ToUnderlyingValue<TEnum>(this TEnum value)
		where TEnum : struct, Enum
		=> value.GetTypeCode() == TypeCode.UInt64
			? Convert.ToUInt64(value, CultureInfo.InvariantCulture)
			: Convert.ToInt64(value, CultureInfo.InvariantCulture);
}
