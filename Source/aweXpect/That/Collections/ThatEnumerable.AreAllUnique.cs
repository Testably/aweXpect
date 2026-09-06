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
	///     Verifies that the collection only contains unique items.
	/// </summary>
	public static ObjectEqualityResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem> AreAllUnique<TItem>(
		this IThat<IEnumerable<TItem>?> source)
	{
		ObjectEqualityOptions<TItem> options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new ObjectEqualityResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new AreAllUniqueConstraint<TItem, TItem>(expectationBuilder, it, grammars, options)),
			source, options
		);
	}

	/// <summary>
	///     Verifies that the collection only contains unique items.
	/// </summary>
	public static StringEqualityResult<IEnumerable<string?>, IThat<IEnumerable<string?>?>> AreAllUnique(
		this IThat<IEnumerable<string?>?> source)
	{
		StringEqualityOptions options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new StringEqualityResult<IEnumerable<string?>, IThat<IEnumerable<string?>?>>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new AreAllUniqueConstraint<string, string>(expectationBuilder, it, grammars, options)),
			source, options
		);
	}

	/// <summary>
	///     Verifies that the collection only contains items with unique members specified by the
	///     <paramref name="memberAccessor" />.
	/// </summary>
	public static ObjectEqualityResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TMember> AreAllUnique<TItem,
		TMember>(
		this IThat<IEnumerable<TItem>?> source,
		Func<TItem, TMember> memberAccessor,
		[CallerArgumentExpression("memberAccessor")]
		string doNotPopulateThisValue = "")
	{
		ObjectEqualityOptions<TMember> options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new ObjectEqualityResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TMember>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new AreAllUniqueWithPredicateConstraint<TItem, TMember, TMember>(
					expectationBuilder,
					it, grammars,
					memberAccessor,
					doNotPopulateThisValue.TrimCommonWhiteSpace(),
					options)),
			source, options
		);
	}

	/// <summary>
	///     Verifies that the collection only contains items with unique members specified by the
	///     <paramref name="memberAccessor" />.
	/// </summary>
	public static StringEqualityResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>> AreAllUnique<TItem>(
		this IThat<IEnumerable<TItem>?> source,
		Func<TItem, string> memberAccessor,
		[CallerArgumentExpression("memberAccessor")]
		string doNotPopulateThisValue = "")
	{
		StringEqualityOptions options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new StringEqualityResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new AreAllUniqueWithPredicateConstraint<TItem, string, string>(
					expectationBuilder,
					it, grammars,
					memberAccessor,
					doNotPopulateThisValue.TrimCommonWhiteSpace(),
					options)),
			source, options
		);
	}

	/// <summary>
	///     Verifies that the collection only contains unique items.
	/// </summary>
	[OverloadResolutionPriority(-1)]
	public static ObjectEqualityResult<IEnumerable, IThat<IEnumerable>, object?> AreAllUnique(
		this IThat<IEnumerable> source)
	{
		ObjectEqualityOptions<object?> options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new ObjectEqualityResult<IEnumerable, IThat<IEnumerable>, object?>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new AreAllUniqueForEnumerableConstraint<IEnumerable, object?>(expectationBuilder, it, grammars,
					options)),
			source, options
		);
	}

	/// <summary>
	///     Verifies that the collection only contains items with unique members specified by the
	///     <paramref name="memberAccessor" />.
	/// </summary>
	[OverloadResolutionPriority(-1)]
	public static ObjectEqualityResult<IEnumerable, IThat<IEnumerable>, TMember> AreAllUnique<TMember>(
		this IThat<IEnumerable> source,
		Func<object?, TMember> memberAccessor,
		[CallerArgumentExpression("memberAccessor")]
		string doNotPopulateThisValue = "")
	{
		ObjectEqualityOptions<TMember> options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new ObjectEqualityResult<IEnumerable, IThat<IEnumerable>, TMember>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new AreAllUniqueWithPredicateForEnumerableConstraint<IEnumerable, TMember, TMember>(
					expectationBuilder,
					it, grammars,
					memberAccessor,
					doNotPopulateThisValue.TrimCommonWhiteSpace(),
					options)),
			source, options
		);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that the collection only contains unique items.
	/// </summary>
	public static ObjectEqualityResult<ImmutableArray<TItem>, IThat<ImmutableArray<TItem>>, TItem> AreAllUnique<TItem>(
		this IThat<ImmutableArray<TItem>> source)
	{
		ObjectEqualityOptions<TItem> options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new ObjectEqualityResult<ImmutableArray<TItem>, IThat<ImmutableArray<TItem>>, TItem>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new AreAllUniqueForEnumerableConstraint<ImmutableArray<TItem>, TItem>(expectationBuilder, it,
					grammars, options)),
			source, options
		);
	}
