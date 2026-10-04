using System;
using aweXpect.Core.Helpers;

namespace aweXpect.Options;

/// <summary>
///     A <see cref="TimeTolerance" /> for date values without a time of day, such as <c>DateOnly</c>, which rejects a
///     tolerance that is not a whole number of days as soon as it is specified.
/// </summary>
/// <remarks>
///     Pass it to a <see cref="Results.TimeToleranceResult{TType,TThat}" /> of an expectation on such values, so that
///     <see cref="ToleranceExtensions.Within{TResult}(TResult, TimeSpan)" /> does not silently drop the part
///     of the tolerance below one day.
/// </remarks>
public sealed class DayTolerance : TimeTolerance
{
	/// <inheritdoc />
	/// <remarks>
	///     Only the whole days of the default tolerance apply, as a date has no time of day.
	/// </remarks>
	public override TimeSpan GetToleranceOrDefault()
		=> Tolerance ?? TimeSpan.FromDays((int)base.GetToleranceOrDefault().TotalDays);

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
