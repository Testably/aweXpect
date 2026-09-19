using System.Collections.Generic;
using System.Linq;
using aweXpect.Core;

namespace aweXpect;

/// <summary>
///     Expectations on <see cref="IDictionary{TKey,TValue}" />.
/// </summary>
public static partial class ThatDictionary
{
	private const string ExpectedDictionaryWasNull = "the expected dictionary was <null>";

	internal static bool ContainsValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TValue value)
		=> value is null
			? dictionary.Any(x => x.Value is null)
			: dictionary.Any(x => value.Equals(x.Value));

	/// <summary>
	///     Looks the <paramref name="key" /> up through the dictionary itself, so that its key comparer decides.
	/// </summary>
	private delegate bool ValueLookup<in TKey, TValue>(TKey key, out TValue? value);

	private static ValueLookup<TKey, TValue> GetLookup<TKey, TValue>(
		IEnumerable<KeyValuePair<TKey, TValue>> dictionary)
		=> dictionary is IDictionary<TKey, TValue> mutableDictionary
			? mutableDictionary.TryGetValue
			: ((IReadOnlyDictionary<TKey, TValue>)dictionary).TryGetValue;

	private static void AddDictionaryContext<TKey, TValue>(ExpectationBuilder expectationBuilder,
		IEnumerable<KeyValuePair<TKey, TValue>> dictionary)
	{
		if (dictionary is IDictionary<TKey, TValue> mutableDictionary)
		{
			expectationBuilder.AddCollectionContext(mutableDictionary);
		}
		else
		{
			expectationBuilder.AddCollectionContext((IReadOnlyDictionary<TKey, TValue>)dictionary);
		}
	}
}
