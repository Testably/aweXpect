using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Toolchains.InProcess.Emit;

namespace aweXpect.Benchmarks;

/// <summary>
///     In these benchmarks we verify expectations whose failure message has contexts (e.g. "Collection:"), once when
///     they succeed and once when they fail and the message is created.
/// </summary>
[Config(typeof(Config))]
[MarkdownExporterAttribute.GitHub]
[MemoryDiagnoser]
public class ContextBenchmarks
{
	private readonly Dictionary<int, string> _dictionary = Enumerable.Range(1, 20).ToDictionary(x => x, x => $"v{x}");
	private readonly int[] _ints = Enumerable.Range(1, 20).ToArray();
	private readonly string[] _longItems = Enumerable.Range(1, 20).Select(x => new string('a', 100) + x).ToArray();
	private readonly string[] _otherLongItems = Enumerable.Range(1, 20).Select(x => new string('a', 100) + x).ToArray();
	private readonly string[] _reversedLongItems = Enumerable.Range(1, 20).Select(x => new string('a', 100) + (21 - x)).ToArray();
	private readonly string _longString = new string('a', 500) + "b";
	private readonly Person _person = new("Alice", [1, 2, 3], [4, 5, 6]);
	private readonly Person _samePerson = new("Alice", [1, 2, 3], [4, 5, 6]);

	[Benchmark]
	public Task<string?> HasCount_Success()
		=> Check(async () => await Expect.That(_ints).HasCount(20));

	[Benchmark]
	public Task<string?> HasCount_Failure()
		=> Check(async () => await Expect.That(_ints).HasCount(21));

	[Benchmark]
	public Task<string?> Contains_Success()
		=> Check(async () => await Expect.That(_ints).Contains(20));

	[Benchmark]
	public Task<string?> Contains_Failure()
		=> Check(async () => await Expect.That(_ints).Contains(21));

	[Benchmark]
	public Task<string?> AllComplyWith_Success()
		=> Check(async () => await Expect.That(_ints).All().ComplyWith(x => x.IsGreaterThan(0)));

	[Benchmark]
	public Task<string?> AllComplyWith_Failure()
		=> Check(async () => await Expect.That(_ints).All().ComplyWith(x => x.IsGreaterThan(10)));

	[Benchmark]
	public Task<string?> CollectionIsEqualTo_Success()
		=> Check(async () => await Expect.That(_longItems).IsEqualTo(_otherLongItems));

	[Benchmark]
	public Task<string?> CollectionIsEqualTo_Failure()
		=> Check(async () => await Expect.That(_longItems).IsEqualTo(_reversedLongItems));

	[Benchmark]
	public Task<string?> StringIsEqualTo_Success()
		=> Check(async () => await Expect.That(_longString).IsEqualTo(new string('a', 500) + "b"));

	[Benchmark]
	public Task<string?> StringIsEqualTo_Failure()
		=> Check(async () => await Expect.That(_longString).IsEqualTo(new string('a', 500) + "c"));

	[Benchmark]
	public Task<string?> WhoseAndWhose_Success()
		=> Check(async () => await Expect.That(_person)
			.Whose(p => p.Items, items => items.HasCount(3))
			.And.Whose(p => p.Other, other => other.Contains(5)));

	[Benchmark]
	public Task<string?> WhoseAndWhose_Failure()
		=> Check(async () => await Expect.That(_person)
			.Whose(p => p.Items, items => items.HasCount(4))
			.And.Whose(p => p.Other, other => other.Contains(7)));

	[Benchmark]
	public Task<string?> DoesNotComplyWith_Success()
		=> Check(async () => await Expect.That(_ints).DoesNotComplyWith(it => it.Contains(21)));

	[Benchmark]
	public Task<string?> DoesNotComplyWith_Failure()
		=> Check(async () => await Expect.That(_ints).DoesNotComplyWith(it => it.Contains(20)));

	[Benchmark]
	public Task<string?> DictionaryContainsKey_Success()
		=> Check(async () => await Expect.That(_dictionary).ContainsKey(20));

	[Benchmark]
	public Task<string?> DictionaryContainsKey_Failure()
		=> Check(async () => await Expect.That(_dictionary).ContainsKey(21));

	[Benchmark]
	public Task<string?> IsEquivalentTo_Success()
		=> Check(async () => await Expect.That(_person).IsEquivalentTo(_samePerson));

	[Benchmark]
	public Task<string?> IsEquivalentTo_Failure()
		=> Check(async () => await Expect.That(_person).IsEquivalentTo(_samePerson with { Name = "Bob", }));

	[Benchmark]
	public Task<string?> Or_Success()
		=> Check(async () => await Expect.That(_ints).Contains(21).Or.HasCount(20));

	[Benchmark]
	public Task<string?> Or_Failure()
		=> Check(async () => await Expect.That(_ints).Contains(21).Or.HasCount(21));

	/// <summary>
	///     Returns the failure message, or <see langword="null" /> when the expectation is met.
	/// </summary>
	private static async Task<string?> Check(Func<Task> expectation)
	{
		try
		{
			await expectation();
			return null;
		}
		catch (Exception exception)
		{
			return exception.Message;
		}
	}

	public sealed record Person(string Name, int[] Items, int[] Other);

	private sealed class Config : ManualConfig
	{
		public Config()
		{
			AddJob(Job.MediumRun
				.WithLaunchCount(1)
				.WithToolchain(InProcessEmitToolchain.Instance)
				.WithId("InProcess"));
		}
	}
}
