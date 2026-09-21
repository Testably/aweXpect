using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatDictionary
{
	/// <summary>
	///     Verifies that the dictionary contains the <paramref name="expected" /> value.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IDictionary<TKey, TValue>, IThat<IDictionary<TKey, TValue>?>> ContainsValue<TKey,
		TValue>(
		this IThat<IDictionary<TKey, TValue>?> subject,
		TValue expected)
	{
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new AndOrResult<IDictionary<TKey, TValue>, IThat<IDictionary<TKey, TValue>?>>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new ContainValueConstraint<IDictionary<TKey, TValue>, TKey, TValue>(expectationBuilder, it, grammars,
					expected)),
			subject
		);
	}

	/// <summary>
	///     Verifies that the dictionary contains the <paramref name="expected" /> value.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<Dictionary<TKey, TValue>, IThat<Dictionary<TKey, TValue>?>> ContainsValue<TKey,
		TValue>(
		this IThat<Dictionary<TKey, TValue>?> subject,
		TValue expected)
		where TKey : notnull
	{
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new AndOrResult<Dictionary<TKey, TValue>, IThat<Dictionary<TKey, TValue>?>>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new ContainValueConstraint<IDictionary<TKey, TValue>, TKey, TValue>(expectationBuilder, it, grammars,
					expected)),
			subject
		);
	}

	/// <summary>
	///     Verifies that the dictionary contains the <paramref name="expected" /> value.
	/// </summary>
	/// <remarks>
	///     Most dictionaries implement both dictionary interfaces, so the two overloads must share a declaring type for the
	///     priority to decide between them.
	/// </remarks>
	[OverloadResolutionPriority(-1)]
	[GuaranteesNotNull]
	public static AndOrResult<IReadOnlyDictionary<TKey, TValue>, IThat<IReadOnlyDictionary<TKey, TValue>?>>
		ContainsValue<TKey,
			TValue>(
			this IThat<IReadOnlyDictionary<TKey, TValue>?> subject,
			TValue expected)
	{
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new AndOrResult<IReadOnlyDictionary<TKey, TValue>, IThat<IReadOnlyDictionary<TKey, TValue>?>>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new ContainValueConstraint<IReadOnlyDictionary<TKey, TValue>, TKey, TValue>(expectationBuilder, it,
					grammars, expected)),
			subject
		);
	}

	/// <summary>
	///     Verifies that the dictionary contains the <paramref name="expected" /> value.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<ReadOnlyDictionary<TKey, TValue>, IThat<ReadOnlyDictionary<TKey, TValue>?>> ContainsValue<
		TKey,
		TValue>(
		this IThat<ReadOnlyDictionary<TKey, TValue>?> subject,
		TValue expected)
		where TKey : notnull
	{
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new AndOrResult<ReadOnlyDictionary<TKey, TValue>, IThat<ReadOnlyDictionary<TKey, TValue>?>>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new ContainValueConstraint<IReadOnlyDictionary<TKey, TValue>, TKey, TValue>(expectationBuilder, it,
					grammars, expected)),
			subject
		);
	}

	/// <summary>
	///     Verifies that the dictionary does not contain the <paramref name="unexpected" /> value.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IDictionary<TKey, TValue>, IThat<IDictionary<TKey, TValue>?>>
		DoesNotContainValue<TKey, TValue>(
			this IThat<IDictionary<TKey, TValue>?> subject,
			TValue unexpected)
	{
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new AndOrResult<IDictionary<TKey, TValue>, IThat<IDictionary<TKey, TValue>?>>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new ContainValueConstraint<IDictionary<TKey, TValue>, TKey, TValue>(expectationBuilder, it, grammars,
					unexpected).Invert()),
			subject
		);
	}

	/// <summary>
	///     Verifies that the dictionary does not contain the <paramref name="unexpected" /> value.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<Dictionary<TKey, TValue>, IThat<Dictionary<TKey, TValue>?>>
		DoesNotContainValue<TKey, TValue>(
			this IThat<Dictionary<TKey, TValue>?> subject,
			TValue unexpected)
		where TKey : notnull
	{
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new AndOrResult<Dictionary<TKey, TValue>, IThat<Dictionary<TKey, TValue>?>>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new ContainValueConstraint<IDictionary<TKey, TValue>, TKey, TValue>(expectationBuilder, it, grammars,
					unexpected).Invert()),
			subject
		);
	}

	/// <summary>
	///     Verifies that the dictionary does not contain the <paramref name="unexpected" /> value.
	/// </summary>
	/// <remarks>
	///     Most dictionaries implement both dictionary interfaces, so the two overloads must share a declaring type for the
	///     priority to decide between them.
	/// </remarks>
	[OverloadResolutionPriority(-1)]
	[GuaranteesNotNull]
	public static AndOrResult<IReadOnlyDictionary<TKey, TValue>, IThat<IReadOnlyDictionary<TKey, TValue>?>>
		DoesNotContainValue<TKey, TValue>(
			this IThat<IReadOnlyDictionary<TKey, TValue>?> subject,
			TValue unexpected)
	{
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new AndOrResult<IReadOnlyDictionary<TKey, TValue>, IThat<IReadOnlyDictionary<TKey, TValue>?>>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new ContainValueConstraint<IReadOnlyDictionary<TKey, TValue>, TKey, TValue>(expectationBuilder, it,
					grammars, unexpected).Invert()),
			subject
		);
	}

	/// <summary>
	///     Verifies that the dictionary does not contain the <paramref name="unexpected" /> value.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<ReadOnlyDictionary<TKey, TValue>, IThat<ReadOnlyDictionary<TKey, TValue>?>>
		DoesNotContainValue<TKey, TValue>(
			this IThat<ReadOnlyDictionary<TKey, TValue>?> subject,
			TValue unexpected)
		where TKey : notnull
	{
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new AndOrResult<ReadOnlyDictionary<TKey, TValue>, IThat<ReadOnlyDictionary<TKey, TValue>?>>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new ContainValueConstraint<IReadOnlyDictionary<TKey, TValue>, TKey, TValue>(expectationBuilder, it,
					grammars, unexpected).Invert()),
			subject
		);
	}

	private sealed class ContainValueConstraint<TDictionary, TKey, TValue>(
		ExpectationBuilder expectationBuilder,
		string it,
		ExpectationGrammars grammars,
		TValue expected)
		: ConstraintResult.WithNotNullValue<TDictionary?>(it, grammars),
			IValueConstraint<TDictionary?>
		where TDictionary : IEnumerable<KeyValuePair<TKey, TValue>>
	{
		public ConstraintResult IsMetBy(TDictionary? actual)
		{
			Actual = actual;
			Outcome = actual is not null && actual.ContainsValue(expected) ? Outcome.Success : Outcome.Failure;
			AddDictionaryContext(expectationBuilder, actual);
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("contains value ", "contain value "));
			Formatter.Format(stringBuilder, expected);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" did not contain ");
			Formatter.Format(stringBuilder, expected);
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
