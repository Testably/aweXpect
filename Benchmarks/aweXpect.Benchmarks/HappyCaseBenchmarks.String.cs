using BenchmarkDotNet.Attributes;
using FluentAssertions;
using FluentAssertions.Primitives;

namespace aweXpect.Benchmarks;

/// <summary>
///     In this benchmark we verify that a <see cref="string" /> is equal to another one.<br />
/// </summary>
public partial class HappyCaseBenchmarks
{
	private readonly string _expectedSongTitle = "Rio";
	private readonly string _capsSongTitle = "RIO";

	[Benchmark]
	public async Task<string?> String_aweXpect()
		=> await Expect.That(_capsSongTitle).IsEqualTo(_expectedSongTitle).IgnoringCase();

	[Benchmark]
	public AndConstraint<StringAssertions> String_FluentAssertions()
		=> _capsSongTitle.Should().BeEquivalentTo(_expectedSongTitle);

	[Benchmark]
	public async Task<string?> String_TUnit()
		=> await Assert.That(_capsSongTitle).IsEqualTo(_expectedSongTitle).IgnoringCase();
}
