using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using FluentAssertions;
using FluentAssertions.Collections;

namespace aweXpect.Benchmarks;

/// <summary>
///     In this benchmark we verify that a <see cref="string" /> array has the same values than another one.<br />
/// </summary>
public partial class HappyCaseBenchmarks
{
	private readonly string[] _stringArrayExpectation = ["foo", "bar", "baz"];
	private readonly string[] _stringArraySubject = ["foo", "bar", "baz"];

	[Benchmark]
	public async Task StringArray_aweXpect()
		=> (await Expect.That(_stringArraySubject).IsEqualTo(_stringArrayExpectation))!.Consume(_consumer);

	[Benchmark]
	public AndConstraint<StringCollectionAssertions<IEnumerable<string>>> StringArray_FluentAssertions()
		=> _stringArraySubject.Should().Equal(_stringArrayExpectation);
}
