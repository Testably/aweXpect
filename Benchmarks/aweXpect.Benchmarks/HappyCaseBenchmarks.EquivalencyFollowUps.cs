using aweXpect.Equivalency;
using BenchmarkDotNet.Attributes;

namespace aweXpect.Benchmarks;

/// <summary>
///     The scenarios of the equivalency follow-ups.<br />
/// </summary>
public partial class HappyCaseBenchmarks
{
	private readonly List<EquivalencyPoco> _equivalencyPocoExpected = EquivalencyPoco.CreateList(1000);
	private readonly List<EquivalencyPoco> _equivalencyPocoSubject = EquivalencyPoco.CreateList(1000);

	private readonly List<object> _equivalencyDerivedExpected =
		EquivalencyPoco.CreateList(1000).Select(x => (object)x.ToDerived()).ToList();

	private readonly List<object> _equivalencyDerivedSubject =
		EquivalencyPoco.CreateList(1000).Select(x => (object)x.ToDerived()).ToList();

	private readonly List<WithDictionary> _equivalencyDictionariesExpected = WithDictionary.CreateList(1000);
	private readonly List<WithDictionary> _equivalencyDictionariesSubject = WithDictionary.CreateList(1000);

	[Benchmark]
	public async Task EquivalencyRegisteredPocos_aweXpect()
		=> await Expect.That(_equivalencyPocoSubject).IsEquivalentTo(_equivalencyPocoExpected);

	[Benchmark]
	public async Task EquivalencyDerivedPocos_aweXpect()
		=> await Expect.That(_equivalencyDerivedSubject).IsEquivalentTo(_equivalencyDerivedExpected);

	[Benchmark]
	public async Task EquivalencyInternalPocos_aweXpect()
		=> await Expect.That(_equivalencyPocoSubject).IsEquivalentTo(_equivalencyPocoExpected,
			o => o.IncludingProperties(IncludeMembers.Public | IncludeMembers.Internal));

	[Benchmark]
	public async Task EquivalencyDictionaries_aweXpect()
		=> await Expect.That(_equivalencyDictionariesSubject).IsEquivalentTo(_equivalencyDictionariesExpected);

	public class EquivalencyPoco
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

		public static List<EquivalencyPoco> CreateList(int count)
			=> Enumerable.Range(0, count).Select(Create<EquivalencyPoco>).ToList();

		public DerivedEquivalencyPoco ToDerived() => Create<DerivedEquivalencyPoco>(A);

		private static T Create<T>(int i) where T : EquivalencyPoco, new()
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

	public sealed class DerivedEquivalencyPoco : EquivalencyPoco;

	public sealed class WithDictionary
	{
		public int Id { get; set; }
		public Dictionary<string, int> Values { get; set; } = new();

		public static List<WithDictionary> CreateList(int count)
			=> Enumerable.Range(0, count).Select(i => new WithDictionary
			{
				Id = i,
				Values = new Dictionary<string, int>
				{
					["a"] = i,
					["b"] = i + 1,
				},
			}).ToList();
	}
}
