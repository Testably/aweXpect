using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
#if NET8_0_OR_GREATER
using System.Collections.Frozen;
using System.Collections.Immutable;
#endif
using System.Reflection;
using aweXpect.Core;
using aweXpect.Core.Metadata;

namespace aweXpect.Equivalency;

/// <summary>
///     Reads the comparer of a set whose item type is only known at runtime, as a comparison of items of type
///     <see cref="object" />.
/// </summary>
internal abstract class SetItemComparer
{
	/// <summary>
	///     Returns whether two items are the same for the comparer of the <paramref name="set" />, or
	///     <see langword="null" /> when the set uses the default equality or its comparer cannot be read.
	/// </summary>
	public abstract Func<object?, object?, bool>? Read(object set);

	/// <summary>
	///     Returns the reader for sets that implement the generic <paramref name="setInterface" />, or
	///     <see langword="null" /> when there is none.
	/// </summary>
	/// <remarks>
	///     A reader is registered for the sets the source generator sees. Any other one is only created while the
	///     <see cref="ReflectionFallback" /> is supported, because instantiating it for a type argument that is only
	///     known at runtime needs dynamic code.
	/// </remarks>
	public static SetItemComparer? For(Type setInterface)
	{
		if (TypeMetadataRegistry.Instance.TryGet(setInterface, out TypeMetadataRegistry.TypeMetadata? metadata) &&
		    metadata.ItemComparer is { } registered)
		{
			return registered;
		}

		if (!ReflectionFallback.IsSupported)
		{
			return null;
		}

		if (CreatedReaders.TryGetValue(setInterface, out SetItemComparer? created))
		{
			return created;
		}

		created = (SetItemComparer)typeof(SetItemComparer)
			.GetMethod(nameof(Create), BindingFlags.Static | BindingFlags.Public)!
			.MakeGenericMethod(setInterface.GetGenericArguments())
			.Invoke(null, null)!;
		CreatedReaders.TryAdd(setInterface, created);
		return created;
	}

	/// <remarks>
	///     A reader holds no state, so the one created by reflection is kept per set interface, as every compared set of
	///     an unregistered type would otherwise create it again.
	/// </remarks>
	private static readonly ConcurrentDictionary<Type, SetItemComparer> CreatedReaders = new();

	/// <summary>
	///     Creates the reader for sets with the given type argument.
	/// </summary>
	public static SetItemComparer<T> Create<T>() => new();
}

/// <inheritdoc cref="SetItemComparer" />
internal sealed class SetItemComparer<T> : SetItemComparer
{
	/// <inheritdoc cref="SetItemComparer.Read(object)" />
	/// <remarks>
	///     A <see langword="null" /> item, or one that is not of the item type, is never handed to the comparer, which
	///     would reject it, and is not the same as any other item for it.
	/// </remarks>
	public override Func<object?, object?, bool>? Read(object set)
	{
		IEqualityComparer<T>? comparer = set switch
		{
			HashSet<T> hashSet => hashSet.Comparer,
#if NET8_0_OR_GREATER
			ImmutableHashSet<T> immutableHashSet => immutableHashSet.KeyComparer,
			FrozenSet<T> frozenSet => frozenSet.Comparer,
#endif
			_ => null,
		};
		if (comparer is null || Equals(comparer, EqualityComparer<T>.Default))
		{
			return null;
		}

		return (x, y) => x is T typedX && y is T typedY && comparer.Equals(typedX, typedY);
	}
}
