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
	///     Verifies that the dictionary contains the <paramref name="expected" /> key.
	/// </summary>
	[GuaranteesNotNull]
	public static ContainsKeyResult<IDictionary<TKey, TValue>, IThat<IDictionary<TKey, TValue>?>, TKey, TValue?>
		ContainsKey<TKey, TValue>(
			this IThat<IDictionary<TKey, TValue>?> subject,
			TKey expected)
	{
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ContainsKeyResult<IDictionary<TKey, TValue>, IThat<IDictionary<TKey, TValue>?>, TKey, TValue?>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new ContainsKeyConstraint<IDictionary<TKey, TValue>, TKey, TValue>(expectationBuilder, it, grammars,
					expected)),
			subject,
			expected,
			f => f.TryGetValue(expected, out TValue? value) ? value : default
		);
	}

	/// <summary>
	///     Verifies that the dictionary contains the <paramref name="expected" /> key.
	/// </summary>
	[GuaranteesNotNull]
	public static ContainsKeyResult<Dictionary<TKey, TValue>, IThat<Dictionary<TKey, TValue>?>, TKey, TValue?>
		ContainsKey<TKey, TValue>(
			this IThat<Dictionary<TKey, TValue>?> subject,
			TKey expected)
		where TKey : notnull
	{
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ContainsKeyResult<Dictionary<TKey, TValue>, IThat<Dictionary<TKey, TValue>?>, TKey, TValue?>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new ContainsKeyConstraint<IDictionary<TKey, TValue>, TKey, TValue>(expectationBuilder, it, grammars,
					expected)),
			subject,
			expected,
			f => f.TryGetValue(expected, out TValue? value) ? value : default
		);
	}

	/// <summary>
	///     Verifies that the dictionary contains the <paramref name="expected" /> key.
	/// </summary>
	/// <remarks>
	///     Most dictionaries implement both dictionary interfaces, so the two overloads must share a declaring type for the
	///     priority to decide between them.
	/// </remarks>
	[OverloadResolutionPriority(-1)]
	[GuaranteesNotNull]
	public static ContainsKeyResult<IReadOnlyDictionary<TKey, TValue>, IThat<IReadOnlyDictionary<TKey, TValue>?>, TKey
			, TValue?>
		ContainsKey<TKey, TValue>(
			this IThat<IReadOnlyDictionary<TKey, TValue>?> subject,
			TKey expected)
	{
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ContainsKeyResult<IReadOnlyDictionary<TKey, TValue>, IThat<IReadOnlyDictionary<TKey, TValue>?>,
			TKey, TValue?>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new ContainsKeyConstraint<IReadOnlyDictionary<TKey, TValue>, TKey, TValue>(expectationBuilder, it,
					grammars, expected)),
			subject,
			expected,
			f => f.TryGetValue(expected, out TValue? value) ? value : default
		);
	}

	/// <summary>
	///     Verifies that the dictionary contains the <paramref name="expected" /> key.
	/// </summary>
	[GuaranteesNotNull]
	public static ContainsKeyResult<ReadOnlyDictionary<TKey, TValue>, IThat<ReadOnlyDictionary<TKey, TValue>?>, TKey,
			TValue?>
		ContainsKey<TKey, TValue>(
			this IThat<ReadOnlyDictionary<TKey, TValue>?> subject,
			TKey expected)
		where TKey : notnull
	{
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ContainsKeyResult<ReadOnlyDictionary<TKey, TValue>, IThat<ReadOnlyDictionary<TKey, TValue>?>, TKey,
			TValue?>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new ContainsKeyConstraint<IReadOnlyDictionary<TKey, TValue>, TKey, TValue>(expectationBuilder, it,
					grammars, expected)),
			subject,
			expected,
			f => f.TryGetValue(expected, out TValue? value) ? value : default
		);
	}

	/// <summary>
	///     Verifies that the dictionary does not contain the <paramref name="unexpected" /> key.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IDictionary<TKey, TValue>, IThat<IDictionary<TKey, TValue>?>>
		DoesNotContainKey<TKey, TValue>(
			this IThat<IDictionary<TKey, TValue>?> subject,
			TKey unexpected)
	{
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new AndOrResult<IDictionary<TKey, TValue>, IThat<IDictionary<TKey, TValue>?>>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new ContainsKeyConstraint<IDictionary<TKey, TValue>, TKey, TValue>(expectationBuilder, it, grammars,
					unexpected).Invert()),
			subject
		);
	}

	/// <summary>
	///     Verifies that the dictionary does not contain the <paramref name="unexpected" /> key.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<Dictionary<TKey, TValue>, IThat<Dictionary<TKey, TValue>?>>
		DoesNotContainKey<TKey, TValue>(
			this IThat<Dictionary<TKey, TValue>?> subject,
			TKey unexpected)
		where TKey : notnull
	{
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new AndOrResult<Dictionary<TKey, TValue>, IThat<Dictionary<TKey, TValue>?>>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new ContainsKeyConstraint<IDictionary<TKey, TValue>, TKey, TValue>(expectationBuilder, it, grammars,
					unexpected).Invert()),
			subject
		);
	}

	/// <summary>
	///     Verifies that the dictionary does not contain the <paramref name="unexpected" /> key.
	/// </summary>
	/// <remarks>
	///     Most dictionaries implement both dictionary interfaces, so the two overloads must share a declaring type for the
	///     priority to decide between them.
	/// </remarks>
	[OverloadResolutionPriority(-1)]
	[GuaranteesNotNull]
	public static AndOrResult<IReadOnlyDictionary<TKey, TValue>, IThat<IReadOnlyDictionary<TKey, TValue>?>>
		DoesNotContainKey<TKey, TValue>(
			this IThat<IReadOnlyDictionary<TKey, TValue>?> subject,
			TKey unexpected)
	{
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new AndOrResult<IReadOnlyDictionary<TKey, TValue>, IThat<IReadOnlyDictionary<TKey, TValue>?>>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new ContainsKeyConstraint<IReadOnlyDictionary<TKey, TValue>, TKey, TValue>(expectationBuilder, it,
					grammars, unexpected).Invert()),
			subject
		);
	}

	/// <summary>
	///     Verifies that the dictionary does not contain the <paramref name="unexpected" /> key.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<ReadOnlyDictionary<TKey, TValue>, IThat<ReadOnlyDictionary<TKey, TValue>?>>
		DoesNotContainKey<TKey, TValue>(
			this IThat<ReadOnlyDictionary<TKey, TValue>?> subject,
			TKey unexpected)
		where TKey : notnull
	{
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new AndOrResult<ReadOnlyDictionary<TKey, TValue>, IThat<ReadOnlyDictionary<TKey, TValue>?>>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new ContainsKeyConstraint<IReadOnlyDictionary<TKey, TValue>, TKey, TValue>(expectationBuilder, it,
					grammars, unexpected).Invert()),
			subject
		);
	}

	private sealed class ContainsKeyConstraint<TDictionary, TKey, TValue>(
		ExpectationBuilder expectationBuilder,
		string it,
		ExpectationGrammars grammars,
		TKey expected)
		: ConstraintResult.WithNotNullValue<TDictionary?>(it, grammars),
			IValueConstraint<TDictionary?>
		where TDictionary : IEnumerable<KeyValuePair<TKey, TValue>>
	{
		public ConstraintResult IsMetBy(TDictionary? actual)
		{
			Actual = actual;
			Outcome = actual is not null && ContainsKey(actual, expected) ? Outcome.Success : Outcome.Failure;
			if (Outcome != Outcome.Success)
			{
				AddDictionaryContext(expectationBuilder, actual);
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
			stringBuilder.Append(It).Append(" did not contain ");
			Formatter.Format(stringBuilder, expected);
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
