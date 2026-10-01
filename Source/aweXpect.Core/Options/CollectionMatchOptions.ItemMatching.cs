using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace aweXpect.Options;

public partial class CollectionMatchOptions
{
	/// <summary>
	///     A maximum one-to-one matching between the subject items and the expected items, which grows with each subject
	///     item.
	/// </summary>
	/// <remarks>
	///     An item can match several expected items, e.g. with a tolerance or a predicate, so assigning it to the first
	///     free one can block a valid assignment. An item without a free match is pending, until an augmenting path
	///     reassigns earlier items or proves that none exists. Only this search compares an item with expected items that
	///     are already assigned, so for an equivalence relation it only runs for items that remain unmatched.<br />
	///     An item for which no augmenting path exists never gets one later, so it is unmatched for good.
	/// </remarks>
	private sealed class ItemMatching<TItem, TExpected>
	{
		private const int Unmatched = -1;
		private const int Discarded = -2;
		private readonly TExpected[] _expected;
		private readonly List<int> _expectedOfItem = new();
		private readonly List<int> _freeExpected;
		private readonly Func<TItem, TExpected, ValueTask<bool>> _isMatch;
		private readonly int[] _itemOfExpected;
		private readonly List<(int Index, TItem Value)> _items = new();
		private readonly List<int> _pendingItems = new();
		private readonly Dictionary<int, TItem> _unmatchedItems;

		/// <param name="expected">The expected items.</param>
		/// <param name="isMatch">Compares an item with an expected item.</param>
		/// <param name="unmatchedItems">Receives the items that are unmatched for good, in the order of their index.</param>
		public ItemMatching(IEnumerable<TExpected> expected, Func<TItem, TExpected, ValueTask<bool>> isMatch,
			Dictionary<int, TItem> unmatchedItems)
		{
			_expected = expected.ToArray();
			_itemOfExpected = Enumerable.Repeat(Unmatched, _expected.Length).ToArray();
			_freeExpected = Enumerable.Range(0, _expected.Length).ToList();
			_isMatch = isMatch;
			_unmatchedItems = unmatchedItems;
		}

		/// <summary>
		///     The expected items that no item is assigned to, in their original order.
		/// </summary>
		public List<TExpected> UnmatchedExpectedItems()
			=> _freeExpected.Select(expectedIndex => _expected[expectedIndex]).ToList();

		/// <summary>
		///     Assigns the <paramref name="value" /> to the first free expected item it matches, otherwise it is pending.
		/// </summary>
		public async ValueTask Add(int index, TItem value)
		{
			for (int i = 0; i < _freeExpected.Count; i++)
			{
				int expectedIndex = _freeExpected[i];
				if (await _isMatch(value, _expected[expectedIndex]))
				{
					_items.Add((index, value));
					_expectedOfItem.Add(expectedIndex);
					_itemOfExpected[expectedIndex] = _items.Count - 1;
					_freeExpected.RemoveAt(i);
					DiscardPendingItemsWhenNothingIsFree();
					return;
				}
			}

			if (_freeExpected.Count == 0)
			{
				_unmatchedItems.Add(index, value);
				return;
			}

			_items.Add((index, value));
			_expectedOfItem.Add(Unmatched);
			_pendingItems.Add(_items.Count - 1);
		}

		/// <summary>
		///     Searches an augmenting path for each pending item, or for each free expected item if there are fewer of
		///     them, as both find a maximum matching; the pending items that remain are unmatched for good.
		/// </summary>
		public async ValueTask ResolvePendingItems()
		{
			if (_freeExpected.Count < _pendingItems.Count)
			{
				foreach (int expectedIndex in _freeExpected.ToList())
				{
					if (_pendingItems.Count > 0 && await TryAugmentFromExpected(expectedIndex))
					{
						_freeExpected.Remove(expectedIndex);
					}
				}
			}
			else
			{
				foreach (int item in _pendingItems.ToList())
				{
					if (_freeExpected.Count > 0 && await TryAugmentFromItem(item))
					{
						_pendingItems.Remove(item);
					}
				}
			}

			DiscardAllPendingItems();
		}

		private void DiscardPendingItemsWhenNothingIsFree()
		{
			if (_freeExpected.Count == 0)
			{
				DiscardAllPendingItems();
			}
		}

		private void DiscardAllPendingItems()
		{
			foreach (int item in _pendingItems)
			{
				_expectedOfItem[item] = Discarded;
				_unmatchedItems.Add(_items[item].Index, _items[item].Value);
			}

			_pendingItems.Clear();
		}

		/// <summary>
		///     Searches an alternating path from the pending <paramref name="root" /> to a free expected item and flips it.
		/// </summary>
		/// <remarks>
		///     The search is iterative, because the path can be as long as the matching. The root is not compared with the
		///     free expected items, because it was compared with all of them when it was added.
		/// </remarks>
		private async ValueTask<bool> TryAugmentFromItem(int root)
		{
			bool[] visited = new bool[_expected.Length];
			List<(int Item, int Next, int Via)> path = [(root, 0, Unmatched),];
			while (path.Count > 0)
			{
				(int item, int next, _) = path[path.Count - 1];
				int found = Unmatched;
				for (int expectedIndex = next; expectedIndex < _expected.Length; expectedIndex++)
				{
					if (visited[expectedIndex] ||
					    (path.Count == 1 && _itemOfExpected[expectedIndex] == Unmatched) ||
					    !await _isMatch(_items[item].Value, _expected[expectedIndex]))
					{
						continue;
					}

					visited[expectedIndex] = true;
					found = expectedIndex;
					break;
				}

				if (found == Unmatched)
				{
					path.RemoveAt(path.Count - 1);
					continue;
				}

				path[path.Count - 1] = (item, found + 1, found);
				if (_itemOfExpected[found] == Unmatched)
				{
					foreach ((int pathItem, _, int via) in path)
					{
						_expectedOfItem[pathItem] = via;
						_itemOfExpected[via] = pathItem;
					}

					_freeExpected.Remove(found);
					return true;
				}

				path.Add((_itemOfExpected[found], 0, Unmatched));
			}

			return false;
		}

		/// <summary>
		///     Searches an alternating path from the free <paramref name="root" /> expected item to a pending item and
		///     flips it.
		/// </summary>
		private async ValueTask<bool> TryAugmentFromExpected(int root)
		{
			bool[] visited = new bool[_items.Count];
			List<(int Expected, int Next, int Via)> path = [(root, 0, Unmatched),];
			while (path.Count > 0)
			{
				(int expectedIndex, int next, _) = path[path.Count - 1];
				int found = Unmatched;
				for (int item = next; item < _items.Count; item++)
				{
					if (visited[item] || _expectedOfItem[item] == Discarded ||
					    !await _isMatch(_items[item].Value, _expected[expectedIndex]))
					{
						continue;
					}

					visited[item] = true;
					found = item;
					break;
				}

				if (found == Unmatched)
				{
					path.RemoveAt(path.Count - 1);
					continue;
				}

				path[path.Count - 1] = (expectedIndex, found + 1, found);
				if (_expectedOfItem[found] == Unmatched)
				{
					foreach ((int pathExpected, _, int via) in path)
					{
						_itemOfExpected[pathExpected] = via;
						_expectedOfItem[via] = pathExpected;
					}

					_pendingItems.Remove(found);
					return true;
				}

				path.Add((_expectedOfItem[found], 0, Unmatched));
			}

			return false;
		}
	}
}
