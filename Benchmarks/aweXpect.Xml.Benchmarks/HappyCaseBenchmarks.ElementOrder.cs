using System.Xml.Linq;
using BenchmarkDotNet.Attributes;
using FluentAssertions;
using FluentAssertions.Collections;

namespace aweXpect.Xml.Benchmarks;

/// <summary>
///     In this benchmark we verify that an <see cref="XElement" /> with 10 child elements is equivalent to the expected
///     XML with the child elements in reverse order.<br />
///     FluentAssertions has no XML equivalency that ignores the element order, so it only approximates the expectation by
///     comparing the <c>sku</c> attributes of the child elements in any order.
/// </summary>
public partial class HappyCaseBenchmarks
{
	private readonly XElement _order = XElement.Parse(
		"""
		<order>
		  <item sku="A1" /><item sku="B2" /><item sku="C3" /><item sku="D4" /><item sku="E5" />
		  <item sku="F6" /><item sku="G7" /><item sku="H8" /><item sku="I9" /><item sku="J10" />
		</order>
		""");

	private readonly string _reversedOrder =
		"""
		<order>
		  <item sku="J10" /><item sku="I9" /><item sku="H8" /><item sku="G7" /><item sku="F6" />
		  <item sku="E5" /><item sku="D4" /><item sku="C3" /><item sku="B2" /><item sku="A1" />
		</order>
		""";

	[Benchmark]
	public async Task ElementOrder_aweXpect()
		=> await Expect.That(_order).MatchesExactly(_reversedOrder, o => o.IgnoringElementOrder());

	[Benchmark]
	public AndConstraint<StringCollectionAssertions<IEnumerable<string>>> ElementOrder_FluentAssertions()
		=> _order.Elements("item").Select(item => item.Attribute("sku")!.Value).Should()
			.BeEquivalentTo(XElement.Parse(_reversedOrder).Elements("item").Select(item => item.Attribute("sku")!.Value));
}
