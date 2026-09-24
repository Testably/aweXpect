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
	private sealed class SameOrderCollectionMatcher<T, T2>(
		EquivalenceRelations equivalenceRelation,
		IEnumerable<T> expected,
		bool ignoreInterspersedItems)
		: SameOrderCollectionMatcherBase<T, T2, T>(equivalenceRelation, expected, ignoreInterspersedItems)
		where T : T2
	{
#if NET8_0_OR_GREATER
		protected override ValueTask<bool> AreConsideredEqual(T value, T expected, IOptionsEquality<T2> options)
#else
		protected override Task<bool> AreConsideredEqual(T value, T expected, IOptionsEquality<T2> options)
#endif
			=> options.AreConsideredEqual(value, expected);
	}

	private sealed class SameOrderFromExpectationCollectionMatcher<T, T2>(
		EquivalenceRelations equivalenceRelation,
		IEnumerable<ExpectationItem<T>> expected,
		bool ignoreInterspersedItems)
		: SameOrderCollectionMatcherBase<T, T2, ExpectationItem<T>>(equivalenceRelation, expected,
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

	private sealed class SameOrderFromPredicateCollectionMatcher<T, T2>(
		EquivalenceRelations equivalenceRelation,
		IEnumerable<Expression<Func<T, bool>>> expected,
		bool ignoreInterspersedItems)
		: SameOrderCollectionMatcherBase<T, T2, Expression<Func<T, bool>>>(equivalenceRelation, expected,
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

	private abstract class SameOrderCollectionMatcherBase<T, T2, T3> : ICollectionMatcher<T, T2>
		where T : T2
	{
		private readonly Dictionary<int, T> _additionalItems = new();
		private readonly bool _comparesByPosition;
		private readonly EquivalenceRelations _equivalenceRelations;
		private readonly T3[] _expectedItems;
		private readonly bool _ignoreInterspersedItems;
		private readonly Dictionary<int, (T Item, T3 Expected)> _incorrectItems = new();
		private readonly List<(int Index, T Item)> _matchingItems = new();
		private readonly List<T3> _missingItems = new();
		private readonly Dictionary<int, T> _outOfOrderItems = new();
		private readonly int _totalExpectedItems;
		private readonly List<T> _values = new();
		private int _alignment;
		private List<int> _candidateOffsets = new();
		private BoundedEditDistance<T, T3>? _editDistance;
		private int _expectationIndex = -1;
		private int _index;
		private int _matchIndex;
		private int _maxMatchIndex;
		private int _positionalDeviations;
		private bool _runIsBroken;

		protected SameOrderCollectionMatcherBase(EquivalenceRelations equivalenceRelation,
			IEnumerable<T3> expected,
			bool ignoreInterspersedItems)
		{
			_equivalenceRelations = equivalenceRelation;
			_ignoreInterspersedItems = ignoreInterspersedItems;
			_comparesByPosition = !ignoreInterspersedItems &&
			                      !equivalenceRelation.HasFlag(EquivalenceRelations.Contains) &&
			                      !equivalenceRelation.HasFlag(EquivalenceRelations.IsContainedIn);
			_expectedItems = expected.ToArray();
			_totalExpectedItems = _expectedItems.Length;
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
				if (_ignoreInterspersedItems)
				{
					await VerifyTheCurrentValueContinuesTheSubsequence(value, options);
				}
				else
				{
					await VerifyTheCurrentValueContinuesTheContiguousRun(value, options);
				}
			}
			else if (_comparesByPosition)
			{
				return await VerifyTheCurrentValueMatchesTheItemAtItsPosition(it, value, options, maximumNumber);
			}
			else if (_matchIndex >= _expectedItems.Length)
			{
				// All expected items were found -> additional items
				_additionalItems.Add(_index, value);
			}
			else if (await AreConsideredEqual(value, _expectedItems[_matchIndex], options))
			{
				VerifyTheCurrentValueIsEqualToTheExpectedValue(value);
			}
			else if (_ignoreInterspersedItems)
			{
				_additionalItems.Add(_index, value);
			}
			else
			{
				await VerifyTheCurrentValueIsDifferentFromTheExpectedValue(value, options);
			}

			_index++;
			int errorThreshold = 2 * maximumNumber;
			int errorCount = _incorrectItems.Count + _outOfOrderItems.Count;
			if (!_equivalenceRelations.HasFlag(EquivalenceRelations.IsContainedIn))
			{
				// Expected items outside the matched run are no deviations.
				errorCount += _missingItems.Count;
			}

			if (!_equivalenceRelations.HasFlag(EquivalenceRelations.Contains))
			{
				errorCount += _additionalItems.Count;
			}

			return errorCount > errorThreshold
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

			if (_comparesByPosition)
			{
				return await VerifyCompleteForPositionalMatch(it, options, maximumNumber);
			}

			int consideredExpectedItems = Math.Max(_expectationIndex - 1, _maxMatchIndex);
			if (_expectedItems.Length > consideredExpectedItems)
			{
				for (int i = consideredExpectedItems; i < _expectedItems.Length; i++)
				{
					T3 item = _expectedItems[i];
					KeyValuePair<int, T> additionalItem = await FirstOrDefault(_additionalItems,
						a => AreConsideredEqual(a.Value, item, options));
					if (!additionalItem.IsDefault())
					{
						_additionalItems.Remove(additionalItem.Key);
						_missingItems.Add(item);
					}
					else if (await All(_additionalItems.Values, x => AreConsideredEqual(x, item, options), true) &&
					         await All(_incorrectItems.Values, x => AreConsideredEqual(x.Item, item, options), true))
					{
						_missingItems.Add(item);
					}

					if (_additionalItems.Count + _incorrectItems.Count + _missingItems.Count >
					    2 * maximumNumber)
					{
						return (true, TooManyDeviationsError(it, maximumNumber, GetDeviations()));
					}
				}
			}

			if (_equivalenceRelations.HasFlag(EquivalenceRelations.Contains) &&
			    _matchIndex >= _expectedItems.Length)
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
			else if (_equivalenceRelations.HasFlag(EquivalenceRelations.ContainsProperly) && !_additionalItems.Any())
			{
				errors.Add("did not contain any additional items");
			}

			errors.AddRange(MissingItemsError(_totalExpectedItems, _missingItems, _equivalenceRelations, false, formatItem));

			string? error = ReturnErrorString(it, errors);
			return (error != null, error);
		}
#pragma warning restore S3776

		/// <summary>
		///     Every subject item was compared with the expected item at its position, so the expected items beyond the
		///     end of the subject are missing; when fewer edits align the subject with the expected items, e.g. because
		///     an item was inserted, these edits are reported instead.
		/// </summary>
#if NET8_0_OR_GREATER
		private async ValueTask<(bool, string?)>
#else
		private async Task<(bool, string?)>
#endif
			VerifyCompleteForPositionalMatch(string it, IOptionsEquality<T2> options, int maximumNumber)
		{
			int positionalDeviations = _positionalDeviations + Math.Max(0, _expectedItems.Length - _index);
			if (_editDistance is not null)
			{
				List<(EditKind Kind, int SubjectIndex, int ExpectedIndex)>? edits = await _editDistance.GetEdits(
					_values, (item, expected) => AreConsideredEqual(item, expected, options));
				if (edits is not null && edits.Count < positionalDeviations)
				{
					return ReturnEditsError(it, edits);
				}
			}

			if (positionalDeviations > 2 * maximumNumber)
			{
				return (true, TooManyDeviationsError(it, maximumNumber, GetDeviations()));
			}

			for (int i = _index; i < _expectedItems.Length; i++)
			{
				_missingItems.Add(_expectedItems[i]);
			}

			return ReturnError(it, _incorrectItems, _additionalItems, _missingItems);
		}

		private (bool, string?) ReturnEditsError(string it,
			List<(EditKind Kind, int SubjectIndex, int ExpectedIndex)> edits)
		{
			Dictionary<int, (T Item, T3 Expected)> incorrectItems = new();
			Dictionary<int, T> additionalItems = new();
			List<T3> missingItems = new();
			foreach ((EditKind kind, int subjectIndex, int expectedIndex) in edits)
			{
				switch (kind)
				{
					case EditKind.Incorrect:
						incorrectItems.Add(subjectIndex, (_values[subjectIndex], _expectedItems[expectedIndex]));
						break;
					case EditKind.Additional:
						additionalItems.Add(subjectIndex, _values[subjectIndex]);
						break;
					default:
						missingItems.Add(_expectedItems[expectedIndex]);
						break;
				}
			}

			return ReturnError(it, incorrectItems, additionalItems, missingItems);
		}

		private (bool, string?) ReturnError(string it, Dictionary<int, (T Item, T3 Expected)> incorrectItems,
			Dictionary<int, T> additionalItems, List<T3> missingItems)
		{
			Func<object?, string> formatItem =
				GetItemFormatter(additionalItems.Values.Cast<object?>(), missingItems.Cast<object?>());
			List<string> errors = new();
			errors.AddRange(IncorrectItemsError(incorrectItems));
			errors.AddRange(AdditionalItemsError(additionalItems, formatItem));
			errors.AddRange(MissingItemsError(_totalExpectedItems, missingItems, _equivalenceRelations, false, formatItem));

			string? error = ReturnErrorString(it, errors);
			return (error != null, error);
		}

		/// <summary>
		///     The subject is contained in the expected collection, when its items appear there as an uninterrupted run;
		///     once no run is left, the remaining items are compared against the abandoned run.
		/// </summary>
		private (bool, string?) VerifyCompleteForContiguousMatch(string it)
		{
			List<string> errors = new();
			errors.AddRange(IncorrectItemsError(_incorrectItems));
			errors.AddRange(AdditionalItemsError(_additionalItems, CreateItemFormatter()));
			if (errors.Count == 0 &&
			    _equivalenceRelations.HasFlag(EquivalenceRelations.IsContainedInProperly) &&
			    _index >= _expectedItems.Length)
			{
				errors.Add("contained all expected items");
			}

			string? error = ReturnErrorString(it, errors);
			return (error != null, error);
		}

		/// <summary>
		///     The subject is contained in the expected collection, when its items appear in the expected collection in the
		///     same relative order; the expected items that are skipped in between are missing items.
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
			if (_equivalenceRelations.HasFlag(EquivalenceRelations.IsContainedInProperly) && !_missingItems.Any())
			{
				errors.Add("contained all expected items");
			}

			string? error = ReturnErrorString(it, errors);
			return (error != null, error);
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

			if (await AreConsideredEqual(value, _expectedItems[_matchIndex], options))
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
			else if (_expectationIndex < 0 || _expectationIndex >= _expectedItems.Length)
			{
				_additionalItems.Add(_index, value);
			}
			else if (await AreConsideredEqual(value, _expectedItems[_expectationIndex], options))
			{
				// The value still matches the expected item it is aligned with, so it is no deviation,
				// although the run that could have matched was abandoned.
				_maxMatchIndex = Math.Max(_expectationIndex + 1, _maxMatchIndex);
			}
			else
			{
				_incorrectItems.Add(_index, (value, _expectedItems[_expectationIndex]));
			}
		}

		/// <summary>
		///     Keeps all offsets in the expected collection at which the subject could still start an uninterrupted run;
		///     when the last one is abandoned, the <paramref name="value" /> and all later items are reported against the
		///     first abandoned offset, unless they still match the item at that offset.
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
				List<int> candidateOffsets = await FindTheRemainingCandidateOffsets(value, options);
				if (candidateOffsets.Count > 0)
				{
					_candidateOffsets = candidateOffsets;
					return;
				}

				_alignment = _candidateOffsets.Count > 0 ? _candidateOffsets[0] : 0;
				_runIsBroken = true;
			}

			int expectedIndex = _alignment + _index;
			if (expectedIndex >= _expectedItems.Length)
			{
				_additionalItems.Add(_index, value);
			}
			else if (!await AreConsideredEqual(value, _expectedItems[expectedIndex], options))
			{
				_incorrectItems.Add(_index, (value, _expectedItems[expectedIndex]));
			}
		}

		/// <summary>
		///     The first item opens a candidate at every offset it matches, each later item keeps the candidates whose
		///     next expected item it matches.
		/// </summary>
		/// <returns>The offsets at which the run can still continue with the <paramref name="value" />.</returns>
