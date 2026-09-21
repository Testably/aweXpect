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
#if NET8_0_OR_GREATER
		protected override ValueTask<bool> AreConsideredEqual(T value, T expected, IOptionsEquality<T2> options)
#else
		protected override Task<bool> AreConsideredEqual(T value, T expected, IOptionsEquality<T2> options)
#endif
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
#if NET8_0_OR_GREATER
		protected override ValueTask<bool>
#else
		protected override Task<bool>
#endif
			AreConsideredEqual(T value, ExpectationItem<T> expected, IOptionsEquality<T2> options)
			=> expected.IsMetBy(value);
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
#if NET8_0_OR_GREATER
		protected override ValueTask<bool> AreConsideredEqual(T value, Expression<Func<T, bool>> expected,
			IOptionsEquality<T2> options)
			=> ValueTask.FromResult(expected.Compile().Invoke(value));
#else
		protected override Task<bool> AreConsideredEqual(T value, Expression<Func<T, bool>> expected,
			IOptionsEquality<T2> options)
			=> Task.FromResult(expected.Compile().Invoke(value));
#endif
	}

	private abstract class SameOrderIgnoreDuplicatesCollectionMatcherBase<T, T2, T3> : ICollectionMatcher<T, T2>
		where T : T2
	{
		private readonly Dictionary<int, T> _additionalItems = new();
		private readonly EquivalenceRelations _equivalenceRelations;
		private readonly T3[] _expectedDistinctItems;
		private readonly T3[] _expectedItems;
		private readonly bool _ignoreInterspersedItems;
		private readonly Dictionary<int, (T Item, T3 Expected)> _incorrectItems = new();
		private readonly bool[] _isRepeatedItem;
		private readonly List<(int Index, T Item)> _matchingItems = new();
		private readonly List<T3> _missingItems = new();
		private readonly Dictionary<int, T> _outOfOrderItems = new();
		private readonly int _totalExpectedItems;
		private readonly HashSet<T> _uniqueItems = new();
		private List<(int Offset, int Cursor)> _candidates = new();
		private int _alignment;
		private int _distinctIndex;
		private int _expectationIndex = -1;
		private int _index;
		private int _matchIndex;
		private int _maxMatchIndex;
		private bool _runIsBroken;

		protected SameOrderIgnoreDuplicatesCollectionMatcherBase(EquivalenceRelations equivalenceRelation,
			IEnumerable<T3> expected,
			IEqualityComparer<T3>? comparer,
			bool ignoreInterspersedItems)
		{
			_equivalenceRelations = equivalenceRelation;
			_ignoreInterspersedItems = ignoreInterspersedItems;
			_expectedItems = expected.ToArray();
			_expectedDistinctItems = _expectedItems.Distinct(comparer).ToArray();
			_totalExpectedItems = _expectedDistinctItems.Length;
			_isRepeatedItem = new bool[_expectedItems.Length];
			HashSet<T3> seenItems = new(comparer);
			for (int i = 0; i < _expectedItems.Length; i++)
			{
				_isRepeatedItem[i] = !seenItems.Add(_expectedItems[i]);
			}
		}

#if NET8_0_OR_GREATER
		public async ValueTask<(bool, string?)>
#else
		public async Task<(bool, string?)>
#endif
			Verify(string it, T value, IOptionsEquality<T2> options, int maximumNumber)
		{
			if (_equivalenceRelations.HasFlag(EquivalenceRelations.IsContainedIn))
			{
				return await VerifyTheCurrentValueIsContainedInTheExpectedItems(it, value, options, maximumNumber);
			}

#pragma warning disable S1871 // The identical branches record the same outcome for distinct reasons and are kept apart to stay readable
			if (_matchIndex >= _expectedDistinctItems.Length)
			{
				if (!_uniqueItems.Add(value))
				{
					_index++;
					return (false, null);
				}

				// All expected items were found -> additional items
				_additionalItems.Add(_index, value);
			}
			else if (await AreConsideredEqual(value, _expectedDistinctItems[_matchIndex], options))
			{
				await VerifyTheCurrentValueIsEqualToTheExpectedValue(value, options);
			}
			else if (_ignoreInterspersedItems)
			{
				if (!_uniqueItems.Add(value))
				{
					_index++;
					return (false, null);
				}

				_additionalItems.Add(_index, value);
			}
			else
			{
				if (!_uniqueItems.Add(value))
				{
					_index++;
					return (false, null);
				}

				await VerifyTheCurrentValueIsDifferentFromTheExpectedValue(value, options);
			}
#pragma warning restore S1871

			_index++;
			return _additionalItems.Count + _incorrectItems.Count + _missingItems.Count > 2 * maximumNumber
				? (true, TooManyDeviationsError(it, maximumNumber, GetDeviations()))
				: (false, null);
		}

		/// <summary>
		///     Additional items are no deviation for the containment relation, so they are left out.
		/// </summary>
		private IEnumerable<string> GetDeviations()
		{
			IEnumerable<string> deviations = IncorrectItemsError(_incorrectItems)
				.Concat(OutOfOrderItemsError(_outOfOrderItems));
			return _equivalenceRelations.HasFlag(EquivalenceRelations.Contains)
				? deviations
				: deviations.Concat(AdditionalItemsError(_additionalItems, CreateItemFormatter()));
		}

		/// <summary>
		///     Only the unique items of the subject have to appear in the expected collection, so an item that repeats
		///     an earlier one is skipped; the expected items it never consumes are no deviations for this relation.
		/// </summary>
#if NET8_0_OR_GREATER
		private async ValueTask<(bool, string?)>
#else
		private async Task<(bool, string?)>
#endif
			VerifyTheCurrentValueIsContainedInTheExpectedItems(string it, T value, IOptionsEquality<T2> options,
				int maximumNumber)
		{
			if (_uniqueItems.Add(value))
			{
				if (_ignoreInterspersedItems)
				{
					await VerifyTheCurrentValueContinuesTheSubsequence(value, options);
				}
				else
				{
					await VerifyTheCurrentValueContinuesTheContiguousRun(value, options);
				}

				_distinctIndex++;
			}

			_index++;
			return _additionalItems.Count + _incorrectItems.Count + _outOfOrderItems.Count > 2 * maximumNumber
				? (true, TooManyDeviationsError(it, maximumNumber, GetDeviations()))
				: (false, null);
		}

#pragma warning disable S3776 // https://rules.sonarsource.com/csharp/RSPEC-3776
#if NET8_0_OR_GREATER
		public async ValueTask<(bool, string?)>
#else
		public async Task<(bool, string?)>
#endif
			VerifyComplete(string it, IOptionsEquality<T2> options, int maximumNumber)
		{
			if (_equivalenceRelations.HasFlag(EquivalenceRelations.IsContainedIn))
			{
				return _ignoreInterspersedItems
					? VerifyCompleteForSubsequenceMatch(it)
					: VerifyCompleteForContiguousMatch(it);
			}

			int maximumNumberOfCollectionItems =
				maximumNumber;
			foreach (T3 item in _expectedDistinctItems.Skip(Math.Max(_expectationIndex - 1, _maxMatchIndex)))
			{
				KeyValuePair<int, T> additionalItem =
					await FirstOrDefault(_additionalItems, a => AreConsideredEqual(a.Value, item, options));
				if (!additionalItem.IsDefault())
				{
					_additionalItems.Remove(additionalItem.Key);
					_missingItems.Add(item);
				}

				if (await Any(_uniqueItems, v => AreConsideredEqual(v, item, options)))
				{
					continue;
				}

				if (await All(_additionalItems, x => AreConsideredEqual(x.Value, item, options), true) &&
				    await All(_incorrectItems, x => AreConsideredEqual(x.Value.Item, item, options), true))
				{
					_missingItems.Add(item);
				}

				if (_additionalItems.Count + _incorrectItems.Count + _missingItems.Count >
				    2 * maximumNumberOfCollectionItems)
				{
					return (true, TooManyDeviationsError(it, maximumNumber, GetDeviations()));
				}
			}

			if (_equivalenceRelations.HasFlag(EquivalenceRelations.Contains) &&
			    _matchIndex >= _expectedDistinctItems.Length)
			{
				// A later complete match supersedes the deviations of abandoned partial matches.
				foreach (KeyValuePair<int, (T Item, T3 Expected)> incorrectItem in _incorrectItems)
				{
					_additionalItems.Add(incorrectItem.Key, incorrectItem.Value.Item);
				}

				_incorrectItems.Clear();
			}

			Func<object?, string> formatItem = CreateItemFormatter();
			List<string> errors = new();
			errors.AddRange(IncorrectItemsError(_incorrectItems));
			if (!_equivalenceRelations.HasFlag(EquivalenceRelations.Contains))
			{
				errors.AddRange(AdditionalItemsError(_additionalItems, formatItem));
			}
			else if (_equivalenceRelations.HasFlag(EquivalenceRelations.ContainsProperly) &&
			         !_additionalItems.Any() &&
			         !await Any(_incorrectItems, i
				         => All(_expectedDistinctItems, e => AreConsideredEqual(i.Value.Item, e, options), true)))
			{
				errors.Add("did not contain any additional items");
			}

			errors.AddRange(MissingItemsError(_totalExpectedItems, _missingItems, _equivalenceRelations, true, formatItem));

			string? error = ReturnErrorString(it, errors);
			return (error != null, error);
		}
#pragma warning restore S3776

		/// <summary>
		///     The subject is contained in the expected collection, when its unique items appear there as an uninterrupted
		///     run; once no run is left, the remaining items are compared against the abandoned run.
		/// </summary>
		private (bool, string?) VerifyCompleteForContiguousMatch(string it)
		{
			List<string> errors = new();
			errors.AddRange(IncorrectItemsError(_incorrectItems));
			errors.AddRange(AdditionalItemsError(_additionalItems, CreateItemFormatter()));
			if (errors.Count == 0 &&
			    _equivalenceRelations.HasFlag(EquivalenceRelations.IsContainedInProperly) &&
			    _distinctIndex >= _expectedDistinctItems.Length)
			{
				errors.Add("contained all expected items");
			}

			string? error = ReturnErrorString(it, errors);
			return (error != null, error);
		}

		/// <summary>
		///     The subject is contained in the expected collection, when its unique items appear there in the same relative
		///     order; the expected items that are skipped in between are missing items.
		/// </summary>
		private (bool, string?) VerifyCompleteForSubsequenceMatch(string it)
		{
			for (int i = _matchIndex; i < _expectedItems.Length; i++)
			{
				_missingItems.Add(_expectedItems[i]);
			}

			List<string> errors = new();
			errors.AddRange(OutOfOrderItemsError(_outOfOrderItems));
			errors.AddRange(AdditionalItemsError(_additionalItems, CreateItemFormatter()));
			if (errors.Count == 0 &&
			    _equivalenceRelations.HasFlag(EquivalenceRelations.IsContainedInProperly) &&
			    _distinctIndex >= _expectedDistinctItems.Length)
			{
				errors.Add("contained all expected items");
			}

			string? error = ReturnErrorString(it, errors);
			return (error != null, error);
		}

		/// <summary>
		///     Keeps all offsets in the expected collection at which the subject could still start an uninterrupted run;
		///     when the last candidate is abandoned, the <paramref name="value" /> and all later items are reported
		///     against it, unless they still match the item they are aligned with.
		/// </summary>
#if NET8_0_OR_GREATER
		private async ValueTask
#else
		private async Task
#endif
			VerifyTheCurrentValueContinuesTheContiguousRun(T value, IOptionsEquality<T2> options)
		{
			if (!_runIsBroken)
			{
				List<(int Offset, int Cursor)> candidates = await FindTheRemainingCandidates(value, options);
				if (candidates.Count > 0)
				{
					_candidates = candidates;
					return;
				}

				_alignment = _candidates.Count > 0 ? _candidates[0].Cursor : 0;
				_runIsBroken = true;
			}

			if (_alignment >= _expectedItems.Length)
			{
				_additionalItems.Add(_index, value);
			}
			else if (!await AreConsideredEqual(value, _expectedItems[_alignment], options))
			{
				_incorrectItems.Add(_index, (value, _expectedItems[_alignment]));
			}

			_alignment++;
		}

		/// <summary>
		///     The first unique item opens a candidate at every offset it matches, each later unique item keeps the
		///     candidates whose next expected item it matches.
		/// </summary>
		/// <returns>
		///     The offsets at which the run can still continue with the <paramref name="value" />, each with the cursor
		///     behind the expected item it matched.
		/// </returns>
#if NET8_0_OR_GREATER
		private async ValueTask<List<(int Offset, int Cursor)>>
#else
		private async Task<List<(int Offset, int Cursor)>>
#endif
			FindTheRemainingCandidates(T value, IOptionsEquality<T2> options)
		{
			List<(int Offset, int Cursor)> candidates = new();
			if (_distinctIndex == 0)
			{
				for (int offset = 0; offset < _expectedItems.Length; offset++)
				{
					if (await AreConsideredEqual(value, _expectedItems[offset], options))
					{
						candidates.Add((offset, offset + 1));
					}
				}
			}
			else
			{
				foreach ((int offset, int cursor) in _candidates)
				{
					int match = await FindTheNextMatchingExpectedItem(cursor, value, options);
					if (match >= 0)
					{
						candidates.Add((offset, match + 1));
					}
				}
			}

			return candidates;
		}

		/// <summary>
		///     Expected items that repeat an earlier one do not interrupt the run, because duplicates are ignored.
		/// </summary>
		/// <returns>The index of the matching expected item, or <c>-1</c> when the run ends before it.</returns>
#if NET8_0_OR_GREATER
		private async ValueTask<int>
#else
		private async Task<int>
#endif
			FindTheNextMatchingExpectedItem(int cursor, T value, IOptionsEquality<T2> options)
		{
			while (cursor < _expectedItems.Length)
			{
				if (await AreConsideredEqual(value, _expectedItems[cursor], options))
				{
					return cursor;
				}

				if (!_isRepeatedItem[cursor])
				{
					break;
				}

				cursor++;
			}

			return -1;
		}

		/// <summary>
		///     Consumes the expected items until the <paramref name="value" /> matches, so that gaps in the expected
		///     collection are allowed, but the subject items must keep their relative order.
		/// </summary>
#if NET8_0_OR_GREATER
		private async ValueTask
#else
		private async Task
#endif
			VerifyTheCurrentValueContinuesTheSubsequence(T value, IOptionsEquality<T2> options)
		{
			for (int i = _matchIndex; i < _expectedItems.Length; i++)
			{
				if (await AreConsideredEqual(value, _expectedItems[i], options))
				{
					for (int j = _matchIndex; j < i; j++)
					{
						_missingItems.Add(_expectedItems[j]);
					}

					_matchIndex = i + 1;
					return;
				}
			}

			if (await Any(_missingItems, m => AreConsideredEqual(value, m, options)))
			{
				_outOfOrderItems.Add(_index, value);
			}
			else
			{
				_additionalItems.Add(_index, value);
			}
		}

#if NET8_0_OR_GREATER
		private async ValueTask
#else
		private async Task
#endif
			VerifyTheCurrentValueIsDifferentFromTheExpectedValue(T value, IOptionsEquality<T2> options)
		{
			if (_expectationIndex >= 0)
			{
				_expectationIndex++;
			}

			_matchIndex = 0;

			if (await AreConsideredEqual(value, _expectedDistinctItems[_matchIndex], options))
			{
				foreach ((int index, T matchingItem) in _matchingItems)
				{
					_additionalItems.Add(index, matchingItem);
				}

				_matchingItems.Clear();
				_matchIndex++;
				_maxMatchIndex = Math.Max(_matchIndex, _maxMatchIndex);
				_expectationIndex = 0;
				_matchingItems.Add((_index, value));
			}
			else if (_expectationIndex < 0 || _expectationIndex >= _expectedDistinctItems.Length)
			{
				_additionalItems.Add(_index, value);
			}
			else if (await AreConsideredEqual(value, _expectedDistinctItems[_expectationIndex], options))
			{
				// The value still matches the expected item it is aligned with, so it is no deviation,
				// although the run that could have matched was abandoned.
				_maxMatchIndex = Math.Max(_expectationIndex + 1, _maxMatchIndex);
			}
			else
			{
				_incorrectItems.Add(_index, (value, _expectedDistinctItems[_expectationIndex]));
			}
		}

#if NET8_0_OR_GREATER
		private async ValueTask
#else
		private async Task
#endif
			VerifyTheCurrentValueIsEqualToTheExpectedValue(T value, IOptionsEquality<T2> options)
		{
			_matchIndex++;
			_maxMatchIndex = Math.Max(_matchIndex, _maxMatchIndex);
			_expectationIndex++;
			_matchingItems.Add((_index, value));
			_uniqueItems.Add(value);
			foreach (int key in await Filter(_additionalItems, item => options.AreConsideredEqual(item.Value, value),
				         x => x.Key))
			{
				_additionalItems.Remove(key);
			}
		}

		/// <summary>
		///     An unexpected and a missing item that format equally differ only in their runtime type.
		/// </summary>
		private Func<object?, string> CreateItemFormatter()
			=> GetItemFormatter(_additionalItems.Values.Cast<object?>(), _missingItems.Cast<object?>());

#if NET8_0_OR_GREATER
		protected abstract ValueTask<bool>
#else
		protected abstract Task<bool>
#endif
			AreConsideredEqual(T value, T3 expected, IOptionsEquality<T2> options);
	}
}
