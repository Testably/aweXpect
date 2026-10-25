using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using aweXpect.Core;
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
	private readonly bool _isMaterialized;
	private readonly IEnumerable<TItem>? _typed;
	private readonly IEnumerable? _untyped;

	private CollectionItems(IEnumerable<TItem>? typed, IEnumerable? untyped, bool isMaterialized = false)
	{
		_typed = typed;
		_untyped = untyped;
		_isMaterialized = isMaterialized;
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
	///     A sequence materialized by <see cref="Materialize{TEnumerable}" /> is read by position for the same reason.
	/// </remarks>
	public Enumerator GetEnumerator() => new(Items, _isMaterialized);

#if NET8_0_OR_GREATER
	/// <summary>
	///     The items of an array or a <see cref="List{T}" />, which can be searched without an enumerator.
	/// </summary>
	/// <remarks>
	///     A <see cref="List{T}" /> detects a modification only in its enumerator, so its items must only be searched
	///     without running code of the caller.
	/// </remarks>
	public bool TryGetSpan(out ReadOnlySpan<TItem> items)
	{
		switch (_typed)
		{
			case TItem[] array:
				items = new ReadOnlySpan<TItem>(array);
				return true;
			case List<TItem> list:
				items = System.Runtime.InteropServices.CollectionsMarshal.AsSpan(list);
				return true;
			default:
				items = default;
				return false;
		}
	}
#endif

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
	/// <remarks>
	///     A collection that knows its number of items is kept as it is, whether it is typed or untyped, so that e.g. a
	///     <see cref="Queue{T}" /> is evaluated like a <see cref="List{T}" />. The <paramref name="context" /> itself
	///     only keeps an <see cref="ICollection{T}" /> as it is.
	/// </remarks>
	public static CollectionItems<TItem> Materialize<TEnumerable>(TEnumerable actual, IEvaluationContext context)
		where TEnumerable : IEnumerable?
	{
		if (IsTypedSubject<TEnumerable>.Value)
		{
			IEnumerable<TItem> typed = (IEnumerable<TItem>)actual!;
			bool isKept = typed is not ICollection<TItem> && KnowsItsCount(typed);
			return isKept
				? new CollectionItems<TItem>(typed, null)
				: new CollectionItems<TItem>(context.UseMaterializedEnumerable(typed), null, true);
		}

		IEnumerable? untyped = actual.GetUntypedCount() is null ? context.UseMaterializedEnumerable(actual) : actual;
		return new CollectionItems<TItem>(null, untyped);
	}

	/// <summary>
	///     The number of items of the <paramref name="actual" /> subject, when it is a collection that knows it without
	///     being enumerated.
	/// </summary>
	/// <remarks>
	///     The collection is asked as code of the caller, so that an exception of its count fails the expectation.
	/// </remarks>
	public static int? CountOf<TEnumerable>(TEnumerable actual)
		where TEnumerable : IEnumerable?
	{
		if (!IsTypedSubject<TEnumerable>.Value)
		{
			return actual.GetUntypedCount();
		}

		return actual switch
		{
			ICollection<TItem> collection => UserCode.Invoke(static subject => subject.Count, collection),
			IReadOnlyCollection<TItem> collection => UserCode.Invoke(static subject => subject.Count, collection),
			ICollection collection => UserCode.Invoke(static subject => subject.Count, collection),
			_ => null,
		};
	}

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

	/// <summary>
	///     Whether items can still have to come from the source.
	/// </summary>
	public bool IsIncomplete()
		=> _typed is not null ? IsIncomplete(_typed) : CollectionHelpers.IsIncomplete(_untyped);

	/// <summary>
	///     Whether the typed <paramref name="items" /> can have further items to come from their source, which neither
	///     a collection that knows its number of items nor a sequence that was read to its end has.
	/// </summary>
	internal static bool IsIncomplete(IEnumerable<TItem> items)
		=> !KnowsItsCount(items) && items is not ICountable { Count: not null, };

	/// <summary>
	///     Whether the typed <paramref name="items" /> are a collection that <see cref="CountOf{TEnumerable}" /> gets
	///     the number of items from.
	/// </summary>
	private static bool KnowsItsCount(IEnumerable<TItem> items)
		=> items is ICollection<TItem> or IReadOnlyCollection<TItem> or ICollection;

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
	///     <br />
	///     The enumerator of any other collection is requested, advanced and read as code of the caller, so that an
	///     exception of a subject that is not materialized fails the expectation as well. The delegates are static and
	///     get the enumerator as their argument, so that guarding an item allocates nothing.
	/// </remarks>
	public struct Enumerator : IDisposable
	{
		private readonly TItem[]? _array;
		private readonly IEnumerator<TItem>? _enumerator;
		private readonly bool _isList;
		private readonly IMaterializedEnumerable<TItem>? _materialized;
		private List<TItem>.Enumerator _listEnumerator;
		private int _index;

		/// <remarks>
		///     Only a sequence that aweXpect <paramref name="isMaterialized" /> itself is read by position, because
		///     <see cref="EvaluationContextExtensions.TryGetItem{TItem}" /> enumerates any other
		///     <see cref="IMaterializedEnumerable{T}" /> again for every item.
		/// </remarks>
		internal Enumerator(IEnumerable<TItem> items, bool isMaterialized)
		{
			_array = null;
			_enumerator = null;
			_isList = false;
			_materialized = null;
			_listEnumerator = default;
			_index = -1;
			Current = default!;
			if (items is TItem[] array)
			{
				_array = array;
			}
			else if (isMaterialized && items is IMaterializedEnumerable<TItem> materialized)
			{
				_materialized = materialized;
			}
			else if (items is List<TItem> list)
			{
				_isList = true;
				_listEnumerator = list.GetEnumerator();
			}
			else
			{
				_enumerator = UserCode.Invoke(static source => source.GetEnumerator(), items);
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

			if (_materialized is not null)
			{
				if (!_materialized.TryGetItem(++_index, out TItem materializedItem))
				{
					return false;
				}

				Current = materializedItem;
				return true;
			}

			(bool hasItem, TItem item) = UserCode.Invoke(
				static enumerator => enumerator.MoveNext() ? (true, enumerator.Current) : (false, default(TItem)!),
				_enumerator!);
			Current = item;
			return hasItem;
		}

		/// <inheritdoc cref="IDisposable.Dispose()" />
		public void Dispose()
		{
			if (_enumerator is not null)
			{
				DisposeIgnoringExceptions(_enumerator);
			}
		}

		/// <remarks>
		///     Kept apart from <see cref="Dispose()" />, which can only be inlined without exception handling.
		/// </remarks>
		private static void DisposeIgnoringExceptions(IEnumerator<TItem> enumerator)
		{
			try
			{
				enumerator.Dispose();
			}
			catch (Exception)
			{
				// The outcome is already decided, so an exception while disposing the enumerator must not replace it.
			}
		}
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
