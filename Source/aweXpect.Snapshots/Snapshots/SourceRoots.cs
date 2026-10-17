using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace aweXpect.Snapshots;

/// <summary>
///     Converts the path of a test file to the <see cref="SnapshotId.SourceFile" />, which is relative to the source root
///     that the build targets of aweXpect.Snapshots recorded in the test assembly, and back.
/// </summary>
/// <remarks>
///     Each recorded value has the form <c>&lt;MappedPath&gt;=&lt;SourceRoot&gt;</c>, where the mapped path is the
///     source root itself, or the prefix such as <c>/_/</c> that a deterministic build emits instead of it. The loaded
///     assemblies are searched for the recorded values once, when the first snapshot needs them, and every assembly that
///     is loaded afterwards adds its own.
/// </remarks>
internal static class SourceRoots
{
	private const string MetadataKey = "aweXpect.Snapshots.SourceRoot";

	private static readonly StringComparison PathComparison = Path.DirectorySeparatorChar == '\\'
		? StringComparison.OrdinalIgnoreCase
		: StringComparison.Ordinal;

	private static readonly ConcurrentQueue<Assembly> LoadedAssemblies = new();
	private static readonly object RecordedLock = new();
	private static KeyValuePair<string, string>[]? _recorded;

	/// <summary>
	///     The recorded values, read from the loaded assemblies on first use and extended by every assembly that is
	///     loaded afterwards.
	/// </summary>
	/// <remarks>
	///     The event is subscribed before the loaded assemblies are read, so that no assembly is missed in between; one
	///     that is read twice only records the same source root twice, which counts as one.<br />
	///     An assembly loaded afterwards is only queued by the event and read on the next access, as the event of a
	///     dynamic assembly is raised before its attributes exist, and as reading the attributes of every assembly that
	///     is loaded would cost even when no snapshot needs them. The array is replaced instead of changed, so that the
	///     common case of nothing new to read needs no lock.
	/// </remarks>
	private static KeyValuePair<string, string>[] Recorded
	{
		get
		{
			KeyValuePair<string, string>[]? recorded = Volatile.Read(ref _recorded);
			if (recorded is not null && LoadedAssemblies.IsEmpty)
			{
				return recorded;
			}

			lock (RecordedLock)
			{
				if (_recorded is null)
				{
					AppDomain.CurrentDomain.AssemblyLoad += (_, args) => LoadedAssemblies.Enqueue(args.LoadedAssembly);
					Volatile.Write(ref _recorded, ReadSourceRoots(AppDomain.CurrentDomain.GetAssemblies()));
				}

				List<Assembly> loadedAssemblies = new();
				while (LoadedAssemblies.TryDequeue(out Assembly? assembly))
				{
					loadedAssemblies.Add(assembly);
				}

				if (loadedAssemblies.Count > 0)
				{
					Volatile.Write(ref _recorded, [.. _recorded!, .. ReadSourceRoots(loadedAssemblies),]);
				}

				return _recorded!;
			}
		}
	}

	/// <summary>
	///     Returns the <paramref name="testFilePath" /> relative to its source root with <c>/</c> as separator, or the
	///     <paramref name="testFilePath" /> unchanged when no source root that contains it was recorded.
	/// </summary>
	/// <remarks>
	///     The longest matching mapped path wins, so that the result does not depend on the order in which the
	///     assemblies were loaded.
	/// </remarks>
	public static string GetSourceFile(string testFilePath)
	{
		string? mappedPath = Recorded.Select(recorded => recorded.Key)
			.Where(key => testFilePath.StartsWith(key, PathComparison))
			.OrderByDescending(key => key.Length)
			.FirstOrDefault();
		return mappedPath is null
			? testFilePath
			: testFilePath.Substring(mappedPath.Length).Replace('\\', '/');
	}

	/// <summary>
	///     Returns the full path of the <paramref name="sourceFile" /> on this machine.
	/// </summary>
	/// <remarks>
	///     With several source roots, which only occurs when test assemblies from different source roots are loaded
	///     into the same process, the one under which the test file exists is used. When that is not exactly one, the
	///     first one is returned, so that the snapshot can still be described; <see cref="ThrowIfUnresolvable" /> reports
	///     the reason.
	/// </remarks>
	public static string GetPath(string sourceFile)
	{
		if (Path.IsPathRooted(sourceFile))
		{
			return sourceFile;
		}

		string[] sourceRoots = GetDistinctSourceRoots();
		if (sourceRoots.Length == 0)
		{
			return sourceFile;
		}

		string[] candidates = sourceRoots.Select(sourceRoot => Combine(sourceRoot, sourceFile)).ToArray();
		string[] existing = candidates.Length == 1 ? candidates : candidates.Where(File.Exists).ToArray();
		return existing.Length == 1 ? existing[0] : candidates[0];
	}

