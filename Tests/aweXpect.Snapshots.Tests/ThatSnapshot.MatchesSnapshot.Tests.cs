using System.Collections.Generic;
using System.IO;
using System.Threading;
using aweXpect.Customization;
using aweXpect.Snapshots.Tests.TestHelpers;
using TUnit.Core.Exceptions;

namespace aweXpect.Snapshots.Tests;

public sealed partial class ThatSnapshot
{
	public sealed partial class MatchesSnapshot
	{
		public sealed class Tests
		{
			private SnapshotId Id { get; } = new($"/src/{Guid.NewGuid():N}/MyTests.cs", "MyTest");

			[Test]
			public async Task ShouldIdentifyTheSnapshotByThePathRelativeToTheSourceRoot()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "foo";

				async Task Act()
					=> await That(subject).MatchesSnapshot();

				await That(Act).Throws<FailException>();
				await That(store.Mismatches.Keys).IsEqualTo(new[]
				{
					new SnapshotId("Tests/aweXpect.Snapshots.Tests/ThatSnapshot.MatchesSnapshot.Tests.cs",
						nameof(ShouldIdentifyTheSnapshotByThePathRelativeToTheSourceRoot)),
				}).InAnyOrder().Because("the build targets record the source root, so the id is the same on every machine");
			}

			[Test]
			public async Task WhenCallerFilePathIsDeterministic_ShouldAcceptIt()
			{
				SnapshotId id = new($"/_9/{Guid.NewGuid():N}/MyTests.cs", "MyTest");
				InMemorySnapshotStore store = new();
				store.Snapshots[id] = "foo";
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "foo";

				async Task Act()
					=> await That(subject).MatchesSnapshot(id.SourceFile, id.MemberName);

				await That(Act).DoesNotThrow()
					.Because("deterministic builds map the path of the test file to a rooted path like '/_9/…'");
			}

			[Test]
			public async Task WhenCallerFilePathIsNotRooted_ShouldThrowArgumentException()
			{
				string subject = "foo";

				async Task Act()
					=> await That(subject).MatchesSnapshot("login");

				await That(Act).Throws<ArgumentException>()
					.WithParamName("callerFilePath").And
					.WithMessage(
						"The 'callerFilePath' must be the rooted path of the test file, which the compiler sets. Name a snapshot with '.Named(…)' instead.*")
					.AsWildcard();
			}

			[Test]
			public async Task WhenNamed_ShouldAddTheNameToTheSnapshotId()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "foo";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).Named("login");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             matches the snapshot "MyTests.MyTest.login",
					             but the snapshot did not exist

