using System.IO;
using System.Runtime.CompilerServices;
using aweXpect.Snapshots.SourceRoots.Tests.TestHelpers;

namespace aweXpect.Snapshots.SourceRoots.Tests;

/// <summary>
///     Every test records further source roots, which stay recorded for the rest of the process, but deletes their
///     files again, so that only the own source root contains a test file of this project for the next test.
/// </summary>
public sealed class SourceRootsTests
{
	private const string CommittedSnapshot = nameof(WhenTheTestFileExistsUnderOneOfSeveralSourceRoots_ShouldUseIt);

	[Fact]
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

	[Fact]
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

	[Fact]
	public async Task WhenTheTestFileExistsUnderALaterSourceRoot_ShouldReadTheSnapshotThere()
	{
		using TemporarySourceRoot emptySourceRoot = new();
		using TemporarySourceRoot otherSourceRoot = new();
		otherSourceRoot.CreateFile("Tests/OtherTests.cs");
		otherSourceRoot.CreateFile("Tests/OtherTests.MyTest.snapshot.txt", "from the other source root");
		RecordedSourceRoots.Record(emptySourceRoot.Path, emptySourceRoot.Path);
		RecordedSourceRoots.Record(otherSourceRoot.Path, otherSourceRoot.Path);
		string subject = "from the other source root";

		async Task Act()
			=> await That(subject).MatchesSnapshot(Path.Combine(otherSourceRoot.Path, "Tests", "OtherTests.cs"), "MyTest");

		await That(Act).DoesNotThrow()
			.Because("the snapshot is read next to the test file, under the only source root that contains it");
	}

	[Fact]
	public async Task WhenTheTestFileExistsUnderNoneOfSeveralSourceRoots_ShouldFail()
	{
		using TemporarySourceRoot otherSourceRoot = new();
		RecordedSourceRoots.Record(otherSourceRoot.Path, otherSourceRoot.Path);
		string subject = "foo";

		async Task Act()
			=> await That(subject).MatchesSnapshot(Path.Combine(otherSourceRoot.Path, "Tests", "Missing.cs"), "MyTest");

		await That(Act).Throws<XunitException>()
			.WithMessage($"""
			              Expected that subject
			              matches the snapshot "*",
			              but the snapshot could not be read: The test file "Tests/Missing.cs" exists under none of the source roots *"{otherSourceRoot.Path}"*, which test assemblies from different source roots in this process record. Run them in separate processes.
			              """).AsWildcard();
	}

	[Fact]
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

	[Fact]
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

		await That(Act).Throws<XunitException>()
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
