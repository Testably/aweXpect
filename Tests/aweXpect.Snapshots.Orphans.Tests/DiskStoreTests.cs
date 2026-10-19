using System.IO;
using aweXpect.Customization;
using aweXpect.Snapshots.Orphans.Tests.TestHelpers;

namespace aweXpect.Snapshots.Orphans.Tests;

/// <summary>
///     Every test checks the snapshots of a temporary project, but as all recorded project directories are listed, this
///     test project must not contain snapshots of its own.
/// </summary>
public sealed class DiskStoreTests
{
	[Test]
	public async Task WhenEverySnapshotIsUsed_ShouldSucceed()
	{
		using TemporaryProject project = new();
		project.CreateFile("AlbumTests.cs");
		project.CreateFile("AlbumTests.Describes.awexpect-snapshot.txt", "Abbey Road");
		await That("Abbey Road").MatchesSnapshot(project.CallerFilePath("AlbumTests.cs"), "Describes");

		async Task Act() => await OrphanedSnapshots.CheckAsync(true);

		await That(Act).DoesNotThrow();
	}

	[Test]
	public async Task WhenASnapshotIsNotUsed_ShouldReportItsFileWithoutDeletingIt()
	{
		using TemporaryProject project = new();
		project.CreateFile("AlbumTests.cs");
		project.CreateFile("AlbumTests.Describes.awexpect-snapshot.txt", "Abbey Road");

		async Task Act() => await OrphanedSnapshots.CheckAsync(true);

		await That(Act).Throws<FailException>()
			.WithMessage($"""
			              Expected that every snapshot is used by a test of this run, but 1 snapshot was not used by any test:
			                {project.GetFullPath("AlbumTests.Describes.awexpect-snapshot.txt")}
			              """);
		await That(project.Exists("AlbumTests.Describes.awexpect-snapshot.txt")).IsTrue();
	}

	[Test]
	public async Task WhenNamedSnapshotsWithEscapedNamesAreUsed_ShouldReadTheirNamesBack()
	{
		using TemporaryProject project = new();
		project.CreateFile("AlbumTests.cs");
		project.CreateFile("AlbumTests.Describes.Abbey Road.awexpect-snapshot.txt", "Abbey Road");
		project.CreateFile("AlbumTests.Describes.10%3A30%2FLet It Be%2E1970.awexpect-snapshot.txt", "Let It Be");
		await That("Abbey Road").MatchesSnapshot(project.CallerFilePath("AlbumTests.cs"), "Describes")
			.Named("Abbey Road");
		await That("Let It Be").MatchesSnapshot(project.CallerFilePath("AlbumTests.cs"), "Describes")
			.Named("10:30/Let It Be.1970");

		async Task Act() => await OrphanedSnapshots.CheckAsync(true);

		await That(Act).DoesNotThrow();
	}

	[Test]
	public async Task WhenSnapshotsUniqueForThisEnvironmentAreUsed_ShouldReadTheirVariantsBack()
	{
		using TemporaryProject project = new();
		project.CreateFile("AlbumTests.cs");
		project.CreateFile($"AlbumTests.Describes@{TemporaryProject.Variant}.awexpect-snapshot.txt", "Abbey Road");
		project.CreateFile($"AlbumTests.Describes.AC%40DC@{TemporaryProject.TargetFramework}.awexpect-snapshot.txt", "AC/DC");
		await That("Abbey Road").MatchesSnapshot(project.CallerFilePath("AlbumTests.cs"), "Describes")
			.UniqueFor.Architecture().UniqueFor.OperatingSystem().UniqueFor.TargetFramework();
		await That("AC/DC").MatchesSnapshot(project.CallerFilePath("AlbumTests.cs"), "Describes")
			.Named("AC@DC").UniqueFor.TargetFramework();

		async Task Act() => await OrphanedSnapshots.CheckAsync(true);

		await That(Act).DoesNotThrow()
			.Because("an '@' in a name is escaped, so the variant starts at the first '@'");
	}

