using BenchmarkDotNet.Attributes;
using FluentAssertions;
using FluentAssertions.Collections;

namespace aweXpect.Benchmarks;

/// <summary>
///     In this benchmark we verify that a collection contains the expected item.<br />
/// </summary>
public partial class HappyCaseBenchmarks
{
	private readonly string _containsExpected = "Here Comes the Sun";
	private readonly List<string> _containsSubject = [..AbbeyRoadTitles,];

	[Benchmark]
	public async Task Contains_aweXpect()
		=> await Expect.That(_containsSubject).Contains(_containsExpected);

	[Benchmark]
	public AndWhichConstraint<StringCollectionAssertions<IEnumerable<string>>, string> Contains_FluentAssertions()
		=> _containsSubject.Should().Contain(_containsExpected);

	[Benchmark]
	public async Task Contains_TUnit()
		=> await Assert.That(_containsSubject).Contains(_containsExpected);
}
