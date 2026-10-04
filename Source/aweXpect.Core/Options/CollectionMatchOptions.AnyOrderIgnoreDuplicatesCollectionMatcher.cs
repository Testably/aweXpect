using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Helpers;

namespace aweXpect.Options;

public partial class CollectionMatchOptions
{
	private sealed class AnyOrderIgnoreDuplicatesCollectionMatcher<T, T2>(
		EquivalenceRelations equivalenceRelation,
		IEnumerable<T> expected)
		: AnyOrderIgnoreDuplicatesCollectionMatcherBase<T, T2, T>(equivalenceRelation, expected.Distinct(), true)
		where T : T2
	{
		protected override ValueTask<bool> AreConsideredEqual(int index, T value, T expected,
			IOptionsEquality<T2> options)
			=> options.AreConsideredEqual(value, expected);
	}

	private sealed class AnyOrderIgnoreDuplicatesFromExpectationCollectionMatcher<T, T2>(
		EquivalenceRelations equivalenceRelation,
		IEnumerable<ExpectationItem<T>> expected)
		: AnyOrderIgnoreDuplicatesCollectionMatcherBase<T, T2, ExpectationItem<T>>(equivalenceRelation, expected,
			false)
		where T : T2
	{
		protected override ValueTask<bool>
			AreConsideredEqual(int index, T value, ExpectationItem<T> expected, IOptionsEquality<T2> options)
			=> expected.IsMetBy(value, index);
	}

	private sealed class AnyOrderIgnoreDuplicatesFromPredicateCollectionMatcher<T, T2>(
		EquivalenceRelations equivalenceRelation,
		IEnumerable<Expression<Func<T, bool>>> expected)
		: AnyOrderIgnoreDuplicatesCollectionMatcherBase<T, T2, Expression<Func<T, bool>>>(equivalenceRelation,
			expected, false)
		where T : T2
	{
		private readonly CompiledPredicates<T> _predicates = new();

		protected override ValueTask<bool> AreConsideredEqual(int index, T value, Expression<Func<T, bool>> expected,
			IOptionsEquality<T2> options)
			=> _predicates.Invoke(expected, value, index);
	}

