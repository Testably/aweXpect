using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using aweXpect.Core;

namespace aweXpect.Options;

public partial class CollectionMatchOptions
{
	private abstract partial class SameOrderIgnoreDuplicatesCollectionMatcherBase<T, T2, T3>
	{
		private readonly int[] _expectedIds;
		private readonly List<int> _firstIndexOfSubjectItem = new();
		private readonly ItemIds<T> _subjectIds = new(null);
		private readonly List<int> _subjectIdAt = new();
		private readonly Dictionary<int, T> _unexpectedItems = new();
		private string? _alignmentError;
		private bool _isAlignmentAborted;

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

			bool isDeviation = true;
			if (!_isAlignmentAborted)
			{
				(_isAlignmentAborted, _alignmentError) = await VerifyAlignment(it, value, options, maximumNumber);
				isDeviation = _additionalItems.ContainsKey(index) || _incorrectItems.ContainsKey(index) ||
				              _outOfOrderItems.ContainsKey(index);
			}

			// Only an item that matches no expected item is a deviation that is known before the end. The alignment
			// reports every such item, so only its deviations are compared with all expected items, until it gives up.
			if (isNew && isDeviation && !_equivalenceRelations.HasFlag(EquivalenceRelations.Contains) &&
			    !IsEqualToAnExpectedItem(value) && !await MatchesAnyExpectedItem(subjectId, options))
			{
				_unexpectedItems.Add(index, value);
				if (_unexpectedItems.Count > 2L * maximumNumber)
				{
					return (true, _isAlignmentAborted
						? _alignmentError
						: TooManyDeviationsError(it, maximumNumber,
							AdditionalItemsError(_unexpectedItems, CreateItemFormatter())));
				}
			}

			return (false, null);
		}

		/// <summary>
		///     Avoids comparing each item with the expected items, when it is known to match one of them.
		/// </summary>
		/// <remarks>
		///     This only decides, whether the comparison can abort early, so it may err towards a match.
		/// </remarks>
		protected virtual bool IsEqualToAnExpectedItem(T value) => false;

		public async ValueTask<(bool, string?)>
			VerifyComplete(string it, IOptionsEquality<T2> options, int maximumNumber)
		{
			DistinctItemsInOrder order = CreateOrder(options);
			bool isInOrder = await order.IsInOrder();
			bool requiresAdditionalItem = _equivalenceRelations.HasFlag(EquivalenceRelations.ContainsProperly) ||
			                              _equivalenceRelations.HasFlag(EquivalenceRelations.IsContainedInProperly);
			if (isInOrder && (!requiresAdditionalItem || await order.HasAdditionalItem()))
			{
				return (false, null);
			}

			if (_isAlignmentAborted)
			{
				return (true, _alignmentError);
			}

			(bool isFailure, string? error) = await CompleteAlignment(it, options, maximumNumber);
			if (isFailure)
			{
				return (true, error);
			}

			return (true, isInOrder
				? ReturnErrorString(it, [MissingAdditionalItemError(),])
				: await DescribeTheMismatch(it, order, options, maximumNumber));
		}

		/// <summary>
		///     The alignment only misses deviations that the decision finds, so they are described from the items that
		///     the first fitting placement leaves over.
		/// </summary>
		private async ValueTask<string?> DescribeTheMismatch(string it, DistinctItemsInOrder order,
			IOptionsEquality<T2> options, int maximumNumber)
		{
			(List<int> unplacedIds, HashSet<int> placedIds, (int Position, int SoughtId)? interruption) =
				await order.PlaceFirstFitting();
			List<string> errors = new();
			if (interruption is not null)
			{
				errors.AddRange(IncorrectItemsError(DescribeTheInterruption(interruption.Value), options));
				return ReturnErrorString(it, errors);
			}

			if (_equivalenceRelations.HasFlag(EquivalenceRelations.IsContainedIn))
			{
				Dictionary<int, T> outOfOrderItems = new();
				foreach (int subjectId in unplacedIds.Where(id => !IsUnexpected(id)))
				{
					outOfOrderItems.Add(_firstIndexOfSubjectItem[subjectId], _subjectIds.Items[subjectId]);
				}

				errors.AddRange(OutOfOrderItemsError(outOfOrderItems));
				errors.AddRange(AdditionalItemsError(_unexpectedItems, CreateItemFormatter()));
				return ReturnErrorString(it, errors);
			}

			Dictionary<int, T> unexpectedItems = new();
			if (!_equivalenceRelations.HasFlag(EquivalenceRelations.Contains))
			{
				// A distinct item that serves no predicate or expectation is not expected either.
				for (int subjectId = 0; subjectId < _subjectIds.Items.Count; subjectId++)
				{
					if (IsUnexpected(subjectId) ||
					    (!RepeatingAMatchedExpectedItemIsADuplicate && !placedIds.Contains(subjectId)))
					{
						unexpectedItems.Add(_firstIndexOfSubjectItem[subjectId], _subjectIds.Items[subjectId]);
					}
				}
			}

			List<T3> missingItems = unplacedIds.Select(id => _expectedDistinctItems[id]).ToList();
			Func<object?, string> formatItem =
				GetItemFormatter(unexpectedItems.Values.Cast<object?>(), missingItems.Cast<object?>());
			errors.AddRange(AdditionalItemsError(unexpectedItems, formatItem));

			errors.AddRange(MissingItemsError(_totalExpectedItems, missingItems, _equivalenceRelations, true,
				formatItem, options, maximumNumber));
			return ReturnErrorString(it, errors);
		}

