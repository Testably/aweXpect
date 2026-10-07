using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect;

/// <summary>
///     Counts how often each distinct member occurs, using the <paramref name="options" /> to decide which members
///     are the same.
/// </summary>
/// <remarks>
///     With a <paramref name="getHashCode" /> that returns the same value for every two members that
///     <paramref name="options" /> consider the same, a member is only compared with the distinct members
///     of the same hash code, in the order they were added, so that it is still counted for the first one it equals.
/// </remarks>
internal sealed class OccurrenceCounter<TMember>(
	IOptionsEquality<TMember> options,
	Func<TMember, int>? getHashCode = null)
{
	/// <summary>
	///     The index of the first distinct member with each hash code.
	/// </summary>
	private readonly Dictionary<int, int>? _candidates = getHashCode is null ? null : new Dictionary<int, int>();

	private readonly List<TMember> _distinctMembers = [];
	private readonly List<int> _occurrences = [];

	/// <summary>
	///     The indices of the further distinct members with the same hash code, which are rare, so that a list is only
	///     created for a hash code that is shared.
	/// </summary>
	private Dictionary<int, List<int>>? _furtherCandidates;

	private int _notUniqueCount;

	/// <summary>
	///     Counts one occurrence of the <paramref name="member" /> and returns the index of its distinct member.
	/// </summary>
	public async ValueTask<int> Add(TMember member)
	{
		if (_candidates is null)
		{
			for (int i = 0; i < _distinctMembers.Count; i++)
			{
				if (await options.AreConsideredEqual(member, _distinctMembers[i]))
				{
					return Count(i);
				}
			}

			return AddDistinct(member);
		}

		int hashCode = getHashCode!(member);
		if (!_candidates.TryGetValue(hashCode, out int firstCandidate))
		{
			int firstIndex = AddDistinct(member);
			_candidates.Add(hashCode, firstIndex);
			return firstIndex;
		}

		if (await options.AreConsideredEqual(member, _distinctMembers[firstCandidate]))
		{
			return Count(firstCandidate);
		}

		_furtherCandidates ??= new Dictionary<int, List<int>>();
		if (!_furtherCandidates.TryGetValue(hashCode, out List<int>? candidates))
		{
			candidates = [];
			_furtherCandidates.Add(hashCode, candidates);
		}

		foreach (int i in candidates)
		{
			if (await options.AreConsideredEqual(member, _distinctMembers[i]))
			{
				return Count(i);
			}
		}

		int index = AddDistinct(member);
		candidates.Add(index);
		return index;
	}

	private int Count(int index)
	{
		_occurrences[index]++;
		_notUniqueCount += _occurrences[index] == 2 ? 2 : 1;
		return index;
	}

	private int AddDistinct(TMember member)
	{
		_distinctMembers.Add(member);
		_occurrences.Add(1);
		return _distinctMembers.Count - 1;
	}

	/// <summary>
	///     Whether the distinct member at <paramref name="index" /> occurred exactly once.
	/// </summary>
	public bool IsUnique(int index) => _occurrences[index] == 1;

	/// <summary>
	///     Whether the members counted so far determine the outcome of the <paramref name="quantifier" />.
	/// </summary>
	/// <remarks>
	///     Only the members that occurred more than once are counted, as they stay not unique, while a member that
	///     occurred once so far can still occur again.
	/// </remarks>
	public bool Determines(EnumerableQuantifier quantifier, bool expectUnique)
		=> expectUnique
			? quantifier.IsDeterminable(0, _notUniqueCount)
			: quantifier.IsDeterminable(_notUniqueCount, 0);
}
