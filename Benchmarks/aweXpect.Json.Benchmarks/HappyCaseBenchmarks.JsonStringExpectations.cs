using System.Text.Json.Nodes;
using BenchmarkDotNet.Attributes;
using FluentAssertions;
using FluentAssertions.Primitives;

namespace aweXpect.Json.Benchmarks;

/// <summary>
///     In this benchmark we verify that a JSON <see cref="string" /> is valid JSON whose title has the expected value.<br />
///     FluentAssertions has no expectations on JSON strings, so it parses the string into a <see cref="JsonNode" />.
/// </summary>
public partial class HappyCaseBenchmarks
{
	[Benchmark]
	public async Task JsonStringExpectations_aweXpect()
		=> await Expect.That(_jsonString).IsValidJson(json => json.HasProperty("title", t => t.IsString("Let It Be")));

	[Benchmark]
	public AndConstraint<StringAssertions> JsonStringExpectations_FluentAssertions()
		=> JsonNode.Parse(_jsonString)!.Should().HaveProperty("title").Which.GetValue<string>().Should()
			.Be("Let It Be");
}
