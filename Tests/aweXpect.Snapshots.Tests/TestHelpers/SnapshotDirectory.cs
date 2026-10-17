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

	public SnapshotDirectory()
	{
		Directory.CreateDirectory(Path);
	}

	public string Path { get; } = System.IO.Path.Combine(System.IO.Path.GetTempPath(),
		$"aweXpect.Snapshots.Tests.{Guid.NewGuid():N}");

	public string PendingPath => System.IO.Path.Combine(Path, $"MyTests.{MemberName}.snapshot.pending.txt");
	public string SnapshotPath => System.IO.Path.Combine(Path, $"MyTests.{MemberName}.snapshot.txt");
	public string TestFilePath => System.IO.Path.Combine(Path, "MyTests.cs");

	public void Dispose() => Directory.Delete(Path, true);

	public void WritePending(string content) => File.WriteAllText(PendingPath, content, new UTF8Encoding(false));

	public void WriteSnapshot(string content, bool withByteOrderMark = false)
		=> File.WriteAllText(SnapshotPath, content, new UTF8Encoding(withByteOrderMark));
}
