using System;
using aweXpect.Core.Helpers;

namespace aweXpect.Options;

/// <summary>
///     A <see cref="TimeTolerance" /> for date values without a time of day, such as <c>DateOnly</c>, which rejects a
///     tolerance that is not a whole number of days as soon as it is specified.
/// </summary>
/// <remarks>
///     Pass it to a <see cref="Results.TimeToleranceResult{TType,TThat}" /> of an expectation on such values, so that
///     <see cref="Results.TimeToleranceResult{TType,TThat,TSelf}.Within(TimeSpan)" /> does not silently drop the part
///     of the tolerance below one day.
/// </remarks>
public sealed class DayTolerance : TimeTolerance
{
	/// <inheritdoc />
	/// <exception cref="ArgumentOutOfRangeException">
	///     The <paramref name="tolerance" /> is negative or not a whole number of days.
	/// </exception>
	/// <remarks>
	///     A negative or repeated tolerance is left to the base class, so that its exception takes precedence, and a
	///     rejected tolerance is never stored.
	/// </remarks>
	public override void SetTolerance(TimeSpan tolerance)
	{
		if (tolerance >= TimeSpan.Zero && Tolerance is null)
		{
			ThrowHelper.ThrowIfToleranceIsNotWholeDays(tolerance);
		}

		base.SetTolerance(tolerance);
	}
}
