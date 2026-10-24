#if NET8_0_OR_GREATER
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace aweXpect.Core;

/// <summary>
///     Wraps a span by providing access to the underlying data.
/// </summary>
public class SpanWrapper<T> : ICollection<T>
{
	private readonly T[] _value;

	/// <inheritdoc cref="SpanWrapper{T}" />
	public SpanWrapper(ReadOnlySpan<T> span)
	{
		_value = span.ToArray();
	}

	/// <inheritdoc cref="SpanWrapper{T}" />
	public SpanWrapper(Span<T> span)
	{
		_value = span.ToArray();
	}

	/// <summary>
	///     Returns a read-only span over the copy of the data that was taken when the expectation was created.
	/// </summary>
	public ReadOnlySpan<T> AsSpan() => _value.AsSpan();

	/// <inheritdoc cref="IEnumerable{T}.GetEnumerator()" />
	public IEnumerator<T> GetEnumerator()
		=> (_value as IEnumerable<T>).GetEnumerator();

	/// <inheritdoc cref="IEnumerable.GetEnumerator()" />
	IEnumerator IEnumerable.GetEnumerator()
		=> GetEnumerator();

	/// <inheritdoc cref="ICollection{T}.Add(T)" />
	void ICollection<T>.Add(T item)
		=> throw Tracing.WriteException(new NotSupportedException("You may not change a SpanWrapper."));

	/// <inheritdoc cref="ICollection{T}.Clear()" />
	void ICollection<T>.Clear()
		=> throw Tracing.WriteException(new NotSupportedException("You may not change a SpanWrapper."));

	/// <inheritdoc cref="ICollection{T}.Contains(T)" />
	public bool Contains(T item)
		=> _value.Contains(item);

	/// <inheritdoc cref="ICollection{T}.CopyTo(T[], int)" />
	public void CopyTo(T[] array, int arrayIndex)
		=> _value.CopyTo(array, arrayIndex);

	/// <inheritdoc cref="ICollection{T}.Remove(T)" />
	bool ICollection<T>.Remove(T item)
		=> throw Tracing.WriteException(new NotSupportedException("You may not change a SpanWrapper."));

	/// <inheritdoc cref="ICollection{T}.Count" />
	public int Count
		=> _value.Length;

	/// <inheritdoc cref="ICollection{T}.IsReadOnly" />
	public bool IsReadOnly
		=> true;
}
#endif
