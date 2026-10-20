using BenchmarkDotNet.Attributes;
using FluentAssertions;
using FluentAssertions.Primitives;

namespace aweXpect.Web.Benchmarks;

/// <summary>
///     In this benchmark we verify that the response has the correct content.<br />
///     FluentAssertions has no expectations on <see cref="HttpResponseMessage" />, so it reads the content as a
///     <see cref="string" />.
/// </summary>
public partial class HappyCaseBenchmarks
{
	[Benchmark]
	public async Task Content_aweXpect()
		=> await Expect.That(_response).HasContent("response-bar");

	[Benchmark]
	public async Task<AndConstraint<StringAssertions>> Content_FluentAssertions()
		=> (await _response.Content.ReadAsStringAsync()).Should().Be("response-bar");
}
