using System.Collections.Generic;

namespace aweXpect.Snapshots.DiffEngine.Tests.TestHelpers;

/// <summary>
///     An <see cref="IDiffRunner" /> that records the launched and closed diffs instead of starting a diff tool.
/// </summary>
internal sealed class RecordingDiffRunner : IDiffRunner
{
	public List<(string ReceivedFile, string SnapshotFile)> Killed { get; } = new();
	public List<(string ReceivedFile, string SnapshotFile)> Launched { get; } = new();
	public Action<string, string>? OnLaunch { get; set; }

	public bool IsDisabled { get; set; }

	public Task LaunchAsync(string receivedFile, string snapshotFile)
	{
		Launched.Add((receivedFile, snapshotFile));
		OnLaunch?.Invoke(receivedFile, snapshotFile);
		return Task.CompletedTask;
	}

	public void Kill(string receivedFile, string snapshotFile)
		=> Killed.Add((receivedFile, snapshotFile));
}
