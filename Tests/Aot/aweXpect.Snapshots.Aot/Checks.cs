using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using aweXpect.Customization;
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
		new("a string with ignored lines matching its snapshot passes",
			() => ShouldPass(StringWithLineRulesMatchesItsSnapshot)),
		new("an object matching its snapshot passes",
			() => ShouldPass(ObjectMatchesItsSnapshot)),
		new("an object with ignored and masked members matching its snapshot passes",
			() => ShouldPass(ObjectWithRulesMatchesItsSnapshot)),
		new("a record matching its snapshot is written member-wise",
			() => ShouldPass(RecordMatchesItsSnapshot)),
		new("a snapshot named after arguments matching its snapshot passes",
			() => ShouldPass(() => NamedAfterMatchesItsSnapshot("Abbey Road", 1969))),
		new("a string unique for the target framework matches the snapshot of its target framework",
			() => ShouldPass(MatchesItsSnapshotForTheTargetFramework)),
		new("a snapshot unique for the operating system and architecture is named after them",
			() => ShouldFail(IsMissingForTheEnvironment,
				$"Checks.IsMissingForTheEnvironment@{ExpectedOperatingSystem()}@{ExpectedArchitecture()}.snapshot.txt",
				"the snapshot did not exist")),
		new("paths of the environment in a string of an object are masked",
			() => ShouldPass(PathsAreMasked)),
		new("every snapshot is used by one of the checks above",
			() => ShouldPass(EverySnapshotIsUsed)),
	];

	/// <remarks>
	///     A record is only written member-wise instead of through its compiler-generated <c>ToString()</c>, which would
	///     write the list as its type name, when the source generator of aweXpect marks it as a record, as this project
	///     disables the reflection fallback.
	/// </remarks>
	private static async Task RecordMatchesItsSnapshot()
		=> await That(new Album("Abbey Road", 12.99m, ["Come Together", "Something",]))
			.MatchesSnapshot().AcceptingChanges(false);

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

	/// <remarks>
	///     The snapshots are listed below the recorded project directory and matched with the mapped paths of the
	///     deterministic build. Accepting is disabled, so that a snapshot is never deleted here.
	/// </remarks>
	private static async Task EverySnapshotIsUsed()
	{
		using CustomizationLifetime _ = Customize.aweXpect.Snapshots().AcceptChanges.Set(false);
		await OrphanedSnapshots.CheckAsync(true);
	}

	/// <remarks>
	///     The paths differ between machines, and the formatter escapes their backslashes on Windows, so the committed
	///     snapshot only matches everywhere with the placeholders. The rest of each path uses <c>/</c>, as it keeps its
	///     own separators.
	/// </remarks>
	private static async Task PathsAreMasked()
	{
		string projectDirectory = typeof(Checks).Assembly.GetCustomAttributes<AssemblyMetadataAttribute>()
			.Single(attribute => attribute.Key == "aweXpect.Snapshots.ProjectDirectory").Value!;
		await That(new Album($"{Trim(projectDirectory)}/albums.json", 12.99m,
			[
				$"{Trim(AppContext.BaseDirectory)}/abbey.tmp",
				$"{Trim(Path.GetTempPath())}/let-it-be.tmp",
				$"{Trim(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile))}/Music",
			])).MatchesSnapshot().AcceptingChanges(false);

		static string Trim(string path) => path.TrimEnd('\\', '/');
	}

	private static async Task MatchesItsSnapshot()
		=> await That("snapshot content").MatchesSnapshot();

	private static async Task NamedAfterMatchesItsSnapshot(string title, int year)
		=> await That($"{title} ({year})").MatchesSnapshot().NamedAfter(title, year).AcceptingChanges(false);

	/// <remarks>
	///     The target framework is recorded by the build targets, so that it is found without reflection.
	/// </remarks>
	private static async Task MatchesItsSnapshotForTheTargetFramework()
		=> await That("snapshot content for net10.0").MatchesSnapshot().UniqueFor.TargetFramework()
			.AcceptingChanges(false);

	/// <remarks>
	///     No snapshot is committed, as it would depend on the machine; the pending snapshot is deleted again by the path
	///     on the last line of the failure.
	/// </remarks>
	private static async Task IsMissingForTheEnvironment()
	{
		try
		{
			await That("snapshot content for this machine").MatchesSnapshot()
				.UniqueFor.Architecture().UniqueFor.OperatingSystem()
				.AcceptingChanges(false);
		}
		catch (FailException exception)
		{
			File.Delete(exception.Message.Substring(exception.Message.LastIndexOf('\n') + 1).Trim());
			throw;
		}
	}

	private static string ExpectedArchitecture()
		=> RuntimeInformation.ProcessArchitecture switch
		{
			Architecture.X64 => "x64",
			Architecture.X86 => "x86",
			Architecture.Arm64 => "arm64",
			Architecture.Arm => "arm",
			_ => "unexpected architecture",
		};

	private static string ExpectedOperatingSystem()
		=> RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "windows"
			: RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? "osx"
			: "linux";

	/// <remarks>
	///     The line rules match with the string options of aweXpect, also as a wildcard or regex pattern.
	/// </remarks>
	private static async Task StringWithLineRulesMatchesItsSnapshot()
		=> await That($"Abbey Road\r\nELAPSED {DateTime.Now.Ticks} ms\r\nTrack 2: Help! (Live)\r\n09:30 Let It Be\r\n\r\nBack in Black")
			.MatchesSnapshot()
			.Ignoring.LinesContaining("elapsed").IgnoringCase()
			.Ignoring.LinesContaining("Track ?: *Live*").AsWildcard()
			.Ignoring.LinesContaining(@"^\d{2}:\d{2} ").AsRegex()
			.Ignoring.Lines(line => line.Length == 0)
			.AcceptingChanges(false);

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
