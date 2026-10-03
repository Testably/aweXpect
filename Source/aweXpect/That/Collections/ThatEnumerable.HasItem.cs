using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;
using aweXpect.SourceGenerators;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect;

public static partial class ThatEnumerable
{
	private const string HasAnItem = "Verifies that the collection has an item…";
	private const string DoesNotHaveAnItem = "Verifies that the collection does not have an item…";

	private const string HasMatchingItem =
		"Verifies that the collection has an item matching the <paramref name=\"predicate\" />…";

	private const string DoesNotHaveMatchingItem =
		"Verifies that the collection does not have an item matching the <paramref name=\"predicate\" />…";

	private const string HasTheItem = "Verifies that the collection has the <paramref name=\"expected\" /> item…";

	private const string DoesNotHaveTheItem =
		"Verifies that the collection does not have the <paramref name=\"unexpected\" /> item…";

	[CreateExpectationFamily("HasItem", NegatedName = "DoesNotHaveItem", GuaranteesNotNull = true,
		Summary = HasAnItem, NegatedSummary = DoesNotHaveAnItem)]
	internal static HasItemWithConditionResult<IEnumerable<TItem>, TItem>
		HasItemCore<TItem>(
			IThat<IEnumerable<TItem>?> subject,
			bool negated)
	{
		CollectionIndexOptions indexOptions = new();
		PredicateOptions<TItem> options = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new HasItemWithConditionResult<IEnumerable<TItem>, TItem>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new HasItemConstraint<IEnumerable<TItem>?, TItem>(it, grammars,
					x => options.Matches(x),
					options.GetDescription,
					indexOptions).InvertIf(negated)),
			subject,
			indexOptions,
			options);
	}

	[CreateExpectationFamily("HasItem", NegatedName = "DoesNotHaveItem", GuaranteesNotNull = true,
		Summary = HasMatchingItem, NegatedSummary = DoesNotHaveMatchingItem)]
	internal static HasItemResult<IEnumerable<TItem>>
		HasMatchingItemCore<TItem>(
			IThat<IEnumerable<TItem>?> subject,
			Func<TItem, bool> predicate,
			string predicateExpression,
			bool negated)
	{
		predicate.ThrowIfNull();
		CollectionIndexOptions indexOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new HasItemResult<IEnumerable<TItem>>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new HasItemConstraint<IEnumerable<TItem>?, TItem>(it, grammars, predicate,
					() => $"matching {predicateExpression}", indexOptions).InvertIf(negated)),
			subject,
			indexOptions);
	}

	[CreateExpectationFamily("HasItem", NegatedName = "DoesNotHaveItem", GuaranteesNotNull = true,
		Summary = HasTheItem, NegatedSummary = DoesNotHaveTheItem, Remarks = SetItemComparerRemarks)]
	internal static ObjectHasItemResult<IEnumerable<TItem>, TItem>
		HasTheItemCore<TItem>(
			IThat<IEnumerable<TItem>?> subject,
			TItem expected,
			bool negated)
	{
		CollectionIndexOptions indexOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		ItemEqualityOptions<TItem> options = new();
		return new ObjectHasItemResult<IEnumerable<TItem>, TItem>(
			expectationBuilder.AddConstraint((it, grammars) =>
			{
				SubjectEqualityOptions<TItem, TItem> itemOptions =
					new(options, () => expected is not null && options.HasDefaultMatchType);
				return new HasItemConstraint<IEnumerable<TItem>?, TItem>(it, grammars,
					a => itemOptions.AreConsideredEqual(a, expected),
					() => options.GetItemExpectation(Formatter.Format(expected), comparison: "equal to") +
					      itemOptions.Comparer,
					indexOptions,
					itemOptions.UseComparerOf,
					appendOptionsContexts: options.AppendContexts).InvertIf(negated);
			}),
			subject,
			indexOptions,
			options);
	}

	[CreateExpectationFamily("HasItem", NegatedName = "DoesNotHaveItem", GuaranteesNotNull = true,
		Factory = typeof(ObjectEqualityWithToleranceOptionsFactory),
		Summary = HasTheItem, NegatedSummary = DoesNotHaveTheItem, Remarks = SetItemComparerRemarks)]
	internal static ObjectHasItemWithToleranceResult<IEnumerable<TItem>, TItem, TTolerance>
		HasTheItemWithToleranceCore<TItem, TTolerance>(
			IThat<IEnumerable<TItem>?> subject,
			TItem expected,
			ObjectEqualityWithToleranceOptions<TItem, TTolerance> options,
			bool negated)
	{
		CollectionIndexOptions indexOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectHasItemWithToleranceResult<IEnumerable<TItem>, TItem, TTolerance>(
			expectationBuilder.AddConstraint((it, grammars) =>
			{
				SubjectEqualityOptions<TItem, TItem> itemOptions = new(options,
					() => expected is not null && ObjectEqualityWithToleranceOptionsFactory.HasDefaultMatchType(options));
				return new HasItemConstraint<IEnumerable<TItem>?, TItem>(it, grammars,
					a => itemOptions.AreConsideredEqual(a, expected),
					() => options.GetItemExpectation(Formatter.Format(expected), comparison: "equal to") +
					      itemOptions.Comparer,
					indexOptions,
					itemOptions.UseComparerOf,
					appendOptionsContexts: options.AppendContexts).InvertIf(negated);
			}),
			subject,
			indexOptions,
			options);
	}

	[CreateExpectationFamily("HasItem", NegatedName = "DoesNotHaveItem", GuaranteesNotNull = true,
		Summary = HasTheItem, NegatedSummary = DoesNotHaveTheItem, Remarks = SetItemComparerRemarks)]
	internal static StringHasItemResult<IEnumerable<string?>>
		HasTheItemForStringsCore(
			IThat<IEnumerable<string?>?> subject,
			string? expected,
			bool negated)
	{
		CollectionIndexOptions indexOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		StringEqualityOptions options = new(negated ? "unexpected" : "expected");
		return new StringHasItemResult<IEnumerable<string?>>(
			expectationBuilder.AddConstraint((it, grammars) =>
			{
				SubjectEqualityOptions<string?, string?> itemOptions =
					new(options, () => expected is not null && options.ComparesByOrdinalEquality);
				return new HasItemConstraint<IEnumerable<string?>?, string?>(it, grammars,
					a => itemOptions.AreConsideredEqual(a, expected),
					() => options.GetExpectation(expected, grammars) + itemOptions.Comparer,
					indexOptions,
					itemOptions.UseComparerOf).InvertIf(negated);
			}),
			subject,
			indexOptions,
			options);
	}

	[CreateExpectationFamily("HasItem", NegatedName = "DoesNotHaveItem", GuaranteesNotNull = true,
		Summary = HasAnItem, NegatedSummary = DoesNotHaveAnItem)]
	internal static HasItemWithConditionResult<IEnumerable, object?>
		HasItemForEnumerableCore(
			IThat<IEnumerable?> subject,
			bool negated)
	{
		CollectionIndexOptions indexOptions = new();
		PredicateOptions<object?> options = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new HasItemWithConditionResult<IEnumerable, object?>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new HasItemConstraint<IEnumerable, object?>(it, grammars,
					x => options.Matches(x), options.GetDescription, indexOptions).InvertIf(negated)),
			subject,
			indexOptions,
			options);
	}

	[CreateExpectationFamily("HasItem", NegatedName = "DoesNotHaveItem", GuaranteesNotNull = true, Priority = -1,
		Summary = HasMatchingItem, NegatedSummary = DoesNotHaveMatchingItem)]
	internal static HasItemResult<IEnumerable>
		HasMatchingItemForEnumerableCore(
			IThat<IEnumerable?> subject,
			Func<object?, bool> predicate,
			string predicateExpression,
			bool negated)
	{
		predicate.ThrowIfNull();
		CollectionIndexOptions indexOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new HasItemResult<IEnumerable>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new HasItemConstraint<IEnumerable, object?>(
					it, grammars,
					predicate, () => $"matching {predicateExpression}",
					indexOptions).InvertIf(negated)),
			subject,
			indexOptions);
	}

	[CreateExpectationFamily("HasItem", NegatedName = "DoesNotHaveItem", GuaranteesNotNull = true,
		Summary = HasTheItem, NegatedSummary = DoesNotHaveTheItem, Remarks = UntypedSetComparerRemarks)]
	internal static ObjectHasItemResult<IEnumerable, object?>
		HasTheItemForEnumerableCore(
			IThat<IEnumerable?> subject,
			object? expected,
			bool negated)
	{
		CollectionIndexOptions indexOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		ItemEqualityOptions<object?> options = new();
		return new ObjectHasItemResult<IEnumerable, object?>(
			expectationBuilder.AddConstraint((it, grammars) =>
			{
				SubjectEqualityOptions<object?, object?> itemOptions =
					new(options, () => expected is not null && options.HasDefaultMatchType);
				return new HasItemConstraint<IEnumerable, object?>(
					it, grammars,
					a => itemOptions.AreConsideredEqual(a, expected),
					() => options.GetItemExpectation(Formatter.Format(expected), comparison: "equal to") +
					      itemOptions.Comparer,
					indexOptions,
					itemOptions.UseComparerOf,
					appendOptionsContexts: options.AppendContexts).InvertIf(negated);
			}),
			subject,
			indexOptions,
			options);
	}

	[CreateExpectationFamily("HasItem", NegatedName = "DoesNotHaveItem", PerSubject = true,
		Summary = HasMatchingItem, NegatedSummary = DoesNotHaveMatchingItem)]
	internal static HasItemResult<TCollection>
		HasMatchingItemForCollectionCore<TCollection, TItem>(
			IThat<TCollection> subject,
			Func<TItem, bool> predicate,
			string predicateExpression,
			bool negated)
		where TCollection : IEnumerable
	{
		predicate.ThrowIfNull();
		CollectionIndexOptions indexOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new HasItemResult<TCollection>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new HasItemConstraint<TCollection, TItem>(
					it, grammars,
					predicate, () => $"matching {predicateExpression}",
					indexOptions).InvertIf(negated)),
			subject,
			indexOptions);
	}

	[CreateExpectationFamily("HasItem", NegatedName = "DoesNotHaveItem", PerSubject = true,
		Summary = HasAnItem, NegatedSummary = DoesNotHaveAnItem)]
	internal static HasItemWithConditionResult<TCollection, TItem>
		HasItemForCollectionCore<TCollection, TItem>(
			IThat<TCollection> subject,
			bool negated)
		where TCollection : IEnumerable
	{
		CollectionIndexOptions indexOptions = new();
		PredicateOptions<TItem> options = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new HasItemWithConditionResult<TCollection, TItem>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new HasItemConstraint<TCollection, TItem>(it, grammars,
					x => options.Matches(x), options.GetDescription, indexOptions).InvertIf(negated)),
			subject,
			indexOptions,
			options);
	}

	[CreateExpectationFamily("HasItem", NegatedName = "DoesNotHaveItem", PerSubject = true,
		Summary = HasTheItem, NegatedSummary = DoesNotHaveTheItem)]
	internal static ObjectHasItemResult<TCollection, TItem>
		HasTheItemForCollectionCore<TCollection, TItem>(
			IThat<TCollection> subject,
			TItem expected,
			bool negated)
		where TCollection : IEnumerable
	{
		CollectionIndexOptions indexOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		ObjectEqualityOptions<TItem> options = new();
		return new ObjectHasItemResult<TCollection, TItem>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new HasItemConstraint<TCollection, TItem>(
					it, grammars,
					a => options.AreConsideredEqual(a, expected),
					() => options.GetItemExpectation(Formatter.Format(expected), comparison: "equal to"),
					indexOptions,
					appendOptionsContexts: options.AppendContexts).InvertIf(negated)),
			subject,
			indexOptions,
			options);
	}

	[CreateExpectationFamily("HasItem", NegatedName = "DoesNotHaveItem", PerSubject = true,
		Factory = typeof(ObjectEqualityWithToleranceOptionsFactory),
		Summary = HasTheItem, NegatedSummary = DoesNotHaveTheItem)]
	internal static ObjectHasItemWithToleranceResult<TCollection, TItem, TTolerance>
		HasTheItemWithToleranceForCollectionCore<TCollection, TItem, TTolerance>(
			IThat<TCollection> subject,
			TItem expected,
			ObjectEqualityWithToleranceOptions<TItem, TTolerance> options,
			bool negated)
		where TCollection : IEnumerable
	{
		CollectionIndexOptions indexOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectHasItemWithToleranceResult<TCollection, TItem, TTolerance>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new HasItemConstraint<TCollection, TItem>(
					it, grammars,
					a => options.AreConsideredEqual(a, expected),
					() => options.GetItemExpectation(Formatter.Format(expected), comparison: "equal to"),
					indexOptions,
					appendOptionsContexts: options.AppendContexts).InvertIf(negated)),
			subject,
			indexOptions,
			options);
	}

	[CreateExpectationFamily("HasItem", NegatedName = "DoesNotHaveItem", PerSubject = true,
		Summary = HasTheItem, NegatedSummary = DoesNotHaveTheItem)]
	internal static StringHasItemResult<TCollection>
		HasTheItemForCollectionStringsCore<TCollection>(
			IThat<TCollection> subject,
			string? expected,
			bool negated)
		where TCollection : IEnumerable
	{
		CollectionIndexOptions indexOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		StringEqualityOptions options = new(negated ? "unexpected" : "expected");
		return new StringHasItemResult<TCollection>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new HasItemConstraint<TCollection, string?>(
					it, grammars,
					a => options.AreConsideredEqual(a, expected),
					() => options.GetExpectation(expected, grammars),
					indexOptions).InvertIf(negated)),
			subject,
			indexOptions,
			options);
	}

	/// <summary>
	///     Counts the items when the index is counted from the end, and returns <see langword="false" /> when the
	///     <paramref name="cancellationToken" /> is canceled before the count is known.
	/// </summary>
	private static bool TryCountForIndex<TItem>(CollectionIndexOptions options, object actual, IEnumerable<TItem> items,
		CancellationToken cancellationToken, out int? count)
	{
		count = null;
		if (options.Match is not CollectionIndexOptions.IMatchFromEnd)
		{
			return true;
		}

		count = actual switch
		{
			ICollection<TItem> collection => collection.Count,
			ICollection collection => collection.Count,
			_ => items.CountUnlessCanceled(cancellationToken),
		};
		return count is not null;
	}

	/// <summary>
	///     Returns <see langword="true" /> when the item at the <paramref name="index" /> is in range,
	///     <see langword="false" /> when no later item can be in range and <see langword="null" /> otherwise.
	/// </summary>
	private static bool? IsIndexInRange(CollectionIndexOptions options, int index, int? count)
		=> options.Match switch
		{
			CollectionIndexOptions.IMatchFromBeginning fromBeginning => fromBeginning.MatchesIndex(index),
			CollectionIndexOptions.IMatchFromEnd fromEnd => fromEnd.MatchesIndex(index, count),
			_ => false,
		};

	private sealed class HasItemConstraint<TEnumerable, TItem>
		: ConstraintResult.WithNotNullValue<TEnumerable>,
			IAsyncContextConstraint<TEnumerable>
		where TEnumerable : IEnumerable?
	{
		private readonly Action<ResultContextCollector>? _appendOptionsContexts;
		private readonly Func<TItem, ValueTask<bool>>? _asyncPredicate;
		private readonly CollectionIndexOptions _options;
		private readonly Func<TItem, bool>? _predicate;
		private readonly Func<string> _predicateDescription;
		private readonly Func<object?, bool>? _useComparerOf;
		private TItem? _actual;
		private CollectionContext _collectionContext;
		private bool _hasIndex;

		public HasItemConstraint(
			string it,
			ExpectationGrammars grammars,
			Func<TItem, bool> predicate,
			Func<string> predicateDescription,
			CollectionIndexOptions options)
			: base(it, grammars)
		{
			_predicate = predicate;
			_predicateDescription = predicateDescription;
			_options = options;
		}

		public HasItemConstraint(
			string it,
			ExpectationGrammars grammars,
			Func<TItem, ValueTask<bool>> predicate,
			Func<string> predicateDescription,
			CollectionIndexOptions options,
			Func<object?, bool>? useComparerOf = null,
			Action<ResultContextCollector>? appendOptionsContexts = null)
			: base(it, grammars)
		{
			_asyncPredicate = predicate;
			_predicateDescription = predicateDescription;
			_options = options;
			_useComparerOf = useComparerOf;
			_appendOptionsContexts = appendOptionsContexts;
		}

		/// <inheritdoc />
		public override void AppendContexts(ResultContextCollector contexts)
		{
			_collectionContext.AppendTo(contexts);
			_appendOptionsContexts?.Invoke(contexts);
		}

		public async Task<ConstraintResult> IsMetBy(TEnumerable actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			_collectionContext = default;
			_actual = default;
			_hasIndex = false;
			Actual = actual;
			if (actual.IsDefaultImmutableArray())
			{
				return this.AsNullSubject(It);
			}

			if (actual is null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			_useComparerOf?.Invoke(actual);
			CollectionItems<TItem> materialized = CollectionItems<TItem>.Materialize(actual, context);
			materialized.SetContext(ref _collectionContext);
			Outcome = Outcome.Failure;

			IEnumerable<TItem> items = materialized.Items;
			if (!TryCountForIndex(_options, actual, items, cancellationToken, out int? count))
			{
				Outcome = Outcome.Undecided;
				return this;
			}

			int index = -1;
			foreach (TItem item in items)
			{
				if (materialized.IsCanceledBeforeTheEnd(cancellationToken))
				{
					Outcome = Outcome.Undecided;
					return this;
				}

				index++;
				bool? isIndexInRange = IsIndexInRange(_options, index, count);
				if (isIndexInRange == false)
				{
					break;
				}

				if (isIndexInRange is null)
				{
					continue;
				}

				_hasIndex = true;
				_actual = item;
				bool isMatch = _asyncPredicate is null
					? UserCode.Invoke(_predicate!, item, "the predicate")
					: await _asyncPredicate(item);
				Outcome = isMatch ? Outcome.Success : Outcome.Failure;
				if (isMatch)
				{
					break;
				}
			}

			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("has an item ", "have an item ")).Append(_predicateDescription())
				.Append(_options.Match.GetDescription());

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_hasIndex)
			{
				if (_options.Match.OnlySingleIndex())
				{
					stringBuilder.Append(It).Append(" had item ");
					Formatter.Format(stringBuilder, _actual);
					stringBuilder.Append(_options.Match.GetDescription());
				}
				else
				{
					stringBuilder.Append(It).Append(" had no matching item").Append(_options.Match.GetDescription());
				}
			}
			else
			{
				stringBuilder.Append(It).Append(" had no item").Append(_options.Match.GetDescription());
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("does not have an item ", "do not have an item "))
				.Append(_predicateDescription())
				.Append(_options.Match.GetDescription());

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" had item ");
			Formatter.Format(stringBuilder, _actual);
			stringBuilder.Append(_options.Match.GetDescription());
		}
	}
}
