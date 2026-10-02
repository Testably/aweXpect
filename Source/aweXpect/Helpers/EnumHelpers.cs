using System;
using System.Globalization;

namespace aweXpect.Helpers;

internal static class EnumHelpers
{
	/// <summary>
	///     Returns whether the <paramref name="value" /> is a named member of the <typeparamref name="TEnum" /> or, for an
	///     enum with the <see cref="FlagsAttribute" />, a combination of the bits of its named members.
	/// </summary>
	public static bool IsDefinedValue<TEnum>(this TEnum value)
		where TEnum : struct, Enum
	{
#if NET8_0_OR_GREATER
		if (Enum.IsDefined(value))
#else
		if (Enum.IsDefined(typeof(TEnum), value))
#endif
		{
			return true;
		}

		ulong bits = value.ToBits();
		return bits != 0 && FlagsMask<TEnum>.Value is { } mask && (bits & ~mask) == 0;
	}

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

	/// <remarks>
	///     A signed value is sign-extended, so a negative member sets the same upper bits as a negative value.
	/// </remarks>
	private static ulong ToBits<TEnum>(this TEnum value)
		where TEnum : struct, Enum
		=> value.GetTypeCode() == TypeCode.UInt64
			? Convert.ToUInt64(value, CultureInfo.InvariantCulture)
			: unchecked((ulong)Convert.ToInt64(value, CultureInfo.InvariantCulture));

	private static class FlagsMask<TEnum>
		where TEnum : struct, Enum
	{
		/// <summary>
		///     The bits of all named members, or <see langword="null" /> if the <typeparamref name="TEnum" /> has no
		///     <see cref="FlagsAttribute" />.
		/// </summary>
		public static readonly ulong? Value = Compute();

		private static ulong? Compute()
		{
			if (!typeof(TEnum).IsDefined(typeof(FlagsAttribute), false))
			{
				return null;
			}

			ulong mask = 0;
#if NET8_0_OR_GREATER
			foreach (TEnum member in Enum.GetValues<TEnum>())
#else
			foreach (TEnum member in (TEnum[])Enum.GetValues(typeof(TEnum)))
#endif
			{
				mask |= member.ToBits();
			}

			return mask;
		}
	}
}
