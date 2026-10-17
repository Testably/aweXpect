using System.Collections.Generic;
using System.Threading;
using aweXpect.Snapshots;

namespace aweXpect.Snapshots.Tests.TestHelpers;

/// <summary>
///     A <see cref="SnapshotStore" /> that keeps the snapshots in memory and records every call.
/// </summary>
internal sealed class InMemorySnapshotStore : SnapshotStore
{
	public Exception? AcceptException { get; set; }
	public Dictionary<SnapshotId, string> Accepted { get; } = new();
	public List<SnapshotId> Matches { get; } = new();
	public Dictionary<SnapshotId, string> Mismatches { get; } = new();
	public Action? OnRead { get; set; }
	public Exception? ReadException { get; set; }
	public Exception? RecordMatchException { get; set; }
	public Exception? RecordMismatchException { get; set; }
	public Dictionary<SnapshotId, string> Snapshots { get; } = new();

	public override ValueTask AcceptAsync(SnapshotId id, string actual, CancellationToken cancellationToken)
	{
		if (AcceptException is not null)
		{
			throw AcceptException;
		}

		Accepted[id] = actual;
		Snapshots[id] = actual;
		return default;
	}

	public override ValueTask<string?> ReadAsync(SnapshotId id, CancellationToken cancellationToken)
	{
		OnRead?.Invoke();
		if (ReadException is not null)
		{
			throw ReadException;
		}

		return new ValueTask<string?>(Snapshots.TryGetValue(id, out string? snapshot) ? snapshot : null);
	}

	public override ValueTask RecordMatchAsync(SnapshotId id, CancellationToken cancellationToken)
	{
		if (RecordMatchException is not null)
		{
			throw RecordMatchException;
		}

		Matches.Add(id);
		return default;
	}

	public override ValueTask RecordMismatchAsync(SnapshotId id, string actual, CancellationToken cancellationToken)
	{
		if (RecordMismatchException is not null)
		{
			throw RecordMismatchException;
		}

		Mismatches[id] = actual;
		return default;
	}
}
