using System;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Options;

namespace aweXpect;

/// <summary>
///     Base class for constraints that classify every item of a collection as matching or not matching and report
///     the outcome through an <see cref="EnumerableQuantifier" />.
/// </summary>
/// <remarks>
///     Use it for an expectation on the elements of a collection, e.g. an extension method on
///     <see cref="ThatEnumerable.Elements{TItem}" />, which exposes the <see cref="EnumerableQuantifier" /> and the
///     subject through <see cref="ThatEnumerable.IElements{TItem}" />. Pass the quantifier to the constructor, set
///     <see cref="ConstraintResult.WithNotNullValue{T}.Actual" /> in <c>IsMetBy</c>, call <see cref="Record" /> for every
///     item and <see cref="Complete" /> afterwards. For a <see langword="null" /> subject, only set the
///     <see cref="ConstraintResult.WithNotNullValue{T}.Actual" /> and return, as the expectation fails for it.
///     <para />
///     The base class renders the expectation and the result for the normal, the negated and the nested case (e.g.
///     "has values of which at least 2 are …") like the built-in expectations and adds the matching or not matching
///     items as context, as far as the quantifier requires them.
/// </remarks>
/// <typeparam name="TValue">The type of the collection.</typeparam>
/// <typeparam name="TItem">The type of the items in the collection.</typeparam>
public abstract class QuantifiedCollectionConstraint<TValue, TItem>
	: ConstraintResult.WithNotNullValue<TValue>
{
	private readonly Func<ExpectationGrammars, string>? _expectationText;
	private readonly EnumerableQuantifier _quantifier;
	private readonly string? _verb;
	private bool _isCompleted;
	private int _matchingCount;
	private LimitedCollection<TItem>? _matchingItems;
	private int _notMatchingCount;
	private LimitedCollection<TItem>? _notMatchingItems;
	private int? _totalCount;

	/// <summary>
	///     Initializes the constraint.
	/// </summary>
	/// <param name="expectationBuilder">The <see cref="ExpectationBuilder" /> of the expectation.</param>
	/// <param name="it">The name of the subject.</param>
	/// <param name="grammars">The grammars of the expectation.</param>
	/// <param name="quantifier">The quantifier for the items, e.g. from <see cref="ThatEnumerable.IElements{TItem}" />.</param>
	/// <param name="expectationText">
	///     Returns the expectation for a single item for the given grammars, e.g. "is even", or "are even" when the
	///     grammars are <see cref="ExpectationGrammars.Plural" />. The quantifier carries the negation, so the text is not
	///     negated.
	/// </param>
	/// <param name="verb">
	///     The verb in the past tense in the result, e.g. "were" in "but only 1 of 3 were".
	/// </param>
	protected QuantifiedCollectionConstraint(
		ExpectationBuilder expectationBuilder,
		string it,
		ExpectationGrammars grammars,
		EnumerableQuantifier quantifier,
		Func<ExpectationGrammars, string> expectationText,
		string verb)
		: this(expectationBuilder, it, grammars, quantifier)
	{
		_expectationText = expectationText;
		_verb = verb;
	}

	/// <summary>
	///     Initializes the constraint for a derived class that appends the expectation and overrides the
	///     <see cref="Verb" />.
	/// </summary>
	private protected QuantifiedCollectionConstraint(
		ExpectationBuilder expectationBuilder,
		string it,
		ExpectationGrammars grammars,
		EnumerableQuantifier quantifier)
		: base(it, grammars)
	{
		ExpectationBuilder = expectationBuilder;
		_quantifier = quantifier;
	}

	/// <summary>
	///     The <see cref="ExpectationBuilder" /> of the expectation.
	/// </summary>
	protected ExpectationBuilder ExpectationBuilder { get; }

	/// <summary>
	///     The quantifier for the items.
	/// </summary>
	private protected EnumerableQuantifier Quantifier => _quantifier;

	/// <summary>
	///     Indicates that the items recorded so far determine the outcome, so the remaining items need not be read.
	/// </summary>
	private protected bool IsDetermined => _quantifier.IsDeterminable(_matchingCount, _notMatchingCount);

	/// <summary>
	///     The type used to format the matching and not matching items.
	/// </summary>
	protected virtual Type ItemType => typeof(TItem);

	/// <summary>
	///     The verb in the past tense in the result, e.g. "were" in "but only 1 of 3 were".
	/// </summary>
	private protected virtual string Verb => _verb!;

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

		if (_matchingItems is null || _notMatchingItems is null)
		{
			_matchingItems = new LimitedCollection<TItem>();
			_notMatchingItems = new LimitedCollection<TItem>();
		}

		if (isMatch)
		{
			_matchingItems.Add(item, _matchingCount + _notMatchingCount);
			_matchingCount++;
		}
		else
		{
			_notMatchingItems.Add(item, _matchingCount + _notMatchingCount);
			_notMatchingCount++;
		}
	}

	/// <summary>
	///     Determines the outcome from the recorded items and adds the contexts required by the quantifier.
	/// </summary>
	protected void Complete()
		=> DetermineOutcome(false);

	/// <summary>
	///     Determines the outcome from the items recorded so far, when the remaining items of the collection are not
	///     read, because they cannot change the outcome.
	/// </summary>
	private protected void CompleteEarly()
		=> DetermineOutcome(true);

	private void DetermineOutcome(bool isIncomplete)
	{
		// Completed again without recording an item, the collection of this evaluation was empty.
		if (_isCompleted)
		{
			Reset();
		}

		_isCompleted = true;
		_totalCount = isIncomplete ? null : _matchingCount + _notMatchingCount;
		Outcome = _quantifier.GetOutcome(_matchingCount, _notMatchingCount, _totalCount);

		ExpectationBuilder.AddQuantifierContexts(this, _quantifier,
			_matchingItems is { Count: > 0 } matchingItems
				? () => matchingItems.Format(Actual, ItemType, _matchingCount).AppendIsIncomplete(isIncomplete)
				: null,
			_notMatchingItems is { Count: > 0 } notMatchingItems
				? () => notMatchingItems.Format(Actual, ItemType, _notMatchingCount).AppendIsIncomplete(isIncomplete)
				: null);
	}

	private void Reset()
	{
		_isCompleted = false;
		_matchingCount = 0;
		_notMatchingCount = 0;
		_matchingItems = null;
		_notMatchingItems = null;
	}

	/// <inheritdoc />
	protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		=> _quantifier.AppendExpectation(stringBuilder, Grammars, (sb, g) => sb.Append(_expectationText!(g)));

	/// <inheritdoc />
	protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		=> _quantifier.AppendResult(stringBuilder, Grammars, It, _matchingCount, _notMatchingCount, _totalCount, Verb);

	/// <inheritdoc />
	protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		=> _quantifier.AppendExpectation(stringBuilder, Grammars, (sb, g) => sb.Append(_expectationText!(g)));

	/// <inheritdoc />
	protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		=> _quantifier.AppendResult(stringBuilder, Grammars, It, _matchingCount, _notMatchingCount, _totalCount, Verb);
}
