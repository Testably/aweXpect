using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Results;
using aweXpect.SourceGenerators;

namespace aweXpect;

public static partial class ThatDictionary
{
	[CreateExpectationFamily("ContainsKey", NegatedName = "DoesNotContainKey", PerSubject = true,
		GuaranteesNotNull = true, NegatedReturnType = NegatedKeyReturnType,
		Summary = "Verifies that the dictionary contains the <paramref name=\"expected\" /> key.",
		NegatedSummary = "Verifies that the dictionary does not contain the <paramref name=\"unexpected\" /> key.")]
	internal static ContainsKeyResult<TCollection, IThat<TCollection?>, TKey, TValue?>
		ContainsKeyCore<TCollection, TKey, TValue>(
			IThat<TCollection?> subject,
			TKey expected,
			bool negated)
		where TCollection : IEnumerable<KeyValuePair<TKey, TValue>>
	{
		expected.ThrowIfNull(negated);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ContainsKeyResult<TCollection, IThat<TCollection?>, TKey, TValue?>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new ContainsKeyConstraint<TCollection, TKey, TValue>(expectationBuilder, it, grammars, expected)
					.InvertIf(negated)),
			subject,
			expected,
			f => GetLookup(f)(expected, out TValue? value) ? value : default
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
			Outcome = actual is not null && UserCode.Invoke(() => ContainsKey(actual, expected))
				? Outcome.Success
				: Outcome.Failure;
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
