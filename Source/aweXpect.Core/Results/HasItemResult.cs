using System;
using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect.Results;

/// <summary>
///     The result for verifying that a collection has a matching item at a given index.
/// </summary>
/// <remarks>
///     <seealso cref="AndOrResult{TType,TSelf}" />
/// </remarks>
public class HasItemResult<TCollection>(
	ExpectationBuilder expectationBuilder,
	IThat<TCollection> collection,
	CollectionIndexOptions collectionIndexOptions)
	: AndOrResult<TCollection, IThat<TCollection>>(expectationBuilder, collection),
		IOptionsProvider<CollectionIndexOptions>
{
	private readonly IThat<TCollection> _collection = collection;
	private readonly ExpectationBuilder _expectationBuilder = expectationBuilder;

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	CollectionIndexOptions IOptionsProvider<CollectionIndexOptions>.Options => collectionIndexOptions;

	/// <summary>
	///     …at the given <paramref name="index" />.
	/// </summary>
	public HasItemAtIndexResult<TCollection> AtIndex(int index)
	{
		collectionIndexOptions.SetMatch(new HasItemResultAtIndexMatch(index));
		return new HasItemAtIndexResult<TCollection>(_expectationBuilder, _collection, collectionIndexOptions);
	}

	private sealed class HasItemResultAtIndexMatch : CollectionIndexOptions.IMatchFromBeginning
	{
		private readonly int _index;

		public HasItemResultAtIndexMatch(int index)
		{
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(index), "The index must be greater than or equal to 0.");
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
		public CollectionIndexOptions.IMatchFromEnd FromEnd() => new HasItemResultAtIndexMatchFromEnd(this);

		private sealed class HasItemResultAtIndexMatchFromEnd(HasItemResultAtIndexMatch inner)
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
}
