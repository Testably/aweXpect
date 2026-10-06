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
	private readonly List<T> _buffer = new();
	private readonly int _limit;

	/// <summary>
	///     Buffers up to the <paramref name="limit" />, by default one more than the maximum number of collection items,
	///     so that exceeding the maximum can be detected.
	/// </summary>
	/// <remarks>
	///     The lists grow with the items and the default saturates, because the maximum can be
	///     <see cref="int.MaxValue" />.
	/// </remarks>
	public LimitedCollection(int? limit = null)
	{
		if (limit is null)
		{
			int maximum = Customize.aweXpect.Formatting().MaximumNumberOfCollectionItems.Get();
			limit = maximum == int.MaxValue ? maximum : maximum + 1;
		}

		_limit = limit.Value;
	}

	/// <inheritdoc cref="ICollection{T}.Count" />
	public int Count
		=> _buffer.Count;

	/// <inheritdoc cref="ICollection{T}.IsReadOnly" />
	public bool IsReadOnly
		=> _buffer.Count >= _limit;

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
}
