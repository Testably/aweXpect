using System.IO;
using System.Text;

namespace aweXpect.Snapshots.Tests.TestHelpers;

/// <summary>
///     A temporary directory that stands in for the directory of a test file.
/// </summary>
/// <remarks>
///     Every test gets its own directory, because the test frameworks run in parallel and would otherwise write the
///     same snapshot files next to this test project.
/// </remarks>
internal sealed class SnapshotDirectory : IDisposable
{
	public const string MemberName = "MyTest";

	/// <param name="path">The directory to use instead of a new temporary one.</param>
	public SnapshotDirectory(string? path = null)
	{
		Path = path ?? System.IO.Path.Combine(System.IO.Path.GetTempPath(),
			$"aweXpect.Snapshots.Tests.{Guid.NewGuid():N}");
		Directory.CreateDirectory(Path);
	}

	public string Path { get; }

	public string PendingPath => System.IO.Path.Combine(Path, $"MyTests.{MemberName}.awexpect-snapshot.pending.txt");
	public string SnapshotPath => System.IO.Path.Combine(Path, $"MyTests.{MemberName}.awexpect-snapshot.txt");
	public string TestFilePath => System.IO.Path.Combine(Path, "MyTests.cs");

	public void Dispose() => Directory.Delete(Path, true);

	/// <summary>
	///     The path of the file with the <paramref name="fileName" /> in the directory of a named snapshot of
	///     <see cref="MemberName" />.
	/// </summary>
	public string GetNamedPath(string fileName)
		=> System.IO.Path.Combine(Path, "MyTests", MemberName, fileName);

	public void WritePending(string content) => File.WriteAllText(PendingPath, content, new UTF8Encoding(false));

	public void WriteSnapshot(string content, bool withByteOrderMark = false)
		=> File.WriteAllText(SnapshotPath, content, new UTF8Encoding(withByteOrderMark));
}
