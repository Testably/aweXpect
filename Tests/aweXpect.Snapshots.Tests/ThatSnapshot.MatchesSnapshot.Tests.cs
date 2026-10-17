using System.IO;
using aweXpect.Snapshots.Tests.TestHelpers;

namespace aweXpect.Snapshots.Tests;

public sealed partial class ThatSnapshot
{
	public sealed class MatchesSnapshot
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenCalledWithoutCallerInformation_ShouldUseTestFileAndMethodName()
			{
				string subject = "stored next to the test file";

				async Task Act()
					=> await That(subject).MatchesSnapshot();

				await That(Act).DoesNotThrow()
					.Because(
						"the snapshot 'ThatSnapshot.MatchesSnapshot.Tests.WhenCalledWithoutCallerInformation_ShouldUseTestFileAndMethodName.snapshot.txt' is committed next to this test file");
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
			public async Task WhenNegated_AndSnapshotCannotBeRead_ShouldFail()
			{
				using SnapshotDirectory directory = new();
				directory.WriteSnapshot("bar");
				using FileStream lockedSnapshot = new(directory.SnapshotPath, FileMode.Open, FileAccess.Read,
					FileShare.None);
				string subject = "foo";

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it
						=> it.MatchesSnapshot(directory.TestFilePath, SnapshotDirectory.MemberName));

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              does not match the snapshot "{directory.SnapshotPath}",
					              but a snapshot cannot be negated
					              """);
			}

			[Fact]
			public async Task WhenNegated_AndSnapshotDiffers_ShouldFail()
			{
				using SnapshotDirectory directory = new();
				directory.WriteSnapshot("bar");
				string subject = "foo";

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it
						=> it.MatchesSnapshot(directory.TestFilePath, SnapshotDirectory.MemberName));

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              does not match the snapshot "{directory.SnapshotPath}",
					              but a snapshot cannot be negated
					              """)
					.Because("accepting the pending snapshot would turn the success into a failure");
			}

			[Fact]
			public async Task WhenNegated_AndSnapshotIsMissing_ShouldFail()
			{
				using SnapshotDirectory directory = new();
				string subject = "foo";

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it
						=> it.MatchesSnapshot(directory.TestFilePath, SnapshotDirectory.MemberName));

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              does not match the snapshot "{directory.SnapshotPath}",
					              but a snapshot cannot be negated
					              """);
			}

			[Fact]
			public async Task WhenNegated_AndSnapshotMatches_ShouldFail()
			{
				using SnapshotDirectory directory = new();
				directory.WriteSnapshot("foo");
				string subject = "foo";

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it
						=> it.MatchesSnapshot(directory.TestFilePath, SnapshotDirectory.MemberName));

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              does not match the snapshot "{directory.SnapshotPath}",
					              but a snapshot cannot be negated
					              """);
			}

			[Fact]
			public async Task WhenNegated_AndSubjectIsNull_ShouldFail()
			{
				using SnapshotDirectory directory = new();
				string? subject = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it
						=> it.MatchesSnapshot(directory.TestFilePath, SnapshotDirectory.MemberName));

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              does not match the snapshot "{directory.SnapshotPath}",
					              but it was <null>
					              """);
				await That(File.Exists(directory.PendingPath)).IsFalse();
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

			[Fact]
			public async Task WhenSnapshotDiffersInSeveralLines_ShouldListTheDifferences()
			{
				using SnapshotDirectory directory = new();
				directory.WriteSnapshot("a\nb\nc\nd");
				string subject = "a\nB\nc\nD";

				async Task Act()
					=> await That(subject).MatchesSnapshot(directory.TestFilePath, SnapshotDirectory.MemberName);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              matches the snapshot "{directory.SnapshotPath}",
					              but it was "a\nB\nc\nD", which differs on line 2 and column 1:
					                    ↓ (actual)
					                "a\nB\nc\nD"
					                "a\nb\nc\nd"
					                    ↑ (expected)

					              Differences:
					                   1   1    a
					                -  2        B
					                +      2    b
					                   3   3    c
					                -  4        D
					                +      4    d

					              Pending snapshot:
					              {directory.PendingPath}
					              """);
			}

			[Fact]
			public async Task WhenSnapshotDiffersOnlyInNewlineStyle_ShouldSucceed()
			{
				using SnapshotDirectory directory = new();
				directory.WriteSnapshot("foo\r\nbar");
				string subject = "foo\nbar";

				async Task Act()
					=> await That(subject).MatchesSnapshot(directory.TestFilePath, SnapshotDirectory.MemberName);

				await That(Act).DoesNotThrow()
					.Because("version control can change the newline style when the snapshot is checked out");
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

				await That(Act).DoesNotThrow();
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

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFailWithoutWritingPendingSnapshot()
			{
				using SnapshotDirectory directory = new();
				string? subject = null;

				async Task Act()
					=> await That(subject).MatchesSnapshot(directory.TestFilePath, SnapshotDirectory.MemberName);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              matches the snapshot "{directory.SnapshotPath}",
					              but it was <null>
					              """);
				await That(File.Exists(directory.PendingPath)).IsFalse();
			}
		}
	}
}
