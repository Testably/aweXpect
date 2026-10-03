using BenchmarkDotNet.Attributes;

namespace aweXpect.Benchmarks;

/// <summary>
///     In this benchmark we verify that a large collection is equal to a copy of it.<br />
/// </summary>
public partial class HappyCaseBenchmarks
{
	private readonly int[] _largeIntExpectation = Enumerable.Range(0, 10_000).ToArray();
	private readonly int[] _largeIntSubject = Enumerable.Range(0, 10_000).ToArray();

	private readonly string[] _largeStringExpectation =
		Enumerable.Range(0, 10_000).Select(x => $"item{x}").ToArray();

	private readonly string[] _largeStringSubject = Enumerable.Range(0, 10_000).Select(x => $"item{x}").ToArray();

	[Benchmark]
	public async Task LargeIntArray_aweXpect()
		=> await Expect.That(_largeIntSubject).IsEqualTo(_largeIntExpectation);

	[Benchmark]
	public async Task LargeStringArray_aweXpect()
		=> await Expect.That(_largeStringSubject).IsEqualTo(_largeStringExpectation);
}
