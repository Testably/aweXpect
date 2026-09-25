using System;
using aweXpect.Core;
using aweXpect.Customization;

namespace aweXpect.Options;

/// <summary>
///     Tolerance for time comparisons.
/// </summary>
public class TimeTolerance
{
	/// <summary>
	///     The tolerance to apply on the time comparisons.
	/// </summary>
	public TimeSpan? Tolerance { get; private set; }

	/// <summary>
	///     Sets the tolerance to apply on the time comparisons.
	/// </summary>
	public virtual void SetTolerance(TimeSpan tolerance)
	{
		if (tolerance < TimeSpan.Zero)
		{
			throw Tracing.WriteException(
				new ArgumentOutOfRangeException(nameof(tolerance), "Tolerance must be non-negative"));
		}

		Tolerance = tolerance;
	}

	/// <summary>
	///     A string representation in total days.
	/// </summary>
	/// <remarks>
	///     Without an explicit <see cref="Tolerance" />, the whole days of the
	///     <see cref="AwexpectCustomization.SettingsCustomizationValue.DefaultTimeComparisonTolerance" /> are shown,
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
	///     <see cref="AwexpectCustomization.SettingsCustomizationValue.DefaultTimeComparisonTolerance" /> is shown,
	///     unless it is zero.
	/// </remarks>
	public override string ToString()
	{
		TimeSpan tolerance = Tolerance ?? GetDefaultTolerance();
		if (Tolerance is null && tolerance == TimeSpan.Zero)
		{
			return "";
		}

		return $" \u00b1 {Formatter.Format(tolerance)}";
	}

	private static TimeSpan GetDefaultTolerance()
		=> Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Get();
}
