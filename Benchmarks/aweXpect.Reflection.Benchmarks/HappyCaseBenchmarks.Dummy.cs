using BenchmarkDotNet.Attributes;

namespace aweXpect.Reflection.Benchmarks;

/// <summary>
///     This is a dummy benchmark in the Reflection template.
/// </summary>
public partial class HappyCaseBenchmarks
{
	[Benchmark]
	public TimeSpan Dummy_aweXpect()
		=> TimeSpan.FromSeconds(10);
}
