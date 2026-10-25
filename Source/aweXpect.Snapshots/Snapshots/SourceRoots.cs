using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using aweXpect.Core.Extending;
using aweXpect.Customization;

namespace aweXpect.Snapshots;

/// <summary>
///     Converts the path of a test file to the <see cref="SnapshotId.SourceFile" />, which is relative to the source root
///     that the build targets of aweXpect.Snapshots recorded in the test assembly, and back.
/// </summary>
/// <remarks>
///     Each recorded value has the form <c>&lt;MappedPath&gt;=&lt;SourceRoot&gt;</c>, where the mapped path is the
///     source root itself, or the prefix such as <c>/_/</c> that a deterministic build emits instead of it. A module
///     initializer that the source generator of aweXpect.Core emits into the test project registers the recorded values
///     in the <see cref="ExtensionRegistry" />. Where the test project cannot compile a module initializer, nothing is
///     registered, and the loaded assemblies are searched for the recorded values instead.<br />
///     The directories of the test projects are recorded as well, so that <see cref="OrphanedSnapshots" /> only lists
///     the snapshots of these projects, each with its target framework for <c>.UniqueFor.TargetFramework()</c>.
/// </remarks>
internal static class SourceRoots
{
	private const string MetadataKey = "aweXpect.Snapshots.SourceRoot";
	private const string ProjectDirectoryKey = "aweXpect.Snapshots.ProjectDirectory";
	private const string TargetFrameworkKey = "aweXpect.Snapshots.TargetFramework";

	private static readonly ConcurrentQueue<Assembly> LoadedAssemblies = new();
	private static readonly object RecordedLock = new();

	/// <remarks>
	///     Decided once, when the first snapshot needs the values, which is after the module initializer of the test
	///     assembly registered its own, as it runs before any code of the test assembly.
	/// </remarks>
	private static readonly Lazy<bool> IsRegistered =
		new(() => ExtensionRegistry.GetValues(ProjectDirectoryKey).Count > 0);

	private static Records? _records;
	private static RegisteredRecords? _registeredRecords;

	/// <summary>
	///     The recorded source roots.
	/// </summary>
	private static KeyValuePair<string, string>[] Recorded => GetRecords().SourceRoots;

	/// <summary>
	///     The recorded values, from the <see cref="ExtensionRegistry" /> where the test assembly registers them, or
	///     else read from the loaded assemblies.
	/// </summary>
	private static Records GetRecords()
		=> IsRegistered.Value ? GetRegisteredRecords() : GetScannedRecords();

	/// <remarks>
	///     A test assembly that is loaded later registers its values when it is loaded. The registry returns the same
	///     lists until a value is added, so the records are only built again when one was added.
	/// </remarks>
	private static Records GetRegisteredRecords()
	{
		IReadOnlyList<string> sourceRoots = ExtensionRegistry.GetValues(MetadataKey);
		IReadOnlyList<string> projectDirectories = ExtensionRegistry.GetValues(ProjectDirectoryKey);
		IReadOnlyList<string> targetFrameworks = ExtensionRegistry.GetValues(TargetFrameworkKey);
		RegisteredRecords? registered = Volatile.Read(ref _registeredRecords);
		if (registered is null ||
		    !ReferenceEquals(registered.SourceRoots, sourceRoots) ||
		    !ReferenceEquals(registered.ProjectDirectories, projectDirectories) ||
		    !ReferenceEquals(registered.TargetFrameworks, targetFrameworks))
		{
			registered = new RegisteredRecords(sourceRoots, projectDirectories, targetFrameworks,
				Parse(sourceRoots, projectDirectories, targetFrameworks));
			Volatile.Write(ref _registeredRecords, registered);
		}

		return registered.Records;
	}

