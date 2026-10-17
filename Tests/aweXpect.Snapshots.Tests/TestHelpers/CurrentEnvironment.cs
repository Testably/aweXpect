using System.Runtime.InteropServices;

namespace aweXpect.Snapshots.Tests.TestHelpers;

/// <summary>
///     The values of the environment that this test run expects in the variant of a snapshot.
/// </summary>
internal static class CurrentEnvironment
{
#if NET10_0
	public const string TargetFramework = "net10.0";
#elif NET8_0
	public const string TargetFramework = "net8.0";
#else
	public const string TargetFramework = "net48";
#endif

	public static string Architecture => RuntimeInformation.ProcessArchitecture.ToString().ToLowerInvariant();

	public static string OperatingSystem
	{
		get
		{
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				return "windows";
			}

			return RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? "osx" : "linux";
		}
	}
}
