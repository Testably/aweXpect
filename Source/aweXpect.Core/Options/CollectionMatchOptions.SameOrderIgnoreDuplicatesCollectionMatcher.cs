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
	private sealed class SameOrderIgnoreDuplicatesCollectionMatcher<T, T2>(
		EquivalenceRelations equivalenceRelation,
		IEnumerable<T> expected,
		bool ignoreInterspersedItems)
		: SameOrderIgnoreDuplicatesCollectionMatcherBase<T, T2, T>(
			equivalenceRelation,
			expected,
			null,
			ignoreInterspersedItems)
		where T : T2
	{
		private readonly HashSet<T> _expectedValues = new(expected);

		protected override bool IsOneToOne => false;

		protected override bool IsEqualToAnExpectedItem(T value) => _expectedValues.Contains(value);

		protected override ValueTask<bool> AreConsideredEqual(int index, T value, T expected,
			IOptionsEquality<T2> options)
			=> options.AreConsideredEqual(value, expected);
	}

	private sealed class SameOrderIgnoreDuplicatesFromExpectationCollectionMatcher<T, T2>(
		EquivalenceRelations equivalenceRelation,
		IEnumerable<ExpectationItem<T>> expected,
		bool ignoreInterspersedItems)
		: SameOrderIgnoreDuplicatesCollectionMatcherBase<T, T2, ExpectationItem<T>>(
			equivalenceRelation,
			expected,
			new ExpectationItemEqualityComparer<T>(),
			ignoreInterspersedItems)
		where T : T2
	{
		protected override ValueTask<bool> AreConsideredEqual(int index, T value, ExpectationItem<T> expected,
			IOptionsEquality<T2> options)
			=> expected.IsMetBy(value, index);
	}

	private sealed class SameOrderIgnoreDuplicatesFromPredicateCollectionMatcher<T, T2>(
		EquivalenceRelations equivalenceRelation,
		IEnumerable<Expression<Func<T, bool>>> expected,
		bool ignoreInterspersedItems)
		: SameOrderIgnoreDuplicatesCollectionMatcherBase<T, T2, Expression<Func<T, bool>>>(
			equivalenceRelation,
			expected,
			new ExpressionEqualityComparer<T, bool>(),
			ignoreInterspersedItems)
		where T : T2
	{
		private readonly CompiledPredicates<T> _predicates = new();

		protected override ValueTask<bool> AreConsideredEqual(int index, T value, Expression<Func<T, bool>> expected,
			IOptionsEquality<T2> options)
			=> _predicates.Invoke(expected, value, index);
	}

	/// <summary>
	///     Stores the distinct items, so that <see cref="DistinctItemsInOrder" /> decides whether they match and
	///     <see cref="InOrderMismatch" /> describes why they do not.
	/// </summary>
	private abstract class SameOrderIgnoreDuplicatesCollectionMatcherBase<T, T2, T3> : ICollectionMatcher<T, T2>
		where T : T2
	{
		private readonly EquivalenceRelations _equivalenceRelations;
		private readonly T3[] _expectedDistinctItems;
		private readonly int[] _expectedIds;
		private readonly T3[] _expectedItems;
		private readonly List<int> _firstIndexOfSubjectItem = new();
		private readonly bool _ignoreInterspersedItems;
		private readonly ItemIds<T> _subjectIds = new(null);
		private readonly List<int> _subjectIdAt = new();
		private int _followedExpectedItems;
		private int _lastMatchedExpectedId = -1;
		private int _subjectItemsMatchingNothing;

		protected SameOrderIgnoreDuplicatesCollectionMatcherBase(EquivalenceRelations equivalenceRelation,
			IEnumerable<T3> expected,
			IEqualityComparer<T3>? comparer,
			bool ignoreInterspersedItems)
		{
			_equivalenceRelations = equivalenceRelation;
			_ignoreInterspersedItems = ignoreInterspersedItems;
			_expectedItems = expected.ToArray();
			ItemIds<T3> expectedIds = new(comparer);
			_expectedIds = _expectedItems.Select(item => expectedIds.GetOrAdd(item, out _)).ToArray();
			_expectedDistinctItems = expectedIds.Items.ToArray();
		}

		/// <summary>
		///     Predicates and expectations need one distinct item each; expected values form a set, so items that match the
		///     same value are duplicates, e.g. when ignoring the casing.
		/// </summary>
		protected virtual bool IsOneToOne => true;

		/// <inheritdoc />
		/// <remarks>
		///     Once the items so far contain the expected items, further items cannot change the containment relation, and
		///     the proper containment only needs an additional item as well.
		/// </remarks>
		public bool IsDetermined { get; private set; }

		public async ValueTask<(bool, string?)>
			Verify(string it, T value, IOptionsEquality<T2> options, int maximumNumber)
		{
			int index = _subjectIdAt.Count;
			int subjectId = _subjectIds.GetOrAdd(value, out bool isNew);
			_subjectIdAt.Add(subjectId);
			if (isNew)
			{
				_firstIndexOfSubjectItem.Add(index);
			}

			if (_equivalenceRelations.HasFlag(EquivalenceRelations.Contains))
			{
				await CheckWhetherTheResultIsDetermined(subjectId, options);
				return (false, null);
			}

			if (isNew && !IsEqualToAnExpectedItem(value) && !await MatchesAnExpectedItem(subjectId, options))
			{
				_subjectItemsMatchingNothing++;
			}

			return CountTheCertainlyUnexpectedItems() > 2L * maximumNumber
				? (true, TooManyDeviationsError(it, maximumNumber,
					(await FindTheDeviations(options)).ListDeviations(_equivalenceRelations, options)))
				: (false, null);
		}

		public async ValueTask<(bool, string?)>
			VerifyComplete(string it, IOptionsEquality<T2> options, int maximumNumber)
		{
			DistinctItemsInOrder order = CreateOrder(options);
			bool requiresAdditionalItem = _equivalenceRelations.HasFlag(EquivalenceRelations.ContainsProperly) ||
			                              _equivalenceRelations.HasFlag(EquivalenceRelations.IsContainedInProperly);
			if (await order.IsInOrder() && (!requiresAdditionalItem || await order.HasAdditionalItem()))
			{
				return (false, null);
			}

			InOrderDeviations<T, T3> deviations = await FindTheDeviations(options);
			string? error = deviations.GetError(it, _equivalenceRelations, true, _expectedDistinctItems.Length,
				options, maximumNumber);
			if (_equivalenceRelations == EquivalenceRelations.Equivalent && deviations.MatchesInAnyOrder)
			{
				error += Environment.NewLine + ItemsMatchInADifferentOrderHint;
			}

			return (true, error);
		}

		/// <summary>
		///     Avoids comparing each item with the expected items, when it is known to match one of them.
		/// </summary>
		/// <remarks>
		///     This only decides, whether the comparison can abort early, so it may err towards a match.
		/// </remarks>
		protected virtual bool IsEqualToAnExpectedItem(T value) => false;

		/// <summary>
		///     The decision is only consulted, once the items so far followed all expected items in order, so that it does
		///     not run for every item; a run or a placement of the expected items implies such a sequence.
		/// </summary>
		private async ValueTask CheckWhetherTheResultIsDetermined(int subjectId, IOptionsEquality<T2> options)
		{
			while (_followedExpectedItems < _expectedDistinctItems.Length &&
			       await IsMatch(subjectId, _followedExpectedItems, options))
			{
				_followedExpectedItems++;
			}

			if (IsDetermined || _followedExpectedItems < _expectedDistinctItems.Length)
			{
				return;
			}

			DistinctItemsInOrder order = CreateOrder(options);
			IsDetermined = await order.IsInOrder() &&
			               (!_equivalenceRelations.HasFlag(EquivalenceRelations.ContainsProperly) ||
			                await order.HasAdditionalItem());
		}

		/// <summary>
		///     A subject item that matches no expected item is unexpected regardless of the other items; one-to-one, so
		///     are the items beyond the number of expected items.
		/// </summary>
		private long CountTheCertainlyUnexpectedItems()
		{
			int beyondTheExpectedItems = _subjectIds.Items.Count - _subjectItemsMatchingNothing -
			                             _expectedDistinctItems.Length;
			return _subjectItemsMatchingNothing + (IsOneToOne ? Math.Max(0, beyondTheExpectedItems) : 0);
		}

		private async ValueTask<bool> MatchesAnExpectedItem(int subjectId, IOptionsEquality<T2> options)
		{
			int expectedId = await FindNear(_lastMatchedExpectedId, _expectedDistinctItems.Length,
				id => IsMatch(subjectId, id, options));
			if (expectedId < 0)
			{
				return false;
			}

			_lastMatchedExpectedId = expectedId;
			return true;
		}

		/// <summary>
		///     The containment searches the subject in the expected items, the other relations search the expected items
		///     in the subject.
		/// </summary>
		private async ValueTask<InOrderDeviations<T, T3>> FindTheDeviations(IOptionsEquality<T2> options)
		{
			if (_equivalenceRelations.HasFlag(EquivalenceRelations.IsContainedIn))
			{
				InOrderMismatch searchedInExpected = await InOrderMismatch.Explain(_expectedIds,
					_expectedDistinctItems.Length, _subjectIds.Items.Count,
					(expectedId, subjectId) => IsMatch(subjectId, expectedId, options), IsOneToOne, GetOrderMatch());
				return InOrderDeviations<T, T3>.From(searchedInExpected, true,
					subjectId => (_firstIndexOfSubjectItem[subjectId], _subjectIds.Items[subjectId]),
					position => _expectedItems[position]);
			}

			InOrderMismatch searchedInSubject = await InOrderMismatch.Explain(_subjectIdAt.ToArray(),
				_subjectIds.Items.Count, _expectedDistinctItems.Length,
				(subjectId, expectedId) => IsMatch(subjectId, expectedId, options), IsOneToOne, GetOrderMatch());
			return InOrderDeviations<T, T3>.From(searchedInSubject, false,
				position => (position, _subjectIds.Items[_subjectIdAt[position]]),
				expectedId => _expectedDistinctItems[expectedId]);
		}

		/// <summary>
		///     The containment searches the subject in the expected items, the other relations search the expected items
		///     in the subject.
		/// </summary>
		private DistinctItemsInOrder CreateOrder(IOptionsEquality<T2> options)
			=> _equivalenceRelations.HasFlag(EquivalenceRelations.IsContainedIn)
				? new DistinctItemsInOrder(_expectedIds, _expectedDistinctItems.Length, _subjectIds.Items.Count,
					(expectedId, subjectId) => IsMatch(subjectId, expectedId, options), IsOneToOne, GetOrderMatch())
				: new DistinctItemsInOrder(_subjectIdAt.ToArray(), _subjectIds.Items.Count,
					_expectedDistinctItems.Length, (subjectId, expectedId) => IsMatch(subjectId, expectedId, options),
					IsOneToOne, GetOrderMatch());

		private OrderMatch GetOrderMatch()
		{
			if ((_equivalenceRelations & (EquivalenceRelations.Contains | EquivalenceRelations.IsContainedIn)) == 0)
			{
				return OrderMatch.Equal;
			}

			return _ignoreInterspersedItems ? OrderMatch.Subsequence : OrderMatch.Contiguous;
		}

		private ValueTask<bool> IsMatch(int subjectId, int expectedId, IOptionsEquality<T2> options)
			=> AreConsideredEqual(_firstIndexOfSubjectItem[subjectId], _subjectIds.Items[subjectId],
				_expectedDistinctItems[expectedId], options);

		/// <summary>
		///     Compares the <paramref name="value" /> at the <paramref name="index" /> with the
		///     <paramref name="expected" /> item.
		/// </summary>
		/// <remarks>
		///     A duplicate is only compared once, so the <paramref name="index" /> is the one of its first occurrence.
		/// </remarks>
		protected abstract ValueTask<bool>
			AreConsideredEqual(int index, T value, T3 expected, IOptionsEquality<T2> options);
	}

	/// <summary>
	///     Numbers the distinct items in the order of their first occurrence.
	/// </summary>
	private sealed class ItemIds<TItem>(IEqualityComparer<TItem>? comparer)
	{
		private readonly Dictionary<Key, int> _ids = new(new KeyComparer(comparer ?? EqualityComparer<TItem>.Default));

		public List<TItem> Items { get; } = new();

		public int GetOrAdd(TItem item, out bool isNew)
		{
			isNew = !_ids.TryGetValue(new Key(item), out int id);
			if (isNew)
			{
				id = Items.Count;
				_ids.Add(new Key(item), id);
				Items.Add(item);
			}

			return id;
		}

		/// <summary>
		///     Wraps the item, because a dictionary key cannot be <see langword="null" />.
		/// </summary>
		private readonly struct Key(TItem item)
		{
			public TItem Item { get; } = item;
		}

		private sealed class KeyComparer(IEqualityComparer<TItem> comparer) : IEqualityComparer<Key>
		{
			public bool Equals(Key x, Key y)
				=> x.Item is null ? y.Item is null : y.Item is not null && comparer.Equals(x.Item, y.Item);

			public int GetHashCode(Key obj)
				=> obj.Item is null ? 0 : comparer.GetHashCode(obj.Item);
		}
	}
}
