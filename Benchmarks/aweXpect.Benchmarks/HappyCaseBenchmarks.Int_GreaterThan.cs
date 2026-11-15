using BenchmarkDotNet.Attributes;
using FluentAssertions;
using FluentAssertions.Numeric;

namespace aweXpect.Benchmarks;

/// <summary>
///     In this benchmark we verify that the subject is greater than the expected minimum.<br />
/// </summary>
public partial class HappyCaseBenchmarks
{
	private readonly int _minimumTempo = 100;
	private readonly int _tempo = 128;

	[Benchmark]
	public async Task<int> Int_GreaterThan_aweXpect()
		=> await Expect.That(_tempo).IsGreaterThan(_minimumTempo);

	[Benchmark]
	public AndConstraint<NumericAssertions<int>> Int_GreaterThan_FluentAssertions()
		=> _tempo.Should().BeGreaterThan(_minimumTempo);

	[Benchmark]
	public async Task<int> Int_GreaterThan_TUnit()
		=> await Assert.That(_tempo).IsGreaterThan(_minimumTempo);
}
