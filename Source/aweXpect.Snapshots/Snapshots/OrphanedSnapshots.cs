using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace aweXpect.Snapshots;

/// <summary>
///     Finds the snapshots that no test uses any more, e.g. after a test or a named case was renamed or deleted.
/// </summary>
public static class OrphanedSnapshots
{
	/// <summary>
	///     Fails when the store lists a snapshot that no <c>MatchesSnapshot()</c> of this test run refers to, and deletes
	///     such snapshots while accepting changes.
	/// </summary>
	/// <param name="force">
	///     Checks also when no build server is detected, e.g. locally after running all tests.
	/// </param>
	/// <param name="cancellationToken">The cancellation of the check.</param>
	/// <remarks>
	///     Call it once after all tests of the test project have run, in the teardown of the test framework.<br />
	///     It only checks on a build server (when <c>CI</c>, <c>TF_BUILD</c>, <c>JENKINS_URL</c> or
	///     <c>TEAMCITY_VERSION</c> is set), or when <paramref name="force" /> is <see langword="true" />, as a test run
	///     that only runs some of the tests, e.g. one test in the IDE, cannot be told apart from a complete one, and would
	///     report the snapshots of the others.<br />
	///     While accepting changes (<c>Customize.aweXpect.Snapshots().AcceptChanges</c> or the environment variable
	///     <c>AWEXPECT_ACCEPT_SNAPSHOTS</c>, which is ignored on a build server), the orphaned snapshots are deleted, and
	///     it still fails, listing them, so that a deletion never goes unnoticed.<br />
	///     A snapshot counts as used as soon as <c>MatchesSnapshot()</c> is called for it, also when it is not compared,
	///     but a test that is skipped or that fails before it calls <c>MatchesSnapshot()</c> leaves its snapshot unused.
	/// </remarks>
	public static async Task CheckAsync(bool force = false, CancellationToken cancellationToken = default)
	{
		if (!force && !AcceptEnvironment.IsBuildServer())
		{
			return;
		}

		SnapshotSettings settings = SnapshotSettings.Read();
		IReadOnlyList<SnapshotId>? snapshots;
		try
		{
			snapshots = await settings.Store.ListAsync(cancellationToken);
		}
		catch (Exception exception) when (exception is not OperationCanceledException ||
		                                  !cancellationToken.IsCancellationRequested)
		{
			Fail.Test($"{Expectation}, but the snapshots could not be listed: {exception.Message}", exception);
			return;
		}

		if (snapshots is null)
		{
			Fail.Test(
				$"{Expectation}, but the snapshot store {settings.Store.GetType().Name} cannot list its snapshots, see SnapshotStore.ListAsync.");
			return;
		}

		string[] orphans = await GetOrphans(settings, snapshots, cancellationToken);
		if (orphans.Length > 0)
		{
			Fail.Test(
				$"{Expectation}, but {(orphans.Length == 1 ? "1 snapshot was" : $"{orphans.Length} snapshots were")} not used by any test:{Environment.NewLine}{string.Join(Environment.NewLine, orphans)}");
		}
	}

	private const string Expectation = "Expected that every snapshot is used by a test of this run";

	/// <summary>
	///     Returns a line for each orphaned snapshot, in order, which also says whether it was deleted.
	/// </summary>
	private static async Task<string[]> GetOrphans(SnapshotSettings settings, IReadOnlyList<SnapshotId> snapshots,
		CancellationToken cancellationToken)
	{
		bool isDeleting = settings.AcceptChanges ?? AcceptEnvironment.IsAccepting();
		List<string> lines = new();
		foreach ((SnapshotId id, string description) in snapshots
			         .Where(id => !ClaimedSnapshots.IsClaimed(id))
			         .Distinct(ClaimedSnapshots.Comparer)
			         .Select(id => (id, settings.Store.Describe(id)))
			         .OrderBy(orphan => orphan.Item2, StringComparer.Ordinal))
		{
			StringBuilder line = new StringBuilder("  ").Append(description);
			if (isDeleting)
			{
				try
				{
					await settings.Store.DeleteAsync(id, cancellationToken);
					line.Append(" (deleted)");
				}
				catch (Exception exception) when (exception is not OperationCanceledException ||
				                                  !cancellationToken.IsCancellationRequested)
				{
					line.Append(" (could not be deleted: ").Append(exception.Message).Append(')');
				}
			}

			lines.Add(line.ToString());
		}

		return lines.ToArray();
	}
}
