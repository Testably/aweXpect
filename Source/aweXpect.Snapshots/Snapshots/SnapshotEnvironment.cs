using System;
using System.Linq;
using System.Runtime.InteropServices;
using aweXpect.Core;

namespace aweXpect.Snapshots;

/// <summary>
///     The values of the environment that a snapshot can be unique for, which form its <see cref="SnapshotId.Variant" />.
/// </summary>
/// <remarks>
///     The values only come from the build targets and <see cref="RuntimeInformation" />, so that they are available
///     without reflection, also under Native AOT.
/// </remarks>
internal static class SnapshotEnvironment
{
	/// <summary>
	///     Separates the parts of a <see cref="SnapshotId.Variant" />.
	/// </summary>
	public const char Separator = '@';

	private static readonly string Architecture = RuntimeInformation.ProcessArchitecture.ToString().ToLowerInvariant();
	private static readonly string? OperatingSystem = GetCurrentOperatingSystem();

	/// <summary>
	///     Returns the architecture of the process in lower case, e.g. <c>x64</c> or <c>arm64</c>.
	/// </summary>
	public static string GetArchitecture() => Architecture;

	/// <summary>
	///     Returns <c>windows</c>, <c>linux</c>, <c>osx</c> or <c>freebsd</c>.
	/// </summary>
	/// <remarks>
	///     Any other operating system throws, as a name made up from its description could change with its version and
	///     would then no longer find the snapshot.
	/// </remarks>
	public static string GetOperatingSystem()
		=> OperatingSystem ?? throw Tracing.WriteException(new PlatformNotSupportedException(
			$"The operating system \"{RuntimeInformation.OSDescription}\" has no name for a snapshot, which only Windows, Linux, macOS and FreeBSD have."));

	/// <summary>
	///     Returns the target framework of the test project of the <paramref name="sourceFile" /> as the build knows it,
	///     e.g. <c>net8.0</c> or <c>net48</c>.
	/// </summary>
	/// <remarks>
	///     Without a recorded target framework it throws instead of deriving one from the runtime, as the runtime can
	///     differ from the target framework, e.g. when a test project for <c>net8.0</c> rolls forward to a newer runtime.
	/// </remarks>
	public static string GetTargetFramework(string sourceFile)
		=> SourceRoots.GetTargetFramework(sourceFile) ?? throw Tracing.WriteException(new InvalidOperationException(
			$"No single target framework is recorded for the test file \"{sourceFile}\", which the build targets of aweXpect.Snapshots record for its test project."));

	/// <summary>
	///     Returns whether every part of the <see cref="SnapshotId.Variant" /> of the <paramref name="id" /> is a value of
	///     this environment, so that a test run here would use the snapshot.
	/// </summary>
	/// <remarks>
	///     A value that cannot be determined here matches no part, so that a snapshot of another environment is never
	///     taken for an orphan. The values are compared ignoring the case, like the claimed snapshots.
	/// </remarks>
	public static bool IsCurrent(SnapshotId id)
	{
		if (id.Variant is null)
		{
			return true;
		}

		string?[] values = [SourceRoots.GetTargetFramework(id.SourceFile), OperatingSystem, Architecture,];
		return id.Variant.Split(Separator)
			.All(part => values.Contains(part, StringComparer.OrdinalIgnoreCase));
	}

	private static string? GetCurrentOperatingSystem()
	{
		if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
		{
			return "windows";
		}

		if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
		{
			return "linux";
		}

		if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
		{
			return "osx";
		}

		return RuntimeInformation.IsOSPlatform(OSPlatform.Create("FREEBSD")) ? "freebsd" : null;
	}

	/// <summary>
	///     The parts of a <see cref="SnapshotId.Variant" />, in their order.
	/// </summary>
	public enum Part
	{
		TargetFramework,
		OperatingSystem,
		Architecture,
	}
}
