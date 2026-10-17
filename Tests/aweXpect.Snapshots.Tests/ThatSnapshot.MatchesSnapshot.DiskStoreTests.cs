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
						"the snapshot 'ThatSnapshot.MatchesSnapshot.DiskStoreTests.WhenCalledWithoutCallerInformation_ShouldUseTestFileAndMethodName.awexpect-snapshot.txt' is committed next to this test file");
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
						.Accepting(SnapshotAcceptance.All);

				await That(Act).Throws<XunitException>()
					.WithMessage("*Accepted snapshot:*").AsWildcard();
				await That(Act).DoesNotThrow()
					.Because("the accepted snapshot is read back unchanged");
			}

			[Fact]
			public async Task WhenAccepting_AndDirectoryDoesNotExist_ShouldReportThatNeitherSnapshotCouldBeWritten()
			{
				using SnapshotDirectory directory = new();
				string testFilePath = Path.Combine(directory.Path, "missing", "MyTests.cs");
				string subject = "foo";

				async Task Act()
					=> await That(subject).MatchesSnapshot(testFilePath, SnapshotDirectory.MemberName)
						.Accepting(SnapshotAcceptance.All);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             *
					             Accepted snapshot:
					             could not be written: *

					             Pending snapshot:
					             could not be written: *
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenAccepting_AndSnapshotIsReadOnly_ShouldWriteThePendingSnapshotInstead()
			{
				using SnapshotDirectory directory = new();
				directory.WriteSnapshot("foo");
				File.SetAttributes(directory.SnapshotPath, FileAttributes.ReadOnly);
				try
				{
					string subject = "bar";

					async Task Act()
						=> await That(subject).MatchesSnapshot(directory.TestFilePath, SnapshotDirectory.MemberName)
							.Accepting(SnapshotAcceptance.All);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              *
						              Accepted snapshot:
						              could not be written: *

						              Pending snapshot:
						              {directory.PendingPath}
						              """).AsWildcard();
					await That(File.ReadAllText(directory.PendingPath)).IsEqualTo("bar" + Environment.NewLine)
						.Because("a version control system like Perforce checks files out read-only, so the value is kept for review");
					await That(File.ReadAllText(directory.SnapshotPath)).IsEqualTo("foo");
				}
				finally
				{
					File.SetAttributes(directory.SnapshotPath, FileAttributes.Normal);
				}
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
						.Accepting(SnapshotAcceptance.All);

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
						.Accepting(SnapshotAcceptance.All);

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
						.For.Text("42").Mask("<id>").Accepting(SnapshotAcceptance.All);

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
						.For.Text(@"\d+").AsRegex().Mask("<id>");

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
					             matches the snapshot "*MyTests.MyTest.awexpect-snapshot.txt",
					             but the snapshot could not be read: The path "/_9/Tests/MyTests.cs" comes from a deterministic build, but the test assembly records no source root for it, which the build targets of aweXpect.Snapshots add.
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenDirectoryDoesNotExist_ShouldReportThatThePendingSnapshotCouldNotBeWritten()
			{
				using SnapshotDirectory directory = new();
				string testFilePath = Path.Combine(directory.Path, "missing", "MyTests.cs");
				string snapshotPath = Path.Combine(directory.Path, "missing", "MyTests.MyTest.awexpect-snapshot.txt");
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
			public async Task WhenOnlyASnapshotWithTheOldMarkerExists_ShouldNotReadIt()
			{
				using SnapshotDirectory directory = new();
				string oldSnapshotPath = Path.Combine(directory.Path, "MyTests.MyTest.snapshot.txt");
				File.WriteAllText(oldSnapshotPath, "foo");
				string subject = "foo";

				async Task Act()
					=> await That(subject).MatchesSnapshot(directory.TestFilePath, SnapshotDirectory.MemberName);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              matches the snapshot "{directory.SnapshotPath}",
					              but the snapshot did not exist
					              *
					              """).AsWildcard()
					.Because("there is no fallback for the '.snapshot.' files of version 0.1");
				await That(File.Exists(directory.PendingPath)).IsTrue();
				await That(File.ReadAllText(oldSnapshotPath)).IsEqualTo("foo");
			}

			[Theory]
			[InlineData("de-DE", "de-DE")]
			[InlineData("äöü", "äöü")]
			[InlineData("10:30", "10%3A30")]
			[InlineData("v1.2", "v1%2E2")]
			[InlineData("50%", "50%25")]
			[InlineData("a<b>c\"d/e\\f|g?h*i", "a%3Cb%3Ec%22d%2Fe%5Cf%7Cg%3Fh%2Ai")]
			[InlineData("a\tb", "a%09b")]
			[InlineData("AC@DC", "AC%40DC")]
			public async Task WhenNamed_ShouldEscapeTheNameInTheFileNames(string name, string escapedName)
			{
				using SnapshotDirectory directory = new();
				string snapshotPath = Path.Combine(directory.Path, $"MyTests.MyTest.{escapedName}.awexpect-snapshot.txt");
				string pendingPath = Path.Combine(directory.Path, $"MyTests.MyTest.{escapedName}.awexpect-snapshot.pending.txt");
				string subject = "foo";

				async Task Act()
					=> await That(subject).MatchesSnapshot(directory.TestFilePath, SnapshotDirectory.MemberName)
						.Named(name);

				await That(Act).Throws<XunitException>()
					.WithMessage($"*\"{snapshotPath}\"*").AsWildcard();
				await That(File.Exists(pendingPath)).IsTrue();
				File.Move(pendingPath, snapshotPath);
				await That(Act).DoesNotThrow()
					.Because("the escaped file name is valid on every operating system and leads back to the snapshot");
			}

			[Fact]
			public async Task WhenNamesOnlyDifferInAnEscapedCharacter_ShouldKeepTheirFilesApart()
			{
				using SnapshotDirectory directory = new();

				await That(async () => await That("first").MatchesSnapshot(directory.TestFilePath,
					SnapshotDirectory.MemberName).Named("a:b")).Throws<XunitException>();
				await That(async () => await That("second").MatchesSnapshot(directory.TestFilePath,
					SnapshotDirectory.MemberName).Named("a%3Ab")).Throws<XunitException>();

				await That(File.ReadAllText(Path.Combine(directory.Path, "MyTests.MyTest.a%3Ab.awexpect-snapshot.pending.txt")))
					.IsEqualTo("first" + Environment.NewLine);
				await That(File.ReadAllText(Path.Combine(directory.Path, "MyTests.MyTest.a%253Ab.awexpect-snapshot.pending.txt")))
					.IsEqualTo("second" + Environment.NewLine)
					.Because("the '%' is escaped as well, so that two names never share a file");
			}

			[Fact]
			public async Task WhenMemberNameContainsCharactersAFileNameCannotHold_ShouldEscapeThem()
			{
				using SnapshotDirectory directory = new();
				string subject = "foo";

				async Task Act()
					=> await That(subject).MatchesSnapshot(directory.TestFilePath, "<Main>$");

				await That(Act).Throws<XunitException>();
				await That(File.Exists(Path.Combine(directory.Path, "MyTests.%3CMain%3E$.awexpect-snapshot.pending.txt"))).IsTrue()
					.Because("top-level statements report the member name '<Main>$'");
			}

			[Fact]
			public async Task WhenNamed_ShouldAppendTheNameToTheFileNames()
			{
				using SnapshotDirectory directory = new();
				string snapshotPath = Path.Combine(directory.Path, "MyTests.MyTest.login.awexpect-snapshot.txt");
				string pendingPath = Path.Combine(directory.Path, "MyTests.MyTest.login.awexpect-snapshot.pending.txt");
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
			public async Task WhenNamedAfterArguments_ShouldAppendTheirNamesAndValuesToTheFileNames()
			{
				using SnapshotDirectory directory = new();
				string snapshotPath = Path.Combine(directory.Path, "MyTests.MyTest.Artist=AC%2FDC_year=1979.awexpect-snapshot.txt");
				string pendingPath =
					Path.Combine(directory.Path, "MyTests.MyTest.Artist=AC%2FDC_year=1979.awexpect-snapshot.pending.txt");
				(string Artist, string Album) release = ("AC/DC", "Highway to Hell");
				int year = 1979;

				async Task Act()
					=> await That("foo").MatchesSnapshot(directory.TestFilePath, SnapshotDirectory.MemberName)
						.NamedAfter(release.Artist, year);

				await That(Act).Throws<XunitException>()
					.WithMessage($"*\"{snapshotPath}\"*").AsWildcard();
				File.Move(pendingPath, snapshotPath);
				await That(Act).DoesNotThrow()
					.Because("the name is escaped like any other, but a member access adds no dot to it");
			}

			[Fact]
			public async Task WhenUniqueForTheEnvironment_ShouldAppendTheVariantToTheFileNames()
			{
				using SnapshotDirectory directory = new();
				string variant = $"{CurrentEnvironment.OperatingSystem}@{CurrentEnvironment.Architecture}";
				string snapshotPath = Path.Combine(directory.Path, $"MyTests.MyTest.Abbey Road@{variant}.awexpect-snapshot.txt");
				string pendingPath = Path.Combine(directory.Path, $"MyTests.MyTest.Abbey Road@{variant}.awexpect-snapshot.pending.txt");
				string subject = "foo";

				async Task Act()
					=> await That(subject).MatchesSnapshot(directory.TestFilePath, SnapshotDirectory.MemberName)
						.Named("Abbey Road")
						.UniqueFor.Architecture()
						.UniqueFor.OperatingSystem()
						.Accepting(SnapshotAcceptance.None);

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
			public async Task WhenUniqueForTargetFramework_ShouldReadTheSnapshotOfTheTargetFramework()
			{
				string subject = CurrentEnvironment.TargetFramework == "net48"
					? "Let It Be on .NET Framework"
					: "Let It Be on .NET";

				async Task Act()
					=> await That(subject).MatchesSnapshot().UniqueFor.TargetFramework();

				await That(Act).DoesNotThrow()
					.Because(
						$"the snapshot 'ThatSnapshot.MatchesSnapshot.DiskStoreTests.WhenUniqueForTargetFramework_ShouldReadTheSnapshotOfTheTargetFramework@{CurrentEnvironment.TargetFramework}.awexpect-snapshot.txt' is committed next to this test file for each target framework");
			}

			[Fact]
			public async Task WithExtension_ShouldReadTheCommittedSnapshotWithTheExtension()
			{
				string subject = """
				                 {
				                   "title": "Abbey Road",
				                   "year": 1969
				                 }
				                 """;

				async Task Act()
					=> await That(subject).MatchesSnapshot().WithExtension("json");

				await That(Act).DoesNotThrow()
					.Because(
						"the snapshot 'ThatSnapshot.MatchesSnapshot.DiskStoreTests.WithExtension_ShouldReadTheCommittedSnapshotWithTheExtension.awexpect-snapshot.json' is committed next to this test file");
			}

			[Fact]
			public async Task WithExtension_ShouldUseItInTheFileNames()
			{
				using SnapshotDirectory directory = new();
				string snapshotPath = Path.Combine(directory.Path, "MyTests.MyTest.awexpect-snapshot.json");
				string pendingPath = Path.Combine(directory.Path, "MyTests.MyTest.awexpect-snapshot.pending.json");
				string subject = "{}";

				async Task Act()
					=> await That(subject).MatchesSnapshot(directory.TestFilePath, SnapshotDirectory.MemberName)
						.WithExtension("json");

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              matches the snapshot "{snapshotPath}",
					              but the snapshot did not exist

					              Actual:
					              {"{}"}

					              Pending snapshot:
					              {pendingPath}
					              """);
				await That(File.Exists(directory.PendingPath)).IsFalse()
					.Because("the pending file has the extension of the snapshot");
				File.Move(pendingPath, snapshotPath);
				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithExtension_WhenNamedAndUniqueForTheEnvironment_ShouldAppendTheExtensionAfterSnapshot()
			{
				using SnapshotDirectory directory = new();
				string baseName = $"MyTests.MyTest.Abbey Road@{CurrentEnvironment.OperatingSystem}";
				string snapshotPath = Path.Combine(directory.Path, $"{baseName}.awexpect-snapshot.html");
				string pendingPath = Path.Combine(directory.Path, $"{baseName}.awexpect-snapshot.pending.html");
				string subject = "<h1>Abbey Road</h1>";

				async Task Act()
					=> await That(subject).MatchesSnapshot(directory.TestFilePath, SnapshotDirectory.MemberName)
						.Named("Abbey Road")
						.UniqueFor.OperatingSystem()
						.WithExtension("html");

				await That(Act).Throws<XunitException>()
					.WithMessage($"*\"{snapshotPath}\"*Pending snapshot:*{pendingPath}").AsWildcard();
				File.Move(pendingPath, snapshotPath);
				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithExtension_WhenAccepting_ShouldWriteTheSnapshotAndDeleteThePendingSnapshot()
			{
				using SnapshotDirectory directory = new();
				string snapshotPath = Path.Combine(directory.Path, "MyTests.MyTest.awexpect-snapshot.xml");
				string pendingPath = Path.Combine(directory.Path, "MyTests.MyTest.awexpect-snapshot.pending.xml");
				File.WriteAllText(pendingPath, "<outdated />");
				directory.WriteSnapshot("<album />");
				string subject = "<album />";

				async Task Act()
					=> await That(subject).MatchesSnapshot(directory.TestFilePath, SnapshotDirectory.MemberName)
						.WithExtension("xml").Accepting(SnapshotAcceptance.All);

				await That(Act).Throws<XunitException>()
					.WithMessage($"*Accepted snapshot:*{snapshotPath}").AsWildcard()
					.Because("the snapshot with the extension 'txt' is another file");
				await That(File.ReadAllText(snapshotPath)).IsEqualTo(subject + Environment.NewLine);
				await That(File.Exists(pendingPath)).IsFalse();
				await That(File.Exists(directory.SnapshotPath)).IsTrue()
					.Because("only the snapshot with the extension is written");
				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithExtension_WhenSnapshotMatches_ShouldDeleteThePendingSnapshotWithTheExtension()
			{
				using SnapshotDirectory directory = new();
				string snapshotPath = Path.Combine(directory.Path, "MyTests.MyTest.awexpect-snapshot.json");
				string pendingPath = Path.Combine(directory.Path, "MyTests.MyTest.awexpect-snapshot.pending.json");
				File.WriteAllText(snapshotPath, "[]");
				File.WriteAllText(pendingPath, "[1]");
				directory.WritePending("[2]");

				await That("[]").MatchesSnapshot(directory.TestFilePath, SnapshotDirectory.MemberName)
					.WithExtension("json");

				await That(File.Exists(pendingPath)).IsFalse();
				await That(File.Exists(directory.PendingPath)).IsTrue()
					.Because("the pending snapshot with the extension 'txt' belongs to another snapshot");
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

			[Fact]
			public async Task WhenSubjectIsAnObject_ShouldWriteItToThePendingSnapshotForAcceptance()
			{
				using SnapshotDirectory directory = new();
				SnapshotLine subject = new("Pen", 2, 1.5m);

				async Task Act()
					=> await That(subject).MatchesSnapshot(directory.TestFilePath, SnapshotDirectory.MemberName);

				await That(Act).Throws<XunitException>()
					.WithMessage("*the snapshot did not exist*").AsWildcard();
				await That(File.ReadAllText(directory.PendingPath)).IsEqualTo("""
				                                                              SnapshotLine {
				                                                                Price = 1.5,
				                                                                Product = "Pen",
				                                                                Quantity = 2
				                                                              }

				                                                              """).IgnoringNewlineStyle();
				File.Move(directory.PendingPath, directory.SnapshotPath);
				await That(Act).DoesNotThrow()
					.Because("renaming the pending snapshot accepts the written object");
			}

			[Fact]
			public async Task WithBinaryData_ShouldReadTheCommittedSnapshot()
			{
				byte[] subject = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 0x00, 0xFF,];

				async Task Act()
					=> await That(subject).MatchesSnapshot().WithExtension("bin");

				await That(Act).DoesNotThrow()
					.Because(
						"the snapshot 'ThatSnapshot.MatchesSnapshot.DiskStoreTests.WithBinaryData_ShouldReadTheCommittedSnapshot.awexpect-snapshot.bin' is committed next to this test file, with both line break styles that a text snapshot would normalize");
			}

			[Fact]
			public async Task WithBinaryData_ShouldWriteThePendingSnapshotUnchanged()
			{
				using SnapshotDirectory directory = new();
				string snapshotPath = Path.Combine(directory.Path, "MyTests.MyTest.awexpect-snapshot.png");
				string pendingPath = Path.Combine(directory.Path, "MyTests.MyTest.awexpect-snapshot.pending.png");
				byte[] subject = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A,];

				async Task Act()
					=> await That(subject).MatchesSnapshot(directory.TestFilePath, SnapshotDirectory.MemberName)
						.WithExtension("png");

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              matches the snapshot "{snapshotPath}",
					              but the snapshot did not exist

					              Pending snapshot:
					              {pendingPath}
					              """);
				await That(File.ReadAllBytes(pendingPath)).IsEqualTo(subject)
					.Because("binary data is written without the encoding and the final line break of a text snapshot");
				File.Move(pendingPath, snapshotPath);
				await That(Act).DoesNotThrow()
					.Because("renaming the pending snapshot accepts the bytes unchanged");
			}

			[Fact]
			public async Task WithBinaryData_WhenAccepting_ShouldWriteTheSnapshotUnchangedAndDeleteThePendingSnapshot()
			{
				using SnapshotDirectory directory = new();
				string snapshotPath = Path.Combine(directory.Path, "MyTests.MyTest.awexpect-snapshot.png");
				string pendingPath = Path.Combine(directory.Path, "MyTests.MyTest.awexpect-snapshot.pending.png");
				File.WriteAllBytes(snapshotPath, [1, 2,]);
				File.WriteAllBytes(pendingPath, [3,]);
				byte[] subject = [1, 2, 3,];

				async Task Act()
					=> await That(subject).MatchesSnapshot(directory.TestFilePath, SnapshotDirectory.MemberName)
						.WithExtension("png").Accepting(SnapshotAcceptance.All);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              matches the snapshot "{snapshotPath}",
					              but it differed at byte 2 (the snapshot has 2 bytes, the subject 3)

					              Accepted snapshot:
					              {snapshotPath}
					              """);
				await That(File.ReadAllBytes(snapshotPath)).IsEqualTo(subject);
				await That(File.Exists(pendingPath)).IsFalse();
				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithFileInfo_ShouldNameTheSnapshotWithTheExtensionOfTheFile()
			{
				using SnapshotDirectory directory = new();
				using SnapshotDirectory output = new();
				string snapshotPath = Path.Combine(directory.Path, "MyTests.MyTest.awexpect-snapshot.pdf");
				string pendingPath = Path.Combine(directory.Path, "MyTests.MyTest.awexpect-snapshot.pending.pdf");
				FileInfo subject = new(Path.Combine(output.Path, "liner-notes.pdf"));
				File.WriteAllBytes(subject.FullName, [0x25, 0x50, 0x44, 0x46,]);

				async Task Act()
					=> await That(subject).MatchesSnapshot(directory.TestFilePath, SnapshotDirectory.MemberName);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              matches the snapshot "{snapshotPath}",
					              but the snapshot did not exist

					              Pending snapshot:
					              {pendingPath}
					              """);
				await That(File.ReadAllBytes(pendingPath)).IsEqualTo(File.ReadAllBytes(subject.FullName));
				File.Move(pendingPath, snapshotPath);
				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithFileInfo_WhenTheFileDoesNotExist_ShouldNameTheSnapshotWithTheExtensionOfTheFile()
			{
				using SnapshotDirectory directory = new();
				string snapshotPath = Path.Combine(directory.Path, "MyTests.MyTest.awexpect-snapshot.pdf");
				FileInfo subject = new(Path.Combine(directory.Path, "liner-notes.pdf"));

				async Task Act()
					=> await That(subject).MatchesSnapshot(directory.TestFilePath, SnapshotDirectory.MemberName);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              matches the snapshot "{snapshotPath}",
					              but the file "{subject.FullName}" did not exist
					              """);
			}
		}
	}
}
