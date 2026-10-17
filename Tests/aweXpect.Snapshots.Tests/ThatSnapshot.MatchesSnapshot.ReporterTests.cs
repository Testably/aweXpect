using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using aweXpect.Customization;
using aweXpect.Snapshots.Tests.TestHelpers;

namespace aweXpect.Snapshots.Tests;

public sealed partial class ThatSnapshot
{
	public sealed partial class MatchesSnapshot
	{
		/// <remarks>
		///     The reporters are never called on a build server, so the tests clear its environment variables.
		/// </remarks>
		[Collection(nameof(EnvironmentCollection))]
		public sealed class ReporterTests : IDisposable
		{
			private readonly EnvironmentScope _environment = new();

			private SnapshotId Id { get; } = new($"/src/{Guid.NewGuid():N}/MyTests.cs", "MyTest");

			public void Dispose() => _environment.Dispose();

			[Fact]
			public async Task WhenSnapshotIsMissing_ShouldReportTheRecordedMismatchWithoutSnapshot()
			{
				InMemorySnapshotStore store = new();
				RecordingReporter reporter = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				using CustomizationLifetime __ = Customize.aweXpect.Snapshots().Reporters.Set([reporter,]);
				string subject = "foo";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName);

				await That(Act).Throws<XunitException>();
				SnapshotMismatch mismatch = await That(reporter.Mismatches).HasSingle();
				await That(mismatch.Id).IsEqualTo(Id);
				await That(mismatch.Store).IsSameAs(store);
				await That(mismatch.Snapshot).IsNull();
				await That(mismatch.Actual.IsText).IsTrue();
				await That(mismatch.Actual.Text).IsEqualTo("foo");
				await That(mismatch.Actual.Bytes).IsSameAs(store.MismatchBytes[Id])
					.Because("the reporter gets the bytes that the store recorded");
				await That(reporter.Matches).IsEmpty();
			}

