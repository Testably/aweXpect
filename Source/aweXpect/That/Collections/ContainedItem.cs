using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.EvaluationContext;
using aweXpect.Helpers;
using aweXpect.Options;

namespace aweXpect;

/// <summary>
///     What a <c>Contains</c> or <c>HasItem</c> expectation looks for: an expected item or an item that matches a
///     predicate.
/// </summary>
/// <remarks>
///     One object per expectation instead of separate delegates for the comparison, the set lookup, the expectation
///     text and the contexts, because every such expectation creates one.
/// </remarks>
internal abstract class ContainedItem<TItem>
{
	/// <summary>
	///     Whether the <paramref name="item" /> is the one looked for.
	/// </summary>
	public abstract ValueTask<bool> Matches(TItem item);

	/// <summary>
	///     Returns the item to match the items with during the evaluation in the <paramref name="context" />.
	/// </summary>
	/// <remarks>
	///     Only an item whose comparison evaluates expectations of its own returns another item than itself, see
	///     <see cref="ObjectEqualityOptions{TSubject}.ForEvaluation(IEvaluationContext, CancellationToken)" />.
	/// </remarks>
	public virtual ContainedItem<TItem> ForEvaluation(IEvaluationContext context,
		CancellationToken cancellationToken)
		=> this;

	/// <summary>
	///     Lets the comparer of a set <paramref name="collection" /> decide, when the comparison allows it.
	/// </summary>
	public virtual void UseComparerOf(object collection)
	{
	}

	/// <summary>
	///     Asks a set <paramref name="collection" /> directly whether it contains the item, or returns
	///     <see langword="null" /> when the items have to be enumerated instead.
	/// </summary>
	public virtual bool? LookUpIn(object collection) => null;

	/// <summary>
	///     The expectation text of <c>Contains</c> for the <paramref name="quantifier" />.
	/// </summary>
	public abstract string GetExpectation(Quantifier quantifier, ExpectationGrammars grammars, bool isNegated);

	/// <summary>
	///     The text after "has an item" of <c>HasItem</c>.
	/// </summary>
	public abstract string GetHasItemExpectation(ExpectationGrammars grammars);

	/// <summary>
	///     Adds the contexts of the options that decide the comparison.
	/// </summary>
	public virtual void AppendContexts(ResultContextCollector contexts)
	{
	}
}

/// <summary>
///     An item that is matched synchronously.
/// </summary>
internal abstract class SynchronouslyMatchedItem<TItem> : ContainedItem<TItem>
{
	/// <summary>
	///     Whether the <paramref name="item" /> matches, without awaiting anything.
	/// </summary>
	public abstract bool IsMatch(TItem item);

	/// <inheritdoc />
	public override ValueTask<bool> Matches(TItem item)
		=> new(IsMatch(item));
}

/// <summary>
///     An item that matches the <paramref name="predicate" />.
/// </summary>
internal sealed class ItemMatchingPredicate<TItem>(Func<TItem, bool> predicate, string predicateExpression)
	: SynchronouslyMatchedItem<TItem>
{
	/// <inheritdoc />
	public override bool IsMatch(TItem item)
		=> UserCode.Invoke(predicate, item, "the predicate");

	/// <inheritdoc />
	public override string GetExpectation(Quantifier quantifier, ExpectationGrammars grammars, bool isNegated)
		=> quantifier.ToContainsExpectation(grammars,
			$"an item matching {predicateExpression.TrimCommonWhiteSpace()}", isNegated);

	/// <inheritdoc />
	public override string GetHasItemExpectation(ExpectationGrammars grammars)
		=> $"matching {predicateExpression.TrimCommonWhiteSpace()}";
}

/// <summary>
///     An item that matches the predicate that is specified in the <paramref name="options" /> after the expectation.
/// </summary>
/// <remarks>
///     Only <c>HasItem</c> specifies the predicate this way.
/// </remarks>
internal sealed class ItemMatchingOptions<TItem>(PredicateOptions<TItem> options) : SynchronouslyMatchedItem<TItem>
{
	/// <inheritdoc />
	public override bool IsMatch(TItem item)
		=> UserCode.Invoke(static values => values.Options.Matches(values.Item), (Options: options, Item: item),
			"the predicate");

