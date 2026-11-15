using System.Xml.Linq;
using BenchmarkDotNet.Attributes;
using FluentAssertions;
using FluentAssertions.Collections;

namespace aweXpect.Xml.Benchmarks;

/// <summary>
///     In this benchmark we verify that an <see cref="XElement" /> with 10 child elements is equivalent to the expected
///     XML with the child elements in reverse order.<br />
///     FluentAssertions has no XML equivalency that ignores the element order, so it only approximates the expectation by
///     comparing the <c>bpm</c> attributes of the child elements in any order.
/// </summary>
public partial class HappyCaseBenchmarks
{
	private readonly XElement _album = XElement.Parse(
		"""
		<album>
		  <song bpm="91" /><song bpm="92" /><song bpm="93" /><song bpm="94" /><song bpm="95" />
		  <song bpm="96" /><song bpm="97" /><song bpm="98" /><song bpm="99" /><song bpm="100" />
		</album>
		""");

	private readonly string _reversedAlbum =
		"""
		<album>
		  <song bpm="100" /><song bpm="99" /><song bpm="98" /><song bpm="97" /><song bpm="96" />
		  <song bpm="95" /><song bpm="94" /><song bpm="93" /><song bpm="92" /><song bpm="91" />
		</album>
		""";

	[Benchmark]
	public async Task ElementOrder_aweXpect()
		=> await Expect.That(_album).MatchesExactly(_reversedAlbum, o => o.IgnoringElementOrder());

	[Benchmark]
	public AndConstraint<StringCollectionAssertions<IEnumerable<string>>> ElementOrder_FluentAssertions()
		=> _album.Elements("song").Select(song => song.Attribute("bpm")!.Value).Should()
			.BeEquivalentTo(XElement.Parse(_reversedAlbum).Elements("song").Select(song => song.Attribute("bpm")!.Value));
}
