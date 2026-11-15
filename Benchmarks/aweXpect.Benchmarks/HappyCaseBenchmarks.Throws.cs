using BenchmarkDotNet.Attributes;
using FluentAssertions;
using FluentAssertions.Specialized;

namespace aweXpect.Benchmarks;

/// <summary>
///     In this benchmark we verify that a delegate throws an exception with the expected message.<br />
/// </summary>
public partial class HappyCaseBenchmarks
{
	private readonly string _alreadyPlayingMessage = "The track is already playing.";
	private readonly Action _playTwice = () => throw new InvalidOperationException("The track is already playing.");

	[Benchmark]
	public async Task Throws_aweXpect()
		=> await Expect.That(_playTwice).Throws<InvalidOperationException>().WithMessage(_alreadyPlayingMessage);

	[Benchmark]
	public ExceptionAssertions<InvalidOperationException> Throws_FluentAssertions()
		=> _playTwice.Should().Throw<InvalidOperationException>().WithMessage(_alreadyPlayingMessage);

	[Benchmark]
	public async Task Throws_TUnit()
		=> await Assert.That(_playTwice).Throws<InvalidOperationException>().WithMessage(_alreadyPlayingMessage);
}
