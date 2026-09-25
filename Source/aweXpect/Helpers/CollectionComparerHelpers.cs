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

	/// <summary>
	///     Indicates whether the <paramref name="collection" /> is a set that exposes a comparer other than the default
	///     one for <typeparamref name="T" />.
	/// </summary>
	/// <remarks>
	///     Only such a set is known to decide differently than the default equality, and a set that does not expose its
	///     comparer cannot be told apart from one that uses the default. The known types are checked one by one, so
	///     that no reflection is needed, which would not survive trimming.
	/// </remarks>
	public static bool IsSetWithCustomComparer<T>(IEnumerable<T> collection)
		=> collection switch
		{
			HashSet<T> set => IsCustom(set.Comparer),
			SortedSet<T> set => IsCustom(set.Comparer),
#if NET8_0_OR_GREATER
			ImmutableHashSet<T> set => IsCustom(set.KeyComparer),
			ImmutableSortedSet<T> set => IsCustom(set.KeyComparer),
			FrozenSet<T> set => IsCustom(set.Comparer),
#endif
			_ => false,
		};

	/// <summary>
	///     Indicates whether the <paramref name="comparer" /> is not the default equality comparer for
	///     <typeparamref name="T" />.
	/// </summary>
	public static bool IsCustom<T>(IEqualityComparer<T> comparer)
		=> !Equals(comparer, EqualityComparer<T>.Default);

	/// <summary>
	///     Indicates whether the <paramref name="comparer" /> is not the default comparer for <typeparamref name="T" />.
	/// </summary>
	public static bool IsCustom<T>(IComparer<T> comparer)
		=> !Equals(comparer, Comparer<T>.Default);
}
