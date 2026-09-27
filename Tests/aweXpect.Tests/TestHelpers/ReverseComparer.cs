using System.Collections.Generic;

namespace aweXpect.Tests;

/// <summary>
///     A comparer that orders strings in reverse ordinal order.
/// </summary>
internal class ReverseComparer : IComparer<string>
{
	public int Compare(string? x, string? y) => string.CompareOrdinal(y, x);
}
