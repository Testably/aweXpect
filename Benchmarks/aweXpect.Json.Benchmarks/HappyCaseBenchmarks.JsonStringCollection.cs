using System.Text.Json.Nodes;
using BenchmarkDotNet.Attributes;
using FluentAssertions;
using FluentAssertions.Collections;

namespace aweXpect.Json.Benchmarks;

/// <summary>
///     In this benchmark we verify that all 100 JSON <see cref="string" />s of a collection are equal to an expected JSON
///     string with a different property order and formatting.<br />
///     FluentAssertions has no expectations on JSON strings, so it parses the expected string once and verifies the
///     result of <see cref="JsonNode.DeepEquals" /> for every item.
/// </summary>
public partial class HappyCaseBenchmarks
{
	private readonly string[] _jsonStrings = Enumerable.Repeat(TrackJson, 100).ToArray();

	[Benchmark]
	public async Task JsonStringCollection_aweXpect()
		=> await Expect.That(_jsonStrings).All().AreEqualTo(_expectedJsonString).AsJson();

	[Benchmark]
	public AndConstraint<StringCollectionAssertions<IEnumerable<string>>> JsonStringCollection_FluentAssertions()
	{
		JsonNode? expected = JsonNode.Parse(_expectedJsonString);
		return _jsonStrings.Should()
			.AllSatisfy(jsonString => JsonNode.DeepEquals(JsonNode.Parse(jsonString), expected).Should().BeTrue());
	}
}
