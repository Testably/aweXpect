using aweXpect.Customization;
using aweXpect.Snapshots.Orphans.Tests.TestHelpers;

namespace aweXpect.Snapshots.Orphans.Tests;

/// <summary>
///     Every test checks the snapshots of a temporary project, but as all recorded project directories are listed, this
///     test project must not contain snapshots of its own.
/// </summary>
public sealed class DiskStoreTests
{
	[Fact]
	public async Task WhenEverySnapshotIsUsed_ShouldSucceed()
	{
		using TemporaryProject project = new();
		project.CreateFile("AlbumTests.cs");
		project.CreateFile("AlbumTests.Describes.snapshot.txt", "Abbey Road");
		await That("Abbey Road").MatchesSnapshot(project.CallerFilePath("AlbumTests.cs"), "Describes");

		async Task Act() => await OrphanedSnapshots.CheckAsync(true);

		await That(Act).DoesNotThrow();
	}

	[Fact]
	public async Task WhenASnapshotIsNotUsed_ShouldReportItsFileWithoutDeletingIt()
	{
		using TemporaryProject project = new();
		project.CreateFile("AlbumTests.cs");
		project.CreateFile("AlbumTests.Describes.snapshot.txt", "Abbey Road");

		async Task Act() => await OrphanedSnapshots.CheckAsync(true);

		await That(Act).Throws<XunitException>()
			.WithMessage($"""
			              Expected that every snapshot is used by a test of this run, but 1 snapshot was not used by any test:
			                {project.GetFullPath("AlbumTests.Describes.snapshot.txt")}
			              """);
		await That(project.Exists("AlbumTests.Describes.snapshot.txt")).IsTrue();
	}

	[Fact]
	public async Task WhenNamedSnapshotsWithEscapedNamesAreUsed_ShouldReadTheirNamesBack()
	{
		using TemporaryProject project = new();
		project.CreateFile("AlbumTests.cs");
		project.CreateFile("AlbumTests.Describes.Abbey Road.snapshot.txt", "Abbey Road");
		project.CreateFile("AlbumTests.Describes.10%3A30%2FLet It Be%2E1970.snapshot.txt", "Let It Be");
		await That("Abbey Road").MatchesSnapshot(project.CallerFilePath("AlbumTests.cs"), "Describes")
			.Named("Abbey Road");
		await That("Let It Be").MatchesSnapshot(project.CallerFilePath("AlbumTests.cs"), "Describes")
			.Named("10:30/Let It Be.1970");

		async Task Act() => await OrphanedSnapshots.CheckAsync(true);

		await That(Act).DoesNotThrow();
	}

	[Fact]
	public async Task WhenSnapshotsUniqueForThisEnvironmentAreUsed_ShouldReadTheirVariantsBack()
	{
		using TemporaryProject project = new();
		project.CreateFile("AlbumTests.cs");
		project.CreateFile($"AlbumTests.Describes@{TemporaryProject.Variant}.snapshot.txt", "Abbey Road");
		project.CreateFile($"AlbumTests.Describes.AC%40DC@{TemporaryProject.TargetFramework}.snapshot.txt", "AC/DC");
		await That("Abbey Road").MatchesSnapshot(project.CallerFilePath("AlbumTests.cs"), "Describes")
			.UniqueFor.Architecture().UniqueFor.OperatingSystem().UniqueFor.TargetFramework();
		await That("AC/DC").MatchesSnapshot(project.CallerFilePath("AlbumTests.cs"), "Describes")
			.Named("AC@DC").UniqueFor.TargetFramework();

		async Task Act() => await OrphanedSnapshots.CheckAsync(true);

		await That(Act).DoesNotThrow()
			.Because("an '@' in a name is escaped, so the variant starts at the first '@'");
	}

	[Fact]
	public async Task WhenASnapshotUniqueForThisEnvironmentIsNotUsed_ShouldReportAndDeleteIt()
	{
		using TemporaryProject project = new();
		project.CreateFile("AlbumTests.cs");
		project.CreateFile($"AlbumTests.Describes@{TemporaryProject.Variant}.snapshot.txt", "Abbey Road");
		using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Acceptance.Set(SnapshotAcceptance.All);

		async Task Act() => await OrphanedSnapshots.CheckAsync(true);

		await That(Act).Throws<XunitException>()
			.WithMessage($"""
			              Expected that every snapshot is used by a test of this run, but 1 snapshot was not used by any test:
			                {project.GetFullPath($"AlbumTests.Describes@{TemporaryProject.Variant}.snapshot.txt")} (deleted)
			              """);
		await That(project.Exists($"AlbumTests.Describes@{TemporaryProject.Variant}.snapshot.txt")).IsFalse();
	}

