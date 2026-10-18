using System.Collections.Generic;
using System.IO;
using aweXpect.Customization;

namespace aweXpect.Snapshots.DiffEngine.Tests.TestHelpers;

/// <summary>
///     A temporary directory that stands in for the directory of a test file, with the environment variables of build
///     servers cleared, as the reporters are never called on a build server.
/// </summary>
/// <remarks>
///     The environment is shared by the whole process, so the tests do not run in parallel.
/// </remarks>
internal sealed class TestScope : IDisposable
{
	public const string MemberName = "MyTest";

	private static readonly string[] BuildServerVariables = ["CI", "TF_BUILD", "JENKINS_URL", "TEAMCITY_VERSION",];

	private readonly Dictionary<string, string?> _originalValues = new();
	private readonly CustomizationLifetime _reporters;

	public TestScope(RecordingDiffRunner diffRunner)
	{
		foreach (string variable in BuildServerVariables)
		{
			_originalValues[variable] = Environment.GetEnvironmentVariable(variable);
			Environment.SetEnvironmentVariable(variable, null);
		}

		Directory.CreateDirectory(Path);
		_reporters = Customize.aweXpect.Snapshots().Reporters.Set([new DiffEngineReporter(diffRunner),]);
		(TemporaryReceivedFile, TemporarySnapshotFile) = DiffEngineReporter.GetTemporaryFiles(Id);
	}

	public SnapshotId Id => new(TestFilePath, MemberName);
	public string Path { get; } = System.IO.Path.Combine(System.IO.Path.GetTempPath(),
		$"aweXpect.Snapshots.DiffEngine.Tests.{Guid.NewGuid():N}");
	public string PendingPath => System.IO.Path.Combine(Path, $"MyTests.{MemberName}.awexpect-snapshot.pending.txt");
	public string SnapshotPath => System.IO.Path.Combine(Path, $"MyTests.{MemberName}.awexpect-snapshot.txt");
	public string TemporaryReceivedFile { get; }
	public string TemporarySnapshotFile { get; }
	public string TestFilePath => System.IO.Path.Combine(Path, "MyTests.cs");

	public void Dispose()
	{
		_reporters.Dispose();
		Directory.Delete(Path, true);
		File.Delete(TemporaryReceivedFile);
		File.Delete(TemporarySnapshotFile);
		foreach (KeyValuePair<string, string?> originalValue in _originalValues)
		{
			Environment.SetEnvironmentVariable(originalValue.Key, originalValue.Value);
		}
	}
}