		/// <summary>
		///     For the containment, the run is searched in the expected items, so the subject item lacks the interrupting
		///     expected item; otherwise the interrupting subject item stands where the sought expected item belongs.
		/// </summary>
		private Dictionary<int, (T Item, T3 Expected)> DescribeTheInterruption((int Position, int SoughtId) interruption)
		{
			if (_equivalenceRelations.HasFlag(EquivalenceRelations.IsContainedIn))
			{
				int subjectId = interruption.SoughtId;
				return new Dictionary<int, (T Item, T3 Expected)>
				{
					{
						_firstIndexOfSubjectItem[subjectId],
						(_subjectIds.Items[subjectId], _expectedItems[interruption.Position])
					},
				};
			}

			return new Dictionary<int, (T Item, T3 Expected)>
			{
				{
					interruption.Position,
					(_subjectIds.Items[_subjectIdAt[interruption.Position]], _expectedDistinctItems[interruption.SoughtId])
				},
			};
		}

		private bool IsUnexpected(int subjectId)
			=> _unexpectedItems.ContainsKey(_firstIndexOfSubjectItem[subjectId]);

		private string MissingAdditionalItemError()
			=> _equivalenceRelations.HasFlag(EquivalenceRelations.IsContainedIn)
				? "contained all expected items"
				: "did not contain any additional items";

		/// <summary>
		///     The containment searches the subject in the expected items, the other relations search the expected items
		///     in the subject.
		/// </summary>
		private DistinctItemsInOrder CreateOrder(IOptionsEquality<T2> options)
		{
			OrderMatch orderMatch = OrderMatch.Contiguous;
			if ((_equivalenceRelations & (EquivalenceRelations.Contains | EquivalenceRelations.IsContainedIn)) == 0)
			{
				orderMatch = OrderMatch.Equal;
			}
			else if (_ignoreInterspersedItems)
			{
				orderMatch = OrderMatch.Subsequence;
			}

			return _equivalenceRelations.HasFlag(EquivalenceRelations.IsContainedIn)
				? new DistinctItemsInOrder(_expectedIds, _expectedDistinctItems.Length, _subjectIds.Items.Count,
					(expectedId, subjectId) => IsMatch(subjectId, expectedId, options),
					!RepeatingAMatchedExpectedItemIsADuplicate, orderMatch)
				: new DistinctItemsInOrder(_subjectIdAt.ToArray(), _subjectIds.Items.Count,
					_expectedDistinctItems.Length,
					(subjectId, expectedId) => IsMatch(subjectId, expectedId, options),
					!RepeatingAMatchedExpectedItemIsADuplicate, orderMatch);
		}

		private async ValueTask<bool> MatchesAnyExpectedItem(int subjectId, IOptionsEquality<T2> options)
		{
			for (int expectedId = 0; expectedId < _expectedDistinctItems.Length; expectedId++)
			{
				if (await IsMatch(subjectId, expectedId, options))
				{
					return true;
				}
			}

			return false;
		}

		private ValueTask<bool> IsMatch(int subjectId, int expectedId, IOptionsEquality<T2> options)
			=> AreConsideredEqual(_subjectIds.Items[subjectId], _expectedDistinctItems[expectedId], options);

		private static int[] AssignIds(T3[] items, IEqualityComparer<T3>? comparer)
		{
			ItemIds<T3> ids = new(comparer);
			return items.Select(item => ids.GetOrAdd(item, out _)).ToArray();
		}
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
