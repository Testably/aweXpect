using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using aweXpect.Options;

namespace aweXpect;

/// <summary>
///     Counts how often each distinct member occurs, using <paramref name="areConsideredEqual" /> to decide which
///     members are the same.
/// </summary>
internal sealed class OccurrenceCounter<TMember>(
	Func<TMember, TMember, ValueTask<bool>> areConsideredEqual)
{
	private readonly List<TMember> _distinctMembers = [];
	private readonly List<int> _occurrences = [];
	private int _notUniqueCount;

	/// <summary>
	///     Counts one occurrence of the <paramref name="member" /> and returns the index of its distinct member.
	/// </summary>
	public async Task<int> Add(TMember member)
	{
		for (int i = 0; i < _distinctMembers.Count; i++)
		{
			if (await areConsideredEqual(member, _distinctMembers[i]))
			{
				_occurrences[i]++;
				_notUniqueCount += _occurrences[i] == 2 ? 2 : 1;
				return i;
			}
		}

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
