using BenchmarkDotNet.Attributes;
using FluentAssertions;

namespace aweXpect.Benchmarks;

/// <summary>
///     In this benchmark we verify that an asynchronous delegate throws an exception with the expected message.<br />
/// </summary>
public partial class HappyCaseBenchmarks
{
	private readonly Func<Task> _playSongTwiceAsync = async () =>
	{
		await Task.CompletedTask;
		throw new InvalidOperationException("The track is already playing.");
	};

	[Benchmark]
	public async Task ThrowsAsync_aweXpect()
		=> await Expect.That(_playSongTwiceAsync).Throws<InvalidOperationException>().WithMessage(_playbackError);

	[Benchmark]
	public async Task ThrowsAsync_FluentAssertions()
		=> await _playSongTwiceAsync.Should().ThrowAsync<InvalidOperationException>().WithMessage(_playbackError);

	[Benchmark]
	public async Task ThrowsAsync_TUnit()
		=> await Assert.That(_playSongTwiceAsync).Throws<InvalidOperationException>().WithMessage(_playbackError);
}
