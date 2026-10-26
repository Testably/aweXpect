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
///     in the <see cref="ExtensionRegistry" />, together with the name of the assembly. Where a project cannot compile a
///     module initializer, nothing is registered, so the loaded assemblies that registered nothing are searched for the
///     recorded values as well.<br />
///     The directories of the test projects are recorded as well, so that <see cref="OrphanedSnapshots" /> only lists
///     the snapshots of these projects, each with its target framework for <c>.UniqueFor.TargetFramework()</c>.
/// </remarks>
internal static class SourceRoots
{
	private const string MetadataKey = "aweXpect.Snapshots.SourceRoot";
	private const string ProjectDirectoryKey = "aweXpect.Snapshots.ProjectDirectory";
	private const string TargetFrameworkKey = "aweXpect.Snapshots.TargetFramework";
	private const string AssemblyKey = "aweXpect.Snapshots.Assembly";

	private static readonly ConcurrentQueue<Assembly> LoadedAssemblies = new();
	private static readonly object RecordedLock = new();

	private static ScannedAssembly[]? _scanned;
	private static CombinedRecords? _combinedRecords;

	/// <summary>
	///     The recorded source roots.
	/// </summary>
	private static KeyValuePair<string, string>[] Recorded => GetRecords().SourceRoots;

	/// <summary>
	///     The recorded values, from the <see cref="ExtensionRegistry" /> and from the loaded assemblies that registered
	///     nothing and that the <c>ExcludedAssemblyPrefixes</c> of the settings do not exclude.
	/// </summary>
	/// <remarks>
	///     Each assembly is decided on its own, as the assemblies of one process can differ, e.g. a test project in F# or
	///     for .NET Framework with C# 7.3 that calls a helper library in C# 9, which registers its values.<br />
	///     The assemblies are decided on each access, not when they are read, as the settings can differ between tests
	///     and an assembly can be read before its module initializer registered its values.<br />
	///     The registry and the settings return the same lists until they change, so the records are only built again
	///     when one changed or an assembly was read.
	/// </remarks>
	private static Records GetRecords()
	{
		IReadOnlyList<string> sourceRoots = ExtensionRegistry.GetValues(MetadataKey);
		IReadOnlyList<string> projectDirectories = ExtensionRegistry.GetValues(ProjectDirectoryKey);
		IReadOnlyList<string> targetFrameworks = ExtensionRegistry.GetValues(TargetFrameworkKey);
		IReadOnlyList<string> registeredAssemblies = ExtensionRegistry.GetValues(AssemblyKey);
		IReadOnlyList<string> excludedPrefixes = Customize.aweXpect.Settings().ExcludedAssemblyPrefixes.Get();
		ScannedAssembly[] scanned = GetScannedAssemblies();
		CombinedRecords? combined = Volatile.Read(ref _combinedRecords);
		if (combined is null ||
		    !ReferenceEquals(combined.SourceRoots, sourceRoots) ||
		    !ReferenceEquals(combined.ProjectDirectories, projectDirectories) ||
		    !ReferenceEquals(combined.TargetFrameworks, targetFrameworks) ||
		    !ReferenceEquals(combined.RegisteredAssemblies, registeredAssemblies) ||
		    !ReferenceEquals(combined.ExcludedPrefixes, excludedPrefixes) ||
		    !ReferenceEquals(combined.Scanned, scanned))
		{
			Records[] records =
			[
				Parse(sourceRoots, projectDirectories, targetFrameworks),
				.. scanned
					.Where(assembly => !registeredAssemblies.Contains(assembly.Name) &&
					                   !AssemblyPrefixes.IsExcluded(assembly.Name, excludedPrefixes))
					.Select(assembly => assembly.Records),
			];
			combined = new CombinedRecords(sourceRoots, projectDirectories, targetFrameworks, registeredAssemblies,
				excludedPrefixes, scanned,
				new Records(
					records.SelectMany(record => record.SourceRoots).ToArray(),
					records.SelectMany(record => record.ProjectDirectories).ToArray(),
					records.SelectMany(record => record.TargetFrameworks).ToArray()));
			Volatile.Write(ref _combinedRecords, combined);
		}

		return combined.Records;
	}

