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
		/// <param name="orderMatch">How the sought items have to appear among the searched ones.</param>
		public DistinctItemsInOrder(int[] searched, int searchedCount, int soughtCount,
			Func<int, int, ValueTask<bool>> isMatch, OrderMatch orderMatch)
		{
			_searched = searched;
			_searchedCount = searchedCount;
			_soughtCount = soughtCount;
			_isMatch = isMatch;
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
		/// <remarks>
		///     As all items that match the same sought item are duplicates, each sought item is either matched by an
		///     already chosen item, or takes the next searched item that matches it; for an equivalence relation, taking
		///     the first one never prevents a match.
		/// </remarks>
		public async ValueTask<bool> IsInOrder()
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

		/// <summary>
		///     The searched items contain a distinct item that is no sought item, after a successful
		///     <see cref="IsInOrder" />.
		/// </summary>
		public async ValueTask<bool> HasAdditionalItem()
		{
			for (int searchedId = 0; searchedId < _searchedCount; searchedId++)
			{
				if (!_placedIds.Contains(searchedId) && !await MatchesAnySoughtItem(searchedId))
				{
					return true;
				}
			}

			return false;
		}

		private async ValueTask<bool> MatchesAnySoughtItem(int searchedId)
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
			if (position + 1 < _searched.Length && await IsMatch(_searched[position + 1], soughtId))
			{
				return position + 1;
			}

			if (await Any(chosen, chosenPosition => IsMatch(_searched[chosenPosition], soughtId)))
			{
				return int.MaxValue;
			}

			for (int next = position + 1; next < _searched.Length; next++)
			{
				if (await IsMatch(_searched[next], soughtId))
				{
					return next;
				}
			}

			return -1;
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
	}
}