#endif

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that the collection only contains unique items.
	/// </summary>
	public static StringEqualityResult<ImmutableArray<string?>, IThat<ImmutableArray<string?>?>> AreAllUnique(
		this IThat<ImmutableArray<string?>?> source)
	{
		StringEqualityOptions options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new StringEqualityResult<ImmutableArray<string?>, IThat<ImmutableArray<string?>?>>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new AreAllUniqueForEnumerableConstraint<string, string>(expectationBuilder, it, grammars, options)),
			source, options
		);
	}
#endif

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that the collection only contains items with unique members specified by the
	///     <paramref name="memberAccessor" />.
	/// </summary>
	public static ObjectEqualityResult<ImmutableArray<TItem>, IThat<ImmutableArray<TItem>>, TMember> AreAllUnique<
		TItem,
		TMember>(
		this IThat<ImmutableArray<TItem>> source,
		Func<TItem, TMember> memberAccessor,
		[CallerArgumentExpression("memberAccessor")]
		string doNotPopulateThisValue = "")
	{
		ObjectEqualityOptions<TMember> options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new ObjectEqualityResult<ImmutableArray<TItem>, IThat<ImmutableArray<TItem>>, TMember>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new AreAllUniqueWithPredicateForEnumerableConstraint<ImmutableArray<TItem>, TMember, TMember>(
					expectationBuilder,
					it, grammars,
					v => memberAccessor((TItem)v!),
					doNotPopulateThisValue.TrimCommonWhiteSpace(),
					options)),
			source, options
		);
	}
#endif

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that the collection only contains items with unique members specified by the
	///     <paramref name="memberAccessor" />.
	/// </summary>
	public static StringEqualityResult<ImmutableArray<TItem>, IThat<ImmutableArray<TItem>>> AreAllUnique<TItem>(
		this IThat<ImmutableArray<TItem>> source,
		Func<TItem, string> memberAccessor,
		[CallerArgumentExpression("memberAccessor")]
		string doNotPopulateThisValue = "")
	{
		StringEqualityOptions options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new StringEqualityResult<ImmutableArray<TItem>, IThat<ImmutableArray<TItem>>>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new AreAllUniqueWithPredicateForEnumerableConstraint<ImmutableArray<TItem>, string, string>(
					expectationBuilder,
					it, grammars,
					v => memberAccessor((TItem)v!),
					doNotPopulateThisValue.TrimCommonWhiteSpace(),
					options)),
			source, options
		);
	}
