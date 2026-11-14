using BenchmarkDotNet.Attributes;
using FluentAssertions;
using FluentAssertions.Numeric;

namespace aweXpect.Benchmarks;

/// <summary>
///     In this benchmark we verify that the subject is one of four candidates.
/// </summary>
public partial class HappyCaseBenchmarks
{
	private readonly int[] _intOneOfCandidates = [7, 13, 21, 42,];
	private readonly int _intOneOfSubject = 42;

	[Benchmark]
	public async Task<int> Int_IsOneOf_aweXpect()
		=> await Expect.That(_intOneOfSubject).IsOneOf(_intOneOfCandidates);

	[Benchmark]
	public AndConstraint<NumericAssertions<int>> Int_IsOneOf_FluentAssertions()
		=> _intOneOfSubject.Should().BeOneOf(_intOneOfCandidates);

	[Benchmark]
	public async Task<int> Int_IsOneOf_TUnit()
		=> await Assert.That(_intOneOfSubject).IsIn(_intOneOfCandidates);
}
