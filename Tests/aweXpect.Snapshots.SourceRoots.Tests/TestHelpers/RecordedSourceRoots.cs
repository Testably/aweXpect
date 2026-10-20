using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace aweXpect.Snapshots.SourceRoots.Tests.TestHelpers;

/// <summary>
///     Records further source roots in the process, the way another test assembly from a different source root would.
/// </summary>
internal static class RecordedSourceRoots
{
	private const string MetadataKey = "aweXpect.Snapshots.SourceRoot";

	/// <summary>
	///     The mapped path and the source root that the build targets recorded in this test assembly.
	/// </summary>
	public static (string MappedPath, string SourceRoot) Own
	{
		get
		{
			string value = typeof(RecordedSourceRoots).Assembly.GetCustomAttributes<AssemblyMetadataAttribute>()
				.Single(attribute => attribute.Key == MetadataKey).Value!;
			int separator = value.IndexOf('=');
			return (value.Substring(0, separator), value.Substring(separator + 1));
		}
	}

	/// <summary>
	///     Loads a dynamic assembly that records the <paramref name="sourceRoot" /> for the
	///     <paramref name="mappedPath" />.
	/// </summary>
	public static void Record(string mappedPath, string sourceRoot)
		=> RecordMetadata(MetadataKey, $"{mappedPath}={sourceRoot}");

	/// <summary>
	///     Loads a dynamic assembly that records the <paramref name="targetFramework" /> for the test project in the
	///     <paramref name="projectDirectory" />.
	/// </summary>
	public static void RecordTargetFramework(string targetFramework, string projectDirectory)
		=> RecordMetadata("aweXpect.Snapshots.TargetFramework", $"{targetFramework}={projectDirectory}");

	private static void RecordMetadata(string key, string value)
		=> AssemblyBuilder.DefineDynamicAssembly(
			new AssemblyName($"SourceRoot{Guid.NewGuid():N}"), AssemblyBuilderAccess.Run,
			[
				new CustomAttributeBuilder(
					typeof(AssemblyMetadataAttribute).GetConstructor([typeof(string), typeof(string),])!,
					[key, value,]),
			]);

	/// <summary>
	///     The path of the calling test file relative to the own source root.
	/// </summary>
	public static string RelativeTestFile([CallerFilePath] string callerFilePath = "")
		=> callerFilePath.Substring(Own.MappedPath.Length).Replace('\\', '/');
}
