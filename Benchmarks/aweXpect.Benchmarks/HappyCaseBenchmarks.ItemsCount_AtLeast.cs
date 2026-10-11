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
	private readonly IEnumerable<int> _enumerableSubject = LazyRange(1000);

	[Benchmark]
	public async Task ItemsCount_AtLeast_aweXpect()
		=> await Expect.That(_enumerableSubject).HasCount().GreaterThanOrEqualTo(_enumerableCount);

	[Benchmark]
	public AndConstraint<GenericCollectionAssertions<int>> ItemsCount_AtLeast_FluentAssertions()
		=> _enumerableSubject.Should().HaveCountGreaterThanOrEqualTo(_enumerableCount);

	[Benchmark]
	public async Task ItemsCount_AtLeast_TUnit()
		=> await Assert.That(_enumerableSubject).Count().IsGreaterThanOrEqualTo(_enumerableCount);

	/// <summary>
	///     Unlike <see cref="Enumerable.Range(int, int)" />, the count is not known without enumerating the items.
	/// </summary>
	private static IEnumerable<int> LazyRange(int count)
	{
		for (int i = 1; i <= count; i++)
		{
			yield return i;
		}
	}
}
