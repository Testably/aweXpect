using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace aweXpect;

public static partial class ThatReadOnlyDictionary
{
	/// <summary>
	///     Verifies that the dictionary only contains unique values.
	/// </summary>
	/// <remarks>
	///     This expectation completely ignores the dictionary keys, as they are unique by design.
	/// </remarks>
	public static ObjectEqualityResult<IReadOnlyDictionary<TKey, TValue>, IThat<IReadOnlyDictionary<TKey, TValue>?>,
			TValue>
		AreAllUnique<TKey, TValue>(
			this IThat<IReadOnlyDictionary<TKey, TValue>?> source)
	{
		ObjectEqualityOptions<TValue> options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new ObjectEqualityResult<IReadOnlyDictionary<TKey, TValue>, IThat<IReadOnlyDictionary<TKey, TValue>?>,
			TValue>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new AllIsUniqueConstraint<TKey, TValue, TValue>(expectationBuilder, it, grammars, options)),
			source, options
		);
	}

	/// <summary>
	///     Verifies that the dictionary only contains unique values.
	/// </summary>
	/// <remarks>
	///     This expectation completely ignores the dictionary keys, as they are unique by design.
	/// </remarks>
	public static StringEqualityResult<IReadOnlyDictionary<TKey, string?>, IThat<IReadOnlyDictionary<TKey, string?>?>>
		AreAllUnique<TKey>(this IThat<IReadOnlyDictionary<TKey, string?>?> source)
	{
		StringEqualityOptions options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new StringEqualityResult<IReadOnlyDictionary<TKey, string?>, IThat<IReadOnlyDictionary<TKey, string?>?>>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new AllIsUniqueConstraint<TKey, string?, string?>(expectationBuilder, it, grammars, options)),
			source, options
		);
	}

	/// <summary>
	///     Verifies that the dictionary only contains values with unique members specified by the
	///     <paramref name="memberAccessor" />.
	/// </summary>
	/// <remarks>
	///     This expectation completely ignores the dictionary keys, as they are unique by design.
	/// </remarks>
	public static ObjectEqualityResult<IReadOnlyDictionary<TKey, TValue>, IThat<IReadOnlyDictionary<TKey, TValue>?>,
			TMember>
		AreAllUnique<TKey,
			TValue, TMember>(
			this IThat<IReadOnlyDictionary<TKey, TValue>?> source,
			Func<TValue, TMember> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
	{
		ObjectEqualityOptions<TMember> options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new ObjectEqualityResult<IReadOnlyDictionary<TKey, TValue>, IThat<IReadOnlyDictionary<TKey, TValue>?>,
			TMember>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new AllIsUniqueWithPredicateConstraint<TKey, TValue, TMember, TMember>(
					expectationBuilder,
					it, grammars,
					memberAccessor,
					doNotPopulateThisValue.TrimCommonWhiteSpace(),
					options)),
			source, options
		);
	}

	/// <summary>
	///     Verifies that the dictionary only contains values with unique members specified by the
	///     <paramref name="memberAccessor" />.
	/// </summary>
	/// <remarks>
	///     This expectation completely ignores the dictionary keys, as they are unique by design.
	/// </remarks>
	public static StringEqualityResult<IReadOnlyDictionary<TKey, TValue>, IThat<IReadOnlyDictionary<TKey, TValue>?>>
		AreAllUnique<TKey,
			TValue>(
			this IThat<IReadOnlyDictionary<TKey, TValue>?> source,
			Func<TValue, string> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
	{
		StringEqualityOptions options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new StringEqualityResult<IReadOnlyDictionary<TKey, TValue>, IThat<IReadOnlyDictionary<TKey, TValue>?>>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new AllIsUniqueWithPredicateConstraint<TKey, TValue, string, string>(
					expectationBuilder,
					it, grammars,
					memberAccessor,
					doNotPopulateThisValue.TrimCommonWhiteSpace(),
					options)),
			source, options
		);
	}

	/// <summary>
	///     Verifies that the dictionary only contains unique values.
	/// </summary>
	/// <remarks>
	///     This expectation completely ignores the dictionary keys, as they are unique by design.
	/// </remarks>
	public static ObjectEqualityResult<ReadOnlyDictionary<TKey, TValue>, IThat<ReadOnlyDictionary<TKey, TValue>?>,
			TValue>
		AreAllUnique<TKey, TValue>(
			this IThat<ReadOnlyDictionary<TKey, TValue>?> source)
		where TKey : notnull
	{
		ObjectEqualityOptions<TValue> options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new ObjectEqualityResult<ReadOnlyDictionary<TKey, TValue>, IThat<ReadOnlyDictionary<TKey, TValue>?>,
			TValue>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new AllIsUniqueConstraint<TKey, TValue, TValue>(expectationBuilder, it, grammars, options)),
			source, options
		);
	}

	/// <summary>
	///     Verifies that the dictionary only contains unique values.
	/// </summary>
	/// <remarks>
	///     This expectation completely ignores the dictionary keys, as they are unique by design.
	/// </remarks>
	public static StringEqualityResult<ReadOnlyDictionary<TKey, string?>, IThat<ReadOnlyDictionary<TKey, string?>?>>
		AreAllUnique<TKey>(this IThat<ReadOnlyDictionary<TKey, string?>?> source)
		where TKey : notnull
	{
		StringEqualityOptions options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new StringEqualityResult<ReadOnlyDictionary<TKey, string?>, IThat<ReadOnlyDictionary<TKey, string?>?>>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new AllIsUniqueConstraint<TKey, string?, string?>(expectationBuilder, it, grammars, options)),
			source, options
		);
	}

	/// <summary>
	///     Verifies that the dictionary only contains values with unique members specified by the
	///     <paramref name="memberAccessor" />.
	/// </summary>
	/// <remarks>
	///     This expectation completely ignores the dictionary keys, as they are unique by design.
	/// </remarks>
	public static ObjectEqualityResult<ReadOnlyDictionary<TKey, TValue>, IThat<ReadOnlyDictionary<TKey, TValue>?>,
			TMember>
		AreAllUnique<TKey,
			TValue, TMember>(
			this IThat<ReadOnlyDictionary<TKey, TValue>?> source,
			Func<TValue, TMember> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
		where TKey : notnull
	{
		ObjectEqualityOptions<TMember> options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new ObjectEqualityResult<ReadOnlyDictionary<TKey, TValue>, IThat<ReadOnlyDictionary<TKey, TValue>?>,
			TMember>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new AllIsUniqueWithPredicateConstraint<TKey, TValue, TMember, TMember>(
					expectationBuilder,
					it, grammars,
					memberAccessor,
					doNotPopulateThisValue.TrimCommonWhiteSpace(),
					options)),
			source, options
		);
	}

	/// <summary>
	///     Verifies that the dictionary only contains values with unique members specified by the
	///     <paramref name="memberAccessor" />.
	/// </summary>
	/// <remarks>
	///     This expectation completely ignores the dictionary keys, as they are unique by design.
	/// </remarks>
	public static StringEqualityResult<ReadOnlyDictionary<TKey, TValue>, IThat<ReadOnlyDictionary<TKey, TValue>?>>
		AreAllUnique<TKey,
			TValue>(
			this IThat<ReadOnlyDictionary<TKey, TValue>?> source,
			Func<TValue, string> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
		where TKey : notnull
	{
		StringEqualityOptions options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new StringEqualityResult<ReadOnlyDictionary<TKey, TValue>, IThat<ReadOnlyDictionary<TKey, TValue>?>>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new AllIsUniqueWithPredicateConstraint<TKey, TValue, string, string>(
					expectationBuilder,
					it, grammars,
					memberAccessor,
					doNotPopulateThisValue.TrimCommonWhiteSpace(),
					options)),
			source, options
		);
	}

	private sealed class AllIsUniqueConstraint<TKey, TValue, TMatch>(
		ExpectationBuilder expectationBuilder,
		string it,
		ExpectationGrammars grammars,
		IOptionsEquality<TMatch> options)
		: ConstraintResult.WithNotNullValue<IReadOnlyDictionary<TKey, TValue>?>(it, grammars),
			IAsyncContextConstraint<IReadOnlyDictionary<TKey, TValue>?>
		where TValue : TMatch
	{
		private readonly List<TValue> _duplicates = [];

		public async Task<ConstraintResult> IsMetBy(IReadOnlyDictionary<TKey, TValue>? actual,
			IEvaluationContext context, CancellationToken cancellationToken)
		{
			Actual = actual;
			if (actual is null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			List<TValue> checkedItems = new();

			IOptionsEquality<TMatch> o = options;
			foreach (TValue item in actual.Values)
			{
				if (await checkedItems.AnyButNotInDuplicatesAsync(_duplicates,
					    compareWith => o.AreConsideredEqual(item, compareWith)))
				{
					_duplicates.Add(item);
				}

				checkedItems.Add(item);
			}

			Outcome = _duplicates.Any() ? Outcome.Failure : Outcome.Success;
			expectationBuilder.AddCollectionContext(actual);
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("only has unique values", "only have unique values"));
			stringBuilder.Append(options);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(CollectionHelpers.CreateDuplicateFailureMessage(It, _duplicates));

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("has duplicate values", "have duplicate values"));
			stringBuilder.Append(options);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("all were unique");
	}

	private sealed class AllIsUniqueWithPredicateConstraint<TKey, TValue, TMember, TMatch>(
		ExpectationBuilder expectationBuilder,
		string it,
		ExpectationGrammars grammars,
		Func<TValue, TMember> memberAccessor,
		string memberAccessorExpression,
		IOptionsEquality<TMatch> options)
		: ConstraintResult.WithNotNullValue<IReadOnlyDictionary<TKey, TValue>?>(it, grammars),
			IAsyncContextConstraint<IReadOnlyDictionary<TKey, TValue>?>
		where TMember : TMatch
	{
		private readonly List<TMember> _duplicates = [];

		public async Task<ConstraintResult> IsMetBy(IReadOnlyDictionary<TKey, TValue>? actual,
			IEvaluationContext context, CancellationToken cancellationToken)
		{
			Actual = actual;
			if (actual is null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			List<TMember> checkedItems = new();

			IOptionsEquality<TMatch> o = options;
			foreach (TValue item in actual.Values)
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
			expectationBuilder.AddCollectionContext(actual);
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("only has unique values for ", "only have unique values for "))
				.Append(memberAccessorExpression);
			stringBuilder.Append(options);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(CollectionHelpers.CreateDuplicateFailureMessage(It, _duplicates));

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("has duplicate values for ", "have duplicate values for "))
				.Append(memberAccessorExpression);
			stringBuilder.Append(options);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("all were unique");
	}
}
