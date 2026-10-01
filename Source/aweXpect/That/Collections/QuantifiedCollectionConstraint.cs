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
public abstract class QuantifiedCollectionConstraint<TValue, TItem>(
	ExpectationBuilder expectationBuilder,
	string it,
	ExpectationGrammars grammars,
	EnumerableQuantifier quantifier,
	Func<ExpectationGrammars, string> expectationText,
	string verb)
	: ConstraintResult.WithNotNullValue<TValue>(it, grammars)
{
	private const string For = " for ";
	private int _matchingCount;
	private LimitedCollection<TItem>? _matchingItems;
	private int _notMatchingCount;
	private LimitedCollection<TItem>? _notMatchingItems;
	private int? _totalCount;

	/// <summary>
	///     The <see cref="ExpectationBuilder" /> of the expectation.
	/// </summary>
	protected ExpectationBuilder ExpectationBuilder { get; } = expectationBuilder;

	/// <summary>
	///     The quantifier for the items.
	/// </summary>
	private protected EnumerableQuantifier Quantifier => quantifier;

	/// <summary>
	///     The type used to format the matching and not matching items.
	/// </summary>
	protected virtual Type ItemType => typeof(TItem);

	/// <summary>
	///     Records whether the <paramref name="item" /> matches the expectation.
	/// </summary>
	protected void Record(TItem item, bool isMatch)
	{
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
		_totalCount = isIncomplete ? null : _matchingCount + _notMatchingCount;
		Outcome = quantifier.GetOutcome(_matchingCount, _notMatchingCount, _totalCount);

		ExpectationBuilder.AddQuantifierContexts(this, quantifier,
			_matchingItems is { Count: > 0 } matchingItems
				? () => matchingItems.Format(Actual, ItemType, _matchingCount).AppendIsIncomplete(isIncomplete)
				: null,
			_notMatchingItems is { Count: > 0 } notMatchingItems
				? () => notMatchingItems.Format(Actual, ItemType, _notMatchingCount).AppendIsIncomplete(isIncomplete)
				: null);
	}

	/// <inheritdoc />
	protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		=> AppendExpectation(stringBuilder, false);

	/// <inheritdoc />
	protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		=> quantifier.AppendResult(stringBuilder, Grammars, It, _matchingCount, _notMatchingCount, _totalCount, verb);

	/// <inheritdoc />
	protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		=> AppendExpectation(stringBuilder, true);

	/// <inheritdoc />
	protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		=> quantifier.AppendResult(stringBuilder, Grammars, It, _matchingCount, _notMatchingCount, _totalCount, verb);

	private void AppendExpectation(StringBuilder stringBuilder, bool isNegated)
	{
		if (Grammars.HasFlag(ExpectationGrammars.Nested))
		{
			stringBuilder.AppendNestedQuantifier(quantifier, isNegated, Grammars, expectationText);
		}
		else
		{
			// The quantifier carries the negation, so the item expectation is not negated.
			stringBuilder.Append(expectationText(isNegated ? Grammars.Negate() : Grammars));
			if (isNegated)
			{
				quantifier.AppendNegated(stringBuilder);
			}
			else
			{
				stringBuilder.Append(For);
				stringBuilder.Append(quantifier);
				stringBuilder.Append(' ');
				stringBuilder.Append(quantifier.GetItemString());
			}
		}
	}
}
