using BenchmarkDotNet.Attributes;
using FluentAssertions;
using FluentAssertions.Specialized;

namespace aweXpect.Benchmarks;

/// <summary>
///     In this benchmark we verify that a delegate throws an exception with the expected message.<br />
/// </summary>
public partial class HappyCaseBenchmarks
{
	private readonly string _playbackError = "The track is already playing.";
	private readonly Action _playSongTwice = () => throw new InvalidOperationException("The track is already playing.");

	[Benchmark]
	public async Task Throws_aweXpect()
		=> await Expect.That(_playSongTwice).Throws<InvalidOperationException>().WithMessage(_playbackError);

	[Benchmark]
	public ExceptionAssertions<InvalidOperationException> Throws_FluentAssertions()
		=> _playSongTwice.Should().Throw<InvalidOperationException>().WithMessage(_playbackError);

	[Benchmark]
	public async Task Throws_TUnit()
		=> await Assert.That(_playSongTwice).Throws<InvalidOperationException>().WithMessage(_playbackError);
}
