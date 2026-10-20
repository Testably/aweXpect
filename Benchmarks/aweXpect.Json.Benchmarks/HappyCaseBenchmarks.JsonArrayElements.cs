using System.Text.Json;
using System.Text.Json.Nodes;
using BenchmarkDotNet.Attributes;
using FluentAssertions;
using FluentAssertions.Collections;

namespace aweXpect.Json.Benchmarks;

/// <summary>
///     In this benchmark we verify that all elements of a JSON array match an expected object.<br />
///     aweXpect verifies a <see cref="JsonElement" />, FluentAssertions a <see cref="JsonArray" />, as it has no
///     expectations on <see cref="JsonElement" />.
/// </summary>
public partial class HappyCaseBenchmarks
{
	private const string TracksJson = """
	                                  [
	                                    {"title": "Let It Be", "artist": "The Beatles", "durationSeconds": 243},
	                                    {"title": "Yesterday", "artist": "The Beatles", "durationSeconds": 125},
	                                    {"title": "Hey Jude", "artist": "The Beatles", "durationSeconds": 431}
	                                  ]
	                                  """;

	private readonly JsonElement _tracksElement = JsonSerializer.Deserialize<JsonElement>(TracksJson);
	private readonly JsonArray _tracksNode = JsonNode.Parse(TracksJson)!.AsArray();

	[Benchmark]
	public async Task JsonArrayElements_aweXpect()
		=> await Expect.That(_tracksElement).IsArray().WhoseElements
			.All().ComplyWith(t => t.Matches(new { artist = "The Beatles", }));

	[Benchmark]
	public AndConstraint<GenericCollectionAssertions<JsonNode?>> JsonArrayElements_FluentAssertions()
		=> _tracksNode.Should().AllSatisfy(t => t.Should().BeEquivalentTo(new { artist = "The Beatles", }));
}
