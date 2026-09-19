using System;
using aweXpect.Core;

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
	public void SetTolerance(TimeSpan tolerance)
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
	public string ToDayString()
	{
		if (Tolerance == null)
		{
			return "";
		}

		int days = (int)Tolerance.Value.TotalDays;
		const char plusMinus = '\u00b1';
		return $" {plusMinus} {days} days";
	}

	/// <inheritdoc />
	public override string ToString()
	{
		if (Tolerance == null)
		{
			return "";
		}

		return $" \u00b1 {Formatter.Format(Tolerance.Value)}";
	}
}
