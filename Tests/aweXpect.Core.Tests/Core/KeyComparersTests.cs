using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace aweXpect.Core.Tests.Core;

public sealed class KeyComparersTests
{
	[Fact]
	public async Task CreateKeySet_ForACustomDictionary_ShouldUseTheDefaultEquality()
	{
		CustomDictionary dictionary = new(new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase));

		ISet<string> keys = KeyComparers.CreateKeySet(dictionary);

		await That(keys.Add("a") && keys.Add("A")).IsTrue()
			.Because("the comparer of a dictionary type that is not known cannot be read");
	}

	[Fact]
	public async Task CreateKeySet_ForADictionary_ShouldUseItsComparer()
	{
		Dictionary<string, int> dictionary = new(StringComparer.OrdinalIgnoreCase);

		ISet<string> keys = KeyComparers.CreateKeySet(dictionary);

		await That(keys.Add("a") && !keys.Add("A")).IsTrue();
	}

	[Fact]
	public async Task CreateKeySet_ForAReadOnlyDictionary_ShouldUseTheComparerOfTheWrappedDictionary()
	{
		ReadOnlyDictionary<string, int> dictionary =
			new(new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase));

		ISet<string> keys = KeyComparers.CreateKeySet(dictionary);

		await That(keys.Add("a") && !keys.Add("A")).IsTrue()
			.Because("the wrapped dictionary decides which keys are the same");
	}

	[Fact]
	public async Task CreateKeySet_ForASortedDictionary_ShouldTreatKeysThatItsComparerOrdersEquallyAsTheSame()
	{
		SortedDictionary<string, int> dictionary = new(StringComparer.OrdinalIgnoreCase);

		ISet<string> keys = KeyComparers.CreateKeySet(dictionary);

		await That(keys).IsExactly<SortedSet<string>>();
		await That(keys.Add("a") && !keys.Add("A")).IsTrue();
	}

	private sealed class CustomDictionary(Dictionary<string, int> inner) : IReadOnlyDictionary<string, int>
	{
		public IEqualityComparer<string> Comparer => inner.Comparer;

		public int this[string key] => inner[key];

		public IEnumerable<string> Keys => inner.Keys;

		public IEnumerable<int> Values => inner.Values;

		public int Count => inner.Count;

		public bool ContainsKey(string key) => inner.ContainsKey(key);

		public bool TryGetValue(string key, out int value) => inner.TryGetValue(key, out value);

		public IEnumerator<KeyValuePair<string, int>> GetEnumerator() => inner.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
