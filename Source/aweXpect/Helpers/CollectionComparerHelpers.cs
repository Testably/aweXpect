using System;
using System.Collections.Generic;
using aweXpect.Core;
#if NET8_0_OR_GREATER
using System.Collections.Frozen;
using System.Collections.Immutable;
#else
using System.Reflection;
#endif

namespace aweXpect.Helpers;

internal static class CollectionComparerHelpers
{
	/// <summary>
	///     Returns the <paramref name="keys" /> of the <paramref name="dictionary" /> as a set with its key comparer,
	///     when that comparer is not the default one, so that expectations on the keys use it as well.
	/// </summary>
	/// <remarks>
	///     The keys of a dictionary are unique for its comparer, so the set holds all of them, in the same order. The keys
	///     of a sorted dictionary with the default comparer keep that comparer as their order, see
	///     <see cref="SortedKeys{TKey}" />.
	/// </remarks>
	public static IEnumerable<TKey>? GetKeys<TKey, TValue>(IEnumerable<KeyValuePair<TKey, TValue>>? dictionary,
		IEnumerable<TKey>? keys)
	{
		if (dictionary is null || keys is null)
		{
			return keys;
		}

		ISet<TKey> set = KeyComparers.CreateKeySet(dictionary);
		if (GetSubjectComparer<TKey>(set) is null)
		{
			return set is SortedSet<TKey> sortedSet ? new SortedKeys<TKey>(keys, sortedSet.Comparer) : keys;
		}

		foreach (TKey key in keys)
		{
			set.Add(key);
		}

		return set;
	}

#if !NET8_0_OR_GREATER
	private static readonly Dictionary<string, string> SetComparerProperties = new()
	{
		["System.Collections.Immutable.ImmutableHashSet`1"] = "KeyComparer",
		["System.Collections.Immutable.ImmutableSortedSet`1"] = "KeyComparer",
		["System.Collections.Frozen.FrozenSet`1"] = "Comparer",
	};

	/// <summary>
	///     Reads the comparer of a <paramref name="collection" /> of one of the types in
	///     <paramref name="comparerProperties" />, which netstandard2.0 lacks or which expose it only on newer runtimes,
	///     when it is a <typeparamref name="TComparer" />.
	/// </summary>
	/// <remarks>
	///     The types are matched by name and their comparer is read from its public property, which needs reflection,
	///     so it is only attempted while the <see cref="ReflectionFallback" /> is supported.
	/// </remarks>
	private static TComparer? ReadComparer<TComparer>(object collection,
		Dictionary<string, string> comparerProperties)
		where TComparer : class
	{
		if (!ReflectionFallback.IsSupported)
		{
			return null;
		}

		for (Type? type = collection.GetType(); type is not null; type = type.BaseType)
		{
			if (type.IsGenericType && type.GetGenericTypeDefinition().FullName is { } name &&
			    comparerProperties.TryGetValue(name, out string? propertyName))
			{
				PropertyInfo? property = type.GetProperty(propertyName);
				return property?.PropertyType == typeof(TComparer) ? property.GetValue(collection) as TComparer : null;
			}
		}

		return null;
	}
#endif

