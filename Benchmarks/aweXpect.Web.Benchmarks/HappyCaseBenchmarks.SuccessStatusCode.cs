using BenchmarkDotNet.Attributes;

namespace aweXpect.Web.Benchmarks;

/// <summary>
///     In this benchmark we verify that the response has a success status code.<br />
/// </summary>
public partial class HappyCaseBenchmarks
{
	[Benchmark]
	public async Task SuccessStatusCode_aweXpect()
		=> await Expect.That(_response).HasStatusCode().Success();
}
