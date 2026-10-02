using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace aweXpect.Formatting;

/// <summary>
///     Formatting context used in the <see cref="ValueFormatter" />.
/// </summary>
public class FormattingContext
{
	/// <summary>
	///     Tracks the objects and collections that are being formatted to catch recursions.
	/// </summary>
	public HashSet<object> FormattedObjects { get; } = new(ReferenceComparer.Instance);

	/// <summary>
	///     The number of objects, collections and tuples whose content is being written.
	/// </summary>
	internal int Depth { get; set; }

	/// <summary>
	///     The number of objects, collections and tuples whose content was written so far.
	/// </summary>
	internal int NumberOfWrittenContents { get; set; }

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
