using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using static aweXpect.Expect;

namespace aweXpect.Snapshots.Aot;

internal static class Checks
{
	public static readonly Check[] All =
	[
		new("a string matching its snapshot passes",
			() => ShouldPass(MatchesItsSnapshot)),
		new("a string differing from its snapshot fails and names the difference",
			() => ShouldFail(DiffersFromItsSnapshot,
				"Checks.DiffersFromItsSnapshot.snapshot.txt", "differs at index 9")),
	];

	private static async Task DiffersFromItsSnapshot()
	{
		try
		{
			await That("snapshot differs").MatchesSnapshot();
		}
		finally
		{
			File.Delete(GetSnapshotPath($"Checks.{nameof(DiffersFromItsSnapshot)}.snapshot.pending.txt"));
		}
	}

	private static string GetSnapshotPath(string fileName, [CallerFilePath] string callerFilePath = "")
		=> Path.Combine(Path.GetDirectoryName(callerFilePath)!, fileName);

	private static async Task MatchesItsSnapshot()
		=> await That("snapshot content").MatchesSnapshot();

	private static async Task<string?> ShouldFail(Func<Task> act, params string[] parts)
	{
		try
		{
			await act();
		}
		catch (FailException exception)
		{
			string? missing = Array.Find(parts, part => !exception.Message.Contains(part, StringComparison.Ordinal));
			return missing is null ? null : $"message lacks \"{missing}\": {exception.Message}";
		}
		catch (Exception exception)
		{
			return
				$"threw {exception.GetType().FullName} instead of {typeof(FailException).FullName}: {exception.Message}";
		}

		return "did not throw";
	}

	private static async Task<string?> ShouldPass(Func<Task> act)
	{
		try
		{
			await act();
			return null;
		}
		catch (Exception exception)
		{
			return $"threw {exception.GetType().FullName}: {exception.Message}";
		}
	}
}
