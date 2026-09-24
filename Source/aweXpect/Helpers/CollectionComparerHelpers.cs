using System.Collections.Generic;
#if NET8_0_OR_GREATER
using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.Collections.Immutable;
#endif

namespace aweXpect.Helpers;

internal static class CollectionComparerHelpers
{
	/// <summary>
	///     Creates an empty set of keys that treats keys as the same exactly when the key comparer of the
	///     <paramref name="dictionary" /> does.
	/// </summary>
	/// <remarks>
	///     The comparer is read through known dictionary types instead of reflection, so that it stays safe for trimming
	///     and Native AOT. A set with the default equality is returned for any other dictionary.
	///     <para />
	///     CS8714 is suppressed, because the <see langword="notnull" /> constraint of these types only concerns the
	///     annotation of <typeparamref name="TKey" />, which a type test at runtime ignores.
	/// </remarks>
#pragma warning disable CS8714
	public static ISet<TKey> CreateKeySet<TKey, TValue>(IEnumerable<KeyValuePair<TKey, TValue>> dictionary)
		=> dictionary switch
		{
			Dictionary<TKey, TValue> d => new HashSet<TKey>(d.Comparer),
			SortedDictionary<TKey, TValue> d => new SortedSet<TKey>(d.Comparer),
			SortedList<TKey, TValue> d => new SortedSet<TKey>(d.Comparer),
#if NET8_0_OR_GREATER
			ConcurrentDictionary<TKey, TValue> d => new HashSet<TKey>(d.Comparer),
			ImmutableDictionary<TKey, TValue> d => new HashSet<TKey>(d.KeyComparer),
			ImmutableSortedDictionary<TKey, TValue> d => new SortedSet<TKey>(d.KeyComparer),
			FrozenDictionary<TKey, TValue> d => new HashSet<TKey>(d.Comparer),
#endif
			_ => new HashSet<TKey>(),
		};
#pragma warning restore CS8714
}
