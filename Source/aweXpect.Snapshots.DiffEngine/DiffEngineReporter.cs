using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace aweXpect.Snapshots;

/// <summary>
///     Opens a diff tool with <see href="https://github.com/VerifyTests/DiffEngine">DiffEngine</see> for a snapshot that
///     is missing or different, and closes it again once the snapshot matches.
/// </summary>
/// <remarks>
///     For a store that keeps the snapshots in files (<see cref="IFileSnapshotStore" />), e.g. the default store, the
///     diff tool compares the pending file with the snapshot file, so that changing the snapshot in the diff tool changes
///     the snapshot. DiffEngine creates an empty snapshot file when the snapshot is missing, as most diff tools need
///     both files.<br />
///     For any other store, the value and the snapshot are written to temporary files in
///     <c>&lt;temp&gt;/aweXpect.Snapshots</c> for a review only, which are deleted again once the snapshot matches.<br />
///     Referencing the package registers it for every snapshot in a project for .NET 5 or later, unless the MSBuild
///     property <c>AweXpectSnapshotsDiffEngine</c> is <c>false</c>; otherwise add it to
///     <c>Customize.aweXpect.Snapshots().Reporters</c>. It respects the opt-outs of DiffEngine, e.g. the
///     environment variable <c>DiffEngine_Disabled</c>, and like every reporter it is never called on a build server.
/// </remarks>
public sealed class DiffEngineReporter : SnapshotReporter
{
	private readonly IDiffRunner _diffRunner;

	/// <summary>
	///     Opens a diff tool with DiffEngine for a snapshot that is missing or different.
	/// </summary>
	public DiffEngineReporter() : this(DiffEngineRunner.Instance)
	{
	}

	internal DiffEngineReporter(IDiffRunner diffRunner)
	{
		_diffRunner = diffRunner;
	}

	/// <summary>
	///     The directory of the temporary files for a store that does not keep the snapshots in files.
	/// </summary>
	internal static string TemporaryDirectory { get; } = Path.Combine(Path.GetTempPath(), "aweXpect.Snapshots");

	/// <inheritdoc />
	public override async ValueTask OnMismatchAsync(SnapshotMismatch mismatch, CancellationToken cancellationToken)
	{
		if (_diffRunner.IsDisabled)
		{
			return;
		}

		if (mismatch.Store is IFileSnapshotStore fileStore)
		{
			await _diffRunner.LaunchAsync(fileStore.GetPendingFile(mismatch.Id),
				fileStore.GetSnapshotFile(mismatch.Id));
			return;
		}

		(string receivedFile, string snapshotFile) = GetTemporaryFiles(mismatch.Id);
		Directory.CreateDirectory(TemporaryDirectory);
		File.WriteAllBytes(receivedFile, mismatch.Actual.Bytes);
		if (mismatch.Snapshot is null)
		{
			File.Delete(snapshotFile);
		}
		else
		{
			File.WriteAllBytes(snapshotFile, mismatch.Snapshot.Bytes);
		}

		await _diffRunner.LaunchAsync(receivedFile, snapshotFile);
	}

	/// <remarks>
	///     Only a diff tool for a mismatch that still exists is closed, so that a matching snapshot does not search the
	///     running processes.
	/// </remarks>
	/// <inheritdoc />
	public override ValueTask OnMatchAsync(SnapshotMatch match, CancellationToken cancellationToken)
	{
		if (match.Store is IFileSnapshotStore fileStore)
		{
			string pendingFile = fileStore.GetPendingFile(match.Id);
			if (File.Exists(pendingFile))
			{
				_diffRunner.Kill(pendingFile, fileStore.GetSnapshotFile(match.Id));
			}

			return default;
		}

		(string receivedFile, string snapshotFile) = GetTemporaryFiles(match.Id);
		if (File.Exists(receivedFile))
		{
			_diffRunner.Kill(receivedFile, snapshotFile);
			File.Delete(receivedFile);
			File.Delete(snapshotFile);
		}

		return default;
	}

	/// <summary>
	///     Returns the temporary files <c>&lt;id&gt;.received.&lt;extension&gt;</c> and
	///     <c>&lt;id&gt;.snapshot.&lt;extension&gt;</c> for the snapshot with the <paramref name="id" />.
	/// </summary>
	/// <remarks>
	///     The name contains the directory of the test file, so that test files with the same name in different
	///     directories do not share a file. The extension of the snapshot is kept, so that the diff tool is chosen for it.
	/// </remarks>
	internal static (string ReceivedFile, string SnapshotFile) GetTemporaryFiles(SnapshotId id)
	{
		string sourceFile = id.SourceFile.Replace('\\', '/');
		string directory = sourceFile.Substring(0, sourceFile.LastIndexOf('/') + 1);
		string baseName = Path.Combine(TemporaryDirectory, Escape(directory + id));
		return ($"{baseName}.received.{id.Extension}", $"{baseName}.snapshot.{id.Extension}");
	}

	/// <summary>
	///     Escapes each character as <c>%XX</c> that a file name cannot hold on every operating system, as well as
	///     <c>%</c>, so that two different ids never share a file.
	/// </summary>
	private static string Escape(string name)
	{
		StringBuilder escaped = new(name.Length);
		foreach (char character in name)
		{
			if (char.IsControl(character) ||
			    character is '<' or '>' or ':' or '"' or '/' or '\\' or '|' or '?' or '*' or '%')
			{
				escaped.Append('%').Append(((int)character).ToString("X2", CultureInfo.InvariantCulture));
			}
			else
			{
				escaped.Append(character);
			}
		}

		return escaped.ToString();
	}
}
