using aweXpect.Equivalency;
using BenchmarkDotNet.Attributes;

namespace aweXpect.Benchmarks;

/// <summary>
///     In this benchmark we verify that two lists of plain objects are equivalent to one another.<br />
/// </summary>
public partial class HappyCaseBenchmarks
{
	private readonly List<Poco> _poco10Expectation = Poco.CreateList(10);
	private readonly List<Poco> _poco10Subject = Poco.CreateList(10);
	private readonly List<Poco> _poco1000Expectation = Poco.CreateList(1000);
	private readonly List<Poco> _poco1000Subject = Poco.CreateList(1000);

	private readonly List<object> _pocoDerivedExpectation =
		Poco.CreateList(1000).Select(x => (object)x.ToDerived()).ToList();

	private readonly List<object> _pocoDerivedSubject =
		Poco.CreateList(1000).Select(x => (object)x.ToDerived()).ToList();

	[Benchmark]
	public async Task EquivalencyPoco10_aweXpect()
		=> await Expect.That(_poco10Subject).IsEquivalentTo(_poco10Expectation);

	[Benchmark]
	public async Task EquivalencyPoco1000_aweXpect()
		=> await Expect.That(_poco1000Subject).IsEquivalentTo(_poco1000Expectation);

	[Benchmark]
	public async Task EquivalencyPoco1000Derived_aweXpect()
		=> await Expect.That(_pocoDerivedSubject).IsEquivalentTo(_pocoDerivedExpectation);

	[Benchmark]
	public async Task EquivalencyPoco1000Internal_aweXpect()
		=> await Expect.That(_poco1000Subject).IsEquivalentTo(_poco1000Expectation,
			o => o.IncludingProperties(IncludeMembers.Public | IncludeMembers.Internal));

	[Benchmark]
	public async Task EquivalencyPoco1000Parallel_aweXpect()
		=> await Parallel.ForEachAsync(Enumerable.Range(0, 8), new ParallelOptions { MaxDegreeOfParallelism = 8, },
			async (_, _) => await Expect.That(_poco1000Subject).IsEquivalentTo(_poco1000Expectation));

	public class Poco
	{
		public int A { get; set; }
		public int B { get; set; }
		public int C { get; set; }
		public int D { get; set; }
		public int E { get; set; }
		public string? F { get; set; }
		public string? G { get; set; }
		public string? H { get; set; }
		public string? I { get; set; }
		public string? J { get; set; }

		public static List<Poco> CreateList(int count)
			=> Enumerable.Range(0, count).Select(Create<Poco>).ToList();

		public DerivedPoco ToDerived() => Create<DerivedPoco>(A);

		private static T Create<T>(int i) where T : Poco, new()
			=> new()
			{
				A = i,
				B = i + 1,
				C = i + 2,
				D = i + 3,
				E = i + 4,
				F = $"f{i}",
				G = $"g{i}",
				H = $"h{i}",
				I = $"i{i}",
				J = $"j{i}",
			};
	}

	public sealed class DerivedPoco : Poco;
}
