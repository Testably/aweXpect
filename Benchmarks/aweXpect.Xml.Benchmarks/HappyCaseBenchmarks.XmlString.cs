using System.Xml.Linq;
using BenchmarkDotNet.Attributes;
using FluentAssertions;
using FluentAssertions.Xml;

namespace aweXpect.Xml.Benchmarks;

/// <summary>
///     In this benchmark we verify that an XML <see cref="string" /> is equivalent to the expected one, regardless of
///     its formatting and the order of its attributes.<br />
///     FluentAssertions has no expectations on XML strings, so it parses both into an <see cref="XDocument" />.
/// </summary>
public partial class HappyCaseBenchmarks
{
	private readonly string _xmlExpectation = """
	                                          <track artist="The Beatles" title="Let It Be">
	                                            <duration>243</duration>
	                                          </track>
	                                          """;

	private readonly string _xmlSubject =
		"""<track title="Let It Be" artist="The Beatles"><duration>243</duration></track>""";

	[Benchmark]
	public async Task XmlString_aweXpect()
		=> await Expect.That(_xmlSubject).IsEqualTo(_xmlExpectation).AsXml();

	[Benchmark]
	public AndConstraint<XDocumentAssertions> XmlString_FluentAssertions()
		=> XDocument.Parse(_xmlSubject).Should().BeEquivalentTo(XDocument.Parse(_xmlExpectation));
}
