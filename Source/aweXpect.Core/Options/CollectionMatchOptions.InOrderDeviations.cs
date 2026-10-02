using System;
using System.Collections.Generic;
using System.Linq;

namespace aweXpect.Options;

public partial class CollectionMatchOptions
{
	/// <summary>
	///     The deviations of an <see cref="InOrderMismatch" /> as items of the subject and of the expected collection.
	/// </summary>
	/// <remarks>
	///     The containment searches the subject in the expected items, the other relations search the expected items in
	///     the subject.
	/// </remarks>
	private sealed class InOrderDeviations<T, T3>
	{
		private InOrderDeviations()
		{
		}

		private Dictionary<int, (T Item, T3 Expected)> InterruptingItems { get; } = new();
		private Dictionary<int, T> OutOfOrderItems { get; } = new();
		private Dictionary<int, T> UnexpectedItems { get; } = new();
		private List<T3> MissingItems { get; } = new();
		private List<T3> RemainingExpectedItems { get; } = new();
		private bool HasAdditionalItem { get; set; }
		private bool HasRemainingExpectedItem { get; set; }

		/// <summary>
		///     For equality, the subject has the same items in a different order.
		/// </summary>
		public bool MatchesInAnyOrder => UnexpectedItems.Count == 0 && MissingItems.Count == 0;

		/// <param name="mismatch">The mismatch to describe.</param>
		/// <param name="isContainedIn">Whether the subject was searched in the expected items.</param>
		/// <param name="subject">
		///     The index and the subject item of a searched position, or of a sought id when the subject was searched.
		/// </param>
		/// <param name="expected">
		///     The expected item of a sought id, or of a searched position when the subject was searched.
		/// </param>
		public static InOrderDeviations<T, T3> From(InOrderMismatch mismatch, bool isContainedIn,
			Func<int, (int Index, T Item)> subject, Func<int, T3> expected)
		{
			InOrderDeviations<T, T3> deviations = new();
			if (isContainedIn)
			{
				deviations.AddSearchedSubject(mismatch, subject, expected);
			}
			else
			{
				deviations.AddSoughtExpectedItems(mismatch, subject, expected);
			}

			return deviations;
		}

		private void AddSoughtExpectedItems(InOrderMismatch mismatch,
			Func<int, (int Index, T Item)> subjectAt, Func<int, T3> expectedOf)
		{
			foreach ((int position, int soughtId) in mismatch.Interruptions)
			{
				InterruptingItems.Add(position, (subjectAt(position).Item, expectedOf(soughtId)));
			}

			foreach (int position in mismatch.OutOfOrder.Select(item => item.Position).OrderBy(position => position))
			{
				OutOfOrderItems.Add(position, subjectAt(position).Item);
			}

			foreach (int position in mismatch.UnmatchedSearchedPositions)
			{
				UnexpectedItems.Add(position, subjectAt(position).Item);
			}

			MissingItems.AddRange(mismatch.UnmatchedSoughtIds.Select(expectedOf));
			HasAdditionalItem = mismatch.UnmatchedSearchedPositions.Count > 0;
		}

		/// <remarks>
		///     A gap in the run of expected items is reported once, at the subject item behind it.
		/// </remarks>
		private void AddSearchedSubject(InOrderMismatch mismatch,
			Func<int, (int Index, T Item)> subjectOf, Func<int, T3> expectedAt)
		{
			foreach ((int position, int soughtId) in mismatch.Interruptions)
			{
				(int index, T item) = subjectOf(soughtId);
				if (!InterruptingItems.ContainsKey(index))
				{
					InterruptingItems.Add(index, (item, expectedAt(position)));
				}
			}

			foreach ((int index, T item) in mismatch.OutOfOrder.Select(item => subjectOf(item.SoughtId))
				         .OrderBy(item => item.Index))
			{
				OutOfOrderItems.Add(index, item);
			}

			foreach ((int index, T item) in mismatch.UnmatchedSoughtIds.Select(subjectOf))
			{
				UnexpectedItems.Add(index, item);
			}

			RemainingExpectedItems.AddRange(mismatch.UnmatchedSearchedPositions.Select(expectedAt));
			HasRemainingExpectedItem = RemainingExpectedItems.Count > 0;
		}

		/// <summary>
		///     The deviations that name a subject item; for the containment relation, additional items are none.
		/// </summary>
		public List<string> ListDeviations(EquivalenceRelations equivalenceRelation, object options)
			=> IncorrectItemsError(InterruptingItems, options)
				.Concat(OutOfOrderItemsError(OutOfOrderItems))
				.Concat(equivalenceRelation.HasFlag(EquivalenceRelations.Contains)
					? []
					: AdditionalItemsError(UnexpectedItems, CreateItemFormatter()))
				.ToList();

		/// <remarks>
		///     Like for the matchers in any order, the containment relation lists all missing items instead of reporting
		///     too many deviations.
		/// </remarks>
		public string? GetError(string it, EquivalenceRelations equivalenceRelation, bool ignoringDuplicates,
			int totalExpectedItems, object options, int maximumNumber)
		{
			bool isContains = equivalenceRelation.HasFlag(EquivalenceRelations.Contains);
			bool isContainedIn = equivalenceRelation.HasFlag(EquivalenceRelations.IsContainedIn);
			List<string> errors = ListDeviations(equivalenceRelation, options);
			int missingDeviations = isContainedIn ? 0 : MissingItems.Count;
			if (errors.Count + missingDeviations > 2L * maximumNumber && (!isContains || errors.Count > 0))
			{
				return TooManyDeviationsError(it, maximumNumber, errors);
			}

			if (equivalenceRelation.HasFlag(EquivalenceRelations.ContainsProperly) && !HasAdditionalItem)
			{
				errors.Add("did not contain any additional items");
			}

			if (!isContainedIn)
			{
				errors.AddRange(MissingItemsError(totalExpectedItems, MissingItems, equivalenceRelation,
					ignoringDuplicates, CreateItemFormatter(), options, maximumNumber));
			}
			else if (equivalenceRelation.HasFlag(EquivalenceRelations.IsContainedInProperly) &&
			         !HasRemainingExpectedItem)
			{
				errors.Add("contained all expected items");
			}

			return ReturnErrorString(it, errors);
		}

		/// <summary>
		///     An unexpected and a missing item that format equally differ only in their runtime type; the containment
		///     lists no missing items, but the expected items that the subject does not use show the same ambiguity.
		/// </summary>
		private Func<object?, string> CreateItemFormatter()
			=> GetItemFormatter(UnexpectedItems.Values.Cast<object?>(),
				MissingItems.Concat(RemainingExpectedItems).Cast<object?>());
	}
}
