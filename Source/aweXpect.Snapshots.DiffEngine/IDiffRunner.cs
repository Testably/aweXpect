using System.Threading.Tasks;

namespace aweXpect.Snapshots;

/// <summary>
///     Launches and closes diff tools, so that the tests of <see cref="DiffEngineReporter" /> never start one.
/// </summary>
internal interface IDiffRunner
{
	bool IsDisabled { get; }

	Task LaunchAsync(string receivedFile, string snapshotFile);

	void Kill(string receivedFile, string snapshotFile);
}
