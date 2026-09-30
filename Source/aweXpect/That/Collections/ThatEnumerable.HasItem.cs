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
				=> new HasItemConstraint<TItem>(expectationBuilder, it, grammars,
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
				=> new HasItemConstraint<TItem>(expectationBuilder, it, grammars, predicate,
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
				return new HasAsyncItemConstraint<TItem>(expectationBuilder, it, grammars,
					a => itemOptions.AreConsideredEqual(a, expected),
					() => options.GetItemExpectation(Formatter.Format(expected), comparison: "equal to") +
					      itemOptions.Comparer,
					indexOptions,
					itemOptions.UseComparerOf).InvertIf(negated);
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
				return new HasAsyncItemConstraint<TItem>(expectationBuilder, it, grammars,
					a => itemOptions.AreConsideredEqual(a, expected),
					() => options.GetItemExpectation(Formatter.Format(expected), comparison: "equal to") +
					      itemOptions.Comparer,
					indexOptions,
					itemOptions.UseComparerOf).InvertIf(negated);
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
				return new HasAsyncItemConstraint<string?>(expectationBuilder, it, grammars,
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
				=> new HasItemForEnumerableConstraint<IEnumerable, object?>(expectationBuilder, it, grammars,
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
				=> new HasItemForEnumerableConstraint<IEnumerable, object?>(
					expectationBuilder, it, grammars,
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
				return new HasAsyncItemForEnumerableConstraint<IEnumerable, object?>(
					expectationBuilder, it, grammars,
					a => itemOptions.AreConsideredEqual(a, expected),
					() => options.GetItemExpectation(Formatter.Format(expected), comparison: "equal to") +
					      itemOptions.Comparer,
					indexOptions,
					itemOptions.UseComparerOf).InvertIf(negated);
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
				=> new HasItemForEnumerableConstraint<TCollection, TItem>(
					expectationBuilder, it, grammars,
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
				=> new HasItemForEnumerableConstraint<TCollection, TItem>(expectationBuilder, it, grammars,
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
				=> new HasAsyncItemForEnumerableConstraint<TCollection, TItem>(
					expectationBuilder, it, grammars,
					a => options.AreConsideredEqual(a, expected),
					() => options.GetItemExpectation(Formatter.Format(expected), comparison: "equal to"),
					indexOptions).InvertIf(negated)),
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
				=> new HasAsyncItemForEnumerableConstraint<TCollection, TItem>(
					expectationBuilder, it, grammars,
					a => options.AreConsideredEqual(a, expected),
					() => options.GetItemExpectation(Formatter.Format(expected), comparison: "equal to"),
					indexOptions).InvertIf(negated)),
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
				=> new HasAsyncItemForEnumerableConstraint<TCollection, string?>(
					expectationBuilder, it, grammars,
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

	private sealed class HasAsyncItemConstraint<TItem>(
		ExpectationBuilder expectationBuilder,
		string it,
		ExpectationGrammars grammars,
		Func<TItem, ValueTask<bool>> predicate,
		Func<string> predicateDescription,
		CollectionIndexOptions options,
		Func<object?, bool>? useComparerOf = null)
		: ConstraintResult.WithNotNullValue<IEnumerable<TItem>?>(it, grammars),
			IAsyncContextConstraint<IEnumerable<TItem>?>
	{
		private TItem? _actual;
		private bool _hasIndex;

		public async Task<ConstraintResult> IsMetBy(IEnumerable<TItem>? actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			Actual = actual;
			if (actual is null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			useComparerOf?.Invoke(actual);
			IEnumerable<TItem> materialized = context.UseMaterializedEnumerable<TItem>(actual);
			expectationBuilder.AddCollectionContext(materialized);
			_hasIndex = false;
			Outcome = Outcome.Failure;

			if (!TryCountForIndex(options, actual, materialized, cancellationToken, out int? count))
			{
				Outcome = Outcome.Undecided;
				return this;
			}

			int index = -1;
			foreach (TItem item in materialized)
			{
				if (cancellationToken.IsCancellationRequested)
				{
					Outcome = Outcome.Undecided;
					return this;
				}

				index++;
				bool? isIndexInRange = IsIndexInRange(options, index, count);
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
				bool isMatch = await predicate(item);
				Outcome = isMatch ? Outcome.Success : Outcome.Failure;
				if (isMatch)
				{
					break;
				}
			}

			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("has an item ", "have an item ")).Append(predicateDescription())
				.Append(options.Match.GetDescription());

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_hasIndex)
			{
				if (options.Match.OnlySingleIndex())
				{
					stringBuilder.Append(It).Append(" had item ");
					Formatter.Format(stringBuilder, _actual);
					stringBuilder.Append(options.Match.GetDescription());
				}
				else
				{
					stringBuilder.Append(It).Append(" had no matching item").Append(options.Match.GetDescription());
				}
			}
			else
			{
				stringBuilder.Append(It).Append(" had no item").Append(options.Match.GetDescription());
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("does not have an item ", "do not have an item "))
				.Append(predicateDescription())
				.Append(options.Match.GetDescription());

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" had item ");
			Formatter.Format(stringBuilder, _actual);
			stringBuilder.Append(options.Match.GetDescription());
		}
	}

	private sealed class HasAsyncItemForEnumerableConstraint<TEnumerable, TItem>(
		ExpectationBuilder expectationBuilder,
		string it,
		ExpectationGrammars grammars,
		Func<TItem, ValueTask<bool>> predicate,
		Func<string> predicateDescription,
		CollectionIndexOptions options,
		Func<object?, bool>? useComparerOf = null)
		: ConstraintResult.WithNotNullValue<TEnumerable>(it, grammars),
			IAsyncContextConstraint<TEnumerable>
		where TEnumerable : IEnumerable?
	{
		private object? _actual;

		public async Task<ConstraintResult> IsMetBy(TEnumerable actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
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

			useComparerOf?.Invoke(actual);
			IEnumerable materialized = context.UseMaterializedEnumerable(actual);
			expectationBuilder.AddCollectionContext(materialized);
			Outcome = Outcome.Failure;

			if (!TryCountForIndex(options, actual, materialized.Cast<TItem>(), cancellationToken, out int? count))
			{
				Outcome = Outcome.Undecided;
				return this;
			}

			int index = -1;
			foreach (TItem item in materialized.Cast<TItem>())
			{
				if (cancellationToken.IsCancellationRequested)
				{
					Outcome = Outcome.Undecided;
					return this;
				}

				index++;
				bool? isIndexInRange = IsIndexInRange(options, index, count);
				if (isIndexInRange == false)
				{
					break;
				}

				if (isIndexInRange is null)
				{
					continue;
				}

				_actual = item;
				bool isMatch = await predicate(item);
				Outcome = isMatch ? Outcome.Success : Outcome.Failure;
				if (isMatch)
				{
					break;
				}
			}

			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("has an item ", "have an item ")).Append(predicateDescription())
				.Append(options.Match.GetDescription());

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_actual is not null)
			{
				if (options.Match.OnlySingleIndex())
				{
					stringBuilder.Append(It).Append(" had item ");
					Formatter.Format(stringBuilder, _actual);
					stringBuilder.Append(options.Match.GetDescription());
				}
				else
				{
					stringBuilder.Append(It).Append(" had no matching item").Append(options.Match.GetDescription());
				}
			}
			else
			{
				stringBuilder.Append(It).Append(" had no item").Append(options.Match.GetDescription());
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("does not have an item ", "do not have an item "))
				.Append(predicateDescription())
				.Append(options.Match.GetDescription());

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" had item ");
			Formatter.Format(stringBuilder, _actual);
			stringBuilder.Append(options.Match.GetDescription());
		}
	}

	private sealed class HasItemConstraint<TItem>(
		ExpectationBuilder expectationBuilder,
		string it,
		ExpectationGrammars grammars,
		Func<TItem, bool> predicate,
		Func<string> predicateDescription,
		CollectionIndexOptions options)
		: ConstraintResult.WithNotNullValue<IEnumerable<TItem>?>(it, grammars),
			IAsyncContextConstraint<IEnumerable<TItem>?>
	{
		private TItem? _actual;
		private bool _hasIndex;

		public Task<ConstraintResult> IsMetBy(IEnumerable<TItem>? actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			Actual = actual;
			if (actual is null)
			{
				Outcome = Outcome.Failure;
				return Task.FromResult<ConstraintResult>(this);
			}

			IEnumerable<TItem> materialized = context.UseMaterializedEnumerable<TItem>(actual);
			expectationBuilder.AddCollectionContext(materialized);
			_hasIndex = false;
			Outcome = Outcome.Failure;

			if (!TryCountForIndex(options, actual, materialized, cancellationToken, out int? count))
			{
				Outcome = Outcome.Undecided;
				return Task.FromResult<ConstraintResult>(this);
			}

			int index = -1;
			foreach (TItem item in materialized)
			{
				if (cancellationToken.IsCancellationRequested)
				{
					Outcome = Outcome.Undecided;
					return Task.FromResult<ConstraintResult>(this);
				}

				index++;
				bool? isIndexInRange = IsIndexInRange(options, index, count);
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
				bool isMatch = UserCode.Invoke(predicate, item, "the predicate");
				Outcome = isMatch ? Outcome.Success : Outcome.Failure;
				if (isMatch)
				{
					break;
				}
			}

			return Task.FromResult<ConstraintResult>(this);
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("has an item ", "have an item ")).Append(predicateDescription())
				.Append(options.Match.GetDescription());

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_hasIndex)
			{
				if (options.Match.OnlySingleIndex())
				{
					stringBuilder.Append(It).Append(" had item ");
					Formatter.Format(stringBuilder, _actual);
					stringBuilder.Append(options.Match.GetDescription());
				}
				else
				{
					stringBuilder.Append(It).Append(" had no matching item").Append(options.Match.GetDescription());
				}
			}
			else
			{
				stringBuilder.Append(It).Append(" had no item").Append(options.Match.GetDescription());
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("does not have an item ", "do not have an item "))
				.Append(predicateDescription())
				.Append(options.Match.GetDescription());

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" had item ");
			Formatter.Format(stringBuilder, _actual);
			stringBuilder.Append(options.Match.GetDescription());
		}
	}

	private sealed class HasItemForEnumerableConstraint<TEnumerable, TItem>(
		ExpectationBuilder expectationBuilder,
		string it,
		ExpectationGrammars grammars,
		Func<TItem, bool> predicate,
		Func<string> predicateDescription,
		CollectionIndexOptions options)
		: ConstraintResult.WithNotNullValue<TEnumerable>(it, grammars),
			IAsyncContextConstraint<TEnumerable>
		where TEnumerable : IEnumerable?
	{
		private object? _actual;

		public Task<ConstraintResult> IsMetBy(TEnumerable actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			Actual = actual;
			if (actual.IsDefaultImmutableArray())
			{
				return Task.FromResult(this.AsNullSubject(It));
			}

			if (actual is null)
			{
				Outcome = Outcome.Failure;
				return Task.FromResult<ConstraintResult>(this);
			}

			IEnumerable materialized = context.UseMaterializedEnumerable(actual);
			expectationBuilder.AddCollectionContext(materialized);
			Outcome = Outcome.Failure;

			if (!TryCountForIndex(options, actual, materialized.Cast<TItem>(), cancellationToken, out int? count))
			{
				Outcome = Outcome.Undecided;
				return Task.FromResult<ConstraintResult>(this);
			}

			int index = -1;
			foreach (TItem item in materialized.Cast<TItem>())
			{
				if (cancellationToken.IsCancellationRequested)
				{
					Outcome = Outcome.Undecided;
					return Task.FromResult<ConstraintResult>(this);
				}

				index++;
				bool? isIndexInRange = IsIndexInRange(options, index, count);
				if (isIndexInRange == false)
				{
					break;
				}

				if (isIndexInRange is null)
				{
					continue;
				}

				_actual = item;
				bool isMatch = UserCode.Invoke(predicate, item, "the predicate");
				Outcome = isMatch ? Outcome.Success : Outcome.Failure;
				if (isMatch)
				{
					break;
				}
			}

			return Task.FromResult<ConstraintResult>(this);
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("has an item ", "have an item ")).Append(predicateDescription())
				.Append(options.Match.GetDescription());

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_actual is not null)
			{
				if (options.Match.OnlySingleIndex())
				{
					stringBuilder.Append(It).Append(" had item ");
					Formatter.Format(stringBuilder, _actual);
					stringBuilder.Append(options.Match.GetDescription());
				}
				else
				{
					stringBuilder.Append(It).Append(" had no matching item").Append(options.Match.GetDescription());
				}
			}
			else
			{
				stringBuilder.Append(It).Append(" had no item").Append(options.Match.GetDescription());
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("does not have an item ", "do not have an item "))
				.Append(predicateDescription())
				.Append(options.Match.GetDescription());

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" had item ");
			Formatter.Format(stringBuilder, _actual);
			stringBuilder.Append(options.Match.GetDescription());
		}
	}
}
