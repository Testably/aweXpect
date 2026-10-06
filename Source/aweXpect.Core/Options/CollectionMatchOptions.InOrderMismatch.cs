using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace aweXpect.Options;

public partial class CollectionMatchOptions
{
	/// <summary>
	///     Searches the first index that matches, starting next to <paramref name="start" /> and moving away in both
	///     directions, as items in or against the expected order find their match next to the previous one.
	/// </summary>
	/// <returns>The matching index, or <c>-1</c> when none matches.</returns>
	private static async ValueTask<int> FindNear(int start, int count, Func<int, ValueTask<bool>> isMatch)
	{
		for (int distance = 0;; distance++)
		{
			int after = start + 1 + distance;
			int before = start - distance;
			if (after >= count && before < 0)
			{
				return -1;
			}

			if (after < count && await isMatch(after))
			{
				return after;
			}

			if (before >= 0 && before < count && await isMatch(before))
			{
				return before;
			}
		}
	}

	/// <summary>
	///     Explains why the sought items do not appear in order among the searched items: the items that are left over
	///     regardless of the order, the found items that are in the wrong order, and for a run the items that interrupt
	///     it.
	/// </summary>
	/// <remarks>
	///     Both collections are given as ids like for <see cref="DistinctItemsInOrder" />; without ignoring duplicates,
	///     each position is its own id. The order is judged on the assignment of the matching that ignores the order:
	///     the items in the wrong order are those outside a longest chain of assigned items in order, so a moved item is
	///     reported once instead of every item it passed.
	/// </remarks>
	private sealed class InOrderMismatch
	{
		private const int None = -1;
		private readonly int[] _assignedSearchedId;
		private readonly Func<int, int, ValueTask<bool>> _isMatch;
		private readonly bool _isOneToOne;
		private readonly bool[] _isSearchedMatched;
		private readonly List<int>[] _occurrences;
		private readonly OrderMatch _orderMatch;
		private readonly int[] _searched;
		private readonly int _soughtCount;

		private InOrderMismatch(int[] searched, int searchedCount, int soughtCount,
			Func<int, int, ValueTask<bool>> isMatch, bool isOneToOne, OrderMatch orderMatch)
		{
			_searched = searched;
			_soughtCount = soughtCount;
			_isMatch = isMatch;
			_isOneToOne = isOneToOne;
			_orderMatch = orderMatch;
			_assignedSearchedId = Enumerable.Repeat(None, soughtCount).ToArray();
			_isSearchedMatched = new bool[searchedCount];
			_occurrences = new List<int>[searchedCount];
			for (int searchedId = 0; searchedId < searchedCount; searchedId++)
			{
				_occurrences[searchedId] = new List<int>();
			}

			for (int position = 0; position < searched.Length; position++)
			{
				_occurrences[searched[position]].Add(position);
			}
		}

		/// <summary>
		///     The sought items that no searched item is assigned to, in their order.
		/// </summary>
		public List<int> UnmatchedSoughtIds { get; } = new();

		/// <summary>
		///     The first position of each searched item that is assigned to no sought item, in their order.
		/// </summary>
		public List<int> UnmatchedSearchedPositions { get; } = new();

		/// <summary>
		///     The sought items that are found, but not in order, each with the first position of its searched item.
		/// </summary>
		public List<(int SoughtId, int Position)> OutOfOrder { get; } = new();

		/// <summary>
		///     For a run, the positions of the searched items that interrupt it, each with the sought item behind it.
		/// </summary>
		public List<(int Position, int SoughtId)> Interruptions { get; } = new();

		/// <param name="searched">The id of each searched item.</param>
		/// <param name="searchedCount">The number of distinct searched items.</param>
		/// <param name="soughtCount">The number of distinct sought items.</param>
		/// <param name="isMatch">Compares a searched with a sought item.</param>
		/// <param name="isOneToOne">
		///     Each sought item needs its own searched item; otherwise the sought items form a set, so items that match the
		///     same sought item are duplicates.
		/// </param>
		/// <param name="orderMatch">How the sought items have to appear among the searched ones.</param>
		public static async ValueTask<InOrderMismatch> Explain(int[] searched, int searchedCount, int soughtCount,
			Func<int, int, ValueTask<bool>> isMatch, bool isOneToOne, OrderMatch orderMatch)
		{
			InOrderMismatch mismatch = new(searched, searchedCount, soughtCount, isMatch, isOneToOne, orderMatch);
			await mismatch.Explain();
			return mismatch;
		}