	[Test]
	public async Task WhenASnapshotUniqueForThisEnvironmentIsNotUsed_ShouldReportAndDeleteIt()
	{
		using TemporaryProject project = new();
		project.CreateFile("AlbumTests.cs");
		project.CreateFile($"AlbumTests.Describes@{TemporaryProject.Variant}.awexpect-snapshot.txt", "Abbey Road");
		using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Acceptance.Set(SnapshotAcceptance.All);

		async Task Act() => await OrphanedSnapshots.CheckAsync(true);

		await That(Act).Throws<FailException>()
			.WithMessage($"""
			              Expected that every snapshot is used by a test of this run, but 1 snapshot was not used by any test:
			                {project.GetFullPath($"AlbumTests.Describes@{TemporaryProject.Variant}.awexpect-snapshot.txt")} (deleted)
			              """);
		await That(project.Exists($"AlbumTests.Describes@{TemporaryProject.Variant}.awexpect-snapshot.txt")).IsFalse();
	}

	[Test]
	public async Task WhenTheTestFileOfASnapshotUniqueForThisEnvironmentWasDeleted_ShouldReportAndDeleteIt()
	{
		using TemporaryProject project = new();
		project.CreateFile($"AlbumTests.Describes.Let It Be@{TemporaryProject.Variant}.awexpect-snapshot.txt", "Let It Be");
		using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Acceptance.Set(SnapshotAcceptance.All);

		async Task Act() => await OrphanedSnapshots.CheckAsync(true);

		await That(Act).Throws<FailException>()
			.WithMessage("*1 snapshot was not used by any test:*(deleted)").AsWildcard()
			.Because("the dots in the target framework are part of the variant, not of the method or the name");
		await That(project.Exists($"AlbumTests.Describes.Let It Be@{TemporaryProject.Variant}.awexpect-snapshot.txt")).IsFalse();
	}

	[Test]
	[Arguments("AlbumTests.Describes@net35.awexpect-snapshot.txt")]
	[Arguments("AlbumTests.Describes@beos.awexpect-snapshot.txt")]
	[Arguments("AlbumTests.Describes@{variant}@sparc.awexpect-snapshot.txt")]
	[Arguments("AlbumTests.Describes.Abbey Road@net35@{variant}.awexpect-snapshot.pending.txt")]
	public async Task WhenASnapshotIsUniqueForAnotherEnvironment_ShouldNeverReportIt(string fileName)
	{
		fileName = fileName.Replace("{variant}", TemporaryProject.Variant);
		using TemporaryProject project = new();
		project.CreateFile("AlbumTests.cs");
		project.CreateFile(fileName, "Abbey Road");
		using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Acceptance.Set(SnapshotAcceptance.All);

		async Task Act() => await OrphanedSnapshots.CheckAsync(true);

		await That(Act).DoesNotThrow()
			.Because("only a test run in the environment of the snapshot can tell whether it is used");
		await That(project.Exists(fileName)).IsTrue();
	}

	[Test]
	public async Task WhenSnapshotsWithAnotherExtensionAreUsed_ShouldReadTheirExtensionsBack()
	{
		using TemporaryProject project = new();
		project.CreateFile("AlbumTests.cs");
		project.CreateFile("AlbumTests.Describes.awexpect-snapshot.json", "{}");
		project.CreateFile("AlbumTests.Describes.awexpect-snapshot.pending.json", "[]");
		project.CreateFile($"AlbumTests.Describes.Abbey Road@{TemporaryProject.TargetFramework}.awexpect-snapshot.html", "<h1/>");
		await That("{}").MatchesSnapshot(project.CallerFilePath("AlbumTests.cs"), "Describes").WithExtension("json");
		await That("<h1/>").MatchesSnapshot(project.CallerFilePath("AlbumTests.cs"), "Describes")
			.Named("Abbey Road").UniqueFor.TargetFramework().WithExtension("html");

		async Task Act() => await OrphanedSnapshots.CheckAsync(true);

		await That(Act).DoesNotThrow()
			.Because("the pending file is read back as the pending file of the snapshot, not as a snapshot with the extension 'pending.json'");
	}

