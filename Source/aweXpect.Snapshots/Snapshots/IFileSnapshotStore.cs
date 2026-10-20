namespace aweXpect.Snapshots;

/// <summary>
///     A <see cref="SnapshotStore" /> that keeps the snapshots in files, e.g. so that a <see cref="SnapshotReporter" />
///     can open them in a diff tool.
/// </summary>
/// <remarks>
///     The default store, which keeps every snapshot next to its test file, implements it.
/// </remarks>
public interface IFileSnapshotStore
{
	/// <summary>
	///     Returns the path of the file of the reviewed snapshot with the <paramref name="id" />, which need not exist.
	/// </summary>
	string GetSnapshotFile(SnapshotId id);

	/// <summary>
	///     Returns the path of the file in which a mismatch of the snapshot with the <paramref name="id" /> is recorded,
	///     which need not exist.
	/// </summary>
	string GetPendingFile(SnapshotId id);
}
