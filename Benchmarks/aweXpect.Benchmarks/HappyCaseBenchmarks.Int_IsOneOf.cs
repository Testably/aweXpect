using BenchmarkDotNet.Attributes;
using FluentAssertions;
using FluentAssertions.Numeric;

namespace aweXpect.Benchmarks;

/// <summary>
///     In this benchmark we verify that the subject is one of four candidates.
/// </summary>
public partial class HappyCaseBenchmarks
{
	private readonly int[] _allowedBeatsPerBar = [2, 3, 4, 6,];
	private readonly int _songBeatsPerBar = 6;

	[Benchmark]
	public async Task<int> Int_IsOneOf_aweXpect()
		=> await Expect.That(_songBeatsPerBar).IsOneOf(_allowedBeatsPerBar);

	[Benchmark]
	public AndConstraint<NumericAssertions<int>> Int_IsOneOf_FluentAssertions()
		=> _songBeatsPerBar.Should().BeOneOf(_allowedBeatsPerBar);

	[Benchmark]
	public async Task<int> Int_IsOneOf_TUnit()
		=> await Assert.That(_songBeatsPerBar).IsIn(_allowedBeatsPerBar);
}
