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
	private readonly string[] _bandMembers = ["Sting", "Andy", "Stewart",];
	private readonly string[] _expectedBandMembers = ["Sting", "Andy", "Stewart",];

	[Benchmark]
	public async Task StringArray_aweXpect()
		=> (await Expect.That(_bandMembers).IsEqualTo(_expectedBandMembers))!.Consume(_consumer);

	[Benchmark]
	public AndConstraint<StringCollectionAssertions<IEnumerable<string>>> StringArray_FluentAssertions()
		=> _bandMembers.Should().Equal(_expectedBandMembers);

	[Benchmark]
	public async Task StringArray_TUnit()
		=> (await Assert.That(_bandMembers)
			.IsEquivalentTo(_expectedBandMembers, CollectionOrdering.Matching))?.Consume(_consumer);
}
