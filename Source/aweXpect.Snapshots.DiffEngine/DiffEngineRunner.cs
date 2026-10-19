using System.Threading.Tasks;
using DiffEngine;

namespace aweXpect.Snapshots;

/// <summary>
///     Launches and closes diff tools with the <see cref="DiffRunner" /> of DiffEngine.
/// </summary>
/// <remarks>
///     <see cref="DiffRunner.Disabled" /> covers the opt-outs of DiffEngine, e.g. the environment variable
///     <c>DiffEngine_Disabled</c> and its detection of build servers.
/// </remarks>
internal sealed class DiffEngineRunner : IDiffRunner
{
	public static readonly DiffEngineRunner Instance = new();

	private DiffEngineRunner()
	{
	}

	public bool IsDisabled => DiffRunner.Disabled;

	public Task LaunchAsync(string receivedFile, string snapshotFile)
		=> DiffRunner.LaunchAsync(receivedFile, snapshotFile);

	public void Kill(string receivedFile, string snapshotFile)
		=> DiffRunner.Kill(receivedFile, snapshotFile);
}
