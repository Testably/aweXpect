using BenchmarkDotNet.Attributes;

namespace aweXpect.Web.Benchmarks;

/// <summary>
///     This is a dummy benchmark in the Web template.
/// </summary>
public partial class HappyCaseBenchmarks
{
	[Benchmark]
	public TimeSpan Dummy_aweXpect()
		=> TimeSpan.FromSeconds(10);
}
