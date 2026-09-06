using System.Collections.Generic;
using System.Linq;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatDictionary
{
	/// <summary>
	///     Verifies that the dictionary contains all <paramref name="expected" /> keys.
	/// </summary>
	public static ContainsValuesResult<IDictionary<TKey, TValue>, IThat<IDictionary<TKey, TValue>?>, TKey, TValue?>
		ContainsKeys<TKey, TValue>(
			this IThat<IDictionary<TKey, TValue>?> source,
			params TKey[] expected)
	{
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new ContainsValuesResult<IDictionary<TKey, TValue>, IThat<IDictionary<TKey, TValue>?>, TKey, TValue?>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new ContainKeysConstraint<TKey, TValue>(expectationBuilder, it, grammars, expected)),
			source,
			expected,
			dictionary => expected
				.Select(key => key is not null && dictionary.TryGetValue(key, out TValue? value) ? value : default)
		);
	}

	/// <summary>
	///     Verifies that the dictionary contains all <paramref name="expected" /> keys.
	/// </summary>
	public static ContainsValuesResult<Dictionary<TKey, TValue>, IThat<Dictionary<TKey, TValue>?>, TKey, TValue?>
		ContainsKeys<TKey, TValue>(
			this IThat<Dictionary<TKey, TValue>?> source,
			params TKey[] expected)
		where TKey : notnull
	{
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new ContainsValuesResult<Dictionary<TKey, TValue>, IThat<Dictionary<TKey, TValue>?>, TKey, TValue?>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new ContainKeysConstraint<TKey, TValue>(expectationBuilder, it, grammars, expected)),
			source,
			expected,
			dictionary => expected
				.Select(key => key is not null && dictionary.TryGetValue(key, out TValue? value) ? value : default)
		);
	}

	/// <summary>
	///     Verifies that the dictionary contains none of the <paramref name="unexpected" /> keys.
	/// </summary>
	public static AndOrResult<IDictionary<TKey, TValue>, IThat<IDictionary<TKey, TValue>?>>
		DoesNotContainKeys<TKey, TValue>(
			this IThat<IDictionary<TKey, TValue>?> source,
			params TKey[] unexpected)
	{
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new AndOrResult<IDictionary<TKey, TValue>, IThat<IDictionary<TKey, TValue>?>>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new ContainKeysConstraint<TKey, TValue>(expectationBuilder, it, grammars, unexpected).Invert()),
			source
		);
	}

	/// <summary>
	///     Verifies that the dictionary contains none of the <paramref name="unexpected" /> keys.
	/// </summary>
	public static AndOrResult<Dictionary<TKey, TValue>, IThat<Dictionary<TKey, TValue>?>>
		DoesNotContainKeys<TKey, TValue>(
			this IThat<Dictionary<TKey, TValue>?> source,
			params TKey[] unexpected)
		where TKey : notnull
	{
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new AndOrResult<Dictionary<TKey, TValue>, IThat<Dictionary<TKey, TValue>?>>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new ContainKeysConstraint<TKey, TValue>(expectationBuilder, it, grammars, unexpected).Invert()),
			source
		);
	}

	private sealed class ContainKeysConstraint<TKey, TValue>(
		ExpectationBuilder expectationBuilder,
		string it,
		ExpectationGrammars grammars,
		TKey[] expected)
		: ConstraintResult.WithNotNullValue<IDictionary<TKey, TValue>>(it, grammars),
			IValueConstraint<IDictionary<TKey, TValue>?>
	{
		private List<TKey>? _existingKeys;
		private List<TKey>? _missingKeys;

		public ConstraintResult IsMetBy(IDictionary<TKey, TValue>? actual)
		{
			Actual = actual;
			if (actual != null)
			{
				_missingKeys = [];
				_existingKeys = [];
				foreach (TKey item in expected)
				{
					if (actual.ContainsKey(item))
					{
						_existingKeys.Add(item);
					}
					else
					{
						_missingKeys.Add(item);
					}
				}
			}

			Outcome = (IsNegated, _missingKeys, _existingKeys) switch
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
			stringBuilder.Append(Grammars.Verb("contains keys ", "contain keys "));
			Formatter.Format(stringBuilder, expected);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" did not contain ");
			Formatter.Format(stringBuilder, _missingKeys, FormattingOptions.MultipleLines);
			stringBuilder.Append(" in ");
			Formatter.Format(stringBuilder, Actual!.Keys, FormattingOptions.MultipleLines);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("does not contain keys ", "do not contain keys "));
			Formatter.Format(stringBuilder, expected);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" did contain ");
			Formatter.Format(stringBuilder, _existingKeys, FormattingOptions.MultipleLines);
		}
	}
}