	[Test]
	public async Task WhenASnapshotWithAnotherExtensionIsNotUsed_ShouldReportAndDeleteItAndItsPendingFile()
	{
		using TemporaryProject project = new();
		project.CreateFile("AlbumTests.cs");
		project.CreateFile("AlbumTests.Describes.awexpect-snapshot.json", "{}");
		project.CreateFile("AlbumTests.Describes.awexpect-snapshot.pending.json", "[]");
		project.CreateFile("AlbumTests.Describes.awexpect-snapshot.txt", "Abbey Road");
		await That("Abbey Road").MatchesSnapshot(project.CallerFilePath("AlbumTests.cs"), "Describes");
		using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Acceptance.Set(SnapshotAcceptance.All);

		async Task Act() => await OrphanedSnapshots.CheckAsync(true);

		await That(Act).Throws<FailException>()
			.WithMessage($"""
			              Expected that every snapshot is used by a test of this run, but 1 snapshot was not used by any test:
			                {project.GetFullPath("AlbumTests.Describes.awexpect-snapshot.json")} (deleted)
			              """)
			.Because("the test uses the snapshot with the extension 'txt'");
		await That(project.Exists("AlbumTests.Describes.awexpect-snapshot.json")).IsFalse();
		await That(project.Exists("AlbumTests.Describes.awexpect-snapshot.pending.json")).IsFalse();
		await That(project.Exists("AlbumTests.Describes.awexpect-snapshot.txt")).IsTrue();
	}

	[Test]
	public async Task WhenBinarySnapshotsAreUsed_ShouldReadThemBack()
	{
		using TemporaryProject project = new();
		project.CreateFile("AlbumTests.cs");
		byte[] coverArt = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 0x00,];
		File.WriteAllBytes(project.GetFullPath("AlbumTests.Cover.awexpect-snapshot.png"), coverArt);
		File.WriteAllBytes(project.GetFullPath("AlbumTests.Cover.awexpect-snapshot.pending.png"), [0x00,]);
		File.WriteAllBytes(project.GetFullPath("AlbumTests.LinerNotes.awexpect-snapshot.pdf"), [0x25, 0x50, 0x44, 0x46,]);
		File.WriteAllBytes(project.GetFullPath("liner-notes.pdf"), [0x25, 0x50, 0x44, 0x46,]);
		await That(coverArt).MatchesSnapshot(project.CallerFilePath("AlbumTests.cs"), "Cover").WithExtension("png");
		await That(new FileInfo(project.GetFullPath("liner-notes.pdf")))
			.MatchesSnapshot(project.CallerFilePath("AlbumTests.cs"), "LinerNotes");

		async Task Act() => await OrphanedSnapshots.CheckAsync(true);

