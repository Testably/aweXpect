using BenchmarkDotNet.Attributes;

namespace aweXpect.Xml.Benchmarks;

public partial class HappyCaseBenchmarks
{
	[Benchmark]
	public async Task Dummy_aweXpect()
		=> await Expect.That(true).IsTrue();
}
