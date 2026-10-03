using BenchmarkDotNet.Attributes;

namespace aweXpect.Benchmarks;

/// <summary>
///     In this benchmark we verify that all items of a collection comply with an expectation.<br />
/// </summary>
public partial class HappyCaseBenchmarks
{
	private readonly List<int> _complyWithInts = Enumerable.Range(1, 1000).ToList();
	private readonly List<string> _complyWithStrings = Enumerable.Repeat("a", 1000).ToList();

	[Benchmark]
	public async Task ComplyWithInt_aweXpect()
		=> await Expect.That(_complyWithInts).All().ComplyWith(x => x.IsGreaterThan(0));

	[Benchmark]
	public async Task ComplyWithString_aweXpect()
		=> await Expect.That(_complyWithStrings).All().ComplyWith(x => x.IsEqualTo("a"));
}