		await That(Act).DoesNotThrow()
			.Because("a binary snapshot is listed like any other snapshot with its extension");
	}

	[Test]
	public async Task WhenABinarySnapshotIsNotUsed_ShouldReportAndDeleteItAndItsPendingFile()
	{
		using TemporaryProject project = new();
		project.CreateFile("AlbumTests.cs");
		File.WriteAllBytes(project.GetFullPath("AlbumTests.Cover.awexpect-snapshot.png"), [0x89, 0x50, 0x4E, 0x47,]);
		File.WriteAllBytes(project.GetFullPath("AlbumTests.Cover.awexpect-snapshot.pending.png"), [0x00,]);
		using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Acceptance.Set(SnapshotAcceptance.All);

		async Task Act() => await OrphanedSnapshots.CheckAsync(true);

		await That(Act).Throws<FailException>()
			.WithMessage($"""
			              Expected that every snapshot is used by a test of this run, but 1 snapshot was not used by any test:
			                {project.GetFullPath("AlbumTests.Cover.awexpect-snapshot.png")} (deleted)
			              """);
		await That(project.Exists("AlbumTests.Cover.awexpect-snapshot.png")).IsFalse();
		await That(project.Exists("AlbumTests.Cover.awexpect-snapshot.pending.png")).IsFalse();
	}

	[Test]
	public async Task WhenOnlyAPendingFileWithAnotherExtensionIsLeft_ShouldReportAndDeleteIt()
	{
		using TemporaryProject project = new();
		project.CreateFile("AlbumTests.cs");
		project.CreateFile("AlbumTests.Describes.awexpect-snapshot.pending.xml", "<album />");
		project.CreateFile("AlbumTests.Describes.awexpect-snapshot.txt", "Abbey Road");
		await That("Abbey Road").MatchesSnapshot(project.CallerFilePath("AlbumTests.cs"), "Describes");
		using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Acceptance.Set(SnapshotAcceptance.All);

		async Task Act() => await OrphanedSnapshots.CheckAsync(true);

		await That(Act).Throws<FailException>()
			.WithMessage($"*1 snapshot was not used by any test:*  {project.GetFullPath("AlbumTests.Describes.awexpect-snapshot.pending.xml")} (deleted)")
			.AsWildcard();
		await That(project.Exists("AlbumTests.Describes.awexpect-snapshot.pending.xml")).IsFalse();
		await That(project.Exists("AlbumTests.Describes.awexpect-snapshot.txt")).IsTrue();
	}

	[Test]
	public async Task WhenTheExtensionIsSnapshot_ShouldReadItBack()
	{
		using TemporaryProject project = new();
		project.CreateFile("AlbumTests.cs");
		project.CreateFile("AlbumTests.Describes.awexpect-snapshot.snapshot", "Abbey Road");
		project.CreateFile("AlbumTests.Describes.awexpect-snapshot.pending.snapshot", "Let It Be");
		await That("Abbey Road").MatchesSnapshot(project.CallerFilePath("AlbumTests.cs"), "Describes")
			.WithExtension("snapshot");

		async Task Act() => await OrphanedSnapshots.CheckAsync(true);

		await That(Act).DoesNotThrow();
	}

	[Test]
	public async Task WhenTheNameIsTheMarker_ShouldReadItBack()
	{
		using TemporaryProject project = new();
		project.CreateFile("AlbumTests.cs");
		project.CreateFile("AlbumTests.Describes.awexpect-snapshot.awexpect-snapshot.txt", "Abbey Road");
		project.CreateFile("AlbumTests.Describes.awexpect-snapshot.awexpect-snapshot.pending.txt", "Let It Be");
		await That("Abbey Road").MatchesSnapshot(project.CallerFilePath("AlbumTests.cs"), "Describes")
			.Named("awexpect-snapshot");

		async Task Act() => await OrphanedSnapshots.CheckAsync(true);

		await That(Act).DoesNotThrow()
			.Because("only the last marker before the extension separates the name from the extension");
	}

	[Test]
	public async Task WhenTheClassNameContainsDots_ShouldReadTheMethodBack()
	{
		using TemporaryProject project = new();
		project.CreateFile("AlbumTests.Tracks.cs");
		project.CreateFile("AlbumTests.Tracks.Count.awexpect-snapshot.txt", "17");
		await That("17").MatchesSnapshot(project.CallerFilePath("AlbumTests.Tracks.cs"), "Count");

		async Task Act() => await OrphanedSnapshots.CheckAsync(true);

		await That(Act).DoesNotThrow();
	}

	[Test]
	public async Task WhenSeveralTestFilesFit_AndTheLongerOneUsesTheSnapshot_ShouldSucceed()
	{
		using TemporaryProject project = new();
		project.CreateFile("AlbumTests.cs");
		project.CreateFile("AlbumTests.Tracks.cs");
		project.CreateFile("AlbumTests.Tracks.Count.awexpect-snapshot.txt", "17");
		await That("17").MatchesSnapshot(project.CallerFilePath("AlbumTests.Tracks.cs"), "Count");

		async Task Act() => await OrphanedSnapshots.CheckAsync(true);

		await That(Act).DoesNotThrow();
	}

	[Test]
	public async Task WhenSeveralTestFilesFit_AndTheShorterOneUsesTheSnapshot_ShouldSucceed()
	{
		using TemporaryProject project = new();
		project.CreateFile("AlbumTests.cs");
		project.CreateFile("AlbumTests.Tracks.cs");
		project.CreateFile("AlbumTests.Tracks.Count.awexpect-snapshot.txt", "17");
		await That("17").MatchesSnapshot(project.CallerFilePath("AlbumTests.cs"), "Tracks").Named("Count");

		async Task Act() => await OrphanedSnapshots.CheckAsync(true);

		await That(Act).DoesNotThrow()
			.Because("the method 'Tracks' of 'AlbumTests.cs' with the name 'Count' uses the same file");
	}

	[Test]
	public async Task WhenOnlyAPendingFileIsLeft_ShouldReportAndDeleteIt()
	{
		using TemporaryProject project = new();
		project.CreateFile("AlbumTests.cs");
		project.CreateFile("AlbumTests.Describes.awexpect-snapshot.pending.txt", "Abbey Road");
		using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Acceptance.Set(SnapshotAcceptance.All);

		async Task Act() => await OrphanedSnapshots.CheckAsync(true);

		await That(Act).Throws<FailException>()
			.WithMessage($"*  {project.GetFullPath("AlbumTests.Describes.awexpect-snapshot.pending.txt")} (deleted)")
			.AsWildcard();
		await That(project.Exists("AlbumTests.Describes.awexpect-snapshot.pending.txt")).IsFalse();
	}

	[Test]
	public async Task WhenAcceptingAll_ShouldDeleteTheSnapshotAndItsPendingFileAndStillFail()
	{
		using TemporaryProject project = new();
		project.CreateFile("AlbumTests.cs");
		project.CreateFile("AlbumTests.Describes.Abbey Road.awexpect-snapshot.txt", "Abbey Road");
		project.CreateFile("AlbumTests.Describes.Let It Be.awexpect-snapshot.txt", "Let It Be");
		project.CreateFile("AlbumTests.Describes.Let It Be.awexpect-snapshot.pending.txt", "Let It Be (Remastered)");
		using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Acceptance.Set(SnapshotAcceptance.All);
		await That("Abbey Road").MatchesSnapshot(project.CallerFilePath("AlbumTests.cs"), "Describes")
			.Named("Abbey Road");

		async Task Act() => await OrphanedSnapshots.CheckAsync(true);

		await That(Act).Throws<FailException>()
			.WithMessage($"""
			              Expected that every snapshot is used by a test of this run, but 1 snapshot was not used by any test:
			                {project.GetFullPath("AlbumTests.Describes.Let It Be.awexpect-snapshot.txt")} (deleted)
			              """);
		await That(project.Exists("AlbumTests.Describes.Let It Be.awexpect-snapshot.txt")).IsFalse();
		await That(project.Exists("AlbumTests.Describes.Let It Be.awexpect-snapshot.pending.txt")).IsFalse();
		await That(project.Exists("AlbumTests.Describes.Abbey Road.awexpect-snapshot.txt")).IsTrue();
	}

	[Test]
	public async Task WhenTheTestFileWasDeleted_ShouldReportAndDeleteItsSnapshots()
	{
		using TemporaryProject project = new();
		project.CreateFile("AlbumTests.Describes.awexpect-snapshot.txt", "Abbey Road");
		project.CreateFile("AlbumTests.Describes.Let It Be.awexpect-snapshot.txt", "Let It Be");
		using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Acceptance.Set(SnapshotAcceptance.All);

		async Task Act() => await OrphanedSnapshots.CheckAsync(true);

		await That(Act).Throws<FailException>()
			.WithMessage("*2 snapshots were not used by any test:*").AsWildcard();
		await That(project.Exists("AlbumTests.Describes.awexpect-snapshot.txt")).IsFalse();
		await That(project.Exists("AlbumTests.Describes.Let It Be.awexpect-snapshot.txt")).IsFalse();
	}

	[Test]
	[Arguments("AlbumTests.Describes.v1.2.awexpect-snapshot.txt")]
	[Arguments("AlbumTests.Describes%ZZ.awexpect-snapshot.txt")]
	[Arguments("AlbumTests.Describes%3a.awexpect-snapshot.txt")]
	[Arguments("Describes.awexpect-snapshot.txt")]
	[Arguments("AlbumTests.Describes@.awexpect-snapshot.txt")]
	[Arguments("AlbumTests.Describes@@x64.awexpect-snapshot.txt")]
	[Arguments("AlbumTests.Describes.awexpect-snapshot")]
	[Arguments("AlbumTests.Describes.awexpect-snapshot.pending")]
	[Arguments("AlbumTests.Describes.awexpect-snapshot.pending.pending")]
	[Arguments("AlbumTests.Describes.awexpect-snapshot.j%on")]
	[Arguments("AlbumTests.Describes.awexpect-snapshot.js on")]
	[Arguments("AlbumTests.Describes.json")]
	public async Task WhenAFileWasNotWrittenByTheDiskStore_ShouldNeverListIt(string fileName)
	{
		using TemporaryProject project = new();
		project.CreateFile("AlbumTests.cs");
		project.CreateFile(fileName, "Abbey Road");
		using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Acceptance.Set(SnapshotAcceptance.All);

		async Task Act() => await OrphanedSnapshots.CheckAsync(true);

		await That(Act).DoesNotThrow();
		await That(project.Exists(fileName)).IsTrue();
	}

	[Test]
	[Arguments("AlbumTests.Describes.snapshot.txt")]
	[Arguments("AlbumTests.Describes.snapshot.pending.txt")]
	[Arguments("AlbumTests.Describes.Abbey Road@net8.0.snapshot.json")]
	public async Task WhenAFileHasTheOldSnapshotMarker_ShouldNeverListIt(string fileName)
	{
		using TemporaryProject project = new();
		project.CreateFile("AlbumTests.cs");
		project.CreateFile(fileName, "Abbey Road");
		using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Acceptance.Set(SnapshotAcceptance.All);

		async Task Act() => await OrphanedSnapshots.CheckAsync(true);

		await That(Act).DoesNotThrow()
			.Because("only files with the marker '.awexpect-snapshot.' are snapshots, so foreign files are never deleted");
		await That(project.Exists(fileName)).IsTrue();
	}

	[Test]
	[Arguments("bin")]
	[Arguments("obj")]
	[Arguments(".vs")]
	[Arguments("Other")]
	public async Task WhenASnapshotIsOutsideOfTheProject_ShouldNeverListIt(string directory)
	{
		using TemporaryProject project = new();
		project.CreateFile("Other/Other.csproj");
		project.CreateFile($"{directory}/AlbumTests.cs");
		project.CreateFile($"{directory}/AlbumTests.Describes.awexpect-snapshot.txt", "Abbey Road");
		using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Acceptance.Set(SnapshotAcceptance.All);

		async Task Act() => await OrphanedSnapshots.CheckAsync(true);

		await That(Act).DoesNotThrow()
			.Because("output directories, hidden directories and other projects are not part of the project");
		await That(project.Exists($"{directory}/AlbumTests.Describes.awexpect-snapshot.txt")).IsTrue();
	}

	[Test]
	public async Task WhenASnapshotIsInASubdirectory_ShouldListIt()
	{
		using TemporaryProject project = new();
		project.CreateFile("Albums/AlbumTests.cs");
		project.CreateFile("Albums/AlbumTests.Describes.awexpect-snapshot.txt", "Abbey Road");
		project.CreateFile("Albums/AlbumTests.Unused.awexpect-snapshot.txt", "Let It Be");
		await That("Abbey Road").MatchesSnapshot(project.CallerFilePath("Albums/AlbumTests.cs"), "Describes");

		async Task Act() => await OrphanedSnapshots.CheckAsync(true);

		await That(Act).Throws<FailException>()
			.WithMessage($"*1 snapshot was not used by any test:*  {project.GetFullPath("Albums/AlbumTests.Unused.awexpect-snapshot.txt")}")
			.AsWildcard();
	}

	[Test]
	public async Task WhenTheBuildIsDeterministic_ShouldMatchTheListedSnapshotsWithTheUsedOnes()
	{
		using TemporaryProject project = new("/_1/");
		project.CreateFile("AlbumTests.cs");
		project.CreateFile("AlbumTests.Describes.awexpect-snapshot.txt", "Abbey Road");
		await That("Abbey Road").MatchesSnapshot(project.CallerFilePath("AlbumTests.cs"), "Describes");

		async Task Act() => await OrphanedSnapshots.CheckAsync(true);

		await That(Act).DoesNotThrow()
			.Because("the compiler emits the mapped path, but the snapshot is found under the source root");
	}
}
