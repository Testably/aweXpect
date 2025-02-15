using BenchmarkDotNet.Attributes;

namespace aweXpect.Json.Benchmarks;

/// <summary>
///     This is a dummy benchmark in the Json template.
/// </summary>
public partial class HappyCaseBenchmarks
{
	[Benchmark]
	public TimeSpan Dummy_aweXpect()
		=> TimeSpan.FromSeconds(10);
}
