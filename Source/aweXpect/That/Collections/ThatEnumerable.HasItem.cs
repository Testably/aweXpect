using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;
#if NET8_0_OR_GREATER
using System.Collections.Immutable;
#endif

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect;

public static partial class ThatEnumerable
{
	/// <summary>
	///     Verifies that the collection has an item…
	/// </summary>
	[GuaranteesNotNull]
	public static HasItemWithConditionResult<IEnumerable<TItem>?, TItem> HasItem<TItem>(
		this IThat<IEnumerable<TItem>?> source)
	{
		CollectionIndexOptions indexOptions = new();
		PredicateOptions<TItem> options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new HasItemWithConditionResult<IEnumerable<TItem>?, TItem>(
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
	[GuaranteesNotNull]
	public static HasItemResult<IEnumerable<TItem>?> HasItem<TItem>(
		this IThat<IEnumerable<TItem>?> source, Func<TItem, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
	{
		CollectionIndexOptions indexOptions = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new HasItemResult<IEnumerable<TItem>?>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new HasItemConstraint<TItem>(expectationBuilder, it, grammars, predicate,
					() => doNotPopulateThisValue, indexOptions)),
			source,
			indexOptions);
	}

	/// <summary>
	///     Verifies that the collection has the <paramref name="expected" /> item…
	/// </summary>
	[GuaranteesNotNull]
	public static ObjectHasItemResult<IEnumerable<TItem>?, TItem> HasItem<TItem>(
		this IThat<IEnumerable<TItem>?> source, TItem expected)
	{
		CollectionIndexOptions indexOptions = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		ObjectEqualityOptions<TItem> options = new();
		return new ObjectHasItemResult<IEnumerable<TItem>?, TItem>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new HasAsyncItemConstraint<TItem>(expectationBuilder, it, grammars,
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
	[GuaranteesNotNull]
	public static StringHasItemResult<IEnumerable<string?>?> HasItem(
		this IThat<IEnumerable<string?>?> source, string? expected)
	{
		CollectionIndexOptions indexOptions = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		StringEqualityOptions options = new();
		return new StringHasItemResult<IEnumerable<string?>?>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new HasAsyncItemConstraint<string?>(expectationBuilder, it, grammars,
					a => options.AreConsideredEqual(a, expected),
					() => options.GetExpectation(expected, grammars),
					indexOptions)),
			source,
			indexOptions,
			options);
	}

	/// <summary>
	///     Verifies that the collection has an item…
	/// </summary>
	[GuaranteesNotNull]
	public static HasItemWithConditionResult<IEnumerable?, object?> HasItem(
		this IThat<IEnumerable?> source)
	{
		CollectionIndexOptions indexOptions = new();
		PredicateOptions<object?> options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new HasItemWithConditionResult<IEnumerable?, object?>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new HasItemForEnumerableConstraint<IEnumerable, object?>(expectationBuilder, it, grammars,
					x => options.Matches(x), options.GetDescription, indexOptions)),
			source,
			indexOptions,
			options);
	}

	/// <summary>
	///     Verifies that the collection has an item matching the <paramref name="predicate" />…
	/// </summary>
	[OverloadResolutionPriority(-1)]
	[GuaranteesNotNull]
	public static HasItemResult<IEnumerable> HasItem(
		this IThat<IEnumerable> source, Func<object?, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
	{
		CollectionIndexOptions indexOptions = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new HasItemResult<IEnumerable>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new HasItemForEnumerableConstraint<IEnumerable, object?>(
					expectationBuilder, it, grammars,
					predicate, () => doNotPopulateThisValue,
					indexOptions)),
			source,
			indexOptions);
	}

	/// <summary>
	///     Verifies that the collection has the <paramref name="expected" /> item…
	/// </summary>
	[GuaranteesNotNull]
	public static ObjectHasItemResult<IEnumerable, object?> HasItem(
		this IThat<IEnumerable> source, object? expected)
	{
		CollectionIndexOptions indexOptions = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		ObjectEqualityOptions<object?> options = new();
		return new ObjectHasItemResult<IEnumerable, object?>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new HasAsyncItemForEnumerableConstraint<IEnumerable, object?>(
					expectationBuilder, it, grammars,
					a => options.AreConsideredEqual(a, expected),
					() => $"{Formatter.Format(expected)}{options}",
					indexOptions)),
			source,
			indexOptions,
			options);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that the collection has an item matching the <paramref name="predicate" />…
	/// </summary>
	[GuaranteesNotNull]
	public static HasItemResult<ImmutableArray<TItem>> HasItem<TItem>(
		this IThat<ImmutableArray<TItem>> source, Func<TItem, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
	{
		CollectionIndexOptions indexOptions = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new HasItemResult<ImmutableArray<TItem>>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new HasItemForEnumerableConstraint<ImmutableArray<TItem>, TItem>(
					expectationBuilder, it, grammars,
					predicate, () => doNotPopulateThisValue,
					indexOptions)),
			source,
			indexOptions);
	}
