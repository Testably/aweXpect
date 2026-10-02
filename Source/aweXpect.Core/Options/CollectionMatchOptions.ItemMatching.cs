using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using aweXpect.Core.Helpers;

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
	///     An item for which no augmenting path exists never gets one later, so it is unmatched for good.<br />
	///     A comparison that code of the caller did not answer (it threw, or an item expectation failed both ways) is no
	///     match. It only decides the result when the item matches no expected item at all.
	/// </remarks>
	private sealed class ItemMatching<TItem, TExpected>
	{
		private const int Unmatched = -1;
		private const int Discarded = -2;
		private readonly TExpected[] _expected;
		private readonly List<int> _expectedOfItem = new();
		private readonly List<int> _freeExpected;
		private readonly Func<int, TItem, TExpected, ValueTask<bool>> _isMatch;
		private readonly int[] _itemOfExpected;
		private readonly List<(int Index, TItem Value)> _items = new();
		private readonly List<int> _pendingItems = new();
		private readonly Dictionary<int, TItem> _unmatchedItems;

		/// <param name="expected">The expected items.</param>
		/// <param name="isMatch">Compares an item at its index with an expected item.</param>
		/// <param name="unmatchedItems">Receives the items that are unmatched for good, in the order of their index.</param>
		public ItemMatching(IEnumerable<TExpected> expected, Func<int, TItem, TExpected, ValueTask<bool>> isMatch,
			Dictionary<int, TItem> unmatchedItems)
		{
			_expected = expected.ToArray();
			_itemOfExpected = Enumerable.Repeat(Unmatched, _expected.Length).ToArray();
			_freeExpected = Enumerable.Range(0, _expected.Length).ToList();
			_isMatch = isMatch;
			_unmatchedItems = unmatchedItems;
		}

		/// <summary>
		///     Whether an item is assigned to every expected item, so that further items stay unmatched.
		/// </summary>
		public bool HasMatchedAllExpectedItems => _freeExpected.Count == 0;

		/// <summary>
		///     The expected items that no item is assigned to, in their original order.
		/// </summary>
		public List<TExpected> UnmatchedExpectedItems()
			=> _freeExpected.Select(expectedIndex => _expected[expectedIndex]).ToList();

		/// <summary>
		///     The pairs of the index of a matched item and the index of its expected item.
		/// </summary>
		public IEnumerable<(int Index, int ExpectedIndex)> MatchedPairs()
			=> _items.Select((item, i) => (item.Index, _expectedOfItem[i]))
				.Where(pair => pair.Item2 >= 0);

		/// <summary>
		///     Assigns the <paramref name="value" /> to the first free expected item it matches, otherwise it is pending.
		/// </summary>
		/// <remarks>
		///     Without a free expected item, the <paramref name="value" /> is an additional item and is not compared.
		///     The free expected items next to the <paramref name="preferredExpectedIndex" /> are tried first, so that items
		///     in or against the expected order find their match right away; any free match keeps the matching maximum.
		/// </remarks>
		/// <returns>The index of the assigned expected item, or <c>-1</c> when the item is not assigned yet.</returns>
		public async ValueTask<int> Add(int index, TItem value, int preferredExpectedIndex = Unmatched)
		{
			if (_freeExpected.Count == 0)
			{
				_unmatchedItems.Add(index, value);
				return Unmatched;
			}

			if (preferredExpectedIndex != Unmatched)
			{
				int assigned = await AssignNextTo(preferredExpectedIndex, index, value);
				if (assigned != Unmatched)
				{
					return assigned;
				}
			}

			Exception? unanswered = null;
			for (int i = 0; i < _freeExpected.Count; i++)
			{
				int expectedIndex = _freeExpected[i];
				(bool isMatch, Exception? exception) = await Compare(index, value, _expected[expectedIndex]);
				unanswered ??= exception;
				if (isMatch)
				{
					Assign(index, value, i);
					return expectedIndex;
				}
			}

			await ThrowIfUnansweredAndNoAssignedExpectedItemMatches(index, value, unanswered);
			_items.Add((index, value));
			_expectedOfItem.Add(Unmatched);
			_pendingItems.Add(_items.Count - 1);
			return Unmatched;
		}

		/// <remarks>
		///     An unanswered comparison is repeated with the other free expected items, which decide about it.
		/// </remarks>
		private async ValueTask<int> AssignNextTo(int preferredExpectedIndex, int index, TItem value)
		{
			for (int expectedIndex = preferredExpectedIndex + 1;
			     expectedIndex >= preferredExpectedIndex - 1;
			     expectedIndex -= 2)
			{
				if (expectedIndex >= 0 && expectedIndex < _expected.Length &&
				    _itemOfExpected[expectedIndex] == Unmatched &&
				    (await Compare(index, value, _expected[expectedIndex])).IsMatch)
				{
					Assign(index, value, _freeExpected.IndexOf(expectedIndex));
					return expectedIndex;
				}
			}

			return Unmatched;
		}

		private void Assign(int index, TItem value, int freeIndex)
		{
			int expectedIndex = _freeExpected[freeIndex];
			_items.Add((index, value));
			_expectedOfItem.Add(expectedIndex);
			_itemOfExpected[expectedIndex] = _items.Count - 1;
			_freeExpected.RemoveAt(freeIndex);
			DiscardPendingItemsWhenNothingIsFree();
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

		/// <summary>
		///     Compares the <paramref name="value" />, which matches no free expected item, with the expected items that are
		///     assigned to other items, and throws the first unanswered comparison, unless one of them matches.
		/// </summary>
		private async ValueTask ThrowIfUnansweredAndNoAssignedExpectedItemMatches(int index, TItem value,
			Exception? unanswered)
		{
			for (int expectedIndex = 0; expectedIndex < _expected.Length; expectedIndex++)
			{
				if (_itemOfExpected[expectedIndex] == Unmatched)
				{
					continue;
				}

				(bool isMatch, Exception? exception) = await Compare(index, value, _expected[expectedIndex]);
				if (isMatch)
				{
					return;
				}

				unanswered ??= exception;
			}

			if (unanswered is not null)
			{
				ExceptionDispatchInfo.Capture(unanswered).Throw();
			}
		}

		private async ValueTask<(bool IsMatch, Exception? Unanswered)> Compare(int index, TItem value,
			TExpected expected)
		{
			try
			{
				return (await _isMatch(index, value, expected), null);
			}
			catch (Exception exception) when (exception is UserCodeException or UnansweredItemException)
			{
				return (false, exception);
			}
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
					    !(await Compare(_items[item].Index, _items[item].Value, _expected[expectedIndex])).IsMatch)
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
					    !(await Compare(_items[item].Index, _items[item].Value, _expected[expectedIndex])).IsMatch)
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
