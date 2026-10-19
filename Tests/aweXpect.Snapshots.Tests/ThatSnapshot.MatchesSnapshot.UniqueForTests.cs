using aweXpect.Customization;
using aweXpect.Snapshots.Tests.TestHelpers;

namespace aweXpect.Snapshots.Tests;

public sealed partial class ThatSnapshot
{
	public sealed partial class MatchesSnapshot
	{
		public sealed class UniqueForTests
		{
			private const string SourceFile = "Tests/aweXpect.Snapshots.Tests/ThatSnapshot.MatchesSnapshot.UniqueForTests.cs";
			private SnapshotId Id { get; } = new($"/src/{Guid.NewGuid():N}/MyTests.cs", "MyTest");

			[Test]
			public async Task WhenUniqueForTargetFramework_ShouldAddTheTargetFrameworkAsVariant()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "Abbey Road";

				async Task Act()
					=> await That(subject).MatchesSnapshot().UniqueFor.TargetFramework();

				await That(Act).Throws<FailException>()
					.WithMessage($"*matches the snapshot \"ThatSnapshot.MatchesSnapshot.UniqueForTests.{nameof(WhenUniqueForTargetFramework_ShouldAddTheTargetFrameworkAsVariant)}@{CurrentEnvironment.TargetFramework}\"*")
					.AsWildcard();
				await That(store.Mismatches.Keys).IsEqualTo(new[]
				{
					new SnapshotId(SourceFile, nameof(WhenUniqueForTargetFramework_ShouldAddTheTargetFrameworkAsVariant))
					{
						Variant = CurrentEnvironment.TargetFramework,
					},
				}).InAnyOrder().Because("the build targets record the target framework of the test project as the build knows it");
			}

			[Test]
			public async Task WhenUniqueForOperatingSystem_ShouldAddTheOperatingSystemAsVariant()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "Abbey Road";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).UniqueFor.OperatingSystem();

				await That(Act).Throws<FailException>();
				await That(store.Mismatches.Keys).IsEqualTo(new[]
				{
					Id with { Variant = CurrentEnvironment.OperatingSystem, },
				}).InAnyOrder();
			}

			[Test]
			public async Task WhenUniqueForArchitecture_ShouldAddTheArchitectureAsVariant()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "Abbey Road";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).UniqueFor.Architecture();

				await That(Act).Throws<FailException>();
				await That(store.Mismatches.Keys).IsEqualTo(new[]
				{
					Id with { Variant = CurrentEnvironment.Architecture, },
				}).InAnyOrder();
			}

			[Test]
			public async Task WhenUniqueForSeveral_ShouldJoinThemInAFixedOrder()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "Abbey Road";

				async Task Act()
					=> await That(subject).MatchesSnapshot()
						.UniqueFor.Architecture()
						.UniqueFor.OperatingSystem()
						.UniqueFor.TargetFramework();

				await That(Act).Throws<FailException>();
				await That(store.Mismatches.Keys).IsEqualTo(new[]
				{
					new SnapshotId(SourceFile, nameof(WhenUniqueForSeveral_ShouldJoinThemInAFixedOrder))
					{
						Variant =
							$"{CurrentEnvironment.TargetFramework}@{CurrentEnvironment.OperatingSystem}@{CurrentEnvironment.Architecture}",
					},
				}).InAnyOrder().Because("the order of the calls must not change the snapshot");
			}

			[Test]
			public async Task WhenUniqueForTheSameEnvironmentTwice_ShouldAddItOnce()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "Abbey Road";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.UniqueFor.OperatingSystem()
						.UniqueFor.OperatingSystem();

				await That(Act).Throws<FailException>();
				await That(store.Mismatches.Keys).IsEqualTo(new[]
				{
					Id with { Variant = CurrentEnvironment.OperatingSystem, },
				}).InAnyOrder();
			}

			[Test]
			public async Task WhenNamed_ShouldKeepTheNameAndTheVariant()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "Let It Be";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.UniqueFor.OperatingSystem()
						.Named("Let It Be")
						.Accepting(SnapshotAcceptance.None);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              matches the snapshot "MyTests.MyTest.Let It Be@{CurrentEnvironment.OperatingSystem}",
					              but the snapshot did not exist

					              Actual:
					              Let It Be
					              """);
				await That(store.Mismatches.Keys).IsEqualTo(new[]
				{
					Id with { Name = "Let It Be", Variant = CurrentEnvironment.OperatingSystem, },
				}).InAnyOrder();
			}

			[Test]
			public async Task WhenOnlyTheSharedSnapshotExists_ShouldFail()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id] = "Abbey Road";
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "Abbey Road";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).UniqueFor.Architecture();

				await That(Act).Throws<FailException>()
					.WithMessage("*but the snapshot did not exist*").AsWildcard()
					.Because("a snapshot per environment never falls back to the snapshot shared by every environment");
			}

			[Test]
			public async Task WhenTheSnapshotOfTheEnvironmentMatches_ShouldSucceed()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id with { Variant = CurrentEnvironment.OperatingSystem, }] = "Abbey Road";
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "Abbey Road";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).UniqueFor.OperatingSystem();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTheTargetFrameworkIsNotRecorded_ShouldThrowInvalidOperationException()
			{
				string subject = "Abbey Road";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).UniqueFor.TargetFramework();

				await That(Act).Throws<InvalidOperationException>()
					.WithMessage(
						$"No single target framework is recorded for the test file \"{Id.SourceFile}\", which the build targets of aweXpect.Snapshots record for its test project.")
					.Because("the runtime can differ from the target framework, so it is no fallback");
			}

			[Test]
			public async Task WhenUniqueForAnotherEnvironment_ShouldNotConflictWithTheSharedSnapshot()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id] = "Abbey Road";
				store.Snapshots[Id with { Variant = CurrentEnvironment.OperatingSystem, }] = "Let It Be";
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);

				async Task Act()
				{
					await That("Abbey Road").MatchesSnapshot(Id.SourceFile, Id.MemberName);
					await That("Let It Be").MatchesSnapshot(Id.SourceFile, Id.MemberName).UniqueFor.OperatingSystem();
				}

				await That(Act).DoesNotThrow()
					.Because("the variant is part of the snapshot id, so the values are no reuse of the same snapshot");
			}
		}
	}
}
