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

		/// <remarks>
		///     Only ordinal string equality and the default equality of a primitive are known to compare like
		///     <see cref="EqualityComparer{T}.Default" />: the default equality of other types also checks the kind of a
		///     <see cref="DateTime" /> and calls <see cref="object.Equals(object)" /> and
		///     <see cref="object.GetHashCode()" /> of the caller, which can throw or disagree with each other.
		/// </remarks>
		protected override ExpectedItemCounts<T>? CountExpectedItems(List<T> expected, IOptionsEquality<T2> options)
			=> options switch
			{
				StringEqualityOptions { ComparesByOrdinalEquality: true, } => new ExpectedItemCounts<T>(expected),
				ObjectEqualityOptions<T2> { UsesEqualsMatch: true, } when typeof(T).IsPrimitive
					=> new ExpectedItemCounts<T>(expected),
				_ => null,
			};
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

		/// <summary>
		///     The counts of the expected items, as long as every item was found among them, so that none was matched.
		/// </summary>
		private ExpectedItemCounts<T>? _expectedCounts;

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

		/// <remarks>
		///     An item that is found among the counted expected items is not matched, as long as all items before it
		///     were found as well.
		/// </remarks>
		public ValueTask<(bool, string?)>
			Verify(string it, T value, IOptionsEquality<T2> options, int maximumNumber)
		{
			if (GetExpectedCounts(options) is not { } expectedCounts)
			{
				return Match(it, value, options, maximumNumber);
			}

			return expectedCounts.TryFind(value)
				? new ValueTask<(bool, string?)>((false, null))
				: MatchAfterTheFoundItems(expectedCounts, it, value, options, maximumNumber);
		}

		private async ValueTask<(bool, string?)> MatchAfterTheFoundItems(ExpectedItemCounts<T> expectedCounts,
			string it, T value, IOptionsEquality<T2> options, int maximumNumber)
		{
			await MatchTheFoundItems(expectedCounts, it, options, maximumNumber);
			return await Match(it, value, options, maximumNumber);
		}

		/// <summary>
		///     Matches the items that were only counted so far, so that the matching continues as if every item had
		///     been matched.
		/// </summary>
		/// <remarks>
		///     Each of these items is equal to a distinct expected item, so none of them is a deviation.
		/// </remarks>
		private async ValueTask MatchTheFoundItems(ExpectedItemCounts<T> expectedCounts, string it,
			IOptionsEquality<T2> options, int maximumNumber)
		{
			// The matching ends the counting, also when no item was found.
			_ = GetMatching(options);
			_expectedCounts = null;
			foreach (T item in expectedCounts.FoundItems)
			{
				await Match(it, item, options, maximumNumber);
			}
		}

		/// <summary>
		///     The counts of the expected items, as long as no item has to be matched.
		/// </summary>
		/// <remarks>
		///     Only equality requires every item to be found among the expected items and the other way round. The
		///     options are only known once the items are compared.
		/// </remarks>
		private ExpectedItemCounts<T>? GetExpectedCounts(IOptionsEquality<T2> options)
		{
			if (_matching is not null || _equivalenceRelations.Includes(EquivalenceRelations.Contains) ||
			    _equivalenceRelations.Includes(EquivalenceRelations.IsContainedIn))
			{
				return null;
			}

			return _expectedCounts ??= CountExpectedItems(_expected, options);
		}

		/// <remarks>
		///     An item whose comparisons complete synchronously returns without a state machine.
		/// </remarks>
		private ValueTask<(bool, string?)> Match(string it, T value, IOptionsEquality<T2> options, int maximumNumber)
		{
			int index = _index++;
			ItemMatching<T, T3> matching = GetMatching(options);
			ValueTask<int> assigned = matching.Add(index, value, _lastAssigned);
			if (!assigned.IsCompletedSuccessfully)
			{
				return VerifyAsync(assigned, it, matching, maximumNumber);
			}

			_lastAssigned = assigned.Result;
			// Resolving each item right away keeps the earlier items matched, so the later ones are reported, and lets
			// the containment relation stop, once the item completes the assignment.
			ValueTask resolved = matching.ResolvePendingItems();
			return resolved.IsCompletedSuccessfully
				? new ValueTask<(bool, string?)>(CountTheAdditionalItems(it, matching, maximumNumber))
				: CountTheAdditionalItemsAsync(resolved, it, matching, maximumNumber);
		}

		private async ValueTask<(bool, string?)> VerifyAsync(ValueTask<int> assigned, string it,
			ItemMatching<T, T3> matching, int maximumNumber)
		{
			_lastAssigned = await assigned;
			await matching.ResolvePendingItems();
			return CountTheAdditionalItems(it, matching, maximumNumber);
		}

		private async ValueTask<(bool, string?)> CountTheAdditionalItemsAsync(ValueTask resolved, string it,
			ItemMatching<T, T3> matching, int maximumNumber)
		{
			await resolved;
			return CountTheAdditionalItems(it, matching, maximumNumber);
		}

		/// <summary>
		///     Aborts early, when more items are additional than can be listed.
		/// </summary>
		/// <remarks>
		///     Additional items are no deviations for the containment relation.
		/// </remarks>
		private (bool, string?) CountTheAdditionalItems(string it, ItemMatching<T, T3> matching, int maximumNumber)
		{
			if (CountAdditionalDeviations() > 2L * maximumNumber)
			{
				_missingItems = matching.UnmatchedExpectedItems();
				return (true, TooManyDeviationsError(it, maximumNumber, GetDeviations()));
			}

			return (false, null);
		}

		/// <remarks>
		///     When every item was found among the counted expected items and none of them is left, the expectation is
		///     met.
		/// </remarks>
		public ValueTask<(bool, string?)>
			VerifyComplete(string it, IOptionsEquality<T2> options, int maximumNumber)
		{
			if (GetExpectedCounts(options) is not { } expectedCounts)
			{
				return CompleteTheMatching(it, options, maximumNumber);
			}

			return expectedCounts.HasFoundAll
				? new ValueTask<(bool, string?)>((false, null))
				: CompleteAfterTheFoundItems(expectedCounts, it, options, maximumNumber);
		}

		private async ValueTask<(bool, string?)> CompleteAfterTheFoundItems(ExpectedItemCounts<T> expectedCounts,
			string it, IOptionsEquality<T2> options, int maximumNumber)
		{
			await MatchTheFoundItems(expectedCounts, it, options, maximumNumber);
			return await CompleteTheMatching(it, options, maximumNumber);
		}

		/// <remarks>
		///     Without pending items that have to be awaited, it returns without a state machine.
		/// </remarks>
		private ValueTask<(bool, string?)>
			CompleteTheMatching(string it, IOptionsEquality<T2> options, int maximumNumber)
		{
			ItemMatching<T, T3> matching = GetMatching(options);
			ValueTask resolved = matching.ResolvePendingItems();
			return resolved.IsCompletedSuccessfully
				? new ValueTask<(bool, string?)>(Complete(it, matching, options, maximumNumber))
				: CompleteAsync(resolved, it, matching, options, maximumNumber);
		}

		private async ValueTask<(bool, string?)> CompleteAsync(ValueTask resolved, string it,
			ItemMatching<T, T3> matching, IOptionsEquality<T2> options, int maximumNumber)
		{
			await resolved;
			return Complete(it, matching, options, maximumNumber);
		}

		private (bool, string?) Complete(string it, ItemMatching<T, T3> matching, IOptionsEquality<T2> options,
			int maximumNumber)
		{
			// When all expected items are matched, none is missing, so only the ones that an aborted item kept are
			// replaced.
			if (!matching.HasMatchedAllExpectedItems || _missingItems.Count > 0)
			{
				_missingItems = matching.UnmatchedExpectedItems();
			}

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

		/// <summary>
		///     Counts the <paramref name="expected" /> items, when the <paramref name="options" /> compare an item with
		///     an expected item like <see cref="EqualityComparer{T}.Default" /> does; otherwise
		///     <see langword="null" />.
		/// </summary>
		protected virtual ExpectedItemCounts<T>? CountExpectedItems(List<T3> expected, IOptionsEquality<T2> options)
			=> null;
	}
}
