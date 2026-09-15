using BenchmarkDotNet.Attributes;
using FluentAssertions;
using FluentAssertions.Collections;

namespace aweXpect.Benchmarks;

/// <summary>
///     In this benchmark we verify that a collection has at least the expected number of items.<br />
/// </summary>
public partial class HappyCaseBenchmarks
{
	private readonly int _enumerableCount = 1000;
	private readonly IEnumerable<int> _enumerableSubject = Enumerable.Range(1, 1000);

	[Benchmark]
	public async Task ItemsCount_AtLeast_aweXpect()
		=> await Expect.That(_enumerableSubject).HasCount().GreaterThanOrEqualTo(_enumerableCount);

	[Benchmark]
	public AndConstraint<GenericCollectionAssertions<int>> ItemsCount_AtLeast_FluentAssertions()
		=> _enumerableSubject.Should().HaveCountGreaterThanOrEqualTo(_enumerableCount);
}
