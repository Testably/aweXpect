using System;
using aweXpect.Core;
using aweXpect.Core.Helpers;

namespace aweXpect.Options;

/// <summary>
///     Options for limitations on a collection index.
/// </summary>
/// <remarks>
///     An immutable value: the option methods return new options, and <see langword="default" /> matches every index,
///     like <see cref="Default" />.
/// </remarks>
[ImmutableOptions]
public readonly struct CollectionIndexOptions
{
	private static readonly IMatch DefaultMatch = new AlwaysMatch();

	/// <summary>
	///     The match of <see cref="AtIndex" /> or <see cref="AtIndexFromEnd" />, or a <see cref="NamedMatch" /> of
	///     <see cref="SetMatch" />.
	/// </summary>
	/// <remarks>
	///     A single reference keeps the options as small as a reference, so that the holder of an expectation stays at
	///     its minimum size; the built-in matches are named by their type.
	/// </remarks>
	private readonly object? _match;

	private CollectionIndexOptions(object match)
	{
		_match = match;
	}

	/// <summary>
	///     The options without a limitation on the index.
	/// </summary>
	public static CollectionIndexOptions Default => default;

	/// <summary>
	///     The object used to check if an index is a match.
	/// </summary>
	public IMatch Match => _match switch
	{
		NamedMatch namedMatch => namedMatch.Match,
		IMatch match => match,
		_ => DefaultMatch,
	};

	/// <summary>
	///     The name of the option that specified the index.
	/// </summary>
	private string? IndexOption => _match switch
	{
		null => null,
		NamedMatch namedMatch => namedMatch.OptionName,
		IMatchFromEnd => nameof(AtIndexFromEnd),
		_ => nameof(AtIndex),
	};

	/// <summary>
	///     Sets the object of the option named <paramref name="optionName" /> to verify the index match.
	/// </summary>
	/// <remarks>
	///     An index of the same option replaces the earlier one.
	/// </remarks>
	/// <exception cref="InvalidOperationException">An index of another option is already specified.</exception>
	public CollectionIndexOptions SetMatch(IMatch match, string optionName)
		=> SetIndex(new NamedMatch(match, optionName), optionName);

	/// <summary>
	///     Only matches the item at the given zero-based <paramref name="index" />.
	/// </summary>
	/// <exception cref="ArgumentOutOfRangeException">The <paramref name="index" /> is negative.</exception>
	/// <exception cref="InvalidOperationException">An index of another option is already specified.</exception>
	public CollectionIndexOptions AtIndex(int index)
		=> SetIndex(new AtIndexMatch(index), nameof(AtIndex));

	/// <summary>
	///     Only matches the item at the given zero-based <paramref name="index" />, counted from the end of the
	///     collection.
	/// </summary>
	/// <exception cref="ArgumentOutOfRangeException">The <paramref name="index" /> is negative.</exception>
	/// <exception cref="InvalidOperationException">An index of another option is already specified.</exception>
	public CollectionIndexOptions AtIndexFromEnd(int index)
		=> SetIndex(new AtIndexMatch(index).FromEnd(), nameof(AtIndexFromEnd));

	/// <summary>
	///     Checks if the item at the zero-based <paramref name="index" /> of a collection with <paramref name="count" />
	///     items is a match of the <see cref="Match" />.
	/// </summary>
	/// <param name="index">The zero-based index of the item.</param>
	/// <param name="count">
	///     The number of items in the collection, or <see langword="null" />, if it is not known yet.
	/// </param>
	/// <returns>
	///     <see langword="true" />, if the <paramref name="index" /> is in range, <see langword="null" />,
	///     if the <paramref name="index" /> is not in range, but could be in range for a larger index,
	///     otherwise <see langword="false" /> when the <paramref name="index" /> is not in range
	///     and will also not be in range for larger values.
	/// </returns>
	/// <remarks>
	///     Only <see langword="true" /> means that the item is in range.
	/// </remarks>
	public bool? MatchesIndex(int index, int? count)
		=> Match switch
		{
			IMatchFromBeginning fromBeginning => fromBeginning.MatchesIndex(index),
			IMatchFromEnd fromEnd => fromEnd.MatchesIndex(index, count),
			_ => false,
		};

	private CollectionIndexOptions SetIndex(object match, string option)
	{
		ThrowHelper.ThrowIfCompetingOption(IndexOption, option);
		return new CollectionIndexOptions(match);
	}

	private sealed class NamedMatch(IMatch match, string optionName)
	{
		public IMatch Match { get; } = match;
		public string OptionName { get; } = optionName;
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
