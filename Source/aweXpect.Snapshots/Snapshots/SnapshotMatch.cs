namespace aweXpect.Snapshots;

/// <summary>
///     A snapshot that matched, see <see cref="SnapshotReporter.OnMatchAsync" />.
/// </summary>
public sealed class SnapshotMatch
{
	internal SnapshotMatch(SnapshotId id, SnapshotStore store)
	{
		Id = id;
		Store = store;
	}

	/// <summary>
	///     The id of the snapshot.
	/// </summary>
	public SnapshotId Id { get; }

	/// <summary>
	///     The store that keeps the snapshot.
	/// </summary>
	/// <remarks>
	///     A store that keeps the snapshots in files implements <see cref="IFileSnapshotStore" />, which the default
	///     store does.
	/// </remarks>
	public SnapshotStore Store { get; }
}