		private async ValueTask Explain()
		{
			if (_isOneToOne)
			{
				await AssignOneToOne();
			}
			else
			{
				await AssignAsSet();
			}

			List<(int SoughtId, int Position)> chain = FindTheLongestChainInOrder();
			if (_orderMatch == OrderMatch.Contiguous && chain.Count > 0 &&
			    UnmatchedSoughtIds.Count == 0 && OutOfOrder.Count == 0)
			{
				await FindTheInterruptions(chain);
			}

			for (int searchedId = 0; searchedId < _isSearchedMatched.Length; searchedId++)
			{
				if (!_isSearchedMatched[searchedId])
				{
					UnmatchedSearchedPositions.Add(_occurrences[searchedId][0]);
				}
			}
		}

		/// <summary>
		///     A maximum one-to-one matching, in which each searched item first tries the sought items next to the one
		///     the previous searched item got.
		/// </summary>
		private async ValueTask AssignOneToOne()
		{
			ItemMatching<int, int> matching = new(Enumerable.Range(0, _soughtCount),
				(_, searchedId, soughtId) => _isMatch(searchedId, soughtId),
				new Dictionary<int, int>());
			int previous = None;
			for (int searchedId = 0; searchedId < _isSearchedMatched.Length; searchedId++)
			{
				int soughtId = await matching.Add(searchedId, searchedId, previous);
				if (soughtId != None)
				{
					previous = soughtId;
				}
			}

			await matching.ResolvePendingItems();
			foreach ((int searchedId, int soughtId) in matching.MatchedPairs())
			{
				_assignedSearchedId[soughtId] = searchedId;
				_isSearchedMatched[searchedId] = true;
			}

			UnmatchedSoughtIds.AddRange(Enumerable.Range(0, _soughtCount).Where(id => _assignedSearchedId[id] == None));
		}

		/// <summary>
		///     Each sought item is assigned a matching searched item, searched next to the previous one, and every
		///     searched item that matches any sought item is matched.
		/// </summary>
		private async ValueTask AssignAsSet()
		{
			int previous = None;
			int[] assignedSoughtId = Enumerable.Repeat(None, _isSearchedMatched.Length).ToArray();
			for (int soughtId = 0; soughtId < _soughtCount; soughtId++)
			{
				int sought = soughtId;
				int searchedId = await FindNear(previous, _isSearchedMatched.Length,
					id => _isMatch(id, sought));
				if (searchedId == None)
				{
					UnmatchedSoughtIds.Add(soughtId);
					continue;
				}

				_assignedSearchedId[soughtId] = searchedId;
				_isSearchedMatched[searchedId] = true;
				if (assignedSoughtId[searchedId] == None)
				{
					assignedSoughtId[searchedId] = soughtId;
				}

				previous = searchedId;
			}

			int nearSoughtId = None;
			for (int searchedId = 0; searchedId < _isSearchedMatched.Length; searchedId++)
			{
				int searched = searchedId;
				if (assignedSoughtId[searchedId] != None)
				{
					nearSoughtId = assignedSoughtId[searchedId];
				}
				else if (await FindNear(nearSoughtId, _soughtCount, id => _isMatch(searched, id)) != None)
				{
					_isSearchedMatched[searchedId] = true;
				}
			}
		}

		/// <summary>
		///     A longest increasing subsequence over the positions of the assigned searched items, in which each sought
		///     item may use any occurrence of its searched item.
		/// </summary>
		/// <remarks>
		///     Sought items that share their searched item are duplicates of each other, so only the first one is placed.
		/// </remarks>
		private List<(int SoughtId, int Position)> FindTheLongestChainInOrder()
		{
			List<(int SoughtId, int Position, int Previous)> entries = new();
			List<int> tails = new();
			HashSet<int> placedSearchedIds = new();
			List<int> placedSoughtIds = new();
			for (int soughtId = 0; soughtId < _soughtCount; soughtId++)
			{
				int searchedId = _assignedSearchedId[soughtId];
				if (searchedId == None || !placedSearchedIds.Add(searchedId))
				{
					continue;
				}

				placedSoughtIds.Add(soughtId);
				ExtendTheChains(soughtId, _occurrences[searchedId], entries, tails);
			}

			List<(int SoughtId, int Position)> chain = new();
			for (int entry = tails.Count > 0 ? tails[tails.Count - 1] : None;
			     entry != None;
			     entry = entries[entry].Previous)
			{
				chain.Add((entries[entry].SoughtId, entries[entry].Position));
			}

			chain.Reverse();
			HashSet<int> chainedSoughtIds = new(chain.Select(link => link.SoughtId));
			OutOfOrder.AddRange(placedSoughtIds
				.Where(soughtId => !chainedSoughtIds.Contains(soughtId))
				.Select(soughtId => (soughtId, _occurrences[_assignedSearchedId[soughtId]][0])));
			return chain;
		}