			[Fact]
			public async Task WhenSnapshotDiffers_ShouldReportTheSnapshotAndTheMaskedSubject()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id] = "bar";
				RecordingReporter reporter = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				using CustomizationLifetime __ = Customize.aweXpect.Snapshots().Reporters.Set([reporter,]);
				string subject = "id 2c1b7a3e-4a2f-4d4e-9a63-0e2b1b9d6f11";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).For.Guids().Mask();

				await That(Act).Throws<XunitException>();
				SnapshotMismatch mismatch = await That(reporter.Mismatches).HasSingle();
				await That(mismatch.Actual.Text).IsEqualTo("id {Guid_1}")
					.Because("the reporter gets the value that was compared");
				await That(mismatch.Snapshot!.IsText).IsTrue();
				await That(mismatch.Snapshot.Text).IsEqualTo("bar");
				await That(mismatch.Snapshot.Bytes).IsSameAs(store.SnapshotBytes[Id]);
			}

			[Fact]
			public async Task WhenBinarySnapshotDiffers_ShouldReportTheBytesWithoutText()
			{
				SnapshotId id = Id with
				{
					Extension = "bin",
				};
				InMemorySnapshotStore store = new();
				store.SnapshotBytes[id] = [1, 2,];
				RecordingReporter reporter = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				using CustomizationLifetime __ = Customize.aweXpect.Snapshots().Reporters.Set([reporter,]);
				byte[] subject = [1, 3,];

				async Task Act()
					=> await That(subject).MatchesSnapshot(id.SourceFile, id.MemberName).WithExtension("bin");

				await That(Act).Throws<XunitException>();
				SnapshotMismatch mismatch = await That(reporter.Mismatches).HasSingle();
				await That(mismatch.Id).IsEqualTo(id);
				await That(mismatch.Actual.IsText).IsFalse();
				await That(mismatch.Actual.Text).IsNull();
				await That(mismatch.Actual.Bytes).IsEqualTo([1, 3,]);
				await That(mismatch.Snapshot!.IsText).IsFalse();
				await That(mismatch.Snapshot.Bytes).IsEqualTo([1, 2,]);
			}

			[Fact]
			public async Task WhenSnapshotDiffers_ShouldReportAfterTheMismatchWasRecorded()
			{
				InMemorySnapshotStore store = new();
				bool isRecorded = false;
				RecordingReporter reporter = new()
				{
					OnCall = () => isRecorded = store.MismatchBytes.ContainsKey(Id),
				};
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				using CustomizationLifetime __ = Customize.aweXpect.Snapshots().Reporters.Set([reporter,]);
				string subject = "foo";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName);

				await That(Act).Throws<XunitException>();
				await That(isRecorded).IsTrue()
					.Because("a diff tool needs the recorded mismatch, e.g. the pending file");
			}

			[Fact]
			public async Task WhenRecordingTheMismatchThrows_ShouldNotReport()
			{
				InMemorySnapshotStore store = new()
				{
					RecordMismatchException = new IOException("disk full"),
				};
				RecordingReporter reporter = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				using CustomizationLifetime __ = Customize.aweXpect.Snapshots().Reporters.Set([reporter,]);
				string subject = "foo";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName);

				await That(Act).Throws<XunitException>()
					.WithMessage("*could not be written: disk full").AsWildcard();
				await That(reporter.Mismatches).IsEmpty()
					.Because("there is no recorded mismatch to review");
			}

			[Fact]
			public async Task WhenAccepting_ShouldNotReport()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id] = "bar";
				RecordingReporter reporter = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				using CustomizationLifetime __ = Customize.aweXpect.Snapshots().Reporters.Set([reporter,]);
				string subject = "foo";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.Accepting(SnapshotAcceptance.All);

				await That(Act).Throws<XunitException>();
				await That(reporter.Mismatches).IsEmpty()
					.Because("an accepted change needs no review");
			}

			[Fact]
			public async Task WhenAcceptingNew_AndSnapshotIsMissing_ShouldNotReport()
			{
				InMemorySnapshotStore store = new();
				RecordingReporter reporter = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				using CustomizationLifetime __ = Customize.aweXpect.Snapshots().Reporters.Set([reporter,]);
				string subject = "foo";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.Accepting(SnapshotAcceptance.New);

				await That(Act).Throws<XunitException>();
				await That(reporter.Mismatches).IsEmpty()
					.Because("the missing snapshot was accepted");
			}

			[Fact]
			public async Task WhenAcceptingNew_AndSnapshotDiffers_ShouldReportTheRecordedMismatch()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id] = "bar";
				RecordingReporter reporter = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				using CustomizationLifetime __ = Customize.aweXpect.Snapshots().Reporters.Set([reporter,]);
				string subject = "foo";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.Accepting(SnapshotAcceptance.New);

				await That(Act).Throws<XunitException>();
				await That(reporter.Mismatches).HasSingle()
					.Because("accepting new snapshots never replaces a different one, which is recorded for review");
			}

			[Fact]
			public async Task WhenAccepting_AndAcceptingThrows_ShouldReportTheRecordedMismatch()
			{
				InMemorySnapshotStore store = new()
				{
					AcceptException = new IOException("read-only"),
				};
				RecordingReporter reporter = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				using CustomizationLifetime __ = Customize.aweXpect.Snapshots().Reporters.Set([reporter,]);
				string subject = "foo";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.Accepting(SnapshotAcceptance.All);

				await That(Act).Throws<XunitException>();
				await That(reporter.Mismatches).HasSingle()
					.Because("the change was recorded for review instead");
			}

			[Fact]
			public async Task WhenNegated_ShouldNotReport()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id] = "bar";
				RecordingReporter reporter = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				using CustomizationLifetime __ = Customize.aweXpect.Snapshots().Reporters.Set([reporter,]);
				string subject = "foo";

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.MatchesSnapshot(Id.SourceFile, Id.MemberName));

				await That(Act).Throws<XunitException>();
				await That(reporter.Mismatches).IsEmpty()
					.Because("a negated snapshot records no mismatch");
			}

			[Fact]
			public async Task WhenOrBranchIsMet_ShouldNotReport()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id] = "bar";
				RecordingReporter reporter = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				using CustomizationLifetime __ = Customize.aweXpect.Snapshots().Reporters.Set([reporter,]);
				string subject = "foo";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).Or.IsEqualTo("foo");

				await That(Act).DoesNotThrow();
				await That(reporter.Mismatches).IsEmpty()
					.Because("the whole expectation is met, so no mismatch is recorded");
			}

			[Fact]
			public async Task WhenSnapshotMatches_ShouldReportTheMatchBeforeTheStoreRecordsIt()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id] = "foo";
				bool isRecorded = true;
				RecordingReporter reporter = new()
				{
					OnCall = () => isRecorded = store.Matches.Contains(Id),
				};
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				using CustomizationLifetime __ = Customize.aweXpect.Snapshots().Reporters.Set([reporter,]);
				string subject = "foo";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName);

				await That(Act).DoesNotThrow();
				SnapshotMatch match = await That(reporter.Matches).HasSingle();
				await That(match.Id).IsEqualTo(Id);
				await That(match.Store).IsSameAs(store);
				await That(isRecorded).IsFalse()
					.Because("a diff tool is closed before the store deletes the files it shows");
				await That(store.Matches).IsEqualTo([Id,]);
				await That(reporter.Mismatches).IsEmpty();
			}

			[Fact]
			public async Task WhenComparerConsidersEqual_ShouldReportAMatch()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id] = "foo";
				RecordingReporter reporter = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				using CustomizationLifetime __ = Customize.aweXpect.Snapshots().Reporters.Set([reporter,]);
				string subject = "bar";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.ComparedWith(FakeSnapshotComparer.Equal());

				await That(Act).DoesNotThrow();
				await That(reporter.Matches).HasSingle()
					.Because("the comparer decides whether the snapshot matches");
				await That(reporter.Mismatches).IsEmpty();
			}

			[Fact]
			public async Task WhenComparerReportsDifference_ShouldReportTheMismatch()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id] = "foo";
				RecordingReporter reporter = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				using CustomizationLifetime __ = Customize.aweXpect.Snapshots().Reporters.Set([reporter,]);
				string subject = "foo";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.ComparedWith(FakeSnapshotComparer.Different("louder"));

				await That(Act).Throws<XunitException>();
				SnapshotMismatch mismatch = await That(reporter.Mismatches).HasSingle();
				await That(mismatch.Snapshot!.Text).IsEqualTo("foo");
				await That(mismatch.Actual.Text).IsEqualTo("foo");
				await That(reporter.Matches).IsEmpty();
			}

			[Fact]
			public async Task WhenSeveralReporters_ShouldCallThemInOrder()
			{
				InMemorySnapshotStore store = new();
				List<string> log = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				using CustomizationLifetime __ = Customize.aweXpect.Snapshots().Reporters.Set([
					new RecordingReporter("first", log), new RecordingReporter("second", log),
				]);

				async Task Act()
					=> await That("foo").MatchesSnapshot(Id.SourceFile, Id.MemberName);

				await That(Act).Throws<XunitException>();
				store.Snapshots[Id] = "foo";
				await That(Act).DoesNotThrow();
				await That(log).IsEqualTo(["first: mismatch", "second: mismatch", "first: match", "second: match",]);
			}

			[Fact]
			public async Task WhenReporterThrowsOnMismatch_ShouldTraceAndIgnoreIt()
			{
				InMemorySnapshotStore store = new();
				InvalidOperationException exception = new("broken reporter");
				RecordingReporter throwingReporter = new()
				{
					Exception = exception,
				};
				RecordingReporter nextReporter = new();
				RecordingTraceWriter traceWriter = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				using CustomizationLifetime __ =
					Customize.aweXpect.Snapshots().Reporters.Set([throwingReporter, nextReporter,]);
				using CustomizationLifetime ___ = Customize.aweXpect.EnableTracing(traceWriter);
				string subject = "foo";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             matches the snapshot "MyTests.MyTest",
					             but the snapshot did not exist

					             Actual:
					             foo
					             """)
					.Because("a reporter never changes the result");
				await That(nextReporter.Mismatches).HasSingle();
				await That(traceWriter.Exceptions).Contains(exception);
			}

			[Fact]
			public async Task WhenReporterThrowsOnMatch_ShouldTraceAndIgnoreIt()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id] = "foo";
				InvalidOperationException exception = new("broken reporter");
				RecordingReporter throwingReporter = new()
				{
					Exception = exception,
				};
				RecordingReporter nextReporter = new();
				RecordingTraceWriter traceWriter = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				using CustomizationLifetime __ =
					Customize.aweXpect.Snapshots().Reporters.Set([throwingReporter, nextReporter,]);
				using CustomizationLifetime ___ = Customize.aweXpect.EnableTracing(traceWriter);
				string subject = "foo";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName);

				await That(Act).DoesNotThrow()
					.Because("a reporter never changes the result");
				await That(nextReporter.Matches).HasSingle();
				await That(store.Matches).IsEqualTo([Id,]);
				await That(traceWriter.Exceptions).Contains(exception);
			}

			[Fact]
			public async Task WithDiskStore_ShouldReportTheFilesOfTheSnapshot()
			{
				using SnapshotDirectory directory = new();
				bool pendingFileExists = false;
				RecordingReporter reporter = new();
				reporter.OnCall = () => pendingFileExists = File.Exists(directory.PendingPath);
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Reporters.Set([reporter,]);
				string subject = "foo";

				async Task Act()
					=> await That(subject).MatchesSnapshot(directory.TestFilePath, SnapshotDirectory.MemberName);

				await That(Act).Throws<XunitException>();
				SnapshotMismatch mismatch = await That(reporter.Mismatches).HasSingle();
				IFileSnapshotStore store = await That(mismatch.Store).Is<IFileSnapshotStore>();
				await That(store.GetPendingFile(mismatch.Id)).IsEqualTo(directory.PendingPath);
				await That(store.GetSnapshotFile(mismatch.Id)).IsEqualTo(directory.SnapshotPath);
				await That(pendingFileExists).IsTrue()
					.Because("the pending file is written before the reporters are called");
				await That(mismatch.Actual.Bytes).IsEqualTo(File.ReadAllBytes(directory.PendingPath));
			}

			[Fact]
			public async Task WithDiskStore_WhenSnapshotMatches_ShouldReportBeforeThePendingFileIsDeleted()
			{
				using SnapshotDirectory directory = new();
				directory.WriteSnapshot("foo");
				directory.WritePending("bar");
				bool pendingFileExists = false;
				RecordingReporter reporter = new();
				reporter.OnCall = () => pendingFileExists = File.Exists(directory.PendingPath);
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Reporters.Set([reporter,]);
				string subject = "foo";

				async Task Act()
					=> await That(subject).MatchesSnapshot(directory.TestFilePath, SnapshotDirectory.MemberName);

				await That(Act).DoesNotThrow();
				await That(pendingFileExists).IsTrue()
					.Because("a diff tool that shows the pending file is closed before it is deleted");
				await That(File.Exists(directory.PendingPath)).IsFalse();
			}

			[Fact]
			public async Task WithDiskStore_SnapshotContent_ShouldHaveTheEncodingOfTheStore()
			{
				using SnapshotDirectory directory = new();
				directory.WriteSnapshot("bar");
				RecordingReporter reporter = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Reporters.Set([reporter,]);
				string subject = "foo";

				async Task Act()
					=> await That(subject).MatchesSnapshot(directory.TestFilePath, SnapshotDirectory.MemberName);

				await That(Act).Throws<XunitException>();
				SnapshotMismatch mismatch = await That(reporter.Mismatches).HasSingle();
				await That(mismatch.Actual.Bytes)
					.IsEqualTo(new UTF8Encoding(false).GetBytes("foo" + Environment.NewLine))
					.Because("the text is encoded as UTF-8 with an additional line break, like the store writes it");
				await That(mismatch.Snapshot!.Bytes).IsEqualTo(File.ReadAllBytes(directory.SnapshotPath));
				await That(mismatch.Snapshot.Text).IsEqualTo("bar");
			}

			[Fact]
			public async Task Reporters_ShouldDefaultToNone()
			{
				IReadOnlyList<SnapshotReporter> reporters = Customize.aweXpect.Snapshots().Reporters.Get();

				await That(reporters).IsEmpty();
			}

			[Fact]
			public async Task Reporters_ShouldCopyTheList()
			{
				List<SnapshotReporter> reporters = [new RecordingReporter(),];
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Reporters.Set(reporters);

				reporters.Add(new RecordingReporter());

				await That(Customize.aweXpect.Snapshots().Reporters.Get()).HasCount(1)
					.Because("changing the list afterwards must not change the setting");
			}

			[Fact]
			public async Task Reporters_WhenSetToNull_ShouldThrowArgumentNullException()
			{
				void Act()
					=> Customize.aweXpect.Snapshots().Reporters.Set(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("value");
			}

			[Fact]
			public async Task Reporters_WhenContainingNull_ShouldThrowArgumentException()
			{
				void Act()
					=> Customize.aweXpect.Snapshots().Reporters.Set([new RecordingReporter(), null!,]);

				await That(Act).Throws<ArgumentException>()
					.WithParamName("value");
			}

			[Fact]
			public async Task DefaultReporter_ShouldDoNothing()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				using CustomizationLifetime __ = Customize.aweXpect.Snapshots().Reporters.Set([new EmptyReporter(),]);

				async Task Act()
					=> await That("foo").MatchesSnapshot(Id.SourceFile, Id.MemberName);

				await That(Act).Throws<XunitException>();
				store.Snapshots[Id] = "foo";
				await That(Act).DoesNotThrow();
				await That(store.Mismatches.Keys).IsEqualTo([Id,]);
			}

			[Theory]
			[InlineData("CI")]
			[InlineData("TF_BUILD")]
			[InlineData("JENKINS_URL")]
			[InlineData("TEAMCITY_VERSION")]
			public async Task OnBuildServer_ShouldNotReport(string buildServerVariable)
			{
				using EnvironmentScope environment = new((buildServerVariable, "true"));
				InMemorySnapshotStore store = new();
				RecordingReporter reporter = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				using CustomizationLifetime __ = Customize.aweXpect.Snapshots().Reporters.Set([reporter,]);

				async Task Act()
					=> await That("foo").MatchesSnapshot(Id.SourceFile, Id.MemberName);

				await That(Act).Throws<XunitException>();
				store.Snapshots[Id] = "foo";
				await That(Act).DoesNotThrow();
				await That(reporter.Mismatches.Concat<object>(reporter.Matches)).IsEmpty()
					.Because("no diff tool may open on a build server");
				await That(store.Mismatches.Keys).IsEqualTo([Id,])
					.Because("the mismatch is still recorded");
			}

			[Fact]
			public async Task WithoutBuildServer_ShouldReport()
			{
				using EnvironmentScope environment = new(("CI", "false"));
				InMemorySnapshotStore store = new();
				RecordingReporter reporter = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				using CustomizationLifetime __ = Customize.aweXpect.Snapshots().Reporters.Set([reporter,]);

				async Task Act()
					=> await That("foo").MatchesSnapshot(Id.SourceFile, Id.MemberName);

				await That(Act).Throws<XunitException>();
				await That(reporter.Mismatches).HasSingle();
			}

			private sealed class EmptyReporter : SnapshotReporter;
		}
	}
}
