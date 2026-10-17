using System.IO;
using aweXpect.Snapshots.Tests.TestHelpers;

namespace aweXpect.Snapshots.Tests;

public sealed partial class ThatSnapshot
{
	public sealed partial class MatchesSnapshot
	{
		public sealed class DiskStoreTests
		{
			[Fact]
			public async Task WhenCalledWithoutCallerInformation_ShouldUseTestFileAndMethodName()
			{
				string subject = "stored next to the test file";

				async Task Act()
					=> await That(subject).MatchesSnapshot();

				await That(Act).DoesNotThrow()
					.Because(
						"the snapshot 'ThatSnapshot.MatchesSnapshot.DiskStoreTests.WhenCalledWithoutCallerInformation_ShouldUseTestFileAndMethodName.snapshot.txt' is committed next to this test file");
			}

			[Theory]
			[InlineData("")]
			[InlineData("foo")]
			[InlineData("foo\n")]
			[InlineData("foo\r\n")]
			[InlineData("\n\nfoo\n\n")]
			public async Task WhenAccepting_ShouldMatchTheAcceptedSnapshotInTheNextRun(string subject)
			{
				using SnapshotDirectory directory = new();
				directory.WriteSnapshot("outdated");

				async Task Act()
					=> await That(subject).MatchesSnapshot(directory.TestFilePath, SnapshotDirectory.MemberName)
						.AcceptingChanges();

				await That(Act).Throws<XunitException>()
					.WithMessage("*Accepted snapshot:*").AsWildcard();
				await That(Act).DoesNotThrow()
					.Because("the accepted snapshot is read back unchanged");
			}

			[Fact]
			public async Task WhenAccepting_AndDirectoryDoesNotExist_ShouldReportThatTheSnapshotCouldNotBeWritten()
			{
				using SnapshotDirectory directory = new();
				string testFilePath = Path.Combine(directory.Path, "missing", "MyTests.cs");
				string subject = "foo";

				async Task Act()
					=> await That(subject).MatchesSnapshot(testFilePath, SnapshotDirectory.MemberName)
						.AcceptingChanges();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             *
					             Accepted snapshot:
					             could not be written: *
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenAccepting_AndPendingSnapshotCannotBeDeleted_ShouldStillAccept()
			{
				using SnapshotDirectory directory = new();
				directory.WritePending("outdated");
				using FileStream lockedPending = new(directory.PendingPath, FileMode.Open, FileAccess.Read,
					FileShare.None);
				string subject = "foo";

				async Task Act()
					=> await That(subject).MatchesSnapshot(directory.TestFilePath, SnapshotDirectory.MemberName)
						.AcceptingChanges();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              *
					              Accepted snapshot:
					              {directory.SnapshotPath}
					              """).AsWildcard()
					.Because("the pending snapshot is only a leftover of an earlier run");
				await That(File.ReadAllText(directory.SnapshotPath)).IsEqualTo("foo" + Environment.NewLine);
			}

			[Fact]
			public async Task WhenAccepting_ShouldWriteTheSnapshotAndDeleteThePendingSnapshot()
			{
				using SnapshotDirectory directory = new();
				directory.WriteSnapshot("foo");
				directory.WritePending("outdated");
				string subject = "bar";

				async Task Act()
					=> await That(subject).MatchesSnapshot(directory.TestFilePath, SnapshotDirectory.MemberName)
						.AcceptingChanges();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              matches the snapshot "{directory.SnapshotPath}",
					              but it was "bar", which differs at index 0:
					                 ↓ (actual)
					                "bar"
					                "foo"
					                 ↑ (expected)

					              Accepted snapshot:
					              {directory.SnapshotPath}
					              """);
				await That(File.ReadAllBytes(directory.SnapshotPath))
					.IsEqualTo(new System.Text.UTF8Encoding(false).GetBytes(subject + Environment.NewLine));
				await That(File.Exists(directory.PendingPath)).IsFalse()
					.Because("the accepted snapshot replaces the pending snapshot");
			}

			[Fact]
			public async Task WhenMasking_AndAccepting_ShouldWriteThePlaceholderToTheSnapshot()
			{
				using SnapshotDirectory directory = new();
				string subject = "id=42";

				async Task Act()
					=> await That(subject).MatchesSnapshot(directory.TestFilePath, SnapshotDirectory.MemberName)
						.Masking("42", "<id>").AcceptingChanges();

				await That(Act).Throws<XunitException>();
				await That(File.ReadAllText(directory.SnapshotPath)).IsEqualTo("id=<id>" + Environment.NewLine);
			}

			[Fact]
			public async Task WhenMasking_ShouldWriteThePlaceholderToThePendingSnapshot()
			{
				using SnapshotDirectory directory = new();
				string subject = "id=42";

				async Task Act()
					=> await That(subject).MatchesSnapshot(directory.TestFilePath, SnapshotDirectory.MemberName)
						.Masking(@"\d+", "<id>").AsRegex();

				await That(Act).Throws<XunitException>();
				await That(File.ReadAllText(directory.PendingPath)).IsEqualTo("id=<id>" + Environment.NewLine);
			}

