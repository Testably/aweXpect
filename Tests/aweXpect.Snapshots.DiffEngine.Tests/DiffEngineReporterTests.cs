using System.IO;
using System.Linq;
using System.Text;
using aweXpect.Customization;
using aweXpect.Snapshots.DiffEngine.Tests.TestHelpers;

namespace aweXpect.Snapshots.DiffEngine.Tests;

public sealed class DiffEngineReporterTests
{
	[Test]
	public async Task WithFileStore_WhenSnapshotDiffers_ShouldDiffThePendingFileWithTheSnapshotFile()
	{
		RecordingDiffRunner diffRunner = new();
		using TestScope scope = new(diffRunner);
		File.WriteAllText(scope.SnapshotPath, "bar");
		bool pendingFileExists = false;
		diffRunner.OnLaunch = (receivedFile, _) => pendingFileExists = File.Exists(receivedFile);

		async Task Act()
			=> await That("foo").MatchesSnapshot(scope.TestFilePath, TestScope.MemberName);

		await That(Act).Throws<FailException>();
		await That(diffRunner.Launched).IsEqualTo([(scope.PendingPath, scope.SnapshotPath),])
			.Because("changing the snapshot in the diff tool changes the snapshot file");
		await That(pendingFileExists).IsTrue();
		await That(File.Exists(scope.TemporaryReceivedFile)).IsFalse();
	}

	[Test]
	public async Task WithFileStore_WhenSnapshotIsMissing_ShouldDiffWithTheMissingSnapshotFile()
	{
		RecordingDiffRunner diffRunner = new();
		using TestScope scope = new(diffRunner);

		async Task Act()
			=> await That("foo").MatchesSnapshot(scope.TestFilePath, TestScope.MemberName);

		await That(Act).Throws<FailException>();
		await That(diffRunner.Launched).IsEqualTo([(scope.PendingPath, scope.SnapshotPath),])
			.Because("DiffEngine creates the missing snapshot file for the diff tool");
	}

	[Test]
	public async Task WithFileStore_WhenSnapshotMatchesWithPendingFile_ShouldCloseTheDiff()
	{
		RecordingDiffRunner diffRunner = new();
		using TestScope scope = new(diffRunner);
		File.WriteAllText(scope.SnapshotPath, "foo");
		File.WriteAllText(scope.PendingPath, "bar");

		async Task Act()
			=> await That("foo").MatchesSnapshot(scope.TestFilePath, TestScope.MemberName);

		await That(Act).DoesNotThrow();
		await That(diffRunner.Killed).IsEqualTo([(scope.PendingPath, scope.SnapshotPath),]);
		await That(diffRunner.Launched).IsEmpty();
		await That(File.Exists(scope.PendingPath)).IsFalse();
	}

	[Test]
	public async Task WithFileStore_WhenSnapshotMatchesWithoutPendingFile_ShouldNotCloseADiff()
	{
		RecordingDiffRunner diffRunner = new();
		using TestScope scope = new(diffRunner);
		File.WriteAllText(scope.SnapshotPath, "foo");

		async Task Act()
			=> await That("foo").MatchesSnapshot(scope.TestFilePath, TestScope.MemberName);

		await That(Act).DoesNotThrow();
		await That(diffRunner.Killed).IsEmpty()
			.Because("without a pending file no diff can be open, so the processes are not searched");
	}

	[Test]
	public async Task WithOtherStore_WhenSnapshotDiffers_ShouldDiffTemporaryFiles()
	{
		RecordingDiffRunner diffRunner = new();
		using TestScope scope = new(diffRunner);
		InMemorySnapshotStore store = new();
		store.Snapshots[scope.Id] = Encoding.UTF8.GetBytes("bar\n");
		using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);

		async Task Act()
			=> await That("foo").MatchesSnapshot(scope.TestFilePath, TestScope.MemberName);

