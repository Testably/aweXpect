using BenchmarkDotNet.Attributes;
using FluentAssertions;
using FluentAssertions.Numeric;

namespace aweXpect.Benchmarks;

/// <summary>
///     In this benchmark we verify that the subject satisfies a predicate.<br />
/// </summary>
public partial class HappyCaseBenchmarks
{
	private readonly int _playCountOfTrack = 42;

	[Benchmark]
	public async Task<int> Satisfies_aweXpect()
		=> await Expect.That(_playCountOfTrack).Satisfies(x => x > 0);

	[Benchmark]
	public AndConstraint<NumericAssertions<int>> Satisfies_FluentAssertions()
		=> _playCountOfTrack.Should().Match(x => x > 0);

	[Benchmark]
	public async Task<int> Satisfies_TUnit()
		=> await Assert.That(_playCountOfTrack).Satisfies(x => x > 0);
}
