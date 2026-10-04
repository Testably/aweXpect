using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using FluentAssertions;
using FluentAssertions.Collections;
using TUnit.Assertions.Enums;

namespace aweXpect.Benchmarks;

/// <summary>
///     In this benchmark we verify that a <see cref="string" /> array has the same values in a different order than
///     another one.<br />
/// </summary>
public partial class HappyCaseBenchmarks
{
	private readonly string[] _stringArrayAnyOrderExpectation = ["foo", "baz", "bar"];
	private readonly string[] _stringArrayAnyOrderSubject = ["foo", "bar", "baz"];

	[Benchmark]
	public async Task StringArrayInAnyOrder_aweXpect()
		=> (await Expect.That(_stringArrayAnyOrderSubject).IsEqualTo(_stringArrayAnyOrderExpectation).InAnyOrder())!
			.Consume(_consumer);

	[Benchmark]
	public AndConstraint<StringCollectionAssertions<IEnumerable<string>>> StringArrayInAnyOrder_FluentAssertions()
		=> _stringArrayAnyOrderSubject.Should().BeEquivalentTo(_stringArrayAnyOrderExpectation);

	[Benchmark]
	public async Task StringArrayInAnyOrder_TUnit()
		=> (await Assert.That(_stringArrayAnyOrderSubject)
			.IsEquivalentTo(_stringArrayAnyOrderExpectation, CollectionOrdering.Any))?.Consume(_consumer);
}
