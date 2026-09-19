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
	///     <see langword="long" />, so it is read through its own backing type; a <see langword="decimal" /> holds
	///     every backing type exactly.
	/// </remarks>
	public static decimal ToUnderlyingValue<TEnum>(this TEnum value)
		where TEnum : struct, Enum
		=> value.GetTypeCode() == TypeCode.UInt64
			? Convert.ToUInt64(value, CultureInfo.InvariantCulture)
			: Convert.ToInt64(value, CultureInfo.InvariantCulture);

	/// <summary>
	///     Renders the underlying value of an enum for a failure message.
	/// </summary>
	/// <remarks>
	///     The underlying value is an integer, so it is spelled as one instead of with the fractional digit that the
	///     general <see langword="decimal" /> formatting appends.
	/// </remarks>
	public static string FormatUnderlyingValue(decimal? value)
		=> value?.ToString(CultureInfo.InvariantCulture) ?? ValueFormatter.NullString;
}
