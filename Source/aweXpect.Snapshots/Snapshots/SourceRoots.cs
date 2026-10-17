using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace aweXpect.Snapshots;

/// <summary>
///     Converts the path of a test file to the <see cref="SnapshotId.SourceFile" />, which is relative to the source root
///     that the build targets of aweXpect.Snapshots recorded in the test assembly, and back.
/// </summary>
/// <remarks>
///     Each recorded value has the form <c>&lt;MappedPath&gt;=&lt;SourceRoot&gt;</c>, where the mapped path is the
///     source root itself, or the prefix such as <c>/_/</c> that a deterministic build emits instead of it. The loaded
///     assemblies are searched for the recorded values once, when the first snapshot needs them.
/// </remarks>
internal static class SourceRoots
{
	private const string MetadataKey = "aweXpect.Snapshots.SourceRoot";

	private static readonly StringComparison PathComparison = Path.DirectorySeparatorChar == '\\'
		? StringComparison.OrdinalIgnoreCase
		: StringComparison.Ordinal;

	private static readonly Lazy<KeyValuePair<string, string>[]> Recorded = new(ReadRecordedSourceRoots);

	/// <summary>
	///     Returns the <paramref name="testFilePath" /> relative to its source root with <c>/</c> as separator, or the
	///     <paramref name="testFilePath" /> unchanged when no source root that contains it was recorded.
	/// </summary>
	public static string GetSourceFile(string testFilePath)
	{
		string? mappedPath = Recorded.Value.Select(recorded => recorded.Key)
			.FirstOrDefault(key => testFilePath.StartsWith(key, PathComparison));
		return mappedPath is null
			? testFilePath
			: testFilePath.Substring(mappedPath.Length).Replace('\\', '/');
	}

	/// <summary>
	///     Returns the full path of the <paramref name="sourceFile" /> on this machine.
	/// </summary>
	/// <remarks>
	///     The build targets record only the source root of the test project, so the first recorded one is used; several
	///     only occur when test assemblies from different source roots are loaded into the same process.
	/// </remarks>
	public static string GetPath(string sourceFile)
	{
		if (Path.IsPathRooted(sourceFile) || Recorded.Value.Length == 0)
		{
			return sourceFile;
		}

		return Path.Combine(Recorded.Value[0].Value, sourceFile.Replace('/', Path.DirectorySeparatorChar));
	}

	/// <summary>
	///     Throws when the <paramref name="sourceFile" /> is a path that a deterministic build emitted, e.g.
	///     <c>/_/Tests/MyTests.cs</c>, which only happens when its source root was not recorded, so that this is reported
	///     instead of a missing snapshot.
	/// </summary>
	public static void ThrowIfDeterministic(string sourceFile)
	{
		if (!sourceFile.StartsWith("/_", StringComparison.Ordinal))
		{
			return;
		}

		int index = 2;
		while (index < sourceFile.Length && sourceFile[index] is >= '0' and <= '9')
		{
			index++;
		}

		if (index < sourceFile.Length && sourceFile[index] == '/')
		{
			throw new DirectoryNotFoundException(
				$"The path \"{sourceFile}\" comes from a deterministic build, but the test assembly records no source root for it, which the build targets of aweXpect.Snapshots add.");
		}
	}

	/// <remarks>
	///     A mapped path never contains <c>=</c>, so the first one separates it from the source root.
	/// </remarks>
	private static KeyValuePair<string, string>[] ReadRecordedSourceRoots()
	{
		List<KeyValuePair<string, string>> sourceRoots = new();
		foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
		{
			foreach (AssemblyMetadataAttribute attribute in GetMetadata(assembly))
			{
				int separator = attribute.Value?.IndexOf('=') ?? -1;
				if (attribute.Key == MetadataKey && separator > 0)
				{
					sourceRoots.Add(new KeyValuePair<string, string>(
						attribute.Value!.Substring(0, separator), attribute.Value.Substring(separator + 1)));
				}
			}
		}

		return sourceRoots.ToArray();
	}

	/// <remarks>
	///     Reading the attributes of an assembly whose dependencies cannot be loaded throws, but such an assembly is not
	///     the test assembly, so it is skipped.
	/// </remarks>
	private static IEnumerable<AssemblyMetadataAttribute> GetMetadata(Assembly assembly)
	{
		try
		{
			return assembly.GetCustomAttributes<AssemblyMetadataAttribute>();
		}
		catch (Exception)
		{
			return [];
		}
	}
}
