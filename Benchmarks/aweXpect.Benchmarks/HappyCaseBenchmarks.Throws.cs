using BenchmarkDotNet.Attributes;
using FluentAssertions;
using FluentAssertions.Specialized;

namespace aweXpect.Benchmarks;

/// <summary>
///     In this benchmark we verify that a delegate throws an exception with the expected message.<br />
/// </summary>
public partial class HappyCaseBenchmarks
{
	private readonly string _throwsMessage = "The track is already playing.";
	private readonly Action _throwsSubject = () => throw new InvalidOperationException("The track is already playing.");

	[Benchmark]
	public async Task Throws_aweXpect()
		=> await Expect.That(_throwsSubject).Throws<InvalidOperationException>().WithMessage(_throwsMessage);

	[Benchmark]
	public ExceptionAssertions<InvalidOperationException> Throws_FluentAssertions()
		=> _throwsSubject.Should().Throw<InvalidOperationException>().WithMessage(_throwsMessage);

	[Benchmark]
	public async Task Throws_TUnit()
		=> await Assert.That(_throwsSubject).Throws<InvalidOperationException>().WithMessage(_throwsMessage);
}
