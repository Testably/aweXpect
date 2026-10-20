using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Toolchains.InProcess.Emit;

namespace aweXpect.Json.Benchmarks;

[Config(typeof(Config))]
[MarkdownExporterAttribute.GitHub]
[MemoryDiagnoser]
public partial class HappyCaseBenchmarks
{
	private const string TrackJson = """{"title": "Let It Be", "artist": "The Beatles", "durationSeconds": 243}""";

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
