using BenchmarkDotNet.Attributes;
using FluentAssertions;
using FluentAssertions.Execution;

namespace aweXpect.Benchmarks;

/// <summary>
///     In this benchmark we verify several expectations at once, so that all failures would be reported together.<br />
/// </summary>
public partial class HappyCaseBenchmarks
{
	private readonly Album _multipleSubject = Album.Create();

	[Benchmark]
	public async Task Multiple_aweXpect()
		=> await Expect.ThatAll(
			Expect.That(_multipleSubject.Title).IsEqualTo("Abbey Road"),
			Expect.That(_multipleSubject.Artist.Name).IsEqualTo("The Beatles"),
			Expect.That(_multipleSubject.Genre).IsEqualTo(Genre.Rock));

	[Benchmark]
	public void Multiple_FluentAssertions()
	{
		using AssertionScope _ = new();
		_multipleSubject.Title.Should().Be("Abbey Road");
		_multipleSubject.Artist.Name.Should().Be("The Beatles");
		_multipleSubject.Genre.Should().Be(Genre.Rock);
	}

	[Benchmark]
	public async Task Multiple_TUnit()
	{
		using (Assert.Multiple())
		{
			await Assert.That(_multipleSubject.Title).IsEqualTo("Abbey Road");
			await Assert.That(_multipleSubject.Artist.Name).IsEqualTo("The Beatles");
			await Assert.That(_multipleSubject.Genre).IsEqualTo(Genre.Rock);
		}
	}
}
