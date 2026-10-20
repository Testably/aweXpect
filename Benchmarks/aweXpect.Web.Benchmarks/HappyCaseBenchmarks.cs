using System.Net;
using BenchmarkDotNet.Attributes;

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
			Content = new StringContent("request-foo"),
		},
		Content = new StringContent("response-bar"),
		StatusCode = HttpStatusCode.OK,
	};
}
