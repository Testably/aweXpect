using System.Collections;
using System.Collections.Generic;

namespace aweXpect.Tests;

/// <summary>
///     A collection of the <paramref name="items" /> that knows its number of items, whose
///     <paramref name="throwingMembers" /> throw the <paramref name="exception" />.
/// </summary>
/// <remarks>
///     <see cref="ThrowingMembers.Enumeration" /> throws when the enumerator is requested and
///     <see cref="ThrowingMembers.MoveNext" /> when the enumerator is advanced past the last item.
/// </remarks>
internal class ThrowingCollection<T>(Exception exception, ThrowingMembers throwingMembers, params T[] items)
	: ICollection<T>
{
	private int _countReads;

	/// <summary>
	///     How often the <see cref="Count" /> is read successfully, before <see cref="ThrowingMembers.Count" /> throws.
	/// </summary>
	public int CountReadsBeforeThrowing { get; set; }

	public int DisposeCount { get; private set; }

	/// <summary>
	///     The exception of <see cref="ThrowingMembers.Dispose" />, when it differs from the one of the other members.
	/// </summary>
	public Exception? DisposeException { get; set; }

	public int Count
		=> Throws(ThrowingMembers.Count) && _countReads++ >= CountReadsBeforeThrowing
			? throw exception
			: items.Length;

	public bool IsReadOnly => true;

	private Exception Exception => exception;

	private T[] Items => items;

	public IEnumerator<T> GetEnumerator()
		=> Throws(ThrowingMembers.Enumeration) ? throw exception : new Enumerator(this);

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	public void Add(T item) => throw new NotSupportedException();

	public void Clear() => throw new NotSupportedException();

	public bool Contains(T item) => Array.IndexOf(items, item) >= 0;

	public void CopyTo(T[] array, int arrayIndex) => items.CopyTo(array, arrayIndex);

	public bool Remove(T item) => throw new NotSupportedException();

	private bool Throws(ThrowingMembers member)
		=> (throwingMembers & member) != 0;

	private sealed class Enumerator(ThrowingCollection<T> owner) : IEnumerator<T>
	{
		private int _index = -1;

		public T Current
			=> owner.Throws(ThrowingMembers.Current) ? throw owner.Exception : owner.Items[_index];

		object? IEnumerator.Current => Current;

		public bool MoveNext()
		{
			if (++_index < owner.Items.Length)
			{
				return true;
			}

			return owner.Throws(ThrowingMembers.MoveNext) ? throw owner.Exception : false;
		}

		public void Reset() => _index = -1;

		public void Dispose()
		{
			owner.DisposeCount++;
			if (owner.Throws(ThrowingMembers.Dispose))
			{
				throw owner.DisposeException ?? owner.Exception;
			}
		}
	}
}

/// <summary>
///     A <see cref="ThrowingCollection{T}" /> that is a read-only collection as well.
/// </summary>
internal sealed class ThrowingReadOnlyCollection<T>(
	Exception exception,
	ThrowingMembers throwingMembers,
	params T[] items)
	: ThrowingCollection<T>(exception, throwingMembers, items), IReadOnlyCollection<T>;

/// <summary>
///     A non-generic collection of the <paramref name="items" />, whose <paramref name="throwingMembers" /> throw the
///     <paramref name="exception" />.
/// </summary>
internal sealed class ThrowingUntypedCollection(
	Exception exception,
	ThrowingMembers throwingMembers,
	params object?[] items) : ICollection
{
	private readonly ThrowingCollection<object?> _items = new(exception, throwingMembers, items);

	/// <inheritdoc cref="ThrowingCollection{T}.CountReadsBeforeThrowing" />
	public int CountReadsBeforeThrowing
	{
		get => _items.CountReadsBeforeThrowing;
		set => _items.CountReadsBeforeThrowing = value;
	}

	public int Count => _items.Count;

	public bool IsSynchronized => false;

	public object SyncRoot => this;

	public IEnumerator GetEnumerator() => _items.GetEnumerator();

	public void CopyTo(Array array, int index) => throw new NotSupportedException();
}
