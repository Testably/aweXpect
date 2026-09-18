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
	private readonly int _limit;

	public LimitedCollection(int? limit = null)
	{
		_limit = limit ?? Customize.aweXpect.Formatting().MaximumNumberOfCollectionItems.Get() + 1;
		_buffer = new List<T>(_limit);
	}

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

	/// <inheritdoc cref="ICollection{T}.Count" />
	public int Count
		=> _buffer.Count;

	/// <inheritdoc cref="ICollection{T}.IsReadOnly" />
	public bool IsReadOnly
		=> _buffer.Count >= _limit;
}
