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

namespace aweXpect;

public static partial class ThatDictionary
{
	/// <summary>
	///     Verifies that the dictionary only contains unique values.
	/// </summary>
	/// <remarks>
	///     This expectation completely ignores the dictionary keys, as they are unique by design.
	/// </remarks>
	public static ObjectEqualityResult<IDictionary<TKey, TValue>, IThat<IDictionary<TKey, TValue>?>, TValue>
		AreAllUnique<TKey, TValue>(
			this IThat<IDictionary<TKey, TValue>?> source)
	{
		ObjectEqualityOptions<TValue> options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new ObjectEqualityResult<IDictionary<TKey, TValue>, IThat<IDictionary<TKey, TValue>?>, TValue>(
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
	public static StringEqualityResult<IDictionary<TKey, string?>, IThat<IDictionary<TKey, string?>?>>
		AreAllUnique<TKey>(this IThat<IDictionary<TKey, string?>?> source)
	{
		StringEqualityOptions options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new StringEqualityResult<IDictionary<TKey, string?>, IThat<IDictionary<TKey, string?>?>>(
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
	public static ObjectEqualityResult<IDictionary<TKey, TValue>, IThat<IDictionary<TKey, TValue>?>, TMember>
		AreAllUnique<TKey,
			TValue, TMember>(
			this IThat<IDictionary<TKey, TValue>?> source,
			Func<TValue, TMember> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
	{
		ObjectEqualityOptions<TMember> options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new ObjectEqualityResult<IDictionary<TKey, TValue>, IThat<IDictionary<TKey, TValue>?>, TMember>(
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
	public static StringEqualityResult<IDictionary<TKey, TValue>, IThat<IDictionary<TKey, TValue>?>>
		AreAllUnique<TKey,
			TValue>(
			this IThat<IDictionary<TKey, TValue>?> source,
			Func<TValue, string> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
	{
		StringEqualityOptions options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new StringEqualityResult<IDictionary<TKey, TValue>, IThat<IDictionary<TKey, TValue>?>>(
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
	public static ObjectEqualityResult<Dictionary<TKey, TValue>, IThat<Dictionary<TKey, TValue>?>, TValue>
		AreAllUnique<TKey, TValue>(
			this IThat<Dictionary<TKey, TValue>?> source)
		where TKey : notnull
	{
		ObjectEqualityOptions<TValue> options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new ObjectEqualityResult<Dictionary<TKey, TValue>, IThat<Dictionary<TKey, TValue>?>, TValue>(
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
	public static StringEqualityResult<Dictionary<TKey, string?>, IThat<Dictionary<TKey, string?>?>>
		AreAllUnique<TKey>(this IThat<Dictionary<TKey, string?>?> source)
		where TKey : notnull
	{
		StringEqualityOptions options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new StringEqualityResult<Dictionary<TKey, string?>, IThat<Dictionary<TKey, string?>?>>(
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
	public static ObjectEqualityResult<Dictionary<TKey, TValue>, IThat<Dictionary<TKey, TValue>?>, TMember>
		AreAllUnique<TKey,
			TValue, TMember>(
			this IThat<Dictionary<TKey, TValue>?> source,
			Func<TValue, TMember> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
		where TKey : notnull
	{
		ObjectEqualityOptions<TMember> options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new ObjectEqualityResult<Dictionary<TKey, TValue>, IThat<Dictionary<TKey, TValue>?>, TMember>(
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
	public static StringEqualityResult<Dictionary<TKey, TValue>, IThat<Dictionary<TKey, TValue>?>>
		AreAllUnique<TKey,
			TValue>(
			this IThat<Dictionary<TKey, TValue>?> source,
			Func<TValue, string> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
		where TKey : notnull
	{
		StringEqualityOptions options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new StringEqualityResult<Dictionary<TKey, TValue>, IThat<Dictionary<TKey, TValue>?>>(
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
		: ConstraintResult.WithNotNullValue<IDictionary<TKey, TValue>?>(it, grammars),
			IAsyncContextConstraint<IDictionary<TKey, TValue>?>
		where TValue : TMatch
	{
		private readonly List<TValue> _duplicates = [];

		public async Task<ConstraintResult> IsMetBy(IDictionary<TKey, TValue>? actual, IEvaluationContext context,
			CancellationToken cancellationToken)
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
		: ConstraintResult.WithNotNullValue<IDictionary<TKey, TValue>?>(it, grammars),
			IAsyncContextConstraint<IDictionary<TKey, TValue>?>
		where TMember : TMatch
	{
		private readonly List<TMember> _duplicates = [];

		public async Task<ConstraintResult> IsMetBy(IDictionary<TKey, TValue>? actual, IEvaluationContext context,
			CancellationToken cancellationToken)
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
