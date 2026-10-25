using aweXpect.Reflection.Collections;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Toolchains.InProcess.Emit;
using Perfolizer.Metrology;

namespace aweXpect.Reflection.Benchmarks;

/// <summary>
///     In these benchmarks we measure the time and the allocations of filters with string options, so that a change of
///     the options model can be compared with the state before it.
/// </summary>
[Config(typeof(Config))]
[MarkdownExporterAttribute.GitHub]
[MemoryDiagnoser]
public class OptionsAllocationBenchmarks
{
	private readonly string _regexPattern = "^Album.*$";

	private readonly Filtered.Types.StringEqualityResult _storedFilter =
		In.AssemblyContaining<OptionsAllocationBenchmarks>().Types().WithName("album").AsPrefix();

	private readonly string _wildcardPattern = "Album*";

	[Benchmark]
	public async Task Types_WithName_AsWildcard()
		=> await Expect.That(In.AssemblyContaining<OptionsAllocationBenchmarks>().Types().WithName(_wildcardPattern)
				.AsWildcard())
			.AreSealed();

	[Benchmark]
	public async Task Types_WithName_AsRegex()
		=> await Expect.That(In.AssemblyContaining<OptionsAllocationBenchmarks>().Types().WithName(_regexPattern)
				.AsRegex())
			.AreSealed();

	[Benchmark]
	public async Task Types_StoredFilter_IgnoringCase()
		=> await Expect.That(_storedFilter.IgnoringCase()).AreSealed();

	private sealed class Config : ManualConfig
	{
		public Config()
		{
			AddJob(Job.MediumRun
				.WithLaunchCount(1)
				.WithToolchain(InProcessEmitToolchain.Instance)
				.WithId("InProcess"));
			SummaryStyle = SummaryStyle.Default.WithSizeUnit(SizeUnit.B);
		}
	}
}
