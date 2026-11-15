using BenchmarkDotNet.Attributes;
using FluentAssertions;
using FluentAssertions.Execution;

namespace aweXpect.Benchmarks;

/// <summary>
///     In this benchmark we verify several expectations at once, so that all failures would be reported together.<br />
/// </summary>
public partial class HappyCaseBenchmarks
{
	private readonly Album _album = Album.Create();

	[Benchmark]
	public async Task Multiple_aweXpect()
		=> await Expect.ThatAll(
			Expect.That(_album.Title).IsEqualTo("Abbey Road"),
			Expect.That(_album.Artist.Name).IsEqualTo("The Beatles"),
			Expect.That(_album.Genre).IsEqualTo(Genre.Rock));

	[Benchmark]
	public void Multiple_FluentAssertions()
	{
		using AssertionScope _ = new();
		_album.Title.Should().Be("Abbey Road");
		_album.Artist.Name.Should().Be("The Beatles");
		_album.Genre.Should().Be(Genre.Rock);
	}

	[Benchmark]
	public async Task Multiple_TUnit()
	{
		using (Assert.Multiple())
		{
			await Assert.That(_album.Title).IsEqualTo("Abbey Road");
			await Assert.That(_album.Artist.Name).IsEqualTo("The Beatles");
			await Assert.That(_album.Genre).IsEqualTo(Genre.Rock);
		}
	}
}
