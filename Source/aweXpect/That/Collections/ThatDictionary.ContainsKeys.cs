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
	private const string ContainsKeysSummary =
		"Verifies that the dictionary contains all <paramref name=\"expected\" /> keys.";

	private const string DoesNotContainKeysSummary =
		"Verifies that the dictionary contains none of the <paramref name=\"unexpected\" /> keys.";

	private const string DoesNotContainKeysRemarks =
		"It fails when the dictionary contains any of the keys. This is stricter than negating <c>ContainsKeys</c>\n" +
		"with <c>DoesNotComplyWith</c>, which only fails when the dictionary contains all of them.";

	[CreateCollectionExpectation("ContainsKeys", NegatedName = "DoesNotContainKeys", PerSubject = true,
		GuaranteesNotNull = true, NegatedReturnType = NegatedKeyReturnType,
		Summary = ContainsKeysSummary, NegatedSummary = DoesNotContainKeysSummary,
		NegatedRemarks = DoesNotContainKeysRemarks)]
	[CreateCollectionExpectation("ContainsKeys", NegatedName = "DoesNotContainKeys", PerSubject = true,
		GuaranteesNotNull = true, Params = true, NegatedReturnType = NegatedKeyReturnType,
		Summary = ContainsKeysSummary, NegatedSummary = DoesNotContainKeysSummary,
		NegatedRemarks = DoesNotContainKeysRemarks)]
	internal static ContainsKeysResult<TCollection, IThat<TCollection?>, TKey, TValue?>
		ContainsKeysCore<TCollection, TKey, TValue>(
			IThat<TCollection?> subject,
			IEnumerable<TKey> expected,
			string? expectedExpression,
			bool negated)
		where TCollection : IEnumerable<KeyValuePair<TKey, TValue>>
	{
		TKey[] keys = expected.ToNonEmptyValues(negated).ToArray();
		foreach (TKey key in keys)
		{
			key.ThrowIfNull(negated);
		}

		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ContainsKeysResult<TCollection, IThat<TCollection?>, TKey, TValue?>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new ContainKeysConstraint<TCollection, TKey, TValue>(expectationBuilder, it, grammars,
					expectedExpression?.TrimCommonWhiteSpace() ?? Formatter.Format(keys), keys).InvertIf(negated)),
			subject,
			keys,
			dictionary => new KeyedValues<TKey, TValue?>(keys
				.Where(key => ContainsKey(dictionary, key))
				.Select(key => new KeyValuePair<TKey, TValue?>(key,
					GetLookup(dictionary)(key, out TValue? value) ? value : default)))
		);
	}

	private sealed class ContainKeysConstraint<TDictionary, TKey, TValue>(
		ExpectationBuilder expectationBuilder,
		string it,
		ExpectationGrammars grammars,
		string expectedExpression,
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
					if (UserCode.Invoke(() => ContainsKey(actual, item)))
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
			stringBuilder.Append(Grammars.Verb("contains keys ", "contain keys ")).Append(expectedExpression);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" did not contain ");
			Formatter.Format(stringBuilder, _missingKeys, FormattingOptions.MultipleLines);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("does not contain keys ", "do not contain keys "))
				.Append(expectedExpression);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" contained ");
			Formatter.Format(stringBuilder, _existingKeys, FormattingOptions.MultipleLines);
		}
	}
}
