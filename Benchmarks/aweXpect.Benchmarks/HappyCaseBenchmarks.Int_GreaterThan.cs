using BenchmarkDotNet.Attributes;
using FluentAssertions;
using FluentAssertions.Numeric;

namespace aweXpect.Benchmarks;

/// <summary>
///     In this benchmark we verify that the subject is greater than the expected minimum.<br />
/// </summary>
public partial class HappyCaseBenchmarks
{
	private readonly int _minimumBpm = 100;
	private readonly int _currentBpm = 128;

	[Benchmark]
	public async Task<int> Int_GreaterThan_aweXpect()
		=> await Expect.That(_currentBpm).IsGreaterThan(_minimumBpm);

	[Benchmark]
	public AndConstraint<NumericAssertions<int>> Int_GreaterThan_FluentAssertions()
		=> _currentBpm.Should().BeGreaterThan(_minimumBpm);

	[Benchmark]
	public async Task<int> Int_GreaterThan_TUnit()
		=> await Assert.That(_currentBpm).IsGreaterThan(_minimumBpm);
}
