using System;
using System.Globalization;
using System.Text;
#if !NET8_0_OR_GREATER
using System.Numerics;
#endif

namespace aweXpect.Snapshots;

/// <summary>
///     Writes a <see cref="double" /> or <see cref="float" /> like <c>R</c> does on .NET: with the fewest significant
///     digits that round-trip, in fixed notation up to 17 digits (9 for a <see cref="float" />) before the decimal point.
/// </summary>
/// <remarks>
///     .NET Framework writes 15 digits with <c>R</c>, or 17 when they do not round-trip, e.g. <c>0.33333333333333331</c>
///     for <c>1.0 / 3</c> instead of <c>0.3333333333333333</c> on .NET. Its formatting and parsing are also not exact
///     beyond 15 digits, so below .NET 8 the digits are calculated exactly, so that a name is the same on every target
///     framework of a test project.
/// </remarks>
internal static class RoundTripNumber
{
#if NET8_0_OR_GREATER
	public static void Append(StringBuilder stringBuilder, double value, bool isSingle)
		=> stringBuilder.Append(isSingle
			? ((float)value).ToString("R", CultureInfo.InvariantCulture)
			: value.ToString("R", CultureInfo.InvariantCulture));
#else
	public static void Append(StringBuilder stringBuilder, double value, bool isSingle)
	{
		if (double.IsNaN(value) || double.IsInfinity(value))
		{
			stringBuilder.Append(value.ToString(CultureInfo.InvariantCulture));
			return;
		}

		long bits = BitConverter.DoubleToInt64Bits(value);
		if (bits < 0)
		{
			stringBuilder.Append('-');
		}

		if ((bits & long.MaxValue) == 0)
		{
			stringBuilder.Append('0');
			return;
		}

		GetMantissaAndExponent(value, isSingle, out long mantissa, out int exponent, out bool hasNarrowerLowerGap);
		int maxDigits = isSingle ? 9 : 17;
		int decimalExponent = GetDecimalExponent(mantissa, exponent);
		for (int digitCount = 1; digitCount <= maxDigits; digitCount++)
		{
			int scale = decimalExponent - digitCount + 1;
			BigInteger digits = RoundToInteger(mantissa, exponent, scale);
			int digitsExponent = decimalExponent;
			if (digits == BigInteger.Pow(10, digitCount))
			{
				digits = BigInteger.Pow(10, digitCount - 1);
				scale++;
				digitsExponent++;
			}

			if (digitCount == maxDigits || RoundTrips(digits, scale, mantissa, exponent, hasNarrowerLowerGap))
			{
				AppendDigits(stringBuilder, digits.ToString(CultureInfo.InvariantCulture).TrimEnd('0'),
					digitsExponent, maxDigits);
				return;
			}
		}
	}

	/// <summary>
	///     The absolute value as <paramref name="mantissa" /> * 2^<paramref name="exponent" />, and whether the next
	///     smaller value is closer than the next larger one, as for a power of two.
	/// </summary>
	private static void GetMantissaAndExponent(double value, bool isSingle, out long mantissa, out int exponent,
		out bool hasNarrowerLowerGap)
	{
		if (isSingle)
		{
			int bits = BitConverter.ToInt32(BitConverter.GetBytes((float)value), 0);
			int biasedExponent = (bits >> 23) & 0xFF;
			mantissa = bits & 0x7FFFFF;
			exponent = biasedExponent == 0 ? -149 : biasedExponent - 150;
			if (biasedExponent != 0)
			{
				mantissa |= 1L << 23;
			}

			hasNarrowerLowerGap = biasedExponent > 1 && mantissa == 1L << 23;
		}
		else
		{
			long bits = BitConverter.DoubleToInt64Bits(value);
			int biasedExponent = (int)((bits >> 52) & 0x7FF);
			mantissa = bits & 0xFFFFFFFFFFFFFL;
			exponent = biasedExponent == 0 ? -1074 : biasedExponent - 1075;
			if (biasedExponent != 0)
			{
				mantissa |= 1L << 52;
			}

			hasNarrowerLowerGap = biasedExponent > 1 && mantissa == 1L << 52;
		}
	}

	/// <summary>
	///     The exponent <c>k</c> with 10^k &lt;= <paramref name="mantissa" /> * 2^<paramref name="exponent" /> &lt;
	///     10^(k+1).
	/// </summary>
	/// <remarks>
	///     The logarithm is only an estimate, which is corrected exactly.
	/// </remarks>
	private static int GetDecimalExponent(long mantissa, int exponent)
	{
		int decimalExponent = (int)Math.Floor(Math.Log10(mantissa) + (exponent * Math.Log10(2)));
		while (Compare(mantissa, exponent, BigInteger.One, decimalExponent) < 0)
		{
			decimalExponent--;
		}

		while (Compare(mantissa, exponent, BigInteger.One, decimalExponent + 1) >= 0)
		{
			decimalExponent++;
		}

		return decimalExponent;
	}