		/// <summary>
		///     Each chain of a given length keeps the entry with the smallest last position, and an occurrence extends the
		///     longest chain that ends before it.
		/// </summary>
		/// <remarks>
		///     The occurrences are added in descending order, so that two occurrences of the same item never extend each
		///     other.
		/// </remarks>
		private static void ExtendTheChains(int soughtId, List<int> occurrences,
			List<(int SoughtId, int Position, int Previous)> entries, List<int> tails)
		{
			for (int i = occurrences.Count - 1; i >= 0; i--)
			{
				int position = occurrences[i];
				int length = FindTheFirstTailNotBefore(tails, entries, position);
				entries.Add((soughtId, position, length > 0 ? tails[length - 1] : None));
				if (length == tails.Count)
				{
					tails.Add(entries.Count - 1);
				}
				else
				{
					tails[length] = entries.Count - 1;
				}
			}
		}

		private static int FindTheFirstTailNotBefore(List<int> tails,
			List<(int SoughtId, int Position, int Previous)> entries, int position)
		{
			int low = 0;
			int high = tails.Count;
			while (low < high)
			{
				int middle = (low + high) / 2;
				if (entries[tails[middle]].Position < position)
				{
					low = middle + 1;
				}
				else
				{
					high = middle;
				}
			}

			return low;
		}

		/// <summary>
		///     Every other item within the run must be a duplicate, like for <see cref="DistinctItemsInOrder" />: another
		///     occurrence of a placed item (for a set, of any item that matches a sought item), or an item that also
		///     occurs outside the run.
		/// </summary>
		private async ValueTask FindTheInterruptions(List<(int SoughtId, int Position)> chain)
		{
			await MoveTowardsTheEnd(chain);
			int start = chain[0].Position;
			int end = chain[chain.Count - 1].Position;
			HashSet<int> chainPositions = new(chain.Select(link => link.Position));
			HashSet<int> placedSearchedIds = new(chain.Select(link => _searched[link.Position]));
			int next = 0;
			for (int position = start + 1; position < end; position++)
			{
				while (chain[next].Position < position)
				{
					next++;
				}

				int searchedId = _searched[position];
				bool isDuplicate = _isOneToOne
					? placedSearchedIds.Contains(searchedId)
					: _isSearchedMatched[searchedId];
				if (!chainPositions.Contains(position) && !isDuplicate && !IsOutside(searchedId, start, end))
				{
					Interruptions.Add((position, chain[next].SoughtId));
				}
			}
		}

		/// <summary>
		///     The chain ends as early as possible, so moving each earlier item as close as possible to the next one
		///     leaves the shortest run that ends there.
		/// </summary>
		private async ValueTask MoveTowardsTheEnd(List<(int SoughtId, int Position)> chain)
		{
			for (int i = chain.Count - 2; i >= 0; i--)
			{
				(int soughtId, int position) = chain[i];
				for (int candidate = chain[i + 1].Position - 1; candidate > position; candidate--)
				{
					if (await CanBeTakenBy(candidate, soughtId))
					{
						chain[i] = (soughtId, candidate);
						break;
					}
				}
			}
		}

		/// <summary>
		///     Another occurrence of the assigned item can always be taken; one-to-one, a searched item that is not
		///     assigned yet can replace the assigned one, as the matching stays maximum.
		/// </summary>
		private async ValueTask<bool> CanBeTakenBy(int position, int soughtId)
		{
			int searchedId = _searched[position];
			int assignedSearchedId = _assignedSearchedId[soughtId];
			if (searchedId == assignedSearchedId)
			{
				return true;
			}

			if (!_isOneToOne || _isSearchedMatched[searchedId] || !await _isMatch(searchedId, soughtId))
			{
				return false;
			}

			_isSearchedMatched[assignedSearchedId] = false;
			_isSearchedMatched[searchedId] = true;
			_assignedSearchedId[soughtId] = searchedId;
			return true;
		}

		private bool IsOutside(int searchedId, int start, int end)
		{
			List<int> occurrences = _occurrences[searchedId];
			return occurrences[0] < start || occurrences[occurrences.Count - 1] > end;
		}
	}
}
