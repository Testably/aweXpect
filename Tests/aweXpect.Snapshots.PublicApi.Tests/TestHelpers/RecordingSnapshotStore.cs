using System.Threading;

namespace aweXpect.Snapshots.PublicApi.Tests.TestHelpers;

/// <summary>
///     A <see cref="SnapshotStore" /> without snapshots, which records the id of the last mismatch.
/// </summary>
internal sealed class RecordingSnapshotStore : SnapshotStore
{
	public SnapshotId? Mismatch { get; private set; }

	public override ValueTask AcceptAsync(SnapshotId id, byte[] actual, CancellationToken cancellationToken)
		=> throw new NotSupportedException();

	public override ValueTask<byte[]?> ReadAsync(SnapshotId id, CancellationToken cancellationToken)
		=> default;

	public override ValueTask RecordMismatchAsync(SnapshotId id, byte[] actual, CancellationToken cancellationToken)
	{
		Mismatch = id;
		return default;
	}
}
