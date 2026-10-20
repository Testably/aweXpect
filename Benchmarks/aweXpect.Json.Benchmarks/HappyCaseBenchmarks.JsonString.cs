using System.Text.Json.Nodes;
using BenchmarkDotNet.Attributes;
using FluentAssertions;
using FluentAssertions.Specialized;

namespace aweXpect.Json.Benchmarks;

/// <summary>
///     In this benchmark we verify that a JSON <see cref="string" /> matches an expected object.<br />
///     FluentAssertions has no expectations on JSON strings, so it parses the string into a <see cref="JsonNode" />.
/// </summary>
public partial class HappyCaseBenchmarks
{
	private readonly string _jsonString = TrackJson;

	[Benchmark]
	public async Task JsonString_aweXpect()
		=> await Expect.That(_jsonString).IsValidJsonMatching(new { title = "Let It Be", artist = "The Beatles", });

	[Benchmark]
	public AndConstraint<JsonNodeAssertions<JsonNode?>> JsonString_FluentAssertions()
		=> JsonNode.Parse(_jsonString).Should().BeEquivalentTo(new { title = "Let It Be", artist = "The Beatles", });
}
