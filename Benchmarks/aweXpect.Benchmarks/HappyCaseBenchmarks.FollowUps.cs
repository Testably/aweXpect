using aweXpect.Equivalency;
using BenchmarkDotNet.Attributes;

namespace aweXpect.Benchmarks;

/// <summary>
///     The scenarios of the performance follow-ups.<br />
/// </summary>
public partial class HappyCaseBenchmarks
{
	private readonly int[] _followUpInts = Enumerable.Range(0, 1000).ToArray();
	private readonly int[] _followUpIntsCopy = Enumerable.Range(0, 1000).ToArray();
	private readonly int[] _followUpLargeInts = Enumerable.Range(0, 10_000).ToArray();
	private readonly int[] _followUpLargeIntsCopy = Enumerable.Range(0, 10_000).ToArray();
	private readonly List<Poco> _followUpPocosActual = Poco.CreateList(200);

	private readonly List<Poco> _followUpPocosExpected =
		Poco.CreateList(200).Select(x => { x.J = "other"; return x; }).ToList();

	private readonly string _followUpLongString = string.Concat(Enumerable.Repeat("abcdefghij", 10_000)) + "needle";
	private readonly string[] _followUpStrings = Enumerable.Range(0, 1000).Select(x => $"item-{x}").ToArray();

	[Benchmark]
	public async Task EquivalencyIntsIgnoringOrder_aweXpect()
		=> await Expect.That(_followUpInts).IsEquivalentTo(_followUpIntsCopy, o => o.IgnoringCollectionOrder());

	[Benchmark]
	public async Task<string> EquivalencyIgnoringOrderFailure_aweXpect()
	{
		try
		{
			await Expect.That(_followUpPocosActual).IsEquivalentTo(_followUpPocosExpected,
				o => o.IgnoringCollectionOrder());
			return "";
		}
		catch (Exception exception)
		{
			return exception.Message;
		}
	}

	[Benchmark]
	public async Task WildcardItems_aweXpect()
		=> await Expect.That(_followUpStrings).All().AreEqualTo("item-*").AsWildcard();

	[Benchmark]
	public async Task RegexItems_aweXpect()
		=> await Expect.That(_followUpStrings).All().AreEqualTo("item-\\d+").AsRegex();

	[Benchmark]
	public async Task StringContains_aweXpect()
		=> await Expect.That(_followUpLongString).Contains("needle");

	[Benchmark]
	public async Task LargeIntsInAnyOrder_aweXpect()
		=> await Expect.That(_followUpLargeInts).IsEqualTo(_followUpLargeIntsCopy).InAnyOrder();

	[Benchmark]
	public async Task AndChain_aweXpect()
		=> await Expect.That(_intSubject).IsGreaterThan(1).And.IsLessThan(99);

	[Benchmark]
	public async Task AsyncEnumerableSatisfy_aweXpect()
		=> await Expect.That(AsyncRange(1000)).All().Satisfy(x => x >= 0);

	private static async IAsyncEnumerable<int> AsyncRange(int count)
	{
		for (int i = 0; i < count; i++)
		{
			yield return i;
		}

		await Task.CompletedTask;
	}
}
