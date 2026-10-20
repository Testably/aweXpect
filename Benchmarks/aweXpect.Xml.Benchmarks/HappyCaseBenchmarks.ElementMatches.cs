using System.Xml.Linq;
using BenchmarkDotNet.Attributes;
using FluentAssertions;
using FluentAssertions.Xml;

namespace aweXpect.Xml.Benchmarks;

/// <summary>
///     In this benchmark we verify that an <see cref="XElement" /> is equivalent to the expected XML, regardless of its
///     formatting and the order of its attributes.<br />
///     FluentAssertions has no expectations against an XML <see cref="string" />, so it parses the expected XML into an
///     <see cref="XElement" />.
/// </summary>
public partial class HappyCaseBenchmarks
{
	[Benchmark]
	public async Task ElementMatches_aweXpect()
		=> await Expect.That(_xmlElement).MatchesExactly(_xmlExpectation);

	[Benchmark]
	public AndConstraint<XElementAssertions> ElementMatches_FluentAssertions()
		=> _xmlElement.Should().BeEquivalentTo(XElement.Parse(_xmlExpectation));
}
