using System.IO;
using System.Linq;
using System.Threading;
using aweXpect.Customization;
using aweXpect.Snapshots.DiffEngine.Tests.TestHelpers;

namespace aweXpect.Snapshots.DiffEngine.Tests;

public sealed class DiffEngineReporterTests
{
	private static SnapshotStore FileStore => Customize.aweXpect.Snapshots().Store.Get();

	[Test]
	public async Task GetTemporaryFiles_ShouldEscapeCharactersThatAFileNameCannotHold()
	{
		SnapshotId id = new("C:/src/MyTests.cs", "MyTest", "a:b%c");

		(string receivedFile, _) = DiffEngineReporter.GetTemporaryFiles(id);

		await That(Path.GetFileName(receivedFile))
			.IsEqualTo("C%3A%2Fsrc%2FMyTests.MyTest.a%3Ab%25c.received.txt");
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
	public async Task Register_WhenTheReporterIsRegistered_ShouldNotAddItAgain()
	{
		DiffEngineReporter.Register();

		SnapshotReporter[] reporters = Customize.aweXpect.Global.Snapshots().Reporters.Get()
			.OfType<DiffEngineReporter>().ToArray<SnapshotReporter>();
		await That(reporters).HasSingle()
			.Because("the build targets of the package registered the reporter when the test assembly was loaded");
	}

	[Test]
	public async Task Registration_ShouldAddTheReporterWhereTheProjectCanCompileAModuleInitializer()
	{
		SnapshotReporter[] reporters = Customize.aweXpect.Global.Snapshots().Reporters.Get()
			.OfType<DiffEngineReporter>().ToArray<SnapshotReporter>();

		await That(reporters).HasSingle()
			.Because("this project declares the ModuleInitializerAttribute for .NET Framework, so the build targets of the package register the reporter on every target framework");
	}

	[Test]
	public async Task WhenDiffEngineIsDisabled_ShouldNeitherLaunchNorWriteFiles()
	{
		RecordingDiffRunner diffRunner = new()
		{
			IsDisabled = true,
		};
		using TestScope scope = new();
		DiffEngineReporter reporter = new(diffRunner);
		SnapshotMismatch mismatch = new(scope.Id, new InMemorySnapshotStore(), null, SnapshotContent.FromText("foo"));

		await reporter.OnMismatchAsync(mismatch, CancellationToken.None);

		await That(diffRunner.Launched).IsEmpty()
			.Because("the opt-outs of DiffEngine are respected");
		await That(File.Exists(scope.TemporaryReceivedFile)).IsFalse();
	}

	[Test]
	public async Task WithFileStore_WhenSnapshotDiffers_ShouldDiffThePendingFileWithTheSnapshotFile()
	{
		RecordingDiffRunner diffRunner = new();
		using TestScope scope = new();
		DiffEngineReporter reporter = new(diffRunner);
		SnapshotMismatch mismatch = new(scope.Id, FileStore,
			SnapshotContent.FromText("bar"), SnapshotContent.FromText("foo"));

		await reporter.OnMismatchAsync(mismatch, CancellationToken.None);

		await That(diffRunner.Launched).IsEqualTo([(scope.PendingPath, scope.SnapshotPath),])
			.Because("changing the snapshot in the diff tool changes the snapshot file");
		await That(File.Exists(scope.TemporaryReceivedFile)).IsFalse();
	}

	[Test]
	public async Task WithFileStore_WhenSnapshotIsMissing_ShouldDiffWithTheMissingSnapshotFile()
	{
		RecordingDiffRunner diffRunner = new();
		using TestScope scope = new();
		DiffEngineReporter reporter = new(diffRunner);
		SnapshotMismatch mismatch = new(scope.Id, FileStore, null, SnapshotContent.FromText("foo"));

		await reporter.OnMismatchAsync(mismatch, CancellationToken.None);

		await That(diffRunner.Launched).IsEqualTo([(scope.PendingPath, scope.SnapshotPath),])
			.Because("DiffEngine creates the missing snapshot file for the diff tool");
	}

	[Test]
	public async Task WithFileStore_WhenSnapshotMatchesWithPendingFile_ShouldCloseTheDiff()
	{
		RecordingDiffRunner diffRunner = new();
		using TestScope scope = new();
		File.WriteAllText(scope.PendingPath, "bar");
		DiffEngineReporter reporter = new(diffRunner);

		await reporter.OnMatchAsync(new SnapshotMatch(scope.Id, FileStore), CancellationToken.None);

		await That(diffRunner.Killed).IsEqualTo([(scope.PendingPath, scope.SnapshotPath),]);
		await That(diffRunner.Launched).IsEmpty();
		await That(File.Exists(scope.PendingPath)).IsTrue()
			.Because("the store deletes the pending file after the reporters were told about the match");
	}

	[Test]
	public async Task WithFileStore_WhenSnapshotMatchesWithoutPendingFile_ShouldNotCloseADiff()
	{
		RecordingDiffRunner diffRunner = new();
		using TestScope scope = new();
		DiffEngineReporter reporter = new(diffRunner);

		await reporter.OnMatchAsync(new SnapshotMatch(scope.Id, FileStore), CancellationToken.None);

		await That(diffRunner.Killed).IsEmpty()
			.Because("without a pending file no diff can be open, so the processes are not searched");
	}

	[Test]
	public async Task WithOtherStore_WhenCanceledBeforeTheMatchIsReported_ShouldNotCloseTheDiff()
	{
		RecordingDiffRunner diffRunner = new();
		using TestScope scope = new();
		DiffEngineReporter reporter = new(diffRunner);
		InMemorySnapshotStore store = new();
		await reporter.OnMismatchAsync(new SnapshotMismatch(scope.Id, store, null, SnapshotContent.FromText("foo")),
			CancellationToken.None);
		using CancellationTokenSource cancellationTokenSource = new();
		cancellationTokenSource.Cancel();

		async Task Act()
			=> await reporter.OnMatchAsync(new SnapshotMatch(scope.Id, store), cancellationTokenSource.Token);

		await That(Act).Throws<OperationCanceledException>().WithMessage(new OperationCanceledException().Message);
		await That(diffRunner.Killed).IsEmpty();
		await That(File.Exists(scope.TemporaryReceivedFile)).IsTrue()
			.Because("a canceled test must not delete the temporary files");
	}

	[Test]
	public async Task WithOtherStore_WhenCanceledBeforeTheMismatchIsReported_ShouldNeitherWriteTemporaryFilesNorLaunch()
	{
		RecordingDiffRunner diffRunner = new();
		using TestScope scope = new();
		DiffEngineReporter reporter = new(diffRunner);
		SnapshotMismatch mismatch = new(scope.Id, new InMemorySnapshotStore(), null, SnapshotContent.FromText("foo"));
		using CancellationTokenSource cancellationTokenSource = new();
		cancellationTokenSource.Cancel();

		async Task Act()
			=> await reporter.OnMismatchAsync(mismatch, cancellationTokenSource.Token);

		await That(Act).Throws<OperationCanceledException>().WithMessage(new OperationCanceledException().Message);
		await That(diffRunner.Launched).IsEmpty();
		await That(File.Exists(scope.TemporaryReceivedFile)).IsFalse()
			.Because("a canceled test must not write temporary files");
	}

	[Test]
	public async Task WithOtherStore_WhenSnapshotDiffers_ShouldDiffTemporaryFiles()
	{
		RecordingDiffRunner diffRunner = new();
		using TestScope scope = new();
		DiffEngineReporter reporter = new(diffRunner);
		SnapshotMismatch mismatch = new(scope.Id, new InMemorySnapshotStore(),
			SnapshotContent.FromText("bar"), SnapshotContent.FromText("foo"));

		await reporter.OnMismatchAsync(mismatch, CancellationToken.None);

		await That(diffRunner.Launched).IsEqualTo([(scope.TemporaryReceivedFile, scope.TemporarySnapshotFile),]);
		await That(File.ReadAllBytes(scope.TemporaryReceivedFile)).IsEqualTo(mismatch.Actual.Bytes)
			.Because("the received file has the bytes that the store recorded");
		await That(File.ReadAllBytes(scope.TemporarySnapshotFile)).IsEqualTo(mismatch.Snapshot!.Bytes);
		await That(File.Exists(scope.PendingPath)).IsFalse();
	}

	[Test]
	public async Task WithOtherStore_WhenSnapshotIsMissing_ShouldDeleteAnOutdatedTemporarySnapshot()
	{
		RecordingDiffRunner diffRunner = new();
		using TestScope scope = new();
		Directory.CreateDirectory(DiffEngineReporter.TemporaryDirectory);
		File.WriteAllText(scope.TemporarySnapshotFile, "outdated");
		DiffEngineReporter reporter = new(diffRunner);
		SnapshotMismatch mismatch = new(scope.Id, new InMemorySnapshotStore(), null, SnapshotContent.FromText("foo"));

		await reporter.OnMismatchAsync(mismatch, CancellationToken.None);

		await That(diffRunner.Launched).IsEqualTo([(scope.TemporaryReceivedFile, scope.TemporarySnapshotFile),]);
		await That(File.Exists(scope.TemporarySnapshotFile)).IsFalse()
			.Because("the snapshot is missing, so the diff must not show an earlier one");
	}

	[Test]
	public async Task WithOtherStore_WhenSnapshotMatches_ShouldCloseTheDiffAndDeleteTheTemporaryFiles()
	{
		RecordingDiffRunner diffRunner = new();
		using TestScope scope = new();
		DiffEngineReporter reporter = new(diffRunner);
		InMemorySnapshotStore store = new();
		await reporter.OnMismatchAsync(new SnapshotMismatch(scope.Id, store,
			SnapshotContent.FromText("bar"), SnapshotContent.FromText("foo")), CancellationToken.None);

		await reporter.OnMatchAsync(new SnapshotMatch(scope.Id, store), CancellationToken.None);

		await That(diffRunner.Killed).IsEqualTo([(scope.TemporaryReceivedFile, scope.TemporarySnapshotFile),]);
		await That(File.Exists(scope.TemporaryReceivedFile)).IsFalse();
		await That(File.Exists(scope.TemporarySnapshotFile)).IsFalse();
	}

	[Test]
	public async Task WithOtherStore_WhenSnapshotMatchesWithoutTemporaryFiles_ShouldNotCloseADiff()
	{
		RecordingDiffRunner diffRunner = new();
		using TestScope scope = new();
		DiffEngineReporter reporter = new(diffRunner);

		await reporter.OnMatchAsync(new SnapshotMatch(scope.Id, new InMemorySnapshotStore()), CancellationToken.None);

		await That(diffRunner.Killed).IsEmpty();
	}

	/// <remarks>
	///     The reporters are never called on a build server, so the tests clear its environment variables, which the whole
	///     process shares.
	/// </remarks>
	[NotInParallel]
	public sealed class MatchesSnapshotTests
	{
		[Test]
		public async Task WithFileStore_WhenSnapshotDiffers_ShouldDiffThePendingFileAfterTheStoreRecordedIt()
		{
			RecordingDiffRunner diffRunner = new();
			using TestScope scope = new(diffRunner);
			File.WriteAllText(scope.SnapshotPath, "bar");
			bool pendingFileExists = false;
			diffRunner.OnLaunch = (receivedFile, _) => pendingFileExists = File.Exists(receivedFile);

			async Task Act()
				=> await That("foo").MatchesSnapshot(scope.TestFilePath, TestScope.MemberName);

			await That(Act).Throws<FailException>()
				.WithMessage($$"""
				               Expected that "foo"
				               matches the snapshot "{{scope.SnapshotPath}}",
				               but it was "foo", which differs at index 0:
				                  ↓ (actual)
				                 "foo"
				                 "bar"
				                  ↑ (expected)

				               Pending snapshot:
				               {{scope.PendingPath}}
				               """);
			await That(diffRunner.Launched).IsEqualTo([(scope.PendingPath, scope.SnapshotPath),]);
			await That(pendingFileExists).IsTrue()
				.Because("the reporter is called after the store recorded the mismatch");
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

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that "foo"
				             matches the snapshot "MyTests.MyTest",
				             but the snapshot did not exist

				             Actual:
				             foo
				             """);
			store.Snapshots[scope.Id] = store.Mismatches[scope.Id];
			await That(Act).DoesNotThrow();
			await That(diffRunner.Launched).IsEqualTo([(scope.TemporaryReceivedFile, scope.TemporarySnapshotFile),]);
			await That(diffRunner.Killed).IsEqualTo([(scope.TemporaryReceivedFile, scope.TemporarySnapshotFile),]);
			await That(File.Exists(scope.TemporaryReceivedFile)).IsFalse();
		}
	}
}
