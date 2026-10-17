using System.Text;
using System.Threading;

namespace aweXpect.Snapshots.SourceRoots.Tests.TestHelpers;

/// <summary>
///     A <see cref="SnapshotStore" /> without snapshots, which keeps the last mismatch in memory.
/// </summary>
internal sealed class RecordingSnapshotStore : SnapshotStore
{
	/// <summary>
	///     The text of the last mismatch, without the line break that the library adds at its end.
	/// </summary>
	public string? Mismatch { get; private set; }

	public override ValueTask AcceptAsync(SnapshotId id, byte[] actual, CancellationToken cancellationToken)
		=> throw new NotSupportedException();

	public override ValueTask<byte[]?> ReadAsync(SnapshotId id, CancellationToken cancellationToken)
		=> default;

	public override ValueTask RecordMismatchAsync(SnapshotId id, byte[] actual, CancellationToken cancellationToken)
	{
		string text = Encoding.UTF8.GetString(actual);
		Mismatch = text.Substring(0, text.Length - Environment.NewLine.Length);
		return default;
	}
}
