using System.Text.Json.Nodes;
using BenchmarkDotNet.Attributes;
using FluentAssertions;
using FluentAssertions.Specialized;

namespace aweXpect.Json.Benchmarks;

/// <summary>
///     In this benchmark we verify several expectations on the same <see cref="JsonNode" />, combined with <c>And</c>:
///     it has three properties, has no property "album" and is an object.<br />
///     FluentAssertions has no expectation that a node is an object, so it verifies that it is no array instead.
/// </summary>
public partial class HappyCaseBenchmarks
{
	[Benchmark]
	public async Task JsonPropertyChain_aweXpect()
		=> await Expect.That(_jsonNode)
			.HasProperty("title").And
			.HasProperty("artist").And
			.HasProperty("durationSeconds").And
			.DoesNotHaveProperty("album").And
			.IsObject();

	[Benchmark]
	public AndConstraint<JsonNodeAssertions<JsonNode>> JsonPropertyChain_FluentAssertions()
	{
		_jsonNode.Should().HaveProperty("title");
		_jsonNode.Should().HaveProperty("artist");
		_jsonNode.Should().HaveProperty("durationSeconds");
		_jsonNode.Should().NotHaveProperty("album");
		return _jsonNode.Should().NotBeAnArray();
	}
}
