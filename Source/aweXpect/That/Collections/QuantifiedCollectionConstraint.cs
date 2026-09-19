using System;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Customization;
using aweXpect.Helpers;
using aweXpect.Options;

namespace aweXpect;

/// <summary>
///     Base class for constraints that classify every item of a collection as matching or not matching and report
///     the outcome through an <see cref="EnumerableQuantifier" />.
/// </summary>
internal abstract class QuantifiedCollectionConstraint<TValue, TItem>(
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
			int maxItems = Customize.aweXpect.Formatting().MaximumNumberOfCollectionItems.Get() + 1;
			_matchingItems = new LimitedCollection<TItem>(maxItems);
			_notMatchingItems = new LimitedCollection<TItem>(maxItems);
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
	{
		_totalCount = _matchingCount + _notMatchingCount;
		Outcome = quantifier.GetOutcomeForCollection(Actual, _matchingCount, _notMatchingCount, _totalCount);

		EnumerableQuantifier.QuantifierContexts quantifierContexts = quantifier.GetQuantifierContext();
		if (quantifierContexts.HasFlag(EnumerableQuantifier.QuantifierContexts.MatchingItems) &&
		    _matchingItems is { Count: > 0 } matchingItems)
		{
			ExpectationBuilder.AddContext(new ResultContext.SyncCallback("Matching items",
				() => matchingItems.Format(Actual, ItemType),
				int.MaxValue));
		}

		if (quantifierContexts.HasFlag(EnumerableQuantifier.QuantifierContexts.NotMatchingItems) &&
		    _notMatchingItems is { Count: > 0 } notMatchingItems)
		{
			ExpectationBuilder.AddContext(new ResultContext.SyncCallback("Not matching items",
				() => notMatchingItems.Format(Actual, ItemType),
				int.MaxValue));
		}
	}

	protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		=> AppendExpectation(stringBuilder, false);

	protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		=> quantifier.AppendResult(stringBuilder, Grammars, _matchingCount, _notMatchingCount, _totalCount, verb);

	protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		=> AppendExpectation(stringBuilder, true);

	protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		=> quantifier.AppendResult(stringBuilder, Grammars, _matchingCount, _notMatchingCount, _totalCount, verb);

	private void AppendExpectation(StringBuilder stringBuilder, bool isNegated)
	{
		if (Grammars.HasFlag(ExpectationGrammars.Nested))
		{
			stringBuilder.AppendNestedQuantifier(quantifier, isNegated);
			stringBuilder.Append(expectationText(Grammars));
		}
		else
		{
			stringBuilder.Append(expectationText(isNegated ? Grammars.Negate() : Grammars));
			stringBuilder.Append(For);
			if (isNegated)
			{
				quantifier.AppendNegated(stringBuilder);
			}
			else
			{
				stringBuilder.Append(quantifier);
				stringBuilder.Append(' ');
				stringBuilder.Append(quantifier.GetItemString());
			}
		}
	}
}
