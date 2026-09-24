using System.Collections.Generic;
#if NET8_0_OR_GREATER
using System.Collections.Frozen;
using System.Collections.Immutable;
#endif

namespace aweXpect.Helpers;

internal static class ComparerHelpers
{
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
