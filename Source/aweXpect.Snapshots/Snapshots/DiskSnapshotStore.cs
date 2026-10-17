using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace aweXpect.Snapshots;

/// <summary>
///     Keeps the snapshot <c>&lt;Class&gt;.&lt;Method&gt;.snapshot.txt</c> next to the test file and records a mismatch
///     in <c>&lt;Class&gt;.&lt;Method&gt;.snapshot.pending.txt</c>, which can be renamed to accept it.
/// </summary>
/// <remarks>
///     The files are encoded as UTF-8 and the pending file ends with an additional line break, which is removed again
///     when the snapshot is read, so that an editor which adds a final line break does not change it.<br />
///     The files are accessed synchronously, as netstandard2.0 lacks the asynchronous methods of <see cref="File" />.<br />
///     A relative <see cref="SnapshotId.SourceFile" /> is resolved against the recorded source root, see
///     <see cref="SourceRoots" />.
/// </remarks>
internal sealed class DiskSnapshotStore : SnapshotStore
{
	public static readonly DiskSnapshotStore Instance = new();
	private static readonly Encoding Utf8WithoutBom = new UTF8Encoding(false);

	private DiskSnapshotStore()
	{
	}

	public override ValueTask<string?> ReadAsync(SnapshotId id, CancellationToken cancellationToken)
	{
		string snapshotPath = GetSnapshotPath(id);
		if (!File.Exists(snapshotPath))
		{
			SourceRoots.ThrowIfDeterministic(id.SourceFile);
			return new ValueTask<string?>((string?)null);
		}

		return new ValueTask<string?>(RemoveFinalLineBreak(File.ReadAllText(snapshotPath)));
	}

	public override ValueTask RecordMismatchAsync(SnapshotId id, string actual, CancellationToken cancellationToken)
	{
		File.WriteAllText(GetPendingPath(id), actual + Environment.NewLine, Utf8WithoutBom);
		return default;
	}

	public override ValueTask RecordMatchAsync(SnapshotId id, CancellationToken cancellationToken)
	{
		string pendingPath = GetPendingPath(id);
		if (File.Exists(pendingPath))
		{
			File.Delete(pendingPath);
		}

		return default;
	}

	public override string Describe(SnapshotId id) => GetSnapshotPath(id);

	public override string DescribeMismatch(SnapshotId id) => GetPendingPath(id);

	private static string GetSnapshotPath(SnapshotId id) => GetBasePath(id) + ".snapshot.txt";

	private static string GetPendingPath(SnapshotId id) => GetBasePath(id) + ".snapshot.pending.txt";

	private static string GetBasePath(SnapshotId id)
	{
		string testFilePath = SourceRoots.GetPath(id.SourceFile);
		return Path.Combine(Path.GetDirectoryName(testFilePath) ?? "", id.Name is null
			? $"{Path.GetFileNameWithoutExtension(testFilePath)}.{id.MemberName}"
			: $"{Path.GetFileNameWithoutExtension(testFilePath)}.{id.MemberName}.{id.Name}");
	}

	private static string RemoveFinalLineBreak(string snapshot)
	{
		if (snapshot.EndsWith("\r\n", StringComparison.Ordinal))
		{
			return snapshot.Substring(0, snapshot.Length - 2);
		}

		return snapshot.Length > 0 && snapshot[snapshot.Length - 1] == '\n'
			? snapshot.Substring(0, snapshot.Length - 1)
			: snapshot;
	}
}