	/// <summary>
	///     Returns the comparer of the <paramref name="collection" />, when it is a set that exposes a comparer other
	///     than the default one for <typeparamref name="T" />, or <see langword="null" /> otherwise.
	/// </summary>
	/// <remarks>
	///     Only such a set is known to decide differently than the default equality, and a set that does not expose its
	///     comparer cannot be told apart from one that uses the default. The known types are checked one by one, so
	///     that no reflection is needed, which would not survive trimming. netstandard2.0 lacks some of these types, so
	///     they are matched by name there. A sorted set considers two items the same when its comparer orders neither
	///     before the other.
	/// </remarks>
	public static SubjectComparer<T>? GetSubjectComparer<T>(object? collection)
		=> collection switch
		{
			HashSet<T> set when IsCustom(set.Comparer) => new SubjectComparer<T>(set.Comparer.Equals, set.Comparer),
			SortedSet<T> set when IsCustom(set.Comparer) => FromOrder(set.Comparer),
#if NET8_0_OR_GREATER
			ImmutableHashSet<T> set when IsCustom(set.KeyComparer)
				=> new SubjectComparer<T>(set.KeyComparer.Equals, set.KeyComparer),
			ImmutableSortedSet<T> set when IsCustom(set.KeyComparer) => FromOrder(set.KeyComparer),
			FrozenSet<T> set when IsCustom(set.Comparer) => new SubjectComparer<T>(set.Comparer.Equals, set.Comparer),
#else
			not null when ReadComparer<IEqualityComparer<T>>(collection, SetComparerProperties) is { } comparer &&
			              IsCustom(comparer)
				=> new SubjectComparer<T>(comparer.Equals, comparer),
			not null when ReadComparer<IComparer<T>>(collection, SetComparerProperties) is { } comparer &&
			              IsCustom(comparer)
				=> FromOrder(comparer),
#endif
			_ => null,
		};

	/// <summary>
	///     Considers two items equal when the <paramref name="comparer" /> orders neither before the other.
	/// </summary>
	/// <remarks>
	///     A lambda in <see cref="GetSubjectComparer{T}" /> would capture its pattern variables in a closure that is
	///     allocated for every call, even for a subject that is no set.
	/// </remarks>
	private static SubjectComparer<T> FromOrder<T>(IComparer<T> comparer)
		=> new((x, y) => comparer.Compare(x, y) == 0, comparer);

	/// <summary>
	///     Returns the comparer that orders the items of the <paramref name="collection" />, when it is a sorted set or
	///     the <see cref="SortedKeys{TKey}" /> of a sorted dictionary with a comparer other than the
	///     <paramref name="defaultOrder" />, or <see langword="null" /> otherwise.
	/// </summary>
	/// <remarks>
	///     The comparison is against the <paramref name="defaultOrder" /> rather than <see cref="Comparer{T}.Default" />,
	///     because they differ for strings, so that a sorted set with the default comparer is also in its own order.
	/// </remarks>
	public static IComparer<T>? GetSubjectOrder<T>(object? collection, IComparer<T> defaultOrder)
		=> collection switch
		{
			SortedSet<T> set when !Equals(set.Comparer, defaultOrder) => set.Comparer,
			SortedKeys<T> keys when !Equals(keys.Comparer, defaultOrder) => keys.Comparer,
#if NET8_0_OR_GREATER
			ImmutableSortedSet<T> set when !Equals(set.KeyComparer, defaultOrder) => set.KeyComparer,
#else
			not null when ReadComparer<IComparer<T>>(collection, SetComparerProperties) is { } comparer &&
			              !Equals(comparer, defaultOrder) => comparer,
#endif
			_ => null,
		};

	/// <summary>
	///     Describes that the <paramref name="comparer" /> of the subject decides, e.g.
	///     <c>using the subject's StringComparer.OrdinalIgnoreCase</c>.
	/// </summary>
	/// <remarks>
	///     A <see cref="StringComparer" /> is named by the property that returns it, because its type is internal to the
	///     framework and differs between target frameworks.
	/// </remarks>
	public static string DescribeSubjectComparer(object comparer)
		=> " using the subject's " + comparer switch
		{
			StringComparer c when c.Equals(StringComparer.Ordinal) => "StringComparer.Ordinal",
			StringComparer c when c.Equals(StringComparer.OrdinalIgnoreCase) => "StringComparer.OrdinalIgnoreCase",
			StringComparer c when c.Equals(StringComparer.InvariantCulture) => "StringComparer.InvariantCulture",
			StringComparer c when c.Equals(StringComparer.InvariantCultureIgnoreCase)
				=> "StringComparer.InvariantCultureIgnoreCase",
			StringComparer c when c.Equals(StringComparer.CurrentCulture) => "StringComparer.CurrentCulture",
			StringComparer c when c.Equals(StringComparer.CurrentCultureIgnoreCase)
				=> "StringComparer.CurrentCultureIgnoreCase",
			_ => Formatter.Format(comparer.GetType()),
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
