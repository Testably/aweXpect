using BenchmarkDotNet.Attributes;
using FluentAssertions;
using FluentAssertions.Numeric;

namespace aweXpect.Benchmarks;

/// <summary>
///     In this benchmark we verify that the subject is greater than the expected minimum.<br />
/// </summary>
public partial class HappyCaseBenchmarks
{
	private readonly int _intMinimum = 20;
	private readonly int _intSubject = 42;

	[Benchmark]
	public async Task<int> Int_GreaterThan_aweXpect()
		=> await Expect.That(_intSubject).IsGreaterThan(_intMinimum);

	[Benchmark]
	public AndConstraint<NumericAssertions<int>> Int_GreaterThan_FluentAssertions()
		=> _intSubject.Should().BeGreaterThan(_intMinimum);

	[Benchmark]
	public async Task<int> Int_GreaterThan_TUnit()
		=> await Assert.That(_intSubject).IsGreaterThan(_intMinimum);
}
