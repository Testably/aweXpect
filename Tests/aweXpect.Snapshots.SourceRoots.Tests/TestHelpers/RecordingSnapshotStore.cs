using System.Threading;

namespace aweXpect.Snapshots.SourceRoots.Tests.TestHelpers;

/// <summary>
///     A <see cref="SnapshotStore" /> without snapshots, which keeps the last mismatch in memory.
/// </summary>
internal sealed class RecordingSnapshotStore : SnapshotStore
{
	public string? Mismatch { get; private set; }

	public override ValueTask AcceptAsync(SnapshotId id, string actual, CancellationToken cancellationToken)
		=> throw new NotSupportedException();

	public override ValueTask<string?> ReadAsync(SnapshotId id, CancellationToken cancellationToken)
		=> default;

	public override ValueTask RecordMismatchAsync(SnapshotId id, string actual, CancellationToken cancellationToken)
	{
		Mismatch = actual;
		return default;
	}
}
