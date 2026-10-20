using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace aweXpect.Snapshots;

/// <summary>
///     The paths of the environment that are masked by default, as they differ between machines.
/// </summary>
/// <remarks>
///     A path is matched with the separators of the system, on Windows <c>\</c> and <c>/</c>, also with escaped
///     backslashes (<c>C:\\Music</c>) as the formatter writes them in a string of an object snapshot, and elsewhere only
///     <c>/</c>, as a backslash is part of a file name there. It only matches as whole segments, so
///     <c>C:\Music\Album</c> does not match inside <c>C:\Music\Albums</c>. The text after a match keeps its own
///     separators. The case is ignored on Windows and macOS, whose file systems ignore it by default.
/// </remarks>
internal static class EnvironmentMaskings
{
	private static readonly char[] Separators = [Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar,];

	private static readonly string Separator = Path.DirectorySeparatorChar == '\\' ? @"(?:\\\\|[\\/])" : "/";

	/// <summary>
	///     Not after a character of the same segment, unless it is the end of an escape sequence such as <c>\n</c>.
	/// </summary>
	private const string SegmentStart = @"(?<!(?<!\\)[\w.-])";

	/// <summary>
	///     Not before a character of the same segment; a dot only continues the segment when another character follows,
	///     so that a path at the end of a sentence is matched.
	/// </summary>
	private const string SegmentEnd = @"(?![\w-]|\.[\w-])";

	private static readonly ConcurrentDictionary<(string Placeholder, string Path), TextMasking> Maskings = new();

	public static readonly RegexOptions PathOptions =
		RuntimeInformation.IsOSPlatform(OSPlatform.Windows) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX)
			? RegexOptions.IgnoreCase
			: RegexOptions.None;

	/// <summary>
	///     Returns the maskings of the paths of the environment for a snapshot of the test file
	///     <paramref name="sourceFile" />, the longest path first, as they are nested in each other, e.g. the output
	///     directory in the project directory in the source root.
	/// </summary>
	/// <remarks>
	///     Only the directory of the project that contains the test file is <c>{ProjectDirectory}</c>, as another
	///     project, e.g. a shared test helper, records its directory as well; every recorded source root is
	///     <c>{SourceRoot}</c>. A path that is unknown, not rooted or a root of the file system, such as <c>C:\</c> or
	///     <c>/</c>, is not masked.
	/// </remarks>
	public static IEnumerable<TextMasking> ForPaths(string sourceFile)
	{
		(string Placeholder, string? Path)[] paths =
		[
			("{ProjectDirectory}", SourceRoots.GetProjectDirectory(sourceFile)),
			.. SourceRoots.GetDistinctSourceRoots().Select(sourceRoot => ("{SourceRoot}", sourceRoot)),
			("{OutputDirectory}", AppContext.BaseDirectory),
			("{TempPath}", Path.GetTempPath()),
			("{UserProfile}", Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)),
		];
		return paths
			.Select(path => (path.Placeholder, Path: path.Path?.TrimEnd(Separators)))
			.Where(path => IsMaskable(path.Path))
			.OrderByDescending(path => path.Path!.Length)
			.Select(path => Maskings.GetOrAdd((path.Placeholder, path.Path!),
				key => TextMasking.ForPath(key.Path, key.Placeholder)));
	}

	/// <summary>
	///     Returns the pattern that matches the <paramref name="path" />, see the remarks of the class.
	/// </summary>
	public static string GetPathPattern(string path)
		=> SegmentStart + string.Join(Separator, path.Split(Separators).Select(Regex.Escape)) + SegmentEnd;

	private static bool IsMaskable(string? path)
		=> !string.IsNullOrEmpty(path) && Path.IsPathRooted(path) &&
		   Path.GetPathRoot(path)!.TrimEnd(Separators).Length < path!.Length;
}
