using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
#if NET8_0_OR_GREATER
using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.Collections.Immutable;
#endif
#if NET10_0_OR_GREATER
using System.Runtime.CompilerServices;
#else
using System.Reflection;
#endif

namespace aweXpect.Core;

/// <summary>
///     Reads the key comparer of a dictionary, so that expectations treat its keys as the same exactly when the
///     dictionary does.
/// </summary>
/// <remarks>
///     The comparer is read through known dictionary types instead of reflection, so that it stays safe for trimming
///     and Native AOT. A <see cref="ReadOnlyDictionary{TKey,TValue}" /> is asked for the dictionary it wraps, which
///     only exposes it to derived types: from .NET 10 through an unsafe accessor, and on older targets by reflection,
///     which is only attempted while the <see cref="ReflectionFallback" /> is supported. netstandard2.0 lacks some of
///     the dictionary types or their comparer, so they are matched by name there, again only while the
///     <see cref="ReflectionFallback" /> is supported. Any other dictionary is treated as using the default equality.
/// </remarks>
public static class KeyComparers
{
	/// <summary>
	///     Creates an empty set of keys that treats keys as the same exactly when the key comparer of the
	///     <paramref name="dictionary" /> does.
	/// </summary>
	/// <remarks>
	///     A sorted dictionary considers two keys the same when its comparer orders neither before the other, so its
	///     keys are collected in a <see cref="SortedSet{T}" /> with that comparer.
	/// </remarks>
	public static ISet<TKey> CreateKeySet<TKey, TValue>(IEnumerable<KeyValuePair<TKey, TValue>> dictionary)
	{
		if (!TryRead(dictionary, out IEqualityComparer<TKey>? equalityComparer, out IComparer<TKey>? comparer))
		{
			return new HashSet<TKey>();
		}

		return equalityComparer is not null
			? new HashSet<TKey>(equalityComparer)
			: new SortedSet<TKey>(comparer);
	}

	/// <summary>
	///     Reads either the <paramref name="equalityComparer" /> or the <paramref name="comparer" /> that decides which
	///     keys of the <paramref name="dictionary" /> are the same.
	/// </summary>
	/// <remarks>
	///     CS8714 is suppressed, because the <see langword="notnull" /> constraint of these types only concerns the
	///     annotation of <typeparamref name="TKey" />, which a type test at runtime ignores.
	/// </remarks>
#pragma warning disable CS8714
	internal static bool TryRead<TKey, TValue>(IEnumerable<KeyValuePair<TKey, TValue>> dictionary,
		out IEqualityComparer<TKey>? equalityComparer, out IComparer<TKey>? comparer)
	{
		equalityComparer = null;
		comparer = null;
		switch (dictionary)
		{
			case Dictionary<TKey, TValue> d:
				equalityComparer = d.Comparer;
				return true;
			case SortedDictionary<TKey, TValue> d:
				comparer = d.Comparer;
				return true;
			case SortedList<TKey, TValue> d:
				comparer = d.Comparer;
				return true;
			case ReadOnlyDictionary<TKey, TValue> d:
				return GetWrappedDictionary(d) is { } inner &&
				       TryRead(inner, out equalityComparer, out comparer);
#if NET8_0_OR_GREATER
			case ConcurrentDictionary<TKey, TValue> d:
				equalityComparer = d.Comparer;
				return true;
			case ImmutableDictionary<TKey, TValue> d:
				equalityComparer = d.KeyComparer;
				return true;
			case ImmutableSortedDictionary<TKey, TValue> d:
				comparer = d.KeyComparer;
				return true;
			case FrozenDictionary<TKey, TValue> d:
				equalityComparer = d.Comparer;
				return true;
#endif
		}

#if !NET8_0_OR_GREATER
		equalityComparer = ReadComparer<IEqualityComparer<TKey>>(dictionary);
		comparer = equalityComparer is null ? ReadComparer<IComparer<TKey>>(dictionary) : null;
		return equalityComparer is not null || comparer is not null;
#else
		return false;
#endif
	}

#if NET10_0_OR_GREATER
	private static IDictionary<TKey, TValue> GetWrappedDictionary<TKey, TValue>(
		ReadOnlyDictionary<TKey, TValue> dictionary)
		=> ReadOnlyDictionaryAccessor<TKey, TValue>.GetDictionary(dictionary);

	private static class ReadOnlyDictionaryAccessor<TKey, TValue>
		where TKey : notnull
	{
		[UnsafeAccessor(UnsafeAccessorKind.Method, Name = "get_Dictionary")]
		public static extern IDictionary<TKey, TValue> GetDictionary(ReadOnlyDictionary<TKey, TValue> dictionary);
	}
#else
	private static IDictionary<TKey, TValue>? GetWrappedDictionary<TKey, TValue>(
		ReadOnlyDictionary<TKey, TValue> dictionary)
		=> ReflectionFallback.IsSupported
			? typeof(ReadOnlyDictionary<TKey, TValue>)
				.GetProperty("Dictionary", BindingFlags.Instance | BindingFlags.NonPublic)
				?.GetValue(dictionary) as IDictionary<TKey, TValue>
			: null;
#endif
#pragma warning restore CS8714

#if !NET8_0_OR_GREATER
	private static readonly Dictionary<string, string> ComparerProperties = new()
	{
		["System.Collections.Concurrent.ConcurrentDictionary`2"] = "Comparer",
		["System.Collections.Immutable.ImmutableDictionary`2"] = "KeyComparer",
		["System.Collections.Immutable.ImmutableSortedDictionary`2"] = "KeyComparer",
		["System.Collections.Frozen.FrozenDictionary`2"] = "Comparer",
	};

	/// <summary>
	///     Reads the comparer of a <paramref name="dictionary" /> of one of the types in
	///     <see cref="ComparerProperties" />, which netstandard2.0 lacks or which expose it only on newer runtimes, when
	///     it is a <typeparamref name="TComparer" />.
	/// </summary>
	/// <remarks>
	///     The <see cref="System.Collections.Concurrent.ConcurrentDictionary{TKey,TValue}" /> of .NET Framework exposes
	///     no comparer at all.
	/// </remarks>
	private static TComparer? ReadComparer<TComparer>(object dictionary)
		where TComparer : class
	{
		if (!ReflectionFallback.IsSupported)
		{
			return null;
		}

		for (Type? type = dictionary.GetType(); type is not null; type = type.BaseType)
		{
			if (type.IsGenericType && type.GetGenericTypeDefinition().FullName is { } name &&
			    ComparerProperties.TryGetValue(name, out string? propertyName))
			{
				PropertyInfo? property = type.GetProperty(propertyName);
				return property?.PropertyType == typeof(TComparer) ? property.GetValue(dictionary) as TComparer : null;
			}
		}

		return null;
	}
#endif
}
