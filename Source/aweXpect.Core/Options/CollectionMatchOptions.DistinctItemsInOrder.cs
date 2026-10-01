using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace aweXpect.Options;

public partial class CollectionMatchOptions
{
	private enum OrderMatch
	{
		Equal,
		Contiguous,
		Subsequence,
	}

	/// <summary>
	///     Decides, whether the distinct items of a sought collection appear in a searched collection in order, when
	///     duplicates are ignored.
	/// </summary>
	/// <remarks>
	///     Each duplicate in the searched collection may be the occurrence that counts, e.g. <c>[2, 3, 2]</c> equals
	///     <c>[3, 2]</c>, while the sought collection is reduced to the first occurrence of each item; letting both sides
	///     choose an occurrence would make the decision NP-hard (an exemplar common subsequence).<br />
	///     Both collections are given as the ids of their distinct items, numbered in the order of their first
	///     occurrence, so the sought items are the ids <c>0</c> to <c>soughtCount - 1</c>.
	/// </remarks>
	private sealed class DistinctItemsInOrder
	{
		private readonly int[] _firstOccurrence;
		private readonly Func<int, int, ValueTask<bool>> _isMatch;
		private readonly Dictionary<(int SearchedId, int SoughtId), bool> _isMatchCache = new();
		private readonly bool _isOneToOne;
		private readonly int[] _lastOccurrence;
		private readonly OrderMatch _orderMatch;
		private readonly int[] _searched;
		private readonly int _searchedCount;
		private readonly int _soughtCount;
		private HashSet<int> _placedIds = new();

		/// <param name="searched">The id of each searched item.</param>
		/// <param name="searchedCount">The number of distinct searched items.</param>
		/// <param name="soughtCount">The number of distinct sought items.</param>
		/// <param name="isMatch">Compares a searched with a sought item.</param>
		/// <param name="isOneToOne">
		///     Predicates and expectations need one distinct item each; expected values form a set, so items that match the
		///     same value are duplicates.
		/// </param>
		/// <param name="orderMatch">How the sought items have to appear among the searched ones.</param>
		public DistinctItemsInOrder(int[] searched, int searchedCount, int soughtCount,
			Func<int, int, ValueTask<bool>> isMatch, bool isOneToOne, OrderMatch orderMatch)
		{
			_searched = searched;
			_searchedCount = searchedCount;
			_soughtCount = soughtCount;
			_isMatch = isMatch;
			_isOneToOne = isOneToOne;
			_orderMatch = orderMatch;
			_firstOccurrence = new int[searchedCount];
			_lastOccurrence = new int[searchedCount];
			for (int position = searched.Length - 1; position >= 0; position--)
			{
				_firstOccurrence[searched[position]] = position;
			}

			for (int position = 0; position < searched.Length; position++)
			{
				_lastOccurrence[searched[position]] = position;
			}
		}

		/// <summary>
		///     The sought items appear in order among the searched items.
		/// </summary>
		public ValueTask<bool> IsInOrder()
			=> _isOneToOne ? IsInOrderOneToOne() : IsInOrderAsSet();

