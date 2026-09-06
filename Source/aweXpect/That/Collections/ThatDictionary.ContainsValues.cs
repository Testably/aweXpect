using System.Collections.Generic;
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
	public static AndOrResult<IDictionary<TKey, TValue>, IThat<IDictionary<TKey, TValue>?>> ContainsValues<TKey,
		TValue>(
		this IThat<IDictionary<TKey, TValue>?> source,
		params TValue[] expected)
	{
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new AndOrResult<IDictionary<TKey, TValue>, IThat<IDictionary<TKey, TValue>?>>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new ContainValuesConstraint<TKey, TValue>(expectationBuilder, it, grammars, expected)),
			source
		);
	}

	/// <summary>
	///     Verifies that the dictionary contains all <paramref name="expected" /> values.
	/// </summary>
	public static AndOrResult<Dictionary<TKey, TValue>, IThat<Dictionary<TKey, TValue>?>> ContainsValues<TKey,
		TValue>(
		this IThat<Dictionary<TKey, TValue>?> source,
		params TValue[] expected)
		where TKey : notnull
	{
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new AndOrResult<Dictionary<TKey, TValue>, IThat<Dictionary<TKey, TValue>?>>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new ContainValuesConstraint<TKey, TValue>(expectationBuilder, it, grammars, expected)),
			source
		);
	}

	/// <summary>
	///     Verifies that the dictionary contains none of the <paramref name="unexpected" /> values.
	/// </summary>
	public static AndOrResult<IDictionary<TKey, TValue>, IThat<IDictionary<TKey, TValue>?>>
		DoesNotContainValues<TKey,
			TValue>(
			this IThat<IDictionary<TKey, TValue>?> source,
			params TValue[] unexpected)
	{
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new AndOrResult<IDictionary<TKey, TValue>, IThat<IDictionary<TKey, TValue>?>>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new ContainValuesConstraint<TKey, TValue>(expectationBuilder, it, grammars, unexpected).Invert()),
			source
		);
	}

	/// <summary>
	///     Verifies that the dictionary contains none of the <paramref name="unexpected" /> values.
	/// </summary>
	public static AndOrResult<Dictionary<TKey, TValue>, IThat<Dictionary<TKey, TValue>?>>
		DoesNotContainValues<TKey,
			TValue>(
			this IThat<Dictionary<TKey, TValue>?> source,
			params TValue[] unexpected)
		where TKey : notnull
	{
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new AndOrResult<Dictionary<TKey, TValue>, IThat<Dictionary<TKey, TValue>?>>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new ContainValuesConstraint<TKey, TValue>(expectationBuilder, it, grammars, unexpected).Invert()),
			source
		);
	}

	private sealed class ContainValuesConstraint<TKey, TValue>(
		ExpectationBuilder expectationBuilder,
		string it,
		ExpectationGrammars grammars,
		TValue[] expected)
		: ConstraintResult.WithNotNullValue<IDictionary<TKey, TValue>?>(it, grammars),
			IValueConstraint<IDictionary<TKey, TValue>?>
	{
		private List<TValue>? _existingValues;
		private List<TValue>? _missingValues;

		public ConstraintResult IsMetBy(IDictionary<TKey, TValue>? actual)
		{
			Actual = actual;
			if (actual != null)
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
			expectationBuilder.AddCollectionContext(actual);
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
			stringBuilder.Append(" in ");
			Formatter.Format(stringBuilder, Actual!.Values, FormattingOptions.MultipleLines);
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
