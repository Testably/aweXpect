using System.Linq;
using System.Reflection;

namespace aweXpect.Snapshots.Tests.TestHelpers;

/// <summary>
///     The paths that the build targets recorded in this test assembly.
/// </summary>
internal static class RecordedPaths
{
	public static string ProjectDirectory => Get("aweXpect.Snapshots.ProjectDirectory");

	public static string SourceRoot
	{
		get
		{
			string value = Get("aweXpect.Snapshots.SourceRoot");
			return value.Substring(value.IndexOf('=') + 1);
		}
	}

	private static string Get(string key)
		=> typeof(RecordedPaths).Assembly.GetCustomAttributes<AssemblyMetadataAttribute>()
			.Single(attribute => attribute.Key == key).Value!;
}
