using System.IO;

namespace aweXpect.Snapshots.SourceRoots.Tests.TestHelpers;

/// <summary>
///     A temporary source root, which is deleted again afterwards, so that its files no longer exist for the later
///     tests, although the root stays recorded in the process.
/// </summary>
internal sealed class TemporarySourceRoot : IDisposable
{
	public TemporarySourceRoot()
	{
		Directory.CreateDirectory(Path);
	}

	public string Path { get; } = System.IO.Path.Combine(System.IO.Path.GetTempPath(),
		$"aweXpect.Snapshots.SourceRoot.{Guid.NewGuid():N}") + System.IO.Path.DirectorySeparatorChar;

	public void Dispose() => Directory.Delete(Path, true);

	public void CreateFile(string relativePath, string content = "")
	{
		string fullPath = System.IO.Path.Combine(Path, relativePath.Replace('/', System.IO.Path.DirectorySeparatorChar));
		Directory.CreateDirectory(System.IO.Path.GetDirectoryName(fullPath)!);
		File.WriteAllText(fullPath, content);
	}
}
