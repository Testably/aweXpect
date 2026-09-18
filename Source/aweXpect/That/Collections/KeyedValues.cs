using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using aweXpect.Helpers;

namespace aweXpect;

/// <summary>
///     The values of key-value pairs, which are formatted together with their keys in a context.
/// </summary>
internal sealed class KeyedValues<TKey, TValue> : ReadOnlyCollection<TValue>, IKeyedCollection
{
	private readonly KeyValuePair<TKey, TValue>[] _pairs;

	public KeyedValues(IEnumerable<KeyValuePair<TKey, TValue>> pairs) : this(pairs.ToArray())
	{
	}

	private KeyedValues(KeyValuePair<TKey, TValue>[] pairs) : base(pairs.Select(pair => pair.Value).ToList())
	{
		_pairs = pairs;
	}

	/// <inheritdoc cref="IKeyedCollection.Format()" />
	public string Format()
		=> Format(_pairs);

	/// <inheritdoc cref="IKeyedCollection.Format(IEnumerable{int})" />
	public string Format(IEnumerable<int> indices)
		=> Format(indices.Select(index => _pairs[index]).ToArray());

	private static string Format(KeyValuePair<TKey, TValue>[] items)
		=> Formatter.Format(items, typeof(TValue).GetFormattingOption(items.Length));
}
