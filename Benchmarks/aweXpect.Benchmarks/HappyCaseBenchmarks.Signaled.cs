using aweXpect.Signaling;
using BenchmarkDotNet.Attributes;

namespace aweXpect.Benchmarks;

/// <summary>
///     In this benchmark we verify that an already signaled <see cref="Signaler" /> was signaled.<br />
/// </summary>
public partial class HappyCaseBenchmarks
{
	private readonly Signaler _signaledSubject = CreateSignaledSignaler();

	[Benchmark]
	public async Task<SignalerResult> Signaled_aweXpect()
		=> await Expect.That(_signaledSubject).Signaled();

	private static Signaler CreateSignaledSignaler()
	{
		Signaler signaler = new();
		signaler.Signal();
		return signaler;
	}
}
