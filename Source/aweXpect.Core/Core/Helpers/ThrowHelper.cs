using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace aweXpect.Core.Helpers;

internal static class ThrowHelper
{
	/// <summary>
	///     Rejects a negative count, because an occurrence count is never below zero.
	/// </summary>
	/// <remarks>
	///     The <paramref name="description" /> defaults to the parameter name, which reads naturally for a
	///     <c>minimum</c> or a <c>maximum</c>, but not for an <c>expected</c> count.
	/// </remarks>
	public static void ThrowIfCountIsNegative(int? count, string? description = null,
		[CallerArgumentExpression(nameof(count))]
		string? paramName = null)
	{
		if (count < 0)
		{
			// ReSharper disable once LocalizableElement
			throw Tracing.WriteException(new ArgumentOutOfRangeException(paramName,
				$"The {description ?? paramName} must not be negative."));
		}
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Rejects a negative number of bytes, because an allocation is never below zero.
	/// </summary>
	public static void ThrowIfBytesAreNegative(long bytes,
		[CallerArgumentExpression(nameof(bytes))]
		string? paramName = null)
	{
		if (bytes < 0)
		{
			// ReSharper disable once LocalizableElement
			throw Tracing.WriteException(new ArgumentOutOfRangeException(paramName,
				$"The {paramName} must not be negative."));
		}
	}
#endif

	/// <summary>
	///     Rejects an exclusive <paramref name="maximum" /> of zero, so that a negated expectation cannot silently
	///     succeed on a range that can never contain anything.
	/// </summary>
	public static void ThrowIfNoCountIsFewerThan(int maximum)
	{
		if (maximum < 1)
		{
			// ReSharper disable once LocalizableElement
			throw Tracing.WriteException(new ArgumentOutOfRangeException(nameof(maximum),
				"The maximum must be greater than zero."));
		}
	}

	/// <summary>
	///     Rejects an exclusive <paramref name="minimum" /> of <see cref="int.MaxValue" />, so that a negated
	///     expectation cannot silently succeed on a range that can never contain anything.
	/// </summary>
	public static void ThrowIfNoCountIsMoreThan(int minimum)
	{
		if (minimum == int.MaxValue)
		{
			// ReSharper disable once LocalizableElement
			throw Tracing.WriteException(new ArgumentOutOfRangeException(nameof(minimum),
				"The minimum must be less than 2147483647."));
		}
	}

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
	///     Rejects the <paramref name="option" /> when the <paramref name="earlierOption" /> is another option of the
	///     same group, because the later option would silently set the same thing in a different way.
	/// </summary>
	/// <remarks>
	///     The same option again replaces the earlier one.
	/// </remarks>
	public static void ThrowIfCompetingOption(string? earlierOption, string option)
	{
		if (earlierOption is not null && earlierOption != option)
		{
			// ReSharper disable once LocalizableElement
			throw Tracing.WriteException(new InvalidOperationException(
				$"{option} cannot be combined with {earlierOption}."));
		}
	}

	/// <summary>
	///     Rejects the <paramref name="option" /> when it <paramref name="isAlreadySpecified" />.
	/// </summary>
	/// <remarks>
	///     Only for settings that span the whole expectation chain, where a second call usually comes from another
	///     expectation of the chain.
	/// </remarks>
	public static void ThrowIfAlreadySpecified(bool isAlreadySpecified, string option)
	{
		if (isAlreadySpecified)
		{
			// ReSharper disable once LocalizableElement
			throw Tracing.WriteException(new InvalidOperationException(
				$"{option} cannot be specified more than once."));
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
		[CallerArgumentExpression(nameof(duration))]
		string? paramName = null)
	{
		if (duration < TimeSpan.Zero)
		{
			// ReSharper disable once LocalizableElement
			throw Tracing.WriteException(new ArgumentOutOfRangeException(paramName,
				$"The {description ?? paramName} must not be negative."));
		}
	}

	/// <summary>
	///     Rejects a negative timeout, except <see cref="System.Threading.Timeout.InfiniteTimeSpan" />, which imposes no
	///     limit.
	/// </summary>
	public static void ThrowIfTimeoutIsNegative(TimeSpan timeout,
		[CallerArgumentExpression(nameof(timeout))]
		string? paramName = null)
	{
		if (timeout != Timeout.InfiniteTimeSpan)
		{
			ThrowIfDurationIsNegative(timeout, paramName: paramName);
		}
	}

	/// <summary>
	///     Rejects a negative timeout like <see cref="ThrowIfTimeoutIsNegative(TimeSpan, string?)" />, while
	///     <see langword="null" /> stands for the default timeout.
	/// </summary>
	public static void ThrowIfTimeoutIsNegative(TimeSpan? timeout,
		[CallerArgumentExpression(nameof(timeout))]
		string? paramName = null)
	{
		if (timeout is not null)
		{
			ThrowIfTimeoutIsNegative(timeout.Value, paramName);
		}
	}

	/// <summary>
	///     Rejects a tolerance with a sub-day remainder, because a date without a time of day cannot honour it and
	///     would silently drop it.
	/// </summary>
	/// <remarks>
	///     aweXpect has its own copy for tolerances outside a <see cref="Options.DayTolerance" />; keep the message
	///     identical.
	/// </remarks>
	public static void ThrowIfToleranceIsNotWholeDays(TimeSpan tolerance)
	{
		if (tolerance.Ticks % TimeSpan.TicksPerDay != 0)
		{
			// ReSharper disable once LocalizableElement
			throw Tracing.WriteException(
				new ArgumentOutOfRangeException(nameof(tolerance), "The tolerance must be a whole number of days."));
		}
	}
}
