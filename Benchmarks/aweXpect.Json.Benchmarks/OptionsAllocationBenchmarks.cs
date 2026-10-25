using System.Text.Json;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Toolchains.InProcess.Emit;
using Perfolizer.Metrology;

namespace aweXpect.Json.Benchmarks;

/// <summary>
///     In these benchmarks we measure the time and the allocations of JSON expectations with options, so that a change
///     of the options model can be compared with the state before it.
/// </summary>
[Config(typeof(Config))]
[MarkdownExporterAttribute.GitHub]
[MemoryDiagnoser]
public class OptionsAllocationBenchmarks
{
	private readonly JsonElement _title = JsonDocument.Parse("\"Let It Be\"").RootElement;

	[Benchmark]
	public async Task<JsonElement> Json_IsString_AsPrefix()
		=> await Expect.That(_title).IsString("Let").AsPrefix();

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
