using System.Net;
using System.Text;
using System.Text.Json.Nodes;
using BenchmarkDotNet.Attributes;
using FluentAssertions;
using FluentAssertions.Specialized;

namespace aweXpect.Web.Benchmarks;

/// <summary>
///     In this benchmark we verify that the response has a problem details content with the expected type, title and
///     status.<br />
///     FluentAssertions has no expectations on <see cref="HttpResponseMessage" />, so it reads the content as a
///     <see cref="string" />, parses it into a <see cref="JsonNode" /> and compares it with an anonymous object.
/// </summary>
public partial class HappyCaseBenchmarks
{
	private readonly HttpResponseMessage _problemResponse = new()
	{
		Content = new StringContent(
			"""{"type": "https://httpstatuses.com/404", "title": "Track not found", "status": 404, "instance": "/tracks/42"}""",
			Encoding.UTF8, "application/problem+json"),
		StatusCode = HttpStatusCode.NotFound,
	};

	[Benchmark]
	public async Task ProblemDetails_aweXpect()
		=> await Expect.That(_problemResponse)
			.HasProblemDetailsContent("https://httpstatuses.com/404")
			.WithTitle("Track not found")
			.WithStatus(404);

	[Benchmark]
	public async Task<AndConstraint<JsonNodeAssertions<JsonNode?>>> ProblemDetails_FluentAssertions()
		=> JsonNode.Parse(await _problemResponse.Content.ReadAsStringAsync()).Should().BeEquivalentTo(new
		{
			type = "https://httpstatuses.com/404",
			title = "Track not found",
			status = 404,
		});
}
