using System;
using aweXpect.Core;
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
/// <remarks>
///     An immutable value: the option methods return a new tolerance. The <see langword="default" /> value has no
///     difference function, so it only considers equal numbers to be within any tolerance.
/// </remarks>
[ImmutableOptions]
public readonly struct NumberTolerance<TNumber>
#if NET8_0_OR_GREATER
	where TNumber : struct, INumber<TNumber>
#else
	where TNumber : struct, IComparable<TNumber>
#endif
{
	private readonly Func<TNumber, TNumber, TNumber?>? _calculateDifference;
	private readonly Func<TNumber, TNumber, TNumber>? _absoluteTolerance;
	private readonly string? _unit;

	/// <summary>
	///     Creates a tolerance without a tolerance value, which compares with the
	///     <paramref name="calculateDifference" /> once a tolerance is specified.
	/// </summary>
	public NumberTolerance(Func<TNumber, TNumber, TNumber?> calculateDifference)
		: this(calculateDifference, null, null, null)
	{
	}

	private NumberTolerance(Func<TNumber, TNumber, TNumber?>? calculateDifference, TNumber? tolerance,
		Func<TNumber, TNumber, TNumber>? absoluteTolerance, string? unit)
	{
		_calculateDifference = calculateDifference;
		Tolerance = tolerance;
		_absoluteTolerance = absoluteTolerance;
		_unit = unit;
	}

	/// <summary>
	///     The tolerance to apply on the number comparisons.
	/// </summary>
	/// <remarks>
	///     A relative tolerance is not yet applied to an expected value; use <see cref="ToleranceFor(TNumber)" /> for
	///     the absolute tolerance.
	/// </remarks>
	public TNumber? Tolerance { get; }

	/// <summary>
	///     Interprets the specified tolerance relative to the expected value.
	/// </summary>
	/// <param name="absoluteTolerance">
	///     Calculates the absolute tolerance from the specified tolerance and the expected value.
	/// </param>
	/// <param name="unit">The unit appended to the tolerance in the expectation text, e.g. <c>%</c>.</param>
	/// <remarks>
	///     A later relative tolerance replaces the earlier one, and a later <see cref="Within(TNumber)" /> replaces
	///     both.
	/// </remarks>
	/// <exception cref="InvalidOperationException">No tolerance was specified with <see cref="Within(TNumber)" />.</exception>
	public NumberTolerance<TNumber> SetRelativeTolerance(Func<TNumber, TNumber, TNumber> absoluteTolerance,
		string unit)
	{
		absoluteTolerance.ThrowIfNull();
		unit.ThrowIfNull();
		ToleranceHelpers.ThrowIfRelativeToleranceHasNoTolerance(Tolerance is not null);
		return new NumberTolerance<TNumber>(_calculateDifference, Tolerance, absoluteTolerance, unit);
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
	/// <remarks>
	///     A later tolerance replaces the earlier one, including a relative tolerance.
	/// </remarks>
	/// <exception cref="ArgumentOutOfRangeException">The <paramref name="tolerance" /> is negative or NaN.</exception>
	public NumberTolerance<TNumber> Within(TNumber tolerance)
	{
		ToleranceHelpers.ThrowIfInvalid(tolerance, IsNaN(tolerance));
		return new NumberTolerance<TNumber>(_calculateDifference, tolerance, null, null);
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

		return Difference(actual.Value, expected.Value);
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
					          Difference(actual.Value, expected.Value) <= ToleranceFor(expected.Value),
#else
					(_, _) => EqualityComparer<TNumber>.Default.Equals(actual.Value, expected.Value) ||
					          Difference(actual.Value, expected.Value)
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

	private TNumber? Difference(TNumber actual, TNumber expected)
		=> _calculateDifference?.Invoke(actual, expected);
}
