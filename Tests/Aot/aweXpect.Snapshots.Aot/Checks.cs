using System;
using System.IO;
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
		new("an object matching its snapshot passes",
			() => ShouldPass(ObjectMatchesItsSnapshot)),
		new("an object with ignored and masked members matching its snapshot passes",
			() => ShouldPass(ObjectWithRulesMatchesItsSnapshot)),
	];

	/// <remarks>
	///     The formatter offers the registered members to the member and type rules, so that they also apply without
	///     reflection.
	/// </remarks>
	private static async Task ObjectWithRulesMatchesItsSnapshot()
		=> await That(new Shipment
			{
				Id = 7,
				Status = ShipmentStatus.Sent,
				Parcels = [new Parcel { Weight = 1.25m, }, new Parcel { Weight = 3m, },],
			}).MatchesSnapshot()
			.Ignoring.Member("Status")
			.Masking.Member("Id")
			.Masking.Type<decimal>("{weight}")
			.AcceptingChanges(false);

	/// <remarks>
	///     The members are only written when the source generator registered them for the call site, as this project
	///     disables the reflection fallback like a trimmed application. Accepting is disabled, so that a difference fails
	///     instead of replacing the committed snapshot.
	/// </remarks>
	private static async Task ObjectMatchesItsSnapshot()
		=> await That(new Shipment
		{
			Id = 7,
			Status = ShipmentStatus.Sent,
			Parcels = [new Parcel { Weight = 1.25m, }, new Parcel { Weight = 3m, },],
		}).MatchesSnapshot().AcceptingChanges(false);

	/// <remarks>
	///     The pending snapshot is deleted by the path on the last line of the failure, as the path of this file is
	///     mapped by the deterministic build. Accepting is disabled, as it would replace the committed snapshot and name
	///     it on the last line instead.
	/// </remarks>
	private static async Task DiffersFromItsSnapshot()
	{
		try
		{
			await That("snapshot differs").MatchesSnapshot().AcceptingChanges(false);
		}
		catch (FailException exception)
		{
			File.Delete(exception.Message.Substring(exception.Message.LastIndexOf('\n') + 1).Trim());
			throw;
		}
	}

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
