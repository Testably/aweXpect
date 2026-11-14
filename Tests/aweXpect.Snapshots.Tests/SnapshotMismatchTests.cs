using aweXpect.Snapshots.Tests.TestHelpers;

namespace aweXpect.Snapshots.Tests;

public sealed class SnapshotMismatchTests
{
	private static readonly SnapshotId Id = new("/src/AlbumTests.cs", "Describes");

	[Test]
	public async Task Constructor_ShouldSetTheProperties()
	{
		InMemorySnapshotStore store = new();
		SnapshotContent snapshot = SnapshotContent.FromText("Abbey Road");
		SnapshotContent actual = SnapshotContent.FromText("Let It Be");

		SnapshotMismatch mismatch = new(Id, store, snapshot, actual);

		await That(mismatch.Id).IsSameAs(Id);
		await That(mismatch.Store).IsSameAs(store);
		await That(mismatch.Snapshot).IsSameAs(snapshot);
		await That(mismatch.Actual).IsSameAs(actual)
			.Because("an extension author can test a reporter with contents created like a store creates them");
	}

	[Test]
	[Arguments("id")]
	[Arguments("store")]
	[Arguments("actual")]
	public async Task Constructor_WhenAnArgumentIsNull_ShouldThrowArgumentNullException(string parameterName)
	{
		SnapshotContent content = SnapshotContent.FromText("Abbey Road");

		void Act() => _ = new SnapshotMismatch(
			parameterName == "id" ? null! : Id,
			parameterName == "store" ? null! : new InMemorySnapshotStore(),
			content,
			parameterName == "actual" ? null! : content);

		await That(Act).Throws<ArgumentNullException>()
			.WithParamName(parameterName).And
			.WithMessage($"The '{parameterName}' cannot be null.*").AsWildcard();
	}

	[Test]
	public async Task Constructor_WhenTheSnapshotIsNull_ShouldBeAMissingSnapshot()
	{
		SnapshotMismatch mismatch = new(Id, new InMemorySnapshotStore(), null,
			SnapshotContent.FromText("Abbey Road"));

		await That(mismatch.Snapshot).IsNull()
			.Because("a mismatch without a snapshot reports a snapshot that did not exist");
	}
}
