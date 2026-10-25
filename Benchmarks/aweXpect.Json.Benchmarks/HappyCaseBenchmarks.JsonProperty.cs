using System.Text.Json.Nodes;
using BenchmarkDotNet.Attributes;
using FluentAssertions;
using FluentAssertions.Specialized;

namespace aweXpect.Json.Benchmarks;

/// <summary>
///     In this benchmark we verify single properties of a <see cref="JsonNode" />: the title has the expected value and
///     the duration is a number.
/// </summary>
public partial class HappyCaseBenchmarks
{
	[Benchmark]
	public async Task JsonProperty_aweXpect()
		=> await Expect.That(_jsonNode)
			.HasProperty("title", t => t.IsString("Let It Be")).And
			.HasProperty("durationSeconds", d => d.IsNumber());

	[Benchmark]
	public AndConstraint<JsonNodeAssertions<JsonNode>> JsonProperty_FluentAssertions()
	{
		_jsonNode.Should().HaveProperty("title").Which.GetValue<string>().Should().Be("Let It Be");
		return _jsonNode.Should().HaveProperty("durationSeconds").Which.Should().BeNumeric();
	}
}
