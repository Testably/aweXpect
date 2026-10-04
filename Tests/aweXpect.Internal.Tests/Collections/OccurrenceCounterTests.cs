using System.Collections.Generic;

namespace aweXpect.Internal.Tests.Collections;

public sealed class OccurrenceCounterTests
{
	[Fact]
	public async Task Add_WhenAllHashCodesAreEqual_ShouldCountEqualMembersOnly()
	{
		OccurrenceCounter<int> sut = new((a, b) => new ValueTask<bool>(a == b), _ => 0);

		List<int> indices = await AddAll(sut, 1, 2, 1, 3, 2, 2);

		await That(indices).IsEqualTo([0, 1, 0, 2, 1, 1,])
			.Because("members with the same hash code are still compared");
		await That(sut.IsUnique(0)).IsFalse();
		await That(sut.IsUnique(1)).IsFalse();
		await That(sut.IsUnique(2)).IsTrue();
	}

	[Fact]
	public async Task Add_WhenHashCodesDiffer_ShouldCountEqualMembers()
	{
		OccurrenceCounter<int> sut = new((a, b) => new ValueTask<bool>(a == b), x => x);

		List<int> indices = await AddAll(sut, 1, 2, 1, 3);

		await That(indices).IsEqualTo([0, 1, 0, 2,]);
		await That(sut.IsUnique(0)).IsFalse();
		await That(sut.IsUnique(1)).IsTrue();
		await That(sut.IsUnique(2)).IsTrue();
	}

	[Fact]
	public async Task Add_WhenSomeHashCodesCollide_ShouldCountEqualMembersOnly()
	{
		OccurrenceCounter<int> sut = new((a, b) => new ValueTask<bool>(a == b), x => x % 2);

		List<int> indices = await AddAll(sut, 1, 3, 2, 5, 3, 4, 2);

		await That(indices).IsEqualTo([0, 1, 2, 3, 1, 4, 2,]);
		await That(sut.IsUnique(0)).IsTrue();
		await That(sut.IsUnique(1)).IsFalse();
		await That(sut.IsUnique(2)).IsFalse();
		await That(sut.IsUnique(3)).IsTrue();
		await That(sut.IsUnique(4)).IsTrue();
	}

	[Fact]
	public async Task Add_WithoutHashCode_ShouldCountEqualMembers()
	{
		OccurrenceCounter<int> sut = new((a, b) => new ValueTask<bool>(a == b));

		List<int> indices = await AddAll(sut, 1, 2, 1);

		await That(indices).IsEqualTo([0, 1, 0,]);
		await That(sut.IsUnique(0)).IsFalse();
		await That(sut.IsUnique(1)).IsTrue();
	}

	private static async Task<List<int>> AddAll(OccurrenceCounter<int> sut, params int[] members)
	{
		List<int> indices = [];
		foreach (int member in members)
		{
			indices.Add(await sut.Add(member));
		}

		return indices;
	}
}