			[Fact]
			public async Task WhenPathComesFromDeterministicBuild_WithoutRecordedSourceRoot_ShouldFail()
			{
				string subject = "foo";

				async Task Act()
					=> await That(subject).MatchesSnapshot("/_9/Tests/MyTests.cs", SnapshotDirectory.MemberName);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             matches the snapshot "*MyTests.MyTest.snapshot.txt",
					             but the snapshot could not be read: The path "/_9/Tests/MyTests.cs" comes from a deterministic build, but the test assembly records no source root for it, which the build targets of aweXpect.Snapshots add.
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenDirectoryDoesNotExist_ShouldReportThatThePendingSnapshotCouldNotBeWritten()
			{
				using SnapshotDirectory directory = new();
				string testFilePath = Path.Combine(directory.Path, "missing", "MyTests.cs");
				string snapshotPath = Path.Combine(directory.Path, "missing", "MyTests.MyTest.snapshot.txt");
				string subject = "foo";

				async Task Act()
					=> await That(subject).MatchesSnapshot(testFilePath, SnapshotDirectory.MemberName);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              matches the snapshot "{snapshotPath}",
					              but the snapshot did not exist

					              Actual:
					              foo

					              Pending snapshot:
					              could not be written: *
					              """).AsWildcard();
			}

			[Fact]
			public async Task WhenNamed_ShouldAppendTheNameToTheFileNames()
			{
				using SnapshotDirectory directory = new();
				string snapshotPath = Path.Combine(directory.Path, "MyTests.MyTest.login.snapshot.txt");
				string pendingPath = Path.Combine(directory.Path, "MyTests.MyTest.login.snapshot.pending.txt");
				string subject = "foo";

				async Task Act()
					=> await That(subject).MatchesSnapshot(directory.TestFilePath, SnapshotDirectory.MemberName)
						.Named("login");

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              matches the snapshot "{snapshotPath}",
					              but the snapshot did not exist

					              Actual:
					              foo

					              Pending snapshot:
					              {pendingPath}
					              """);
				File.Move(pendingPath, snapshotPath);
				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenPendingSnapshotCannotBeDeleted_AndSnapshotMatches_ShouldSucceed()
			{
				using SnapshotDirectory directory = new();
				directory.WriteSnapshot("foo");
				directory.WritePending("bar");
				using FileStream lockedPending = new(directory.PendingPath, FileMode.Open, FileAccess.Read,
					FileShare.None);
				string subject = "foo";

				async Task Act()
					=> await That(subject).MatchesSnapshot(directory.TestFilePath, SnapshotDirectory.MemberName);

				await That(Act).DoesNotThrow()
					.Because("the snapshot matched, and the pending snapshot is only a leftover of an earlier run");
			}

			[Fact]
			public async Task WhenPendingSnapshotExists_AndSnapshotDiffers_ShouldOverwritePendingSnapshot()
			{
				using SnapshotDirectory directory = new();
				directory.WriteSnapshot("foo");
				directory.WritePending("a much longer outdated value");
				string subject = "bar";

				async Task Act()
					=> await That(subject).MatchesSnapshot(directory.TestFilePath, SnapshotDirectory.MemberName);

				await That(Act).Throws<XunitException>()
					.WithMessage("*Pending snapshot:*").AsWildcard();
				await That(File.ReadAllText(directory.PendingPath)).IsEqualTo("bar" + Environment.NewLine);
			}

			[Fact]
			public async Task WhenPendingSnapshotExists_AndSnapshotMatches_ShouldDeletePendingSnapshot()
			{
				using SnapshotDirectory directory = new();
				directory.WriteSnapshot("foo");
				directory.WritePending("bar");
				string subject = "foo";

				await That(subject).MatchesSnapshot(directory.TestFilePath, SnapshotDirectory.MemberName);

				await That(File.Exists(directory.PendingPath)).IsFalse();
			}

			[Theory]
			[InlineData("")]
			[InlineData("foo")]
			[InlineData("foo\n")]
			[InlineData("foo\r\n")]
			[InlineData("\n\nfoo\n\n")]
			public async Task WhenPendingSnapshotIsAccepted_ShouldSucceed(string subject)
			{
				using SnapshotDirectory directory = new();

				async Task Act()
					=> await That(subject).MatchesSnapshot(directory.TestFilePath, SnapshotDirectory.MemberName);

				await That(Act).Throws<XunitException>()
					.WithMessage("*the snapshot did not exist*").AsWildcard();
				File.Move(directory.PendingPath, directory.SnapshotPath);
				await That(Act).DoesNotThrow()
					.Because("renaming the pending snapshot accepts the subject unchanged");
			}

			[Fact]
			public async Task WhenSnapshotCannotBeRead_ShouldFailWithoutWritingPendingSnapshot()
			{
				using SnapshotDirectory directory = new();
				directory.WriteSnapshot("bar");
				using FileStream lockedSnapshot = new(directory.SnapshotPath, FileMode.Open, FileAccess.Read,
					FileShare.None);
				string subject = "foo";

				async Task Act()
					=> await That(subject).MatchesSnapshot(directory.TestFilePath, SnapshotDirectory.MemberName);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              matches the snapshot "{directory.SnapshotPath}",
					              but the snapshot could not be read: *
					              """).AsWildcard();
				await That(File.Exists(directory.PendingPath)).IsFalse()
					.Because("the subject could not be compared with the snapshot");
			}

			[Fact]
			public async Task WhenSnapshotDiffers_ShouldFail()
			{
				using SnapshotDirectory directory = new();
				directory.WriteSnapshot("foo bar");
				string subject = "foo baz";

				async Task Act()
					=> await That(subject).MatchesSnapshot(directory.TestFilePath, SnapshotDirectory.MemberName);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              matches the snapshot "{directory.SnapshotPath}",
					              but it was "foo baz", which differs at index 6:
					                       ↓ (actual)
					                "foo baz"
					                "foo bar"
					                       ↑ (expected)

					              Pending snapshot:
					              {directory.PendingPath}
					              """);
			}

