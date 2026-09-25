using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
#if !NET8_0_OR_GREATER
using aweXpect.Helpers;
using aweXpect.Options;
#endif

namespace aweXpect;

/// <summary>
///     Expectations on numeric values.
/// </summary>
public static partial class ThatNumber
{
#if NET8_0_OR_GREATER
	private static bool IsFinite<TNumber>([NotNullWhen(true)] TNumber? value)
		where TNumber : struct, INumberBase<TNumber>
		=> value is not null && !TNumber.IsNaN(value.Value) && !TNumber.IsInfinity(value.Value);

	private static TNumber? CalculateDifference<TNumber>(TNumber actual, TNumber expected)
		where TNumber : struct, INumber<TNumber>
	{
		if (actual == expected)
		{
			return default(TNumber);
		}

		if (!IsFinite<TNumber>(actual) || !IsFinite<TNumber>(expected))
		{
			return null;
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
	///     way round, so the magnitude with an explicit sign remains as fallback. When neither is representable, the
	///     difference is calculated in a wider type.
	/// </remarks>
	private static void AppendDifference<TNumber>(StringBuilder stringBuilder, TNumber? actual, TNumber? expected)
		where TNumber : struct, INumber<TNumber>
	{
		if (actual is null || expected is null)
		{
			return;
		}

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
					return;
				}
			}
		}
		catch (OverflowException)
		{
			// Fall back to the wider type below.
		}

		try
		{
			AppendWideDifference(stringBuilder, typeof(TNumber) == typeof(float) || typeof(TNumber) == typeof(Half)
				? double.CreateChecked(actual.Value) - double.CreateChecked(expected.Value)
				: decimal.CreateChecked(actual.Value) - decimal.CreateChecked(expected.Value));
		}
		catch (Exception ex) when (ex is OverflowException or NotSupportedException)
		{
			// Do not display the difference, if it overflows even in the wider type or cannot be converted to it.
		}
	}
#else
	private static bool IsFinite<T>([NotNullWhen(true)] T? value) => value switch
	{
		null => false,
		double d => !double.IsNaN(d) && !double.IsInfinity(d),
		float f => !float.IsNaN(f) && !float.IsInfinity(f),
		_ => true,
	};

	/// <remarks>
	///     The generated negated overloads name the parameter <c>unexpected</c>, so the exception names it as well.
	/// </remarks>
	private static void ThrowIfNaN<TNumber>(TNumber? expected, bool negated)
		where TNumber : struct, IComparable<TNumber>
	{
		if (negated)
		{
			TNumber? unexpected = expected;
			unexpected.ThrowIfNaN("unexpected value");
		}
		else
		{
			expected.ThrowIfNaN("expected value");
		}
	}

	/// <remarks>
	///     The signed difference is preferred, because the magnitude of a difference towards
	///     <c>MinValue</c> is not representable, while the difference itself is. For unsigned types it is the other
	///     way round, so the magnitude with an explicit sign remains as fallback. When neither is representable, the
	///     difference is calculated in a wider type.
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
				return;
			}
		}
		catch (OverflowException)
		{
			// Fall back to the wider type below.
		}

		AppendWideDifference(stringBuilder, CalculateWideDifference(actual.Value, expected.Value));
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

	/// <remarks>
	///     Only the signed integer types and <see langword="float" /> are listed, because the magnitude of every other
	///     type is either representable or has no wider type.
	/// </remarks>
	private static object? CalculateWideDifference<TNumber>(TNumber actual, TNumber expected)
		where TNumber : struct, IComparable<TNumber>
		=> (actual, expected) switch
		{
			(sbyte a, sbyte e) => (decimal)a - e,
			(short a, short e) => (decimal)a - e,
			(int a, int e) => (decimal)a - e,
			(long a, long e) => (decimal)a - e,
			(float a, float e) => (double)a - e,
			_ => null,
		};
#endif

	private static void AppendWideDifference(StringBuilder stringBuilder, object? difference)
	{
		switch (difference)
		{
			// The decimal formatter would render a trailing ".0" for the difference of two integers.
			case decimal integerDifference:
				stringBuilder.Append(" which differs by ")
					.Append(integerDifference.ToString(CultureInfo.InvariantCulture));
				break;
			case double floatingPointDifference when IsFinite<double>(floatingPointDifference):
				stringBuilder.Append(" which differs by ");
				Formatter.Format(stringBuilder, floatingPointDifference);
				break;
		}
	}
}