	[Fact]
	public async Task WhenTheTestFileOfASnapshotUniqueForThisEnvironmentWasDeleted_ShouldReportAndDeleteIt()
	{
		using TemporaryProject project = new();
		project.CreateFile($"AlbumTests.Describes.Let It Be@{TemporaryProject.Variant}.snapshot.txt", "Let It Be");
		using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Acceptance.Set(SnapshotAcceptance.All);

		async Task Act() => await OrphanedSnapshots.CheckAsync(true);

		await That(Act).Throws<XunitException>()
			.WithMessage("*1 snapshot was not used by any test:*(deleted)").AsWildcard()
			.Because("the dots in the target framework are part of the variant, not of the method or the name");
		await That(project.Exists($"AlbumTests.Describes.Let It Be@{TemporaryProject.Variant}.snapshot.txt")).IsFalse();
	}

	[Theory]
	[InlineData("AlbumTests.Describes@net35.snapshot.txt")]
	[InlineData("AlbumTests.Describes@beos.snapshot.txt")]
	[InlineData("AlbumTests.Describes@{variant}@sparc.snapshot.txt")]
	[InlineData("AlbumTests.Describes.Abbey Road@net35@{variant}.snapshot.pending.txt")]
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

	[Fact]
	public async Task WhenSnapshotsWithAnotherExtensionAreUsed_ShouldReadTheirExtensionsBack()
	{
		using TemporaryProject project = new();
		project.CreateFile("AlbumTests.cs");
		project.CreateFile("AlbumTests.Describes.snapshot.json", "{}");
		project.CreateFile("AlbumTests.Describes.snapshot.pending.json", "[]");
		project.CreateFile($"AlbumTests.Describes.Abbey Road@{TemporaryProject.TargetFramework}.snapshot.html", "<h1/>");
		await That("{}").MatchesSnapshot(project.CallerFilePath("AlbumTests.cs"), "Describes").WithExtension("json");
		await That("<h1/>").MatchesSnapshot(project.CallerFilePath("AlbumTests.cs"), "Describes")
			.Named("Abbey Road").UniqueFor.TargetFramework().WithExtension("html");

		async Task Act() => await OrphanedSnapshots.CheckAsync(true);

		await That(Act).DoesNotThrow()
			.Because("the pending file is read back as the pending file of the snapshot, not as a snapshot with the extension 'pending.json'");
	}

	[Fact]
	public async Task WhenASnapshotWithAnotherExtensionIsNotUsed_ShouldReportAndDeleteItAndItsPendingFile()
	{
		using TemporaryProject project = new();
		project.CreateFile("AlbumTests.cs");
		project.CreateFile("AlbumTests.Describes.snapshot.json", "{}");
		project.CreateFile("AlbumTests.Describes.snapshot.pending.json", "[]");
		project.CreateFile("AlbumTests.Describes.snapshot.txt", "Abbey Road");
		await That("Abbey Road").MatchesSnapshot(project.CallerFilePath("AlbumTests.cs"), "Describes");
		using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Acceptance.Set(SnapshotAcceptance.All);

		async Task Act() => await OrphanedSnapshots.CheckAsync(true);

		await That(Act).Throws<XunitException>()
			.WithMessage($"""
			              Expected that every snapshot is used by a test of this run, but 1 snapshot was not used by any test:
			                {project.GetFullPath("AlbumTests.Describes.snapshot.json")} (deleted)
			              """)
			.Because("the test uses the snapshot with the extension 'txt'");
		await That(project.Exists("AlbumTests.Describes.snapshot.json")).IsFalse();
		await That(project.Exists("AlbumTests.Describes.snapshot.pending.json")).IsFalse();
		await That(project.Exists("AlbumTests.Describes.snapshot.txt")).IsTrue();
	}

	[Fact]
	public async Task WhenOnlyAPendingFileWithAnotherExtensionIsLeft_ShouldReportAndDeleteIt()
	{
		using TemporaryProject project = new();
		project.CreateFile("AlbumTests.cs");
		project.CreateFile("AlbumTests.Describes.snapshot.pending.xml", "<album />");
		project.CreateFile("AlbumTests.Describes.snapshot.txt", "Abbey Road");
		await That("Abbey Road").MatchesSnapshot(project.CallerFilePath("AlbumTests.cs"), "Describes");
		using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Acceptance.Set(SnapshotAcceptance.All);

		async Task Act() => await OrphanedSnapshots.CheckAsync(true);

		await That(Act).Throws<XunitException>()
			.WithMessage($"*1 snapshot was not used by any test:*  {project.GetFullPath("AlbumTests.Describes.snapshot.pending.xml")} (deleted)")
			.AsWildcard();
		await That(project.Exists("AlbumTests.Describes.snapshot.pending.xml")).IsFalse();
		await That(project.Exists("AlbumTests.Describes.snapshot.txt")).IsTrue();
	}

