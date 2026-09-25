using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using aweXpect.Core;

namespace aweXpect.Helpers;

internal static class ThrowHelper
{
	/// <summary>
	///     Rejects a key that occurs more than once in the <paramref name="entries" /> of a dictionary expectation and
	///     returns them materialized, so that a sequence which can only be enumerated once survives both the guard
	///     and the subsequent comparison.
	/// </summary>
	/// <remarks>
	///     A dictionary holds one value per key, so a second entry for the same key either contradicts the first one
	///     or repeats it. The keys are compared with their default equality, because the key comparer of the subject
	///     is runtime data and must not decide whether the expectation itself is valid.
	/// </remarks>
	public static ICollection<KeyValuePair<TKey, TValue>>? EnsureDistinctKeys<TKey, TValue>(
		IEnumerable<KeyValuePair<TKey, TValue>>? entries,
		[CallerArgumentExpression(nameof(entries))] string? paramName = null)
		=> EnsureDistinctKeysNamed(entries, paramName);

	/// <summary>
	///     <see cref="EnsureDistinctKeys{TKey,TValue}(IEnumerable{KeyValuePair{TKey,TValue}}?,string?)" /> for the
	///     <paramref name="entries" /> named after the polarity of the expectation: the expected dictionary, or the
	///     unexpected one when <paramref name="negated" />.
	/// </summary>
	public static ICollection<KeyValuePair<TKey, TValue>>? EnsureDistinctKeys<TKey, TValue>(
		IEnumerable<KeyValuePair<TKey, TValue>>? entries, bool negated)
		=> EnsureDistinctKeysNamed(entries, negated ? "unexpected" : "expected");

	private static ICollection<KeyValuePair<TKey, TValue>>? EnsureDistinctKeysNamed<TKey, TValue>(
		IEnumerable<KeyValuePair<TKey, TValue>>? entries, string? paramName)
	{
		if (entries is null)
		{
			return null;
		}

		ICollection<KeyValuePair<TKey, TValue>> materializedEntries =
			entries as ICollection<KeyValuePair<TKey, TValue>> ?? entries.ToList();
		IGrouping<TKey, KeyValuePair<TKey, TValue>>? duplicate = materializedEntries
			.GroupBy(entry => entry.Key)
			.FirstOrDefault(group => group.Skip(1).Any());
		if (duplicate is not null)
		{
			// ReSharper disable once LocalizableElement
			throw Tracing.WriteException(new ArgumentException(
				$"The key {Formatter.Format(duplicate.Key)} must not occur more than once.", paramName));
		}

		return materializedEntries;
	}

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
	///     Rejects a negative duration, because an elapsed time is never below zero.
	/// </summary>
	public static void ThrowIfDurationIsNegative(TimeSpan duration,
		[CallerArgumentExpression(nameof(duration))] string? paramName = null)
	{
		if (duration < TimeSpan.Zero)
		{
			// ReSharper disable once LocalizableElement
			throw Tracing.WriteException(new ArgumentOutOfRangeException(paramName,
				$"The {paramName} must not be negative."));
		}
	}

	/// <summary>
	///     Rejects an inverted range, so that a tolerance cannot silently turn it into a satisfiable one.
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
	///     Rejects an inverted range of a reference type, so that a negated expectation cannot silently succeed.
	/// </summary>
	public static void ThrowIfMaximumIsBelowMinimum<T>(T? minimum, T? maximum)
		where T : class, IComparable<T>
	{
		if (minimum is not null && maximum is not null && maximum.CompareTo(minimum) < 0)
		{
			// ReSharper disable once LocalizableElement
			throw Tracing.WriteException(new ArgumentOutOfRangeException(nameof(maximum),
				"The maximum must be greater than or equal to the minimum."));
		}
	}

	/// <summary>
	///     Rejects a recursion depth below one at the call site, so that the exception names the caller's parameter
	///     instead of the option that is set from it.
	/// </summary>
	public static void ThrowIfRecursionDepthIsNotPositive(int maximumRecursionDepth)
	{
		if (maximumRecursionDepth < 1)
		{
			throw Tracing.WriteException(
				new ArgumentOutOfRangeException(nameof(maximumRecursionDepth), maximumRecursionDepth,
					"The maximum recursion depth must be greater than zero."));
		}
	}

	/// <summary>
	///     Rejects a tolerance with a sub-day remainder, because a date without a time of day cannot honour it and
	///     would silently drop it.
	/// </summary>
	public static void ThrowIfToleranceIsNotWholeDays(TimeSpan tolerance)
	{
		if (tolerance.Ticks % TimeSpan.TicksPerDay != 0)
		{
			throw Tracing.WriteException(
				new ArgumentOutOfRangeException(nameof(tolerance), "Tolerance must be a whole number of days"));
		}
	}
}
