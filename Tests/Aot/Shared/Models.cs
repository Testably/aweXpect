using System;
using System.Collections;
using System.Collections.Generic;

namespace aweXpect.Aot;

public sealed class Order
{
	public int Id { get; set; }
	public Customer Customer { get; set; } = new();
	public List<Item> Items { get; set; } = [];
	public Dictionary<string, int> Tags { get; set; } = [];
}

public sealed class Customer
{
	public string Name { get; set; } = "";
	public Address Address { get; set; } = new();
}

public sealed class Address
{
	public string City { get; set; } = "";
}

public sealed class Item
{
#pragma warning disable S1104 // A public field is registered and compared differently from a property
	public string Sku = "";
#pragma warning restore S1104
	public decimal Price { get; set; }
}

/// <summary>
///     Only ever reaches a comparison declared as <see langword="object" />, so the generator never sees it.
/// </summary>
public sealed class Hidden
{
	public string Secret { get; set; } = "";
}

/// <summary>
///     Implements <see cref="IReadOnlyDictionary{TKey,TValue}" /> without <see cref="IDictionary" />, so a comparison
///     only recognizes it as a dictionary through the reflective interface walk.
/// </summary>
public sealed class ReadOnlyTags(Dictionary<string, int> entries) : IReadOnlyDictionary<string, int>
{
	public int this[string key] => entries[key];

	public IEnumerable<string> Keys => entries.Keys;

	public IEnumerable<int> Values => entries.Values;

	public int Count => entries.Count;

	public bool ContainsKey(string key) => entries.ContainsKey(key);

	public bool TryGetValue(string key, out int value) => entries.TryGetValue(key, out value);

	public IEnumerator<KeyValuePair<string, int>> GetEnumerator() => entries.GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public sealed class Publisher
{
	public delegate void CountedHandler(int count, string name, bool flag, DateTime at, int? optional);

	public event EventHandler? Changed;
	public event CountedHandler? Counted;
	public event Action<int>? Ticked;

	public void RaiseChanged() => Changed?.Invoke(this, EventArgs.Empty);

	public void RaiseCounted(int count) => Counted?.Invoke(count, "name", true, DateTime.MinValue, null);

	public void RaiseTicked(int value) => Ticked?.Invoke(value);
}

public interface IPublisher
{
	event EventHandler? Changed;

	void RaiseChanged();
}

/// <summary>
///     Only ever recorded through <see cref="IPublisher" />, so the generator never sees the runtime type.
/// </summary>
public sealed class HiddenPublisher : IPublisher
{
	public event EventHandler? Changed;

	public void RaiseChanged() => Changed?.Invoke(this, EventArgs.Empty);
}
