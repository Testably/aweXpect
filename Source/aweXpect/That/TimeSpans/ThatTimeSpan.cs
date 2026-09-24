using System;
using aweXpect.Customization;
using aweXpect.Helpers;

namespace aweXpect;

/// <summary>
///     Expectations on <see cref="TimeSpan" /> values.
/// </summary>
public static partial class ThatTimeSpan
{
	private static bool IsWithinTolerance(TimeSpan? tolerance, TimeSpan actual, TimeSpan expected)
		=> actual.IsConsideredEqualTo(expected,
			tolerance ?? Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Get());
}