		/// <summary>
		///     The searched items contain a distinct item that is no sought item, after a successful
		///     <see cref="IsInOrder" />.
		/// </summary>
		public async ValueTask<bool> HasAdditionalItem()
		{
			if (_isOneToOne)
			{
				return _searchedCount > _soughtCount;
			}

			for (int searchedId = 0; searchedId < _searchedCount; searchedId++)
			{
				if (!_placedIds.Contains(searchedId) && !await MatchesAnySoughtItem(searchedId))
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		///     Lets each sought item take the first fitting searched item, without backtracking, to describe a mismatch.
		/// </summary>
		/// <returns>
		///     The sought items that found no fitting item, the searched items that were taken, and for a run the first
		///     item that interrupts it, with the sought item behind it.
		/// </returns>
		public async ValueTask<(List<int> UnplacedSoughtIds, HashSet<int> PlacedSearchedIds,
				(int Position, int SoughtId)? Interruption)>
			PlaceFirstFitting()
		{
			List<int> unplaced = new();
			List<int> positions = new();
			List<int> soughtIds = new();
			int position = -1;
			for (int soughtId = 0; soughtId < _soughtCount; soughtId++)
			{
				int next = await FindNext(position, soughtId, positions);
				if (next < 0)
				{
					unplaced.Add(soughtId);
				}
				else if (next < _searched.Length)
				{
					positions.Add(next);
					soughtIds.Add(soughtId);
					position = next;
				}
			}

			HashSet<int> placedIds = new(positions.Select(chosen => _searched[chosen]));
			(int, int)? interruption = _orderMatch == OrderMatch.Contiguous && unplaced.Count == 0
				? await FindInterruption(positions, soughtIds, placedIds)
				: null;
			return (unplaced, placedIds, interruption);
		}

		public async ValueTask<bool> MatchesAnySoughtItem(int searchedId)
		{
			for (int soughtId = 0; soughtId < _soughtCount; soughtId++)
			{
				if (await IsMatch(searchedId, soughtId))
				{
					return true;
				}
			}

			return false;
		}

		private async ValueTask<(int, int)?> FindInterruption(List<int> positions, List<int> soughtIds,
			HashSet<int> placedIds)
		{
			int start = positions[0];
			int end = positions[positions.Count - 1];
			for (int i = 1; i < positions.Count; i++)
			{
				for (int position = positions[i - 1] + 1; position < positions[i]; position++)
				{
					int searchedId = _searched[position];
					bool isDuplicate = _isOneToOne
						? placedIds.Contains(searchedId)
						: await MatchesAnySoughtItem(searchedId);
					if (!isDuplicate && !IsOutside(searchedId, start, end))
					{
						return (position, soughtIds[i]);
					}
				}
			}

			return null;
		}

		/// <returns>
		///     The next position after <paramref name="position" /> that fits the <paramref name="soughtId" />,
		///     <see cref="int.MaxValue" /> when an already chosen item matches it, or <c>-1</c> when none fits.
		/// </returns>
		/// <remarks>
		///     When the item right after the <paramref name="position" /> matches, it is taken without asking whether a
		///     chosen item matches as well: the item would then be a duplicate, which no later sought item can use, so
		///     taking it loses nothing, and items in the expected order need no comparison with the chosen ones.
		/// </remarks>
		private async ValueTask<int> FindNext(int position, int soughtId, List<int> chosen)
		{
			if (!_isOneToOne)
			{
				if (position + 1 < _searched.Length && await IsMatch(_searched[position + 1], soughtId))
				{
					return position + 1;
				}

				if (await Any(chosen, chosenPosition => IsMatch(_searched[chosenPosition], soughtId)))
				{
					return int.MaxValue;
				}
			}

			for (int next = position + 1; next < _searched.Length; next++)
			{
				int searchedId = _searched[next];
				if ((!_isOneToOne || !chosen.Exists(chosenPosition => _searched[chosenPosition] == searchedId)) &&
				    await IsMatch(searchedId, soughtId))
				{
					return next;
				}
			}

			return -1;
		}

		/// <summary>
		///     As all items that match the same value are duplicates, each sought item is either matched by an already
		///     chosen item, or takes the next searched item that matches it; for an equivalence relation, taking the first
		///     one never prevents a match.
		/// </summary>
		private async ValueTask<bool> IsInOrderAsSet()
		{
			if (_orderMatch != OrderMatch.Contiguous || _soughtCount == 0)
			{
				List<int>? placement = await PlaceAsSet(-1);
				if (placement is null)
				{
					return false;
				}

				_placedIds = new HashSet<int>(placement.Select(position => _searched[position]));
				return _orderMatch != OrderMatch.Equal || await IsEveryOtherItemADuplicate();
			}

			for (int start = 0; start < _searched.Length; start++)
			{
				if (!await IsMatch(_searched[start], 0))
				{
					continue;
				}

				List<int>? chosen = await PlaceAsSet(start);
				if (chosen is null)
				{
					// A later start can only place the items later.
					return false;
				}

				if (await IsContiguousAsSet(start, chosen))
				{
					_placedIds = new HashSet<int>(chosen.Select(position => _searched[position]));
					return true;
				}
			}

			return false;
		}

		private async ValueTask<List<int>?> PlaceAsSet(int start)
		{
			List<int> chosen = new();
			int position = start;
			if (start >= 0)
			{
				chosen.Add(start);
			}

			for (int soughtId = chosen.Count; soughtId < _soughtCount; soughtId++)
			{
				int next = await FindNext(position, soughtId, chosen);
				if (next < 0)
				{
					return null;
				}

				if (next < _searched.Length)
				{
					chosen.Add(next);
					position = next;
				}
			}

			return chosen;
		}

		/// <summary>
		///     For equality, each searched item must match a sought item.
		/// </summary>
		private async ValueTask<bool> IsEveryOtherItemADuplicate()
		{
			for (int searchedId = 0; searchedId < _searchedCount; searchedId++)
			{
				if (!_placedIds.Contains(searchedId) && !await MatchesAnySoughtItem(searchedId))
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		///     Every other item within the run must be a duplicate: it matches a sought item, or the same item occurs
		///     outside the run, so that this occurrence counts instead.
		/// </summary>
		private async ValueTask<bool> IsContiguousAsSet(int start, List<int> chosen)
		{
			int end = chosen[chosen.Count - 1];
			HashSet<int> chosenPositions = new(chosen);
			for (int position = start + 1; position < end; position++)
			{
				int searchedId = _searched[position];
				if (!chosenPositions.Contains(position) && !IsOutside(searchedId, start, end) &&
				    !await MatchesAnySoughtItem(searchedId))
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		///     Searches distinct searched items for the sought items in order, with backtracking.
		/// </summary>
		/// <remarks>
		///     Taking the first fitting item is safe, unless the same item occurs again later and could serve a later
		///     sought item (for a run, unless it occurs anywhere else), so the search only branches at such occurrences:
		///     without them it is linear, with <c>b</c> of them it takes at most <c>2^b</c> times as many steps. Each
		///     comparison is cached, so each pair of distinct items is compared at most once, and a matching that ignores
		///     the order rules out most mismatches in polynomial time before the search.
		/// </remarks>
		private async ValueTask<bool> IsInOrderOneToOne()
		{
			if (_soughtCount == 0)
			{
				return _orderMatch != OrderMatch.Equal || _searchedCount == 0;
			}

			if ((_orderMatch == OrderMatch.Equal && _searchedCount != _soughtCount) ||
			    !await CanAssignEachSoughtItem())
			{
				return false;
			}

			if (_orderMatch != OrderMatch.Contiguous)
			{
				return await SearchOneToOne(-1);
			}

			for (int start = 0; start < _searched.Length; start++)
			{
				if (await IsMatch(_searched[start], 0) && await SearchOneToOne(start))
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		///     Each sought item needs its own distinct searched item, regardless of the order.
		/// </summary>
		private async ValueTask<bool> CanAssignEachSoughtItem()
		{
			ItemMatching<int, int> matching = new(Enumerable.Range(0, _soughtCount), IsMatch, new Dictionary<int, int>());
			for (int searchedId = 0; searchedId < _searchedCount; searchedId++)
			{
				await matching.Add(searchedId, searchedId);
			}

			await matching.ResolvePendingItems();
			return matching.UnmatchedExpectedItems().Count == 0;
		}

		/// <param name="start">The position of the first sought item in a run, otherwise <c>-1</c>.</param>
		private async ValueTask<bool> SearchOneToOne(int start)
		{
			OneToOneSearch search = new(_soughtCount, _searchedCount, start, start >= 0 ? _searched[start] : -1);
			while (true)
			{
				if (search.Step == _soughtCount)
				{
					if (IsCompleteOneToOne(search))
					{
						return true;
					}
				}
				else if (await TryChooseNext(search))
				{
					continue;
				}

				if (!await TryBacktrack(search))
				{
					return false;
				}
			}
		}

		private async ValueTask<bool> TryChooseNext(OneToOneSearch search)
		{
			for (int next = search.Position; next < _searched.Length; next++)
			{
				int searchedId = _searched[next];
				if (search.IsUsed[searchedId])
				{
					continue;
				}

				if (await IsMatch(searchedId, search.Step))
				{
					search.Chosen[search.Step] = next;
					search.IsUsed[searchedId] = true;
					search.Step++;
					search.Position = next + 1;
					return true;
				}

				if (!CanSkip(next, search.Start))
				{
					return false;
				}
			}

			return false;
		}

		/// <summary>
		///     Goes back to the last chosen item that may be skipped, and continues behind it.
		/// </summary>
		private async ValueTask<bool> TryBacktrack(OneToOneSearch search)
		{
			while (search.Step > search.FirstStep)
			{
				search.Step--;
				int skipped = search.Chosen[search.Step];
				search.IsUsed[_searched[skipped]] = false;
				if (await CanSkipChosen(skipped, search.Step, search.Start))
				{
					search.Position = skipped + 1;
					return true;
				}
			}

			return false;
		}

		/// <summary>
		///     An item that is not chosen must occur again where it can count instead.
		/// </summary>
		private bool CanSkip(int position, int start)
		{
			int searchedId = _searched[position];
			return _orderMatch switch
			{
				OrderMatch.Equal => _lastOccurrence[searchedId] > position,
				OrderMatch.Contiguous => _firstOccurrence[searchedId] < start ||
				                         _lastOccurrence[searchedId] > position,
				_ => true,
			};
		}

		/// <summary>
		///     Skipping a chosen item only helps, when it can serve a later sought item; for a run, it only has to occur
		///     elsewhere.
		/// </summary>
		private async ValueTask<bool> CanSkipChosen(int position, int step, int start)
		{
			int searchedId = _searched[position];
			if (_orderMatch == OrderMatch.Contiguous)
			{
				return CanSkip(position, start);
			}

			if (_lastOccurrence[searchedId] <= position)
			{
				return false;
			}

			for (int soughtId = step + 1; soughtId < _soughtCount; soughtId++)
			{
				if (await IsMatch(searchedId, soughtId))
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		///     Within a run, every item that is not chosen must be a duplicate of a chosen one, or occur outside the run.
		/// </summary>
		private bool IsCompleteOneToOne(OneToOneSearch search)
		{
			if (_orderMatch != OrderMatch.Contiguous)
			{
				return true;
			}

			int end = search.Chosen[search.Chosen.Length - 1];
			for (int position = search.Start + 1; position < end; position++)
			{
				int searchedId = _searched[position];
				if (!search.IsUsed[searchedId] && !IsOutside(searchedId, search.Start, end))
				{
					return false;
				}
			}

			return true;
		}

		private bool IsOutside(int searchedId, int start, int end)
			=> _firstOccurrence[searchedId] < start || _lastOccurrence[searchedId] > end;

		private async ValueTask<bool> IsMatch(int searchedId, int soughtId)
		{
			if (!_isMatchCache.TryGetValue((searchedId, soughtId), out bool isMatch))
			{
				isMatch = await _isMatch(searchedId, soughtId);
				_isMatchCache.Add((searchedId, soughtId), isMatch);
			}

			return isMatch;
		}

		/// <summary>
		///     The state of <see cref="SearchOneToOne" />: the sought item to place next, the position to continue at,
		///     and the chosen positions.
		/// </summary>
		private sealed class OneToOneSearch
		{
			public OneToOneSearch(int soughtCount, int searchedCount, int start, int startId)
			{
				Chosen = new int[soughtCount];
				IsUsed = new bool[searchedCount];
				Start = start;
				Position = start + 1;
				if (start >= 0)
				{
					Chosen[0] = start;
					IsUsed[startId] = true;
					FirstStep = 1;
				}

				Step = FirstStep;
			}

			public int[] Chosen { get; }
			public int FirstStep { get; }
			public bool[] IsUsed { get; }
			public int Position { get; set; }
			public int Start { get; }
			public int Step { get; set; }
		}
	}
}
