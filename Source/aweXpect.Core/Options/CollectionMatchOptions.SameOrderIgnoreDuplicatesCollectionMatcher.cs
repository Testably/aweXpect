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
			expected.Distinct(),
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
			expected.Distinct(new ExpectationItemEqualityComparer<T>()),
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
			expected.Distinct(new ExpressionEqualityComparer<T, bool>()),
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
		private readonly bool _ignoreInterspersedItems;
		private readonly Dictionary<int, (T Item, T3 Expected)> _incorrectItems = new();
		private readonly List<(int Index, T Item)> _matchingItems = new();
		private readonly List<T3> _missingItems = new();
		private readonly Dictionary<int, T> _outOfOrderItems = new();
		private readonly int _totalExpectedItems;
		private readonly HashSet<T> _uniqueItems = new();
		private int _expectationIndex = -1;
		private int _index;
		private int _matchIndex;
		private int _maxMatchIndex;

		protected SameOrderIgnoreDuplicatesCollectionMatcherBase(EquivalenceRelations equivalenceRelation,
			IEnumerable<T3> expected,
			bool ignoreInterspersedItems)
		{
			_equivalenceRelations = equivalenceRelation;
			_ignoreInterspersedItems = ignoreInterspersedItems;
			_expectedDistinctItems = expected.ToArray();
			_totalExpectedItems = _expectedDistinctItems.Length;
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
				if (_uniqueItems.Add(value))
				{
					await VerifyTheCurrentValueContinuesTheSubsequence(value, options);
				}

				_index++;
				return (_additionalItems.Count + _outOfOrderItems.Count > 2 * maximumNumber, null);
			}

#pragma warning disable S1871 // The identical branches record the same outcome for distinct reasons and are kept apart to stay readable
			if (_matchIndex >= _expectedDistinctItems.Length)
			{
				if (!_uniqueItems.Add(value))
				{
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
					return (false, null);
				}

				_additionalItems.Add(_index, value);
			}
			else
			{
				if (!_uniqueItems.Add(value))
				{
					return (false, null);
				}

				await VerifyTheCurrentValueIsDifferentFromTheExpectedValue(value, options);
			}
#pragma warning restore S1871

			_index++;
			return (_additionalItems.Count + _incorrectItems.Count + _missingItems.Count >
			        2 * maximumNumber, null);
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
				return VerifyCompleteForSubsequenceMatch(it, maximumNumber);
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
					return (true, null);
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

			List<string> errors = new();
			errors.AddRange(IncorrectItemsError(_incorrectItems));
			if (!_equivalenceRelations.HasFlag(EquivalenceRelations.Contains))
			{
				errors.AddRange(AdditionalItemsError(_additionalItems));
			}
			else if (_equivalenceRelations.HasFlag(EquivalenceRelations.ContainsProperly) &&
			         !_additionalItems.Any() &&
			         !await Any(_incorrectItems, i
				         => All(_expectedDistinctItems, e => AreConsideredEqual(i.Value.Item, e, options), true)))
			{
				errors.Add("did not contain any additional items");
			}

			errors.AddRange(MissingItemsError(_totalExpectedItems, _missingItems, _equivalenceRelations, true));

			string? error = ReturnErrorString(it, errors);
			return (error != null, error);
		}
#pragma warning restore S3776

		/// <summary>
		///     The subject is contained in the expected collection, when its unique items appear in the unique expected items in
		///     the same relative order; the expected items that are skipped in between are missing items.
		/// </summary>
		private (bool, string?) VerifyCompleteForSubsequenceMatch(string it, int maximumNumber)
		{
			for (int i = _matchIndex; i < _expectedDistinctItems.Length; i++)
			{
				_missingItems.Add(_expectedDistinctItems[i]);
				if (_additionalItems.Count + _outOfOrderItems.Count + _missingItems.Count > 2 * maximumNumber)
				{
					return (true, null);
				}
			}

			List<string> errors = new();
			errors.AddRange(OutOfOrderItemsError(_outOfOrderItems));
			errors.AddRange(AdditionalItemsError(_additionalItems));
			if (_equivalenceRelations.HasFlag(EquivalenceRelations.IsContainedInProperly) && !_missingItems.Any())
			{
				errors.Add("contained all expected items");
			}

			string? error = ReturnErrorString(it, errors);
			return (error != null, error);
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
			for (int i = _matchIndex; i < _expectedDistinctItems.Length; i++)
			{
				if (await AreConsideredEqual(value, _expectedDistinctItems[i], options))
				{
					for (int j = _matchIndex; j < i; j++)
					{
						_missingItems.Add(_expectedDistinctItems[j]);
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

#if NET8_0_OR_GREATER
		protected abstract ValueTask<bool>
#else
		protected abstract Task<bool>
#endif
			AreConsideredEqual(T value, T3 expected, IOptionsEquality<T2> options);
	}
}
