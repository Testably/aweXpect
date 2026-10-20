using BenchmarkDotNet.Attributes;
using FluentAssertions;
using FluentAssertions.Primitives;

namespace aweXpect.Web.Benchmarks;

/// <summary>
///     In this benchmark we verify the method and the URI of the request that led to the response.<br />
///     FluentAssertions has no expectations on <see cref="HttpResponseMessage" />, so it compares the
///     <see cref="HttpRequestMessage.Method" /> and the <see cref="HttpRequestMessage.RequestUri" /> of the
///     <see cref="HttpResponseMessage.RequestMessage" />.
/// </summary>
public partial class HappyCaseBenchmarks
{
	[Benchmark]
	public async Task RequestMessage_aweXpect()
		=> await Expect.That(_response).HasRequestMessage(request => request
			.HasMethod(HttpMethod.Get).And
			.HasRequestUri("https://music.example.com/tracks/1"));

	[Benchmark]
	public AndConstraint<ObjectAssertions> RequestMessage_FluentAssertions()
	{
		_response.RequestMessage.Should().NotBeNull();
		_response.RequestMessage!.Method.Should().Be(HttpMethod.Get);
		return _response.RequestMessage.RequestUri.Should().Be(new Uri("https://music.example.com/tracks/1"));
	}
}
