using System;
using System.Collections.Generic;
using System.Reflection;
using aweXpect.Core;
using aweXpect.Core.Metadata;

namespace aweXpect.Equivalency;

/// <summary>
///     Reads the key comparer of a dictionary whose type arguments are only known at runtime, as a comparer of keys of
///     type <see cref="object" />.
/// </summary>
internal abstract class DictionaryKeyComparer
{
	/// <summary>
	///     Returns an <see cref="IEqualityComparer{T}" /> or an <see cref="IComparer{T}" /> of <see cref="object" /> that
	///     decides like the key comparer of the <paramref name="dictionary" />, or <see langword="null" /> when that
	///     comparer cannot be read.
	/// </summary>
	public abstract object? Read(object dictionary);

	/// <summary>
	///     Returns the reader for dictionaries that implement the generic <paramref name="dictionaryInterface" />, or
	///     <see langword="null" /> when there is none.
	/// </summary>
	/// <remarks>
	///     A reader is registered for the dictionaries the source generator sees. Any other one is only created while
	///     the <see cref="ReflectionFallback" /> is supported, because instantiating it for type arguments that are only
	///     known at runtime needs dynamic code.
	/// </remarks>
	public static DictionaryKeyComparer? For(Type dictionaryInterface)
	{
		if (TypeMetadataRegistry.Instance.TryGet(dictionaryInterface, out TypeMetadataRegistry.TypeMetadata? metadata) &&
		    metadata.KeyComparer is { } registered)
		{
			return registered;
		}

		if (!ReflectionFallback.IsSupported)
		{
			return null;
		}

		return (DictionaryKeyComparer?)typeof(DictionaryKeyComparer)
			.GetMethod(nameof(Create), BindingFlags.Static | BindingFlags.Public)!
			.MakeGenericMethod(dictionaryInterface.GetGenericArguments())
			.Invoke(null, null);
	}

	/// <summary>
	///     Creates the reader for dictionaries with the given type arguments.
	/// </summary>
	public static DictionaryKeyComparer<TKey, TValue> Create<TKey, TValue>() => new();
}

/// <inheritdoc cref="DictionaryKeyComparer" />
internal sealed class DictionaryKeyComparer<TKey, TValue> : DictionaryKeyComparer
{
	/// <inheritdoc cref="DictionaryKeyComparer.Read(object)" />
	public override object? Read(object dictionary)
	{
		if (dictionary is not IEnumerable<KeyValuePair<TKey, TValue>> typed ||
		    !KeyComparers.TryRead(typed, out IEqualityComparer<TKey>? equalityComparer, out IComparer<TKey>? comparer))
		{
			return null;
		}

		return equalityComparer is not null
			? new KeyEqualityComparer(equalityComparer)
			: new KeyOrderComparer(comparer!);
	}

	/// <remarks>
	///     A <see langword="null" /> key, or one that is not of the key type, is compared with its own
	///     <see cref="object.Equals(object)" /> and never handed to the comparer, which would reject it.
	/// </remarks>
	private sealed class KeyEqualityComparer(IEqualityComparer<TKey> comparer) : IEqualityComparer<object>
	{
		bool IEqualityComparer<object>.Equals(object? x, object? y)
			=> x is TKey typedX && y is TKey typedY ? comparer.Equals(typedX, typedY) : Equals(x, y);

		int IEqualityComparer<object>.GetHashCode(object obj)
			=> obj is TKey typed ? comparer.GetHashCode(typed) : obj.GetHashCode();
	}

	/// <remarks>
	///     Only orders the keys that the dictionary itself holds or found, which are of its key type and not
	///     <see langword="null" />: an expected key that the dictionary did not find is never looked up in a set that
	///     uses this comparer.
	/// </remarks>
	private sealed class KeyOrderComparer(IComparer<TKey> comparer) : IComparer<object>
	{
		int IComparer<object>.Compare(object? x, object? y) => comparer.Compare((TKey)x!, (TKey)y!);
	}
}
