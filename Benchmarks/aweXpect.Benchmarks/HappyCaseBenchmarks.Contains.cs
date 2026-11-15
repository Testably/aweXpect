using BenchmarkDotNet.Attributes;
using FluentAssertions;
using FluentAssertions.Collections;

namespace aweXpect.Benchmarks;

/// <summary>
///     In this benchmark we verify that a collection contains the expected item.<br />
/// </summary>
public partial class HappyCaseBenchmarks
{
	private readonly string _mostStreamedSong = "Here Comes the Sun";
	private readonly List<string> _abbeyRoadTracks = [..AbbeyRoadTitles,];

	[Benchmark]
	public async Task Contains_aweXpect()
		=> await Expect.That(_abbeyRoadTracks).Contains(_mostStreamedSong);

	[Benchmark]
	public AndWhichConstraint<StringCollectionAssertions<IEnumerable<string>>, string> Contains_FluentAssertions()
		=> _abbeyRoadTracks.Should().Contain(_mostStreamedSong);

	[Benchmark]
	public async Task Contains_TUnit()
		=> await Assert.That(_abbeyRoadTracks).Contains(_mostStreamedSong);
}
