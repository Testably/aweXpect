using System;
using aweXpect.Customization;
using aweXpect.Options;

namespace aweXpect.Helpers;

internal static class ObjectEqualityWithToleranceOptionsFactory
{
	public static ObjectEqualityWithToleranceOptions<double, double> CreateDouble() =>
		new((a, e, t) => a.IsConsideredEqualTo(e, t),
			t => $" ± {Formatter.Format(t)}");

	public static ObjectEqualityWithToleranceOptions<double?, double> CreateNullableDouble() =>
		new((a, e, t) => a.IsConsideredEqualTo(e, t),
			t => $" ± {Formatter.Format(t)}");

	public static ObjectEqualityWithToleranceOptions<float, float> CreateFloat() =>
		new((a, e, t) => a.IsConsideredEqualTo(e, t),
			t => $" ± {Formatter.Format(t)}");

	public static ObjectEqualityWithToleranceOptions<float?, float> CreateNullableFloat() =>
		new((a, e, t) => a.IsConsideredEqualTo(e, t),
			t => $" ± {Formatter.Format(t)}");

	public static ObjectEqualityWithToleranceOptions<decimal, decimal> CreateDecimal() =>
		new((a, e, t) => a.IsConsideredEqualTo(e, t),
			t => $" ± {Formatter.Format(t)}");

	public static ObjectEqualityWithToleranceOptions<decimal?, decimal> CreateNullableDecimal() =>
		new((a, e, t) => a.IsConsideredEqualTo(e, t),
			t => $" ± {Formatter.Format(t)}");

	public static ObjectEqualityWithToleranceOptions<DateTime, TimeSpan> CreateDateTime() =>
		new ObjectEqualityWithToleranceOptions<DateTime, TimeSpan>((a, e, t) => a.IsConsideredEqualTo(e, t),
			t => $" ± {Formatter.Format(t)}").WithDefaultTolerance(DefaultTimeTolerance);

	public static ObjectEqualityWithToleranceOptions<DateTime?, TimeSpan> CreateNullableDateTime() =>
		new ObjectEqualityWithToleranceOptions<DateTime?, TimeSpan>((a, e, t) => a.IsConsideredEqualTo(e, t),
			t => $" ± {Formatter.Format(t)}").WithDefaultTolerance(DefaultTimeTolerance);

	public static ObjectEqualityWithToleranceOptions<DateTimeOffset, TimeSpan> CreateDateTimeOffset() =>
		new ObjectEqualityWithToleranceOptions<DateTimeOffset, TimeSpan>((a, e, t) => a.IsConsideredEqualTo(e, t),
			t => $" ± {Formatter.Format(t)}").WithDefaultTolerance(DefaultTimeTolerance);

	public static ObjectEqualityWithToleranceOptions<DateTimeOffset?, TimeSpan> CreateNullableDateTimeOffset() =>
		new ObjectEqualityWithToleranceOptions<DateTimeOffset?, TimeSpan>((a, e, t) => a.IsConsideredEqualTo(e, t),
			t => $" ± {Formatter.Format(t)}").WithDefaultTolerance(DefaultTimeTolerance);

	public static ObjectEqualityWithToleranceOptions<TimeSpan, TimeSpan> CreateTimeSpan() =>
		new ObjectEqualityWithToleranceOptions<TimeSpan, TimeSpan>((a, e, t) => a.IsConsideredEqualTo(e, t),
			t => $" ± {Formatter.Format(t)}").WithDefaultTolerance(DefaultTimeTolerance);

	public static ObjectEqualityWithToleranceOptions<TimeSpan?, TimeSpan> CreateNullableTimeSpan() =>
		new ObjectEqualityWithToleranceOptions<TimeSpan?, TimeSpan>((a, e, t) => a.IsConsideredEqualTo(e, t),
			t => $" ± {Formatter.Format(t)}").WithDefaultTolerance(DefaultTimeTolerance);

	private static TimeSpan DefaultTimeTolerance()
		=> Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Get();
}
