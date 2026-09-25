using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Helpers;

namespace aweXpect.Options;

public partial class CollectionMatchOptions
{
	private sealed class AnyOrderIgnoreDuplicatesCollectionMatcher<T, T2>(
		EquivalenceRelations equivalenceRelation,
		IEnumerable<T> expected)
		: AnyOrderIgnoreDuplicatesCollectionMatcherBase<T, T2, T>(
			equivalenceRelation,
			expected.Distinct().ToList())
		where T : T2
	{
		protected override bool RepeatingAMatchedExpectedItemIsADuplicate => true;

		protected override ValueTask<bool> AreConsideredEqual(T value, T expected, IOptionsEquality<T2> options)
			=> options.AreConsideredEqual(value, expected);
	}

	private sealed class AnyOrderIgnoreDuplicatesFromExpectationCollectionMatcher<T, T2>(
		EquivalenceRelations equivalenceRelation,
		IEnumerable<ExpectationItem<T>> expected)
		: AnyOrderIgnoreDuplicatesCollectionMatcherBase<T, T2, ExpectationItem<T>>(
			equivalenceRelation,
			expected.Distinct(new ExpectationItemEqualityComparer<T>()).ToList())
		where T : T2
	{
		protected override ValueTask<bool>
			AreConsideredEqual(T value, ExpectationItem<T> expected, IOptionsEquality<T2> options)
			=> expected.IsMetBy(value);
	}

	private sealed class AnyOrderIgnoreDuplicatesFromPredicateCollectionMatcher<T, T2>(
		EquivalenceRelations equivalenceRelation,
		IEnumerable<Expression<Func<T, bool>>> expected)
		: AnyOrderIgnoreDuplicatesCollectionMatcherBase<T, T2, Expression<Func<T, bool>>>(
			equivalenceRelation,
			expected.Distinct(new ExpressionEqualityComparer<T, bool>()).ToList())
		where T : T2
	{
		protected override ValueTask<bool> AreConsideredEqual(T value, Expression<Func<T, bool>> expected,
			IOptionsEquality<T2> options)
			=> new ValueTask<bool>(UserCode.Invoke(expected.Compile(), value));
	}

	private abstract class AnyOrderIgnoreDuplicatesCollectionMatcherBase<T, T2, T3> : ICollectionMatcher<T, T2>
		where T : T2
	{
		private readonly Dictionary<int, T> _additionalItems = new();
		private readonly EquivalenceRelations _equivalenceRelations;
		private readonly List<T3> _matchedExpectedItems = new();
		private readonly List<T3> _missingItems;
		private readonly int _totalExpectedCount;
		private readonly HashSet<T> _uniqueItems = new();
		private int _index;

		protected AnyOrderIgnoreDuplicatesCollectionMatcherBase(EquivalenceRelations equivalenceRelation,
			List<T3> expected)
		{
			_equivalenceRelations = equivalenceRelation;
			_missingItems = expected;
			_totalExpectedCount = _missingItems.Count;
		}

		/// <summary>
		///     Only expected values are compared using the equality options, so an item that matches an expected value
		///     that an earlier item already matched repeats it, e.g. when ignoring the casing; a predicate or an
		///     expectation can also match unrelated items.
		/// </summary>
		protected virtual bool RepeatingAMatchedExpectedItemIsADuplicate => false;

		public async ValueTask<(bool, string?)>
			Verify(string it, T value, IOptionsEquality<T2> options, int maximumNumber)
		{
			if (_uniqueItems.Contains(value))
			{
				_index++;
				return (false, null);
			}

			int missingIndex = await FindTheMissingItem(value, options);
			if (missingIndex >= 0)
			{
				if (RepeatingAMatchedExpectedItemIsADuplicate)
				{
					_matchedExpectedItems.Add(_missingItems[missingIndex]);
				}

				_missingItems.RemoveAt(missingIndex);
			}
			else
			{
				// Only an item that would be a deviation is compared with every matched expected item.
				if (!_equivalenceRelations.HasFlag(EquivalenceRelations.Contains) &&
				    RepeatingAMatchedExpectedItemIsADuplicate &&
				    await Any(_matchedExpectedItems, expected => AreConsideredEqual(value, expected, options)))
				{
					_index++;
					return (false, null);
				}

				_additionalItems.Add(_index, value);
			}

			_uniqueItems.Add(value);
			_index++;

			return _additionalItems.Count > 2 * maximumNumber
				? (true, TooManyDeviationsError(it, maximumNumber, GetDeviations()))
				: (false, null);
		}

		/// <returns>The index of the first missing item that the <paramref name="value" /> matches, otherwise <c>-1</c>.</returns>
		private async ValueTask<int>
			FindTheMissingItem(T value, IOptionsEquality<T2> options)
		{
			for (int i = 0; i < _missingItems.Count; i++)
			{
				if (await AreConsideredEqual(value, _missingItems[i], options))
				{
					return i;
				}
			}

			return -1;
		}

		public ValueTask<(bool, string?)>
			VerifyComplete(string it, IOptionsEquality<T2> options, int maximumNumber)
		{
			if (_missingItems.Count + _additionalItems.Count > 2 * maximumNumber)
			{
				string tooManyDeviations = TooManyDeviationsError(it, maximumNumber, GetDeviations());
				return new ValueTask<(bool, string?)>((true, tooManyDeviations));
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
				errors.AddRange(MissingItemsError(_totalExpectedCount, _missingItems, _equivalenceRelations, true, formatItem));
			}
			else if (_equivalenceRelations.HasFlag(EquivalenceRelations.IsContainedInProperly) && !_missingItems.Any())
			{
				errors.Add("contained all expected items");
			}

			string? error = ReturnErrorString(it, errors);
			return new ValueTask<(bool, string?)>((error != null, error));
		}

		/// <summary>
		///     Additional items are no deviation for the containment relation, so they are left out.
		/// </summary>
		private IEnumerable<string> GetDeviations()
			=> _equivalenceRelations.HasFlag(EquivalenceRelations.Contains)
				? []
				: AdditionalItemsError(_additionalItems, CreateItemFormatter());

		/// <summary>
		///     An unexpected and a missing item that format equally differ only in their runtime type.
		/// </summary>
		private Func<object?, string> CreateItemFormatter()
			=> GetItemFormatter(_additionalItems.Values.Cast<object?>(), _missingItems.Cast<object?>());

		protected abstract ValueTask<bool>
			AreConsideredEqual(T value, T3 expected, IOptionsEquality<T2> options);
	}
}
