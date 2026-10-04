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
		protected override ValueTask<bool> AreConsideredEqual(int index, T value, T expected,
			IOptionsEquality<T2> options)
			=> options.AreConsideredEqual(value, expected);
	}

	private sealed class AnyOrderFromExpectationCollectionMatcher<T, T2>(
		EquivalenceRelations equivalenceRelation,
		IEnumerable<ExpectationItem<T>> expected)
		: AnyOrderCollectionMatcherBase<T, T2, ExpectationItem<T>>(equivalenceRelation, expected)
		where T : T2
	{
		protected override ValueTask<bool> AreConsideredEqual(int index, T value, ExpectationItem<T> expected,
			IOptionsEquality<T2> options)
			=> expected.IsMetBy(value, index);
	}

	private sealed class AnyOrderFromPredicateCollectionMatcher<T, T2>(
		EquivalenceRelations equivalenceRelation,
		IEnumerable<Expression<Func<T, bool>>> expected)
		: AnyOrderCollectionMatcherBase<T, T2, Expression<Func<T, bool>>>(equivalenceRelation, expected)
		where T : T2
	{
		private readonly CompiledPredicates<T> _predicates = new();

		protected override ValueTask<bool> AreConsideredEqual(int index, T value, Expression<Func<T, bool>> expected,
			IOptionsEquality<T2> options)
			=> _predicates.Invoke(expected, value, index);
	}

	/// <summary>
	///     Matches each subject item with a distinct expected item.
	/// </summary>
	private abstract class AnyOrderCollectionMatcherBase<T, T2, T3> : ICollectionMatcher<T, T2>
		where T : T2
	{
		private readonly Dictionary<int, T> _additionalItems = new();
		private readonly EquivalenceRelations _equivalenceRelations;
		private readonly List<T3> _expected;
		private int _index;

		/// <summary>
		///     The expected item that the previous item was assigned to, or <c>-1</c> when it was not assigned.
		/// </summary>
		/// <remarks>
		///     Its neighbours are tried first, so that a subject in or against the expected order finds each match without
		///     comparing it with all free expected items.
		/// </remarks>
		private int _lastAssigned = -1;

		private ItemMatching<T, T3>? _matching;
		private List<T3> _missingItems = new();

		protected AnyOrderCollectionMatcherBase(EquivalenceRelations equivalenceRelation, IEnumerable<T3> expected)
		{
			_equivalenceRelations = equivalenceRelation;
			_expected = expected.ToList();
		}

		/// <inheritdoc />
		/// <remarks>
		///     Once an item is assigned to every expected item, the containment relation is met, and properly met as soon
		///     as there is also an additional item.
		/// </remarks>
		public bool IsDetermined
			=> _equivalenceRelations.Includes(EquivalenceRelations.Contains) &&
			   _matching?.HasMatchedAllExpectedItems == true &&
			   (!_equivalenceRelations.Includes(EquivalenceRelations.ContainsProperly) || _additionalItems.Count > 0);

		public async ValueTask<(bool, string?)>
			Verify(string it, T value, IOptionsEquality<T2> options, int maximumNumber)
		{
			int index = _index++;
			ItemMatching<T, T3> matching = GetMatching(options);
			_lastAssigned = await matching.Add(index, value, _lastAssigned);
			if (_equivalenceRelations.Includes(EquivalenceRelations.Contains))
			{
				// Additional items are no deviations, so the reassignment is deferred until it decides the result.
				return (false, null);
			}

			// Resolving each item right away keeps the earlier items matched, so the later ones are reported.
			await matching.ResolvePendingItems();
			if (_additionalItems.Count > 2L * maximumNumber)
			{
				_missingItems = matching.UnmatchedExpectedItems();
				return (true, TooManyDeviationsError(it, maximumNumber, GetDeviations()));
			}

			return (false, null);
		}

		public async ValueTask<(bool, string?)>
			VerifyComplete(string it, IOptionsEquality<T2> options, int maximumNumber)
		{
			ItemMatching<T, T3> matching = GetMatching(options);
			await matching.ResolvePendingItems();
			_missingItems = matching.UnmatchedExpectedItems();
			if (!HasDeviations())
			{
				return (false, null);
			}

			// For the containment relation, all deviations are missing items, which are known completely here.
			if (!_equivalenceRelations.Includes(EquivalenceRelations.Contains) &&
			    CountAdditionalDeviations() + CountMissingDeviations() > 2L * maximumNumber)
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
				errors.AddRange(MissingItemsError(_expected.Count, _missingItems, _equivalenceRelations,
					false, formatItem, options, maximumNumber));
			}
			else if (_equivalenceRelations.Includes(EquivalenceRelations.IsContainedInProperly) && !_missingItems.Any())
			{
				errors.Add("contained all expected items");
			}

			string? error = ReturnErrorString(it, errors);
			return (error != null, error);
		}

		/// <summary>
		///     Whether <see cref="VerifyComplete" /> reports an error, without creating the error texts, as a met
		///     expectation has none.
		/// </summary>
		private bool HasDeviations()
		{
			bool hasAdditionalItemError = _equivalenceRelations.Includes(EquivalenceRelations.Contains)
				? _equivalenceRelations.Includes(EquivalenceRelations.ContainsProperly) && _additionalItems.Count == 0
				: _additionalItems.Count > 0;
			bool hasMissingItemError = _equivalenceRelations.Includes(EquivalenceRelations.IsContainedIn)
				? _equivalenceRelations.Includes(EquivalenceRelations.IsContainedInProperly) && _missingItems.Count == 0
				: _expected.Count > 0 && _missingItems.Count > 0;
			return hasAdditionalItemError || hasMissingItemError;
		}

		/// <summary>
		///     The options are only known once the items are compared.
		/// </summary>
		private ItemMatching<T, T3> GetMatching(IOptionsEquality<T2> options)
			=> _matching ??= new ItemMatching<T, T3>(_expected, (index, value, expected)
				=> AreConsideredEqual(index, value, expected, options), _additionalItems);

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

		/// <summary>
		///     Compares the <paramref name="value" /> at the <paramref name="index" /> with the
		///     <paramref name="expected" /> item.
		/// </summary>
		protected abstract ValueTask<bool>
			AreConsideredEqual(int index, T value, T3 expected, IOptionsEquality<T2> options);
	}
}
