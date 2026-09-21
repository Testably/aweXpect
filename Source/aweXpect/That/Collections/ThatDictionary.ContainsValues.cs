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
	///     Verifies that the dictionary contains all <paramref name="expected" /> values.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IDictionary<TKey, TValue>, IThat<IDictionary<TKey, TValue>?>> ContainsValues<TKey,
		TValue>(
		this IThat<IDictionary<TKey, TValue>?> subject,
		params TValue[] expected)
	{
		expected.ThrowIfNullOrEmpty();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new AndOrResult<IDictionary<TKey, TValue>, IThat<IDictionary<TKey, TValue>?>>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new ContainValuesConstraint<IDictionary<TKey, TValue>, TKey, TValue>(expectationBuilder, it, grammars,
					expected)),
			subject
		);
	}

	/// <summary>
	///     Verifies that the dictionary contains all <paramref name="expected" /> values.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<Dictionary<TKey, TValue>, IThat<Dictionary<TKey, TValue>?>> ContainsValues<TKey,
		TValue>(
		this IThat<Dictionary<TKey, TValue>?> subject,
		params TValue[] expected)
		where TKey : notnull
	{
		expected.ThrowIfNullOrEmpty();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new AndOrResult<Dictionary<TKey, TValue>, IThat<Dictionary<TKey, TValue>?>>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new ContainValuesConstraint<IDictionary<TKey, TValue>, TKey, TValue>(expectationBuilder, it, grammars,
					expected)),
			subject
		);
	}

	/// <summary>
	///     Verifies that the dictionary contains all <paramref name="expected" /> values.
	/// </summary>
	/// <remarks>
	///     Most dictionaries implement both dictionary interfaces, so the two overloads must share a declaring type for the
	///     priority to decide between them.
	/// </remarks>
	[OverloadResolutionPriority(-1)]
	[GuaranteesNotNull]
	public static AndOrResult<IReadOnlyDictionary<TKey, TValue>, IThat<IReadOnlyDictionary<TKey, TValue>?>>
		ContainsValues<TKey,
			TValue>(
			this IThat<IReadOnlyDictionary<TKey, TValue>?> subject,
			params TValue[] expected)
	{
		expected.ThrowIfNullOrEmpty();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new AndOrResult<IReadOnlyDictionary<TKey, TValue>, IThat<IReadOnlyDictionary<TKey, TValue>?>>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new ContainValuesConstraint<IReadOnlyDictionary<TKey, TValue>, TKey, TValue>(expectationBuilder, it,
					grammars, expected)),
			subject
		);
	}

	/// <summary>
	///     Verifies that the dictionary contains all <paramref name="expected" /> values.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<ReadOnlyDictionary<TKey, TValue>, IThat<ReadOnlyDictionary<TKey, TValue>?>>
		ContainsValues<TKey,
			TValue>(
			this IThat<ReadOnlyDictionary<TKey, TValue>?> subject,
			params TValue[] expected)
		where TKey : notnull
	{
		expected.ThrowIfNullOrEmpty();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new AndOrResult<ReadOnlyDictionary<TKey, TValue>, IThat<ReadOnlyDictionary<TKey, TValue>?>>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new ContainValuesConstraint<IReadOnlyDictionary<TKey, TValue>, TKey, TValue>(expectationBuilder, it,
					grammars, expected)),
			subject
		);
	}

	/// <summary>
	///     Verifies that the dictionary contains none of the <paramref name="unexpected" /> values.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IDictionary<TKey, TValue>, IThat<IDictionary<TKey, TValue>?>>
		DoesNotContainValues<TKey,
			TValue>(
			this IThat<IDictionary<TKey, TValue>?> subject,
			params TValue[] unexpected)
	{
		unexpected.ThrowIfNullOrEmpty();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new AndOrResult<IDictionary<TKey, TValue>, IThat<IDictionary<TKey, TValue>?>>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new ContainValuesConstraint<IDictionary<TKey, TValue>, TKey, TValue>(expectationBuilder, it, grammars,
					unexpected).Invert()),
			subject
		);
	}

	/// <summary>
	///     Verifies that the dictionary contains none of the <paramref name="unexpected" /> values.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<Dictionary<TKey, TValue>, IThat<Dictionary<TKey, TValue>?>>
		DoesNotContainValues<TKey,
			TValue>(
			this IThat<Dictionary<TKey, TValue>?> subject,
			params TValue[] unexpected)
		where TKey : notnull
	{
		unexpected.ThrowIfNullOrEmpty();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new AndOrResult<Dictionary<TKey, TValue>, IThat<Dictionary<TKey, TValue>?>>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new ContainValuesConstraint<IDictionary<TKey, TValue>, TKey, TValue>(expectationBuilder, it, grammars,
					unexpected).Invert()),
			subject
		);
	}

	/// <summary>
	///     Verifies that the dictionary contains none of the <paramref name="unexpected" /> values.
	/// </summary>
	/// <remarks>
	///     Most dictionaries implement both dictionary interfaces, so the two overloads must share a declaring type for the
	///     priority to decide between them.
	/// </remarks>
	[OverloadResolutionPriority(-1)]
	[GuaranteesNotNull]
	public static AndOrResult<IReadOnlyDictionary<TKey, TValue>, IThat<IReadOnlyDictionary<TKey, TValue>?>>
		DoesNotContainValues<TKey,
			TValue>(
			this IThat<IReadOnlyDictionary<TKey, TValue>?> subject,
			params TValue[] unexpected)
	{
		unexpected.ThrowIfNullOrEmpty();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new AndOrResult<IReadOnlyDictionary<TKey, TValue>, IThat<IReadOnlyDictionary<TKey, TValue>?>>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new ContainValuesConstraint<IReadOnlyDictionary<TKey, TValue>, TKey, TValue>(expectationBuilder, it,
					grammars, unexpected).Invert()),
			subject
		);
	}

	/// <summary>
	///     Verifies that the dictionary contains none of the <paramref name="unexpected" /> values.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<ReadOnlyDictionary<TKey, TValue>, IThat<ReadOnlyDictionary<TKey, TValue>?>>
		DoesNotContainValues<TKey,
			TValue>(
			this IThat<ReadOnlyDictionary<TKey, TValue>?> subject,
			params TValue[] unexpected)
		where TKey : notnull
	{
		unexpected.ThrowIfNullOrEmpty();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new AndOrResult<ReadOnlyDictionary<TKey, TValue>, IThat<ReadOnlyDictionary<TKey, TValue>?>>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new ContainValuesConstraint<IReadOnlyDictionary<TKey, TValue>, TKey, TValue>(expectationBuilder, it,
					grammars, unexpected).Invert()),
			subject
		);
	}

	private sealed class ContainValuesConstraint<TDictionary, TKey, TValue>(
		ExpectationBuilder expectationBuilder,
		string it,
		ExpectationGrammars grammars,
		TValue[] expected)
		: ConstraintResult.WithNotNullValue<TDictionary?>(it, grammars),
			IValueConstraint<TDictionary?>
		where TDictionary : IEnumerable<KeyValuePair<TKey, TValue>>
	{
		private List<TValue>? _existingValues;
		private List<TValue>? _missingValues;

		public ConstraintResult IsMetBy(TDictionary? actual)
		{
			Actual = actual;
			if (actual is not null)
			{
				_missingValues = [];
				_existingValues = [];
				foreach (TValue item in expected)
				{
					if (actual.ContainsValue(item))
					{
						_existingValues.Add(item);
					}
					else
					{
						_missingValues.Add(item);
					}
				}
			}

			Outcome = (IsNegated, _missingKeys: _missingValues, _existingKeys: _existingValues) switch
			{
				(true, _, []) => Outcome.Failure,
				(true, _, _) => Outcome.Success,
				(false, [], _) => Outcome.Success,
				(false, _, _) => Outcome.Failure,
			};
			AddDictionaryContext(expectationBuilder, actual);
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("contains values ", "contain values "));
			Formatter.Format(stringBuilder, expected);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" did not contain ");
			Formatter.Format(stringBuilder, _missingValues, FormattingOptions.MultipleLines);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("does not contain values ", "do not contain values "));
			Formatter.Format(stringBuilder, expected);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" did contain ");
			Formatter.Format(stringBuilder, _existingValues, FormattingOptions.MultipleLines);
		}
	}
}
