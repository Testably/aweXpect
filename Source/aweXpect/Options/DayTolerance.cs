#if NET8_0_OR_GREATER
using System;
using aweXpect.Helpers;

namespace aweXpect.Options;

/// <summary>
///     A <see cref="TimeTolerance" /> for <see cref="DateOnly" /> values, which rejects a tolerance that is not a whole
///     number of days as soon as it is specified.
/// </summary>
internal sealed class DayTolerance : TimeTolerance
{
	/// <inheritdoc />
	public override void SetTolerance(TimeSpan tolerance)
	{
		base.SetTolerance(tolerance);
		ThrowHelper.ThrowIfToleranceIsNotWholeDays(tolerance);
	}
}
#endif
