using BenchmarkDotNet.Attributes;
using FluentAssertions;
using FluentAssertions.Execution;

namespace aweXpect.Benchmarks;

/// <summary>
///     In this benchmark we verify several expectations at once, so that all failures would be reported together.<br />
/// </summary>
public partial class HappyCaseBenchmarks
{
	private readonly Album _abbeyRoadRecord = Album.Create();

	[Benchmark]
	public async Task Multiple_aweXpect()
		=> await Expect.ThatAll(
			Expect.That(_abbeyRoadRecord.Title).IsEqualTo("Abbey Road"),
			Expect.That(_abbeyRoadRecord.Artist.Name).IsEqualTo("The Beatles"),
			Expect.That(_abbeyRoadRecord.Genre).IsEqualTo(Genre.Rock));

	[Benchmark]
	public void Multiple_FluentAssertions()
	{
		using AssertionScope _ = new();
		_abbeyRoadRecord.Title.Should().Be("Abbey Road");
		_abbeyRoadRecord.Artist.Name.Should().Be("The Beatles");
		_abbeyRoadRecord.Genre.Should().Be(Genre.Rock);
	}

	[Benchmark]
	public async Task Multiple_TUnit()
	{
		using (Assert.Multiple())
		{
			await Assert.That(_abbeyRoadRecord.Title).IsEqualTo("Abbey Road");
			await Assert.That(_abbeyRoadRecord.Artist.Name).IsEqualTo("The Beatles");
			await Assert.That(_abbeyRoadRecord.Genre).IsEqualTo(Genre.Rock);
		}
	}
}
