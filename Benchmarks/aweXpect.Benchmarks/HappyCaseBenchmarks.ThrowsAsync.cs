using BenchmarkDotNet.Attributes;
using FluentAssertions;

namespace aweXpect.Benchmarks;

/// <summary>
///     In this benchmark we verify that an asynchronous delegate throws an exception with the expected message.<br />
/// </summary>
public partial class HappyCaseBenchmarks
{
	private readonly Func<Task> _playTwiceAsync = async () =>
	{
		await Task.CompletedTask;
		throw new InvalidOperationException("The track is already playing.");
	};

	[Benchmark]
	public async Task ThrowsAsync_aweXpect()
		=> await Expect.That(_playTwiceAsync).Throws<InvalidOperationException>().WithMessage(_alreadyPlayingMessage);

	[Benchmark]
	public async Task ThrowsAsync_FluentAssertions()
		=> await _playTwiceAsync.Should().ThrowAsync<InvalidOperationException>().WithMessage(_alreadyPlayingMessage);

	[Benchmark]
	public async Task ThrowsAsync_TUnit()
		=> await Assert.That(_playTwiceAsync).Throws<InvalidOperationException>().WithMessage(_alreadyPlayingMessage);
}
