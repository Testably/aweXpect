using System.IO;
using System.Runtime.CompilerServices;
using aweXpect.Customization;
using aweXpect.Snapshots.SourceRoots.Tests.TestHelpers;

namespace aweXpect.Snapshots.SourceRoots.Tests;

/// <summary>
///     Every test records further source roots, which stay recorded for the rest of the process, but deletes their
///     files again, so that only the own source root contains a test file of this project for the next test.
/// </summary>
public sealed class SourceRootsTests
{
	private const string CommittedSnapshot = nameof(WhenTheTestFileExistsUnderOneOfSeveralSourceRoots_ShouldUseIt);

	[Test]
	public async Task WhenDeterministicBuildsRecordSeveralSourceRoots_ShouldUseTheOneWithTheTestFile()
	{
		using TemporarySourceRoot otherSourceRoot = new();
		RecordedSourceRoots.Record("/_/", RecordedSourceRoots.Own.SourceRoot);
		RecordedSourceRoots.Record("/_/", otherSourceRoot.Path);
		string subject = "found";

		async Task Act()
			=> await That(subject).MatchesSnapshot("/_/" + RecordedSourceRoots.RelativeTestFile(), CommittedSnapshot);

		await That(Act).DoesNotThrow()
			.Because("every deterministic build maps its source root to '/_/', and the test file only exists in one");
	}

	[Test]
	public async Task WhenOtherDirectoriesRecordATargetFramework_ShouldUseTheInnermostProjectOfTheTestFile()
	{
		using TemporarySourceRoot otherSourceRoot = new();
		RecordedSourceRoots.Record("/_/", RecordedSourceRoots.Own.SourceRoot);
		RecordedSourceRoots.Record("/_/", otherSourceRoot.Path);
		RecordedSourceRoots.RecordTargetFramework("net35", RecordedSourceRoots.Own.SourceRoot);
		RecordedSourceRoots.RecordTargetFramework("net40", otherSourceRoot.Path + "Tests");
		string subject = "found";

		async Task Act()
			=> await That(subject).MatchesSnapshot("/_/" + RecordedSourceRoots.RelativeTestFile(),
				nameof(WhenOtherDirectoriesRecordATargetFramework_ShouldUseTheInnermostProjectOfTheTestFile))
				.UniqueFor.TargetFramework();

		await That(Act).DoesNotThrow()
			.Because(
				"the snapshot of each target framework of this test project is committed, and neither an enclosing directory nor another source root decides it");
	}

	[Test]
	public async Task WhenSeveralSourceRootsAreRecorded_ShouldMaskEachOneButNoRootOfTheFileSystem()
	{
		using TemporarySourceRoot otherSourceRoot = new();
		string fileSystemRoot = Path.GetPathRoot(otherSourceRoot.Path)!;
		RecordedSourceRoots.Record("/_other/", otherSourceRoot.Path);
		RecordedSourceRoots.Record("/_root/", fileSystemRoot);
		RecordingSnapshotStore store = new();
		using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
		string subject =
			$"{Path.Combine(otherSourceRoot.Path, "Let It Be.txt")} {Path.Combine(fileSystemRoot, "Abbey Road.txt")}";

		async Task Act()
			=> await That(subject).MatchesSnapshot(GetCallerFilePath(), "Masking");

		await That(Act).Throws<FailException>();
		await That(store.Mismatch)
			.IsEqualTo($"{{SourceRoot}}{Path.DirectorySeparatorChar}Let It Be.txt {fileSystemRoot}Abbey Road.txt")
			.Because("every recorded source root is masked, unless it is a root of the file system, which would mask every path");
	}