	/// <summary>
	///     Throws when the <paramref name="sourceFile" /> cannot be resolved to one test file on this machine, so that
	///     this is reported instead of a missing or a wrong snapshot.
	/// </summary>
	/// <remarks>
	///     A path that a deterministic build emitted, e.g. <c>/_/Tests/MyTests.cs</c>, only remains when its source root
	///     was not recorded. A relative path is ambiguous when several source roots are recorded and the test file exists
	///     under none or more than one of them. The test file is checked instead of its directory, as a directory like
	///     <c>Tests</c> commonly exists in several repositories, and a test file always exists where it was compiled.
	/// </remarks>
	public static void ThrowIfUnresolvable(string sourceFile)
	{
		if (IsDeterministic(sourceFile))
		{
			throw new DirectoryNotFoundException(
				$"The path \"{sourceFile}\" comes from a deterministic build, but the test assembly records no source root for it, which the build targets of aweXpect.Snapshots add.");
		}

		if (Path.IsPathRooted(sourceFile))
		{
			return;
		}

		string[] sourceRoots = GetDistinctSourceRoots();
		if (sourceRoots.Length < 2)
		{
			return;
		}

		int existing = sourceRoots.Count(sourceRoot => File.Exists(Combine(sourceRoot, sourceFile)));
		if (existing != 1)
		{
			throw new FileNotFoundException(
				$"The test file \"{sourceFile}\" exists under {(existing == 0 ? "none" : "more than one")} of the source roots {string.Join(", ", sourceRoots.Select(sourceRoot => $"\"{sourceRoot}\""))}, which test assemblies from different source roots in this process record. Run them in separate processes.");
		}
	}

	private static string Combine(string sourceRoot, string sourceFile)
		=> Path.Combine(sourceRoot, sourceFile.Replace('/', Path.DirectorySeparatorChar));

	/// <remarks>
	///     A source root that is recorded several times, also with another trailing separator or, on Windows, another
	///     case of the drive letter, counts as one.
	/// </remarks>
	private static string[] GetDistinctSourceRoots()
	{
		List<string> sourceRoots = new();
		foreach (string sourceRoot in Recorded.Select(recorded => recorded.Value))
		{
			string normalized = Normalize(sourceRoot);
			if (!sourceRoots.Any(existing => string.Equals(Normalize(existing), normalized, PathComparison)))
			{
				sourceRoots.Add(sourceRoot);
			}
		}

		return sourceRoots.ToArray();
	}

	private static string Normalize(string sourceRoot)
		=> sourceRoot.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar)
			.TrimEnd(Path.DirectorySeparatorChar);

	private static bool IsDeterministic(string sourceFile)
	{
		if (!sourceFile.StartsWith("/_", StringComparison.Ordinal))
		{
			return false;
		}

		int index = 2;
		while (index < sourceFile.Length && sourceFile[index] is >= '0' and <= '9')
		{
			index++;
		}

		return index < sourceFile.Length && sourceFile[index] == '/';
	}

	/// <remarks>
	///     A mapped path never contains <c>=</c>, so the first one separates it from the source root.
	/// </remarks>
	private static KeyValuePair<string, string>[] ReadSourceRoots(IEnumerable<Assembly> assemblies)
		=> assemblies.SelectMany(GetMetadata)
			.Where(attribute => attribute.Key == MetadataKey && attribute.Value?.IndexOf('=') > 0)
			.Select(attribute => new KeyValuePair<string, string>(
				attribute.Value!.Substring(0, attribute.Value.IndexOf('=')),
				attribute.Value.Substring(attribute.Value.IndexOf('=') + 1)))
			.ToArray();

	/// <remarks>
	///     Reading the attributes of an assembly whose dependencies cannot be loaded throws, but such an assembly is not
	///     the test assembly, so it is skipped.
	/// </remarks>
	private static AssemblyMetadataAttribute[] GetMetadata(Assembly assembly)
	{
		try
		{
			return assembly.GetCustomAttributes<AssemblyMetadataAttribute>().ToArray();
		}
		catch (Exception)
		{
			return [];
		}
	}
}
