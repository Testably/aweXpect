using System.Xml.Linq;
using BenchmarkDotNet.Attributes;
using FluentAssertions;
using FluentAssertions.Collections;

namespace aweXpect.Xml.Benchmarks;

/// <summary>
///     In this benchmark we verify that an <see cref="XElement" /> with 10 child elements is equivalent to the expected
///     XML with the child elements in reverse order.<br />
///     FluentAssertions has no XML equivalency that ignores the element order, so it only approximates the expectation by
///     comparing the <c>id</c> attributes of the child elements in any order.
/// </summary>
public partial class HappyCaseBenchmarks
{
	private readonly XElement _playlist = XElement.Parse(
		"""
		<playlist>
		  <track id="T1" /><track id="T2" /><track id="T3" /><track id="T4" /><track id="T5" />
		  <track id="T6" /><track id="T7" /><track id="T8" /><track id="T9" /><track id="T10" />
		</playlist>
		""");

	private readonly string _reversedPlaylist =
		"""
		<playlist>
		  <track id="T10" /><track id="T9" /><track id="T8" /><track id="T7" /><track id="T6" />
		  <track id="T5" /><track id="T4" /><track id="T3" /><track id="T2" /><track id="T1" />
		</playlist>
		""";

	[Benchmark]
	public async Task ElementOrder_aweXpect()
		=> await Expect.That(_playlist).MatchesExactly(_reversedPlaylist, o => o.IgnoringElementOrder());

	[Benchmark]
	public AndConstraint<StringCollectionAssertions<IEnumerable<string>>> ElementOrder_FluentAssertions()
		=> _playlist.Elements("track").Select(track => track.Attribute("id")!.Value).Should()
			.BeEquivalentTo(XElement.Parse(_reversedPlaylist).Elements("track").Select(track => track.Attribute("id")!.Value));
}
