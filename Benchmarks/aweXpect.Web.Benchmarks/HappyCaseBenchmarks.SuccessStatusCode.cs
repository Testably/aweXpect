using BenchmarkDotNet.Attributes;
using FluentAssertions;
using FluentAssertions.Primitives;

namespace aweXpect.Web.Benchmarks;

/// <summary>
///     In this benchmark we verify that the response has a success status code.<br />
///     FluentAssertions has no expectations on <see cref="HttpResponseMessage" />, so it verifies
///     <see cref="HttpResponseMessage.IsSuccessStatusCode" />.
/// </summary>
public partial class HappyCaseBenchmarks
{
	[Benchmark]
	public async Task SuccessStatusCode_aweXpect()
		=> await Expect.That(_response).HasStatusCode().Success();

	[Benchmark]
	public AndConstraint<BooleanAssertions> SuccessStatusCode_FluentAssertions()
		=> _response.IsSuccessStatusCode.Should().BeTrue();
}
