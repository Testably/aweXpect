#if NET8_0_OR_GREATER
using System;
using System.Collections.Generic;
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

public static partial class ThatAsyncEnumerable
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
	internal static HasItemWithConditionResult<IAsyncEnumerable<TItem>?, TItem>
		HasItemCore<TItem>(
			IThat<IAsyncEnumerable<TItem>?> subject,
			bool negated)
	{
		CollectionIndexOptions indexOptions = new();
		PredicateOptions<TItem> options = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new HasItemWithConditionResult<IAsyncEnumerable<TItem>?, TItem>(
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
	internal static HasItemResult<IAsyncEnumerable<TItem>?>
		HasMatchingItemCore<TItem>(
			IThat<IAsyncEnumerable<TItem>?> subject,
			Func<TItem, bool> predicate,
			string predicateExpression,
			bool negated)
	{
		predicate.ThrowIfNull();
		CollectionIndexOptions indexOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new HasItemResult<IAsyncEnumerable<TItem>?>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new HasItemConstraint<TItem>(expectationBuilder, it, grammars, predicate,
					() => $"matching {predicateExpression}", indexOptions).InvertIf(negated)),
			subject,
			indexOptions);
	}

	[CreateCollectionExpectation("HasItem", NegatedName = "DoesNotHaveItem", GuaranteesNotNull = true,
		Summary = HasTheItem, NegatedSummary = DoesNotHaveTheItem)]
	internal static ObjectHasItemResult<IAsyncEnumerable<TItem>?, TItem>
		HasTheItemCore<TItem>(
			IThat<IAsyncEnumerable<TItem>?> subject,
			TItem expected,
			bool negated)
	{
		CollectionIndexOptions indexOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		ObjectEqualityOptions<TItem> options = new();
		return new ObjectHasItemResult<IAsyncEnumerable<TItem>?, TItem>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new AsyncHasItemConstraint<TItem>(expectationBuilder, it, grammars,
					a => options.AreConsideredEqual(a, expected),
					() => options.GetItemExpectation(Formatter.Format(expected), comparison: "equal to"),
					indexOptions).InvertIf(negated)),
			subject,
			indexOptions,
			options);
	}

	[CreateCollectionExpectation("HasItem", NegatedName = "DoesNotHaveItem", GuaranteesNotNull = true,
		Summary = HasTheItem, NegatedSummary = DoesNotHaveTheItem)]
	internal static StringHasItemResult<IAsyncEnumerable<string?>?>
		HasTheItemForStringsCore(
			IThat<IAsyncEnumerable<string?>?> subject,
			string? expected,
			bool negated)
	{
		CollectionIndexOptions indexOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		StringEqualityOptions options = new(negated ? "unexpected" : "expected");
		return new StringHasItemResult<IAsyncEnumerable<string?>?>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new AsyncHasItemConstraint<string?>(expectationBuilder, it, grammars,
					a => options.AreConsideredEqual(a, expected),
					() => options.GetExpectation(expected, grammars),
					indexOptions).InvertIf(negated)),
			subject,
			indexOptions,
			options);
	}

	private sealed class HasItemConstraint<TItem>(
		ExpectationBuilder expectationBuilder,
		string it,
		ExpectationGrammars grammars,
		Func<TItem, bool> predicate,
		Func<string> predicateDescription,
		CollectionIndexOptions options)
		: ConstraintResult.WithNotNullValue<IAsyncEnumerable<TItem>?>(it, grammars),
			IAsyncContextConstraint<IAsyncEnumerable<TItem>?>
	{
		private TItem? _actual;
		private bool _hasIndex;

		public async Task<ConstraintResult> IsMetBy(IAsyncEnumerable<TItem>? actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			Actual = actual;
			if (actual is null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			IAsyncEnumerable<TItem> materialized =
				context.UseMaterializedAsyncEnumerable<TItem, IAsyncEnumerable<TItem>>(actual, cancellationToken);
			await expectationBuilder.AddCollectionContext(materialized as IMaterializedEnumerable<TItem>);
			_hasIndex = false;
			Outcome = Outcome.Failure;

			int? count = null;
			if (options.Match is CollectionIndexOptions.IMatchFromEnd)
			{
				count = (await (materialized as IMaterializedEnumerable<TItem>)!.MaterializeItems(null)).Count;
			}

			int index = -1;
			await foreach (TItem item in materialized.WithCancellation(cancellationToken))
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
				bool isMatch = UserCode.Invoke(predicate, item, "the predicate");
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

	private sealed class AsyncHasItemConstraint<TItem>(
		ExpectationBuilder expectationBuilder,
		string it,
		ExpectationGrammars grammars,
		Func<TItem, ValueTask<bool>> predicate,
		Func<string> predicateDescription,
		CollectionIndexOptions options)
		: ConstraintResult.WithNotNullValue<IAsyncEnumerable<TItem>?>(it, grammars),
			IAsyncContextConstraint<IAsyncEnumerable<TItem>?>
	{
		private TItem? _actual;
		private bool _hasIndex;

		public async Task<ConstraintResult> IsMetBy(IAsyncEnumerable<TItem>? actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			Actual = actual;
			if (actual is null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			IAsyncEnumerable<TItem> materialized =
				context.UseMaterializedAsyncEnumerable<TItem, IAsyncEnumerable<TItem>>(actual, cancellationToken);
			await expectationBuilder.AddCollectionContext(materialized as IMaterializedEnumerable<TItem>);
			_hasIndex = false;
			Outcome = Outcome.Failure;

			int? count = null;
			if (options.Match is CollectionIndexOptions.IMatchFromEnd)
			{
				count = (await (materialized as IMaterializedEnumerable<TItem>)!.MaterializeItems(null)).Count;
			}

			int index = -1;
			await foreach (TItem item in materialized.WithCancellation(cancellationToken))
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
}
#endif
