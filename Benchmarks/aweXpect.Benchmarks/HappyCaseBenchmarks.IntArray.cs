using BenchmarkDotNet.Attributes;

namespace aweXpect.Benchmarks;

/// <summary>
///     In this benchmark we verify that an <see langword="int" /> collection is equal to another one or contains an item.<br />
/// </summary>
public partial class HappyCaseBenchmarks
{
	private readonly int[] _intArrayExpectation = Enumerable.Range(0, 1000).ToArray();
	private readonly int[] _intArraySubject = Enumerable.Range(0, 1000).ToArray();
	private readonly List<int> _intListSubject = Enumerable.Range(0, 1000).ToList();

	[Benchmark]
	public async Task IntArray_aweXpect()
		=> await Expect.That(_intArraySubject).IsEqualTo(_intArrayExpectation);

	[Benchmark]
	public async Task IntLazy_aweXpect()
		=> await Expect.That(_intArraySubject.Select(x => x)).IsEqualTo(_intArrayExpectation);

	[Benchmark]
	public async Task IntListContains_aweXpect()
		=> await Expect.That(_intListSubject).Contains(999);
}