					             Actual:
					             foo
					             """);
				await That(store.Mismatches.Keys).IsEqualTo(new[]
				{
					Id with { Name = "login", },
				}).InAnyOrder();
			}

			[Test]
			public async Task WhenNamedWithCharactersAFileNameCannotHold_ShouldPassTheNameUnchangedToTheStore()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id with { Name = "10:30/v1.2", }] = "foo";
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "foo";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).Named("10:30/v1.2");

				await That(Act).DoesNotThrow()
					.Because("only the disk store escapes names for its file names");
			}

			[Test]
			[Arguments("")]
			[Arguments(" ")]
			public async Task WhenNamedWithEmptyName_ShouldThrowArgumentException(string name)
			{
				string subject = "foo";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).Named(name);

				await That(Act).Throws<ArgumentException>()
					.WithParamName("name").And
					.WithMessage("The 'name' cannot be empty.*").AsWildcard();
			}

			[Test]
			public async Task WhenNamedWithNull_ShouldThrowArgumentNullException()
			{
				string subject = "foo";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).Named(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("name");
			}

			[Test]
			public async Task WhenNegated_AndSnapshotCannotBeRead_ShouldFail()
			{
				InMemorySnapshotStore store = new()
				{
					ReadException = new IOException("locked"),
				};
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "foo";

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.MatchesSnapshot(Id.SourceFile, Id.MemberName));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not match the snapshot "MyTests.MyTest",
					             but a snapshot cannot be negated
					             """);
			}

			[Test]
			public async Task WhenNegated_AndSnapshotDiffers_ShouldFail()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id] = "bar";
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "foo";

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.MatchesSnapshot(Id.SourceFile, Id.MemberName));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not match the snapshot "MyTests.MyTest",
					             but a snapshot cannot be negated
					             """)
					.Because("accepting the pending snapshot would turn the success into a failure");
			}

			[Test]
			public async Task WhenNegated_AndSnapshotIsMissing_ShouldFail()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "foo";

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.MatchesSnapshot(Id.SourceFile, Id.MemberName));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not match the snapshot "MyTests.MyTest",
					             but a snapshot cannot be negated
					             """);
			}

			[Test]
			public async Task WhenNegated_AndSnapshotMatches_ShouldFail()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id] = "foo";
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "foo";

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.MatchesSnapshot(Id.SourceFile, Id.MemberName));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not match the snapshot "MyTests.MyTest",
					             but a snapshot cannot be negated
					             """);
			}

			[Test]
			public async Task WhenNegated_AndSubjectIsNull_ShouldFail()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string? subject = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.MatchesSnapshot(Id.SourceFile, Id.MemberName));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not match the snapshot "MyTests.MyTest",
					             but it was <null>
					             """);
				await That(store.Mismatches).IsEmpty();
			}

			[Test]
			public async Task WhenReadingIsCanceled_ShouldNotReportThatTheSnapshotCouldNotBeRead()
			{
				using CancellationTokenSource cancellationTokenSource = new();
				InMemorySnapshotStore store = new()
				{
					OnRead = () =>
					{
						cancellationTokenSource.Cancel();
						cancellationTokenSource.Token.ThrowIfCancellationRequested();
					},
				};
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "foo";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.WithCancellation(cancellationTokenSource.Token);

				await That(Act).Throws<InconclusiveTestException>()
					.WithMessage("""
					             Expected that subject
					             matches the snapshot "MyTests.MyTest",
					             but it could not be verified, because the evaluation was already canceled

					             Actual:
					             foo
					             """);
			}

			[Test]
			public async Task WhenReadingThrows_ShouldFailWithTheExceptionAsCause()
			{
				InMemorySnapshotStore store = new()
				{
					ReadException = new IOException("locked"),
				};
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "foo";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName);

				FailException exception = await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             matches the snapshot "MyTests.MyTest",
					             but the snapshot could not be read: locked
					             """);
				await That(exception.InnerException).IsSameAs(store.ReadException);
				await That(store.Mismatches).IsEmpty()
					.Because("the subject could not be compared with the snapshot");
			}

			[Test]
			public async Task WhenReadingThrowsOperationCanceledException_WithoutCancellation_ShouldFail()
			{
				InMemorySnapshotStore store = new()
				{
					ReadException = new OperationCanceledException("timed out"),
				};
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "foo";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName);

				await That(Act).Throws<FailException>()
					.WithMessage("*but the snapshot could not be read: timed out").AsWildcard()
					.Because("only a cancellation of the evaluation aborts it");
			}

			[Test]
			public async Task WhenRecordingMatchThrows_ShouldSucceed()
			{
				InMemorySnapshotStore store = new()
				{
					RecordMatchException = new IOException("locked"),
				};
				store.Snapshots[Id] = "foo";
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "foo";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName);

				await That(Act).DoesNotThrow()
					.Because("the snapshot matched, and recording the match only cleans up");
			}

			[Test]
			public async Task WhenRecordingMismatchThrows_ShouldNameTheErrorInTheFailure()
			{
				InMemorySnapshotStore store = new()
				{
					RecordMismatchException = new IOException("disk full"),
				};
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "foo";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             matches the snapshot "MyTests.MyTest",
					             but the snapshot did not exist

					             Actual:
					             foo

					             Pending snapshot:
					             could not be written: disk full
					             """);
			}

			[Test]
			public async Task WhenSnapshotDiffers_ShouldFail()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id] = "foo bar";
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "foo baz";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             matches the snapshot "MyTests.MyTest",
					             but it was "foo baz", which differs at index 6:
					                      ↓ (actual)
					               "foo baz"
					               "foo bar"
					                      ↑ (expected)
					             """)
					.Because("a store without a description of the mismatch leaves out the pending snapshot");
			}

			[Test]
			public async Task WhenSnapshotDiffers_ShouldRecordMismatch()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id] = "foo";
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "bar";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName);

				await That(Act).Throws<FailException>();
				await That(store.Mismatches).IsEqualTo(new Dictionary<SnapshotId, string>
				{
					[Id] = "bar",
				});
				await That(store.Matches).IsEmpty();
				await That(store.Snapshots[Id]).IsEqualTo("foo")
					.Because("the snapshot is only changed by the user");
			}

			[Test]
			public async Task WhenSnapshotDiffersInSeveralLines_ShouldListTheDifferences()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id] = "a\nb\nc\nd";
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "a\nB\nc\nD";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             matches the snapshot "MyTests.MyTest",
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
					             """);
			}

			[Test]
			public async Task WhenSnapshotDiffersOnlyInNewlineStyle_ShouldSucceed()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id] = "foo\r\nbar";
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "foo\nbar";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName);

				await That(Act).DoesNotThrow()
					.Because("version control can change the newline style when the snapshot is checked out");
			}

			[Test]
			public async Task WhenSnapshotIsEmpty_AndSubjectIsEmpty_ShouldSucceed()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id] = "";
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSnapshotIsMissing_ShouldFail()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "foo";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             matches the snapshot "MyTests.MyTest",
					             but the snapshot did not exist

					             Actual:
					             foo
					             """);
				await That(store.Mismatches).IsEqualTo(new Dictionary<SnapshotId, string>
				{
					[Id] = "foo",
				});
			}

			[Test]
			public async Task WhenSnapshotMatches_ShouldRecordMatch()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id] = "foo\nbar";
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "foo\nbar";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName);

				await That(Act).DoesNotThrow();
				await That(store.Matches).IsEqualTo(new[] { Id, });
				await That(store.Mismatches).IsEmpty();
			}

			[Test]
			public async Task WhenSnapshotIsUsedAgainForTheSameValue_ShouldSucceed()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id] = "foo";
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "foo";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName);

				await That(Act).DoesNotThrow();
				await That(Act).DoesNotThrow()
					.Because("a retried or repeated test produces the same value again");
			}

			[Test]
			public async Task WhenSnapshotIsUsedForAnotherValue_ShouldFailWithoutRecordingMismatch()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id] = "foo";
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				await That("foo").MatchesSnapshot(Id.SourceFile, Id.MemberName);
				string subject = "bar";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             matches the snapshot "MyTests.MyTest",
					             but the snapshot was already used for another value in this test run, so name each one with .Named(…) or .NamedAfter(…)
					             """);
				await That(store.Mismatches).IsEmpty()
					.Because("the values would overwrite each other's pending snapshot");
			}

			[Test]
			public async Task WhenSnapshotsAreNamedDifferently_ShouldKeepThemApart()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id with { Name = "first", }] = "foo";
				store.Snapshots[Id with { Name = "second", }] = "bar";
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);

				async Task Act()
				{
					await That("foo").MatchesSnapshot(Id.SourceFile, Id.MemberName).Named("first");
					await That("bar").MatchesSnapshot(Id.SourceFile, Id.MemberName).Named("second");
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenStoreIsSetToNull_ShouldThrowArgumentNullException()
			{
				void Act()
					=> Customize.aweXpect.Snapshots().Store.Set(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("value");
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFailWithoutAccessingTheStore()
			{
				InMemorySnapshotStore store = new()
				{
					ReadException = new IOException("must not be read"),
				};
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string? subject = null;

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             matches the snapshot "MyTests.MyTest",
					             but it was <null>
					             """);
				await That(store.Mismatches).IsEmpty();
			}
		}
	}
}
