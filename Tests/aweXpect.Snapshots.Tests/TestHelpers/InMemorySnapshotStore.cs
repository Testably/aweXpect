using System.Collections.Generic;
using System.Linq;
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
	public bool CanList { get; set; } = true;
	public List<SnapshotId> Deleted { get; } = new();
	public Dictionary<SnapshotId, Exception> DeleteExceptions { get; } = new();
	public Exception? ListException { get; set; }
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

	public override ValueTask<IReadOnlyList<SnapshotId>?> ListAsync(CancellationToken cancellationToken)
	{
		if (ListException is not null)
		{
			throw ListException;
		}

		return new ValueTask<IReadOnlyList<SnapshotId>?>(CanList
			? Snapshots.Keys.Union(Mismatches.Keys).ToArray()
			: null);
	}

	public override ValueTask DeleteAsync(SnapshotId id, CancellationToken cancellationToken)
	{
		if (DeleteExceptions.TryGetValue(id, out Exception? exception))
		{
			throw exception;
		}

		Deleted.Add(id);
		Snapshots.Remove(id);
		Mismatches.Remove(id);
		return default;
	}
}
