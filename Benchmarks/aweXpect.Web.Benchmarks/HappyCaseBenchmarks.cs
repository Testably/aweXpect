using System.Net;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Toolchains.InProcess.Emit;

namespace aweXpect.Web.Benchmarks;

[MarkdownExporterAttribute.GitHub]
[MemoryDiagnoser]
public partial class HappyCaseBenchmarks
{
	private readonly HttpResponseMessage _response = new()
	{
		RequestMessage = new HttpRequestMessage
		{
			Method = HttpMethod.Get,
			RequestUri = new Uri("https://aweXpect.com"),
			Content = new StringContent("request-foo"),
		},
		Content = new StringContent("response-bar"),
		StatusCode = HttpStatusCode.OK,
	};

	private class Config : ManualConfig
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
