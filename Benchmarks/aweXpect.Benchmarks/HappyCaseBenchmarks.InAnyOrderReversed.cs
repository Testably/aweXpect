using aweXpect.Equivalency;
using BenchmarkDotNet.Attributes;

namespace aweXpect.Benchmarks;

/// <summary>
///     In this benchmark we verify that a collection has the same items as another one in reversed order.<br />
/// </summary>
public partial class HappyCaseBenchmarks
{
	private readonly int[] _intReversedExpectation = Enumerable.Range(0, 1000).Reverse().ToArray();
	private readonly Poco _pocoLast = Poco.CreateList(1000)[999];
	private readonly List<Poco> _pocoReversedExpectation = Enumerable.Reverse(Poco.CreateList(200)).ToList();
	private readonly List<Poco> _pocoReversedSubject = Poco.CreateList(200);

	private readonly string[] _stringReversedExpectation =
		Enumerable.Range(0, 1000).Reverse().Select(x => $"item{x}").ToArray();

	private readonly string[] _stringReversedSubject = Enumerable.Range(0, 1000).Select(x => $"item{x}").ToArray();

	[Benchmark]
	public async Task IntInAnyOrderReversed_aweXpect()
		=> await Expect.That(_intArraySubject).IsEqualTo(_intReversedExpectation).InAnyOrder();

	[Benchmark]
	public async Task StringInAnyOrderReversed_aweXpect()
		=> await Expect.That(_stringReversedSubject).IsEqualTo(_stringReversedExpectation).InAnyOrder();

	[Benchmark]
	public async Task EquivalencyPocoReversed_aweXpect()
		=> await Expect.That(_pocoReversedSubject).IsEquivalentTo(_pocoReversedExpectation,
			o => o.IgnoringCollectionOrder());

	[Benchmark]
	public async Task ContainsLastEquivalent_aweXpect()
		=> await Expect.That(_poco1000Subject).Contains(_pocoLast).Equivalent();
}