	[Fact]
	public async Task WhenTheExtensionIsSnapshot_ShouldReadItBack()
	{
		using TemporaryProject project = new();
		project.CreateFile("AlbumTests.cs");
		project.CreateFile("AlbumTests.Describes.snapshot.snapshot", "Abbey Road");
		project.CreateFile("AlbumTests.Describes.snapshot.pending.snapshot", "Let It Be");
		await That("Abbey Road").MatchesSnapshot(project.CallerFilePath("AlbumTests.cs"), "Describes")
			.WithExtension("snapshot");

		async Task Act() => await OrphanedSnapshots.CheckAsync(true);

		await That(Act).DoesNotThrow();
	}

	[Fact]
	public async Task WhenTheClassNameContainsDots_ShouldReadTheMethodBack()
	{
		using TemporaryProject project = new();
		project.CreateFile("AlbumTests.Tracks.cs");
		project.CreateFile("AlbumTests.Tracks.Count.snapshot.txt", "17");
		await That("17").MatchesSnapshot(project.CallerFilePath("AlbumTests.Tracks.cs"), "Count");

		async Task Act() => await OrphanedSnapshots.CheckAsync(true);

		await That(Act).DoesNotThrow();
	}

	[Fact]
	public async Task WhenSeveralTestFilesFit_AndTheLongerOneUsesTheSnapshot_ShouldSucceed()
	{
		using TemporaryProject project = new();
		project.CreateFile("AlbumTests.cs");
		project.CreateFile("AlbumTests.Tracks.cs");
		project.CreateFile("AlbumTests.Tracks.Count.snapshot.txt", "17");
		await That("17").MatchesSnapshot(project.CallerFilePath("AlbumTests.Tracks.cs"), "Count");

		async Task Act() => await OrphanedSnapshots.CheckAsync(true);

		await That(Act).DoesNotThrow();
	}

	[Fact]
	public async Task WhenSeveralTestFilesFit_AndTheShorterOneUsesTheSnapshot_ShouldSucceed()
	{
		using TemporaryProject project = new();
		project.CreateFile("AlbumTests.cs");
		project.CreateFile("AlbumTests.Tracks.cs");
		project.CreateFile("AlbumTests.Tracks.Count.snapshot.txt", "17");
		await That("17").MatchesSnapshot(project.CallerFilePath("AlbumTests.cs"), "Tracks").Named("Count");

		async Task Act() => await OrphanedSnapshots.CheckAsync(true);

		await That(Act).DoesNotThrow()
			.Because("the method 'Tracks' of 'AlbumTests.cs' with the name 'Count' uses the same file");
	}

	[Fact]
	public async Task WhenOnlyAPendingFileIsLeft_ShouldReportAndDeleteIt()
	{
		using TemporaryProject project = new();
		project.CreateFile("AlbumTests.cs");
		project.CreateFile("AlbumTests.Describes.snapshot.pending.txt", "Abbey Road");
		using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Acceptance.Set(SnapshotAcceptance.All);

		async Task Act() => await OrphanedSnapshots.CheckAsync(true);

		await That(Act).Throws<XunitException>()
			.WithMessage($"*  {project.GetFullPath("AlbumTests.Describes.snapshot.pending.txt")} (deleted)")
			.AsWildcard();
		await That(project.Exists("AlbumTests.Describes.snapshot.pending.txt")).IsFalse();
	}

	[Fact]
	public async Task WhenAcceptingAll_ShouldDeleteTheSnapshotAndItsPendingFileAndStillFail()
	{
		using TemporaryProject project = new();
		project.CreateFile("AlbumTests.cs");
		project.CreateFile("AlbumTests.Describes.Abbey Road.snapshot.txt", "Abbey Road");
		project.CreateFile("AlbumTests.Describes.Let It Be.snapshot.txt", "Let It Be");
		project.CreateFile("AlbumTests.Describes.Let It Be.snapshot.pending.txt", "Let It Be (Remastered)");
		using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Acceptance.Set(SnapshotAcceptance.All);
		await That("Abbey Road").MatchesSnapshot(project.CallerFilePath("AlbumTests.cs"), "Describes")
			.Named("Abbey Road");

		async Task Act() => await OrphanedSnapshots.CheckAsync(true);

		await That(Act).Throws<XunitException>()
			.WithMessage($"""
			              Expected that every snapshot is used by a test of this run, but 1 snapshot was not used by any test:
			                {project.GetFullPath("AlbumTests.Describes.Let It Be.snapshot.txt")} (deleted)
			              """);
		await That(project.Exists("AlbumTests.Describes.Let It Be.snapshot.txt")).IsFalse();
		await That(project.Exists("AlbumTests.Describes.Let It Be.snapshot.pending.txt")).IsFalse();
		await That(project.Exists("AlbumTests.Describes.Abbey Road.snapshot.txt")).IsTrue();
	}

