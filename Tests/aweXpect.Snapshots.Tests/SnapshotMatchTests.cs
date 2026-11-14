using aweXpect.Snapshots.Tests.TestHelpers;

namespace aweXpect.Snapshots.Tests;

public sealed class SnapshotMatchTests
{
	private static readonly SnapshotId Id = new("/src/AlbumTests.cs", "Describes");

	[Test]
	public async Task Constructor_ShouldSetTheProperties()
	{
		InMemorySnapshotStore store = new();

		SnapshotMatch match = new(Id, store);

		await That(match.Id).IsSameAs(Id);
		await That(match.Store).IsSameAs(store)
			.Because("an extension author can test a reporter with a match of an own store");
	}

	[Test]
	[Arguments("id")]
	[Arguments("store")]
	public async Task Constructor_WhenAnArgumentIsNull_ShouldThrowArgumentNullException(string parameterName)
	{
		void Act() => _ = new SnapshotMatch(
			parameterName == "id" ? null! : Id,
			parameterName == "store" ? null! : new InMemorySnapshotStore());

		await That(Act).Throws<ArgumentNullException>()
			.WithParamName(parameterName).And
			.WithMessage($"The '{parameterName}' cannot be null.*").AsWildcard();
	}
}
