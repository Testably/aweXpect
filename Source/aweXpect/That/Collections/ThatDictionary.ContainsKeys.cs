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

	[CreateExpectationFamily("ContainsKeys", NegatedName = "DoesNotContainKeys", PerSubject = true,
		GuaranteesNotNull = true, NegatedReturnType = NegatedKeyReturnType,
		Summary = ContainsKeysSummary, NegatedSummary = DoesNotContainKeysSummary,
		NegatedRemarks = DoesNotContainKeysRemarks)]
	[CreateExpectationFamily("ContainsKeys", NegatedName = "DoesNotContainKeys", PerSubject = true,
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
			expectationBuilder.AddConstraint((ExpectedExpression: expectedExpression, Keys: keys, Negated: negated),
				static (state, it, grammars) =>
					new ContainKeysConstraint<TCollection, TKey, TValue>(it, grammars,
						state.ExpectedExpression?.TrimCommonWhiteSpace() ?? Formatter.Format(state.Keys), state.Keys,
						state.Negated)
						.InvertIf(state.Negated)),
			subject,
			keys,
			dictionary => new KeyedValues<TKey, TValue?>(keys
				.Where(key => ContainsKey(dictionary, key))
				.Select(key => new KeyValuePair<TKey, TValue?>(key,
					TryLookUp(GetLookup(dictionary), key, out TValue? value) ? value : default)))
		);
	}

	/// <remarks>
	///     <paramref name="isAny" /> selects at construction whether the constraint asks for any or for all of the
	///     <paramref name="expected" /> keys, so that a later negation inverts the same question its text describes.
	/// </remarks>
	private sealed class ContainKeysConstraint<TDictionary, TKey, TValue>(
		string it,
		ExpectationGrammars grammars,
		string expectedExpression,
		TKey[] expected,
		bool isAny)
		: ConstraintResult.WithNotNullValue<TDictionary?>(it, grammars),
			IValueConstraint<TDictionary?>
		where TDictionary : IEnumerable<KeyValuePair<TKey, TValue>>
	{
		private List<TKey>? _existingKeys;
		private List<TKey>? _missingKeys;

		/// <inheritdoc />
		public override void AppendContexts(ResultContextCollector contexts)
			=> AddDictionaryContext(contexts, Actual);

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

			Outcome = (isAny, _missingKeys, _existingKeys) switch
			{
				(true, _, []) => Outcome.Failure,
				(true, _, _) => Outcome.Success,
				(false, [], _) => Outcome.Success,
				(false, _, _) => Outcome.Failure,
			};
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(isAny
					? Grammars.Verb("contains any of keys ", "contain any of keys ")
					: Grammars.Verb("contains keys ", "contain keys "))
				.Append(expectedExpression);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" did not contain ");
			Formatter.Format(stringBuilder, _missingKeys, FormattingOptions.MultipleLines);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(isAny
					? Grammars.Verb("does not contain keys ", "do not contain keys ")
					: Grammars.Verb("does not contain all keys ", "do not contain all keys "))
				.Append(expectedExpression);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" contained ");
			Formatter.Format(stringBuilder, _existingKeys, FormattingOptions.MultipleLines);
		}
	}
}
