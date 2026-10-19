using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
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
				"Checks.DiffersFromItsSnapshot.awexpect-snapshot.txt", "differs at index 9")),
		new("a string with ignored lines matching its snapshot passes",
			() => ShouldPass(StringWithLineRulesMatchesItsSnapshot)),
		new("an object matching its snapshot passes",
			() => ShouldPass(ObjectMatchesItsSnapshot)),
		new("an object with ignored and masked members matching its snapshot passes",
			() => ShouldPass(ObjectWithRulesMatchesItsSnapshot)),
		new("an object with members selected by their declaring type and empty collections left out matching its snapshot passes",
			() => ShouldPass(ObjectWithDeclaredMembersAndEmptyCollectionsMatchesItsSnapshot)),
		new("an object with converted members matching its snapshot passes",
			() => ShouldPass(ObjectWithConvertedMembersMatchesItsSnapshot)),
		new("a record matching its snapshot is written member-wise",
			() => ShouldPass(RecordMatchesItsSnapshot)),
		new("a framework exception matching its snapshot is written member-wise",
			() => ShouldPass(FrameworkExceptionMatchesItsSnapshot)),
		new("a custom exception matching its snapshot is written without its own members",
			() => ShouldPass(CustomExceptionMatchesItsSnapshot)),
		new("a snapshot named after arguments matching its snapshot passes",
			() => ShouldPass(() => NamedAfterMatchesItsSnapshot("Abbey Road", 1969))),
		new("a string unique for the target framework matches the snapshot of its target framework",
			() => ShouldPass(MatchesItsSnapshotForTheTargetFramework)),
		new("a snapshot unique for the operating system and architecture is named after them",
			() => ShouldFail(IsMissingForTheEnvironment,
				$"Checks.IsMissingForTheEnvironment@{ExpectedOperatingSystem()}@{ExpectedArchitecture()}.awexpect-snapshot.txt",
				"the snapshot did not exist")),
		new("a string with the extension json matching its snapshot passes",
			() => ShouldPass(JsonMatchesItsSnapshot)),
		new("binary data matching its snapshot passes",
			() => ShouldPass(CoverArtMatchesItsSnapshot)),
		new("a stream matching its snapshot passes",
			() => ShouldPass(LinerNotesMatchesItsSnapshot)),
		new("paths of the environment in a string of an object are masked",
			() => ShouldPass(PathsAreMasked)),
		new("a reporter is told about a match",
			() => ShouldPass(() => WithoutBuildServer(ReporterIsToldAboutAMatch))),
		new("a reporter is told about the recorded mismatch with the files of the store",
			() => ShouldPass(() => WithoutBuildServer(ReporterIsToldAboutTheRecordedMismatch))),
		new("a comparer of the expectation or for an extension decides whether the subject matches",
			() => ShouldPass(ComparersDecideTheMatch)),
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
			.MatchesSnapshot().Accepting(SnapshotAcceptance.None);

	/// <remarks>
	///     The members of the framework exceptions are read without reflection. The message does not name the actual
	///     value, as it would on a line of its own with the line break of the operating system.
	/// </remarks>
	private static async Task FrameworkExceptionMatchesItsSnapshot()
		=> await That(() => throw new ArgumentOutOfRangeException("playCount", "Play count must be positive.")
			{
				Data =
				{
					["Track"] = "Come Together",
				},
			}).Throws<ArgumentOutOfRangeException>()
			.Which.MatchesSnapshot().Accepting(SnapshotAcceptance.None);

	/// <remarks>
	///     The source generator of aweXpect registers no type derived from <see cref="Exception" />, as
	///     <see cref="Exception.TargetSite" /> requires unreferenced code, so without the reflection fallback that this
	///     project disables, the own members of a custom exception, here <see cref="RecordPlayerException.Side" />, are
	///     left out, while its message and inner exception are still written.
	/// </remarks>
	private static async Task CustomExceptionMatchesItsSnapshot()
		=> await That(() => throw new RecordPlayerException("The record is stuck", 3,
				new InvalidOperationException("The record player is not running")))
			.Throws<RecordPlayerException>()
			.Which.MatchesSnapshot().Accepting(SnapshotAcceptance.None);

	/// <remarks>
	///     The formatter offers the registered members to the member and value rules, so that they also apply without
	///     reflection.
	/// </remarks>
	private static async Task ObjectWithRulesMatchesItsSnapshot()
		=> await That(new Shipment
			{
				Id = 7,
				Status = ShipmentStatus.Sent,
				Parcels = [new Parcel { Weight = 1.25m, }, new Parcel { Weight = 3m, },],
			}).MatchesSnapshot()
			.For.Members().WithName(nameof(Shipment.Status)).Ignore()
			.For.Members().WithName(nameof(Shipment.Id)).Mask()
			.For.Values().OfType<decimal>().Mask("{weight}")
			.Accepting(SnapshotAcceptance.None);

	/// <remarks>
	///     The declaring types are compared without reflection, and the empty collections are recognized without
	///     reflection, including a set of a value type, which is no non-generic collection.
	/// </remarks>
	private static async Task ObjectWithDeclaredMembersAndEmptyCollectionsMatchesItsSnapshot()
		=> await That(new Playlist
			{
				Id = 1,
				Name = "Road Trip",
				Songs = [new Song { Id = 7, Title = "Highway to Hell", }, new Song { Id = 9, Title = "Let It Be", },],
			}).MatchesSnapshot()
			.For.Members().WhichAreEmptyCollections().Ignore()
			.For.Members().DeclaredIn<Playlist>().WithName(nameof(Playlist.Id)).Ignore()
			.For.Members().DeclaredIn<Song>().WithName(nameof(Song.Id)).Mask()
			.Accepting(SnapshotAcceptance.None);

	/// <remarks>
	///     The converters are only called, never compiled, and the anonymous type that a converter returns is written
	///     member-wise, as the source generator registers it at the call site.
	/// </remarks>
	private static async Task ObjectWithConvertedMembersMatchesItsSnapshot()
		=> await That(new Concert
			{
				Date = new DateTime(1969, 1, 30, 12, 30, 0),
				Duration = TimeSpan.FromMinutes(42),
				Venue = "Apple Corps rooftop",
			}).MatchesSnapshot()
			.For.Members().DeclaredIn<Concert>().WithName(nameof(Concert.Date)).OfType<DateTime>()
			.Convert(date => new { date.Year, date.Month, })
			.For.Members().OfType<TimeSpan>().Convert(duration => Math.Round(duration.TotalMinutes))
			.Accepting(SnapshotAcceptance.None);

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
		}).MatchesSnapshot().Accepting(SnapshotAcceptance.None);

	/// <remarks>
	///     The pending snapshot is deleted by the path on the last line of the failure, as the path of this file is
	///     mapped by the deterministic build. Accepting is disabled, as it would replace the committed snapshot and name
	///     it on the last line instead.
	/// </remarks>
	private static async Task DiffersFromItsSnapshot()
	{
		try
		{
			await That("snapshot differs").MatchesSnapshot().Accepting(SnapshotAcceptance.None);
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
		using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Acceptance.Set(SnapshotAcceptance.None);
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
			])).MatchesSnapshot().Accepting(SnapshotAcceptance.None);

		static string Trim(string path) => path.TrimEnd('\\', '/');
	}

	private static async Task MatchesItsSnapshot()
		=> await That("snapshot content").MatchesSnapshot();

	private static async Task ReporterIsToldAboutAMatch()
	{
		RecordingReporter reporter = new();
		using (Customize.aweXpect.Snapshots().Reporters.Set([reporter,]))
		{
			await That("reported match").MatchesSnapshot().Accepting(SnapshotAcceptance.None);
		}

		await That(reporter.Match?.Id.MemberName).IsEqualTo(nameof(ReporterIsToldAboutAMatch));
	}

	/// <remarks>
	///     No snapshot is committed, so that the pending snapshot is recorded; it is deleted again by the path that the
	///     store reports.
	/// </remarks>
	private static async Task ReporterIsToldAboutTheRecordedMismatch()
	{
		RecordingReporter reporter = new();
		using (Customize.aweXpect.Snapshots().Reporters.Set([reporter,]))
		{
			try
			{
				await That("reported mismatch").MatchesSnapshot().Accepting(SnapshotAcceptance.None);
			}
			catch (FailException)
			{
				// The snapshot is missing on purpose, see the remarks.
			}
		}

		SnapshotMismatch mismatch = reporter.Mismatch ??
		                            throw new InvalidOperationException("the reporter was not told about the mismatch");
		string pendingFile = ((IFileSnapshotStore)mismatch.Store).GetPendingFile(mismatch.Id);
		bool isRecorded = File.Exists(pendingFile);
		File.Delete(pendingFile);
		await That(isRecorded).IsTrue();
		await That(mismatch.Snapshot).IsNull();
		await That(mismatch.Actual.Text).IsEqualTo("reported mismatch");
	}

	/// <remarks>
	///     The snapshot is kept in memory, so that no snapshot is committed that only differs in its case.
	/// </remarks>
	private static async Task ComparersDecideTheMatch()
	{
		using (Customize.aweXpect.Snapshots().Store.Set(new FixedSnapshotStore("Abbey Road")))
		using (Customize.aweXpect.Snapshots().Comparers.Set([
			       SnapshotComparer.ForExtension("bin", new IgnoringCaseComparer()),
		       ]))
		{
			await That("ABBEY ROAD").MatchesSnapshot().ComparedWith(new IgnoringCaseComparer())
				.Accepting(SnapshotAcceptance.None);
			await That(Encoding.UTF8.GetBytes("abbey road")).MatchesSnapshot().WithExtension("bin").Named("cover")
				.Accepting(SnapshotAcceptance.None);
		}
	}

	private sealed class IgnoringCaseComparer : SnapshotComparer
	{
		public override ValueTask<SnapshotComparison> CompareAsync(SnapshotComparisonContext context,
			CancellationToken cancellationToken)
			=> new(string.Equals(Encoding.UTF8.GetString(context.Snapshot.Bytes).TrimEnd(),
				Encoding.UTF8.GetString(context.Actual.Bytes).TrimEnd(), StringComparison.OrdinalIgnoreCase)
				? SnapshotComparison.Equal
				: SnapshotComparison.Different("the text differs in more than its case"));
	}

	/// <summary>
	///     Returns the <paramref name="snapshot" /> for every id and writes nothing.
	/// </summary>
	private sealed class FixedSnapshotStore(string snapshot) : SnapshotStore
	{
		public override ValueTask<byte[]?> ReadAsync(SnapshotId id, CancellationToken cancellationToken)
			=> new(Encoding.UTF8.GetBytes(snapshot));

		public override ValueTask RecordMismatchAsync(SnapshotId id, byte[] actual,
			CancellationToken cancellationToken)
			=> default;

		public override ValueTask AcceptAsync(SnapshotId id, byte[] actual, CancellationToken cancellationToken)
			=> default;
	}

	/// <summary>
	///     Runs the <paramref name="act" /> with the environment variables of build servers cleared, as the reporters are
	///     never called on a build server.
	/// </summary>
	private static async Task WithoutBuildServer(Func<Task> act)
	{
		string[] variables = ["CI", "TF_BUILD", "JENKINS_URL", "TEAMCITY_VERSION",];
		string?[] values = variables.Select(Environment.GetEnvironmentVariable).ToArray();
		try
		{
			foreach (string variable in variables)
			{
				Environment.SetEnvironmentVariable(variable, null);
			}

			await act();
		}
		finally
		{
			for (int i = 0; i < variables.Length; i++)
			{
				Environment.SetEnvironmentVariable(variables[i], values[i]);
			}
		}
	}

	private sealed class RecordingReporter : SnapshotReporter
	{
		public SnapshotMatch? Match { get; private set; }
		public SnapshotMismatch? Mismatch { get; private set; }

		public override ValueTask OnMatchAsync(SnapshotMatch match, CancellationToken cancellationToken)
		{
			Match = match;
			return default;
		}

		public override ValueTask OnMismatchAsync(SnapshotMismatch mismatch, CancellationToken cancellationToken)
		{
			Mismatch = mismatch;
			return default;
		}
	}

	/// <remarks>
	///     The committed <c>.awexpect-snapshot.json</c> is also listed back by the orphan check below.
	/// </remarks>
	private static async Task JsonMatchesItsSnapshot()
		=> await That("{ \"title\": \"Abbey Road\", \"year\": 1969 }").MatchesSnapshot().WithExtension("json")
			.Accepting(SnapshotAcceptance.None);

	/// <remarks>
	///     The committed <c>.awexpect-snapshot.bin</c> contains both line break styles, which binary data keeps
	///     unchanged.
	/// </remarks>
	private static async Task CoverArtMatchesItsSnapshot()
		=> await That(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 0x00, })
			.MatchesSnapshot().WithExtension("bin").Accepting(SnapshotAcceptance.None);

	private static async Task LinerNotesMatchesItsSnapshot()
	{
		using MemoryStream linerNotes = new([0x25, 0x50, 0x44, 0x46, 0x0A,]);
		await That(linerNotes).MatchesSnapshot().WithExtension("bin").Accepting(SnapshotAcceptance.None);
	}

	private static async Task NamedAfterMatchesItsSnapshot(string title, int year)
		=> await That($"{title} ({year})").MatchesSnapshot().NamedAfter(title, year).Accepting(SnapshotAcceptance.None);

	/// <remarks>
	///     The target framework is recorded by the build targets, so that it is found without reflection.
	/// </remarks>
	private static async Task MatchesItsSnapshotForTheTargetFramework()
		=> await That("snapshot content for net10.0").MatchesSnapshot().UniqueFor.TargetFramework()
			.Accepting(SnapshotAcceptance.None);

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
				.Accepting(SnapshotAcceptance.None);
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
			.For.Lines().Containing("elapsed").IgnoringCase().Ignore()
			.For.Lines().Containing("Track ?: *Live*").AsWildcard().Ignore()
			.For.Lines().Containing(@"^\d{2}:\d{2} ").AsRegex().Ignore()
			.For.Lines().Which(line => line.Length == 0).Ignore()
			.Accepting(SnapshotAcceptance.None);

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