#if NET8_0_OR_GREATER
		private async ValueTask<List<int>>
#else
		private async Task<List<int>>
#endif
			FindTheRemainingCandidateOffsets(T value, IOptionsEquality<T2> options)
		{
			List<int> candidateOffsets = new();
			if (_index == 0)
			{
				for (int offset = 0; offset < _expectedItems.Length; offset++)
				{
					if (await AreConsideredEqual(value, _expectedItems[offset], options))
					{
						candidateOffsets.Add(offset);
					}
				}
			}
			else
			{
				foreach (int offset in _candidateOffsets)
				{
					if (offset + _index < _expectedItems.Length &&
					    await AreConsideredEqual(value, _expectedItems[offset + _index], options))
					{
						candidateOffsets.Add(offset);
					}
				}
			}

			return candidateOffsets;
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

		/// <summary>
		///     Equality compares each item with the expected item at its position, so that a deviating item never restarts
		///     the comparison; only the containment relations and interspersed items search for the expected items.
		/// </summary>
		/// <remarks>
		///     After the first deviation, the edit distance is tracked as well, so that the failure can report inserted or
		///     removed items instead of every shifted item; the positional deviations are only recorded as far as they
		///     can be listed.
		/// </remarks>
#if NET8_0_OR_GREATER
		private async ValueTask<(bool, string?)>
#else
		private async Task<(bool, string?)>
#endif
			VerifyTheCurrentValueMatchesTheItemAtItsPosition(string it, T value, IOptionsEquality<T2> options,
				int maximumNumber)
		{
			_values.Add(value);
			bool isAdditional = _index >= _expectedItems.Length;
			if (isAdditional || !await AreConsideredEqual(value, _expectedItems[_index], options))
			{
				if (_positionalDeviations++ <= 2 * maximumNumber)
				{
					if (isAdditional)
					{
						_additionalItems.Add(_index, value);
					}
					else
					{
						_incorrectItems.Add(_index, (value, _expectedItems[_index]));
					}
				}

				_editDistance ??= new BoundedEditDistance<T, T3>(_expectedItems, 2 * maximumNumber, _index);
			}

			_index++;
			if (_editDistance is not null &&
			    !await _editDistance.Add(value, (item, expected) => AreConsideredEqual(item, expected, options)))
			{
				return (true, TooManyDeviationsError(it, maximumNumber, GetDeviations()));
			}

			return (false, null);
		}

		private void VerifyTheCurrentValueIsEqualToTheExpectedValue(T value)
		{
			_matchIndex++;
			_maxMatchIndex = Math.Max(_matchIndex, _maxMatchIndex);
			_expectationIndex++;
			_matchingItems.Add((_index, value));
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
