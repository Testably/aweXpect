using System.Collections.Generic;
using System.Collections.ObjectModel;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatReadOnlyDictionary
{
	/// <summary>
	///     Verifies that the dictionary contains the <paramref name="expected" /> value.
	/// </summary>
	public static AndOrResult<IReadOnlyDictionary<TKey, TValue>, IThat<IReadOnlyDictionary<TKey, TValue>?>>
		ContainsValue<TKey,
			TValue>(
			this IThat<IReadOnlyDictionary<TKey, TValue>?> source,
			TValue expected)
	{
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new AndOrResult<IReadOnlyDictionary<TKey, TValue>, IThat<IReadOnlyDictionary<TKey, TValue>?>>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new ContainValueConstraint<TKey, TValue>(expectationBuilder, it, grammars, expected)),
			source
		);
	}

	/// <summary>
	///     Verifies that the dictionary contains the <paramref name="expected" /> value.
	/// </summary>
	public static AndOrResult<ReadOnlyDictionary<TKey, TValue>, IThat<ReadOnlyDictionary<TKey, TValue>?>> ContainsValue<
		TKey,
		TValue>(
		this IThat<ReadOnlyDictionary<TKey, TValue>?> source,
		TValue expected)
		where TKey : notnull
	{
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new AndOrResult<ReadOnlyDictionary<TKey, TValue>, IThat<ReadOnlyDictionary<TKey, TValue>?>>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new ContainValueConstraint<TKey, TValue>(expectationBuilder, it, grammars, expected)),
			source
		);
	}

	/// <summary>
	///     Verifies that the dictionary does not contain the <paramref name="unexpected" /> value.
	/// </summary>
	public static AndOrResult<IReadOnlyDictionary<TKey, TValue>, IThat<IReadOnlyDictionary<TKey, TValue>?>>
		DoesNotContainValue<TKey, TValue>(
			this IThat<IReadOnlyDictionary<TKey, TValue>?> source,
			TValue unexpected)
	{
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new AndOrResult<IReadOnlyDictionary<TKey, TValue>, IThat<IReadOnlyDictionary<TKey, TValue>?>>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new ContainValueConstraint<TKey, TValue>(expectationBuilder, it, grammars, unexpected).Invert()),
			source
		);
	}

	/// <summary>
	///     Verifies that the dictionary does not contain the <paramref name="unexpected" /> value.
	/// </summary>
	public static AndOrResult<ReadOnlyDictionary<TKey, TValue>, IThat<ReadOnlyDictionary<TKey, TValue>?>>
		DoesNotContainValue<TKey, TValue>(
			this IThat<ReadOnlyDictionary<TKey, TValue>?> source,
			TValue unexpected)
		where TKey : notnull
	{
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new AndOrResult<ReadOnlyDictionary<TKey, TValue>, IThat<ReadOnlyDictionary<TKey, TValue>?>>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new ContainValueConstraint<TKey, TValue>(expectationBuilder, it, grammars, unexpected).Invert()),
			source
		);
	}

	private sealed class ContainValueConstraint<TKey, TValue>(
		ExpectationBuilder expectationBuilder,
		string it,
		ExpectationGrammars grammars,
		TValue expected)
		: ConstraintResult.WithNotNullValue<IReadOnlyDictionary<TKey, TValue>?>(it, grammars),
			IValueConstraint<IReadOnlyDictionary<TKey, TValue>?>
	{
		public ConstraintResult IsMetBy(IReadOnlyDictionary<TKey, TValue>? actual)
		{
			Actual = actual;
			Outcome = actual?.ContainsValue(expected) == true ? Outcome.Success : Outcome.Failure;
			expectationBuilder.AddCollectionContext(actual);
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("contains value ", "contain value "));
			Formatter.Format(stringBuilder, expected);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" contained only ");
			Formatter.Format(stringBuilder, Actual?.Values, FormattingOptions.MultipleLines);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("does not contain value ", "do not contain value "));
			Formatter.Format(stringBuilder, expected);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" did");
	}
}
