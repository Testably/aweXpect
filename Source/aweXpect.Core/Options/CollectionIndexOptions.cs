using System;
using aweXpect.Core;

namespace aweXpect.Options;

/// <summary>
///     Options for limitations on a collection index.
/// </summary>
public class CollectionIndexOptions
{
	private static readonly IMatch DefaultMatch = new AlwaysMatch();

	/// <summary>
	///     The object used to check if an index is a match.
	/// </summary>
	public IMatch Match { get; private set; } = DefaultMatch;

	/// <summary>
	///     Sets the object to verify the index match.
	/// </summary>
	public void SetMatch(IMatch match)
		=> Match = match;

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
		///     Flag indicating, if only a single index is considered a match.
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
