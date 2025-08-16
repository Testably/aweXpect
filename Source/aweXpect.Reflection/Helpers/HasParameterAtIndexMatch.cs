using System;
using aweXpect.Options;

namespace aweXpect.Reflection.Helpers;

internal sealed class HasParameterAtIndexMatch : CollectionIndexOptions.IMatchFromBeginning
{
	private readonly int _index;

	public HasParameterAtIndexMatch(int index)
	{
		if (index < 0)
		{
			throw new ArgumentOutOfRangeException(nameof(index),
				"The index must be greater than or equal to 0.");
		}

		_index = index;
	}

	/// <inheritdoc cref="CollectionIndexOptions.IMatch.GetDescription()" />
	public string GetDescription() => $" at index {_index}";

	/// <inheritdoc cref="CollectionIndexOptions.IMatch.OnlySingleIndex()" />
	public bool OnlySingleIndex() => true;

	/// <inheritdoc cref="CollectionIndexOptions.IMatchFromBeginning.MatchesIndex(int)" />
	public bool? MatchesIndex(int index)
	{
		if (index < _index)
		{
			return null;
		}

		return index == _index;
	}

	/// <inheritdoc cref="CollectionIndexOptions.IMatchFromBeginning.FromEnd()" />
	public CollectionIndexOptions.IMatchFromEnd FromEnd() => new HasParameterAtIndexMatchFromEnd(this);

	private sealed class HasParameterAtIndexMatchFromEnd(HasParameterAtIndexMatch inner)
		: CollectionIndexOptions.IMatchFromEnd
	{
		/// <inheritdoc cref="CollectionIndexOptions.IMatch.GetDescription()" />
		public string GetDescription()
			=> inner.GetDescription() + " from end";

		/// <inheritdoc cref="CollectionIndexOptions.IMatch.OnlySingleIndex()" />
		public bool OnlySingleIndex()
			=> inner.OnlySingleIndex();

		/// <inheritdoc cref="CollectionIndexOptions.IMatchFromEnd.MatchesIndex(int, int?)" />
		public bool? MatchesIndex(int index, int? count)
		{
			if (count is null)
			{
				return null;
			}

			int expected = count.Value - inner._index - 1;
			if (index < expected)
			{
				return null;
			}

			return index == expected;
		}
	}
}