#endif

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that the collection has an item…
	/// </summary>
	[GuaranteesNotNull]
	public static HasItemWithConditionResult<ImmutableArray<TItem>, TItem> HasItem<TItem>(
		this IThat<ImmutableArray<TItem>> source)
	{
		CollectionIndexOptions indexOptions = new();
		PredicateOptions<TItem> options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new HasItemWithConditionResult<ImmutableArray<TItem>, TItem>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new HasItemForEnumerableConstraint<ImmutableArray<TItem>, TItem>(expectationBuilder, it, grammars,
					x => options.Matches(x), options.GetDescription, indexOptions)),
			source,
			indexOptions,
			options);
	}
#endif

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that the collection has the <paramref name="expected" /> item…
	/// </summary>
	[GuaranteesNotNull]
	public static ObjectHasItemResult<ImmutableArray<TItem>, TItem> HasItem<TItem>(
		this IThat<ImmutableArray<TItem>> source, TItem expected)
	{
		CollectionIndexOptions indexOptions = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		ObjectEqualityOptions<TItem> options = new();
		return new ObjectHasItemResult<ImmutableArray<TItem>, TItem>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new HasAsyncItemForEnumerableConstraint<ImmutableArray<TItem>, TItem>(
					expectationBuilder, it, grammars,
					a => options.AreConsideredEqual(a, expected),
					() => $"{Formatter.Format(expected)}{options}",
					indexOptions)),
			source,
			indexOptions,
			options);
	}
#endif

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that the collection has the <paramref name="expected" /> item…
	/// </summary>
	[GuaranteesNotNull]
	public static StringHasItemResult<ImmutableArray<string?>> HasItem(
		this IThat<ImmutableArray<string?>> source, string? expected)
	{
		CollectionIndexOptions indexOptions = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		StringEqualityOptions options = new();
		return new StringHasItemResult<ImmutableArray<string?>>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new HasAsyncItemForEnumerableConstraint<ImmutableArray<string?>, string?>(
					expectationBuilder, it, grammars,
					a => options.AreConsideredEqual(a, expected),
					() => $"{Formatter.Format(expected)}{options}",
					indexOptions)),
			source,
			indexOptions,
			options);
	}
#endif

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
		: ConstraintResult.WithValue<IEnumerable<TItem>?>(grammars),
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
				Outcome = await predicate(item) ? Outcome.Success : Outcome.Failure;
				if (Outcome == Outcome.Success)
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
		: ConstraintResult.WithValue<TEnumerable>(grammars),
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
				Outcome = await predicate(item) ? Outcome.Success : Outcome.Failure;
				if (Outcome == Outcome.Success)
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
			if (Actual is null)
			{
				stringBuilder.ItWasNull(it, Grammars);
			}
			else if (_actual is not null)
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

	private sealed class HasItemConstraint<TItem>(
		ExpectationBuilder expectationBuilder,
		string it,
		ExpectationGrammars grammars,
		Func<TItem, bool> predicate,
		Func<string> predicateDescription,
		CollectionIndexOptions options)
		: ConstraintResult.WithValue<IEnumerable<TItem>?>(grammars),
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
				Outcome = predicate(item) ? Outcome.Success : Outcome.Failure;
				if (Outcome == Outcome.Success)
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

	private sealed class HasItemForEnumerableConstraint<TEnumerable, TItem>(
		ExpectationBuilder expectationBuilder,
		string it,
		ExpectationGrammars grammars,
		Func<TItem, bool> predicate,
		Func<string> predicateDescription,
		CollectionIndexOptions options)
		: ConstraintResult.WithValue<TEnumerable>(grammars),
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
				Outcome = predicate(item) ? Outcome.Success : Outcome.Failure;
				if (Outcome == Outcome.Success)
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
			if (Actual is null)
			{
				stringBuilder.ItWasNull(it, Grammars);
			}
			else if (_actual is not null)
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
