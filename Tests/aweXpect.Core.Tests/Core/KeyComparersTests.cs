using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Dynamic;

namespace aweXpect.Core.Tests.Core;

public sealed class KeyComparersTests
{
#if NET8_0_OR_GREATER
	[Test]
	public async Task CreateKeySet_ForAConcurrentDictionary_ShouldUseItsComparer()
	{
		ConcurrentDictionary<string, int> dictionary = new(StringComparer.OrdinalIgnoreCase);

		ISet<string> keys = KeyComparers.CreateKeySet(dictionary);

		await That(keys.Add("a") && !keys.Add("A")).IsTrue();
	}
#else
	[Test]
	public async Task CreateKeySet_ForAConcurrentDictionary_ShouldUseTheDefaultEquality()
	{
		ConcurrentDictionary<string, int> dictionary = new(StringComparer.OrdinalIgnoreCase);

		ISet<string> keys = KeyComparers.CreateKeySet(dictionary);

		await That(keys.Add("a") && keys.Add("A")).IsTrue()
			.Because("the ConcurrentDictionary of .NET Framework exposes no comparer");
	}
#endif

	[Test]
	public async Task CreateKeySet_ForACustomDictionary_ShouldUseTheDefaultEquality()
	{
		CustomDictionary dictionary = new(new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase));

		ISet<string> keys = KeyComparers.CreateKeySet(dictionary);

		await That(keys.Add("a") && keys.Add("A")).IsTrue()
			.Because("the comparer of a dictionary type that is not known cannot be read");
	}

	[Test]
	public async Task CreateKeySet_ForADictionary_ShouldUseItsComparer()
	{
		Dictionary<string, int> dictionary = new(StringComparer.OrdinalIgnoreCase);

		ISet<string> keys = KeyComparers.CreateKeySet(dictionary);

		await That(keys.Add("a") && !keys.Add("A")).IsTrue();
	}

	[Test]
	public async Task CreateKeySet_ForAFrozenDictionary_ShouldUseItsComparer()
	{
		FrozenDictionary<string, int> dictionary = new Dictionary<string, int>
		{
			["b"] = 1,
		}.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);

		ISet<string> keys = KeyComparers.CreateKeySet(dictionary);

		await That(keys.Add("a") && !keys.Add("A")).IsTrue();
	}

	[Test]
	public async Task CreateKeySet_ForAnImmutableDictionary_ShouldUseItsComparer()
	{
		ImmutableDictionary<string, int> dictionary = ImmutableDictionary.Create<string, int>(
			StringComparer.OrdinalIgnoreCase);

		ISet<string> keys = KeyComparers.CreateKeySet(dictionary);

		await That(keys.Add("a") && !keys.Add("A")).IsTrue();
	}

	[Test]
	public async Task CreateKeySet_ForAnImmutableSortedDictionary_ShouldTreatKeysThatItsComparerOrdersEquallyAsTheSame()
	{
		ImmutableSortedDictionary<string, int> dictionary = ImmutableSortedDictionary.Create<string, int>(
			StringComparer.OrdinalIgnoreCase);

		ISet<string> keys = KeyComparers.CreateKeySet(dictionary);

		await That(keys).IsExactly<SortedSet<string>>();
		await That(keys.Add("a") && !keys.Add("A")).IsTrue();
	}

	[Test]
	public async Task CreateKeySet_ForAReadOnlyDictionary_ShouldUseTheComparerOfTheWrappedDictionary()
	{
		ReadOnlyDictionary<string, int> dictionary =
			new(new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase));

		ISet<string> keys = KeyComparers.CreateKeySet(dictionary);

		await That(keys.Add("a") && !keys.Add("A")).IsTrue()
			.Because("the wrapped dictionary decides which keys are the same");
	}

	[Test]
	public async Task CreateKeySet_ForAReadOnlyDictionaryOfAnUnknownDictionary_ShouldUseTheDefaultEquality()
	{
		ReadOnlyDictionary<string, object?> dictionary = new(new ExpandoObject());

		ISet<string> keys = KeyComparers.CreateKeySet(dictionary);

		await That(keys.Add("a") && keys.Add("A")).IsTrue()
			.Because("the comparer of a wrapped dictionary type that is not known cannot be read");
	}

	[Test]
	public async Task CreateKeySet_ForASortedDictionary_ShouldTreatKeysThatItsComparerOrdersEquallyAsTheSame()
	{
		SortedDictionary<string, int> dictionary = new(StringComparer.OrdinalIgnoreCase);

		ISet<string> keys = KeyComparers.CreateKeySet(dictionary);

		await That(keys).IsExactly<SortedSet<string>>();
		await That(keys.Add("a") && !keys.Add("A")).IsTrue();
	}

	[Test]
	public async Task CreateKeySet_ForASortedList_ShouldTreatKeysThatItsComparerOrdersEquallyAsTheSame()
	{
		SortedList<string, int> dictionary = new(StringComparer.OrdinalIgnoreCase);

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
