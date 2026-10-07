using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using aweXpect.Core;

namespace aweXpect.Options;

public partial class CollectionMatchOptions
{
	private sealed class SameOrderIgnoreDuplicatesCollectionMatcher<T, T2>(
		EquivalenceRelations equivalenceRelation,
		IEnumerable<T> expected,
		bool ignoreInterspersedItems,
		int[]? dimensions)
		: SameOrderIgnoreDuplicatesCollectionMatcherBase<T, T2, T>(
			equivalenceRelation,
			expected,
			EqualityComparer<T>.Default,
			ignoreInterspersedItems,
			dimensions)
		where T : T2
	{
		private HashSet<T>? _expectedValues;

		/// <remarks>
		///     The set is only built once a containment relation asks for it, as equality never does.
		/// </remarks>
		protected override bool IsEqualToAnExpectedItem(T value)
			=> (_expectedValues ??= new HashSet<T>(ExpectedItems)).Contains(value);

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
			null,
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
			null,
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
		private readonly IEqualityComparer<T3>? _comparer;

		/// <summary>
		///     The dimensions of a subject that is an array of rank greater than one.
		/// </summary>
		private readonly int[]? _dimensions;

		private readonly EquivalenceRelations _equivalenceRelations;
		private readonly List<int> _firstIndexOfSubjectItem = new();
		private readonly bool _ignoreInterspersedItems;
		private readonly List<int> _subjectIdAt = new();
		private readonly ItemIds<T> _subjectIds = new(null);
		private bool _areExpectedItemsUnique;
		private T3[] _expectedDistinctItems = [];
		private int[] _expectedIds = [];
		private int _followedExpectedItems;
		private int _lastMatchedExpectedId = -1;
		private bool? _mergesEqualItems;
		private int _subjectItemsMatchingNothing;

		/// <remarks>
		///     The <paramref name="comparer" /> merges equal expected values, when the comparison cannot tell them apart;
		///     predicates and expectations cannot be compared with each other, so without it each expected item stands for
		///     itself.
		/// </remarks>
		protected SameOrderIgnoreDuplicatesCollectionMatcherBase(EquivalenceRelations equivalenceRelation,
			IEnumerable<T3> expected,
			IEqualityComparer<T3>? comparer,
			bool ignoreInterspersedItems,
			int[]? dimensions = null)
		{
			_equivalenceRelations = equivalenceRelation;
			_ignoreInterspersedItems = ignoreInterspersedItems;
			_dimensions = dimensions;
			ExpectedItems = expected as T3[] ?? expected.ToArray();
			_comparer = comparer;
		}

		protected T3[] ExpectedItems { get; }

		/// <inheritdoc />
		/// <remarks>
		///     Once the items so far contain the expected items, further items cannot change the containment relation, and
		///     the proper containment only needs an additional item as well.
		/// </remarks>
		public bool IsDetermined { get; private set; }

		public async ValueTask<(bool, string?)>
			Verify(string it, T value, IOptionsEquality<T2> options, int maximumNumber)
		{
			bool mergesEqualItems = MergesEqualItems(options);
			int index = _subjectIdAt.Count;
			int subjectId = _subjectIds.GetOrAdd(value, out bool isNew);
			if (!isNew && !mergesEqualItems && !IsIdenticalToEqualValues(value) &&
			    !await MatchesTheSameExpectedItems(subjectId, index, value, options))
			{
				subjectId = _subjectIds.Add(value);
				isNew = true;
			}

			_subjectIdAt.Add(subjectId);
			if (isNew)
			{
				_firstIndexOfSubjectItem.Add(index);
			}

			if (_equivalenceRelations.Includes(EquivalenceRelations.Contains))
			{
				await CheckWhetherTheResultIsDetermined(subjectId, options);
				return (false, null);
			}

			if (isNew && !IsEqualToAnExpectedItem(value) && !await MatchesAnExpectedItem(subjectId, options))
			{
				_subjectItemsMatchingNothing++;
			}

			return _subjectItemsMatchingNothing > 2L * maximumNumber
				? (true, TooManyDeviationsError(it, maximumNumber,
					(await FindTheDeviations(options)).ListDeviations(_equivalenceRelations, options)))
				: (false, null);
		}

		public async ValueTask<(bool, string?)>
			VerifyComplete(string it, IOptionsEquality<T2> options, int maximumNumber)
		{
			MergesEqualItems(options);
			DistinctItemsInOrder order = CreateOrder(options);
			bool requiresAdditionalItem = _equivalenceRelations.Includes(EquivalenceRelations.ContainsProperly) ||
			                              _equivalenceRelations.Includes(EquivalenceRelations.IsContainedInProperly);
			if (await order.IsInOrder() && (!requiresAdditionalItem || await order.HasAdditionalItem()))
			{
				return (false, null);
			}

			InOrderDeviations<T, T3> deviations = await FindTheDeviations(options);
			string? error = deviations.GetError(it, _equivalenceRelations, _areExpectedItemsUnique,
				_expectedDistinctItems.Length, options, maximumNumber);
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
		///     Whether all equal items are merged without comparing them, which depends on the <paramref name="options" />.
		/// </summary>
		/// <remarks>
		///     Otherwise only identical items are merged, and another item only with an earlier equal item that matches
		///     the same expected items.
		/// </remarks>
		private bool MergesEqualItems(IOptionsEquality<T2> options)
		{
			if (_mergesEqualItems is { } mergesEqualItems)
			{
				return mergesEqualItems;
			}

			mergesEqualItems = CanMergeEqualItems<T, T2>(options);
			_areExpectedItemsUnique = _comparer is not null;
			if (_comparer is null)
			{
				_expectedIds = Enumerable.Range(0, ExpectedItems.Length).ToArray();
				_expectedDistinctItems = ExpectedItems;
			}
			else
			{
				NumberTheExpectedValues(_comparer, mergesEqualItems);
			}

			_mergesEqualItems = mergesEqualItems;
			return mergesEqualItems;
		}

		/// <remarks>
		///     When an equal value is kept apart, the count of expected items is no longer the count of unique ones.
		/// </remarks>
		private void NumberTheExpectedValues(IEqualityComparer<T3> comparer, bool mergesEqualItems)
		{
			ItemIds<T3> expectedIds = new(comparer);
			_expectedIds = new int[ExpectedItems.Length];
			for (int i = 0; i < ExpectedItems.Length; i++)
			{
				T3 value = ExpectedItems[i];
				_expectedIds[i] = expectedIds.GetOrAdd(value, out bool isNew);
				if (!isNew && !mergesEqualItems && !IsIdenticalToEqualValues(value))
				{
					_expectedIds[i] = expectedIds.Add(value);
					_areExpectedItemsUnique = false;
				}
			}

			_expectedDistinctItems = expectedIds.Items.ToArray();
		}

		private async ValueTask<bool> MatchesTheSameExpectedItems(int subjectId, int index, T value,
			IOptionsEquality<T2> options)
		{
			for (int expectedId = 0; expectedId < _expectedDistinctItems.Length; expectedId++)
			{
				if (await IsMatch(subjectId, expectedId, options) !=
				    await AreConsideredEqual(index, value, _expectedDistinctItems[expectedId], options))
				{
					return false;
				}
			}

			return true;
		}

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
			               (!_equivalenceRelations.Includes(EquivalenceRelations.ContainsProperly) ||
			                await order.HasAdditionalItem());
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
			if (_equivalenceRelations.Includes(EquivalenceRelations.IsContainedIn))
			{
				InOrderMismatch searchedInExpected = await InOrderMismatch.Explain(_expectedIds,
					_expectedDistinctItems.Length, _subjectIds.Items.Count,
					(expectedId, subjectId) => IsMatch(subjectId, expectedId, options), false, GetOrderMatch());
				return InOrderDeviations<T, T3>.From(searchedInExpected, true,
					subjectId => (_firstIndexOfSubjectItem[subjectId], _subjectIds.Items[subjectId]),
					position => ExpectedItems[position], _dimensions);
			}

			InOrderMismatch searchedInSubject = await InOrderMismatch.Explain(_subjectIdAt.ToArray(),
				_subjectIds.Items.Count, _expectedDistinctItems.Length,
				(subjectId, expectedId) => IsMatch(subjectId, expectedId, options), false, GetOrderMatch());
			return InOrderDeviations<T, T3>.From(searchedInSubject, false,
				position => (position, _subjectIds.Items[_subjectIdAt[position]]),
				expectedId => _expectedDistinctItems[expectedId], _dimensions);
		}

		/// <summary>
		///     The containment searches the subject in the expected items, the other relations search the expected items
		///     in the subject.
		/// </summary>
		private DistinctItemsInOrder CreateOrder(IOptionsEquality<T2> options)
			=> _equivalenceRelations.Includes(EquivalenceRelations.IsContainedIn)
				? new DistinctItemsInOrder(_expectedIds, _expectedDistinctItems.Length, _subjectIds.Items.Count,
					(expectedId, subjectId) => IsMatch(subjectId, expectedId, options), GetOrderMatch())
				: new DistinctItemsInOrder(_subjectIdAt.ToArray(), _subjectIds.Items.Count,
					_expectedDistinctItems.Length, (subjectId, expectedId) => IsMatch(subjectId, expectedId, options),
					GetOrderMatch());

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
		///     Numbers the <paramref name="item" /> as a distinct item, although an equal item already has an id.
		/// </summary>
		public int Add(TItem item)
		{
			Items.Add(item);
			return Items.Count - 1;
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
