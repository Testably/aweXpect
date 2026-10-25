using System;
using System.Collections.Generic;
#if NET8_0_OR_GREATER
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
#endif

namespace aweXpect.Options;

public readonly partial struct CollectionMatchOptions
{
	/// <summary>
	///     Counts how often each expected item is still to be found, for items that are compared like
	///     <see cref="EqualityComparer{T}.Default" /> compares them.
	/// </summary>
	/// <remarks>
	///     An item that is found among the expected items is equal to a distinct expected item, which is what matching
	///     the items in any order searches, without comparing the items with each other.<br />
	///     The found items are kept in their order, so that they can still be matched once an item is not found.
	/// </remarks>
	private sealed class ExpectedItemCounts<TItem>
	{
		private const int MaximumSearchedCount = 8;

		/// <remarks>
		///     The items are wrapped, because a dictionary rejects a <see langword="null" /> key.
		/// </remarks>
		private readonly Dictionary<ValueTuple<TItem>, int>? _counts;

		private readonly TItem[] _expected;
		private readonly bool[]? _isFound;

		private int _remaining;

		/// <remarks>
		///     Few expected items are searched one by one, which is faster than hashing them.
		/// </remarks>
		public ExpectedItemCounts(TItem[] expected)
		{
			_expected = expected;
			FoundItems = new List<TItem>(expected.Length);
			_remaining = expected.Length;
			if (expected.Length <= MaximumSearchedCount)
			{
				_isFound = new bool[expected.Length];
				return;
			}

			_counts = new Dictionary<ValueTuple<TItem>, int>(expected.Length);
			foreach (TItem item in expected)
			{
#if NET8_0_OR_GREATER
				CollectionsMarshal.GetValueRefOrAddDefault(_counts, new ValueTuple<TItem>(item), out _)++;
#else
				ValueTuple<TItem> key = new(item);
				_counts.TryGetValue(key, out int count);
				_counts[key] = count + 1;
#endif
			}
		}

		/// <summary>
		///     The items that were found, in the order in which they were searched.
		/// </summary>
		public List<TItem> FoundItems { get; }

		/// <summary>
		///     Whether every expected item was found.
		/// </summary>
		public bool HasFoundAll => _remaining == 0;

		/// <summary>
		///     Finds the <paramref name="item" /> among the expected items that were not found yet.
		/// </summary>
		public bool TryFind(TItem item)
		{
			if (!(_isFound is null ? TryCount(item) : TrySearch(item, _isFound)))
			{
				return false;
			}

			_remaining--;
			FoundItems.Add(item);
			return true;
		}

		private bool TrySearch(TItem item, bool[] isFound)
		{
			for (int i = 0; i < _expected.Length; i++)
			{
				if (!isFound[i] && EqualityComparer<TItem>.Default.Equals(_expected[i], item))
				{
					isFound[i] = true;
					return true;
				}
			}

			return false;
		}

		private bool TryCount(TItem item)
		{
#if NET8_0_OR_GREATER
			ref int count = ref CollectionsMarshal.GetValueRefOrNullRef(_counts!, new ValueTuple<TItem>(item));
			if (Unsafe.IsNullRef(ref count) || count == 0)
			{
				return false;
			}

			count--;
#else
			ValueTuple<TItem> key = new(item);
			if (!_counts!.TryGetValue(key, out int count) || count == 0)
			{
				return false;
			}

			_counts[key] = count - 1;
#endif
			return true;
		}
	}
}
