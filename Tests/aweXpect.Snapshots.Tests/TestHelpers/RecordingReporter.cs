using System.Collections.Generic;
using System.Threading;

namespace aweXpect.Snapshots.Tests.TestHelpers;

/// <summary>
///     A <see cref="SnapshotReporter" /> that records every call, optionally in a <paramref name="log" /> shared with
///     other reporters.
/// </summary>
internal sealed class RecordingReporter(string name = "", List<string>? log = null) : SnapshotReporter
{
	public Exception? Exception { get; set; }
	public List<SnapshotMatch> Matches { get; } = new();
	public List<SnapshotMismatch> Mismatches { get; } = new();
	public Action? OnCall { get; set; }

	public override ValueTask OnMatchAsync(SnapshotMatch match, CancellationToken cancellationToken)
	{
		Matches.Add(match);
		log?.Add($"{name}: match");
		OnCall?.Invoke();
		return Exception is null ? default : throw Exception;
	}

	public override ValueTask OnMismatchAsync(SnapshotMismatch mismatch, CancellationToken cancellationToken)
	{
		Mismatches.Add(mismatch);
		log?.Add($"{name}: mismatch");
		OnCall?.Invoke();
		return Exception is null ? default : throw Exception;
	}
}
