using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using FluentAssertions;
using FluentAssertions.Collections;
using TUnit.Assertions.Enums;

namespace aweXpect.Benchmarks;

/// <summary>
///     In this benchmark we verify that a <see cref="string" /> array has the same values than another one.<br />
/// </summary>
public partial class HappyCaseBenchmarks
{
	private readonly string[] _scheduledStationGenres = ["Pop", "Rap", "Ska",];
	private readonly string[] _radioStationGenres = ["Pop", "Rap", "Ska",];

	[Benchmark]
	public async Task StringArray_aweXpect()
		=> (await Expect.That(_radioStationGenres).IsEqualTo(_scheduledStationGenres))!.Consume(_consumer);

	[Benchmark]
	public AndConstraint<StringCollectionAssertions<IEnumerable<string>>> StringArray_FluentAssertions()
		=> _radioStationGenres.Should().Equal(_scheduledStationGenres);

	[Benchmark]
	public async Task StringArray_TUnit()
		=> (await Assert.That(_radioStationGenres)
			.IsEquivalentTo(_scheduledStationGenres, CollectionOrdering.Matching))?.Consume(_consumer);
}
