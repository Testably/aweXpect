using System;
using System.Runtime.CompilerServices;

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
	public static void ThrowIfCountIsNegative(int count, string? description = null,
		[CallerArgumentExpression(nameof(count))] string? paramName = null)
	{
		if (count < 0)
		{
			// ReSharper disable once LocalizableElement
			throw Tracing.WriteException(new ArgumentOutOfRangeException(paramName,
				$"The {description ?? paramName} must not be negative."));
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
	///     Rejects the <paramref name="option" /> when the <paramref name="existingOption" /> already specified the
	///     same setting, because the later option would silently replace the earlier one.
	/// </summary>
	public static void ThrowIfOptionIsAlreadySpecified(string? existingOption, string option)
	{
		if (existingOption is not null)
		{
			// ReSharper disable once LocalizableElement
			throw Tracing.WriteException(new InvalidOperationException(existingOption == option
				? $"{option} cannot be specified more than once."
				: $"{option} cannot be combined with {existingOption}."));
		}
	}

	/// <summary>
	///     Rejects the <paramref name="option" /> when it <paramref name="isAlreadySpecified" />, because the later
	///     value would silently replace the earlier one.
	/// </summary>
	public static void ThrowIfOptionIsAlreadySpecified(bool isAlreadySpecified, string option)
		=> ThrowIfOptionIsAlreadySpecified(isAlreadySpecified ? option : null, option);

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

	/// <summary>
	///     Rejects a negative timeout, except <see cref="System.Threading.Timeout.InfiniteTimeSpan" />, which imposes no
	///     limit.
	/// </summary>
	public static void ThrowIfTimeoutIsNegative(TimeSpan timeout,
		[CallerArgumentExpression(nameof(timeout))] string? paramName = null)
	{
		if (timeout != System.Threading.Timeout.InfiniteTimeSpan)
		{
			ThrowIfDurationIsNegative(timeout, paramName: paramName);
		}
	}
}
