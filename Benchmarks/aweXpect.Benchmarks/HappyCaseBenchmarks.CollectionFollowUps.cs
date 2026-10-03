using BenchmarkDotNet.Attributes;

namespace aweXpect.Benchmarks;

/// <summary>
///     The scenarios of the collection follow-ups.<br />
/// </summary>
public partial class HappyCaseBenchmarks
{
	private readonly Dictionary<int, int> _collectionDictionary = Enumerable.Range(0, 10).ToDictionary(x => x, x => x);
	private readonly Dictionary<int, int> _collectionDictionaryCopy = Enumerable.Range(0, 10).ToDictionary(x => x, x => x);
	private readonly int[] _collectionInts = Enumerable.Range(0, 1000).ToArray();
	private readonly int[] _collectionLargeInts = Enumerable.Range(0, 100_000).ToArray();
	private readonly int[] _collectionMiddleInts = Enumerable.Range(500, 100).ToArray();
	private readonly string[] _collectionStrings = Enumerable.Range(0, 1000).Select(x => $"item-{x}").ToArray();

	[Benchmark]
	public async Task AreUniqueInts_aweXpect()
		=> await Expect.That(_collectionInts).All().AreUnique();

	[Benchmark]
	public async Task AreUniqueStrings_aweXpect()
		=> await Expect.That(_collectionStrings).All().AreUnique();

	[Benchmark]
	public async Task AreUniqueMembers_aweXpect()
		=> await Expect.That(_collectionStrings).All().AreUnique(x => int.Parse(x[5..]));

	[Benchmark]
	public async Task EndsWithLargeInts_aweXpect()
		=> await Expect.That(_collectionLargeInts).EndsWith(99_997, 99_998, 99_999);

	[Benchmark]
	public async Task ContainsRun_aweXpect()
		=> await Expect.That(_collectionInts).Contains(_collectionMiddleInts);

	[Benchmark]
	public async Task IsContainedInRun_aweXpect()
		=> await Expect.That(_collectionMiddleInts).IsContainedIn(_collectionInts);

	[Benchmark]
	public async Task DictionaryContainsKey_aweXpect()
		=> await Expect.That(_collectionDictionary).ContainsKey(1);

	[Benchmark]
	public async Task DictionaryIsEqualTo_aweXpect()
		=> await Expect.That(_collectionDictionary).IsEqualTo(_collectionDictionaryCopy);
}
