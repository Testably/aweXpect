using BenchmarkDotNet.Attributes;

namespace aweXpect.Benchmarks;

/// <summary>
///     In this benchmark we verify that a delegate throws or does not throw.<br />
/// </summary>
public partial class HappyCaseBenchmarks
{
	[Benchmark]
	public async Task DoesNotThrow_aweXpect()
		=> await Expect.That(() => { }).DoesNotThrow();

	[Benchmark]
	public async Task Throws_aweXpect()
		=> await Expect.That(() => throw new InvalidOperationException()).Throws<InvalidOperationException>();
}
