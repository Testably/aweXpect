using System.IO;
using System.Reflection;
using System.Reflection.Emit;

namespace aweXpect.Snapshots.Orphans.Tests.TestHelpers;

/// <summary>
///     A test project in a temporary source root, which is recorded in the process the way the build targets record it
///     in another test assembly, and deleted again afterwards.
/// </summary>
/// <remarks>
///     The records stay in the process, but the deleted directory is no longer listed. The project directory has a unique
///     name, as it is part of the ids of its snapshots, which every test claims for the whole process.
/// </remarks>
internal sealed class TemporaryProject : IDisposable
{
	private readonly string _sourceRoot = System.IO.Path.Combine(System.IO.Path.GetTempPath(),
		$"aweXpect.Snapshots.Orphans.{Guid.NewGuid():N}") + System.IO.Path.DirectorySeparatorChar;

	/// <param name="mappedPath">
	///     The path that a deterministic build emits instead of the source root, e.g. <c>/_1/</c>, or
	///     <see langword="null" /> for the source root itself.
	/// </param>
	public TemporaryProject(string? mappedPath = null)
	{
		MappedPath = mappedPath ?? _sourceRoot;
		Name = $"Album{Guid.NewGuid():N}";
		Path = System.IO.Path.Combine(_sourceRoot, Name);
		Directory.CreateDirectory(Path);
		Record("aweXpect.Snapshots.SourceRoot", $"{MappedPath}={_sourceRoot}");
		Record("aweXpect.Snapshots.ProjectDirectory", Path);
	}

	public string MappedPath { get; }
	public string Name { get; }
	public string Path { get; }

	public void Dispose() => Directory.Delete(_sourceRoot, true);

	/// <summary>
	///     The path of the <paramref name="relativePath" /> that the compiler sets for a test file in this project.
	/// </summary>
	public string CallerFilePath(string relativePath) => MappedPath + Name + "/" + relativePath;

	public void CreateFile(string relativePath, string content = "")
	{
		string fullPath = GetFullPath(relativePath);
		Directory.CreateDirectory(System.IO.Path.GetDirectoryName(fullPath)!);
		File.WriteAllText(fullPath, content);
	}

	public bool Exists(string relativePath) => File.Exists(GetFullPath(relativePath));

	public string GetFullPath(string relativePath)
		=> System.IO.Path.Combine(Path, relativePath.Replace('/', System.IO.Path.DirectorySeparatorChar));

	private static void Record(string key, string value)
		=> AssemblyBuilder.DefineDynamicAssembly(
			new AssemblyName($"Orphans{Guid.NewGuid():N}"), AssemblyBuilderAccess.Run,
			[
				new CustomAttributeBuilder(
					typeof(AssemblyMetadataAttribute).GetConstructor([typeof(string), typeof(string),])!,
					[key, value,]),
			]);
}
