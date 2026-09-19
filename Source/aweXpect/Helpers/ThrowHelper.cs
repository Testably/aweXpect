using System;

namespace aweXpect.Helpers;

internal static class ThrowHelper
{
	public static ArgumentException EmptyCollection()
		=> new("You have to provide at least one expected value!");

	/// <summary>
	///     Rejects an inverted range, so that a tolerance cannot silently turn it into a satisfiable one.
	/// </summary>
	public static void ThrowIfMaximumIsBelowMinimum<T>(T? minimum, T? maximum)
		where T : struct, IComparable<T>
	{
		if (minimum is not null && maximum is not null && maximum.Value.CompareTo(minimum.Value) < 0)
		{
			// ReSharper disable once LocalizableElement
			throw new ArgumentOutOfRangeException(nameof(maximum),
				"The maximum must be greater than or equal to the minimum.");
		}
	}
}
