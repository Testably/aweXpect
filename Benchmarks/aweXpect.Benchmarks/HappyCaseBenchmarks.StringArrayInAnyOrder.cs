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
	private readonly string[] _lineUp = ["Sting", "Andy", "Stewart",];
	private readonly string[] _shuffledLineUp = ["Sting", "Stewart", "Andy",];

	[Benchmark]
	public async Task StringArrayInAnyOrder_aweXpect()
		=> (await Expect.That(_lineUp).IsEqualTo(_shuffledLineUp).InAnyOrder())!
			.Consume(_consumer);

	[Benchmark]
	public AndConstraint<StringCollectionAssertions<IEnumerable<string>>> StringArrayInAnyOrder_FluentAssertions()
		=> _lineUp.Should().BeEquivalentTo(_shuffledLineUp);

	[Benchmark]
	public async Task StringArrayInAnyOrder_TUnit()
		=> (await Assert.That(_lineUp)
			.IsEquivalentTo(_shuffledLineUp))?.Consume(_consumer);
}
