using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace aweXpect.Formatting;

/// <summary>
///     Formatting context used in the <see cref="ValueFormatter" />.
/// </summary>
public class FormattingContext
{
	/// <summary>
	///     Tracks already formatted objects to catch recursions.
	/// </summary>
	public HashSet<object> FormattedObjects { get; } = new(ReferenceComparer.Instance);

	/// <remarks>
	///     A recursion is the same instance coming round again, not an equal one, and an anonymous type aggregates its
	///     members into <see cref="object.Equals(object)" /> and <see cref="object.GetHashCode" />, so a member that
	///     refuses either would fail the whole message.
	/// </remarks>
	private sealed class ReferenceComparer : IEqualityComparer<object>
	{
		public static readonly ReferenceComparer Instance = new();

		bool IEqualityComparer<object>.Equals(object? x, object? y) => ReferenceEquals(x, y);

		int IEqualityComparer<object>.GetHashCode(object obj) => RuntimeHelpers.GetHashCode(obj);
	}
}
