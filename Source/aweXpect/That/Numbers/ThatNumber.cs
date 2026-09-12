using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
#if !NET8_0_OR_GREATER
using aweXpect.Options;
#endif

namespace aweXpect;

/// <summary>
///     Expectations on numeric values.
/// </summary>
public static partial class ThatNumber
{
	private static bool IsFinite<T>([NotNullWhen(true)] T? value) => value switch
	{
		null => false,
		double d => !double.IsNaN(d) && !double.IsInfinity(d),
		float f => !float.IsNaN(f) && !float.IsInfinity(f),
#if NET8_0_OR_GREATER
		Half h => !Half.IsNaN(h) && !Half.IsInfinity(h),
#endif
		_ => true,
	};

#if NET8_0_OR_GREATER
	private static TNumber? CalculateDifference<TNumber>(TNumber actual, TNumber expected)
		where TNumber : struct, INumber<TNumber>
	{
		if (actual == expected)
		{
			return default(TNumber);
		}

		if (!IsFinite(actual))
		{
			return !IsFinite(expected) ? default(TNumber) : null;
		}

		try
		{
			checked
			{
				return actual < expected
					? expected - actual
					: actual - expected;
			}
		}
		catch (OverflowException)
		{
			return null;
		}
	}

	/// <remarks>
	///     The signed difference is preferred, because the magnitude of a difference towards
	///     <c>MinValue</c> is not representable, while the difference itself is. For unsigned types it is the other
	///     way round, so the magnitude with an explicit sign remains as fallback.
	/// </remarks>
	private static void AppendDifference<TNumber>(StringBuilder stringBuilder, TNumber? actual, TNumber? expected)
		where TNumber : struct, INumber<TNumber>
	{
		try
		{
			checked
			{
				TNumber? difference = actual - expected;
				if (IsFinite(difference))
				{
					stringBuilder.Append(" which differs by ");
					Formatter.Format(stringBuilder, difference);
					return;
				}
			}
		}
		catch (OverflowException)
		{
			// Fall back to the magnitude below.
		}

		try
		{
			checked
			{
				TNumber? magnitude = actual > expected ? actual - expected : expected - actual;
				if (IsFinite(magnitude))
				{
					stringBuilder.Append(actual > expected ? " which differs by " : " which differs by -");
					Formatter.Format(stringBuilder, magnitude);
				}
			}
		}
		catch (OverflowException)
		{
			// Do not display the difference in case of overflow.
		}
	}
#else
	/// <remarks>
	///     The signed difference is preferred, because the magnitude of a difference towards
	///     <c>MinValue</c> is not representable, while the difference itself is. For unsigned types it is the other
	///     way round, so the magnitude with an explicit sign remains as fallback.
	/// </remarks>
	private static void AppendDifference<TNumber>(StringBuilder stringBuilder, TNumber? actual, TNumber? expected,
		NumberTolerance<TNumber> options)
		where TNumber : struct, IComparable<TNumber>
	{
		if (actual is null || expected is null)
		{
			return;
		}

		TNumber? difference = CalculateSignedDifference(actual.Value, expected.Value);
		if (IsFinite(difference))
		{
			stringBuilder.Append(" which differs by ");
			Formatter.Format(stringBuilder, difference);
			return;
		}

		try
		{
			TNumber? magnitude = options.CalculateDifference(actual, expected);
			if (IsFinite(magnitude))
			{
				stringBuilder.Append(actual.Value.CompareTo(expected.Value) >= 0
					? " which differs by "
					: " which differs by -");
				Formatter.Format(stringBuilder, magnitude);
			}
		}
		catch (OverflowException)
		{
			// Do not display the difference in case of overflow.
		}
	}

	/// <remarks>
	///     Without generic math the subtraction has to be written out per type. Only the signed integer types are
	///     listed, because for every other type the magnitude with an explicit sign is representable whenever the
	///     signed difference is.
	/// </remarks>
	private static TNumber? CalculateSignedDifference<TNumber>(TNumber actual, TNumber expected)
		where TNumber : struct, IComparable<TNumber>
		=> (actual, expected) switch
		{
			(sbyte a, sbyte e) when a - e >= sbyte.MinValue && a - e <= sbyte.MaxValue
				=> (TNumber)(object)(sbyte)(a - e),
			(short a, short e) when a - e >= short.MinValue && a - e <= short.MaxValue
				=> (TNumber)(object)(short)(a - e),
			(int a, int e) when (long)a - e >= int.MinValue && (long)a - e <= int.MaxValue
				=> (TNumber)(object)(int)((long)a - e),
			(long a, long e) when (decimal)a - e >= long.MinValue && (decimal)a - e <= long.MaxValue
				=> (TNumber)(object)(long)((decimal)a - e),
			_ => null,
		};
#endif
}
