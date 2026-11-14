namespace aweXpect.Snapshots;

/// <summary>
///     A snapshot that matched, see <see cref="SnapshotReporter.OnMatchAsync" />.
/// </summary>
public sealed class SnapshotMatch
{
	/// <summary>
	///     Initializes the match of the snapshot with the <paramref name="id" /> in the <paramref name="store" />, e.g. to
	///     test a <see cref="SnapshotReporter" /> on its own.
	/// </summary>
	public SnapshotMatch(SnapshotId id, SnapshotStore store)
	{
		Id = RuleArguments.ThrowIfNull(id, nameof(id));
		Store = RuleArguments.ThrowIfNull(store, nameof(store));
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
