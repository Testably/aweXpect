using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.EvaluationContext;
using aweXpect.Core.Extending;
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
	///     Returns an item that matches the items without awaiting anything, or <see langword="null" /> when the
	///     comparison may have to be awaited.
	/// </summary>
	/// <remarks>
	///     Asked once the subject is known, as the comparer of a set subject can decide the comparison.
	/// </remarks>
	public virtual SynchronouslyMatchedItem<TItem>? GetSynchronousItem() => this as SynchronouslyMatchedItem<TItem>;

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

#if NET8_0_OR_GREATER
/// <summary>
///     An item that searches the items of a collection directly.
/// </summary>
/// <remarks>
///     Only an item whose comparison runs no code of the caller searches directly, as the caller could modify a list
///     while it is searched, which only its enumerator detects.
/// </remarks>
internal interface IDirectlySearchedItem<TItem>
{
	/// <summary>
	///     The index of the first of the <paramref name="items" /> that matches, or <c>-1</c> when none does.
	/// </summary>
	int IndexIn(ReadOnlySpan<TItem> items);
}
#endif

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
	///     Whether a failure suggests the item of the collection that the expected string most likely meant.
	/// </summary>
	public virtual bool SuggestsClosestMatch => false;

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
	private DefaultEqualItem? _defaultEqualItem;

	/// <inheritdoc />
	public override bool UsesDefaultEquality
		=> Expected is not null && options is IHasDefaultMatchType { HasDefaultMatchType: true, };

	/// <inheritdoc />
	/// <remarks>
	///     The default match type compares a primitive, an enum, a <see langword="decimal" /> and a
	///     <see cref="Guid" /> like <see cref="EqualityComparer{T}.Default" />, whose comparison of them cannot throw, so
	///     it needs neither to be awaited nor guarded as code of the caller. Only the time types have a default
	///     tolerance instead, see <see cref="ObjectEqualityWithToleranceOptionsFactory" />.
	/// </remarks>
	public override SynchronouslyMatchedItem<TItem>? GetSynchronousItem()
		=> HasFrameworkEquality.Value && options is IHasDefaultMatchType { HasDefaultMatchType: true, }
			? _defaultEqualItem ??= new DefaultEqualItem(this)
			: null;

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

	/// <summary>
	///     The <paramref name="equalItem" /> compared like the default match type compares it.
	/// </summary>
	/// <remarks>
	///     The comparison of these types runs no code of the caller, so the items of a collection can be searched
	///     directly.
	/// </remarks>
	private sealed class DefaultEqualItem(EqualItem<TItem> equalItem) : SynchronouslyMatchedItem<TItem>
#if NET8_0_OR_GREATER
		, IDirectlySearchedItem<TItem>
#endif
	{
		/// <inheritdoc />
		public override bool IsMatch(TItem item)
			=> EqualityComparer<TItem>.Default.Equals(item, equalItem.Expected);

#if NET8_0_OR_GREATER
		/// <inheritdoc />
		public int IndexIn(ReadOnlySpan<TItem> items)
		{
			TItem expected = equalItem.Expected;
			for (int index = 0; index < items.Length; index++)
			{
				if (EqualityComparer<TItem>.Default.Equals(items[index], expected))
				{
					return index;
				}
			}

			return -1;
		}
#endif

		/// <inheritdoc />
		public override string GetExpectation(Quantifier quantifier, ExpectationGrammars grammars, bool isNegated)
			=> equalItem.GetExpectation(quantifier, grammars, isNegated);

		/// <inheritdoc />
		public override string GetHasItemExpectation(ExpectationGrammars grammars)
			=> equalItem.GetHasItemExpectation(grammars);
	}

	private static class HasFrameworkEquality
	{
		public static readonly bool Value = typeof(TItem).IsPrimitive || typeof(TItem).IsEnum ||
		                                    typeof(TItem) == typeof(decimal) || typeof(TItem) == typeof(Guid);
	}
}

/// <summary>
///     A string that is equal to the <paramref name="expected" /> one according to the <paramref name="options" />.
/// </summary>
internal sealed class EqualStringItem(StringEqualityOptions options, string? expected)
	: ExpectedItem<string?>(expected)
{
	private OrdinalStringItem? _ordinalItem;

	/// <inheritdoc />
	public override bool UsesDefaultEquality
		=> Expected is not null && options.ComparesByOrdinalEquality;

	/// <inheritdoc />
	/// <remarks>
	///     A pattern, e.g. a wildcard or a prefix, is not a value that could have been mistyped.
	/// </remarks>
	public override bool SuggestsClosestMatch
		=> !options.InspectsSubject;

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
		StringEqualityOptions evaluationOptions = options.ForEvaluation(context, cancellationToken);
		return ReferenceEquals(evaluationOptions, options) ? this : new EqualStringItem(evaluationOptions, Expected);
	}

	/// <inheritdoc />
	/// <remarks>
	///     Without an option, the items are compared ordinally, which needs nothing to be awaited.
	/// </remarks>
	public override SynchronouslyMatchedItem<string?>? GetSynchronousItem()
		=> options.ComparesByOrdinalEquality ? _ordinalItem ??= new OrdinalStringItem(this) : null;

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

	/// <inheritdoc />
	public override void AppendContexts(ResultContextCollector contexts)
		=> options.AppendContexts(contexts);

	/// <summary>
	///     The <paramref name="equalItem" /> compared ordinally, like the options compare it without an option.
	/// </summary>
	private sealed class OrdinalStringItem(EqualStringItem equalItem) : SynchronouslyMatchedItem<string?>
#if NET8_0_OR_GREATER
		, IDirectlySearchedItem<string?>
#endif
	{
		/// <inheritdoc />
		public override bool IsMatch(string? item)
			=> string.Equals(item, equalItem.Expected);

#if NET8_0_OR_GREATER
		/// <inheritdoc />
		public int IndexIn(ReadOnlySpan<string?> items)
		{
			string? expected = equalItem.Expected;
			for (int index = 0; index < items.Length; index++)
			{
				if (string.Equals(items[index], expected))
				{
					return index;
				}
			}

			return -1;
		}
#endif

		/// <inheritdoc />
		public override string GetExpectation(Quantifier quantifier, ExpectationGrammars grammars, bool isNegated)
			=> equalItem.GetExpectation(quantifier, grammars, isNegated);

		/// <inheritdoc />
		public override string GetHasItemExpectation(ExpectationGrammars grammars)
			=> equalItem.GetHasItemExpectation(grammars);
	}
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
	public override bool SuggestsClosestMatch
		=> inner.SuggestsClosestMatch;

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
	/// <remarks>
	///     The comparer of the subject is called directly, so only the inner item can be matched synchronously, when the
	///     subject has none.
	/// </remarks>
	public override SynchronouslyMatchedItem<TItem>? GetSynchronousItem()
		=> _comparer is null ? inner.GetSynchronousItem() : null;

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
