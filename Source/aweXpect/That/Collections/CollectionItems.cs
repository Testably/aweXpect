using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using aweXpect.Core.EvaluationContext;
using aweXpect.Helpers;

namespace aweXpect;

/// <summary>
///     The materialized items of a collection subject, which are read with their item type when the subject has it, and
///     as untyped items otherwise, e.g. for a non-generic <see cref="IEnumerable" />.
/// </summary>
/// <remarks>
///     An untyped subject keeps the untyped materialization, which other constraints on the same subject share, and its
///     "Collection" context, which is laid out by the type of the listed items.
/// </remarks>
internal readonly struct CollectionItems<TItem>
{
	private readonly IEnumerable<TItem>? _typed;
	private readonly IEnumerable? _untyped;

	private CollectionItems(IEnumerable<TItem>? typed, IEnumerable? untyped)
	{
		_typed = typed;
		_untyped = untyped;
	}

	/// <summary>
	///     The items, read with the item type.
	/// </summary>
	public IEnumerable<TItem> Items => _typed ?? _untyped!.Cast<TItem>();

	/// <summary>
	///     The materialized collection, e.g. for the result text.
	/// </summary>
	public object Value => _typed ?? (object)_untyped!;

	/// <summary>
	///     Enumerates the <see cref="Items" />.
	/// </summary>
	/// <remarks>
	///     An array or a <see cref="List{T}" /> is enumerated without allocating an enumerator, as most subjects are one.
	/// </remarks>
	public Enumerator GetEnumerator() => new(Items);

	/// <summary>
	///     The items of the <paramref name="actual" /> subject without materializing them, e.g. for a collection that
	///     knows its number of items.
	/// </summary>
	public static CollectionItems<TItem> Of<TEnumerable>(TEnumerable actual)
		where TEnumerable : IEnumerable?
		=> IsTypedSubject<TEnumerable>.Value
			? new CollectionItems<TItem>((IEnumerable<TItem>)actual!, null)
			: new CollectionItems<TItem>(null, actual);

	/// <summary>
	///     Materializes the items of the <paramref name="actual" /> subject in the <paramref name="context" />.
	/// </summary>
	public static CollectionItems<TItem> Materialize<TEnumerable>(TEnumerable actual, IEvaluationContext context)
		where TEnumerable : IEnumerable?
		=> IsTypedSubject<TEnumerable>.Value
			? new CollectionItems<TItem>(context.UseMaterializedEnumerable((IEnumerable<TItem>)actual!), null)
			: new CollectionItems<TItem>(null, context.UseMaterializedEnumerable(actual));

	/// <summary>
	///     The number of items of the <paramref name="actual" /> subject, when it is a collection that knows it without
	///     being enumerated.
	/// </summary>
	public static int? CountOf<TEnumerable>(TEnumerable actual)
		where TEnumerable : IEnumerable?
		=> IsTypedSubject<TEnumerable>.Value
			? (actual as ICollection<TItem>)?.Count
			: (actual as ICollection)?.Count;

	/// <summary>
	///     Casts the <paramref name="item" /> to <typeparamref name="TMatch" />, which can fail for an untyped item.
	/// </summary>
	/// <remarks>
	///     A <see langword="null" /> item is not matched by a type pattern, but is a valid value whenever
	///     <typeparamref name="TMatch" /> admits it.
	/// </remarks>
	public static bool TryCast<TMatch>(TItem item, out TMatch matched)
	{
		if (item is TMatch typed)
		{
			matched = typed;
			return true;
		}

		matched = default!;
		return item is null && default(TMatch) is null;
	}

	/// <summary>
	///     Whether a subject of type <typeparamref name="TEnumerable" /> is read with the item type.
	/// </summary>
	public static bool IsTyped<TEnumerable>()
		=> IsTypedSubject<TEnumerable>.Value;

	/// <summary>
	///     Whether the <paramref name="cancellationToken" /> is canceled before all items are available.
	/// </summary>
	/// <remarks>
	///     It is checked for every item, so the common case of a token that is not canceled stays inlined.
	/// </remarks>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool IsCanceledBeforeTheEnd(CancellationToken cancellationToken)
		=> cancellationToken.IsCancellationRequested && IsIncomplete();

	private bool IsIncomplete()
		=> _typed is not null
			? _typed is not (ICollection<TItem> or ICountable { Count: not null, })
			: _untyped is not (ICollection or ICountable { Count: not null, });

	/// <summary>
	///     Keeps the items for the "Collection" <paramref name="context" />.
	/// </summary>
	public void SetContext(ref CollectionContext context, bool isIncomplete = false)
	{
		if (_typed is not null)
		{
			context.Set(_typed, isIncomplete);
		}
		else
		{
			context.Set(_untyped, isIncomplete);
		}
	}

	/// <summary>
	///     The enumerator of <see cref="CollectionItems{TItem}" />.
	/// </summary>
	/// <remarks>
	///     A <see cref="List{T}" /> keeps its own enumerator, so that a modification during the enumeration still throws.
	/// </remarks>
	public struct Enumerator : IDisposable
	{
		private readonly TItem[]? _array;
		private readonly IEnumerator<TItem>? _enumerator;
		private readonly bool _isList;
		private List<TItem>.Enumerator _listEnumerator;
		private int _index;

		internal Enumerator(IEnumerable<TItem> items)
		{
			_array = null;
			_enumerator = null;
			_isList = false;
			_listEnumerator = default;
			_index = -1;
			Current = default!;
			if (items is TItem[] array)
			{
				_array = array;
			}
			else if (items is List<TItem> list)
			{
				_isList = true;
				_listEnumerator = list.GetEnumerator();
			}
			else
			{
				_enumerator = items.GetEnumerator();
			}
		}

		/// <inheritdoc cref="IEnumerator{T}.Current" />
		public TItem Current { get; private set; }

		/// <inheritdoc cref="IEnumerator.MoveNext()" />
		public bool MoveNext()
		{
			if (_array is not null)
			{
				if (++_index >= _array.Length)
				{
					return false;
				}

				Current = _array[_index];
				return true;
			}

			if (_isList)
			{
				if (!_listEnumerator.MoveNext())
				{
					return false;
				}

				Current = _listEnumerator.Current;
				return true;
			}

			if (!_enumerator!.MoveNext())
			{
				return false;
			}

			Current = _enumerator.Current;
			return true;
		}

		/// <inheritdoc cref="IDisposable.Dispose()" />
		public void Dispose() => _enumerator?.Dispose();
	}

	/// <remarks>
	///     Untyped items are read as <see langword="object" />, which every collection of a reference type converts to by
	///     variance, e.g. an <c>ImmutableArray&lt;string&gt;</c>, although the other collection interfaces, e.g.
	///     <see cref="ICollection{T}" />, are invariant. So only a subject of exactly <c>IEnumerable&lt;object&gt;</c>
	///     reads <see langword="object" /> items typed.
	/// </remarks>
	private static class IsTypedSubject<TEnumerable>
	{
		public static readonly bool Value = typeof(TEnumerable) == typeof(IEnumerable<TItem>) ||
		                                    (typeof(TItem) != typeof(object) &&
		                                     typeof(IEnumerable<TItem>).IsAssignableFrom(typeof(TEnumerable)));
	}
}
