using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using aweXpect.Core;

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
		private readonly HashSet<T> _expectedValues = new(expected);

		protected override bool IsEqualToAnExpectedItem(T value) => _expectedValues.Contains(value);

		protected override ValueTask<bool> AreConsideredEqual(T value, T expected, IOptionsEquality<T2> options)
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
		protected override ValueTask<bool>
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
		private readonly CompiledPredicates<T> _predicates = new();

		protected override ValueTask<bool> AreConsideredEqual(T value, Expression<Func<T, bool>> expected,
			IOptionsEquality<T2> options)
			=> _predicates.Invoke(expected, value);
	}

	/// <summary>
	///     Equality compares each item with the expected item at its position; the containment relations search a run
	///     or a subsequence, and their failures are described by <see cref="InOrderMismatch" />.
	/// </summary>
	private abstract class SameOrderCollectionMatcherBase<T, T2, T3> : ICollectionMatcher<T, T2>
		where T : T2
	{
		private readonly Dictionary<int, T> _additionalItems = new();
		private readonly EquivalenceRelations _equivalenceRelations;
		private readonly T3[] _expectedItems;
		private readonly bool _ignoreInterspersedItems;
		private readonly Dictionary<int, (T Item, T3 Expected)> _incorrectItems = new();
		private readonly List<T> _values = new();
		private List<int> _candidateOffsets = new();
		private BoundedEditDistance<T, T3>? _editDistance;
		private bool _isBroken;
		private bool _isFound;
		private int _lastMatchedExpectedIndex = -1;
		private int _matchIndex;
		private int _positionalDeviations;
		private List<int> _runLengths = new();
		private int _subjectItemsMatchingNothing;

		protected SameOrderCollectionMatcherBase(EquivalenceRelations equivalenceRelation,
			IEnumerable<T3> expected,
			bool ignoreInterspersedItems)
		{
			_equivalenceRelations = equivalenceRelation;
			_ignoreInterspersedItems = ignoreInterspersedItems;
			_expectedItems = expected.ToArray();
			_isFound = _expectedItems.Length == 0;
		}

		/// <inheritdoc />
		/// <remarks>
		///     Once all expected items are found, the containment relation is met, and properly met as soon as there is
		///     also another item.
		/// </remarks>
		public bool IsDetermined
			=> _equivalenceRelations.HasFlag(EquivalenceRelations.Contains) && _isFound &&
			   (!_equivalenceRelations.HasFlag(EquivalenceRelations.ContainsProperly) ||
			    _values.Count > _expectedItems.Length);

		public async ValueTask<(bool, string?)>
			Verify(string it, T value, IOptionsEquality<T2> options, int maximumNumber)
		{
			if (_equivalenceRelations.HasFlag(EquivalenceRelations.IsContainedIn))
			{
				return await VerifyTheCurrentValueIsContainedInTheExpectedItems(it, value, options, maximumNumber);
			}

			if (!_equivalenceRelations.HasFlag(EquivalenceRelations.Contains))
			{
				return await VerifyTheCurrentValueMatchesTheItemAtItsPosition(it, value, options, maximumNumber);
			}

			_values.Add(value);
			if (!_isFound)
			{
				_isFound = _ignoreInterspersedItems
					? await ContinuesTheSubsequence(value, options)
					: await CompletesARun(value, options);
			}

			// Any later item can still complete the expected items, so no deviation is known before the end.
			return (false, null);
		}

		public async ValueTask<(bool, string?)>
			VerifyComplete(string it, IOptionsEquality<T2> options, int maximumNumber)
		{
			if (!_equivalenceRelations.HasFlag(EquivalenceRelations.Contains) &&
			    !_equivalenceRelations.HasFlag(EquivalenceRelations.IsContainedIn))
			{
				return await VerifyCompleteForPositionalMatch(it, options, maximumNumber);
			}

			if (IsDetermined || IsContainedInTheExpectedItems())
			{
				return (false, null);
			}

			string? error = (await FindTheDeviations(options)).GetError(it, _equivalenceRelations, false,
				_expectedItems.Length, options, maximumNumber);
			return (true, error);
		}

		/// <summary>
		///     The proper containment needs an expected item that the subject does not use.
		/// </summary>
		private bool IsContainedInTheExpectedItems()
			=> _equivalenceRelations.HasFlag(EquivalenceRelations.IsContainedIn) && !_isBroken &&
			   (!_equivalenceRelations.HasFlag(EquivalenceRelations.IsContainedInProperly) ||
			    _values.Count < _expectedItems.Length);

		/// <summary>
		///     The containment searches the subject in the expected items, the other relations search the expected items
		///     in the subject.
		/// </summary>
		private async ValueTask<InOrderDeviations<T, T3>> FindTheDeviations(IOptionsEquality<T2> options)
		{
			OrderMatch orderMatch = _ignoreInterspersedItems ? OrderMatch.Subsequence : OrderMatch.Contiguous;
			if (_equivalenceRelations.HasFlag(EquivalenceRelations.IsContainedIn))
			{
				InOrderMismatch searchedInExpected = await InOrderMismatch.Explain(
					Enumerable.Range(0, _expectedItems.Length).ToArray(), _expectedItems.Length, _values.Count,
					(expectedIndex, index) => AreConsideredEqual(_values[index], _expectedItems[expectedIndex], options),
					true, orderMatch);
				return InOrderDeviations<T, T3>.From(searchedInExpected, true,
					index => (index, _values[index]), expectedIndex => _expectedItems[expectedIndex]);
			}

			InOrderMismatch searchedInSubject = await InOrderMismatch.Explain(
				Enumerable.Range(0, _values.Count).ToArray(), _values.Count, _expectedItems.Length,
				(index, expectedIndex) => AreConsideredEqual(_values[index], _expectedItems[expectedIndex], options),
				true, orderMatch);
			return InOrderDeviations<T, T3>.From(searchedInSubject, false,
				index => (index, _values[index]), expectedIndex => _expectedItems[expectedIndex]);
		}

		/// <summary>
		///     Keeps the length of each partial run that the <paramref name="value" /> continues, and starts a new one,
		///     because the expected items can overlap with themselves, e.g. <c>[1, 1, 2]</c> in <c>[1, 1, 1, 2]</c>.
		/// </summary>
		/// <returns><see langword="true" />, when a run is complete.</returns>
		private async ValueTask<bool> CompletesARun(T value, IOptionsEquality<T2> options)
		{
			List<int> runLengths = new();
			foreach (int runLength in _runLengths)
			{
				if (await AreConsideredEqual(value, _expectedItems[runLength], options))
				{
					runLengths.Add(runLength + 1);
				}
			}

			if (await AreConsideredEqual(value, _expectedItems[0], options))
			{
				runLengths.Add(1);
			}

			_runLengths = runLengths;
			return runLengths.Count > 0 && runLengths[0] == _expectedItems.Length;
		}

		/// <summary>
		///     Taking the first item that matches the next expected item never prevents finding a subsequence.
		/// </summary>
		/// <returns><see langword="true" />, when all expected items are found.</returns>
		private async ValueTask<bool> ContinuesTheSubsequence(T value, IOptionsEquality<T2> options)
		{
			if (await AreConsideredEqual(value, _expectedItems[_matchIndex], options))
			{
				_matchIndex++;
			}

			return _matchIndex == _expectedItems.Length;
		}

		/// <summary>
		///     Once the subject leaves the expected items, the failure is certain; it aborts early, when more items are
		///     unexpected regardless of the order than can be listed.
		/// </summary>
		private async ValueTask<(bool, string?)>
			VerifyTheCurrentValueIsContainedInTheExpectedItems(string it, T value, IOptionsEquality<T2> options,
				int maximumNumber)
		{
			_values.Add(value);
			if (!_isBroken)
			{
				_isBroken = _ignoreInterspersedItems
					? !await ContinuesTheSubsequenceInTheExpectedItems(value, options)
					: !await ContinuesTheRunInTheExpectedItems(value, options);
				if (!_isBroken)
				{
					return (false, null);
				}
			}

			if (!IsEqualToAnExpectedItem(value) && !await MatchesAnExpectedItem(value, options))
			{
				_subjectItemsMatchingNothing++;
			}

			// One-to-one, the items beyond the number of expected items are unexpected as well.
			long unexpectedItems = _subjectItemsMatchingNothing +
			                       Math.Max(0, _values.Count - _subjectItemsMatchingNothing - _expectedItems.Length);
			return unexpectedItems > 2L * maximumNumber
				? (true, TooManyDeviationsError(it, maximumNumber,
					(await FindTheDeviations(options)).ListDeviations(_equivalenceRelations, options)))
				: (false, null);
		}

		/// <summary>
		///     The subject continues the run at every offset in the expected items at which all its items so far match.
		/// </summary>
		private async ValueTask<bool> ContinuesTheRunInTheExpectedItems(T value, IOptionsEquality<T2> options)
		{
			int index = _values.Count - 1;
			List<int> candidateOffsets = new();
			IEnumerable<int> offsets = index == 0 ? Enumerable.Range(0, _expectedItems.Length) : _candidateOffsets;
			foreach (int offset in offsets)
			{
				if (offset + index < _expectedItems.Length &&
				    await AreConsideredEqual(value, _expectedItems[offset + index], options))
				{
					candidateOffsets.Add(offset);
				}
			}

			_candidateOffsets = candidateOffsets;
			return candidateOffsets.Count > 0;
		}

		/// <summary>
		///     Taking the first expected item that matches never prevents finding the subject as a subsequence.
		/// </summary>
		private async ValueTask<bool> ContinuesTheSubsequenceInTheExpectedItems(T value, IOptionsEquality<T2> options)
		{
			for (int i = _matchIndex; i < _expectedItems.Length; i++)
			{
				if (await AreConsideredEqual(value, _expectedItems[i], options))
				{
					_matchIndex = i + 1;
					return true;
				}
			}

			return false;
		}

		/// <summary>
		///     Avoids comparing each item with the expected items, when it is known to match one of them.
		/// </summary>
		/// <remarks>
		///     This only decides, whether the comparison can abort early, so it may err towards a match.
		/// </remarks>
		protected virtual bool IsEqualToAnExpectedItem(T value) => false;

		private async ValueTask<bool> MatchesAnExpectedItem(T value, IOptionsEquality<T2> options)
		{
			int expectedIndex = await FindNear(_lastMatchedExpectedIndex, _expectedItems.Length,
				index => AreConsideredEqual(value, _expectedItems[index], options));
			if (expectedIndex < 0)
			{
				return false;
			}

			_lastMatchedExpectedIndex = expectedIndex;
			return true;
		}

		/// <summary>
		///     Additional items are no deviation for the containment relation, so they are left out.
		/// </summary>
		private IEnumerable<string> GetDeviations(IOptionsEquality<T2> options)
			=> IncorrectItemsError(_incorrectItems, options)
				.Concat(AdditionalItemsError(_additionalItems, CreateItemFormatter()));

		/// <summary>
		///     Every subject item was compared with the expected item at its position, so the expected items beyond the
		///     end of the subject are missing; when fewer edits align the subject with the expected items, e.g. because
		///     an item was inserted, these edits are reported instead.
		/// </summary>
		private async ValueTask<(bool, string?)>
			VerifyCompleteForPositionalMatch(string it, IOptionsEquality<T2> options, int maximumNumber)
		{
			int positionalDeviations = _positionalDeviations + Math.Max(0, _expectedItems.Length - _values.Count);
			if (_editDistance is not null)
			{
				List<(EditKind Kind, int SubjectIndex, int ExpectedIndex)>? edits = await _editDistance.GetEdits(
					_values, (item, expected) => AreConsideredEqual(item, expected, options));
				if (edits is not null && edits.Count < positionalDeviations)
				{
					return await ReturnEditsError(it, edits, options, maximumNumber);
				}
			}

			if (positionalDeviations > 2L * maximumNumber)
			{
				return (true, TooManyDeviationsError(it, maximumNumber, GetDeviations(options)));
			}

			List<T3> missingItems = _expectedItems.Skip(_values.Count).ToList();
			return ReturnError(it, _incorrectItems, new Dictionary<int, T>(), _additionalItems, missingItems, options,
				maximumNumber);
		}

		/// <summary>
		///     An additional item that matches a missing item was moved, so both are reported as one item in the wrong
		///     order.
		/// </summary>
		private async ValueTask<(bool, string?)>
			ReturnEditsError(string it, List<(EditKind Kind, int SubjectIndex, int ExpectedIndex)> edits,
				IOptionsEquality<T2> options, int maximumNumber)
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

			Dictionary<int, T> outOfOrderItems = new();
			foreach (KeyValuePair<int, T> additionalItem in additionalItems.ToList())
			{
				for (int i = 0; i < missingItems.Count; i++)
				{
					if (await AreConsideredEqual(additionalItem.Value, missingItems[i], options))
					{
						missingItems.RemoveAt(i);
						additionalItems.Remove(additionalItem.Key);
						outOfOrderItems.Add(additionalItem.Key, additionalItem.Value);
						break;
					}
				}
			}

			return ReturnError(it, incorrectItems, outOfOrderItems, additionalItems, missingItems, options,
				maximumNumber);
		}

		private (bool, string?) ReturnError(string it, Dictionary<int, (T Item, T3 Expected)> incorrectItems,
			Dictionary<int, T> outOfOrderItems, Dictionary<int, T> additionalItems, List<T3> missingItems,
			IOptionsEquality<T2> options, int maximumNumber)
		{
			Func<object?, string> formatItem =
				GetItemFormatter(additionalItems.Values.Cast<object?>(), missingItems.Cast<object?>());
			List<string> errors = new();
			errors.AddRange(IncorrectItemsError(incorrectItems, options));
			errors.AddRange(OutOfOrderItemsError(outOfOrderItems));
			errors.AddRange(AdditionalItemsError(additionalItems, formatItem));
			errors.AddRange(MissingItemsError(_expectedItems.Length, missingItems, _equivalenceRelations, false,
				formatItem, options, maximumNumber));

			string? error = ReturnErrorString(it, errors);
			return (error != null, error);
		}

		/// <summary>
		///     Equality compares each item with the expected item at its position, so that a deviating item never restarts
		///     the comparison.
		/// </summary>
		/// <remarks>
		///     After the first deviation, the edit distance is tracked as well, so that the failure can report inserted or
		///     removed items instead of every shifted item; the positional deviations are only recorded as far as they
		///     can be listed.
		/// </remarks>
		private async ValueTask<(bool, string?)>
			VerifyTheCurrentValueMatchesTheItemAtItsPosition(string it, T value, IOptionsEquality<T2> options,
				int maximumNumber)
		{
			int index = _values.Count;
			_values.Add(value);
			bool isAdditional = index >= _expectedItems.Length;
			if (isAdditional || !await AreConsideredEqual(value, _expectedItems[index], options))
			{
				if (_positionalDeviations++ <= 2L * maximumNumber)
				{
					if (isAdditional)
					{
						_additionalItems.Add(index, value);
					}
					else
					{
						_incorrectItems.Add(index, (value, _expectedItems[index]));
					}
				}

				_editDistance ??= new BoundedEditDistance<T, T3>(_expectedItems,
					(int)Math.Min(2L * maximumNumber, int.MaxValue), index);
			}

			if (_editDistance is not null &&
			    !await _editDistance.Add(value, (item, expected) => AreConsideredEqual(item, expected, options)))
			{
				return (true, TooManyDeviationsError(it, maximumNumber, GetDeviations(options)));
			}

			return (false, null);
		}

		/// <summary>
		///     An unexpected and a missing item that format equally differ only in their runtime type.
		/// </summary>
		private Func<object?, string> CreateItemFormatter()
			=> GetItemFormatter(_additionalItems.Values.Cast<object?>(), []);

		protected abstract ValueTask<bool>
			AreConsideredEqual(T value, T3 expected, IOptionsEquality<T2> options);
	}
}
