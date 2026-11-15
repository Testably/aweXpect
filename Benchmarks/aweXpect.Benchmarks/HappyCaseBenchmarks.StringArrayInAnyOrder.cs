using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using FluentAssertions;
using FluentAssertions.Collections;

namespace aweXpect.Benchmarks;

/// <summary>
///     In this benchmark we verify that a <see cref="string" /> array has the same values in a different order than
///     another one.<br />
/// </summary>
public partial class HappyCaseBenchmarks
{
	private readonly string[] _radioStationGenresInOtherOrder = ["Pop", "Ska", "Rap",];
	private readonly string[] _radioStationGenresAsListed = ["Pop", "Rap", "Ska",];

	[Benchmark]
	public async Task StringArrayInAnyOrder_aweXpect()
		=> (await Expect.That(_radioStationGenresAsListed).IsEqualTo(_radioStationGenresInOtherOrder).InAnyOrder())!
			.Consume(_consumer);

	[Benchmark]
	public AndConstraint<StringCollectionAssertions<IEnumerable<string>>> StringArrayInAnyOrder_FluentAssertions()
		=> _radioStationGenresAsListed.Should().BeEquivalentTo(_radioStationGenresInOtherOrder);

	[Benchmark]
	public async Task StringArrayInAnyOrder_TUnit()
		=> (await Assert.That(_radioStationGenresAsListed)
			.IsEquivalentTo(_radioStationGenresInOtherOrder))?.Consume(_consumer);
}
