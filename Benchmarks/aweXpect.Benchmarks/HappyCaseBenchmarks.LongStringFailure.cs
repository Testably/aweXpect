using BenchmarkDotNet.Attributes;

namespace aweXpect.Benchmarks;

/// <summary>
///     In this benchmark we verify the failure of two long strings that differ near the end.<br />
/// </summary>
public partial class HappyCaseBenchmarks
{
	private readonly string _longStringExpectation = new string('a', 10_000_000) + "b";
	private readonly string _longStringSubject = new string('a', 10_000_000) + "c";

	[Benchmark]
	public async Task<string> LongStringFailure_aweXpect()
	{
		try
		{
			await Expect.That(_longStringSubject).IsEqualTo(_longStringExpectation);
			return "";
		}
		catch (Exception exception)
		{
			return exception.Message;
		}
	}
}
