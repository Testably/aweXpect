using BenchmarkDotNet.Attributes;

namespace aweXpect.Reflection.Benchmarks;

/// <summary>
///     In this benchmark we verify that the <see cref="HappyCaseBenchmarks" /> type is not static.
/// </summary>
public partial class HappyCaseBenchmarks
{
	private readonly Type _type = typeof(HappyCaseBenchmarks);

	[Benchmark]
	public async Task TypeIsNotStatic_aweXpect()
		=> await Expect.That(_type).IsNotStatic();
}
