using System.Web;
using BenchmarkDotNet.Attributes;
using FluentAssertions;
using FluentAssertions.Primitives;

namespace aweXpect.Web.Benchmarks;

/// <summary>
///     In this benchmark we verify that a <see cref="Uri" /> has a query parameter with the expected value.<br />
///     FluentAssertions has no expectations on <see cref="Uri" />, so it parses the query with
///     <see cref="HttpUtility.ParseQueryString(string)" /> and verifies the value of the parameter.
/// </summary>
public partial class HappyCaseBenchmarks
{
	private readonly Uri _uri = new("https://music.example.com/tracks?genre=rock&page=2&pageSize=20");

	[Benchmark]
	public async Task QueryParameter_aweXpect()
		=> await Expect.That(_uri).HasQueryParameter("page").WithValue("2");

	[Benchmark]
	public AndConstraint<StringAssertions> QueryParameter_FluentAssertions()
		=> HttpUtility.ParseQueryString(_uri.Query)["page"].Should().Be("2");
}
