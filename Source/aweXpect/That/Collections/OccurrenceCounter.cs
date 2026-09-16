using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace aweXpect;

/// <summary>
///     Counts how often each distinct member occurs, using <paramref name="areConsideredEqual" /> to decide which
///     members are the same.
/// </summary>
internal sealed class OccurrenceCounter<TMember>(
#if NET8_0_OR_GREATER
	Func<TMember, TMember, ValueTask<bool>> areConsideredEqual)
#else
	Func<TMember, TMember, Task<bool>> areConsideredEqual)
#endif
{
	private readonly List<TMember> _distinctMembers = [];
	private readonly List<int> _occurrences = [];

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
}
