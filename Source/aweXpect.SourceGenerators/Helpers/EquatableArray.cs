using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace aweXpect.SourceGenerators.Helpers;

/// <summary>
///     An immutable, equatable array. This is equivalent to <see cref="Array" /> but with value equality support.
/// </summary>
/// <remarks>
///     <see
///         href="https://github.com/andrewlock/blog-examples/blob/master/NetEscapades.EnumGenerators/src/NetEscapades.EnumGenerators/EquatableArray.cs" />
///     ,<br />
///     <see href="https://andrewlock.net/creating-a-source-generator-part-1-creating-an-incremental-source-generator/" />
/// </remarks>
/// <typeparam name="T">The type of values in the array.</typeparam>
[ExcludeFromCodeCoverage]
internal readonly struct EquatableArray<T> : IEquatable<EquatableArray<T>>, IEnumerable<T>
	where T : IEquatable<T>
{
	private readonly T[]? _array;

	/// <see cref="EquatableArray{T}" />
	public EquatableArray(T[] array)
	{
		_array = array;
	}

	/// <summary>
	///     Gets the length of the array, or 0 if the array is null
	/// </summary>
	public int Count => _array?.Length ?? 0;

	/// <summary>
	///     Checks whether two <see cref="EquatableArray{T}" /> values are the same.
	/// </summary>
	public static bool operator ==(EquatableArray<T> left, EquatableArray<T> right)
		=> left.Equals(right);

	/// <summary>
	///     Checks whether two <see cref="EquatableArray{T}" /> values are not the same.
	/// </summary>
	public static bool operator !=(EquatableArray<T> left, EquatableArray<T> right)
		=> !left.Equals(right);

	/// <inheritdoc />
	public bool Equals(EquatableArray<T> array) => AsSpan().SequenceEqual(array.AsSpan());

	/// <inheritdoc />
	public override bool Equals(object? obj) => obj is EquatableArray<T> array && Equals(array);

	/// <inheritdoc />
	public override int GetHashCode()
	{
		if (_array is not { } array)
		{
			return 0;
		}

		int hashCode = 0;
		int multiplier = 1;

		foreach (T item in array)
		{
			hashCode += item.GetHashCode() * multiplier;
			multiplier *= 17;
		}

		return hashCode;
	}

	/// <summary>
	///     Returns a <see cref="ReadOnlySpan{T}" /> wrapping the current items.
	/// </summary>
	public ReadOnlySpan<T> AsSpan() => _array.AsSpan();

	/// <inheritdoc />
	IEnumerator<T> IEnumerable<T>.GetEnumerator() => ((IEnumerable<T>)(_array ?? [])).GetEnumerator();

	/// <inheritdoc />
	IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<T>)(_array ?? [])).GetEnumerator();
}
