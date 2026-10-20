using System.IO;
using aweXpect.Customization;
using aweXpect.Snapshots.Tests.TestHelpers;

namespace aweXpect.Snapshots.Tests;

public sealed class OrphanedSnapshotsTests
{
	private string SourceFile { get; } = $"/src/{Guid.NewGuid():N}/AlbumTests.cs";

	[Test]
	public async Task WhenEverySnapshotIsUsed_ShouldSucceed()
	{
		InMemorySnapshotStore store = new();
		store.Snapshots[new SnapshotId(SourceFile, "Describes")] = "Abbey Road";
		using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
		await That("Abbey Road").MatchesSnapshot(SourceFile, "Describes");

		async Task Act() => await OrphanedSnapshots.CheckAsync(true);

		await That(Act).DoesNotThrow();
	}

	[Test]
	public async Task WhenASnapshotIsNotUsed_ShouldFailWithoutDeletingIt()
	{
		InMemorySnapshotStore store = new();
		store.Snapshots[new SnapshotId(SourceFile, "Describes")] = "Abbey Road";
		using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);

		async Task Act() => await OrphanedSnapshots.CheckAsync(true);

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that every snapshot is used by a test of this run, but 1 snapshot was not used by any test:
			               AlbumTests.Describes
			             """);
		await That(store.Deleted).IsEmpty()
			.Because("snapshots are only deleted while accepting changes");
	}

	[Test]
	public async Task WhenSeveralSnapshotsAreNotUsed_ShouldListThemInOrder()
	{
		InMemorySnapshotStore store = new();
		store.Snapshots[new SnapshotId(SourceFile, "Describes", "Let It Be")] = "Let It Be";
		store.Snapshots[new SnapshotId(SourceFile, "Describes", "Abbey Road")] = "Abbey Road";
		using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);

		async Task Act() => await OrphanedSnapshots.CheckAsync(true);

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that every snapshot is used by a test of this run, but 2 snapshots were not used by any test:
			               AlbumTests.Describes.Abbey Road
			               AlbumTests.Describes.Let It Be
			             """);
	}

	[Test]
	public async Task WhenOnlyAMismatchIsRecorded_ShouldReportIt()
	{
		InMemorySnapshotStore store = new();
		store.Mismatches[new SnapshotId(SourceFile, "Describes")] = "Abbey Road";
		using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);

		async Task Act() => await OrphanedSnapshots.CheckAsync(true);

		await That(Act).Throws<FailException>()
			.WithMessage("*1 snapshot was not used by any test:*AlbumTests.Describes").AsWildcard();
	}

	[Test]
	public async Task WhenTheSnapshotIsNamed_ShouldOnlyCountTheNamedSnapshotAsUsed()
	{
		InMemorySnapshotStore store = new();
		store.Snapshots[new SnapshotId(SourceFile, "Describes")] = "Abbey Road";
		store.Snapshots[new SnapshotId(SourceFile, "Describes", "Abbey Road")] = "Abbey Road";
		using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
		await That("Abbey Road").MatchesSnapshot(SourceFile, "Describes").Named("Abbey Road");

		async Task Act() => await OrphanedSnapshots.CheckAsync(true);

		await That(Act).Throws<FailException>()
			.WithMessage("*1 snapshot was not used by any test:*  AlbumTests.Describes").AsWildcard()
			.Because("naming the snapshot moves it away from the unnamed one");
	}

	[Test]
	public async Task WhenTheSnapshotIsNotCompared_ShouldStillCountItAsUsed()
	{
		InMemorySnapshotStore store = new();
		store.Snapshots[new SnapshotId(SourceFile, "Describes")] = "Let It Be";
		using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
		await That("Abbey Road").IsEqualTo("Abbey Road").Or.MatchesSnapshot(SourceFile, "Describes");

		async Task Act() => await OrphanedSnapshots.CheckAsync(true);

		await That(Act).DoesNotThrow()
			.Because("the test still refers to the snapshot, although the other expectation already succeeded");
	}

	[Test]
	public async Task WhenTheSnapshotOnlyDiffersInCase_ShouldCountItAsUsed()
	{
		InMemorySnapshotStore store = new();
		store.Snapshots[new SnapshotId(SourceFile.ToLowerInvariant(), "describes", "abbey road")] = "Abbey Road";
		using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
		await That("Abbey Road").IsEqualTo("Abbey Road")
			.Or.MatchesSnapshot(SourceFile.ToUpperInvariant(), "Describes").Named("Abbey Road");

		async Task Act() => await OrphanedSnapshots.CheckAsync(true);

		await That(Act).DoesNotThrow()
			.Because("on a file system that ignores the case it is the same file, which must never be deleted");
	}

	[Test]
	public async Task WhenTheSnapshotIsUniqueForTheEnvironment_ShouldOnlyCountTheVariantAsUsed()
	{
		InMemorySnapshotStore store = new();
		store.Snapshots[new SnapshotId(SourceFile, "Describes")] = "Abbey Road";
		store.Snapshots[new SnapshotId(SourceFile, "Describes") { Variant = CurrentEnvironment.OperatingSystem, }] =
			"Abbey Road";
		using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
		await That("Abbey Road").MatchesSnapshot(SourceFile, "Describes").UniqueFor.OperatingSystem();

		async Task Act() => await OrphanedSnapshots.CheckAsync(true);

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that every snapshot is used by a test of this run, but 1 snapshot was not used by any test:
			               AlbumTests.Describes
			             """)
			.Because("there is no fallback to the snapshot shared by every environment");
	}

	[Test]
	public async Task WhenAVariantOfThisEnvironmentIsNotUsed_ShouldReportIt()
	{
		string sourceFile = "Tests/aweXpect.Snapshots.Tests/OrphanedSnapshotsTests.cs";
		string memberName = $"Describes{Guid.NewGuid():N}";
		string variant =
			$"{CurrentEnvironment.TargetFramework}@{CurrentEnvironment.OperatingSystem}@{CurrentEnvironment.Architecture}";
		InMemorySnapshotStore store = new();
		store.Snapshots[new SnapshotId(sourceFile, memberName) { Variant = variant, }] = "Abbey Road";
		using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);

		async Task Act() => await OrphanedSnapshots.CheckAsync(true);

		await That(Act).Throws<FailException>()
			.WithMessage($"*  OrphanedSnapshotsTests.{memberName}@{variant}").AsWildcard()
			.Because("every part of the variant is a value of this environment, so this run would have used it");
	}

	[Test]
	[Arguments("beos")]
	[Arguments("{os}@sparc")]
	[Arguments("net35@{os}")]
	public async Task WhenAVariantIsOfAnotherEnvironment_ShouldNeverReportIt(string variant)
	{
		variant = variant.Replace("{os}", CurrentEnvironment.OperatingSystem);
		InMemorySnapshotStore store = new();
		store.Snapshots[new SnapshotId(SourceFile, "Describes") { Variant = variant, }] = "Abbey Road";
		using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
		using CustomizationLifetime __ = Customize.aweXpect.Snapshots().Acceptance.Set(SnapshotAcceptance.All);

		async Task Act() => await OrphanedSnapshots.CheckAsync(true);

		await That(Act).DoesNotThrow()
			.Because("only a test run in the environment of the variant can tell whether it is used");
		await That(store.Deleted).IsEmpty();
	}

	[Test]
	public async Task WhenTheTargetFrameworkOfAVariantCannotBeResolved_ShouldNeverReportIt()
	{
		InMemorySnapshotStore store = new();
		store.Snapshots[new SnapshotId(SourceFile, "Describes") { Variant = CurrentEnvironment.TargetFramework, }] =
			"Abbey Road";
		using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);

		async Task Act() => await OrphanedSnapshots.CheckAsync(true);

		await That(Act).DoesNotThrow()
			.Because("no target framework is recorded for a test file outside of the test projects");
	}

	[Test]
	public async Task WhenAcceptingAll_ShouldDeleteTheSnapshotsAndStillFail()
	{
		SnapshotId used = new(SourceFile, "Describes", "Abbey Road");
		SnapshotId unused = new(SourceFile, "Describes", "Let It Be");
		InMemorySnapshotStore store = new();
		store.Snapshots[used] = "Abbey Road";
		store.Snapshots[unused] = "Let It Be";
		store.Mismatches[unused] = "Let It Be (Remastered)";
		using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
		using CustomizationLifetime __ = Customize.aweXpect.Snapshots().Acceptance.Set(SnapshotAcceptance.All);
		await That("Abbey Road").MatchesSnapshot(SourceFile, "Describes").Named("Abbey Road");

		async Task Act() => await OrphanedSnapshots.CheckAsync(true);

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that every snapshot is used by a test of this run, but 1 snapshot was not used by any test:
			               AlbumTests.Describes.Let It Be (deleted)
			             """)
			.Because("a deletion must never go unnoticed");
		await That(store.Deleted).IsEqualTo([unused,]);
	}

	[Test]
	public async Task WhenAcceptingAll_AndDeletingThrows_ShouldNameTheErrorAndDeleteTheOthers()
	{
		SnapshotId readOnly = new(SourceFile, "Describes", "Abbey Road");
		SnapshotId other = new(SourceFile, "Describes", "Let It Be");
		InMemorySnapshotStore store = new();
		store.Snapshots[readOnly] = "Abbey Road";
		store.Snapshots[other] = "Let It Be";
		store.DeleteExceptions[readOnly] = new IOException("read-only");
		using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
		using CustomizationLifetime __ = Customize.aweXpect.Snapshots().Acceptance.Set(SnapshotAcceptance.All);

		async Task Act() => await OrphanedSnapshots.CheckAsync(true);

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that every snapshot is used by a test of this run, but 2 snapshots were not used by any test:
			               AlbumTests.Describes.Abbey Road (could not be deleted: read-only)
			               AlbumTests.Describes.Let It Be (deleted)
			             """);
		await That(store.Deleted).IsEqualTo([other,]);
	}

	[Test]
	public async Task WhenAcceptingNew_ShouldReportWithoutDeleting()
	{
		SnapshotId unused = new(SourceFile, "Describes", "Let It Be");
		InMemorySnapshotStore store = new();
		store.Snapshots[unused] = "Let It Be";
		using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
		using CustomizationLifetime __ = Customize.aweXpect.Snapshots().Acceptance.Set(SnapshotAcceptance.New);

		async Task Act() => await OrphanedSnapshots.CheckAsync(true);

		await That(Act).Throws<FailException>()
			.WithMessage("*  AlbumTests.Describes.Let It Be").AsWildcard();
		await That(store.Deleted).IsEmpty()
			.Because("accepting new snapshots only adds snapshots, it never removes one");
	}

	[Test]
	public async Task WhenTheStoreCannotList_ShouldFail()
	{
		InMemorySnapshotStore store = new()
		{
			CanList = false,
		};
		using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);

		async Task Act() => await OrphanedSnapshots.CheckAsync(true);

		await That(Act).Throws<FailException>()
			.WithMessage(
				"Expected that every snapshot is used by a test of this run, but the snapshot store InMemorySnapshotStore cannot list its snapshots, see SnapshotStore.ListAsync.")
			.Because("a check that was asked for must not pass without checking anything");
	}

	[Test]
	public async Task WhenListingThrows_ShouldFailWithTheException()
	{
		IOException exception = new("access denied");
		InMemorySnapshotStore store = new()
		{
			ListException = exception,
		};
		using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);

		async Task Act() => await OrphanedSnapshots.CheckAsync(true);

		await That(Act).Throws<FailException>()
			.WithMessage(
				"Expected that every snapshot is used by a test of this run, but the snapshots could not be listed: access denied")
			.And.WithInner<IOException>(inner => inner.IsSameAs(exception));
	}

	[NotInParallel]
	public sealed class EnvironmentTests
	{
		private string SourceFile { get; } = $"/src/{Guid.NewGuid():N}/AlbumTests.cs";

		[Test]
		public async Task WhenNotForced_AndNoBuildServerIsDetected_ShouldNotCheck()
		{
			using EnvironmentScope _ = new();
			InMemorySnapshotStore store = new()
			{
				CanList = false,
			};
			using CustomizationLifetime __ = Customize.aweXpect.Snapshots().Store.Set(store);

			async Task Act() => await OrphanedSnapshots.CheckAsync();

			await That(Act).DoesNotThrow()
				.Because("a local run that only runs some of the tests cannot be told apart from a complete one");
		}

		[Test]
		[Arguments("CI")]
		[Arguments("TF_BUILD")]
		[Arguments("JENKINS_URL")]
		[Arguments("TEAMCITY_VERSION")]
		public async Task WhenNotForced_AndABuildServerIsDetected_ShouldReportWithoutDeleting(string variable)
		{
			using EnvironmentScope _ = new((variable, "true"), ("AWEXPECT_ACCEPT_SNAPSHOTS", "1"));
			InMemorySnapshotStore store = new();
			store.Snapshots[new SnapshotId(SourceFile, "Describes")] = "Abbey Road";
			using CustomizationLifetime __ = Customize.aweXpect.Snapshots().Store.Set(store);

			async Task Act() => await OrphanedSnapshots.CheckAsync();

			await That(Act).Throws<FailException>()
				.WithMessage("*1 snapshot was not used by any test:*  AlbumTests.Describes").AsWildcard();
			await That(store.Deleted).IsEmpty()
				.Because("the accept variable is ignored on a build server");
		}

		[Test]
		public async Task WhenForced_AndTheAcceptVariableIsNew_ShouldReportWithoutDeleting()
		{
			using EnvironmentScope _ = new(("AWEXPECT_ACCEPT_SNAPSHOTS", "new"));
			InMemorySnapshotStore store = new();
			store.Snapshots[new SnapshotId(SourceFile, "Describes")] = "Abbey Road";
			using CustomizationLifetime __ = Customize.aweXpect.Snapshots().Store.Set(store);

			async Task Act() => await OrphanedSnapshots.CheckAsync(true);

			await That(Act).Throws<FailException>()
				.WithMessage("*  AlbumTests.Describes").AsWildcard();
			await That(store.Deleted).IsEmpty();
		}

		[Test]
		public async Task WhenForced_AndTheAcceptVariableIsInvalid_ShouldThrow()
		{
			using EnvironmentScope _ = new(("AWEXPECT_ACCEPT_SNAPSHOTS", "yes"));
			InMemorySnapshotStore store = new();
			store.Snapshots[new SnapshotId(SourceFile, "Describes")] = "Abbey Road";
			using CustomizationLifetime __ = Customize.aweXpect.Snapshots().Store.Set(store);

			async Task Act() => await OrphanedSnapshots.CheckAsync(true);

			await That(Act).Throws<InvalidOperationException>()
				.WithMessage("*AWEXPECT_ACCEPT_SNAPSHOTS*'yes'*").AsWildcard();
			await That(store.Deleted).IsEmpty();
		}

		[Test]
		[Arguments("1")]
		[Arguments("all")]
		public async Task WhenForced_AndTheAcceptVariableIsSet_ShouldDelete(string value)
		{
			using EnvironmentScope _ = new(("AWEXPECT_ACCEPT_SNAPSHOTS", value));
			SnapshotId id = new(SourceFile, "Describes");
			InMemorySnapshotStore store = new();
			store.Snapshots[id] = "Abbey Road";
			using CustomizationLifetime __ = Customize.aweXpect.Snapshots().Store.Set(store);

			async Task Act() => await OrphanedSnapshots.CheckAsync(true);

			await That(Act).Throws<FailException>()
				.WithMessage("*  AlbumTests.Describes (deleted)").AsWildcard();
			await That(store.Deleted).IsEqualTo([id,]);
		}
	}
}
