using System.Text.Json;
using System.Text.Json.Nodes;
using BenchmarkDotNet.Attributes;
using FluentAssertions;
using FluentAssertions.Specialized;

namespace aweXpect.Json.Benchmarks;

/// <summary>
///     In this benchmark we verify that a JSON array of 10 titles matches the expected titles in reverse order.<br />
///     aweXpect verifies a <see cref="JsonElement" />, FluentAssertions a <see cref="JsonNode" />, as it has no
///     expectations on <see cref="JsonElement" />; its <c>BeEquivalentTo</c> ignores the order by default.
/// </summary>
public partial class HappyCaseBenchmarks
{
	private const string TitlesJson = """
	                                  ["Two of Us", "Dig a Pony", "Across the Universe", "I Me Mine", "Dig It",
	                                   "Let It Be", "Maggie Mae", "I've Got a Feeling", "One After 909", "The Long and Winding Road"]
	                                  """;

	private readonly string[] _reversedTitles =
	[
		"The Long and Winding Road", "One After 909", "I've Got a Feeling", "Maggie Mae", "Let It Be",
		"Dig It", "I Me Mine", "Across the Universe", "Dig a Pony", "Two of Us",
	];

	private readonly JsonElement _titlesElement = JsonSerializer.Deserialize<JsonElement>(TitlesJson);
	private readonly JsonNode _titlesNode = JsonNode.Parse(TitlesJson)!;

	[Benchmark]
	public async Task JsonArrayInAnyOrder_aweXpect()
		=> await Expect.That(_titlesElement).MatchesExactly(_reversedTitles, o => o.IgnoringArrayOrder());

	[Benchmark]
	public AndConstraint<JsonNodeAssertions<JsonNode>> JsonArrayInAnyOrder_FluentAssertions()
		=> _titlesNode.Should().BeEquivalentTo(_reversedTitles);
}
