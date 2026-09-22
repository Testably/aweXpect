using System.Collections.Generic;
using System.Linq;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Results;
using aweXpect.SourceGenerators;

namespace aweXpect;

public static partial class ThatDictionary
{
	[CreateCollectionExpectation("ContainsKeys", PerSubject = true, GuaranteesNotNull = true, Params = true,
		Summary = "Verifies that the dictionary contains all <paramref name=\"expected\" /> keys.")]
	internal static ContainsKeysResult<TCollection, IThat<TCollection?>, TKey, TValue?>
		ContainsKeysCore<TCollection, TKey, TValue>(
			IThat<TCollection?> subject,
			TKey[] expected)
		where TCollection : IEnumerable<KeyValuePair<TKey, TValue>>
	{
		expected.ThrowIfNullOrEmpty();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ContainsKeysResult<TCollection, IThat<TCollection?>, TKey, TValue?>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new ContainKeysConstraint<TCollection, TKey, TValue>(expectationBuilder, it, grammars, expected)),
			subject,
			expected,
			dictionary => new KeyedValues<TKey, TValue?>(expected
				.Where(key => key is not null && ContainsKey(dictionary, key))
				.Select(key => new KeyValuePair<TKey, TValue?>(key,
					GetLookup(dictionary)(key, out TValue? value) ? value : default)))
		);
	}

	[CreateCollectionExpectation("DoesNotContainKeys", PerSubject = true, GuaranteesNotNull = true, Params = true,
		Summary = "Verifies that the dictionary contains none of the <paramref name=\"unexpected\" /> keys.")]
	internal static AndOrResult<TCollection, IThat<TCollection?>>
		DoesNotContainKeysCore<TCollection, TKey, TValue>(
			IThat<TCollection?> subject,
			TKey[] unexpected)
		where TCollection : IEnumerable<KeyValuePair<TKey, TValue>>
	{
		unexpected.ThrowIfNullOrEmpty();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new AndOrResult<TCollection, IThat<TCollection?>>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new ContainKeysConstraint<TCollection, TKey, TValue>(expectationBuilder, it, grammars,
					unexpected).Invert()),
			subject
		);
	}

	private sealed class ContainKeysConstraint<TDictionary, TKey, TValue>(
		ExpectationBuilder expectationBuilder,
		string it,
		ExpectationGrammars grammars,
		TKey[] expected)
		: ConstraintResult.WithNotNullValue<TDictionary?>(it, grammars),
			IValueConstraint<TDictionary?>
		where TDictionary : IEnumerable<KeyValuePair<TKey, TValue>>
	{
		private List<TKey>? _existingKeys;
		private List<TKey>? _missingKeys;

		public ConstraintResult IsMetBy(TDictionary? actual)
		{
			Actual = actual;
			if (actual is not null)
			{
				_missingKeys = [];
				_existingKeys = [];
				foreach (TKey item in expected)
				{
					if (ContainsKey(actual, item))
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
			AddDictionaryContext(expectationBuilder, actual);
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
