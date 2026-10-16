using System;
using aweXpect.Core.Helpers;
#if NET8_0_OR_GREATER
using System.Numerics;
#else
using System.Collections.Generic;
#endif

namespace aweXpect.Options;

/// <summary>
///     Tolerance for number comparisons.
/// </summary>
public class NumberTolerance<TNumber>(
	Func<TNumber, TNumber, TNumber?> calculateDifference)
	: IRelativeToleranceOptions<TNumber>
#if NET8_0_OR_GREATER
	where TNumber : struct, INumber<TNumber>
#else
	where TNumber : struct, IComparable<TNumber>
#endif
{
	private Func<TNumber, TNumber, TNumber>? _absoluteTolerance;
	private string _unit = "";

	/// <summary>
	///     The tolerance to apply on the number comparisons.
	/// </summary>
	/// <remarks>
	///     A relative tolerance is not yet applied to an expected value; use <see cref="ToleranceFor(TNumber)" /> for
	///     the absolute tolerance.
	/// </remarks>
	public TNumber? Tolerance { get; private set; }

	/// <inheritdoc cref="IRelativeToleranceOptions{TTolerance}.UseRelativeTolerance(Func{TTolerance, TTolerance, TTolerance}, string)" />
	public void UseRelativeTolerance(Func<TNumber, TNumber, TNumber> absoluteTolerance, string unit)
	{
		_absoluteTolerance = absoluteTolerance;
		_unit = unit;
	}

	/// <summary>
	///     Returns the absolute tolerance to apply on a comparison with the <paramref name="expected" /> value.
	/// </summary>
	public TNumber? ToleranceFor(TNumber expected)
		=> Tolerance is { } tolerance && _absoluteTolerance is not null
			? _absoluteTolerance(tolerance, expected)
			: Tolerance;

	/// <summary>
	///     Sets the tolerance to apply on the number comparisons.
	/// </summary>
	/// <exception cref="ArgumentOutOfRangeException">The <paramref name="tolerance" /> is negative or NaN.</exception>
	/// <exception cref="InvalidOperationException">A tolerance is already set.</exception>
	public void SetTolerance(TNumber tolerance)
	{
		ToleranceHelpers.ThrowIfInvalid(tolerance, IsNaN(tolerance));
		ThrowHelper.ThrowIfOptionIsAlreadySpecified(Tolerance is not null, "Within");
		Tolerance = tolerance;
	}

#if NET8_0_OR_GREATER
	private static bool IsNaN(TNumber tolerance) => TNumber.IsNaN(tolerance);
#else
	/// <remarks>
	///     Without generic math the check has to be written out per type; only the floating point types can be NaN.
	/// </remarks>
	private static bool IsNaN(TNumber tolerance) => tolerance switch
	{
		double d => double.IsNaN(d),
		float f => float.IsNaN(f),
		_ => false,
	};
#endif

	/// <inheritdoc />
	public override string ToString()
		=> Tolerance is { } tolerance ? ToleranceHelpers.Format(tolerance) + _unit : "";

	/// <summary>
	///     Calculates the difference between the <paramref name="actual" /> number
	///     and the <paramref name="expected" /> number.
	/// </summary>
	public TNumber? CalculateDifference(TNumber? actual, TNumber? expected)
	{
		if (actual == null || expected == null)
		{
			return null;
		}

		return calculateDifference(actual.Value, expected.Value);
	}

	/// <summary>
	///     Checks if the <paramref name="actual" /> number is within the tolerance to the
	///     <paramref name="expected" /> number.
	/// </summary>
	public bool IsWithinTolerance(TNumber? actual, TNumber? expected)
	{
		try
		{
			checked
			{
				return (actual, expected) switch
				{
					(null, null) => true,
					(_, null) => false,
					(null, _) => false,
#if NET8_0_OR_GREATER
					(_, _) => actual.Value.Equals(expected.Value) ||
					          calculateDifference(actual.Value, expected.Value) <= ToleranceFor(expected.Value),
#else
					(_, _) => EqualityComparer<TNumber>.Default.Equals(actual.Value, expected.Value) ||
					          calculateDifference(actual.Value, expected.Value)
						          ?.CompareTo(ToleranceFor(expected.Value) ?? default) <= 0,
#endif
				};
			}
		}
		catch (OverflowException)
		{
			return false;
		}
	}
}