	[Test]
	public async Task WhenTheSameSourceRootIsRecordedTwice_ShouldCountItOnce()
	{
		string sourceRoot = RecordedSourceRoots.Own.SourceRoot.TrimEnd(Path.DirectorySeparatorChar);
		if (Path.DirectorySeparatorChar == '\\')
		{
			sourceRoot = (char.IsUpper(sourceRoot[0])
				? char.ToLowerInvariant(sourceRoot[0])
				: char.ToUpperInvariant(sourceRoot[0])) + sourceRoot.Substring(1);
		}

		RecordedSourceRoots.Record(sourceRoot, sourceRoot);
		string subject = "found";

		async Task Act()
			=> await That(subject).MatchesSnapshot(GetCallerFilePath(), CommittedSnapshot);

		await That(Act).DoesNotThrow()
			.Because("a source root with another trailing separator or case of the drive letter is the same directory");
	}

	[Test]
	public async Task WhenTheTestFileExistsUnderALaterSourceRoot_ShouldReadTheSnapshotThere()
	{
		using TemporarySourceRoot emptySourceRoot = new();
		using TemporarySourceRoot otherSourceRoot = new();
		otherSourceRoot.CreateFile("Tests/OtherTests.cs");
		otherSourceRoot.CreateFile("Tests/OtherTests.MyTest.awexpect-snapshot.txt", "from the other source root");
		RecordedSourceRoots.Record(emptySourceRoot.Path, emptySourceRoot.Path);
		RecordedSourceRoots.Record(otherSourceRoot.Path, otherSourceRoot.Path);
		string subject = "from the other source root";

		async Task Act()
			=> await That(subject).MatchesSnapshot(Path.Combine(otherSourceRoot.Path, "Tests", "OtherTests.cs"), "MyTest");

		await That(Act).DoesNotThrow()
			.Because("the snapshot is read next to the test file, under the only source root that contains it");
	}

	[Test]
	public async Task WhenTheTestFileExistsUnderNoneOfSeveralSourceRoots_ShouldFail()
	{
		using TemporarySourceRoot otherSourceRoot = new();
		RecordedSourceRoots.Record(otherSourceRoot.Path, otherSourceRoot.Path);
		string subject = "foo";

		async Task Act()
			=> await That(subject).MatchesSnapshot(Path.Combine(otherSourceRoot.Path, "Tests", "Missing.cs"), "MyTest");

		await That(Act).Throws<FailException>()
			.WithMessage($"""
			              Expected that subject
			              matches the snapshot "*",
			              but the snapshot could not be read: The test file "Tests/Missing.cs" exists under none of the source roots *"{otherSourceRoot.Path}"*, which test assemblies from different source roots in this process record. Run them in separate processes.
			              """).AsWildcard();
	}

	[Test]
	public async Task WhenTheTestFileExistsUnderOneOfSeveralSourceRoots_ShouldUseIt()
	{
		using TemporarySourceRoot otherSourceRoot = new();
		RecordedSourceRoots.Record(otherSourceRoot.Path, otherSourceRoot.Path);
		string subject = "found";

		async Task Act()
			=> await That(subject).MatchesSnapshot();

		await That(Act).DoesNotThrow()
			.Because("the test file only exists under the own source root, next to its committed snapshot");
	}

	[Test]
	public async Task WhenTheTestFileExistsUnderSeveralSourceRoots_ShouldFail()
	{
		await That("found").MatchesSnapshot(GetCallerFilePath(), CommittedSnapshot);
		using TemporarySourceRoot otherSourceRoot = new();
		string relativeTestFile = RecordedSourceRoots.RelativeTestFile();
		otherSourceRoot.CreateFile(relativeTestFile);
		RecordedSourceRoots.Record(otherSourceRoot.Path, otherSourceRoot.Path);
		string subject = "foo";

		async Task Act()
			=> await That(subject).MatchesSnapshot();

		await That(Act).Throws<FailException>()
			.WithMessage($"""
			              Expected that subject
			              matches the snapshot "*",
			              but the snapshot could not be read: The test file "{relativeTestFile}" exists under more than one of the source roots *"{otherSourceRoot.Path}"*, which test assemblies from different source roots in this process record. Run them in separate processes.
			              """).AsWildcard()
			.Because("the other source root was recorded by an assembly that was loaded after the first snapshot");
	}

	private static string GetCallerFilePath([CallerFilePath] string callerFilePath = "")
		=> callerFilePath;
}
