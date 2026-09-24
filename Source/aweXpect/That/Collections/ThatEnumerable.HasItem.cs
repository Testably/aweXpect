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

	[CreateCollectionExpectation("HasItem", NegatedName = "DoesNotHaveItem", GuaranteesNotNull = true,
		Summary = HasAnItem, NegatedSummary = DoesNotHaveAnItem)]
	internal static HasItemWithConditionResult<IEnumerable<TItem>?, TItem>
		HasItemCore<TItem>(
			IThat<IEnumerable<TItem>?> subject,
			bool negated)
	{
		CollectionIndexOptions indexOptions = new();
		PredicateOptions<TItem> options = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new HasItemWithConditionResult<IEnumerable<TItem>?, TItem>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new HasItemConstraint<TItem>(expectationBuilder, it, grammars,
					x => options.Matches(x),
					options.GetDescription,
					indexOptions).InvertIf(negated)),
			subject,
			indexOptions,
			options);
	}

	[CreateCollectionExpectation("HasItem", NegatedName = "DoesNotHaveItem", GuaranteesNotNull = true,
		Summary = HasMatchingItem, NegatedSummary = DoesNotHaveMatchingItem)]
	internal static HasItemResult<IEnumerable<TItem>?>
		HasMatchingItemCore<TItem>(
			IThat<IEnumerable<TItem>?> subject,
			Func<TItem, bool> predicate,
			string predicateExpression,
			bool negated)
	{
		CollectionIndexOptions indexOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new HasItemResult<IEnumerable<TItem>?>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new HasItemConstraint<TItem>(expectationBuilder, it, grammars, predicate,
					() => $"matching {predicateExpression}", indexOptions).InvertIf(negated)),
			subject,
			indexOptions);
	}

	[CreateCollectionExpectation("HasItem", NegatedName = "DoesNotHaveItem", GuaranteesNotNull = true,
		Summary = HasTheItem, NegatedSummary = DoesNotHaveTheItem)]
	internal static ObjectHasItemResult<IEnumerable<TItem>?, TItem>
		HasTheItemCore<TItem>(
			IThat<IEnumerable<TItem>?> subject,
			TItem expected,
			bool negated)
	{
		CollectionIndexOptions indexOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		ObjectEqualityOptions<TItem> options = new();
		return new ObjectHasItemResult<IEnumerable<TItem>?, TItem>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new HasAsyncItemConstraint<TItem>(expectationBuilder, it, grammars,
					a => options.AreConsideredEqual(a, expected),
					() => options.GetItemExpectation(Formatter.Format(expected), comparison: "equal to"),
					indexOptions).InvertIf(negated)),
			subject,
			indexOptions,
			options);
	}

	[CreateCollectionExpectation("HasItem", NegatedName = "DoesNotHaveItem", GuaranteesNotNull = true,
		Summary = HasTheItem, NegatedSummary = DoesNotHaveTheItem)]
	internal static StringHasItemResult<IEnumerable<string?>?>
		HasTheItemForStringsCore(
			IThat<IEnumerable<string?>?> subject,
			string? expected,
			bool negated)
	{
		CollectionIndexOptions indexOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		StringEqualityOptions options = new(negated ? "unexpected" : "expected");
		return new StringHasItemResult<IEnumerable<string?>?>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new HasAsyncItemConstraint<string?>(expectationBuilder, it, grammars,
					a => options.AreConsideredEqual(a, expected),
					() => options.GetExpectation(expected, grammars),
					indexOptions).InvertIf(negated)),
			subject,
			indexOptions,
			options);
	}

	[CreateCollectionExpectation("HasItem", NegatedName = "DoesNotHaveItem", GuaranteesNotNull = true,
		Summary = HasAnItem, NegatedSummary = DoesNotHaveAnItem)]
	internal static HasItemWithConditionResult<IEnumerable?, object?>
		HasItemForEnumerableCore(
			IThat<IEnumerable?> subject,
			bool negated)
	{
		CollectionIndexOptions indexOptions = new();
		PredicateOptions<object?> options = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new HasItemWithConditionResult<IEnumerable?, object?>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new HasItemForEnumerableConstraint<IEnumerable, object?>(expectationBuilder, it, grammars,
					x => options.Matches(x), options.GetDescription, indexOptions).InvertIf(negated)),
			subject,
			indexOptions,
			options);
	}

	[CreateCollectionExpectation("HasItem", NegatedName = "DoesNotHaveItem", GuaranteesNotNull = true, Priority = -1,
		Summary = HasMatchingItem, NegatedSummary = DoesNotHaveMatchingItem)]
	internal static HasItemResult<IEnumerable?>
		HasMatchingItemForEnumerableCore(
			IThat<IEnumerable?> subject,
			Func<object?, bool> predicate,
			string predicateExpression,
			bool negated)
	{
		CollectionIndexOptions indexOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new HasItemResult<IEnumerable?>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new HasItemForEnumerableConstraint<IEnumerable, object?>(
					expectationBuilder, it, grammars,
					predicate, () => $"matching {predicateExpression}",
					indexOptions).InvertIf(negated)),
			subject,
			indexOptions);
	}

	[CreateCollectionExpectation("HasItem", NegatedName = "DoesNotHaveItem", GuaranteesNotNull = true,
		Summary = HasTheItem, NegatedSummary = DoesNotHaveTheItem)]
	internal static ObjectHasItemResult<IEnumerable?, object?>
		HasTheItemForEnumerableCore(
			IThat<IEnumerable?> subject,
			object? expected,
			bool negated)
	{
		CollectionIndexOptions indexOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		ObjectEqualityOptions<object?> options = new();
		return new ObjectHasItemResult<IEnumerable?, object?>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new HasAsyncItemForEnumerableConstraint<IEnumerable, object?>(
					expectationBuilder, it, grammars,
					a => options.AreConsideredEqual(a, expected),
					() => options.GetItemExpectation(Formatter.Format(expected), comparison: "equal to"),
					indexOptions).InvertIf(negated)),
			subject,
			indexOptions,
			options);
	}

	[CreateCollectionExpectation("HasItem", NegatedName = "DoesNotHaveItem", PerSubject = true,
		Summary = HasMatchingItem, NegatedSummary = DoesNotHaveMatchingItem)]
	internal static HasItemResult<TCollection>
		HasMatchingItemForCollectionCore<TCollection, TItem>(
			IThat<TCollection> subject,
			Func<TItem, bool> predicate,
			string predicateExpression,
			bool negated)
		where TCollection : IEnumerable
	{
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

	[CreateCollectionExpectation("HasItem", NegatedName = "DoesNotHaveItem", PerSubject = true,
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

	[CreateCollectionExpectation("HasItem", NegatedName = "DoesNotHaveItem", PerSubject = true,
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

	[CreateCollectionExpectation("HasItem", NegatedName = "DoesNotHaveItem", PerSubject = true,
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

	private sealed class HasAsyncItemConstraint<TItem>(
		ExpectationBuilder expectationBuilder,
		string it,
		ExpectationGrammars grammars,
#if NET8_0_OR_GREATER
		Func<TItem, ValueTask<bool>> predicate,
#else
		Func<TItem, Task<bool>> predicate,
#endif
		Func<string> predicateDescription,
		CollectionIndexOptions options)
		: ConstraintResult.WithNotNullValue<IEnumerable<TItem>?>(it, grammars),
			IAsyncContextConstraint<IEnumerable<TItem>?>
	{
		private TItem? _actual;
		private bool _hasIndex;

#pragma warning disable S3776 // Cognitive Complexity of methods should not be too high
		public async Task<ConstraintResult> IsMetBy(IEnumerable<TItem>? actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			Actual = actual;
			if (actual is null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			IEnumerable<TItem> materialized = context.UseMaterializedEnumerable<TItem, IEnumerable<TItem>>(actual);
			expectationBuilder.AddCollectionContext(materialized);
			_hasIndex = false;
			Outcome = Outcome.Failure;

			int? count = null;
			if (options.Match is CollectionIndexOptions.IMatchFromEnd)
			{
				count = actual is ICollection<TItem> collection ? collection.Count : materialized.Count();
			}

			int index = -1;
			foreach (TItem item in materialized)
			{
				index++;
				bool? isIndexInRange = options.Match switch
				{
					CollectionIndexOptions.IMatchFromBeginning fromBeginning => fromBeginning.MatchesIndex(index),
					CollectionIndexOptions.IMatchFromEnd fromEnd => fromEnd.MatchesIndex(index, count),
					_ => false,
				};
				if (isIndexInRange != true)
				{
					if (isIndexInRange == false)
					{
						break;
					}

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
#pragma warning restore S3776

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("has item ", "have item ")).Append(predicateDescription())
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
					string optionDescription = options.Match.GetDescription();
					if (string.IsNullOrEmpty(optionDescription))
					{
						optionDescription = " at any index";
					}

					stringBuilder.Append(It).Append(" did not match").Append(optionDescription);
				}
			}
			else
			{
				stringBuilder.Append(It).Append(" did not contain any item").Append(options.Match.GetDescription());
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("does not have item ", "do not have item "))
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
#if NET8_0_OR_GREATER
		Func<TItem, ValueTask<bool>> predicate,
#else
		Func<TItem, Task<bool>> predicate,
#endif
		Func<string> predicateDescription,
		CollectionIndexOptions options)
		: ConstraintResult.WithNotNullValue<TEnumerable>(it, grammars),
			IAsyncContextConstraint<TEnumerable>
		where TEnumerable : IEnumerable?
	{
		private object? _actual;

#pragma warning disable S3776 // Cognitive Complexity of methods should not be too high
		public async Task<ConstraintResult> IsMetBy(TEnumerable actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			Actual = actual;
			if (actual is null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			IEnumerable materialized = context.UseMaterializedEnumerable(actual);
			expectationBuilder.AddCollectionContext(materialized);
			Outcome = Outcome.Failure;

			int? count = null;
			if (options.Match is CollectionIndexOptions.IMatchFromEnd)
			{
				count = actual is ICollection collection ? collection.Count : materialized.Cast<TItem>().Count();
			}

			int index = -1;
			foreach (TItem item in materialized.Cast<TItem>())
			{
				index++;
				bool? isIndexInRange = options.Match switch
				{
					CollectionIndexOptions.IMatchFromBeginning fromBeginning => fromBeginning.MatchesIndex(index),
					CollectionIndexOptions.IMatchFromEnd fromEnd => fromEnd.MatchesIndex(index, count),
					_ => false,
				};
				if (isIndexInRange != true)
				{
					if (isIndexInRange == false)
					{
						break;
					}

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
#pragma warning restore S3776

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("has item ", "have item ")).Append(predicateDescription())
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
					string optionDescription = options.Match.GetDescription();
					if (string.IsNullOrEmpty(optionDescription))
					{
						optionDescription = " at any index";
					}

					stringBuilder.Append(It).Append(" did not match").Append(optionDescription);
				}
			}
			else
			{
				stringBuilder.Append(It).Append(" did not contain any item").Append(options.Match.GetDescription());
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("does not have item ", "do not have item "))
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
			IContextConstraint<IEnumerable<TItem>?>
	{
		private TItem? _actual;
		private bool _hasIndex;

#pragma warning disable S3776 // Cognitive Complexity of methods should not be too high
		public ConstraintResult IsMetBy(IEnumerable<TItem>? actual, IEvaluationContext context)
		{
			Actual = actual;
			if (actual is null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			IEnumerable<TItem> materialized = context.UseMaterializedEnumerable<TItem, IEnumerable<TItem>>(actual);
			expectationBuilder.AddCollectionContext(materialized);
			_hasIndex = false;
			Outcome = Outcome.Failure;

			int? count = null;
			if (options.Match is CollectionIndexOptions.IMatchFromEnd)
			{
				count = actual is ICollection<TItem> collection ? collection.Count : materialized.Count();
			}

			int index = -1;
			foreach (TItem item in materialized)
			{
				index++;
				bool? isIndexInRange = options.Match switch
				{
					CollectionIndexOptions.IMatchFromBeginning fromBeginning => fromBeginning.MatchesIndex(index),
					CollectionIndexOptions.IMatchFromEnd fromEnd => fromEnd.MatchesIndex(index, count),
					_ => false,
				};
				if (isIndexInRange != true)
				{
					if (isIndexInRange == false)
					{
						break;
					}

					continue;
				}

				_hasIndex = true;
				_actual = item;
				bool isMatch = predicate(item);
				Outcome = isMatch ? Outcome.Success : Outcome.Failure;
				if (isMatch)
				{
					break;
				}
			}

			return this;
		}
#pragma warning restore S3776

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("has item ", "have item ")).Append(predicateDescription())
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
					string optionDescription = options.Match.GetDescription();
					if (string.IsNullOrEmpty(optionDescription))
					{
						optionDescription = " at any index";
					}

					stringBuilder.Append(It).Append(" did not match").Append(optionDescription);
				}
			}
			else
			{
				stringBuilder.Append(It).Append(" did not contain any item").Append(options.Match.GetDescription());
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("does not have item ", "do not have item "))
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
			IContextConstraint<TEnumerable>
		where TEnumerable : IEnumerable?
	{
		private object? _actual;

#pragma warning disable S3776 // Cognitive Complexity of methods should not be too high
		public ConstraintResult IsMetBy(TEnumerable actual, IEvaluationContext context)
		{
			Actual = actual;
			if (actual is null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			IEnumerable materialized = context.UseMaterializedEnumerable(actual);
			expectationBuilder.AddCollectionContext(materialized);
			Outcome = Outcome.Failure;

			int? count = null;
			if (options.Match is CollectionIndexOptions.IMatchFromEnd)
			{
				count = actual is ICollection collection ? collection.Count : materialized.Cast<TItem>().Count();
			}

			int index = -1;
			foreach (TItem item in materialized.Cast<TItem>())
			{
				index++;
				bool? isIndexInRange = options.Match switch
				{
					CollectionIndexOptions.IMatchFromBeginning fromBeginning => fromBeginning.MatchesIndex(index),
					CollectionIndexOptions.IMatchFromEnd fromEnd => fromEnd.MatchesIndex(index, count),
					_ => false,
				};
				if (isIndexInRange != true)
				{
					if (isIndexInRange == false)
					{
						break;
					}

					continue;
				}

				_actual = item;
				bool isMatch = predicate(item);
				Outcome = isMatch ? Outcome.Success : Outcome.Failure;
				if (isMatch)
				{
					break;
				}
			}

			return this;
		}
#pragma warning restore S3776

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("has item ", "have item ")).Append(predicateDescription())
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
					string optionDescription = options.Match.GetDescription();
					if (string.IsNullOrEmpty(optionDescription))
					{
						optionDescription = " at any index";
					}

					stringBuilder.Append(It).Append(" did not match").Append(optionDescription);
				}
			}
			else
			{
				stringBuilder.Append(It).Append(" did not contain any item").Append(options.Match.GetDescription());
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("does not have item ", "do not have item "))
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
