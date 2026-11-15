using BenchmarkDotNet.Attributes;
using FluentAssertions;
using FluentAssertions.Primitives;

namespace aweXpect.Web.Benchmarks;

/// <summary>
///     In this benchmark we verify that the response has a header with the expected value.<br />
///     FluentAssertions has no expectations on <see cref="HttpResponseMessage" />, so it verifies that the header has a
///     single value which is equal to the expected one.
/// </summary>
public partial class HappyCaseBenchmarks
{
	[Benchmark]
	public async Task Header_aweXpect()
		=> await Expect.That(_response).HasHeader("x-writer").WithValue("Lennon");

	[Benchmark]
	public AndConstraint<StringAssertions> Header_FluentAssertions()
		=> _response.Headers.GetValues("x-writer").Should().ContainSingle().Which.Should().Be("Lennon");
}
