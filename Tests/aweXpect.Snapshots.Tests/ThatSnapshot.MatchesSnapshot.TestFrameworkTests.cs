using System.Linq;
using System.Threading;
using aweXpect.Customization;
using aweXpect.Snapshots.Tests.TestHelpers;

namespace aweXpect.Snapshots.Tests;

public sealed partial class ThatSnapshot
{
	public sealed partial class MatchesSnapshot
	{
		public sealed class TestFrameworkTests
		{
			private static readonly SnapshotId RetriedId = new($"/src/{Guid.NewGuid():N}/MyTests.cs", "MyTest");
			private static readonly InMemorySnapshotStore RetriedStore = new();
			private static int _retriedAttempts;

			[Test]
			public async Task WhenAttachingPendingFiles_ShouldAttachThePendingFileToTheTest()
			{
				using SnapshotDirectory directory = new();
				directory.WriteSnapshot("outdated");
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().AttachPendingFiles.Set(true);

				async Task Act()
					=> await That("foo").MatchesSnapshot(directory.TestFilePath, SnapshotDirectory.MemberName);

				await That(Act).Throws<FailException>()
					.WithMessage("*Pending snapshot:*").AsWildcard();
				await That(TestContext.Current!.Output.Artifacts.Select(x => x.File.FullName))
					.Contains(directory.PendingPath)
					.Because("the recorded mismatch can be reviewed in the results of the test, e.g. on a build server");
			}

			[Test]
			public async Task WhenMismatchIsRecordedInAFile_ShouldNotAttachThePendingFileByDefault()
			{
				using SnapshotDirectory directory = new();
				directory.WriteSnapshot("outdated");

				async Task Act()
					=> await That("foo").MatchesSnapshot(directory.TestFilePath, SnapshotDirectory.MemberName);

				await That(Act).Throws<FailException>()
					.WithMessage("*Pending snapshot:*").AsWildcard();
				await That(TestContext.Current!.Output.Artifacts.Select(x => x.File.FullName))
					.DoesNotContain(directory.PendingPath)
					.Because("attaching the pending file is opt-in");
			}

			[Test]
			[Retry(1)]
			public async Task WhenRetried_ShouldNotConflictWithTheValueOfTheFailedAttempt()
			{
				int attempt = Interlocked.Increment(ref _retriedAttempts) - 1;
				RetriedStore.Snapshots[RetriedId] = "expected";
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(RetriedStore);
				string subject = attempt == 0 ? "flaky" : "expected";

				await That(subject).MatchesSnapshot(RetriedId.SourceFile, RetriedId.MemberName)
					.Because("the value of the failed first attempt is no conflict for its retry");
			}
		}
	}
}
