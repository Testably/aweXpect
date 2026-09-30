using System;
using System.Collections.Generic;
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
	///     Returns the equality options for values of type <typeparamref name="TItem" />, which apply the default time
	///     tolerance when <typeparamref name="TItem" /> is one of the time types.
	/// </summary>
	/// <remarks>
	///     Serves the expectations whose value type is only a type parameter, so that none of the <c>Create…</c> methods
	///     can be chosen at compile time. Its name must not start with <c>Create</c>, as the generator would pick it up.
	/// </remarks>
	public static ObjectEqualityOptions<TItem> ForValuesOf<TItem>()
		=> TimeOptionsFactories.TryGetValue(typeof(TItem), out Func<object>? create)
			? (ObjectEqualityOptions<TItem>)create()
			: new ObjectEqualityOptions<TItem>();

	private static readonly Dictionary<Type, Func<object>> TimeOptionsFactories = new()
	{
		{ typeof(DateTime), CreateDateTime },
		{ typeof(DateTime?), CreateNullableDateTime },
		{ typeof(DateTimeOffset), CreateDateTimeOffset },
		{ typeof(DateTimeOffset?), CreateNullableDateTimeOffset },
		{ typeof(TimeSpan), CreateTimeSpan },
		{ typeof(TimeSpan?), CreateNullableTimeSpan },
#if NET8_0_OR_GREATER
		{ typeof(DateOnly), CreateDateOnly },
		{ typeof(DateOnly?), CreateNullableDateOnly },
		{ typeof(TimeOnly), CreateTimeOnly },
		{ typeof(TimeOnly?), CreateNullableTimeOnly },
#endif
	};

	/// <summary>
	///     Tells whether the tolerance <paramref name="options" /> created here still use their default match type.
	/// </summary>
	public static bool HasDefaultMatchType<TItem, TTolerance>(ObjectEqualityWithToleranceOptions<TItem, TTolerance> options)
		=> options is ItemEqualityWithToleranceOptions<TItem, TTolerance> { HasDefaultMatchType: true, };

	private static TimeSpan DefaultTimeTolerance()
		=> Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Get();
}
