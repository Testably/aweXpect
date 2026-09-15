using System.Collections.Immutable;

namespace aweXpect.Generators;

/// <summary>
///     An <see cref="ImmutableArray{T}" /> that compares by content, so that an unchanged pipeline value is
///     recognised as unchanged.
/// </summary>
/// <remarks>
///     <see cref="ImmutableArray{T}" /> compares by reference, which would re-run every step downstream of a
///     collected value on each edit.
/// </remarks>
internal readonly struct EquatableArray<T>(ImmutableArray<T> values) : IEquatable<EquatableArray<T>>
	where T : IEquatable<T>
{
	public ImmutableArray<T> Values { get; } = values.IsDefault ? ImmutableArray<T>.Empty : values;

	public bool Equals(EquatableArray<T> other) => Values.SequenceEqual(other.Values);

	public override bool Equals(object? obj) => obj is EquatableArray<T> other && Equals(other);

	public override int GetHashCode()
	{
		unchecked
		{
			int hash = 17;
			foreach (T value in Values)
			{
				hash = (hash * 31) + value.GetHashCode();
			}

			return hash;
		}
	}
}
