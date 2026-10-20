using System.Xml.Linq;
using BenchmarkDotNet.Attributes;
using FluentAssertions;
using FluentAssertions.Xml;

namespace aweXpect.Xml.Benchmarks;

/// <summary>
///     In this benchmark we verify that an <see cref="XElement" /> has an attribute with the expected value.
/// </summary>
public partial class HappyCaseBenchmarks
{
	private readonly XElement _xmlElement =
		XElement.Parse("""<track title="Let It Be" artist="The Beatles"><duration>243</duration></track>""");

	[Benchmark]
	public async Task Attribute_aweXpect()
		=> await Expect.That(_xmlElement).HasAttribute("title", it => it.IsEqualTo("Let It Be"));

	[Benchmark]
	public AndConstraint<XElementAssertions> Attribute_FluentAssertions()
		=> _xmlElement.Should().HaveAttributeWithValue("title", "Let It Be");
}
