using System.Text.Json.Nodes;
using BenchmarkDotNet.Attributes;
using FluentAssertions;
using FluentAssertions.Primitives;

namespace aweXpect.Json.Benchmarks;

/// <summary>
///     In this benchmark we verify that a JSON <see cref="string" /> is equal to an expected JSON string with a different
///     property order and formatting.<br />
///     FluentAssertions has no expectations on JSON strings and fails to compare two <see cref="JsonNode" />s with
///     <c>BeEquivalentTo</c>, so it parses both strings and verifies the result of <see cref="JsonNode.DeepEquals" />.
/// </summary>
public partial class HappyCaseBenchmarks
{
	private readonly string _expectedJsonString = """
	                                              {
	                                                "artist": "The Beatles",
	                                                "durationSeconds": 243,
	                                                "title": "Let It Be"
	                                              }
	                                              """;

	[Benchmark]
	public async Task JsonEquality_aweXpect()
		=> await Expect.That(_jsonString).IsEqualTo(_expectedJsonString).AsJson();

	[Benchmark]
	public AndConstraint<BooleanAssertions> JsonEquality_FluentAssertions()
		=> JsonNode.DeepEquals(JsonNode.Parse(_jsonString), JsonNode.Parse(_expectedJsonString)).Should().BeTrue();
}
