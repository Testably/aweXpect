using System;
using aweXpect.Customization;
using aweXpect.Helpers;

namespace aweXpect;

/// <summary>
///     Expectations on <see cref="TimeSpan" />? values.
/// </summary>
public static partial class ThatNullableTimeSpan
{
	/// <remarks>
	///     Compares the shifted values instead of the difference, because the difference between two
	///     <see cref="TimeSpan" /> values can exceed the range of a <see cref="TimeSpan" />.
	/// </remarks>
	private static bool IsWithinTolerance(TimeSpan? tolerance, TimeSpan? actual, TimeSpan? expected)
	{
		if (actual is null || expected is null)
		{
			return false;
		}

		tolerance ??= Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Get();

		return actual.Value.ShiftedTicks(tolerance.Value) >= expected.Value.Ticks &&
		       expected.Value.ShiftedTicks(tolerance.Value) >= actual.Value.Ticks;
	}
}
