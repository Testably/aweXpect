namespace aweXpect.Snapshots;

/// <summary>
///     A missing or different snapshot that was recorded for review, see
///     <see cref="SnapshotReporter.OnMismatchAsync" />.
/// </summary>
public sealed class SnapshotMismatch
{
	internal SnapshotMismatch(SnapshotId id, SnapshotStore store, SnapshotContent? snapshot, SnapshotContent actual)
	{
		Id = id;
		Store = store;
		Snapshot = snapshot;
		Actual = actual;
	}

	/// <summary>
	///     The value that was compared with the snapshot and recorded, after the rules of the expectation were applied,
	///     e.g. with masked GUIDs.
	/// </summary>
	public SnapshotContent Actual { get; }

	/// <summary>
	///     The id of the snapshot.
	/// </summary>
	public SnapshotId Id { get; }

	/// <summary>
	///     The reviewed snapshot, or <see langword="null" /> when it did not exist.
	/// </summary>
	public SnapshotContent? Snapshot { get; }

	/// <summary>
	///     The store that keeps the snapshot and recorded the mismatch.
	/// </summary>
	/// <remarks>
	///     A store that keeps the snapshots in files implements <see cref="IFileSnapshotStore" />, which the default
	///     store does.
	/// </remarks>
	public SnapshotStore Store { get; }
}
