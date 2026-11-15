using BenchmarkDotNet.Attributes;
using FluentAssertions;
using FluentAssertions.Numeric;

namespace aweXpect.Benchmarks;

/// <summary>
///     In this benchmark we verify that the subject is one of four candidates.
/// </summary>
public partial class HappyCaseBenchmarks
{
	private readonly int _beatsPerBar = 6;
	private readonly int[] _commonBeatsPerBar = [2, 3, 4, 6,];

	[Benchmark]
	public async Task<int> Int_IsOneOf_aweXpect()
		=> await Expect.That(_beatsPerBar).IsOneOf(_commonBeatsPerBar);

	[Benchmark]
	public AndConstraint<NumericAssertions<int>> Int_IsOneOf_FluentAssertions()
		=> _beatsPerBar.Should().BeOneOf(_commonBeatsPerBar);

	[Benchmark]
	public async Task<int> Int_IsOneOf_TUnit()
		=> await Assert.That(_beatsPerBar).IsIn(_commonBeatsPerBar);
}
