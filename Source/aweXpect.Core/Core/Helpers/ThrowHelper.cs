using System;
using System.Runtime.CompilerServices;

namespace aweXpect.Core.Helpers;

internal static class ThrowHelper
{
	/// <summary>
	///     Rejects an inverted range, so that a negated expectation cannot silently succeed on a range that can
	///     never contain anything.
	/// </summary>
	public static void ThrowIfMaximumIsBelowMinimum<T>(T? minimum, T? maximum)
		where T : struct, IComparable<T>
	{
		if (minimum is not null && maximum is not null && maximum.Value.CompareTo(minimum.Value) < 0)
		{
			// ReSharper disable once LocalizableElement
			throw Tracing.WriteException(new ArgumentOutOfRangeException(nameof(maximum),
				"The maximum must be greater than or equal to the minimum."));
		}
	}

	/// <summary>
	///     Rejects a negative duration, because an elapsed time is never below zero and the bound could therefore
	///     only ever be unsatisfiable or vacuous.
	/// </summary>
	/// <remarks>
	///     The <paramref name="description" /> defaults to the parameter name, which reads naturally for a
	///     <c>duration</c>, a <c>minimum</c> or a <c>maximum</c>, but not for every caller.
	/// </remarks>
	public static void ThrowIfDurationIsNegative(TimeSpan duration, string? description = null,
		[CallerArgumentExpression(nameof(duration))] string? paramName = null)
	{
		if (duration < TimeSpan.Zero)
		{
			// ReSharper disable once LocalizableElement
			throw Tracing.WriteException(new ArgumentOutOfRangeException(paramName,
				$"The {description ?? paramName} must not be negative."));
		}
	}
}
