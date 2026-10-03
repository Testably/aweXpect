using BenchmarkDotNet.Attributes;

namespace aweXpect.Benchmarks;

/// <summary>
///     In this benchmark we verify several expectations together.<br />
/// </summary>
public partial class HappyCaseBenchmarks
{
	[Benchmark]
	public async Task ThatAll_aweXpect()
		=> await Expect.ThatAll(
			Expect.That(_intSubject).IsEqualTo(42),
			Expect.That(_stringArraySubject).IsEqualTo(_stringArrayExpectation),
			Expect.That(_stringSubject).StartsWith("F"));
}
