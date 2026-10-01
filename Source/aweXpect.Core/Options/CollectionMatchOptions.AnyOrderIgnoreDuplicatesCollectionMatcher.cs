using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using aweXpect.Core;

namespace aweXpect.Options;

public partial class CollectionMatchOptions
{
	/// <summary>
	///     Ignoring duplicates compares the expected values as a set: each subject item has to match an expected value,
	///     and each expected value has to be matched by a subject item.
	/// </summary>
	/// <remarks>
	///     The equality options only ever compare a subject item with an expected value, so items that match the same
	///     expected value are duplicates, e.g. "a" and "A" when ignoring the casing, and so are expected values that the
	///     same item matches. This needs no comparison of two items of the same side, which the options cannot do for
	///     untyped subjects or patterns.
	/// </remarks>
	private sealed class AnyOrderIgnoreDuplicatesCollectionMatcher<T, T2> : ICollectionMatcher<T, T2>
		where T : T2
	{
		private readonly Dictionary<int, T> _additionalItems = new();
		private readonly List<T> _coveredItems = new();
		private readonly EquivalenceRelations _equivalenceRelations;
		private readonly List<T> _missingItems;
		private readonly int _totalExpectedCount;
		private readonly HashSet<T> _uniqueItems = new();
		private int _index;

		public AnyOrderIgnoreDuplicatesCollectionMatcher(EquivalenceRelations equivalenceRelation,
			IEnumerable<T> expected)
		{
			_equivalenceRelations = equivalenceRelation;
			_missingItems = expected.Distinct().ToList();
			_totalExpectedCount = _missingItems.Count;
		}

		public async ValueTask<(bool, string?)>
			Verify(string it, T value, IOptionsEquality<T2> options, int maximumNumber)
		{
			int index = _index++;
			if (!_uniqueItems.Add(value))
			{
				return (false, null);
			}

			if (!await CoverAMissingItem(value, options) && IsAdditionalItemRelevant() &&
			    !await Any(_coveredItems, expected => options.AreConsideredEqual(value, expected)))
			{
				_additionalItems.Add(index, value);
			}

			return CountAdditionalDeviations() > 2L * maximumNumber
				? (true, TooManyDeviationsError(it, maximumNumber, GetDeviations()))
				: (false, null);
		}

		public async ValueTask<(bool, string?)>
			VerifyComplete(string it, IOptionsEquality<T2> options, int maximumNumber)
		{
			await CoverTheRemainingMissingItems(options, maximumNumber);

			// For the containment relation, all deviations are missing items, which are known completely here.
			if (!_equivalenceRelations.HasFlag(EquivalenceRelations.Contains) &&
			    CountMissingDeviations() + CountAdditionalDeviations() > 2L * maximumNumber)
			{
				return (true, TooManyDeviationsError(it, maximumNumber, GetDeviations()));
			}

			Func<object?, string> formatItem = CreateItemFormatter();
			List<string> errors = new();
			if (!_equivalenceRelations.HasFlag(EquivalenceRelations.Contains))
			{
				errors.AddRange(AdditionalItemsError(_additionalItems, formatItem));
			}
			else if (_equivalenceRelations.HasFlag(EquivalenceRelations.ContainsProperly) && !_additionalItems.Any())
			{
				errors.Add("did not contain any additional items");
			}

			if (!_equivalenceRelations.HasFlag(EquivalenceRelations.IsContainedIn))
			{
				errors.AddRange(MissingItemsError(_totalExpectedCount, _missingItems, _equivalenceRelations, true,
					formatItem, options, maximumNumber));
			}
			else if (_equivalenceRelations.HasFlag(EquivalenceRelations.IsContainedInProperly) && !_missingItems.Any())
			{
				errors.Add("contained all expected items");
			}

			string? error = ReturnErrorString(it, errors);
			return (error != null, error);
		}

		/// <summary>
		///     Moves the first missing item that the <paramref name="value" /> matches to the covered items; other missing
		///     items it matches are only searched for at the end, so that a successful comparison compares each item only
		///     until its first match.
		/// </summary>
		/// <returns><see langword="true" />, when the <paramref name="value" /> matched a missing item.</returns>
		private async ValueTask<bool> CoverAMissingItem(T value, IOptionsEquality<T2> options)
		{
			for (int i = 0; i < _missingItems.Count; i++)
			{
				if (await options.AreConsideredEqual(value, _missingItems[i]))
				{
					_coveredItems.Add(_missingItems[i]);
					_missingItems.RemoveAt(i);
					return true;
				}
			}

			return false;
		}

		/// <summary>
		///     A missing item can still be matched by an item that matched another expected item first.
		/// </summary>
		/// <remarks>
		///     The search stops once the result no longer depends on it: the proper containment only needs one missing
		///     item, and too many deviations only list the additional items.
		/// </remarks>
		private async ValueTask CoverTheRemainingMissingItems(IOptionsEquality<T2> options, int maximumNumber)
		{
			if (_equivalenceRelations == EquivalenceRelations.IsContainedIn)
			{
				return;
			}

			int stillMissing = 0;
			for (int i = 0; i < _missingItems.Count; i++)
			{
				T expected = _missingItems[i];
				if (await Any(_uniqueItems, value => options.AreConsideredEqual(value, expected)))
				{
					_missingItems.RemoveAt(i--);
				}
				else if (_equivalenceRelations.HasFlag(EquivalenceRelations.IsContainedIn) ||
				         (!_equivalenceRelations.HasFlag(EquivalenceRelations.Contains) &&
				          ++stillMissing + CountAdditionalDeviations() > 2L * maximumNumber))
				{
					return;
				}
			}
		}

		/// <summary>
		///     The containment relation only needs one additional item, for the proper containment.
		/// </summary>
		private bool IsAdditionalItemRelevant()
			=> !_equivalenceRelations.HasFlag(EquivalenceRelations.Contains) ||
			   (_equivalenceRelations.HasFlag(EquivalenceRelations.ContainsProperly) && _additionalItems.Count == 0);

		/// <summary>
		///     Additional items are no deviation for the containment relation, so they are not counted.
		/// </summary>
		private int CountAdditionalDeviations()
			=> _equivalenceRelations.HasFlag(EquivalenceRelations.Contains) ? 0 : _additionalItems.Count;

		/// <summary>
		///     Missing items are no deviation for the IsContainedIn relation, so they are not counted.
		/// </summary>
		private int CountMissingDeviations()
			=> _equivalenceRelations.HasFlag(EquivalenceRelations.IsContainedIn) ? 0 : _missingItems.Count;

		private IEnumerable<string> GetDeviations()
			=> AdditionalItemsError(_additionalItems, CreateItemFormatter());

		/// <summary>
		///     An unexpected and a missing item that format equally differ only in their runtime type.
		/// </summary>
		private Func<object?, string> CreateItemFormatter()
			=> GetItemFormatter(_additionalItems.Values.Cast<object?>(), _missingItems.Cast<object?>());
	}
}
