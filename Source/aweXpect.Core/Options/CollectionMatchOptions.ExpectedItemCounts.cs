using System;
using System.Collections.Generic;

namespace aweXpect.Options;

public partial class CollectionMatchOptions
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
		/// <remarks>
		///     The items are wrapped, because a dictionary rejects a <see langword="null" /> key.
		/// </remarks>
		private readonly Dictionary<ValueTuple<TItem>, int> _counts;

		private int _remaining;

		public ExpectedItemCounts(List<TItem> expected)
		{
			_counts = new Dictionary<ValueTuple<TItem>, int>(expected.Count);
			FoundItems = new List<TItem>(expected.Count);
			_remaining = expected.Count;
			foreach (TItem item in expected)
			{
				ValueTuple<TItem> key = new(item);
				_counts.TryGetValue(key, out int count);
				_counts[key] = count + 1;
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
			ValueTuple<TItem> key = new(item);
			if (!_counts.TryGetValue(key, out int count) || count == 0)
			{
				return false;
			}

			_counts[key] = count - 1;
			_remaining--;
			FoundItems.Add(item);
			return true;
		}
	}
}
