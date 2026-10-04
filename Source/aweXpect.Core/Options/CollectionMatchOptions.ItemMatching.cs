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
		private readonly FreeIndices _freeExpected;
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
			_freeExpected = new FreeIndices(_expected.Length);
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
			=> _freeExpected.ToList().ConvertAll(expectedIndex => _expected[expectedIndex]);

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
		///     The comparisons are only awaited from the first one that does not complete synchronously on.
		/// </remarks>
		/// <returns>The index of the assigned expected item, or <c>-1</c> when the item is not assigned yet.</returns>
		public ValueTask<int> Add(int index, TItem value, int preferredExpectedIndex = Unmatched)
		{
			if (_freeExpected.Count == 0)
			{
				_unmatchedItems.Add(index, value);
				return new ValueTask<int>(Unmatched);
			}

			Exception? unanswered = null;
			for ((int ExpectedIndex, bool IsNeighbour) candidate = FirstCandidate(preferredExpectedIndex);
			     candidate.ExpectedIndex >= 0;
			     candidate = NextCandidate(candidate, preferredExpectedIndex))
			{
				ValueTask<(bool IsMatch, Exception? Unanswered)> comparison =
					Compare(index, value, _expected[candidate.ExpectedIndex]);
				if (!comparison.IsCompletedSuccessfully)
				{
					return AddAsync(comparison, candidate, index, value, preferredExpectedIndex, unanswered);
				}

				if (TryAssign(comparison.Result, candidate, index, value, ref unanswered))
				{
					return new ValueTask<int>(candidate.ExpectedIndex);
				}
			}

			return AddPending(index, value, unanswered);
		}

		private async ValueTask<int> AddAsync(ValueTask<(bool IsMatch, Exception? Unanswered)> comparison,
			(int ExpectedIndex, bool IsNeighbour) candidate, int index, TItem value, int preferredExpectedIndex,
			Exception? unanswered)
		{
			while (!TryAssign(await comparison, candidate, index, value, ref unanswered))
			{
				candidate = NextCandidate(candidate, preferredExpectedIndex);
				if (candidate.ExpectedIndex < 0)
				{
					return await AddPending(index, value, unanswered);
				}

				comparison = Compare(index, value, _expected[candidate.ExpectedIndex]);
			}

			return candidate.ExpectedIndex;
		}

		/// <summary>
		///     Assigns the <paramref name="value" /> to the <paramref name="candidate" />, when the
		///     <paramref name="comparison" /> matched.
		/// </summary>
		/// <remarks>
		///     An unanswered comparison with a neighbour is repeated with the other free expected items, which decide
		///     about it.
		/// </remarks>
		private bool TryAssign((bool IsMatch, Exception? Unanswered) comparison,
			(int ExpectedIndex, bool IsNeighbour) candidate, int index, TItem value, ref Exception? unanswered)
		{
			if (!candidate.IsNeighbour)
			{
				unanswered ??= comparison.Unanswered;
			}

			if (!comparison.IsMatch)
			{
				return false;
			}

			Assign(index, value, candidate.ExpectedIndex);
			return true;
		}

		/// <summary>
		///     The free neighbours of the <paramref name="preferredExpectedIndex" /> come first, followed by all free
		///     expected items.
		/// </summary>
		private (int ExpectedIndex, bool IsNeighbour) FirstCandidate(int preferredExpectedIndex)
			=> preferredExpectedIndex == Unmatched
				? (_freeExpected.First, false)
				: FreeNeighbourFrom(preferredExpectedIndex + 1, preferredExpectedIndex);

		/// <returns>The next candidate, or one with a negative index when none is left.</returns>
		private (int ExpectedIndex, bool IsNeighbour) NextCandidate((int ExpectedIndex, bool IsNeighbour) candidate,
			int preferredExpectedIndex)
			=> candidate.IsNeighbour
				? FreeNeighbourFrom(candidate.ExpectedIndex - 2, preferredExpectedIndex)
				: (_freeExpected.Next(candidate.ExpectedIndex), false);

		/// <summary>
		///     The neighbour after the <paramref name="preferredExpectedIndex" /> is tried before the one before it.
		/// </summary>
		private (int ExpectedIndex, bool IsNeighbour) FreeNeighbourFrom(int expectedIndex, int preferredExpectedIndex)
		{
			for (; expectedIndex >= preferredExpectedIndex - 1; expectedIndex -= 2)
			{
				if (expectedIndex >= 0 && expectedIndex < _expected.Length &&
				    _itemOfExpected[expectedIndex] == Unmatched)
				{
					return (expectedIndex, true);
				}
			}

			return (_freeExpected.First, false);
		}

		/// <summary>
		///     Keeps the <paramref name="value" />, which matches no free expected item, as pending.
		/// </summary>
		private async ValueTask<int> AddPending(int index, TItem value, Exception? unanswered)
		{
			await ThrowIfUnansweredAndNoAssignedExpectedItemMatches(index, value, unanswered);
			_items.Add((index, value));
			_expectedOfItem.Add(Unmatched);
			_pendingItems.Add(_items.Count - 1);
			return Unmatched;
		}

		private void Assign(int index, TItem value, int expectedIndex)
		{
			_items.Add((index, value));
			_expectedOfItem.Add(expectedIndex);
			_itemOfExpected[expectedIndex] = _items.Count - 1;
			_freeExpected.Remove(expectedIndex);
			DiscardPendingItemsWhenNothingIsFree();
		}

		/// <summary>
		///     Searches an augmenting path for each pending item, or for each free expected item if there are fewer of
		///     them, as both find a maximum matching; the pending items that remain are unmatched for good.
		/// </summary>
		/// <remarks>
		///     Without pending items there is nothing to resolve, which is the case after every item of a subject whose
		///     items each match a free expected item.
		/// </remarks>
		public ValueTask ResolvePendingItems()
			=> _pendingItems.Count == 0 ? default : ResolvePendingItemsAsync();

		private async ValueTask ResolvePendingItemsAsync()
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

		/// <remarks>
		///     A comparison that completes synchronously returns without a state machine.
		/// </remarks>
		private ValueTask<(bool IsMatch, Exception? Unanswered)> Compare(int index, TItem value, TExpected expected)
		{
			ValueTask<bool> isMatch;
			try
			{
				isMatch = _isMatch(index, value, expected);
			}
			catch (Exception exception) when (IsUnanswered(exception))
			{
				return new ValueTask<(bool IsMatch, Exception? Unanswered)>((false, exception));
			}

			if (!isMatch.IsCompletedSuccessfully)
			{
				return CompareAsync(isMatch);
			}

			return new ValueTask<(bool IsMatch, Exception? Unanswered)>((isMatch.Result, null));
		}

		private static async ValueTask<(bool IsMatch, Exception? Unanswered)> CompareAsync(ValueTask<bool> isMatch)
		{
			try
			{
				return (await isMatch, null);
			}
			catch (Exception exception) when (IsUnanswered(exception))
			{
				return (false, exception);
			}
		}

		private static bool IsUnanswered(Exception exception)
			=> exception is UserCodeException or UnansweredItemException;

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
		///     The indices from <c>0</c> to a count, in ascending order, from which any index can be removed in constant
		///     time.
		/// </summary>
		/// <remarks>
		///     An item that is in or near the expected order is assigned to the first free expected item, so removing
		///     from the front of a list would shift all others for every item.
		/// </remarks>
		private sealed class FreeIndices
		{
			private readonly bool[] _isFree;
			private readonly int[] _next;
			private readonly int[] _previous;

			public FreeIndices(int count)
			{
				_isFree = new bool[count];
				_next = new int[count];
				_previous = new int[count];
				for (int i = 0; i < count; i++)
				{
					_isFree[i] = true;
					_next[i] = i + 1 < count ? i + 1 : -1;
					_previous[i] = i - 1;
				}

				First = count > 0 ? 0 : -1;
				Count = count;
			}

			public int Count { get; private set; }

			/// <summary>
			///     The lowest free index, or <c>-1</c> when none is left.
			/// </summary>
			public int First { get; private set; }

			/// <summary>
			///     The next free index after the free <paramref name="index" />, or <c>-1</c> when none is left.
			/// </summary>
			public int Next(int index) => _next[index];

			public void Remove(int index)
			{
				if (!_isFree[index])
				{
					return;
				}

				_isFree[index] = false;
				int previous = _previous[index];
				int next = _next[index];
				if (previous >= 0)
				{
					_next[previous] = next;
				}
				else
				{
					First = next;
				}

				if (next >= 0)
				{
					_previous[next] = previous;
				}

				Count--;
			}

			public List<int> ToList()
			{
				List<int> indices = new(Count);
				for (int index = First; index >= 0; index = _next[index])
				{
					indices.Add(index);
				}

				return indices;
			}
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
