using aweXpect.Customization;
using aweXpect.Snapshots.Tests.TestHelpers;

namespace aweXpect.Snapshots.Tests;

public sealed partial class ThatSnapshot
{
	public sealed partial class MatchesSnapshot
	{
		public sealed class OutcomeTests
		{
			private SnapshotId Id { get; } = new($"/src/{Guid.NewGuid():N}/MyTests.cs", "MyTest");

			[Test]
			public async Task WhenAndBranchFails_ShouldRecordTheMismatch()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id] = "bar";
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "foo";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).And.IsEmpty();

				await That(Act).Throws<FailException>();
				await That(store.Mismatches[Id]).IsEqualTo("foo")
					.Because("the whole expectation fails");
			}

			[Test]
			public async Task WhenNegated_AndAccepting_ShouldNotAcceptTheSnapshot()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id] = "bar";
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "foo";

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it
						=> it.MatchesSnapshot(Id.SourceFile, Id.MemberName).Accepting(SnapshotAcceptance.All));

				await That(Act).Throws<FailException>()
					.WithMessage("*a snapshot cannot be negated").AsWildcard();
				await That(store.Accepted).IsEmpty()
					.Because("a negated snapshot must not replace the reviewed snapshot");
				await That(store.Mismatches).IsEmpty();
			}

			[Test]
			public async Task WhenNegated_ShouldNotRecordTheMismatch()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id] = "bar";
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "foo";

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.MatchesSnapshot(Id.SourceFile, Id.MemberName));

				await That(Act).Throws<FailException>();
				await That(store.Mismatches).IsEmpty()
					.Because("a snapshot cannot be negated, so nothing is written for it");
			}

			[Test]
			public async Task WhenOrBranchAfterTheSnapshotIsMet_AndAccepting_ShouldNotAcceptTheSnapshot()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id] = "bar";
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "foo";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.Accepting(SnapshotAcceptance.All)
						.Or.IsEqualTo("foo");

				await That(Act).DoesNotThrow();
				await That(store.Accepted).IsEmpty()
					.Because("a passing test must not change a snapshot unnoticed");
				await That(store.Snapshots[Id]).IsEqualTo("bar");
			}

			[Test]
			public async Task WhenOrBranchAfterTheSnapshotIsMet_ShouldWriteNothing()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id] = "bar";
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "foo";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).Or.IsEqualTo("foo");

				await That(Act).DoesNotThrow();
				await That(store.Mismatches).IsEmpty()
					.Because("the whole expectation is met");
			}

			[Test]
			public async Task WhenOrBranchBeforeTheSnapshotIsMet_ShouldWriteNothing()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id] = "bar";
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "foo";

				async Task Act()
					=> await That(subject).IsEqualTo("foo").Or.MatchesSnapshot(Id.SourceFile, Id.MemberName);

				await That(Act).DoesNotThrow();
				await That(store.Mismatches).IsEmpty();
			}

			[Test]
			public async Task WhenPartOfThatAll_AndTheSnapshotDiffers_ShouldRecordTheMismatch()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id] = "bar";
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "foo";

				async Task Act()
					=> await ThatAll(
						That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName),
						That(subject).IsNotEmpty());

				await That(Act).Throws<FailException>();
				await That(store.Mismatches[Id]).IsEqualTo("foo");
			}

			[Test]
			public async Task WhenPartOfThatAny_AndAnotherExpectationIsMet_ShouldWriteNothing()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id] = "bar";
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "foo";

				async Task Act()
					=> await ThatAny(
						That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName),
						That(subject).IsNotEmpty());

				await That(Act).DoesNotThrow();
				await That(store.Mismatches).IsEmpty()
					.Because("a combination passes its own outcome to the snapshot");
			}
		}
	}
}
