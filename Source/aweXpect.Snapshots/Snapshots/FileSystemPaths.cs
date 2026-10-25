using System;
using System.Runtime.InteropServices;

namespace aweXpect.Snapshots;

/// <summary>
///     Decides how the paths of the file system are compared, so that resolving the source roots, finding the test
///     files of the snapshots and masking the paths of the environment agree.
/// </summary>
/// <remarks>
///     The case is ignored on Windows and macOS, whose file systems ignore it by default, and respected elsewhere. It
///     depends on the operating system and not on the directory separator, as macOS uses <c>/</c> like Linux.
/// </remarks>
internal static class FileSystemPaths
{
	/// <summary>
	///     Whether the case of a path is ignored.
	/// </summary>
	public static readonly bool IgnoresCase =
		RuntimeInformation.IsOSPlatform(OSPlatform.Windows) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

	/// <summary>
	///     Compares two paths, see <see cref="IgnoresCase" />.
	/// </summary>
	public static readonly StringComparison Comparison =
		IgnoresCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
}
