using System;
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
		new((a, e, t) => a.IsConsideredEqualTo(e, t),
			t => $" ± {Formatter.Format(t)}");

	public static ObjectEqualityWithToleranceOptions<DateTime?, TimeSpan> CreateNullableDateTime() =>
		new((a, e, t) => a.IsConsideredEqualTo(e, t),
			t => $" ± {Formatter.Format(t)}");
}
