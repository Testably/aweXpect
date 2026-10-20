using System.Text;
using System.Threading;
using aweXpect.Snapshots.Tests.TestHelpers;

namespace aweXpect.Snapshots.Tests;

public sealed class SnapshotComparisonContextTests
{
	private static readonly SnapshotId Id = new("/src/AlbumTests.cs", "Describes");

	[Test]
	public async Task FromText_ShouldEncodeTheTextLikeTheStore()
	{
		SnapshotContent content = SnapshotContent.FromText("Abbey Road");

		await That(content.Text).IsEqualTo("Abbey Road");
		await That(content.IsText).IsTrue();
		await That(content.Bytes).IsEqualTo(Encoding.UTF8.GetBytes("Abbey Road" + Environment.NewLine))
			.Because("a store keeps text as UTF-8 without a byte order mark and with an additional line break");
	}

	[Test]
	public async Task FromBytes_ShouldBeBinaryData()
	{
		byte[] bytes = [0x50, 0x4E, 0x47,];

		SnapshotContent content = SnapshotContent.FromBytes(bytes);

		await That(content.Bytes).IsSameAs(bytes);
		await That(content.IsText).IsFalse();
		await That(content.Text).IsNull();
	}

	[Test]
	public async Task FromText_WhenNull_ShouldThrowArgumentNullException()
	{
		void Act() => SnapshotContent.FromText(null!);

		await That(Act).Throws<ArgumentNullException>().WithParamName("text");
	}

	[Test]
	public async Task FromBytes_WhenNull_ShouldThrowArgumentNullException()
	{
		void Act() => SnapshotContent.FromBytes(null!);

		await That(Act).Throws<ArgumentNullException>().WithParamName("bytes");
	}

	[Test]
	[Arguments("id")]
	[Arguments("snapshot")]
	[Arguments("actual")]
	public async Task Constructor_WhenAnArgumentIsNull_ShouldThrowArgumentNullException(string parameterName)
	{
		SnapshotContent content = SnapshotContent.FromText("Abbey Road");

		void Act() => _ = new SnapshotComparisonContext(
			parameterName == "id" ? null! : Id,
			parameterName == "snapshot" ? null! : content,
			parameterName == "actual" ? null! : content);

		await That(Act).Throws<ArgumentNullException>().WithParamName(parameterName);
	}

	[Test]
	public async Task Constructor_ShouldAllowTestingAComparerWithoutASnapshot()
	{
		FakeSnapshotComparer comparer = new(context => context.Actual.Text!.Equals(context.Snapshot.Text,
			StringComparison.OrdinalIgnoreCase)
			? SnapshotComparison.Equal
			: SnapshotComparison.Different("the titles differ"));
		SnapshotComparisonContext context = new(Id,
			SnapshotContent.FromText("Abbey Road"), SnapshotContent.FromText("ABBEY ROAD"));

		SnapshotComparison comparison = await comparer.CompareAsync(context, CancellationToken.None);

		await That(comparison.IsEqual).IsTrue()
			.Because("an extension author can test a comparer with contents created like a store creates them");
	}
}
