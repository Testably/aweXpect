#if NET8_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect;

public static partial class ThatAsyncEnumerable
{
	/// <summary>
	///     Verifies that the collection has an item…
	/// </summary>
	public static HasItemWithConditionResult<IAsyncEnumerable<TItem>?, TItem> HasItem<TItem>(
		this IThat<IAsyncEnumerable<TItem>?> source)
	{
		CollectionIndexOptions indexOptions = new();
		PredicateOptions<TItem> options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new HasItemWithConditionResult<IAsyncEnumerable<TItem>?, TItem>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new HasItemConstraint<TItem>(expectationBuilder, it, grammars,
					x => options.Matches(x),
					options.GetDescription,
					indexOptions)),
			source,
			indexOptions,
			options);
	}

	/// <summary>
	///     Verifies that the collection has an item matching the <paramref name="predicate" />…
	/// </summary>
	public static HasItemResult<IAsyncEnumerable<TItem>?> HasItem<TItem>(
		this IThat<IAsyncEnumerable<TItem>?> source, Func<TItem, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
	{
		CollectionIndexOptions indexOptions = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new HasItemResult<IAsyncEnumerable<TItem>?>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new HasItemConstraint<TItem>(expectationBuilder, it, grammars, predicate,
					() => doNotPopulateThisValue, indexOptions)),
			source,
			indexOptions);
	}

	/// <summary>
	///     Verifies that the collection has the <paramref name="expected" /> item…
	/// </summary>
	public static ObjectHasItemResult<IAsyncEnumerable<TItem>?, TItem> HasItem<TItem>(
		this IThat<IAsyncEnumerable<TItem>?> source, TItem expected)
	{
		CollectionIndexOptions indexOptions = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		ObjectEqualityOptions<TItem> options = new();
		return new ObjectHasItemResult<IAsyncEnumerable<TItem>?, TItem>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new AsyncHasItemConstraint<TItem>(expectationBuilder, it, grammars,
					a => options.AreConsideredEqual(a, expected),
					() => $"{Formatter.Format(expected)}{options}",
					indexOptions)),
			source,
			indexOptions,
			options);
	}

	/// <summary>
	///     Verifies that the collection has the <paramref name="expected" /> item…
	/// </summary>
	public static StringHasItemResult<IAsyncEnumerable<string?>?> HasItem(
		this IThat<IAsyncEnumerable<string?>?> source, string? expected)
	{
		CollectionIndexOptions indexOptions = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		StringEqualityOptions options = new();
		return new StringHasItemResult<IAsyncEnumerable<string?>?>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new AsyncHasItemConstraint<string?>(expectationBuilder, it, grammars,
					a => options.AreConsideredEqual(a, expected),
					() => options.GetExpectation(expected, grammars),
					indexOptions)),
			source,
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
		: ConstraintResult.WithValue<IAsyncEnumerable<TItem>?>(grammars),
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
				context.UseMaterializedAsyncEnumerable<TItem, IAsyncEnumerable<TItem>>(actual);
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
				Outcome = predicate(item) ? Outcome.Success : Outcome.Failure;
				if (Outcome == Outcome.Success)
				{
					break;
				}
			}

			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("has item ", "have item ")).Append(predicateDescription())
				.Append(options.Match.GetDescription());

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (Actual is null)
			{
				stringBuilder.ItWasNull(it, Grammars);
			}
			else if (_hasIndex)
			{
				if (options.Match.OnlySingleIndex())
				{
					stringBuilder.Append(it).Append(" had item ");
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

					stringBuilder.Append(it).Append(" did not match").Append(optionDescription);
				}
			}
			else
			{
				stringBuilder.Append(it).Append(" did not contain any item").Append(options.Match.GetDescription());
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("does not have item ", "do not have item "))
				.Append(predicateDescription())
				.Append(options.Match.GetDescription());

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_actual is null)
			{
				stringBuilder.ItWasNull(it, Grammars);
			}
			else
			{
				stringBuilder.Append(it).Append(" did");
			}
		}
	}

	private sealed class AsyncHasItemConstraint<TItem>(
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
		: ConstraintResult.WithValue<IAsyncEnumerable<TItem>?>(grammars),
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
				context.UseMaterializedAsyncEnumerable<TItem, IAsyncEnumerable<TItem>>(actual);
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
				Outcome = await predicate(item) ? Outcome.Success : Outcome.Failure;
				if (Outcome == Outcome.Success)
				{
					break;
				}
			}

			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("has item ", "have item ")).Append(predicateDescription())
				.Append(options.Match.GetDescription());

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (Actual is null)
			{
				stringBuilder.ItWasNull(it, Grammars);
			}
			else if (_hasIndex)
			{
				if (options.Match.OnlySingleIndex())
				{
					stringBuilder.Append(it).Append(" had item ");
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

					stringBuilder.Append(it).Append(" did not match").Append(optionDescription);
				}
			}
			else
			{
				stringBuilder.Append(it).Append(" did not contain any item").Append(options.Match.GetDescription());
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("does not have item ", "do not have item "))
				.Append(predicateDescription())
				.Append(options.Match.GetDescription());

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_actual is null)
			{
				stringBuilder.ItWasNull(it, Grammars);
			}
			else
			{
				stringBuilder.Append(it).Append(" did");
			}
		}
	}
}
#endif
