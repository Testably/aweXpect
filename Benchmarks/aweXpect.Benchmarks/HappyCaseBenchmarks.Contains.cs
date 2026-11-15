using BenchmarkDotNet.Attributes;
using FluentAssertions;
using FluentAssertions.Collections;

namespace aweXpect.Benchmarks;

/// <summary>
///     In this benchmark we verify that a collection contains the expected item.<br />
/// </summary>
public partial class HappyCaseBenchmarks
{
	private readonly string _favoriteTitle = "Here Comes the Sun";
	private readonly List<string> _trackTitles = [..AbbeyRoadTitles,];

	[Benchmark]
	public async Task Contains_aweXpect()
		=> await Expect.That(_trackTitles).Contains(_favoriteTitle);

	[Benchmark]
	public AndWhichConstraint<StringCollectionAssertions<IEnumerable<string>>, string> Contains_FluentAssertions()
		=> _trackTitles.Should().Contain(_favoriteTitle);

	[Benchmark]
	public async Task Contains_TUnit()
		=> await Assert.That(_trackTitles).Contains(_favoriteTitle);
}
