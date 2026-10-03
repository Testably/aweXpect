using aweXpect.Customization;
using aweXpect.Results;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Toolchains.InProcess.Emit;
using FluentAssertions;
using FluentAssertions.Primitives;

namespace aweXpect.Benchmarks;

[Config(typeof(Config))]
[MemoryDiagnoser]
public class BoolBreakdownBenchmarks
{
	private readonly bool _subject = true;

	[Benchmark(Baseline = true)]
	public async Task<bool> Full_aweXpect()
		=> await Expect.That(_subject).IsTrue();

	[Benchmark]
	public object BuildOnly_aweXpect()
		=> Expect.That(_subject).IsTrue();

	[Benchmark]
	public object? CustomizationReads()
	{
		object? testCancellation = Customize.aweXpect.Settings().TestCancellation.Get();
		return testCancellation;
	}

	[Benchmark]
	public async Task<bool> TaskFromResult()
		=> await Task.FromResult(_subject);

	[Benchmark]
	public AndConstraint<BooleanAssertions> FluentAssertions()
		=> _subject.Should().BeTrue();

	private sealed class Config : ManualConfig
	{
		public Config()
		{
			AddJob(Job.MediumRun
				.WithLaunchCount(1)
				.WithToolchain(InProcessEmitToolchain.Instance)
				.WithId("InProcess"));
		}
	}
}
