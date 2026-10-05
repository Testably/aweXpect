using System.Collections;
using System.Collections.Generic;

namespace aweXpect.Tests;

/// <summary>
///     A key comparer that throws the <paramref name="exception" /> once it <see cref="IsArmed" />, so that a
///     dictionary can be filled before its lookups throw.
/// </summary>
internal sealed class ThrowingKeyComparer<TKey>(Exception exception) : IEqualityComparer<TKey>
{
	public bool IsArmed { get; set; }

	public bool Equals(TKey? x, TKey? y)
		=> IsArmed ? throw exception : EqualityComparer<TKey?>.Default.Equals(x, y);

	public int GetHashCode(TKey obj)
		=> IsArmed ? throw exception : EqualityComparer<TKey>.Default.GetHashCode(obj!);
}

/// <summary>
///     The members of a <see cref="ThrowingDictionary{TKey,TValue}" /> that throw.
/// </summary>
[Flags]
internal enum ThrowingMembers
{
	Enumeration = 1,
	TryGetValue = 2,
	ContainsKey = 4,
	Count = 8,
}

/// <summary>
///     A dictionary whose <paramref name="throwingMembers" /> throw the <paramref name="exception" /> when they are
///     accessed through one of the dictionary interfaces.
/// </summary>
internal sealed class ThrowingDictionary<TKey, TValue>(Exception exception, ThrowingMembers throwingMembers)
	: Dictionary<TKey, TValue>, IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>
	where TKey : notnull
{
	int ICollection<KeyValuePair<TKey, TValue>>.Count
		=> Unless(ThrowingMembers.Count).Count;

	bool IDictionary<TKey, TValue>.ContainsKey(TKey key)
		=> Unless(ThrowingMembers.ContainsKey).ContainsKey(key);

	bool IDictionary<TKey, TValue>.TryGetValue(TKey key, out TValue value)
		=> Unless(ThrowingMembers.TryGetValue).TryGetValue(key, out value!);

	IEnumerator IEnumerable.GetEnumerator()
		=> Unless(ThrowingMembers.Enumeration).GetEnumerator();

	IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
		=> Unless(ThrowingMembers.Enumeration).GetEnumerator();

	int IReadOnlyCollection<KeyValuePair<TKey, TValue>>.Count
		=> Unless(ThrowingMembers.Count).Count;

	bool IReadOnlyDictionary<TKey, TValue>.ContainsKey(TKey key)
		=> Unless(ThrowingMembers.ContainsKey).ContainsKey(key);

	bool IReadOnlyDictionary<TKey, TValue>.TryGetValue(TKey key, out TValue value)
		=> Unless(ThrowingMembers.TryGetValue).TryGetValue(key, out value!);

	private ThrowingDictionary<TKey, TValue> Unless(ThrowingMembers member)
		=> (throwingMembers & member) != 0 ? throw exception : this;
}
