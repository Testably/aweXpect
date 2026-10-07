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
		bool ignoreInterspersedItems,
		bool addsInAnyOrderHint,
		int[]? dimensions)
		: SameOrderCollectionMatcherBase<T, T2, T>(equivalenceRelation, expected, ignoreInterspersedItems,
			addsInAnyOrderHint, dimensions)
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

		protected override ICollectionMatcher<T, T2> CreateAnyOrderMatcher()
			=> new AnyOrderCollectionMatcher<T, T2>(EquivalenceRelation, ExpectedItems, null);
	}

	private sealed class SameOrderFromExpectationCollectionMatcher<T, T2>(
		EquivalenceRelations equivalenceRelation,
		IEnumerable<ExpectationItem<T>> expected,
		bool ignoreInterspersedItems,
		bool addsInAnyOrderHint)
		: SameOrderCollectionMatcherBase<T, T2, ExpectationItem<T>>(equivalenceRelation, expected,
			ignoreInterspersedItems, addsInAnyOrderHint)
		where T : T2
	{
		protected override ValueTask<bool> AreConsideredEqual(int index, T value, ExpectationItem<T> expected,
			IOptionsEquality<T2> options)
			=> expected.IsMetBy(value, index);

		protected override ICollectionMatcher<T, T2> CreateAnyOrderMatcher()
			=> new AnyOrderFromExpectationCollectionMatcher<T, T2>(EquivalenceRelation, ExpectedItems);
	}

	private sealed class SameOrderFromPredicateCollectionMatcher<T, T2>(
		EquivalenceRelations equivalenceRelation,
		IEnumerable<Expression<Func<T, bool>>> expected,
		bool ignoreInterspersedItems,
		bool addsInAnyOrderHint)
		: SameOrderCollectionMatcherBase<T, T2, Expression<Func<T, bool>>>(equivalenceRelation, expected,
			ignoreInterspersedItems, addsInAnyOrderHint)
		where T : T2
	{
		private readonly CompiledPredicates<T> _predicates = new();

		protected override ValueTask<bool> AreConsideredEqual(int index, T value, Expression<Func<T, bool>> expected,
			IOptionsEquality<T2> options)
			=> _predicates.Invoke(expected, value, index);

		protected override ICollectionMatcher<T, T2> CreateAnyOrderMatcher()
			=> new AnyOrderFromPredicateCollectionMatcher<T, T2>(EquivalenceRelation, ExpectedItems);
	}

	/// <summary>
	///     Equality compares each item with the expected item at its position; the containment relations search a run
	///     or a subsequence, and their failures are described by <see cref="InOrderMismatch" />.
	/// </summary>
	private abstract class SameOrderCollectionMatcherBase<T, T2, T3> : ICollectionMatcher<T, T2>
		where T : T2
	{
		/// <summary>
		///     Whether a failure gets a hint, when the same items match in any order.
		/// </summary>
		private readonly bool _addsInAnyOrderHint;

		/// <summary>
		///     The dimensions of a subject that is an array of rank greater than one.
		/// </summary>
		private readonly int[]? _dimensions;

		private readonly bool _ignoreInterspersedItems;
		private readonly List<T> _values;

		/// <summary>
		///     The deviating items, which are only created for the first deviation, as a met expectation has none.
		/// </summary>
		private Dictionary<int, T>? _additionalItems;

		/// <summary>
		///     The lists of the run searches, which only the containment relations create.
		/// </summary>
		private List<int>? _candidateOffsets;

		private BoundedEditDistance<T3>? _editDistance;

		/// <inheritdoc cref="_additionalItems" />
		private Dictionary<int, (T Item, T3 Expected)>? _incorrectItems;

		private bool _isBroken;
		private bool _isFound;
		private int _lastMatchedExpectedIndex = -1;
		private int _matchIndex;

		/// <summary>
		///     The list that the next item fills for <see cref="_candidateOffsets" /> or <see cref="_runLengths" />, so
		///     that the two are swapped instead of allocating a new list for every item.
		/// </summary>
		private List<int>? _nextOffsetsOrRunLengths;

		private int _positionalDeviations;

		/// <inheritdoc cref="_candidateOffsets" />
		private List<int>? _runLengths;

		private int _subjectItemsMatchingNothing;

		protected SameOrderCollectionMatcherBase(EquivalenceRelations equivalenceRelation,
			IEnumerable<T3> expected,
			bool ignoreInterspersedItems,
			bool addsInAnyOrderHint,
			int[]? dimensions = null)
		{
			EquivalenceRelation = equivalenceRelation;
			_ignoreInterspersedItems = ignoreInterspersedItems;
			_addsInAnyOrderHint = addsInAnyOrderHint;
			_dimensions = dimensions;
			ExpectedItems = expected as T3[] ?? expected.ToArray();
			_values = new List<T>(ExpectedItems.Length);
			_isFound = ExpectedItems.Length == 0;
		}

		protected EquivalenceRelations EquivalenceRelation { get; }

		protected T3[] ExpectedItems { get; }

		/// <inheritdoc />
		/// <remarks>
		///     Once all expected items are found, the containment relation is met, and properly met as soon as there is
		///     also another item.
		/// </remarks>
		public bool IsDetermined
			=> EquivalenceRelation.Includes(EquivalenceRelations.Contains) && _isFound &&
			   (!EquivalenceRelation.Includes(EquivalenceRelations.ContainsProperly) ||
			    _values.Count > ExpectedItems.Length);

		public ValueTask<(bool, string?)>
			Verify(string it, T value, IOptionsEquality<T2> options, int maximumNumber)
		{
			if (EquivalenceRelation.Includes(EquivalenceRelations.IsContainedIn))
			{
				return VerifyTheCurrentValueIsContainedInTheExpectedItems(it, value, options, maximumNumber);
			}

			if (!EquivalenceRelation.Includes(EquivalenceRelations.Contains))
			{
				return VerifyTheCurrentValueMatchesTheItemAtItsPosition(it, value, options, maximumNumber);
			}

			return VerifyTheCurrentValueContainsTheExpectedItems(value, options);
		}

		/// <remarks>
		///     The matcher that checks the items in any order for the hint is only created for a failure. Equality
		///     without a deviation and with as many items as expected is met, which returns without a state machine.
		/// </remarks>
		public ValueTask<(bool, string?)>
			VerifyComplete(string it, IOptionsEquality<T2> options, int maximumNumber)
		{
			if (_positionalDeviations == 0 && _values.Count == ExpectedItems.Length &&
			    !EquivalenceRelation.Includes(EquivalenceRelations.Contains) &&
			    !EquivalenceRelation.Includes(EquivalenceRelations.IsContainedIn))
			{
				return new ValueTask<(bool, string?)>((false, null));
			}

			return _addsInAnyOrderHint
				? VerifyCompleteWithInAnyOrderHint(it, options, maximumNumber)
				: VerifyCompleteInOrder(it, options, maximumNumber);
		}

		/// <remarks>
		///     Any later item can still complete the expected items, so no deviation is known before the end. An item
		///     whose comparisons complete synchronously returns without a state machine.
		/// </remarks>
		private ValueTask<(bool, string?)>
			VerifyTheCurrentValueContainsTheExpectedItems(T value, IOptionsEquality<T2> options)
		{
			_values.Add(value);
			if (_isFound)
			{
				return new ValueTask<(bool, string?)>((false, null));
			}

			ValueTask<bool> isFound = _ignoreInterspersedItems
				? ContinuesTheSubsequence(_values.Count - 1, options)
				: CompletesARun(_values.Count - 1, options);
			if (!isFound.IsCompletedSuccessfully)
			{
				return KeepWhetherTheExpectedItemsAreFound(isFound);
			}

			_isFound = isFound.Result;
			return new ValueTask<(bool, string?)>((false, null));
		}

		private async ValueTask<(bool, string?)> KeepWhetherTheExpectedItemsAreFound(ValueTask<bool> isFound)
		{
			_isFound = await isFound;
			return (false, null);
		}

		private async ValueTask<(bool, string?)>
			VerifyCompleteWithInAnyOrderHint(string it, IOptionsEquality<T2> options, int maximumNumber)
		{
			(bool isFailure, string? error) = await VerifyCompleteInOrder(it, options, maximumNumber);
			if (error is null || !await MatchesInAnyOrder(it, options, maximumNumber))
			{
				return (isFailure, error);
			}

			return (isFailure, error + Environment.NewLine + ItemsMatchInADifferentOrderHint);
		}

		private async ValueTask<bool> MatchesInAnyOrder(string it, IOptionsEquality<T2> options, int maximumNumber)
		{
			ICollectionMatcher<T, T2> matcher = CreateAnyOrderMatcher();
			foreach (T value in _values)
			{
				(bool isFailure, string? _) = await matcher.Verify(it, value, options, maximumNumber);
				if (isFailure)
				{
					return false;
				}
			}

			(bool isCompleteFailure, string? _) = await matcher.VerifyComplete(it, options, maximumNumber);
			return !isCompleteFailure;
		}

		private async ValueTask<(bool, string?)>
			VerifyCompleteInOrder(string it, IOptionsEquality<T2> options, int maximumNumber)
		{
			if (!EquivalenceRelation.Includes(EquivalenceRelations.Contains) &&
			    !EquivalenceRelation.Includes(EquivalenceRelations.IsContainedIn))
			{
				return await VerifyCompleteForPositionalMatch(it, options, maximumNumber);
			}

			if (IsDetermined || IsContainedInTheExpectedItems())
			{
				return (false, null);
			}

			string? error = (await FindTheDeviations(options)).GetError(it, EquivalenceRelation, false,
				ExpectedItems.Length, options, maximumNumber);
			return (true, error);
		}

		/// <summary>
		///     The proper containment needs an expected item that the subject does not use.
		/// </summary>
		private bool IsContainedInTheExpectedItems()
			=> EquivalenceRelation.Includes(EquivalenceRelations.IsContainedIn) && !_isBroken &&
			   (!EquivalenceRelation.Includes(EquivalenceRelations.IsContainedInProperly) ||
			    _values.Count < ExpectedItems.Length);

		/// <summary>
		///     The containment searches the subject in the expected items, the other relations search the expected items
		///     in the subject.
		/// </summary>
		private async ValueTask<InOrderDeviations<T, T3>> FindTheDeviations(IOptionsEquality<T2> options)
		{
			OrderMatch orderMatch = _ignoreInterspersedItems ? OrderMatch.Subsequence : OrderMatch.Contiguous;
			if (EquivalenceRelation.Includes(EquivalenceRelations.IsContainedIn))
			{
				InOrderMismatch searchedInExpected = await InOrderMismatch.Explain(
					Enumerable.Range(0, ExpectedItems.Length).ToArray(), ExpectedItems.Length, _values.Count,
					(expectedIndex, index) => IsMatch(index, ExpectedItems[expectedIndex], options),
					true, orderMatch);
				return InOrderDeviations<T, T3>.From(searchedInExpected, true,
					index => (index, _values[index]), expectedIndex => ExpectedItems[expectedIndex], _dimensions);
			}

			InOrderMismatch searchedInSubject = await InOrderMismatch.Explain(
				Enumerable.Range(0, _values.Count).ToArray(), _values.Count, ExpectedItems.Length,
				(index, expectedIndex) => IsMatch(index, ExpectedItems[expectedIndex], options),
				true, orderMatch);
			return InOrderDeviations<T, T3>.From(searchedInSubject, false,
				index => (index, _values[index]), expectedIndex => ExpectedItems[expectedIndex], _dimensions);
		}

		/// <summary>
		///     Keeps the length of each partial run that the item at the <paramref name="index" /> continues, and starts a
		///     new one, because the expected items can overlap with themselves, e.g. <c>[1, 1, 2]</c> in <c>[1, 1, 1, 2]</c>.
		/// </summary>
		/// <returns><see langword="true" />, when a run is complete.</returns>
		/// <remarks>
		///     After the partial runs, the item starts a new one, i.e. continues a run of length <c>0</c>.
		/// </remarks>
		private ValueTask<bool> CompletesARun(int index, IOptionsEquality<T2> options)
		{
			List<int> runLengths = _nextOffsetsOrRunLengths ??= new List<int>();
			runLengths.Clear();
			int count = _runLengths?.Count ?? 0;
			for (int i = 0; i <= count; i++)
			{
				int runLength = RunLengthAt(i, count);
				ValueTask<bool> isMatch = IsMatch(index, ExpectedItems[runLength], options);
				if (!isMatch.IsCompletedSuccessfully)
				{
					return CompletesARunAsync(isMatch, index, i, count, runLengths, options);
				}

				if (isMatch.Result)
				{
					runLengths.Add(runLength + 1);
				}
			}

			return new ValueTask<bool>(KeepTheRunLengths(runLengths));
		}

		private async ValueTask<bool> CompletesARunAsync(ValueTask<bool> isMatch, int index, int i, int count,
			List<int> runLengths, IOptionsEquality<T2> options)
		{
			while (true)
			{
				if (await isMatch)
				{
					runLengths.Add(RunLengthAt(i, count) + 1);
				}

				if (++i > count)
				{
					return KeepTheRunLengths(runLengths);
				}

				isMatch = IsMatch(index, ExpectedItems[RunLengthAt(i, count)], options);
			}
		}

		private int RunLengthAt(int i, int count)
			=> i < count ? _runLengths![i] : 0;

		private bool KeepTheRunLengths(List<int> runLengths)
		{
			_nextOffsetsOrRunLengths = _runLengths;
			_runLengths = runLengths;
			return runLengths.Count > 0 && runLengths[0] == ExpectedItems.Length;
		}

		/// <summary>
		///     Taking the first item that matches the next expected item never prevents finding a subsequence.
		/// </summary>
		/// <returns><see langword="true" />, when all expected items are found.</returns>
		private ValueTask<bool> ContinuesTheSubsequence(int index, IOptionsEquality<T2> options)
		{
			ValueTask<bool> isMatch = IsMatch(index, ExpectedItems[_matchIndex], options);
			if (!isMatch.IsCompletedSuccessfully)
			{
				return ContinuesTheSubsequenceAsync(isMatch);
			}

			return new ValueTask<bool>(AdvancesTheSubsequence(isMatch.Result));
		}

		private async ValueTask<bool> ContinuesTheSubsequenceAsync(ValueTask<bool> isMatch)
			=> AdvancesTheSubsequence(await isMatch);

		private bool AdvancesTheSubsequence(bool isMatch)
		{
			if (isMatch)
			{
				_matchIndex++;
			}

			return _matchIndex == ExpectedItems.Length;
		}

		/// <summary>
		///     Once the subject leaves the expected items, the failure is certain; it aborts early, when more items are
		///     unexpected regardless of the order than can be listed.
		/// </summary>
		/// <remarks>
		///     An item that continues the expected items and whose comparisons complete synchronously returns without a
		///     state machine.
		/// </remarks>
		private ValueTask<(bool, string?)>
			VerifyTheCurrentValueIsContainedInTheExpectedItems(string it, T value, IOptionsEquality<T2> options,
				int maximumNumber)
		{
			_values.Add(value);
			if (_isBroken)
			{
				return CountTheUnexpectedItems(it, value, options, maximumNumber);
			}

			ValueTask<bool> continues = ContinuesInTheExpectedItems(_values.Count - 1, options);
			if (!continues.IsCompletedSuccessfully)
			{
				return VerifyWhetherTheExpectedItemsAreLeft(continues, it, value, options, maximumNumber);
			}

			if (continues.Result)
			{
				return new ValueTask<(bool, string?)>((false, null));
			}

			_isBroken = true;
			return CountTheUnexpectedItems(it, value, options, maximumNumber);
		}

		private ValueTask<bool> ContinuesInTheExpectedItems(int index, IOptionsEquality<T2> options)
		{
			if (_ignoreInterspersedItems)
			{
				return ContinuesTheSubsequenceInTheExpectedItems(index, options);
			}

			return ContinuesTheRunInTheExpectedItems(index, options);
		}

		private async ValueTask<(bool, string?)> VerifyWhetherTheExpectedItemsAreLeft(ValueTask<bool> continues,
			string it, T value, IOptionsEquality<T2> options, int maximumNumber)
		{
			_isBroken = !await continues;
			return _isBroken
				? await CountTheUnexpectedItems(it, value, options, maximumNumber)
				: (false, null);
		}

		private async ValueTask<(bool, string?)> CountTheUnexpectedItems(string it, T value,
			IOptionsEquality<T2> options, int maximumNumber)
		{
			if (!IsEqualToAnExpectedItem(value) && !await MatchesAnExpectedItem(_values.Count - 1, options))
			{
				_subjectItemsMatchingNothing++;
			}

			// One-to-one, the items beyond the number of expected items are unexpected as well.
			long unexpectedItems = _subjectItemsMatchingNothing +
			                       Math.Max(0, _values.Count - _subjectItemsMatchingNothing - ExpectedItems.Length);
			return unexpectedItems > 2L * maximumNumber
				? (true, TooManyDeviationsError(it, maximumNumber,
					(await FindTheDeviations(options)).ListDeviations(EquivalenceRelation, options)))
				: (false, null);
		}

		/// <summary>
		///     The subject continues the run at every offset in the expected items at which all its items so far match.
		/// </summary>
		private ValueTask<bool> ContinuesTheRunInTheExpectedItems(int index, IOptionsEquality<T2> options)
		{
			List<int> candidateOffsets = _nextOffsetsOrRunLengths ??= new List<int>();
			candidateOffsets.Clear();
			int count = index == 0 ? ExpectedItems.Length : _candidateOffsets!.Count;
			for (int i = 0; i < count; i++)
			{
				int offset = OffsetAt(index, i);
				if (offset + index < ExpectedItems.Length)
				{
					ValueTask<bool> isMatch = IsMatch(index, ExpectedItems[offset + index], options);
					if (!isMatch.IsCompletedSuccessfully)
					{
						return ContinuesTheRunInTheExpectedItemsAsync(isMatch, index, i, count, candidateOffsets,
							options);
					}

					if (isMatch.Result)
					{
						candidateOffsets.Add(offset);
					}
				}
			}

			return new ValueTask<bool>(KeepTheCandidateOffsets(candidateOffsets));
		}

		private async ValueTask<bool> ContinuesTheRunInTheExpectedItemsAsync(ValueTask<bool> isMatch, int index,
			int i, int count, List<int> candidateOffsets, IOptionsEquality<T2> options)
		{
			if (await isMatch)
			{
				candidateOffsets.Add(OffsetAt(index, i));
			}

			for (i++; i < count; i++)
			{
				int offset = OffsetAt(index, i);
				if (offset + index < ExpectedItems.Length &&
				    await IsMatch(index, ExpectedItems[offset + index], options))
				{
					candidateOffsets.Add(offset);
				}
			}

			return KeepTheCandidateOffsets(candidateOffsets);
		}

		/// <summary>
		///     The first item can start at every offset, the later ones only continue the remaining candidates.
		/// </summary>
		private int OffsetAt(int index, int i)
			=> index == 0 ? i : _candidateOffsets![i];

		private bool KeepTheCandidateOffsets(List<int> candidateOffsets)
		{
			_nextOffsetsOrRunLengths = _candidateOffsets;
			_candidateOffsets = candidateOffsets;
			return candidateOffsets.Count > 0;
		}

		/// <summary>
		///     Taking the first expected item that matches never prevents finding the subject as a subsequence.
		/// </summary>
		private ValueTask<bool> ContinuesTheSubsequenceInTheExpectedItems(int index, IOptionsEquality<T2> options)
		{
			for (int i = _matchIndex; i < ExpectedItems.Length; i++)
			{
				ValueTask<bool> isMatch = IsMatch(index, ExpectedItems[i], options);
				if (!isMatch.IsCompletedSuccessfully)
				{
					return ContinuesTheSubsequenceInTheExpectedItemsAsync(isMatch, index, i, options);
				}

				if (isMatch.Result)
				{
					_matchIndex = i + 1;
					return new ValueTask<bool>(true);
				}
			}

			return new ValueTask<bool>(false);
		}

		private async ValueTask<bool> ContinuesTheSubsequenceInTheExpectedItemsAsync(ValueTask<bool> isMatch,
			int index, int i, IOptionsEquality<T2> options)
		{
			while (!await isMatch)
			{
				if (++i >= ExpectedItems.Length)
				{
					return false;
				}

				isMatch = IsMatch(index, ExpectedItems[i], options);
			}

			_matchIndex = i + 1;
			return true;
		}

		/// <summary>
		///     Avoids comparing each item with the expected items, when it is known to match one of them.
		/// </summary>
		/// <remarks>
		///     This only decides, whether the comparison can abort early, so it may err towards a match.
		/// </remarks>
		protected virtual bool IsEqualToAnExpectedItem(T value) => false;

		private async ValueTask<bool> MatchesAnExpectedItem(int index, IOptionsEquality<T2> options)
		{
			int expectedIndex = await FindNear(_lastMatchedExpectedIndex, ExpectedItems.Length,
				candidate => IsMatch(index, ExpectedItems[candidate], options));
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
			=> IncorrectItemsError(_incorrectItems ?? new Dictionary<int, (T Item, T3 Expected)>(), options,
					_dimensions)
				.Concat(AdditionalItemsError(_additionalItems ?? new Dictionary<int, T>(), CreateItemFormatter(),
					_dimensions));

		/// <summary>
		///     Every subject item was compared with the expected item at its position, so the expected items beyond the
		///     end of the subject are missing; when fewer edits align the subject with the expected items, e.g. because
		///     an item was inserted, these edits are reported instead.
		/// </summary>
		private async ValueTask<(bool, string?)>
			VerifyCompleteForPositionalMatch(string it, IOptionsEquality<T2> options, int maximumNumber)
		{
			int positionalDeviations = _positionalDeviations + Math.Max(0, ExpectedItems.Length - _values.Count);
			if (_editDistance is not null)
			{
				List<(EditKind Kind, int SubjectIndex, int ExpectedIndex)>? edits = await _editDistance.GetEdits((index, expected) => IsMatch(index, expected, options));
				if (edits is not null && edits.Count < positionalDeviations)
				{
					return await ReturnEditsError(it, edits, options, maximumNumber);
				}
			}

			List<T3> missingItems = ExpectedItems.Skip(_values.Count).ToList();
			// Without a deviating item, all deviations are missing items, which are known completely here.
			if (_positionalDeviations > 0 && positionalDeviations > 2L * maximumNumber)
			{
				return (true, TooManyDeviationsError(it, maximumNumber, GetDeviations(options),
					MissingItemsError(ExpectedItems.Length, missingItems, EquivalenceRelation, false,
						CreateItemFormatter(), options, maximumNumber)));
			}

			return ReturnError(it, _incorrectItems ?? new Dictionary<int, (T Item, T3 Expected)>(),
				new Dictionary<int, T>(), _additionalItems ?? new Dictionary<int, T>(), missingItems, options,
				maximumNumber);
		}

		/// <summary>
		///     An additional item that matches a missing item was moved, so both are reported as one item in the wrong
		///     order.
		/// </summary>
		/// <remarks>
		///     An item can match several missing items, so the items are paired by a maximum matching, which reports as
		///     few items as not expected or missing as possible.
		/// </remarks>
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
						incorrectItems.Add(subjectIndex, (_values[subjectIndex], ExpectedItems[expectedIndex]));
						break;
					case EditKind.Additional:
						additionalItems.Add(subjectIndex, _values[subjectIndex]);
						break;
					default:
						missingItems.Add(ExpectedItems[expectedIndex]);
						break;
				}
			}

			Dictionary<int, T> unexpectedItems = new();
			ItemMatching<T, T3> matching = new(missingItems,
				(index, _, expected) => IsMatch(index, expected, options), unexpectedItems);
			foreach (KeyValuePair<int, T> additionalItem in additionalItems)
			{
				await matching.Add(additionalItem.Key, additionalItem.Value);
				await matching.ResolvePendingItems();
			}

			Dictionary<int, T> outOfOrderItems = matching.MatchedPairs()
				.ToDictionary(pair => pair.Index, pair => _values[pair.Index]);
			return ReturnError(it, incorrectItems, outOfOrderItems, unexpectedItems,
				matching.UnmatchedExpectedItems(), options, maximumNumber);
		}

		private (bool, string?) ReturnError(string it, Dictionary<int, (T Item, T3 Expected)> incorrectItems,
			Dictionary<int, T> outOfOrderItems, Dictionary<int, T> additionalItems, List<T3> missingItems,
			IOptionsEquality<T2> options, int maximumNumber)
		{
			Func<object?, string> formatItem =
				GetItemFormatter(additionalItems.Values.Cast<object?>(), missingItems.Cast<object?>());
			List<string> errors = new();
			errors.AddRange(IncorrectItemsError(incorrectItems, options, _dimensions));
			errors.AddRange(OutOfOrderItemsError(outOfOrderItems, _dimensions));
			errors.AddRange(AdditionalItemsError(additionalItems, formatItem, _dimensions));
			errors.AddRange(MissingItemsError(ExpectedItems.Length, missingItems, EquivalenceRelation, false,
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
		/// <remarks>
		///     An item that matches synchronously before the first deviation returns without a state machine, because
		///     every item of a met expectation does.
		/// </remarks>
		private ValueTask<(bool, string?)>
			VerifyTheCurrentValueMatchesTheItemAtItsPosition(string it, T value, IOptionsEquality<T2> options,
				int maximumNumber)
		{
			int index = _values.Count;
			_values.Add(value);
			return index < ExpectedItems.Length
				? ContinueAfterTheComparison(IsMatch(index, ExpectedItems[index], options), it, index, value, options,
					maximumNumber)
				: ContinueAfterADeviation(it, index, value, options, maximumNumber);
		}

		private ValueTask<(bool, string?)> ContinueAfterTheComparison(ValueTask<bool> isMatch, string it, int index,
			T value, IOptionsEquality<T2> options, int maximumNumber)
		{
			if (_editDistance is null && isMatch.IsCompletedSuccessfully)
			{
				return isMatch.Result
					? new ValueTask<(bool, string?)>((false, null))
					: ContinueAfterADeviation(it, index, value, options, maximumNumber);
			}

			return ContinueAfterTheComparisonAsync(isMatch, it, index, value, options, maximumNumber);
		}

		private async ValueTask<(bool, string?)> ContinueAfterTheComparisonAsync(ValueTask<bool> isMatch, string it,
			int index, T value, IOptionsEquality<T2> options, int maximumNumber)
		{
			if (!await isMatch)
			{
				RecordADeviation(index, value, maximumNumber);
			}

			return await ContinueTheEditDistance(it, options, maximumNumber);
		}

		private async ValueTask<(bool, string?)> ContinueAfterADeviation(string it, int index, T value,
			IOptionsEquality<T2> options, int maximumNumber)
		{
			RecordADeviation(index, value, maximumNumber);
			return await ContinueTheEditDistance(it, options, maximumNumber);
		}

		/// <summary>
		///     Records the <paramref name="value" /> at the <paramref name="index" /> as additional or incorrect, as far as
		///     the deviations can be listed, and starts tracking the edit distance.
		/// </summary>
		private void RecordADeviation(int index, T value, int maximumNumber)
		{
			if (_positionalDeviations++ <= 2L * maximumNumber)
			{
				if (index >= ExpectedItems.Length)
				{
					(_additionalItems ??= new Dictionary<int, T>()).Add(index, value);
				}
				else
				{
					(_incorrectItems ??= new Dictionary<int, (T Item, T3 Expected)>()).Add(index,
						(value, ExpectedItems[index]));
				}
			}

			_editDistance ??= new BoundedEditDistance<T3>(ExpectedItems,
				(int)Math.Min(2L * maximumNumber, int.MaxValue), index);
		}

		private async ValueTask<(bool, string?)> ContinueTheEditDistance(string it, IOptionsEquality<T2> options,
			int maximumNumber)
		{
			if (_editDistance is not null &&
			    !await _editDistance.Add((subjectIndex, expected) => IsMatch(subjectIndex, expected, options)))
			{
				return (true, TooManyDeviationsError(it, maximumNumber, GetDeviations(options),
					exceededDeviations: _editDistance.MaximumEdits));
			}

			return (false, null);
		}

		/// <summary>
		///     An unexpected and a missing item that format equally differ only in their runtime type.
		/// </summary>
		private Func<object?, string> CreateItemFormatter()
			=> GetItemFormatter((_additionalItems ?? new Dictionary<int, T>()).Values.Cast<object?>(), []);

		private ValueTask<bool> IsMatch(int index, T3 expected, IOptionsEquality<T2> options)
			=> AreConsideredEqual(index, _values[index], expected, options);

		/// <summary>
		///     Compares the <paramref name="value" /> at the <paramref name="index" /> with the
		///     <paramref name="expected" /> item.
		/// </summary>
		protected abstract ValueTask<bool>
			AreConsideredEqual(int index, T value, T3 expected, IOptionsEquality<T2> options);

		/// <summary>
		///     Creates the matcher that checks whether the same items match in any order.
		/// </summary>
		protected abstract ICollectionMatcher<T, T2> CreateAnyOrderMatcher();
	}
}
