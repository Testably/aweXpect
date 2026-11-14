using System.Collections.Generic;
using System.IO;
using aweXpect.Customization;

namespace aweXpect.Snapshots.DiffEngine.Tests.TestHelpers;

/// <summary>
///     A temporary directory that stands in for the directory of a test file, and the temporary files of its snapshot.
/// </summary>
/// <remarks>
///     With a diff runner, the reporter is set for the expectations and the environment variables of build servers are
///     cleared, as the reporters are never called on a build server. The environment is shared by the whole process, so
///     these tests must not run in parallel.
/// </remarks>
internal sealed class TestScope : IDisposable
{
	public const string MemberName = "MyTest";

	private static readonly string[] BuildServerVariables = ["CI", "TF_BUILD", "JENKINS_URL", "TEAMCITY_VERSION",];

	private readonly Dictionary<string, string?> _originalValues = new();
	private readonly CustomizationLifetime? _reporters;

	public TestScope()
	{
		Directory.CreateDirectory(Path);
		(TemporaryReceivedFile, TemporarySnapshotFile) = DiffEngineReporter.GetTemporaryFiles(Id);
	}

	public TestScope(RecordingDiffRunner diffRunner) : this()
	{
		foreach (string variable in BuildServerVariables)
		{
			_originalValues[variable] = Environment.GetEnvironmentVariable(variable);
			Environment.SetEnvironmentVariable(variable, null);
		}

		_reporters = Customize.aweXpect.Snapshots().Reporters.Set([new DiffEngineReporter(diffRunner),]);
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
		_reporters?.Dispose();
		Directory.Delete(Path, true);
		// File.Delete throws when the directory is missing, which only a mismatch outside of the file store creates.
		if (Directory.Exists(DiffEngineReporter.TemporaryDirectory))
		{
			File.Delete(TemporaryReceivedFile);
			File.Delete(TemporarySnapshotFile);
		}
		foreach (KeyValuePair<string, string?> originalValue in _originalValues)
		{
			Environment.SetEnvironmentVariable(originalValue.Key, originalValue.Value);
		}
	}
}
