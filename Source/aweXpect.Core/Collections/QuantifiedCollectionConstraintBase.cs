using System;
using System.Collections.Generic;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Options;

namespace aweXpect;

/// <summary>
///     Base class for constraints that classify every item of a collection as matching or not matching and report
///     the outcome through an <see cref="EnumerableQuantifier" />, and that append the expectation on a single item
///     themselves.
/// </summary>
/// <remarks>
///     For an expectation text that only depends on the grammars, derive from
///     <see cref="QuantifiedCollectionConstraint{TValue,TItem}" /> instead. Derive from this class when the text or the
///     verb in the result are only known while the failure message is created, e.g. because they come from nested
///     expectations.
///     <para />
///     Set <see cref="ConstraintResult.WithValue{T}.Actual" /> in <c>IsMetBy</c>, call
///     <see cref="Record(TItem, bool)" /> for every item, or <see cref="Record(TItem, ConstraintResult)" /> when the items
///     are verified by nested expectations, and <see cref="Complete" /> afterwards, or <see cref="CompleteEarly" /> as
///     soon as <see cref="IsDetermined" />. For a <see langword="null" /> subject, only set the
///     <see cref="ConstraintResult.WithValue{T}.Actual" /> and return, as the expectation fails for it.
///     <para />
///     The base class renders the expectation and the result for the normal, the negated and the nested case (e.g.
///     "has values of which at least 2 are …") like the built-in expectations and adds the matching or not matching
///     items as context, as far as the quantifier requires them, together with the contexts of the first such item.
/// </remarks>
/// <typeparam name="TValue">The type of the collection.</typeparam>
/// <typeparam name="TItem">The type of the items in the collection.</typeparam>
public abstract class QuantifiedCollectionConstraintBase<TValue, TItem>
	: ConstraintResult.WithNotNullValue<TValue>
{
	private ItemContexts _firstMatchingItem;
	private ItemContexts _firstNotMatchingItem;
	private bool _isCompleted;
	private bool _isIncomplete;
	private int _matchingCount;
	private LimitedCollection<TItem>? _matchingItems;
	private int _notMatchingCount;
	private LimitedCollection<TItem>? _notMatchingItems;
	private int? _totalCount;

	/// <summary>
	///     Initializes the constraint.
	/// </summary>
	/// <param name="it">The name of the subject.</param>
	/// <param name="grammars">The grammars of the expectation.</param>
	/// <param name="quantifier">The quantifier for the items, e.g. from <see cref="IEnumerableElements{TItem}" />.</param>
	protected QuantifiedCollectionConstraintBase(
		string it,
		ExpectationGrammars grammars,
		EnumerableQuantifier quantifier)
		: base(it, grammars)
	{
		Quantifier = quantifier;
	}

	/// <summary>
	///     The quantifier for the items.
	/// </summary>
	protected EnumerableQuantifier Quantifier { get; }

	/// <summary>
	///     Indicates that the items recorded so far determine the outcome, so the remaining items need not be read.
	/// </summary>
	protected bool IsDetermined => Quantifier.IsDeterminable(_matchingCount, _notMatchingCount);

	/// <summary>
	///     The type used to format the matching and not matching items.
	/// </summary>
	protected virtual Type ItemType => typeof(TItem);

	/// <summary>
	///     The verb in the past tense in the result, e.g. "were" in "but only 1 of 3 were".
	/// </summary>
	protected abstract string Verb { get; }

	/// <summary>
	///     Appends the expectation on a single item for the <paramref name="grammars" />, e.g. "is even", or "are even"
	///     when the grammars are <see cref="ExpectationGrammars.Plural" />.
	/// </summary>
	/// <remarks>
	///     The quantifier carries the negation, so the <paramref name="grammars" /> are not negated.
	/// </remarks>
	protected abstract void AppendItemExpectation(StringBuilder stringBuilder, ExpectationGrammars grammars,
		string? indentation);

	/// <summary>
	///     Records whether the <paramref name="item" /> matches the expectation.
	/// </summary>
	/// <remarks>
	///     The first item after a completed evaluation starts a new one, as the constraint can be evaluated again, e.g.
	///     for each item of an outer collection.
	/// </remarks>
	protected void Record(TItem item, bool isMatch)
	{
		if (_isCompleted)
		{
			Reset();
		}

		if (isMatch)
		{
			(_matchingItems ??= new LimitedCollection<TItem>()).Add(item, _matchingCount + _notMatchingCount);
			_matchingCount++;
		}
		else
		{
			(_notMatchingItems ??= new LimitedCollection<TItem>()).Add(item, _matchingCount + _notMatchingCount);
			_notMatchingCount++;
		}
	}

	/// <summary>
	///     Records whether the <paramref name="item" /> matches the expectations on the items, which led to the
	///     <paramref name="itemResult" />.
	/// </summary>
	/// <remarks>
	///     The contexts of the first matching and of the first not matching item are kept, so that the failure message
	///     shows those of the item that explains it, labelled with its index, e.g. <c>Actual (item [2]):</c>.
	/// </remarks>
	protected void Record(TItem item, ConstraintResult itemResult)
	{
		bool isMatch = itemResult.Outcome == Outcome.Success;
		Record(item, isMatch);
		int index = _matchingCount + _notMatchingCount - 1;
		if (isMatch)
		{
			_firstMatchingItem.KeepFirst(itemResult, index,
				CouldShow(EnumerableQuantifier.QuantifierContexts.MatchingItems));
		}
		else
		{
			_firstNotMatchingItem.KeepFirst(itemResult, index,
				CouldShow(EnumerableQuantifier.QuantifierContexts.NotMatchingItems));
		}
	}

	/// <summary>
	///     Determines the outcome from the recorded items.
	/// </summary>
	protected void Complete()
		=> DetermineOutcome(false);

	/// <summary>
	///     Determines the outcome from the items recorded so far, when the remaining items of the collection are not
	///     read, because they cannot change the outcome.
	/// </summary>
	protected void CompleteEarly()
		=> DetermineOutcome(true);

	/// <inheritdoc />
	/// <remarks>
	///     Adds the matching or the not matching items, as far as the quantifier requires them.
	/// </remarks>
	public override void AppendContexts(ResultContextCollector contexts)
	{
		if (!_isCompleted)
		{
			return;
		}

		bool isIncomplete = _isIncomplete;
		int matchingCount = _matchingCount;
		int notMatchingCount = _notMatchingCount;
		TValue? actual = Actual;
		EnumerableQuantifier.QuantifierContexts shown = Grammars.IsNegated()
			? Quantifier.GetNegatedQuantifierContext()
			: Quantifier.GetQuantifierContext();
		contexts.AddQuantifierContexts(Quantifier, shown,
			_matchingItems is { Count: > 0, } matchingItems
				? () => matchingItems.Format(actual, ItemType, matchingCount).AppendIsIncomplete(isIncomplete)
				: null,
			_notMatchingItems is { Count: > 0, } notMatchingItems
				? () => notMatchingItems.Format(actual, ItemType, notMatchingCount).AppendIsIncomplete(isIncomplete)
				: null);
		if (shown.HasFlag(EnumerableQuantifier.QuantifierContexts.NotMatchingItems))
		{
			_firstNotMatchingItem.AppendTo(contexts);
		}

		if (shown.HasFlag(EnumerableQuantifier.QuantifierContexts.MatchingItems))
		{
			_firstMatchingItem.AppendTo(contexts);
		}
	}

	/// <inheritdoc />
	protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		=> Quantifier.AppendExpectation(stringBuilder, Grammars,
			(itemExpectation, grammars) => AppendItemExpectation(itemExpectation, grammars, indentation));

	/// <inheritdoc />
	protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		=> Quantifier.AppendResult(stringBuilder, Grammars, It, _matchingCount, _notMatchingCount, _totalCount, Verb);

	/// <inheritdoc />
	protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		=> Quantifier.AppendExpectation(stringBuilder, Grammars,
			(itemExpectation, grammars) => AppendItemExpectation(itemExpectation, grammars, indentation));

	/// <inheritdoc />
	protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		=> Quantifier.AppendResult(stringBuilder, Grammars, It, _matchingCount, _notMatchingCount, _totalCount, Verb);

	private void DetermineOutcome(bool isIncomplete)
	{
		// Completed again without recording an item, the collection of this evaluation was empty.
		if (_isCompleted)
		{
			Reset();
		}

		_isCompleted = true;
		_totalCount = isIncomplete ? null : _matchingCount + _notMatchingCount;
		Outcome = Quantifier.GetOutcome(_matchingCount, _notMatchingCount, _totalCount);

		_isIncomplete = isIncomplete;
	}

	/// <summary>
	///     Whether the items are shown as context, for the expectation or its negation, which is only known when the
	///     failure message is created.
	/// </summary>
	private bool CouldShow(EnumerableQuantifier.QuantifierContexts items)
		=> (Quantifier.GetQuantifierContext() | Quantifier.GetNegatedQuantifierContext()).HasFlag(items);

	private void Reset()
	{
		_firstMatchingItem = default;
		_firstNotMatchingItem = default;
		_isCompleted = false;
		_matchingCount = 0;
		_notMatchingCount = 0;
		_matchingItems = null;
		_notMatchingItems = null;
	}

	/// <summary>
	///     The contexts of the first item of a kind and its index.
	/// </summary>
	private struct ItemContexts
	{
		private IReadOnlyList<ResultContextCollector.Entry>? _contexts;
		private int _index;
		private bool _isKept;

		public void KeepFirst(ConstraintResult itemResult, int index, bool couldBeShown)
		{
			if (_isKept || !couldBeShown)
			{
				return;
			}

			_isKept = true;
			_index = index;
			_contexts = ResultContextCollector.Capture(itemResult);
		}

		public readonly void AppendTo(ResultContextCollector contexts)
		{
			if (_contexts is not null)
			{
				contexts.AddCaptured($"[{_index}]", _contexts);
			}
		}
	}
}
