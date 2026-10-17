using System;
using aweXpect.Core;

namespace aweXpect.Snapshots;

/// <summary>
///     The result of a <see cref="SnapshotComparer" />.
/// </summary>
public sealed class SnapshotComparison
{
	private SnapshotComparison(string? description)
	{
		Description = description;
	}

	/// <summary>
	///     The value matches the snapshot, which is kept unchanged.
	/// </summary>
	public static SnapshotComparison Equal { get; } = new(null);

	/// <summary>
	///     Why the value differs from the snapshot, or <see langword="null" /> when it is <see cref="IsEqual" />.
	/// </summary>
	public string? Description { get; }

	/// <summary>
	///     Whether the value matches the snapshot.
	/// </summary>
	public bool IsEqual => Description is null;

	/// <summary>
	///     The value differs from the snapshot, as the <paramref name="description" /> explains, e.g.
	///     <c>2.3 % of the pixels differ</c>.
	/// </summary>
	/// <remarks>
	///     The failure shows the <paramref name="description" /> after <c>but it differed: </c>.
	/// </remarks>
	public static SnapshotComparison Different(string description)
	{
		if (description is null)
		{
			throw Tracing.WriteException(
				new ArgumentNullException(nameof(description), "The 'description' cannot be null."));
		}

		if (string.IsNullOrWhiteSpace(description))
		{
			throw Tracing.WriteException(
				new ArgumentException("The 'description' cannot be empty.", nameof(description)));
		}

		return new SnapshotComparison(description);
	}
}