	/// <inheritdoc />
	public override string GetExpectation(Quantifier quantifier, ExpectationGrammars grammars, bool isNegated)
		=> throw new NotSupportedException("Only HasItem specifies the predicate in its options.");

	/// <inheritdoc />
	public override string GetHasItemExpectation(ExpectationGrammars grammars)
		=> options.GetDescription();
}

/// <summary>
///     An expected item.
/// </summary>
internal abstract class ExpectedItem<TItem>(TItem expected) : ContainedItem<TItem>
{
	/// <summary>
	///     The expected item.
	/// </summary>
	public TItem Expected => expected;

	/// <summary>
	///     Whether the comparison is still the default one, so that the comparer of a set subject may decide instead.
	/// </summary>
	public virtual bool UsesDefaultEquality => false;

	/// <summary>
	///     The item of the set <paramref name="collection" /> that <see cref="ContainedItem{TItem}.LookUpIn" /> found.
	/// </summary>
	public virtual TItem FoundIn(object collection) => Expected;

	/// <summary>
	///     The text for the expected item, without the quantifier.
	/// </summary>
	public abstract string GetItemExpectation();

	/// <inheritdoc />
	public override string GetExpectation(Quantifier quantifier, ExpectationGrammars grammars, bool isNegated)
		=> quantifier.ToContainsExpectation(grammars, GetItemExpectation(), isNegated);
}

/// <summary>
///     An item that is equal to the <paramref name="expected" /> one according to the <paramref name="options" />.
/// </summary>
internal sealed class EqualItem<TItem>(ObjectEqualityOptions<TItem> options, TItem expected)
	: ExpectedItem<TItem>(expected)
{
	/// <inheritdoc />
	public override bool UsesDefaultEquality
		=> Expected is not null && options is IHasDefaultMatchType { HasDefaultMatchType: true, };

	/// <inheritdoc />
	public override ValueTask<bool> Matches(TItem item)
		=> options.AreConsideredEqual(item, Expected);

	/// <inheritdoc />
	public override ContainedItem<TItem> ForEvaluation(IEvaluationContext context,
		CancellationToken cancellationToken)
	{
		ObjectEqualityOptions<TItem> evaluationOptions = options.ForEvaluation(context, cancellationToken);
		return ReferenceEquals(evaluationOptions, options) ? this : new EqualItem<TItem>(evaluationOptions, Expected);
	}

	/// <inheritdoc />
	/// <remarks>
	///     The verb keeps a direct object, so a match type that describes the item reads "contains an item
	///     equivalent to …", and one that only formats it names the comparison itself instead of leaving the
	///     reader to guess it from "contains 3".
	/// </remarks>
	public override string GetItemExpectation()
		=> options.GetItemExpectation(Formatter.Format(Expected), "an item", "equal to");

	/// <inheritdoc />
	public override string GetHasItemExpectation(ExpectationGrammars grammars)
		=> options.GetItemExpectation(Formatter.Format(Expected), comparison: "equal to");

	/// <inheritdoc />
	public override void AppendContexts(ResultContextCollector contexts)
		=> options.AppendContexts(contexts);
}

