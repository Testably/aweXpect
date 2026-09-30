using System;
using System.Collections.Generic;
#if NET8_0_OR_GREATER
using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.Collections.Immutable;
#else
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
#endif
using aweXpect.Core;

namespace aweXpect.Helpers;

internal static class CollectionComparerHelpers
{
	/// <summary>
	///     Creates an empty set of keys that treats keys as the same exactly when the key comparer of the
	///     <paramref name="dictionary" /> does.
	/// </summary>
	/// <remarks>
	///     The comparer is read through known dictionary types instead of reflection, so that it stays safe for trimming
	///     and Native AOT. A set with the default equality is returned for any other dictionary. netstandard2.0 lacks some
	///     of these types or their comparer, so they are matched by name there.
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
#else
			_ when ReadKeyComparer<IEqualityComparer<TKey>>(dictionary) is { } comparer
				=> new HashSet<TKey>(comparer),
			_ when ReadKeyComparer<IComparer<TKey>>(dictionary) is { } comparer => new SortedSet<TKey>(comparer),
#endif
			_ => new HashSet<TKey>(),
		};

	/// <summary>
	///     Returns the <paramref name="keys" /> of the <paramref name="dictionary" /> as a set with its key comparer,
	///     when that comparer is not the default one, so that expectations on the keys use it as well.
	/// </summary>
	/// <remarks>
	///     The keys of a dictionary are unique for its comparer, so the set holds all of them, in the same order.
	/// </remarks>
	public static IEnumerable<TKey>? GetKeys<TKey, TValue>(IEnumerable<KeyValuePair<TKey, TValue>>? dictionary,
		IEnumerable<TKey>? keys)
		=> dictionary switch
		{
			Dictionary<TKey, TValue> d when IsCustom(d.Comparer) => new HashSet<TKey>(d.Keys, d.Comparer),
			SortedDictionary<TKey, TValue> d when IsCustom(d.Comparer) => new SortedSet<TKey>(d.Keys, d.Comparer),
			SortedList<TKey, TValue> d when IsCustom(d.Comparer) => new SortedSet<TKey>(d.Keys, d.Comparer),
#if NET8_0_OR_GREATER
			ConcurrentDictionary<TKey, TValue> d when IsCustom(d.Comparer) => new HashSet<TKey>(d.Keys, d.Comparer),
			ImmutableDictionary<TKey, TValue> d when IsCustom(d.KeyComparer)
				=> new HashSet<TKey>(d.Keys, d.KeyComparer),
			ImmutableSortedDictionary<TKey, TValue> d when IsCustom(d.KeyComparer)
				=> new SortedSet<TKey>(d.Keys, d.KeyComparer),
			FrozenDictionary<TKey, TValue> d when IsCustom(d.Comparer) => new HashSet<TKey>(d.Keys, d.Comparer),
#else
			not null when ReadKeyComparer<IEqualityComparer<TKey>>(dictionary) is { } comparer && IsCustom(comparer)
				=> new HashSet<TKey>(dictionary.Select(pair => pair.Key), comparer),
			not null when ReadKeyComparer<IComparer<TKey>>(dictionary) is { } comparer && IsCustom(comparer)
				=> new SortedSet<TKey>(dictionary.Select(pair => pair.Key), comparer),
#endif
			_ => keys,
		};
#pragma warning restore CS8714

#if !NET8_0_OR_GREATER
	private static readonly Dictionary<string, string> KeyComparerProperties = new()
	{
		["System.Collections.Concurrent.ConcurrentDictionary`2"] = "Comparer",
		["System.Collections.Immutable.ImmutableDictionary`2"] = "KeyComparer",
		["System.Collections.Immutable.ImmutableSortedDictionary`2"] = "KeyComparer",
		["System.Collections.Frozen.FrozenDictionary`2"] = "Comparer",
	};

	/// <summary>
	///     Reads the key comparer of a dictionary type that netstandard2.0 lacks or that exposes it only on newer runtimes,
	///     when it is a <typeparamref name="TComparer" />.
	/// </summary>
	/// <remarks>
	///     The types are matched by name and their comparer is read from its public property, which needs reflection,
	///     so it is only attempted while the <see cref="ReflectionFallback" /> is supported. The
	///     <see cref="ConcurrentDictionary{TKey,TValue}" /> of .NET Framework exposes no comparer at all.
	/// </remarks>
	private static TComparer? ReadKeyComparer<TComparer>(object dictionary)
		where TComparer : class
	{
		if (!ReflectionFallback.IsSupported)
		{
			return null;
		}

		for (Type? type = dictionary.GetType(); type is not null; type = type.BaseType)
		{
			if (type.IsGenericType && type.GetGenericTypeDefinition().FullName is { } name &&
			    KeyComparerProperties.TryGetValue(name, out string? propertyName))
			{
				PropertyInfo? property = type.GetProperty(propertyName);
				return property?.PropertyType == typeof(TComparer) ? property.GetValue(dictionary) as TComparer : null;
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
	///     that no reflection is needed, which would not survive trimming. A sorted set considers two items the same
	///     when its comparer orders neither before the other.
	/// </remarks>
	public static SubjectComparer<T>? GetSubjectComparer<T>(object? collection)
		=> collection switch
		{
			HashSet<T> set when IsCustom(set.Comparer) => new SubjectComparer<T>(set.Comparer.Equals, set.Comparer),
			SortedSet<T> set when IsCustom(set.Comparer)
				=> new SubjectComparer<T>((x, y) => set.Comparer.Compare(x, y) == 0, set.Comparer),
#if NET8_0_OR_GREATER
			ImmutableHashSet<T> set when IsCustom(set.KeyComparer)
				=> new SubjectComparer<T>(set.KeyComparer.Equals, set.KeyComparer),
			ImmutableSortedSet<T> set when IsCustom(set.KeyComparer)
				=> new SubjectComparer<T>((x, y) => set.KeyComparer.Compare(x, y) == 0, set.KeyComparer),
			FrozenSet<T> set when IsCustom(set.Comparer) => new SubjectComparer<T>(set.Comparer.Equals, set.Comparer),
#endif
			_ => null,
		};

	/// <summary>
	///     Returns the comparer that orders the items of the <paramref name="collection" />, when it is a sorted set
	///     with a comparer other than the default one for <typeparamref name="T" />, or <see langword="null" />
	///     otherwise.
	/// </summary>
	public static IComparer<T>? GetSubjectOrder<T>(object? collection)
		=> collection switch
		{
			SortedSet<T> set when IsCustom(set.Comparer) => set.Comparer,
#if NET8_0_OR_GREATER
			ImmutableSortedSet<T> set when IsCustom(set.KeyComparer) => set.KeyComparer,
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
