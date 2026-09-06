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
	///     Verifies that the dictionary contains the <paramref name="expected" /> key.
	/// </summary>
	public static ContainsValueResult<IReadOnlyDictionary<TKey, TValue>, IThat<IReadOnlyDictionary<TKey, TValue>?>, TKey
			, TValue?>
		ContainsKey<TKey, TValue>(
			this IThat<IReadOnlyDictionary<TKey, TValue>?> source,
			TKey expected)
	{
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new ContainsValueResult<IReadOnlyDictionary<TKey, TValue>, IThat<IReadOnlyDictionary<TKey, TValue>?>,
			TKey, TValue?>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new ContainsKeyConstraint<TKey, TValue>(expectationBuilder, it, grammars, expected)),
			source,
			expected,
			f => f.TryGetValue(expected, out TValue? value) ? value : default
		);
	}

	/// <summary>
	///     Verifies that the dictionary contains the <paramref name="expected" /> key.
	/// </summary>
	public static ContainsValueResult<ReadOnlyDictionary<TKey, TValue>, IThat<ReadOnlyDictionary<TKey, TValue>?>, TKey,
			TValue?>
		ContainsKey<TKey, TValue>(
			this IThat<ReadOnlyDictionary<TKey, TValue>?> source,
			TKey expected)
		where TKey : notnull
	{
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new ContainsValueResult<ReadOnlyDictionary<TKey, TValue>, IThat<ReadOnlyDictionary<TKey, TValue>?>, TKey,
			TValue?>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new ContainsKeyConstraint<TKey, TValue>(expectationBuilder, it, grammars, expected)),
			source,
			expected,
			f => f.TryGetValue(expected, out TValue? value) ? value : default
		);
	}

	/// <summary>
	///     Verifies that the dictionary does not contain the <paramref name="unexpected" /> key.
	/// </summary>
	public static AndOrResult<IReadOnlyDictionary<TKey, TValue>, IThat<IReadOnlyDictionary<TKey, TValue>?>>
		DoesNotContainKey<TKey, TValue>(
			this IThat<IReadOnlyDictionary<TKey, TValue>?> source,
			TKey unexpected)
	{
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new AndOrResult<IReadOnlyDictionary<TKey, TValue>, IThat<IReadOnlyDictionary<TKey, TValue>?>>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new ContainsKeyConstraint<TKey, TValue>(expectationBuilder, it, grammars, unexpected).Invert()),
			source
		);
	}


	/// <summary>
	///     Verifies that the dictionary does not contain the <paramref name="unexpected" /> key.
	/// </summary>
	public static AndOrResult<ReadOnlyDictionary<TKey, TValue>, IThat<ReadOnlyDictionary<TKey, TValue>?>>
		DoesNotContainKey<TKey, TValue>(
			this IThat<ReadOnlyDictionary<TKey, TValue>?> source,
			TKey unexpected)
		where TKey : notnull
	{
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new AndOrResult<ReadOnlyDictionary<TKey, TValue>, IThat<ReadOnlyDictionary<TKey, TValue>?>>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new ContainsKeyConstraint<TKey, TValue>(expectationBuilder, it, grammars, unexpected).Invert()),
			source
		);
	}

	private sealed class ContainsKeyConstraint<TKey, TValue>(
		ExpectationBuilder expectationBuilder,
		string it,
		ExpectationGrammars grammars,
		TKey expected)
		: ConstraintResult.WithNotNullValue<IReadOnlyDictionary<TKey, TValue>?>(it, grammars),
			IValueConstraint<IReadOnlyDictionary<TKey, TValue>?>
	{
		public ConstraintResult IsMetBy(IReadOnlyDictionary<TKey, TValue>? actual)
		{
			Actual = actual;
			Outcome = actual?.ContainsKey(expected) == true ? Outcome.Success : Outcome.Failure;
			if (Outcome != Outcome.Success)
			{
				expectationBuilder.AddCollectionContext(actual);
			}

			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("contains key ", "contain key "));
			Formatter.Format(stringBuilder, expected);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" contained only ");
			Formatter.Format(stringBuilder, Actual?.Keys, FormattingOptions.MultipleLines);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("does not contain key ", "do not contain key "));
			Formatter.Format(stringBuilder, expected);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" did");
	}
}
