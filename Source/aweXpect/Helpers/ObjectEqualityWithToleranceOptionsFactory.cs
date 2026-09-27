using System;
using aweXpect.Customization;
using aweXpect.Options;

namespace aweXpect.Helpers;

internal static class ObjectEqualityWithToleranceOptionsFactory
{
	public static ObjectEqualityWithToleranceOptions<double, double> CreateDouble() =>
		new ItemEqualityWithToleranceOptions<double, double>((a, e, t) => a.IsConsideredEqualTo(e, t),
			t => $" ± {Formatter.Format(t)}");

	public static ObjectEqualityWithToleranceOptions<double?, double> CreateNullableDouble() =>
		new ItemEqualityWithToleranceOptions<double?, double>((a, e, t) => a.IsConsideredEqualTo(e, t),
			t => $" ± {Formatter.Format(t)}");

	public static ObjectEqualityWithToleranceOptions<float, float> CreateFloat() =>
		new ItemEqualityWithToleranceOptions<float, float>((a, e, t) => a.IsConsideredEqualTo(e, t),
			t => $" ± {Formatter.Format(t)}");

	public static ObjectEqualityWithToleranceOptions<float?, float> CreateNullableFloat() =>
		new ItemEqualityWithToleranceOptions<float?, float>((a, e, t) => a.IsConsideredEqualTo(e, t),
			t => $" ± {Formatter.Format(t)}");

	public static ObjectEqualityWithToleranceOptions<decimal, decimal> CreateDecimal() =>
		new ItemEqualityWithToleranceOptions<decimal, decimal>((a, e, t) => a.IsConsideredEqualTo(e, t),
			t => $" ± {Formatter.Format(t)}");

	public static ObjectEqualityWithToleranceOptions<decimal?, decimal> CreateNullableDecimal() =>
		new ItemEqualityWithToleranceOptions<decimal?, decimal>((a, e, t) => a.IsConsideredEqualTo(e, t),
			t => $" ± {Formatter.Format(t)}");

	public static ObjectEqualityWithToleranceOptions<DateTime, TimeSpan> CreateDateTime() =>
		new ItemEqualityWithToleranceOptions<DateTime, TimeSpan>((a, e, t) => a.IsConsideredEqualTo(e, t),
			t => $" ± {Formatter.Format(t)}", DefaultTimeTolerance);

	public static ObjectEqualityWithToleranceOptions<DateTime?, TimeSpan> CreateNullableDateTime() =>
		new ItemEqualityWithToleranceOptions<DateTime?, TimeSpan>((a, e, t) => a.IsConsideredEqualTo(e, t),
			t => $" ± {Formatter.Format(t)}", DefaultTimeTolerance);

	public static ObjectEqualityWithToleranceOptions<DateTimeOffset, TimeSpan> CreateDateTimeOffset() =>
		new ItemEqualityWithToleranceOptions<DateTimeOffset, TimeSpan>((a, e, t) => a.IsConsideredEqualTo(e, t),
			t => $" ± {Formatter.Format(t)}", DefaultTimeTolerance);

	public static ObjectEqualityWithToleranceOptions<DateTimeOffset?, TimeSpan> CreateNullableDateTimeOffset() =>
		new ItemEqualityWithToleranceOptions<DateTimeOffset?, TimeSpan>((a, e, t) => a.IsConsideredEqualTo(e, t),
			t => $" ± {Formatter.Format(t)}", DefaultTimeTolerance);

	public static ObjectEqualityWithToleranceOptions<TimeSpan, TimeSpan> CreateTimeSpan() =>
		new ItemEqualityWithToleranceOptions<TimeSpan, TimeSpan>((a, e, t) => a.IsConsideredEqualTo(e, t),
			t => $" ± {Formatter.Format(t)}", DefaultTimeTolerance);

	public static ObjectEqualityWithToleranceOptions<TimeSpan?, TimeSpan> CreateNullableTimeSpan() =>
		new ItemEqualityWithToleranceOptions<TimeSpan?, TimeSpan>((a, e, t) => a.IsConsideredEqualTo(e, t),
			t => $" ± {Formatter.Format(t)}", DefaultTimeTolerance);

#if NET8_0_OR_GREATER
	public static ObjectEqualityWithToleranceOptions<DateOnly, TimeSpan> CreateDateOnly() =>
		new ItemEqualityWithToleranceOptions<DateOnly, TimeSpan>((a, e, t) => a.IsConsideredEqualTo(e, t),
				ToDayString, DefaultDayTolerance)
			.WithToleranceValidation(ThrowHelper.ThrowIfToleranceIsNotWholeDays);

	public static ObjectEqualityWithToleranceOptions<DateOnly?, TimeSpan> CreateNullableDateOnly() =>
		new ItemEqualityWithToleranceOptions<DateOnly?, TimeSpan>((a, e, t) => a.IsConsideredEqualTo(e, t),
				ToDayString, DefaultDayTolerance)
			.WithToleranceValidation(ThrowHelper.ThrowIfToleranceIsNotWholeDays);

	public static ObjectEqualityWithToleranceOptions<TimeOnly, TimeSpan> CreateTimeOnly() =>
		new ItemEqualityWithToleranceOptions<TimeOnly, TimeSpan>((a, e, t) => a.IsConsideredEqualTo(e, t),
			t => $" ± {Formatter.Format(t)}", DefaultTimeTolerance);

	public static ObjectEqualityWithToleranceOptions<TimeOnly?, TimeSpan> CreateNullableTimeOnly() =>
		new ItemEqualityWithToleranceOptions<TimeOnly?, TimeSpan>((a, e, t) => a.IsConsideredEqualTo(e, t),
			t => $" ± {Formatter.Format(t)}", DefaultTimeTolerance);

	/// <remarks>
	///     A date only honours the whole days of the default tolerance, so a shorter one is not named in the text.
	/// </remarks>
	private static TimeSpan DefaultDayTolerance()
		=> TimeSpan.FromDays((int)DefaultTimeTolerance().TotalDays);

	private static string ToDayString(TimeSpan tolerance)
	{
		int days = (int)tolerance.TotalDays;
		return days == 1 ? " ± 1 day" : $" ± {days} days";
	}
#endif

	/// <summary>
	///     Tells whether the tolerance <paramref name="options" /> created here still use their default match type.
	/// </summary>
	public static bool HasDefaultMatchType<TItem, TTolerance>(ObjectEqualityWithToleranceOptions<TItem, TTolerance> options)
		=> options is ItemEqualityWithToleranceOptions<TItem, TTolerance> { HasDefaultMatchType: true, };

	private static TimeSpan DefaultTimeTolerance()
		=> Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Get();
}
