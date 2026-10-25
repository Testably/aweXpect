using System.Text.Json.Nodes;
using BenchmarkDotNet.Attributes;
using FluentAssertions;
using FluentAssertions.Specialized;

namespace aweXpect.Json.Benchmarks;

/// <summary>
///     In this benchmark we verify that the object in a property of a <see cref="JsonNode" /> has three properties.
/// </summary>
public partial class HappyCaseBenchmarks
{
	private readonly JsonNode _nestedJsonNode = JsonNode.Parse($$"""{"track": {{TrackJson}}}""")!;

	[Benchmark]
	public async Task JsonNestedPropertyChain_aweXpect()
		=> await Expect.That(_nestedJsonNode)
			.HasProperty("track", track => track
				.HasProperty("title").And
				.HasProperty("artist").And
				.HasProperty("durationSeconds"));

	[Benchmark]
	public AndWhichConstraint<JsonNodeAssertions<JsonNode>, JsonNode> JsonNestedPropertyChain_FluentAssertions()
	{
		JsonNode track = _nestedJsonNode.Should().HaveProperty("track").Which;
		track.Should().HaveProperty("title");
		track.Should().HaveProperty("artist");
		return track.Should().HaveProperty("durationSeconds");
	}
}