/// <summary>
///     A string that is equal to the <paramref name="expected" /> one according to the <paramref name="options" />.
/// </summary>
internal sealed class EqualStringItem(StringEqualityOptions options, string? expected)
	: ExpectedItem<string?>(expected)
{
	/// <inheritdoc />
	public override bool UsesDefaultEquality
		=> Expected is not null && options.ComparesByOrdinalEquality;

	/// <inheritdoc />
	public override ValueTask<bool> Matches(string? item)
		=> options.AreConsideredEqual(item, Expected);

	/// <inheritdoc />
	/// <remarks>
	///     The expected string is validated here, so that an unusable pattern is rejected whichever items the subject
	///     has.
	/// </remarks>
	public override ContainedItem<string?> ForEvaluation(IEvaluationContext context,
		CancellationToken cancellationToken)
	{
		options.ValidateExpected(Expected);
		return this;
	}

	/// <inheritdoc />
	/// <remarks>
	///     A match type other than equality describes the item, so it reads "contains an item matching regex …".
	/// </remarks>
	public override string GetItemExpectation()
		=> options.InspectsSubject
			? "an item " + options.GetExpectation(Expected, ExpectationGrammars.None)
			: Formatter.Format(Expected) + options;

	/// <inheritdoc />
	public override string GetHasItemExpectation(ExpectationGrammars grammars)
		=> options.GetExpectation(Expected, grammars);
}

/// <summary>
///     The <paramref name="inner" /> item, compared by the comparer of a subject that is a set of
///     <typeparamref name="TSetItem" /> with a custom comparer, as long as the comparison of the item is the default one.
/// </summary>
/// <remarks>
///     The subject is only known during the evaluation, so <see cref="UseComparerOf" /> or <see cref="LookUpIn" /> has
///     to be called before comparing, and the texts name the comparer only afterwards.
/// </remarks>
internal sealed class SubjectComparedItem<TSetItem, TItem>(ExpectedItem<TItem> inner) : ExpectedItem<TItem>(inner.Expected)
{
	private SubjectComparer<TSetItem>? _comparer;

	/// <inheritdoc />
	public override ValueTask<bool> Matches(TItem item)
		=> _comparer is null
			? inner.Matches(item)
			: new ValueTask<bool>(_comparer.AreEqual(item, Expected));

	/// <inheritdoc />
	/// <remarks>
	///     The comparer of the subject only decides while the comparison of the inner item is the default one, so the
	///     item is kept then, and an inner item for the evaluation is matched directly otherwise.
	/// </remarks>
	public override ContainedItem<TItem> ForEvaluation(IEvaluationContext context,
		CancellationToken cancellationToken)
	{
		if (inner.UsesDefaultEquality)
		{
			return this;
		}

		ContainedItem<TItem> evaluationItem = inner.ForEvaluation(context, cancellationToken);
		return ReferenceEquals(evaluationItem, inner) ? this : evaluationItem;
	}

	/// <inheritdoc />
	public override void UseComparerOf(object collection)
		=> _comparer = inner.UsesDefaultEquality
			? CollectionComparerHelpers.GetSubjectComparer<TSetItem>(collection)
			: null;

	/// <inheritdoc />
	/// <remarks>
	///     A <see langword="null" /> item is enumerated, because a set whose comparer rejects <see langword="null" />
	///     throws when asked for it.
	/// </remarks>
	public override bool? LookUpIn(object collection)
	{
		UseComparerOf(collection);
		return _comparer is not null && Expected is TSetItem expected
			? UserCode.Invoke(static values => values.Collection.Contains(values.Expected),
				(Collection: (ICollection<TSetItem>)collection, Expected: expected), "the comparer")
			: null;
	}

	/// <inheritdoc />
	/// <remarks>
	///     The set only tells whether it holds an item equal to the expected one, so the result searches its items for
	///     it.
	/// </remarks>
	public override TItem FoundIn(object collection)
	{
		foreach (TSetItem item in (IEnumerable<TSetItem>)collection)
		{
			if (item is TItem typedItem && _comparer!.AreEqual(typedItem, Expected))
			{
				return typedItem;
			}
		}

		return Expected;
	}

	/// <inheritdoc />
	public override string GetItemExpectation()
		=> inner.GetItemExpectation() + _comparer;

	/// <inheritdoc />
	public override string GetHasItemExpectation(ExpectationGrammars grammars)
		=> inner.GetHasItemExpectation(grammars) + _comparer;

	/// <inheritdoc />
	public override void AppendContexts(ResultContextCollector contexts)
		=> inner.AppendContexts(contexts);
}