	/// <summary>
	///     Ignoring duplicates compares the expected items as a set: each subject item has to match an expected item,
	///     and each expected item has to be matched by a subject item.
	/// </summary>
	/// <remarks>
	///     Only a subject item is ever compared with an expected item, so items that match the same expected item are
	///     duplicates, e.g. "a" and "A" when ignoring the casing, and so are expected items that the same item matches.
	///     This needs no comparison of two items of the same side, which the options cannot do for untyped subjects or
	///     patterns, and which predicates and expectations do not support at all.<br />
	///     A comparison that code of the caller did not answer is no match. It only decides the result when the item
	///     matches no expected item at all.
	/// </remarks>
	private abstract class AnyOrderIgnoreDuplicatesCollectionMatcherBase<T, T2, T3> : ICollectionMatcher<T, T2>
		where T : T2
	{
		private readonly Dictionary<int, T> _additionalItems = new();
		private readonly bool _areExpectedItemsUnique;
		private readonly List<T3> _coveredItems = new();
		private readonly List<(int Index, T Value)> _distinctItems = new();
		private readonly EquivalenceRelations _equivalenceRelations;
		private readonly List<T3> _missingItems;
		private readonly int _totalExpectedCount;
		private readonly HashSet<T> _uniqueItems = new();
		private int _index;

		/// <remarks>
		///     Only expected values can be told apart from each other, so only for them the count of expected items is
		///     the count of unique ones.
		/// </remarks>
		protected AnyOrderIgnoreDuplicatesCollectionMatcherBase(EquivalenceRelations equivalenceRelation,
			IEnumerable<T3> expected, bool areExpectedItemsUnique)
		{
			_equivalenceRelations = equivalenceRelation;
			_missingItems = expected.ToList();
			_totalExpectedCount = _missingItems.Count;
			_areExpectedItemsUnique = areExpectedItemsUnique;
		}

		/// <inheritdoc />
		/// <remarks>
		///     Once all expected items are found, the containment relation is met, and properly met as soon as there is
		///     also an additional item.
		/// </remarks>
		public bool IsDetermined
			=> _equivalenceRelations.Includes(EquivalenceRelations.Contains) &&
			   _missingItems.Count == 0 &&
			   (!_equivalenceRelations.Includes(EquivalenceRelations.ContainsProperly) || _additionalItems.Count > 0);

		public async ValueTask<(bool, string?)>
			Verify(string it, T value, IOptionsEquality<T2> options, int maximumNumber)
		{
			int index = _index++;
			if (!_uniqueItems.Add(value))
			{
				return (false, null);
			}

			_distinctItems.Add((index, value));
			if (await IsAdditionalItem(index, value, options))
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
			if (!_equivalenceRelations.Includes(EquivalenceRelations.Contains) &&
			    CountMissingDeviations() + CountAdditionalDeviations() > 2L * maximumNumber)
			{
				return (true, TooManyDeviationsError(it, maximumNumber, GetDeviations()));
			}

			Func<object?, string> formatItem = CreateItemFormatter();
			List<string> errors = new();
			if (!_equivalenceRelations.Includes(EquivalenceRelations.Contains))
			{
				errors.AddRange(AdditionalItemsError(_additionalItems, formatItem));
			}
			else if (_equivalenceRelations.Includes(EquivalenceRelations.ContainsProperly) && !_additionalItems.Any())
			{
				errors.Add("did not contain any additional items");
			}

			if (!_equivalenceRelations.Includes(EquivalenceRelations.IsContainedIn))
			{
				errors.AddRange(MissingItemsError(_totalExpectedCount, _missingItems, _equivalenceRelations,
					_areExpectedItemsUnique, formatItem, options, maximumNumber));
			}
			else if (_equivalenceRelations.Includes(EquivalenceRelations.IsContainedInProperly) && !_missingItems.Any())
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
		/// <remarks>
		///     For the containment relation, which can be decided before the end, the value also covers the missing items
		///     right behind its match as long as it matches them, so that an expected item that repeats the previous one
		///     does not delay the decision; this costs a single further comparison per item.
		/// </remarks>
		/// <returns>
		///     <see langword="true" />, when the <paramref name="value" /> matches no expected item and is relevant as an
		///     additional item.
		/// </returns>
		private async ValueTask<bool> IsAdditionalItem(int index, T value, IOptionsEquality<T2> options)
		{
			Exception? unanswered = null;
			for (int i = 0; i < _missingItems.Count; i++)
			{
				(bool isMatch, Exception? exception) = await Compare(index, value, _missingItems[i], options);
				unanswered ??= exception;
				if (isMatch)
				{
					do
					{
						_coveredItems.Add(_missingItems[i]);
						_missingItems.RemoveAt(i);
					} while (_equivalenceRelations.Includes(EquivalenceRelations.Contains) && i < _missingItems.Count &&
					         (await Compare(index, value, _missingItems[i], options)).IsMatch);

					return false;
				}
			}

			if (unanswered is null && !IsAdditionalItemRelevant())
			{
				return false;
			}

			foreach (T3 coveredItem in _coveredItems)
			{
				(bool isMatch, Exception? exception) = await Compare(index, value, coveredItem, options);
				unanswered ??= exception;
				if (isMatch)
				{
					return false;
				}
			}

			if (unanswered is not null)
			{
				ExceptionDispatchInfo.Capture(unanswered).Throw();
			}

			return IsAdditionalItemRelevant();
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
				T3 expected = _missingItems[i];
				if (await Any(_distinctItems, async item => (await Compare(item.Index, item.Value, expected, options)).IsMatch))
				{
					_missingItems.RemoveAt(i--);
				}
				else if (_equivalenceRelations.Includes(EquivalenceRelations.IsContainedIn) ||
				         (!_equivalenceRelations.Includes(EquivalenceRelations.Contains) &&
				          ++stillMissing + CountAdditionalDeviations() > 2L * maximumNumber))
				{
					return;
				}
			}
		}

		private async ValueTask<(bool IsMatch, Exception? Unanswered)> Compare(int index, T value, T3 expected,
			IOptionsEquality<T2> options)
		{
			try
			{
				return (await AreConsideredEqual(index, value, expected, options), null);
			}
			catch (Exception exception) when (exception is UserCodeException or UnansweredItemException)
			{
				return (false, exception);
			}
		}

		/// <summary>
		///     The containment relation only needs one additional item, for the proper containment.
		/// </summary>
		private bool IsAdditionalItemRelevant()
			=> !_equivalenceRelations.Includes(EquivalenceRelations.Contains) ||
			   (_equivalenceRelations.Includes(EquivalenceRelations.ContainsProperly) && _additionalItems.Count == 0);

		/// <summary>
		///     Additional items are no deviation for the containment relation, so they are not counted.
		/// </summary>
		private int CountAdditionalDeviations()
			=> _equivalenceRelations.Includes(EquivalenceRelations.Contains) ? 0 : _additionalItems.Count;

		/// <summary>
		///     Missing items are no deviation for the IsContainedIn relation, so they are not counted.
		/// </summary>
		private int CountMissingDeviations()
			=> _equivalenceRelations.Includes(EquivalenceRelations.IsContainedIn) ? 0 : _missingItems.Count;

		private IEnumerable<string> GetDeviations()
			=> AdditionalItemsError(_additionalItems, CreateItemFormatter());

		/// <summary>
		///     An unexpected and a missing item that format equally differ only in their runtime type.
		/// </summary>
		private Func<object?, string> CreateItemFormatter()
			=> GetItemFormatter(_additionalItems.Values.Cast<object?>(), _missingItems.Cast<object?>());

		protected abstract ValueTask<bool>
			AreConsideredEqual(int index, T value, T3 expected, IOptionsEquality<T2> options);
	}
}
