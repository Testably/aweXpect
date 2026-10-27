using System;
using aweXpect.Core;
using aweXpect.Core.Helpers;
using aweXpect.Customization;

namespace aweXpect.Options;

/// <summary>
///     Tolerance for time comparisons.
/// </summary>
/// <remarks>
///     An immutable value: <see cref="Within(TimeSpan)" /> returns a new tolerance, and <see langword="default" /> is
///     the <see cref="Default" /> tolerance. For date values without a time of day, such as <c>DateOnly</c>, start
///     from <see cref="Days" /> instead, which rejects a tolerance that is not a whole number of days.
/// </remarks>
[ImmutableOptions]
public readonly struct TimeTolerance
{
	private const long UnsetDays = 1;
	private const long FirstSetDays = 2;

	/// <summary>
	///     Encodes the kind and the tolerance in one number: <c>0</c> is an unset time tolerance, a negative value the
	///     complement of the ticks of a set time tolerance, <c>1</c> an unset day tolerance and a larger value the whole
	///     days of a set day tolerance plus two.
	/// </summary>
	/// <remarks>
	///     The complement covers every tolerance up to <see cref="TimeSpan.MaxValue" />, and a day tolerance is always a
	///     whole number of days, so that the value is as small as a <see cref="TimeSpan" />.
	/// </remarks>
	private readonly long _value;

	private TimeTolerance(long value)
	{
		_value = value;
	}

	/// <summary>
	///     The tolerance of time values before one is specified, which applies the
	///     <see cref="AwexpectCustomization.SettingsCustomization.DefaultTimeComparisonTolerance" />.
	/// </summary>
	public static TimeTolerance Default => default;

	/// <summary>
	///     The tolerance of date values without a time of day, such as <c>DateOnly</c>, before one is specified.
	/// </summary>
	/// <remarks>
	///     It rejects a tolerance that is not a whole number of days as soon as it is specified, applies only the whole
	///     days of the <see cref="AwexpectCustomization.SettingsCustomization.DefaultTimeComparisonTolerance" /> and is
	///     written in days.
	/// </remarks>
	public static TimeTolerance Days => new(UnsetDays);

	/// <summary>
	///     The tolerance to apply on the time comparisons.
	/// </summary>
	public TimeSpan? Tolerance => _value switch
	{
		< 0 => new TimeSpan(~_value),
		>= FirstSetDays => new TimeSpan((_value - FirstSetDays) * TimeSpan.TicksPerDay),
		_ => null,
	};

	private bool IsDays => _value > 0;

	/// <summary>
	///     Returns the <see cref="Tolerance" />, or the
	///     <see cref="AwexpectCustomization.SettingsCustomization.DefaultTimeComparisonTolerance" /> if none is set.
	/// </summary>
	/// <remarks>
	///     For a tolerance of <see cref="Days" />, only the whole days of the default apply, as a date has no time of day.
	/// </remarks>
	public TimeSpan GetToleranceOrDefault()
	{
		if (Tolerance is { } tolerance)
		{
			return tolerance;
		}

		TimeSpan defaultTolerance = GetDefaultTolerance();
		return IsDays ? TimeSpan.FromDays((int)defaultTolerance.TotalDays) : defaultTolerance;
	}

	/// <summary>
	///     Sets the tolerance to apply on the time comparisons.
	/// </summary>
	/// <remarks>
	///     A later tolerance replaces the earlier one.
	/// </remarks>
	/// <exception cref="ArgumentOutOfRangeException">
	///     The <paramref name="tolerance" /> is negative, or not a whole number of days for a tolerance of
	///     <see cref="Days" />.
	/// </exception>
	public TimeTolerance Within(TimeSpan tolerance)
	{
		ToleranceHelpers.ThrowIfInvalid(tolerance);
		if (!IsDays)
		{
			return new TimeTolerance(~tolerance.Ticks);
		}

		ThrowHelper.ThrowIfToleranceIsNotWholeDays(tolerance);
		return new TimeTolerance(FirstSetDays + tolerance.Ticks / TimeSpan.TicksPerDay);
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
		TimeSpan? tolerance = Tolerance;
		int days = (int)(tolerance ?? GetDefaultTolerance()).TotalDays;
		if (tolerance is null && days == 0)
		{
			return "";
		}

		const char plusMinus = '±';
		return days == 1 ? $" {plusMinus} 1 day" : $" {plusMinus} {days} days";
	}

	/// <inheritdoc />
	/// <remarks>
	///     Without an explicit <see cref="Tolerance" />, the
	///     <see cref="AwexpectCustomization.SettingsCustomization.DefaultTimeComparisonTolerance" /> is shown,
	///     unless it is zero. A tolerance of <see cref="Days" /> is written like <see cref="ToDayString()" />.
	/// </remarks>
	public override string ToString()
	{
		if (IsDays)
		{
			return ToDayString();
		}

		TimeSpan? explicitTolerance = Tolerance;
		TimeSpan tolerance = explicitTolerance ?? GetDefaultTolerance();
		if (explicitTolerance is null && tolerance == TimeSpan.Zero)
		{
			return "";
		}

		return ToleranceHelpers.Format(tolerance);
	}

	private static TimeSpan GetDefaultTolerance()
		=> Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Get();
}
