using System.Collections;
using System.Collections.Generic;

namespace aweXpect.Tests;

public partial class ThatReadOnlyDictionary
{
	/// <summary>
	///     A dictionary that implements only <see cref="IReadOnlyDictionary{TKey,TValue}" />, so that an expectation
	///     cannot reach a key through <see cref="IDictionary{TKey,TValue}" />.
	/// </summary>
	public sealed class ReadOnlyOnlyDictionary<TKey, TValue>(IDictionary<TKey, TValue> inner)
		: IReadOnlyDictionary<TKey, TValue>
	{
		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => inner.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public int Count => inner.Count;

		public bool ContainsKey(TKey key) => inner.ContainsKey(key);

		public bool TryGetValue(TKey key, out TValue value) => inner.TryGetValue(key, out value!);

		public TValue this[TKey key] => inner[key];

		public IEnumerable<TKey> Keys => inner.Keys;

		public IEnumerable<TValue> Values => inner.Values;
	}

	public static IReadOnlyDictionary<TKey, TValue> ToDictionary<TKey, TValue>(TKey[] keys, TValue[] values)
		where TKey : notnull
	{
		if (values.Length != keys.Length)
		{
			throw new ArgumentException("The number of keys and the number of values do not match.");
		}

		Dictionary<TKey, TValue> result = new();
		int index = 0;
		foreach (TValue value in values)
		{
			TKey key = keys[index++];
			result.Add(key, value);
		}

		return result;
	}

	public static IReadOnlyDictionary<int, T?> ToDictionary<T>(T[] items)
	{
		Dictionary<int, T?> result = new();
		int index = 0;
		foreach (T item in items)
		{
			result.Add(index++, item);
		}

		return result;
	}

	public static IReadOnlyDictionary<int, TMember> ToDictionary<TSource, TMember>(TSource[] items,
		Func<TSource, TMember> mapper)
	{
		Dictionary<int, TMember> result = new();
		int index = 0;
		foreach (TSource item in items)
		{
			result.Add(index++, mapper(item));
		}

		return result;
	}

	public class MyClass(int value = 0)
	{
		public InnerClass? Inner { get; set; }
		public int Value { get; set; } = value;
	}

	public class InnerClass
	{
		public IEnumerable<string>? Collection { get; set; }

		public InnerClass? Inner { get; set; }
		public string? Value { get; set; }
	}
}