	/// <summary>
	///     Rounds <paramref name="mantissa" /> * 2^<paramref name="exponent" /> / 10^<paramref name="scale" /> to an
	///     integer, half to even.
	/// </summary>
	private static BigInteger RoundToInteger(long mantissa, int exponent, int scale)
	{
		Scale(mantissa, exponent, BigInteger.One, scale, out BigInteger numerator, out BigInteger denominator);
		BigInteger quotient = BigInteger.DivRem(numerator, denominator, out BigInteger remainder);
		int half = (remainder * 2).CompareTo(denominator);
		return half > 0 || (half == 0 && !quotient.IsEven) ? quotient + 1 : quotient;
	}

	/// <summary>
	///     Whether <paramref name="digits" /> * 10^<paramref name="scale" /> is parsed back to the value, i.e. lies
	///     between the midpoints to its neighbors, where a midpoint belongs to the value with the even mantissa.
	/// </summary>
	/// <remarks>
	///     In units of 2^(exponent - 2), the value is 4 * mantissa, and its neighbors are 4 apart, but the smaller one only
	///     2 for a power of two.
	/// </remarks>
	private static bool RoundTrips(BigInteger digits, int scale, long mantissa, int exponent,
		bool hasNarrowerLowerGap)
	{
		long upperMidpoint = (4 * mantissa) + 2;
		long lowerMidpoint = hasNarrowerLowerGap ? (4 * mantissa) - 1 : (4 * mantissa) - 2;
		int toUpper = Compare(upperMidpoint, exponent - 2, digits, scale);
		int toLower = Compare(lowerMidpoint, exponent - 2, digits, scale);
		return mantissa % 2 == 0 ? toUpper >= 0 && toLower <= 0 : toUpper > 0 && toLower < 0;
	}

	/// <summary>
	///     Compares <paramref name="mantissa" /> * 2^<paramref name="exponent" /> with <paramref name="digits" /> *
	///     10^<paramref name="scale" />.
	/// </summary>
	private static int Compare(long mantissa, int exponent, BigInteger digits, int scale)
	{
		Scale(mantissa, exponent, digits, scale, out BigInteger left, out BigInteger right);
		return left.CompareTo(right);
	}

	/// <summary>
	///     Multiplies <paramref name="mantissa" /> * 2^<paramref name="exponent" /> and <paramref name="digits" /> *
	///     10^<paramref name="scale" /> with the same factor, so that both are integers.
	/// </summary>
	private static void Scale(long mantissa, int exponent, BigInteger digits, int scale, out BigInteger left,
		out BigInteger right)
	{
		left = mantissa;
		right = digits;
		if (exponent >= 0)
		{
			left <<= exponent;
		}
		else
		{
			right <<= -exponent;
		}

		if (scale >= 0)
		{
			right *= BigInteger.Pow(10, scale);
		}
		else
		{
			left *= BigInteger.Pow(10, -scale);
		}
	}

	/// <summary>
	///     Writes the <paramref name="digits" /> in the notation of <c>R</c> on .NET: in scientific notation when the
	///     decimal point would be more than <paramref name="maxDigits" /> digits right of the first digit, or more than
	///     three zeros left of it.
	/// </summary>
	private static void AppendDigits(StringBuilder stringBuilder, string digits, int exponent, int maxDigits)
	{
		int decimalPoint = exponent + 1;
		if (decimalPoint > maxDigits || decimalPoint < -3)
		{
			stringBuilder.Append(digits[0]);
			if (digits.Length > 1)
			{
				stringBuilder.Append('.').Append(digits, 1, digits.Length - 1);
			}

			stringBuilder.Append(exponent < 0 ? "E-" : "E+")
				.Append(Math.Abs(exponent).ToString("00", CultureInfo.InvariantCulture));
		}
		else if (decimalPoint <= 0)
		{
			stringBuilder.Append("0.").Append('0', -decimalPoint).Append(digits);
		}
		else if (decimalPoint >= digits.Length)
		{
			stringBuilder.Append(digits).Append('0', decimalPoint - digits.Length);
		}
		else
		{
			stringBuilder.Append(digits, 0, decimalPoint).Append('.')
				.Append(digits, decimalPoint, digits.Length - decimalPoint);
		}
	}
#endif
}
