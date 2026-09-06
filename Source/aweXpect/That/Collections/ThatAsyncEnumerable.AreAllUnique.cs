#if NET8_0_OR_GREATER
using System;
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

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect;

public static partial class ThatAsyncEnumerable
{
	/// <summary>
	///     Verifies that the collection only contains unique items.
	/// </summary>
	public static ObjectEqualityResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem>
		AreAllUnique<TItem>(
			this IThat<IAsyncEnumerable<TItem>?> source)
	{
		ObjectEqualityOptions<TItem> options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new ObjectEqualityResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new AreAllUniqueConstraint<TItem, TItem>(expectationBuilder, it, grammars, options)),
			source, options
		);
	}

	/// <summary>
	///     Verifies that the collection only contains unique items.
	/// </summary>
	public static StringEqualityResult<IAsyncEnumerable<string?>, IThat<IAsyncEnumerable<string?>?>> AreAllUnique(
		this IThat<IAsyncEnumerable<string?>?> source)
	{
		StringEqualityOptions options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new StringEqualityResult<IAsyncEnumerable<string?>, IThat<IAsyncEnumerable<string?>?>>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new AreAllUniqueConstraint<string, string>(expectationBuilder, it, grammars, options)),
			source, options
		);
	}

	/// <summary>
	///     Verifies that the collection only contains items with unique members specified by the
	///     <paramref name="memberAccessor" />.
	/// </summary>
	public static ObjectEqualityResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TMember>
		AreAllUnique<TItem, TMember>(
			this IThat<IAsyncEnumerable<TItem>?> source,
			Func<TItem, TMember> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
	{
		ObjectEqualityOptions<TMember> options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new ObjectEqualityResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TMember>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new AreAllUniqueWithPredicateConstraint<TItem, TMember, TMember>(expectationBuilder, it, grammars,
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
	public static StringEqualityResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>>
		AreAllUnique<TItem>(
			this IThat<IAsyncEnumerable<TItem>?> source,
			Func<TItem, string> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
	{
		StringEqualityOptions options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new StringEqualityResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new AreAllUniqueWithPredicateConstraint<TItem, string, string>(expectationBuilder, it, grammars,
					memberAccessor,
					doNotPopulateThisValue.TrimCommonWhiteSpace(),
					options)),
			source, options
		);
	}

	private sealed class AreAllUniqueConstraint<TItem, TMatch>(
		ExpectationBuilder expectationBuilder,
		string it,
		ExpectationGrammars grammars,
		IOptionsEquality<TMatch> options)
		: ConstraintResult.WithNotNullValue<IAsyncEnumerable<TItem>?>(it, grammars),
			IAsyncContextConstraint<IAsyncEnumerable<TItem>?>
		where TItem : TMatch
	{
		private readonly List<TItem> _duplicates = [];

		public async Task<ConstraintResult> IsMetBy(IAsyncEnumerable<TItem>? actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			Actual = actual;
			if (actual is null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			IAsyncEnumerable<TItem> materialized = context
				.UseMaterializedAsyncEnumerable<TItem, IAsyncEnumerable<TItem>>(actual);
			List<TItem> checkedItems = new();
			LimitedCollection<TItem> items = new();

			IOptionsEquality<TMatch> o = options;
			await foreach (TItem item in materialized.WithCancellation(cancellationToken))
			{
				if (await checkedItems.AnyButNotInDuplicatesAsync(_duplicates,
					    compareWith => o.AreConsideredEqual(item, compareWith)))
				{
					_duplicates.Add(item);
				}

				checkedItems.Add(item);
				items.Add(item);
			}

			Outcome = _duplicates.Any() ? Outcome.Failure : Outcome.Success;
			expectationBuilder.AddCollectionContext(items);
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
		: ConstraintResult.WithNotNullValue<IAsyncEnumerable<TItem>?>(it, grammars),
			IAsyncContextConstraint<IAsyncEnumerable<TItem>?>
		where TMember : TMatch
	{
		private readonly List<TMember> _duplicates = [];

		public async Task<ConstraintResult> IsMetBy(IAsyncEnumerable<TItem>? actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			Actual = actual;
			if (actual is null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			IAsyncEnumerable<TItem> materialized = context
				.UseMaterializedAsyncEnumerable<TItem, IAsyncEnumerable<TItem>>(actual);
			List<TMember> checkedItems = new();
			LimitedCollection<TItem> items = new();

			IOptionsEquality<TMatch> o = options;
			await foreach (TItem item in materialized.WithCancellation(cancellationToken))
			{
				TMember itemMember = memberAccessor(item);
				if (await checkedItems.AnyButNotInDuplicatesAsync(_duplicates,
					    compareWith => o.AreConsideredEqual(itemMember, compareWith)))
				{
					_duplicates.Add(itemMember);
				}

				checkedItems.Add(itemMember);
				items.Add(item);
			}

			Outcome = _duplicates.Any() ? Outcome.Failure : Outcome.Success;
			expectationBuilder.AddCollectionContext(items);
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
#endif