	/// <summary>
	///     The loaded assemblies that record values, read on first use and extended by every assembly that is loaded
	///     afterwards.
	/// </summary>
	/// <remarks>
	///     The event is subscribed before the loaded assemblies are read, so that no assembly is missed in between; one
	///     that is read twice only records the same value twice, which counts as one.<br />
	///     An assembly loaded afterwards is only queued by the event and read on the next access, as the event of a
	///     dynamic assembly is raised before its attributes exist, and as reading the attributes of every assembly that
	///     is loaded would cost even when no snapshot needs them. The array is replaced instead of changed, so that the
	///     common case of nothing new to read needs no lock.
	/// </remarks>
	private static ScannedAssembly[] GetScannedAssemblies()
	{
		ScannedAssembly[]? scanned = Volatile.Read(ref _scanned);
		if (scanned is not null && LoadedAssemblies.IsEmpty)
		{
			return scanned;
		}

		lock (RecordedLock)
		{
			if (_scanned is null)
			{
				AppDomain.CurrentDomain.AssemblyLoad += (_, args) => LoadedAssemblies.Enqueue(args.LoadedAssembly);
				Volatile.Write(ref _scanned, Read(AppDomain.CurrentDomain.GetAssemblies()));
			}

			List<Assembly> loadedAssemblies = new();
			while (LoadedAssemblies.TryDequeue(out Assembly? assembly))
			{
				loadedAssemblies.Add(assembly);
			}

			if (loadedAssemblies.Count > 0)
			{
				Volatile.Write(ref _scanned, [.. _scanned!, .. Read(loadedAssemblies),]);
			}

			return _scanned!;
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
	///     <c>Tests</c> commonly exists in several repositories, and a test file always exists where it was compiled.<br />
	///     A test assembly whose name matches one of the <c>ExcludedAssemblyPrefixes</c>, e.g. <c>Microsoft.MyApp.Tests</c>,
	///     is not read when it cannot register its values, which the message then names.
	/// </remarks>
	public static void ThrowIfUnresolvable(string sourceFile)
	{
		if (IsDeterministic(sourceFile))
		{
			string[] excludedAssemblies = GetExcludedAssembliesRecording(sourceFile);
			throw new DirectoryNotFoundException(excludedAssemblies.Length > 0
				? $"The path \"{sourceFile}\" comes from a deterministic build, but the ExcludedAssemblyPrefixes of the settings exclude the assemblies that record a source root for it: {string.Join(", ", excludedAssemblies.Select(name => $"\"{name}\""))}. Remove the prefix that matches them from Customize.aweXpect.Settings().ExcludedAssemblyPrefixes."
				: $"The path \"{sourceFile}\" comes from a deterministic build, but the test assembly records no source root for it, which the build targets of aweXpect.Snapshots add.");
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
	private static ScannedAssembly[] Read(IEnumerable<Assembly> assemblies)
		=> assemblies
			.Select(assembly =>
			{
				AssemblyMetadataAttribute[] attributes = GetMetadata(assembly);
				return new ScannedAssembly(assembly.GetName().Name, Parse(GetValues(attributes, MetadataKey),
					GetValues(attributes, ProjectDirectoryKey), GetValues(attributes, TargetFrameworkKey)));
			})
			.Where(scanned => scanned.Records.SourceRoots.Length > 0 ||
			                  scanned.Records.ProjectDirectories.Length > 0 ||
			                  scanned.Records.TargetFrameworks.Length > 0)
			.ToArray();

	/// <summary>
	///     Returns the name of each loaded assembly that the <c>ExcludedAssemblyPrefixes</c> of the settings exclude, and
	///     that records a source root whose mapped path the <paramref name="sourceFile" /> starts with.
	/// </summary>
	private static string[] GetExcludedAssembliesRecording(string sourceFile)
	{
		IReadOnlyList<string> excludedPrefixes = Customize.aweXpect.Settings().ExcludedAssemblyPrefixes.Get();
		IReadOnlyList<string> registeredAssemblies = ExtensionRegistry.GetValues(AssemblyKey);
		return GetScannedAssemblies()
			.Where(assembly => !registeredAssemblies.Contains(assembly.Name) &&
			                   AssemblyPrefixes.IsExcluded(assembly.Name, excludedPrefixes) &&
			                   assembly.Records.SourceRoots.Any(recorded
				                   => sourceFile.StartsWith(recorded.Key, FileSystemPaths.Comparison)))
			.Select(assembly => assembly.Name!)
			.Distinct(StringComparer.Ordinal)
			.ToArray();
	}

	private static IEnumerable<string> GetValues(AssemblyMetadataAttribute[] attributes, string key)
		=> attributes
			.Where(attribute => attribute.Key == key && attribute.Value is not null)
			.Select(attribute => attribute.Value!);

	private static Records Parse(IEnumerable<string> sourceRoots, IEnumerable<string> projectDirectories,
		IEnumerable<string> targetFrameworks)
		=> new(
			SplitAtSeparator(sourceRoots)
				.ToArray(),
			projectDirectories
				.Where(value => value.Length > 0)
				.ToArray(),
			SplitAtSeparator(targetFrameworks)
				.Select(pair => new KeyValuePair<string, string>(pair.Value, pair.Key))
				.ToArray());

	/// <remarks>
	///     A value without a key before the first <c>=</c> is skipped, as an empty mapped path or target framework is
	///     never recorded.
	/// </remarks>
	private static IEnumerable<KeyValuePair<string, string>> SplitAtSeparator(IEnumerable<string> values)
		=> values
			.Select(value => (Value: value, Separator: value.IndexOf('=')))
			.Where(item => item.Separator > 0)
			.Select(item => new KeyValuePair<string, string>(
				item.Value.Substring(0, item.Separator),
				item.Value.Substring(item.Separator + 1)));

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
	///     The values that the assembly with the <paramref name="Name" /> records.
	/// </summary>
	private sealed record ScannedAssembly(string? Name, Records Records);

	/// <summary>
	///     The <paramref name="Records" /> built from the lists that the <see cref="ExtensionRegistry" /> and the settings
	///     returned and from the <paramref name="Scanned" /> assemblies.
	/// </summary>
	private sealed record CombinedRecords(
		IReadOnlyList<string> SourceRoots,
		IReadOnlyList<string> ProjectDirectories,
		IReadOnlyList<string> TargetFrameworks,
		IReadOnlyList<string> RegisteredAssemblies,
		IReadOnlyList<string> ExcludedPrefixes,
		ScannedAssembly[] Scanned,
		Records Records);
}