#endif

	private sealed class AreAllUniqueConstraint<TItem, TMatch>(
		ExpectationBuilder expectationBuilder,
		string it,
		ExpectationGrammars grammars,
		IOptionsEquality<TMatch> options)
		: ConstraintResult.WithNotNullValue<IEnumerable<TItem>?>(it, grammars),
			IAsyncContextConstraint<IEnumerable<TItem>?>
		where TItem : TMatch
	{
		private readonly List<TItem> _duplicates = [];

		public async Task<ConstraintResult> IsMetBy(IEnumerable<TItem>? actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			Actual = actual;
			if (actual is null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			IEnumerable<TItem> materialized = context
				.UseMaterializedEnumerable<TItem, IEnumerable<TItem>>(actual);
			List<TItem> checkedItems = new();

			IOptionsEquality<TMatch> o = options;
			foreach (TItem item in materialized)
			{
				if (await checkedItems.AnyButNotInDuplicatesAsync(_duplicates,
					    compareWith => o.AreConsideredEqual(item, compareWith)))
				{
					_duplicates.Add(item);
				}

				checkedItems.Add(item);
			}

			Outcome = _duplicates.Any() ? Outcome.Failure : Outcome.Success;
			expectationBuilder.AddCollectionContext(materialized);
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("only has unique items", "only have unique items"));
			stringBuilder.Append(options);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(CollectionHelpers.CreateDuplicateFailureMessage(It, _duplicates));

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("has duplicate items", "have duplicate items"));
			stringBuilder.Append(options);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("all were unique");
	}

	private sealed class AreAllUniqueWithPredicateConstraint<TItem, TMember, TMatch>(
		ExpectationBuilder expectationBuilder,
		string it,
		ExpectationGrammars grammars,
		Func<TItem, TMember> memberAccessor,
		string memberAccessorExpression,
		IOptionsEquality<TMatch> options)
		: ConstraintResult.WithNotNullValue<IEnumerable<TItem>?>(it, grammars),
			IAsyncContextConstraint<IEnumerable<TItem>?>
		where TMember : TMatch
	{
		private readonly List<TMember> _duplicates = [];

		public async Task<ConstraintResult> IsMetBy(IEnumerable<TItem>? actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			Actual = actual;
			if (actual is null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			IEnumerable<TItem> materialized = context
				.UseMaterializedEnumerable<TItem, IEnumerable<TItem>>(actual);
			List<TMember> checkedItems = new();

			IOptionsEquality<TMatch> o = options;
			foreach (TItem item in materialized)
			{
				TMember itemMember = memberAccessor(item);
				if (await checkedItems.AnyButNotInDuplicatesAsync(_duplicates,
					    compareWith => o.AreConsideredEqual(itemMember, compareWith)))
				{
					_duplicates.Add(itemMember);
				}

				checkedItems.Add(itemMember);
			}

			Outcome = _duplicates.Any() ? Outcome.Failure : Outcome.Success;
			expectationBuilder.AddCollectionContext(materialized);
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("only has unique items for ", "only have unique items for "))
				.Append(memberAccessorExpression);
			stringBuilder.Append(options);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(CollectionHelpers.CreateDuplicateFailureMessage(It, _duplicates));

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("has duplicate items for ", "have duplicate items for "))
				.Append(memberAccessorExpression);
			stringBuilder.Append(options);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("all were unique");
	}

	private sealed class AreAllUniqueForEnumerableConstraint<TEnumerable, TMatch>(
		ExpectationBuilder expectationBuilder,
		string it,
		ExpectationGrammars grammars,
		IOptionsEquality<TMatch> options)
		: ConstraintResult.WithNotNullValue<TEnumerable?>(it, grammars),
			IAsyncContextConstraint<TEnumerable?>
		where TEnumerable : IEnumerable?
	{
		private readonly List<object?> _duplicates = [];

		public async Task<ConstraintResult> IsMetBy(TEnumerable? actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			Actual = actual;
			if (actual is null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			IEnumerable materialized = context.UseMaterializedEnumerable(actual);
			List<object?> checkedItems = new();

			IOptionsEquality<TMatch> o = options;
			foreach (object? item in materialized)
			{
				if (item is TMatch matchedItem &&
				    await checkedItems.AnyButNotInDuplicatesAsync(_duplicates,
					    compareWith => o.AreConsideredEqual(matchedItem, compareWith)))
				{
					_duplicates.Add(item);
				}

				checkedItems.Add(item);
			}

			Outcome = _duplicates.Any() ? Outcome.Failure : Outcome.Success;
			expectationBuilder.AddCollectionContext(materialized);
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("only has unique items", "only have unique items"));
			stringBuilder.Append(options);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(CollectionHelpers.CreateDuplicateFailureMessage(It, _duplicates));

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("has duplicate items", "have duplicate items"));
			stringBuilder.Append(options);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("all were unique");
	}

	private sealed class AreAllUniqueWithPredicateForEnumerableConstraint<TEnumerable, TMember, TMatch>(
		ExpectationBuilder expectationBuilder,
		string it,
		ExpectationGrammars grammars,
		Func<object?, TMember> memberAccessor,
		string memberAccessorExpression,
		IOptionsEquality<TMatch> options)
		: ConstraintResult.WithNotNullValue<TEnumerable?>(it, grammars),
			IAsyncContextConstraint<TEnumerable?>
		where TEnumerable : IEnumerable?
		where TMember : TMatch
	{
		private readonly List<TMember> _duplicates = [];

		public async Task<ConstraintResult> IsMetBy(TEnumerable? actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			Actual = actual;
			if (actual is null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			IEnumerable materialized = context.UseMaterializedEnumerable(actual);
			List<TMember> checkedItems = new();

			IOptionsEquality<TMatch> o = options;
			foreach (object? item in materialized)
			{
				TMember itemMember = memberAccessor(item);
				if (await checkedItems.AnyButNotInDuplicatesAsync(_duplicates,
					    compareWith => o.AreConsideredEqual(itemMember, compareWith)))
				{
					_duplicates.Add(itemMember);
				}

				checkedItems.Add(itemMember);
			}

			Outcome = _duplicates.Any() ? Outcome.Failure : Outcome.Success;
			expectationBuilder.AddCollectionContext(materialized);
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("only has unique items for ", "only have unique items for "))
				.Append(memberAccessorExpression);
			stringBuilder.Append(options);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(CollectionHelpers.CreateDuplicateFailureMessage(It, _duplicates));

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("has duplicate items for ", "have duplicate items for "))
				.Append(memberAccessorExpression);
			stringBuilder.Append(options);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("all were unique");
	}
}
