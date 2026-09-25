using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using aweXpect.Core;

namespace aweXpect.Options;

public partial class CollectionMatchOptions
{
	private sealed class AnyOrderCollectionMatcher<T, T2>(
		EquivalenceRelations equivalenceRelation,
		IEnumerable<T> expected)
		: AnyOrderCollectionMatcherBase<T, T2, T>(equivalenceRelation, expected)
		where T : T2
	{
		protected override ValueTask<bool> AreConsideredEqual(T value, T expected, IOptionsEquality<T2> options)
			=> options.AreConsideredEqual(value, expected);
	}

	private sealed class AnyOrderFromExpectationCollectionMatcher<T, T2>(
		EquivalenceRelations equivalenceRelation,
		IEnumerable<ExpectationItem<T>> expected)
		: AnyOrderCollectionMatcherBase<T, T2, ExpectationItem<T>>(equivalenceRelation, expected)
		where T : T2
	{
		protected override ValueTask<bool>
			AreConsideredEqual(T value, ExpectationItem<T> expected, IOptionsEquality<T2> options)
			=> expected.IsMetBy(value);
	}

	private sealed class AnyOrderFromPredicateCollectionMatcher<T, T2>(
		EquivalenceRelations equivalenceRelation,
		IEnumerable<Expression<Func<T, bool>>> expected)
		: AnyOrderCollectionMatcherBase<T, T2, Expression<Func<T, bool>>>(equivalenceRelation, expected)
		where T : T2
	{
		protected override ValueTask<bool> AreConsideredEqual(T value, Expression<Func<T, bool>> expected,
			IOptionsEquality<T2> options)
			=> new ValueTask<bool>(expected.Compile().Invoke(value));
	}

	private abstract class AnyOrderCollectionMatcherBase<T, T2, T3> : ICollectionMatcher<T, T2>
		where T : T2
	{
		private readonly Dictionary<int, T> _additionalItems = new();
		private readonly EquivalenceRelations _equivalenceRelations;
		private readonly List<T3> _missingItems;
		private readonly int _totalExpectedCount;
		private int _index;

		protected AnyOrderCollectionMatcherBase(EquivalenceRelations equivalenceRelation, IEnumerable<T3> expected)
		{
			_equivalenceRelations = equivalenceRelation;
			_missingItems = expected.ToList();
			_totalExpectedCount = _missingItems.Count;
		}

		public async ValueTask<(bool, string?)>
			Verify(string it, T value, IOptionsEquality<T2> options, int maximumNumber)
		{
			if (await All(_missingItems, e => AreConsideredEqual(value, e, options), true))
			{
				_additionalItems.Add(_index, value);
			}

			await RemoveFirst(_missingItems, e => AreConsideredEqual(value, e, options));
			_index++;
			return _additionalItems.Count > 2 * maximumNumber
				? (true, TooManyDeviationsError(it, maximumNumber, GetDeviations()))
				: (false, null);
		}

		public ValueTask<(bool, string?)>
			VerifyComplete(string it, IOptionsEquality<T2> options, int maximumNumber)
		{
			if (_additionalItems.Count + _missingItems.Count > 2 * maximumNumber)
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
				errors.AddRange(MissingItemsError(_totalExpectedCount, _missingItems, _equivalenceRelations, false, formatItem));
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
