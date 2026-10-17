using System;
using System.Globalization;
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
///     <see cref="SourceRoots" />.<br />
///     The method name and the name of the snapshot are escaped in the file name, see
///     <see cref="EscapeFileNamePart(string)" />.
/// </remarks>
internal sealed class DiskSnapshotStore : SnapshotStore
{
	public static readonly DiskSnapshotStore Instance = new();
	private static readonly Encoding Utf8WithoutBom = new UTF8Encoding(false);

	private DiskSnapshotStore()
	{
	}

	/// <remarks>
	///     The test file is resolved before the snapshot is read, so that an ambiguous source root is reported instead of
	///     reading the snapshot next to another test file with the same relative path.
	/// </remarks>
	public override ValueTask<string?> ReadAsync(SnapshotId id, CancellationToken cancellationToken)
	{
		SourceRoots.ThrowIfUnresolvable(id.SourceFile);
		string snapshotPath = GetSnapshotPath(id);
		if (!File.Exists(snapshotPath))
		{
			return new ValueTask<string?>((string?)null);
		}

		return new ValueTask<string?>(RemoveFinalLineBreak(File.ReadAllText(snapshotPath)));
	}

	public override ValueTask RecordMismatchAsync(SnapshotId id, string actual, CancellationToken cancellationToken)
	{
		File.WriteAllText(GetPendingPath(id), actual + Environment.NewLine, Utf8WithoutBom);
		return default;
	}

	/// <remarks>
	///     The snapshot is written like a pending file, so that the next run reads the <paramref name="actual" /> value
	///     unchanged. The accepted snapshot makes a leftover pending file obsolete, so one that cannot be deleted does not
	///     fail it; the next matching run deletes it.
	/// </remarks>
	public override ValueTask AcceptAsync(SnapshotId id, string actual, CancellationToken cancellationToken)
	{
		File.WriteAllText(GetSnapshotPath(id), actual + Environment.NewLine, Utf8WithoutBom);
		try
		{
			DeletePending(id);
		}
		catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
		{
			// The pending file is only a leftover, see the remarks.
		}

		return default;
	}

	public override ValueTask RecordMatchAsync(SnapshotId id, CancellationToken cancellationToken)
	{
		DeletePending(id);
		return default;
	}

	private static void DeletePending(SnapshotId id)
	{
		string pendingPath = GetPendingPath(id);
		if (File.Exists(pendingPath))
		{
			File.Delete(pendingPath);
		}
	}

	public override string Describe(SnapshotId id) => GetSnapshotPath(id);

	public override string DescribeMismatch(SnapshotId id) => GetPendingPath(id);

	private static string GetSnapshotPath(SnapshotId id) => GetBasePath(id) + ".snapshot.txt";

	private static string GetPendingPath(SnapshotId id) => GetBasePath(id) + ".snapshot.pending.txt";

	private static string GetBasePath(SnapshotId id)
	{
		string testFilePath = SourceRoots.GetPath(id.SourceFile);
		return Path.Combine(Path.GetDirectoryName(testFilePath) ?? "", id.Name is null
			? $"{Path.GetFileNameWithoutExtension(testFilePath)}.{EscapeFileNamePart(id.MemberName)}"
			: $"{Path.GetFileNameWithoutExtension(testFilePath)}.{EscapeFileNamePart(id.MemberName)}.{EscapeFileNamePart(id.Name)}");
	}

	/// <summary>
	///     Escapes each character as <c>%XX</c> that a file name cannot hold on every operating system
	///     (<c>&lt; &gt; : " / \ | ? *</c> and control characters), as the snapshots are committed and checked out on other
	///     machines, as well as <c>%</c> and <c>.</c>.
	/// </summary>
	/// <remarks>
	///     Escaping <c>%</c> keeps the file names of two different names apart, e.g. <c>a:b</c> and <c>a%3Ab</c>, and
	///     escaping <c>.</c> keeps the dots in <c>&lt;Class&gt;.&lt;Method&gt;.&lt;Name&gt;</c> as the only separators,
	///     so that a file name can be read back into its parts. A name without such characters is used unchanged.
	/// </remarks>
	private static string EscapeFileNamePart(string part)
	{
		StringBuilder? escaped = null;
		for (int i = 0; i < part.Length; i++)
		{
			char character = part[i];
			if (char.IsControl(character) ||
			    character is '<' or '>' or ':' or '"' or '/' or '\\' or '|' or '?' or '*' or '%' or '.')
			{
				escaped ??= new StringBuilder(part, 0, i, part.Length + 8);
				escaped.Append('%').Append(((int)character).ToString("X2", CultureInfo.InvariantCulture));
			}
			else
			{
				escaped?.Append(character);
			}
		}

		return escaped?.ToString() ?? part;
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
