using BenchmarkDotNet.Attributes;

namespace aweXpect.Web.Benchmarks;

/// <summary>
///     In this benchmark we verify that the response has the correct content.<br />
/// </summary>
public partial class HappyCaseBenchmarks
{
	[Benchmark]
	public async Task Content_aweXpect()
		=> await Expect.That(_response).HasContent("response-bar");
}