	/// <summary>
	///     The recorded values, read from the loaded assemblies on first use and extended by every assembly that is
	///     loaded afterwards.
	/// </summary>
	/// <remarks>
	///     The assemblies that the <c>ExcludedAssemblyPrefixes</c> of the settings exclude are not read.<br />
	///     The event is subscribed before the loaded assemblies are read, so that no assembly is missed in between; one
	///     that is read twice only records the same value twice, which counts as one.<br />
	///     An assembly loaded afterwards is only queued by the event and read on the next access, as the event of a
	///     dynamic assembly is raised before its attributes exist, and as reading the attributes of every assembly that
	///     is loaded would cost even when no snapshot needs them. The records are replaced instead of changed, so that
	///     the common case of nothing new to read needs no lock.
	/// </remarks>
	private static Records GetScannedRecords()
	{
		Records? records = Volatile.Read(ref _records);
		if (records is not null && LoadedAssemblies.IsEmpty)
		{
			return records;
		}

		lock (RecordedLock)
		{
			if (_records is null)
			{
				AppDomain.CurrentDomain.AssemblyLoad += (_, args) => LoadedAssemblies.Enqueue(args.LoadedAssembly);
				Volatile.Write(ref _records, Read(AppDomain.CurrentDomain.GetAssemblies()));
			}

			List<Assembly> loadedAssemblies = new();
			while (LoadedAssemblies.TryDequeue(out Assembly? assembly))
			{
				loadedAssemblies.Add(assembly);
			}

			if (loadedAssemblies.Count > 0)
			{
				Records loaded = Read(loadedAssemblies);
				Volatile.Write(ref _records, new Records(
					[.. _records!.SourceRoots, .. loaded.SourceRoots,],
					[.. _records.ProjectDirectories, .. loaded.ProjectDirectories,],
					[.. _records.TargetFrameworks, .. loaded.TargetFrameworks,]));
			}

			return _records!;
		}
	}

	/// <summary>
	///     Returns the full path of every recorded project directory, each one once.
	/// </summary>
	public static string[] GetProjectDirectories()
	{
		List<string> projectDirectories = new();
		foreach (string projectDirectory in GetRecords().ProjectDirectories)
		{
			string normalized = Normalize(projectDirectory);
			if (!projectDirectories.Any(existing
				    => string.Equals(Normalize(existing), normalized, FileSystemPaths.Comparison)))
			{
				projectDirectories.Add(projectDirectory);
			}
		}

		return projectDirectories.ToArray();
	}

	/// <summary>
	///     Returns the target framework of the test project whose directory contains the <paramref name="sourceFile" />,
	///     or <see langword="null" /> when none or several different ones are recorded for it.
	/// </summary>
	/// <remarks>
	///     The innermost project directory wins, so that a project in an enclosing directory does not decide it.<br />
	///     With several source roots, the ones under which the test file exists are used, or every one when it exists
	///     under none, e.g. after it was deleted, so that the snapshots of a deleted test file are still checked.
	/// </remarks>
	public static string? GetTargetFramework(string sourceFile)
	{
		string[] paths = Path.IsPathRooted(sourceFile)
			? [sourceFile,]
			: GetDistinctSourceRoots().Select(sourceRoot => Combine(sourceRoot, sourceFile)).ToArray();
		string[] existing = paths.Where(File.Exists).ToArray();
		string[] targetFrameworks = (existing.Length > 0 ? existing : paths)
			.SelectMany(GetTargetFrameworksOfPath)
			.Distinct(StringComparer.Ordinal)
			.ToArray();
		return targetFrameworks.Length == 1 ? targetFrameworks[0] : null;
	}

	private static IEnumerable<string> GetTargetFrameworksOfPath(string path)
	{
		string normalizedPath = Normalize(path);
		(string Directory, string TargetFramework)[] containing = GetRecords().TargetFrameworks
			.Select(recorded => (Directory: Normalize(recorded.Key), TargetFramework: recorded.Value))
			.Where(recorded => normalizedPath.StartsWith(recorded.Directory + Path.DirectorySeparatorChar,
				FileSystemPaths.Comparison))
			.ToArray();
		if (containing.Length == 0)
		{
			return [];
		}

		int innermost = containing.Max(recorded => recorded.Directory.Length);
		return containing
			.Where(recorded => recorded.Directory.Length == innermost)
			.Select(recorded => recorded.TargetFramework);
	}

	/// <summary>
	///     Returns the recorded project directory that contains the test file <paramref name="sourceFile" />, the
	///     innermost one when they are nested, or <see langword="null" /> when none contains it.
	/// </summary>
	public static string? GetProjectDirectory(string sourceFile)
	{
		string testFile = Normalize(GetPath(sourceFile));
		return GetProjectDirectories()
			.Select(Normalize)
			.Where(projectDirectory => testFile.StartsWith(projectDirectory + Path.DirectorySeparatorChar,
				FileSystemPaths.Comparison))
			.OrderByDescending(projectDirectory => projectDirectory.Length)
			.FirstOrDefault();
	}

