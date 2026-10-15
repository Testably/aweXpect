using System;
using System.Collections.Generic;

namespace aweXpect.Core.Helpers;

internal static class StringComparerExtensions
{
	/// <summary>
	///     The comparison that decides like the <paramref name="comparer" />, when it is one of the ordinal or culture-aware
	///     comparers of <see cref="StringComparer" />.
	/// </summary>
	/// <remarks>
	///     A culture-aware comparer is recognized by its equality to the well-known comparer, which compares the culture
	///     and the options, so that this does not call into a custom comparer.
	/// </remarks>
	public static StringComparison? GetComparison(this IEqualityComparer<string> comparer)
	{
		if (ReferenceEquals(comparer, StringComparer.Ordinal))
		{
			return StringComparison.Ordinal;
		}

		if (ReferenceEquals(comparer, StringComparer.OrdinalIgnoreCase))
		{
			return StringComparison.OrdinalIgnoreCase;
		}

		if (StringComparer.InvariantCulture.Equals(comparer))
		{
			return StringComparison.InvariantCulture;
		}

		if (StringComparer.InvariantCultureIgnoreCase.Equals(comparer))
		{
			return StringComparison.InvariantCultureIgnoreCase;
		}

		if (StringComparer.CurrentCulture.Equals(comparer))
		{
			return StringComparison.CurrentCulture;
		}

		if (StringComparer.CurrentCultureIgnoreCase.Equals(comparer))
		{
			return StringComparison.CurrentCultureIgnoreCase;
		}

		return null;
	}
}
