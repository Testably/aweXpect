using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using aweXpect.Options;
#if NET8_0_OR_GREATER
using System.Runtime.InteropServices;
#endif
#if !NET8_0_OR_GREATER
using aweXpect.Helpers;
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

	/// <summary>
	///     Appends the difference to the bound of the range that <paramref name="actual" /> lies outside of, if any.
	/// </summary>
	private static void AppendDifferenceToRange<TNumber>(StringBuilder stringBuilder, TNumber? actual,
		TNumber? minimum, TNumber? maximum)
		where TNumber : struct, INumber<TNumber>
	{
		if (actual < minimum)
		{
			AppendDifference(stringBuilder, actual, minimum, "the minimum");
		}
		else if (actual > maximum)
		{
			AppendDifference(stringBuilder, actual, maximum, "the maximum");
		}
	}

	/// <summary>
	///     Appends the difference to the closest of the <paramref name="expected" /> values, if any has a finite
	///     distance to <paramref name="actual" />.
	/// </summary>
	private static void AppendDifferenceToClosest<TNumber>(StringBuilder stringBuilder, TNumber? actual,
		IEnumerable<TNumber?> expected, NumberTolerance<TNumber> options)
		where TNumber : struct, INumber<TNumber>
	{
		TNumber? closest = null;
		TNumber? smallestDistance = null;
		foreach (TNumber? value in expected)
		{
			if (options.CalculateDifference(actual, value) is { } distance && IsFinite<TNumber>(distance) &&
			    (smallestDistance is null || distance < smallestDistance))
			{
				closest = value;
				smallestDistance = distance;
			}
		}

		AppendDifference(stringBuilder, actual, closest, "the closest value");
	}

	/// <remarks>
	///     The signed difference is preferred, because the magnitude of a difference towards
	///     <c>MinValue</c> is not representable, while the difference itself is. For unsigned types it is the other
	///     way round, so the magnitude with an explicit sign remains as fallback. When neither is representable, the
	///     difference is calculated in a wider type. A difference of zero is omitted, because it tells nothing.
	/// </remarks>
	private static void AppendDifference<TNumber>(StringBuilder stringBuilder, TNumber? actual, TNumber? expected,
		string? reference = null)
		where TNumber : struct, INumber<TNumber>
	{
		if (actual is null || expected is null || actual == expected)
		{
			return;
		}

		if (typeof(TNumber) == typeof(char))
		{
			// A char difference is a distance between code points and would be unreadable as a character.
			AppendDifference<int>(stringBuilder, int.CreateTruncating(actual.Value), int.CreateTruncating(expected.Value),
				reference);
			return;
		}

		try
		{
			checked
			{
				TNumber? difference = actual - expected;
				if (IsFinite(difference))
				{
					stringBuilder.Append(", which differs by ");
					AppendDifferenceValue(stringBuilder, difference);
					AppendReference(stringBuilder, reference);
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
					stringBuilder.Append(actual > expected ? ", which differs by " : ", which differs by -");
					AppendDifferenceValue(stringBuilder, magnitude);
					AppendReference(stringBuilder, reference);
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
				: decimal.CreateChecked(actual.Value) - decimal.CreateChecked(expected.Value), reference);
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

	/// <summary>
	///     Appends the difference to the bound of the range that <paramref name="actual" /> lies outside of, if any.
	/// </summary>
	private static void AppendDifferenceToRange<TNumber>(StringBuilder stringBuilder, TNumber? actual,
		TNumber? minimum, TNumber? maximum, NumberTolerance<TNumber> options)
		where TNumber : struct, IComparable<TNumber>
	{
		if (actual is null)
		{
			return;
		}

		if (minimum is not null && actual.Value.CompareTo(minimum.Value) < 0)
		{
			AppendDifference(stringBuilder, actual, minimum, options, "the minimum");
		}
		else if (maximum is not null && actual.Value.CompareTo(maximum.Value) > 0)
		{
			AppendDifference(stringBuilder, actual, maximum, options, "the maximum");
		}
	}

	/// <summary>
	///     Appends the difference to the closest of the <paramref name="expected" /> values, if any has a finite
	///     distance to <paramref name="actual" />.
	/// </summary>
	private static void AppendDifferenceToClosest<TNumber>(StringBuilder stringBuilder, TNumber? actual,
		IEnumerable<TNumber?> expected, NumberTolerance<TNumber> options)
		where TNumber : struct, IComparable<TNumber>
	{
		TNumber? closest = null;
		TNumber? smallestDistance = null;
		foreach (TNumber? value in expected)
		{
			TNumber? distance;
			try
			{
				distance = options.CalculateDifference(actual, value);
			}
			catch (OverflowException)
			{
				continue;
			}

			if (distance is { } finiteDistance && IsFinite(finiteDistance) &&
			    (smallestDistance is null || finiteDistance.CompareTo(smallestDistance.Value) < 0))
			{
				closest = value;
				smallestDistance = finiteDistance;
			}
		}

		AppendDifference(stringBuilder, actual, closest, options, "the closest value");
	}

	/// <remarks>
	///     The signed difference is preferred, because the magnitude of a difference towards
	///     <c>MinValue</c> is not representable, while the difference itself is. For unsigned types it is the other
	///     way round, so the magnitude with an explicit sign remains as fallback. When neither is representable, the
	///     difference is calculated in a wider type. A difference of zero is omitted, because it tells nothing.
	/// </remarks>
	private static void AppendDifference<TNumber>(StringBuilder stringBuilder, TNumber? actual, TNumber? expected,
		NumberTolerance<TNumber> options, string? reference = null)
		where TNumber : struct, IComparable<TNumber>
	{
		if (actual is null || expected is null || actual.Value.CompareTo(expected.Value) == 0)
		{
			return;
		}

		TNumber? difference = CalculateSignedDifference(actual.Value, expected.Value);
		if (IsFinite(difference))
		{
			stringBuilder.Append(", which differs by ");
			AppendDifferenceValue(stringBuilder, difference);
			AppendReference(stringBuilder, reference);
			return;
		}

		try
		{
			TNumber? magnitude = options.CalculateDifference(actual, expected);
			if (IsFinite(magnitude))
			{
				stringBuilder.Append(actual.Value.CompareTo(expected.Value) >= 0
					? ", which differs by "
					: ", which differs by -");
				AppendDifferenceValue(stringBuilder, magnitude);
				AppendReference(stringBuilder, reference);
				return;
			}
		}
		catch (OverflowException)
		{
			// Fall back to the wider type below.
		}

		AppendWideDifference(stringBuilder, CalculateWideDifference(actual.Value, expected.Value), reference);
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

	private static void AppendWideDifference(StringBuilder stringBuilder, object? difference, string? reference)
	{
		switch (difference)
		{
			// The decimal formatter would render a trailing ".0" for the difference of two integers.
			case decimal integerDifference:
				stringBuilder.Append(", which differs by ")
					.Append(integerDifference.ToString(CultureInfo.InvariantCulture));
				AppendReference(stringBuilder, reference);
				break;
			case double floatingPointDifference when IsFinite<double>(floatingPointDifference):
				stringBuilder.Append(", which differs by ");
				AppendDifferenceValue(stringBuilder, floatingPointDifference);
				AppendReference(stringBuilder, reference);
				break;
		}
	}

	/// <remarks>
	///     A floating-point difference is rounded to 15 significant digits (7 for a <see langword="float" />): the values
	///     themselves are shown with all digits needed to tell them apart, so the difference only has to show how far
	///     apart they are, and should read 0.9 instead of 0.8999999999999999 for 2.0 and 1.1.
	/// </remarks>
	private static void AppendDifferenceValue(StringBuilder stringBuilder, object? difference)
		=> Formatter.Format(stringBuilder, difference switch
		{
			double value => RoundToSignificantDigits(value, 15),
			float value => (float)RoundToSignificantDigits(value, 7),
#if NET8_0_OR_GREATER
			NFloat value => (NFloat)RoundToSignificantDigits(value.Value, 15),
#endif
			_ => difference,
		});

	private static double RoundToSignificantDigits(double value, int significantDigits)
		=> double.TryParse(value.ToString($"G{significantDigits}", CultureInfo.InvariantCulture),
			NumberStyles.Float, CultureInfo.InvariantCulture, out double roundedValue) &&
		   !double.IsInfinity(roundedValue)
			? roundedValue
			: value;

	private static void AppendReference(StringBuilder stringBuilder, string? reference)
	{
		if (reference is not null)
		{
			stringBuilder.Append(" from ").Append(reference);
		}
	}
}
