using System.Collections.Generic;
using System.Threading;

namespace aweXpect.Snapshots.DiffEngine.Tests.TestHelpers;

/// <summary>
///     A <see cref="SnapshotStore" /> that keeps the snapshots in memory, so it has no files to diff.
/// </summary>
internal sealed class InMemorySnapshotStore : SnapshotStore
{
	public Dictionary<SnapshotId, byte[]> Mismatches { get; } = new();
	public Dictionary<SnapshotId, byte[]> Snapshots { get; } = new();
	public Action? OnRead { get; set; }
	public Action? OnRecordMismatch { get; set; }

	public override ValueTask<byte[]?> ReadAsync(SnapshotId id, CancellationToken cancellationToken)
	{
		OnRead?.Invoke();
		return new ValueTask<byte[]?>(Snapshots.TryGetValue(id, out byte[]? snapshot) ? snapshot : null);
	}

	public override ValueTask RecordMismatchAsync(SnapshotId id, byte[] actual, CancellationToken cancellationToken)
	{
		Mismatches[id] = actual;
		OnRecordMismatch?.Invoke();
		return default;
	}

	public override ValueTask AcceptAsync(SnapshotId id, byte[] actual, CancellationToken cancellationToken)
	{
		Snapshots[id] = actual;
		return default;
	}
}
