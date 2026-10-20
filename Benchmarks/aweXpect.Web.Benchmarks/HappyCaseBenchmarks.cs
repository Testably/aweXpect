using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Toolchains.InProcess.Emit;
using System.Net;

namespace aweXpect.Web.Benchmarks;

[Config(typeof(Config))]
[MarkdownExporterAttribute.GitHub]
[MemoryDiagnoser]
public partial class HappyCaseBenchmarks
{
	private readonly HttpResponseMessage _response = new()
	{
		RequestMessage = new HttpRequestMessage
		{
			Method = HttpMethod.Get,
			RequestUri = new Uri("https://music.example.com/tracks/1"),
			Content = new StringContent("request-foo"),
		},
		Content = new StringContent("response-bar"),
		Headers =
		{
			{ "x-vendor", "VENDOR" },
		},
		StatusCode = HttpStatusCode.OK,
	};

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
