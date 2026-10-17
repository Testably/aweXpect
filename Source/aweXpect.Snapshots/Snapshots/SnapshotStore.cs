using System.Threading;
using System.Threading.Tasks;

namespace aweXpect.Snapshots;

/// <summary>
///     Stores the snapshots that <c>MatchesSnapshot()</c> compares with.
/// </summary>
/// <remarks>
///     The default store keeps every snapshot in a file next to its test file. Another store, e.g. one that keeps the
///     snapshots in a database, is set with <c>Customize.aweXpect.Snapshots().Store</c>.<br />
///     The operations describe what happened in the expectation instead of how it is stored, so that a store decides
///     on its own how it keeps a value that awaits review apart from the reviewed snapshot.
/// </remarks>
public abstract class SnapshotStore
{
	/// <summary>
	///     Reads the reviewed snapshot with the <paramref name="id" />.
	/// </summary>
	/// <returns>The snapshot, or <see langword="null" /> when it does not exist.</returns>
	/// <remarks>
	///     An exception fails the expectation and its negation, with the exception as cause.
	/// </remarks>
	public abstract ValueTask<string?> ReadAsync(SnapshotId id, CancellationToken cancellationToken);

	/// <summary>
	///     Records the <paramref name="actual" /> value for the snapshot with the <paramref name="id" />, which was missing
	///     or different, so that it can be reviewed and accepted.
	/// </summary>
	/// <remarks>
	///     The reviewed snapshot must stay unchanged. An exception is named in the failure, which the mismatch causes
	///     anyway.
	/// </remarks>
	public abstract ValueTask RecordMismatchAsync(SnapshotId id, string actual, CancellationToken cancellationToken);

	/// <summary>
	///     Accepts the <paramref name="actual" /> value as the reviewed snapshot with the <paramref name="id" />, which was
	///     missing or different while accepting changes was enabled.
	/// </summary>
	/// <remarks>
	///     It replaces the reviewed snapshot, so a value that an earlier mismatch recorded is obsolete. An exception is
	///     named in the failure, which the expectation reports anyway.
	/// </remarks>
	public abstract ValueTask AcceptAsync(SnapshotId id, string actual, CancellationToken cancellationToken);

	/// <summary>
	///     Records that the snapshot with the <paramref name="id" /> matched, e.g. to remove a value that an earlier
	///     mismatch recorded.
	/// </summary>
	/// <remarks>
	///     An exception is ignored, as the snapshot matched.
	/// </remarks>
	public virtual ValueTask RecordMatchAsync(SnapshotId id, CancellationToken cancellationToken) => default;

	/// <summary>
	///     Describes the snapshot with the <paramref name="id" /> in the failure message.
	/// </summary>
	public virtual string Describe(SnapshotId id) => id.ToString();

	/// <summary>
	///     Describes where a mismatch for the snapshot with the <paramref name="id" /> is recorded in the failure message,
	///     or returns <see langword="null" /> to leave it out.
	/// </summary>
	public virtual string? DescribeMismatch(SnapshotId id) => null;
}