	/// <summary>
	///     Returns the full <paramref name="path" /> of a test file on this machine relative to the source root that
	///     contains it, like <see cref="GetSourceFile" /> returns it for the path that the compiler set.
	/// </summary>
	/// <remarks>
	///     A deterministic build emits a mapped path, but a file found on this machine has the real path, so the real
	///     source roots are matched instead of the mapped paths.
	/// </remarks>
	public static string GetSourceFileOfPath(string path)
	{
		string? sourceRoot = Recorded.Select(recorded => recorded.Value)
			.Where(value => path.StartsWith(value, FileSystemPaths.Comparison))
			.OrderByDescending(value => value.Length)
			.FirstOrDefault();
		return sourceRoot is null
			? path
			: path.Substring(sourceRoot.Length).Replace('\\', '/');
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
			.Where(key => testFilePath.StartsWith(key, FileSystemPaths.Comparison))
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
	///     A source root that is recorded several times, also with another trailing separator or, where the file system
	///     ignores the case (see <see cref="FileSystemPaths" />), in another case, counts as one.
	/// </remarks>
	public static string[] GetDistinctSourceRoots()
	{
		List<string> sourceRoots = new();
		foreach (string sourceRoot in Recorded.Select(recorded => recorded.Value))
		{
			string normalized = Normalize(sourceRoot);
			if (!sourceRoots.Any(existing => string.Equals(Normalize(existing), normalized, FileSystemPaths.Comparison)))
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
	///     A mapped path never contains <c>=</c>, so the first one separates it from the source root. The same holds for
	///     a target framework, which is recorded as <c>&lt;TargetFramework&gt;=&lt;ProjectDirectory&gt;</c>.
	/// </remarks>
	private static Records Read(IEnumerable<Assembly> assemblies)
	{
		IReadOnlyList<string> excludedPrefixes = Customize.aweXpect.Settings().ExcludedAssemblyPrefixes.Get();
		AssemblyMetadataAttribute[] attributes = assemblies
			.SelectMany(assembly => GetMetadata(assembly, excludedPrefixes))
			.ToArray();
		return Parse(GetValues(attributes, MetadataKey), GetValues(attributes, ProjectDirectoryKey),
			GetValues(attributes, TargetFrameworkKey));
	}

	private static IEnumerable<string> GetValues(AssemblyMetadataAttribute[] attributes, string key)
		=> attributes
			.Where(attribute => attribute.Key == key && attribute.Value is not null)
			.Select(attribute => attribute.Value!);

	private static Records Parse(IEnumerable<string> sourceRoots, IEnumerable<string> projectDirectories,
		IEnumerable<string> targetFrameworks)
		=> new(
			sourceRoots
				.Where(value => value.IndexOf('=') > 0)
				.Select(value => new KeyValuePair<string, string>(
					value.Substring(0, value.IndexOf('=')),
					value.Substring(value.IndexOf('=') + 1)))
				.ToArray(),
			projectDirectories
				.Where(value => value.Length > 0)
				.ToArray(),
			targetFrameworks
				.Where(value => value.IndexOf('=') > 0)
				.Select(value => new KeyValuePair<string, string>(
					value.Substring(value.IndexOf('=') + 1),
					value.Substring(0, value.IndexOf('='))))
				.ToArray());

	/// <remarks>
	///     Reading the attributes of an assembly whose dependencies cannot be loaded throws, but such an assembly is not
	///     the test assembly, so it is skipped.
	/// </remarks>
	private static AssemblyMetadataAttribute[] GetMetadata(Assembly assembly, IReadOnlyList<string> excludedPrefixes)
	{
		try
		{
			return AssemblyPrefixes.IsExcluded(assembly.GetName().Name, excludedPrefixes)
				? []
				: assembly.GetCustomAttributes<AssemblyMetadataAttribute>().ToArray();
		}
		catch (Exception)
		{
			return [];
		}
	}

	/// <param name="SourceRoots">Each recorded <c>&lt;MappedPath&gt;=&lt;SourceRoot&gt;</c> as key and value.</param>
	/// <param name="ProjectDirectories">The full path of each recorded project directory.</param>
	/// <param name="TargetFrameworks">
	///     The full path of each recorded project directory as key and its target framework as value.
	/// </param>
	private sealed record Records(
		KeyValuePair<string, string>[] SourceRoots,
		string[] ProjectDirectories,
		KeyValuePair<string, string>[] TargetFrameworks);

	/// <summary>
	///     The <paramref name="Records" /> built from the lists that the <see cref="ExtensionRegistry" /> returned.
	/// </summary>
	private sealed record RegisteredRecords(
		IReadOnlyList<string> SourceRoots,
		IReadOnlyList<string> ProjectDirectories,
		IReadOnlyList<string> TargetFrameworks,
		Records Records);
}
