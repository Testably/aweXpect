using System.Text.Json;
using System.Text.Json.Nodes;
using BenchmarkDotNet.Attributes;
using FluentAssertions;
using FluentAssertions.Specialized;

namespace aweXpect.Json.Benchmarks;

/// <summary>
///     In this benchmark we verify that parsed JSON matches an expected object.<br />
///     aweXpect verifies a <see cref="JsonElement" />, FluentAssertions a <see cref="JsonNode" />, as it has no
///     expectations on <see cref="JsonElement" />.
/// </summary>
public partial class HappyCaseBenchmarks
{
	private readonly JsonElement _jsonElement = JsonSerializer.Deserialize<JsonElement>(TrackJson);
	private readonly JsonNode _jsonNode = JsonNode.Parse(TrackJson)!;

	[Benchmark]
	public async Task ParsedJson_aweXpect()
		=> await Expect.That(_jsonElement).Matches(new { title = "Let It Be", artist = "The Beatles", });

	[Benchmark]
	public AndConstraint<JsonNodeAssertions<JsonNode>> ParsedJson_FluentAssertions()
		=> _jsonNode.Should().BeEquivalentTo(new { title = "Let It Be", artist = "The Beatles", });
}
