using BenchmarkDotNet.Attributes;
using FluentAssertions;
using FluentAssertions.Collections;

namespace aweXpect.Benchmarks;

/// <summary>
///     In this benchmark we verify that a collection has at least the expected number of items.<br />
/// </summary>
public partial class HappyCaseBenchmarks
{
	private readonly int _minimumTrackCount = 1000;
	private readonly int[] _trackIds = Enumerable.Range(1, 1000).ToArray();

	[Benchmark]
	public async Task ItemsCount_AtLeast_aweXpect()
		=> await Expect.That(_trackIds).HasCount().GreaterThanOrEqualTo(_minimumTrackCount);

	[Benchmark]
	public AndConstraint<GenericCollectionAssertions<int>> ItemsCount_AtLeast_FluentAssertions()
		=> _trackIds.Should().HaveCountGreaterThanOrEqualTo(_minimumTrackCount);

	[Benchmark]
	public async Task ItemsCount_AtLeast_TUnit()
		=> await Assert.That(_trackIds).Count().IsGreaterThanOrEqualTo(_minimumTrackCount);
}
