using System.Collections;
using System.Collections.Generic;

namespace aweXpect.Helpers;

/// <summary>
///     The <paramref name="keys" /> of a sorted dictionary together with the <paramref name="comparer" /> that orders them.
/// </summary>
/// <remarks>
///     Unlike a set, the keys stay an ordered sequence, so that only the order expectations use the comparer.
/// </remarks>
internal sealed class SortedKeys<TKey>(IEnumerable<TKey> keys, IComparer<TKey> comparer) : IEnumerable<TKey>
{
	/// <summary>
	///     The comparer that orders the keys.
	/// </summary>
	public IComparer<TKey> Comparer { get; } = comparer;

	/// <inheritdoc />
	public IEnumerator<TKey> GetEnumerator() => keys.GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