		await That(Act).Throws<FailException>();
		await That(diffRunner.Launched).IsEqualTo([(scope.TemporaryReceivedFile, scope.TemporarySnapshotFile),]);
		await That(File.ReadAllBytes(scope.TemporaryReceivedFile)).IsEqualTo(store.Mismatches[scope.Id])
			.Because("the received file has the bytes that the store recorded");
		await That(File.ReadAllBytes(scope.TemporarySnapshotFile)).IsEqualTo(store.Snapshots[scope.Id]);
		await That(File.Exists(scope.PendingPath)).IsFalse();
	}

	[Test]
	public async Task WithOtherStore_WhenSnapshotIsMissing_ShouldDeleteAnOutdatedTemporarySnapshot()
	{
		RecordingDiffRunner diffRunner = new();
		using TestScope scope = new(diffRunner);
		InMemorySnapshotStore store = new();
		using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
		Directory.CreateDirectory(DiffEngineReporter.TemporaryDirectory);
		File.WriteAllText(scope.TemporarySnapshotFile, "outdated");

		async Task Act()
			=> await That("foo").MatchesSnapshot(scope.TestFilePath, TestScope.MemberName);

		await That(Act).Throws<FailException>();
		await That(diffRunner.Launched).IsEqualTo([(scope.TemporaryReceivedFile, scope.TemporarySnapshotFile),]);
		await That(File.Exists(scope.TemporarySnapshotFile)).IsFalse()
			.Because("the snapshot is missing, so the diff must not show an earlier one");
	}

	[Test]
	public async Task WithOtherStore_WhenSnapshotMatches_ShouldCloseTheDiffAndDeleteTheTemporaryFiles()
	{
		RecordingDiffRunner diffRunner = new();
		using TestScope scope = new(diffRunner);
		InMemorySnapshotStore store = new();
		using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);

		async Task Act()
			=> await That("foo").MatchesSnapshot(scope.TestFilePath, TestScope.MemberName);

		await That(Act).Throws<FailException>();
		store.Snapshots[scope.Id] = store.Mismatches[scope.Id];
		await That(Act).DoesNotThrow();
		await That(diffRunner.Killed).IsEqualTo([(scope.TemporaryReceivedFile, scope.TemporarySnapshotFile),]);
		await That(File.Exists(scope.TemporaryReceivedFile)).IsFalse();
		await That(File.Exists(scope.TemporarySnapshotFile)).IsFalse();
	}

	[Test]
	public async Task WithOtherStore_WhenSnapshotMatchesWithoutTemporaryFiles_ShouldNotCloseADiff()
	{
		RecordingDiffRunner diffRunner = new();
		using TestScope scope = new(diffRunner);
		InMemorySnapshotStore store = new();
		store.Snapshots[scope.Id] = Encoding.UTF8.GetBytes("foo\n");
		using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);

		async Task Act()
			=> await That("foo").MatchesSnapshot(scope.TestFilePath, TestScope.MemberName);

		await That(Act).DoesNotThrow();
		await That(diffRunner.Killed).IsEmpty();
	}

	[Test]
	public async Task WhenDiffEngineIsDisabled_ShouldNeitherLaunchNorWriteFiles()
	{
		RecordingDiffRunner diffRunner = new()
		{
			IsDisabled = true,
		};
		using TestScope scope = new(diffRunner);
		InMemorySnapshotStore store = new();
		using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);

		async Task Act()
			=> await That("foo").MatchesSnapshot(scope.TestFilePath, TestScope.MemberName);

		await That(Act).Throws<FailException>();
		await That(diffRunner.Launched).IsEmpty()
			.Because("the opt-outs of DiffEngine are respected");
		await That(File.Exists(scope.TemporaryReceivedFile)).IsFalse();
	}

	[Test]
	public async Task GetTemporaryFiles_ShouldKeepTheExtensionAndTellDirectoriesApart()
	{
		SnapshotId id = new("Tests/Albums/AlbumTests.cs", "Describes", "Abbey Road")
		{
			Extension = "json",
		};
		SnapshotId otherDirectory = id with
		{
			SourceFile = "Tests/Singles/AlbumTests.cs",
		};

		(string receivedFile, string snapshotFile) = DiffEngineReporter.GetTemporaryFiles(id);
		(string otherReceivedFile, _) = DiffEngineReporter.GetTemporaryFiles(otherDirectory);

		await That(Path.GetFileName(receivedFile))
			.IsEqualTo("Tests%2FAlbums%2FAlbumTests.Describes.Abbey Road.received.json");
		await That(Path.GetFileName(snapshotFile))
			.IsEqualTo("Tests%2FAlbums%2FAlbumTests.Describes.Abbey Road.snapshot.json");
		await That(Path.GetDirectoryName(receivedFile)).IsEqualTo(DiffEngineReporter.TemporaryDirectory);
		await That(otherReceivedFile).IsNotEqualTo(receivedFile)
			.Because("test files with the same name in different directories must not share a file");
	}

	[Test]
	public async Task GetTemporaryFiles_ShouldEscapeCharactersThatAFileNameCannotHold()
	{
		SnapshotId id = new("C:/src/MyTests.cs", "MyTest", "a:b%c");

		(string receivedFile, _) = DiffEngineReporter.GetTemporaryFiles(id);

		await That(Path.GetFileName(receivedFile))
			.IsEqualTo("C%3A%2Fsrc%2FMyTests.MyTest.a%3Ab%25c.received.txt");
	}

	[Test]
	public async Task Registration_ShouldAddTheReporterOnlyWhereModuleInitializersExist()
	{
		SnapshotReporter[] reporters = Customize.aweXpect.Global.Snapshots().Reporters.Get()
			.OfType<DiffEngineReporter>().ToArray<SnapshotReporter>();

#if NET5_0_OR_GREATER
		await That(reporters).HasSingle()
			.Because("the build targets of the package register the reporter once");
#else
		await That(reporters).IsEmpty()
			.Because(".NET Framework has no module initializers, so the reporter is registered explicitly there");
#endif
	}
}
