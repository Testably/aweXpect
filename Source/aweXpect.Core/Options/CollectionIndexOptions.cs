using System;
using aweXpect.Core;
using aweXpect.Core.Helpers;

namespace aweXpect.Options;

/// <summary>
///     Options for limitations on a collection index.
/// </summary>
public class CollectionIndexOptions
{
	private static readonly IMatch DefaultMatch = new AlwaysMatch();
	private string? _indexOption;

	/// <summary>
	///     The object used to check if an index is a match.
	/// </summary>
	public IMatch Match { get; private set; } = DefaultMatch;

	/// <summary>
	///     Sets the object to verify the index match.
	/// </summary>
	public void SetMatch(IMatch match)
		=> Match = match;

	/// <summary>
	///     Sets the object of the given <paramref name="option" /> to verify the index match.
	/// </summary>
	/// <exception cref="InvalidOperationException">An index is already specified.</exception>
	public void SetMatch(IMatch match, string option)
		=> SetIndex(match, option);

	/// <summary>
	///     Only matches the item at the given zero-based <paramref name="index" />.
	/// </summary>
	/// <exception cref="ArgumentOutOfRangeException">The <paramref name="index" /> is negative.</exception>
	/// <exception cref="InvalidOperationException">An index is already specified.</exception>
	public void AtIndex(int index)
		=> SetIndex(new AtIndexMatch(index), nameof(AtIndex));

	/// <summary>
	///     Only matches the item at the given zero-based <paramref name="index" />, counted from the end of the
	///     collection.
	/// </summary>
	/// <exception cref="ArgumentOutOfRangeException">The <paramref name="index" /> is negative.</exception>
	/// <exception cref="InvalidOperationException">An index is already specified.</exception>
	public void AtIndexFromEnd(int index)
		=> SetIndex(new AtIndexMatch(index).FromEnd(), nameof(AtIndexFromEnd));

	private void SetIndex(IMatch match, string option)
	{
		ThrowHelper.ThrowIfOptionIsAlreadySpecified(_indexOption, option);
		_indexOption = option;
		Match = match;
	}

	private sealed class AtIndexMatch : IMatchFromBeginning
	{
		private readonly int _index;

		public AtIndexMatch(int index)
		{
			if (index < 0)
			{
				// ReSharper disable once LocalizableElement
				throw Tracing.WriteException(
					new ArgumentOutOfRangeException(nameof(index), "The index must not be negative."));
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
		public IMatchFromEnd FromEnd() => new AtIndexMatchFromEnd(this);

		private sealed class AtIndexMatchFromEnd(AtIndexMatch inner) : IMatchFromEnd
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

	private sealed class AlwaysMatch : IMatchFromBeginning
	{
		/// <inheritdoc cref="CollectionIndexOptions.IMatch.GetDescription()" />
		public string GetDescription()
			=> "";

		/// <inheritdoc cref="CollectionIndexOptions.IMatch.OnlySingleIndex()" />
		public bool OnlySingleIndex()
			=> false;

		/// <inheritdoc cref="CollectionIndexOptions.IMatchFromBeginning.MatchesIndex(int)" />
		public bool? MatchesIndex(int index)
			=> true;

		/// <inheritdoc cref="CollectionIndexOptions.IMatchFromBeginning.FromEnd()" />
		public IMatchFromEnd FromEnd()
			=> throw Tracing.WriteException(new NotSupportedException("You have to specify a dedicated index condition first."));
	}

	/// <summary>
	///     Base interface for objects used to check if an index is a match.
	/// </summary>
	public interface IMatch
	{
		/// <summary>
		///     Returns the description of the <see cref="CollectionIndexOptions.IMatch" />.
		/// </summary>
		string GetDescription();

		/// <summary>
		///     Flag indicating if only a single index is considered a match.
		/// </summary>
		bool OnlySingleIndex();
	}

	/// <summary>
	///     Checks if an index is a match from the beginning of the collection.
	/// </summary>
	public interface IMatchFromBeginning : IMatch
	{
		/// <summary>
		///     Checks if the <paramref name="index" /> is a match.
		/// </summary>
		/// <returns>
		///     <see langword="true" />, if the <paramref name="index" /> is in range, <see langword="null" />,
		///     if the <paramref name="index" /> is not in range, but could be in range for a larger index,
		///     otherwise <see langword="false" /> when the <paramref name="index" /> is not in range
		///     and will also not be in range for larger values.
		/// </returns>
		bool? MatchesIndex(int index);

		/// <summary>
		///     Check the index match from the end of the collection.
		/// </summary>
		IMatchFromEnd FromEnd();
	}

	/// <summary>
	///     Checks if an index is a match from the end of the collection.
	/// </summary>
	public interface IMatchFromEnd : IMatch
	{
		/// <summary>
		///     Checks if the <paramref name="index" /> is a match from end with <paramref name="count" /> total items.
		/// </summary>
		/// <returns>
		///     <see langword="true" />, if the <paramref name="index" /> is in range, <see langword="null" />,
		///     if the <paramref name="index" /> is not in range, but could be in range for a larger index,
		///     otherwise <see langword="false" /> when the <paramref name="index" /> is not in range
		///     and will also not be in range for larger values.
		/// </returns>
		bool? MatchesIndex(int index, int? count);
	}
}