			[Fact]
			public async Task WhenSnapshotDiffers_ShouldWritePendingSnapshot()
			{
				using SnapshotDirectory directory = new();
				directory.WriteSnapshot("foo");
				string subject = "bar";

				async Task Act()
					=> await That(subject).MatchesSnapshot(directory.TestFilePath, SnapshotDirectory.MemberName);

				await That(Act).Throws<XunitException>()
					.WithMessage("*Pending snapshot:*").AsWildcard();
				await That(File.ReadAllText(directory.PendingPath)).IsEqualTo("bar" + Environment.NewLine);
				await That(File.ReadAllText(directory.SnapshotPath)).IsEqualTo("foo")
					.Because("the snapshot is only changed by the user");
			}

			[Theory]
			[InlineData("foo\n")]
			[InlineData("foo\r\n")]
			public async Task WhenSnapshotEndsWithLineBreak_ShouldIgnoreIt(string snapshot)
			{
				using SnapshotDirectory directory = new();
				directory.WriteSnapshot(snapshot);
				string subject = "foo";

				async Task Act()
					=> await That(subject).MatchesSnapshot(directory.TestFilePath, SnapshotDirectory.MemberName);

				await That(Act).DoesNotThrow()
					.Because("an editor can add a final line break when it saves the snapshot");
			}

			[Fact]
			public async Task WhenSnapshotHasByteOrderMark_ShouldSucceed()
			{
				using SnapshotDirectory directory = new();
				directory.WriteSnapshot("äöü", true);
				string subject = "äöü";

				async Task Act()
					=> await That(subject).MatchesSnapshot(directory.TestFilePath, SnapshotDirectory.MemberName);

				await That(Act).DoesNotThrow()
					.Because("an editor can save the snapshot with a byte order mark");
			}

			[Fact]
			public async Task WhenSnapshotIsEmpty_AndSubjectIsEmpty_ShouldSucceed()
			{
				using SnapshotDirectory directory = new();
				directory.WriteSnapshot("");
				string subject = "";

				async Task Act()
					=> await That(subject).MatchesSnapshot(directory.TestFilePath, SnapshotDirectory.MemberName);

				await That(Act).DoesNotThrow()
					.Because("an empty snapshot file exists, so it is not a missing snapshot");
			}

			[Fact]
			public async Task WhenSnapshotIsMissing_ShouldFail()
			{
				using SnapshotDirectory directory = new();
				string subject = "foo";

				async Task Act()
					=> await That(subject).MatchesSnapshot(directory.TestFilePath, SnapshotDirectory.MemberName);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              matches the snapshot "{directory.SnapshotPath}",
					              but the snapshot did not exist

					              Actual:
					              foo

					              Pending snapshot:
					              {directory.PendingPath}
					              """);
			}

			[Fact]
			public async Task WhenSnapshotIsMissing_ShouldWritePendingSnapshotAsUtf8WithoutByteOrderMark()
			{
				using SnapshotDirectory directory = new();
				string subject = "äöü\r\nfoo";

				async Task Act()
					=> await That(subject).MatchesSnapshot(directory.TestFilePath, SnapshotDirectory.MemberName);

				await That(Act).Throws<XunitException>()
					.WithMessage("*Pending snapshot:*").AsWildcard();
				await That(File.ReadAllBytes(directory.PendingPath))
					.IsEqualTo(new System.Text.UTF8Encoding(false).GetBytes(subject + Environment.NewLine));
			}

			[Fact]
			public async Task WhenSnapshotMatches_ShouldSucceed()
			{
				using SnapshotDirectory directory = new();
				directory.WriteSnapshot("foo\nbar");
				string subject = "foo\nbar";

				async Task Act()
					=> await That(subject).MatchesSnapshot(directory.TestFilePath, SnapshotDirectory.MemberName);

				await That(Act).DoesNotThrow();
				await That(File.Exists(directory.PendingPath)).IsFalse();
			}
		}
	}
}