	[Fact]
	public async Task WhenTheTestFileWasDeleted_ShouldReportAndDeleteItsSnapshots()
	{
		using TemporaryProject project = new();
		project.CreateFile("AlbumTests.Describes.snapshot.txt", "Abbey Road");
		project.CreateFile("AlbumTests.Describes.Let It Be.snapshot.txt", "Let It Be");
		using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Acceptance.Set(SnapshotAcceptance.All);

		async Task Act() => await OrphanedSnapshots.CheckAsync(true);

		await That(Act).Throws<XunitException>()
			.WithMessage("*2 snapshots were not used by any test:*").AsWildcard();
		await That(project.Exists("AlbumTests.Describes.snapshot.txt")).IsFalse();
		await That(project.Exists("AlbumTests.Describes.Let It Be.snapshot.txt")).IsFalse();
	}

	[Theory]
	[InlineData("AlbumTests.Describes.v1.2.snapshot.txt")]
	[InlineData("AlbumTests.Describes%ZZ.snapshot.txt")]
	[InlineData("AlbumTests.Describes%3a.snapshot.txt")]
	[InlineData("Describes.snapshot.txt")]
	[InlineData("AlbumTests.Describes@.snapshot.txt")]
	[InlineData("AlbumTests.Describes@@x64.snapshot.txt")]
	[InlineData("AlbumTests.Describes.snapshot")]
	[InlineData("AlbumTests.Describes.snapshot.pending")]
	[InlineData("AlbumTests.Describes.snapshot.pending.pending")]
	[InlineData("AlbumTests.Describes.snapshot.j%on")]
	[InlineData("AlbumTests.Describes.snapshot.js on")]
	[InlineData("AlbumTests.Describes.json")]
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

	[Theory]
	[InlineData("bin")]
	[InlineData("obj")]
	[InlineData(".vs")]
	[InlineData("Other")]
	public async Task WhenASnapshotIsOutsideOfTheProject_ShouldNeverListIt(string directory)
	{
		using TemporaryProject project = new();
		project.CreateFile("Other/Other.csproj");
		project.CreateFile($"{directory}/AlbumTests.cs");
		project.CreateFile($"{directory}/AlbumTests.Describes.snapshot.txt", "Abbey Road");
		using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Acceptance.Set(SnapshotAcceptance.All);

		async Task Act() => await OrphanedSnapshots.CheckAsync(true);

		await That(Act).DoesNotThrow()
			.Because("output directories, hidden directories and other projects are not part of the project");
		await That(project.Exists($"{directory}/AlbumTests.Describes.snapshot.txt")).IsTrue();
	}

	[Fact]
	public async Task WhenASnapshotIsInASubdirectory_ShouldListIt()
	{
		using TemporaryProject project = new();
		project.CreateFile("Albums/AlbumTests.cs");
		project.CreateFile("Albums/AlbumTests.Describes.snapshot.txt", "Abbey Road");
		project.CreateFile("Albums/AlbumTests.Unused.snapshot.txt", "Let It Be");
		await That("Abbey Road").MatchesSnapshot(project.CallerFilePath("Albums/AlbumTests.cs"), "Describes");

		async Task Act() => await OrphanedSnapshots.CheckAsync(true);

		await That(Act).Throws<XunitException>()
			.WithMessage($"*1 snapshot was not used by any test:*  {project.GetFullPath("Albums/AlbumTests.Unused.snapshot.txt")}")
			.AsWildcard();
	}

	[Fact]
	public async Task WhenTheBuildIsDeterministic_ShouldMatchTheListedSnapshotsWithTheUsedOnes()
	{
		using TemporaryProject project = new("/_1/");
		project.CreateFile("AlbumTests.cs");
		project.CreateFile("AlbumTests.Describes.snapshot.txt", "Abbey Road");
		await That("Abbey Road").MatchesSnapshot(project.CallerFilePath("AlbumTests.cs"), "Describes");

		async Task Act() => await OrphanedSnapshots.CheckAsync(true);

		await That(Act).DoesNotThrow()
			.Because("the compiler emits the mapped path, but the snapshot is found under the source root");
	}
}
