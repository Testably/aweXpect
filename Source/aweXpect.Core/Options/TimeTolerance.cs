using System;
using aweXpect.Core.Helpers;
using aweXpect.Customization;

namespace aweXpect.Options;

/// <summary>
///     Tolerance for time comparisons.
/// </summary>
/// <remarks>
///     For date values without a time of day, such as <c>DateOnly</c>, use a <see cref="DayTolerance" /> instead,
///     which rejects a tolerance that is not a whole number of days.
/// </remarks>
public class TimeTolerance
{
	/// <summary>
	///     The tolerance to apply on the time comparisons.
	/// </summary>
	public TimeSpan? Tolerance { get; private set; }

	/// <summary>
	///     Returns the <see cref="Tolerance" />, or the
	///     <see cref="AwexpectCustomization.SettingsCustomization.DefaultTimeComparisonTolerance" /> if none is set.
	/// </summary>
	public virtual TimeSpan GetToleranceOrDefault()
		=> Tolerance ?? GetDefaultTolerance();

	/// <summary>
	///     Sets the tolerance to apply on the time comparisons.
	/// </summary>
	/// <exception cref="ArgumentOutOfRangeException">The <paramref name="tolerance" /> is negative.</exception>
	/// <exception cref="InvalidOperationException">A tolerance is already set.</exception>
	public virtual void SetTolerance(TimeSpan tolerance)
	{
		ToleranceHelpers.ThrowIfInvalid(tolerance);
		ThrowHelper.ThrowIfOptionIsAlreadySpecified(Tolerance is not null, "Within");
		Tolerance = tolerance;
	}

	/// <summary>
	///     A string representation in total days.
	/// </summary>
	/// <remarks>
	///     Without an explicit <see cref="Tolerance" />, the whole days of the
	///     <see cref="AwexpectCustomization.SettingsCustomization.DefaultTimeComparisonTolerance" /> are shown,
	///     unless there are none.
	/// </remarks>
	public string ToDayString()
	{
		int days = (int)(Tolerance ?? GetDefaultTolerance()).TotalDays;
		if (Tolerance is null && days == 0)
		{
			return "";
		}

		const char plusMinus = '\u00b1';
		return days == 1 ? $" {plusMinus} 1 day" : $" {plusMinus} {days} days";
	}

	/// <inheritdoc />
	/// <remarks>
	///     Without an explicit <see cref="Tolerance" />, the
	///     <see cref="AwexpectCustomization.SettingsCustomization.DefaultTimeComparisonTolerance" /> is shown,
	///     unless it is zero.
	/// </remarks>
	public override string ToString()
	{
		TimeSpan tolerance = Tolerance ?? GetDefaultTolerance();
		if (Tolerance is null && tolerance == TimeSpan.Zero)
		{
			return "";
		}

		return ToleranceHelpers.Format(tolerance);
	}

	private static TimeSpan GetDefaultTolerance()
		=> Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Get();
}
