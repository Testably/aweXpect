using System.Collections;
using System.Collections.Generic;
using aweXpect.Customization;

namespace aweXpect;

/// <summary>
///     Buffers up to a limit of items, but deliberately is no <see cref="ICollection{T}" />: its count is not the
///     number of items that were added, so the formatter must not render it as the remaining item count.
/// </summary>
internal class LimitedCollection<T> : IEnumerable<T>
{
	private readonly List<T> _buffer;
	private readonly List<int> _indices;
	private readonly int _limit;

	public LimitedCollection(int? limit = null)
	{
		_limit = limit ?? Customize.aweXpect.Formatting().MaximumNumberOfCollectionItems.Get() + 1;
		_buffer = new List<T>(_limit);
		_indices = new List<int>(_limit);
	}

	/// <summary>
	///     The positions in the source collection of the items added with <see cref="Add(T, int)" />.
	/// </summary>
	public IEnumerable<int> Indices
		=> _indices;

	/// <inheritdoc cref="IEnumerable{T}.GetEnumerator()" />
	public IEnumerator<T> GetEnumerator()
		=> _buffer.GetEnumerator();

	/// <inheritdoc cref="IEnumerable.GetEnumerator()" />
	IEnumerator IEnumerable.GetEnumerator()
		=> GetEnumerator();

	/// <inheritdoc cref="ICollection{T}.Add(T)" />
	public void Add(T item)
	{
		if (!IsReadOnly)
		{
			_buffer.Add(item);
		}
	}

	/// <summary>
	///     Adds the <paramref name="item" /> found at <paramref name="index" /> in the source collection.
	/// </summary>
	public void Add(T item, int index)
	{
		if (!IsReadOnly)
		{
			_buffer.Add(item);
			_indices.Add(index);
		}
	}

	/// <inheritdoc cref="ICollection{T}.Count" />
	public int Count
		=> _buffer.Count;

	/// <inheritdoc cref="ICollection{T}.IsReadOnly" />
	public bool IsReadOnly
		=> _buffer.Count >= _limit;
}
