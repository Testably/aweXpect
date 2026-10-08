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
			expectationBuilder.AddConstraint((Expected: expected, Negated: negated),
				static (state, it, grammars) =>
					new ContainsKeyConstraint<TCollection, TKey, TValue>(it, grammars, state.Expected)
						.InvertIf(state.Negated)),
			subject,
			expected,
			f => TryLookUp(GetLookup(f), expected, out TValue? value) ? value : default
		);
	}

	private sealed class ContainsKeyConstraint<TDictionary, TKey, TValue>(
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
			Outcome = actual is not null && ContainsKey(actual, expected)
				? Outcome.Success
				: Outcome.Failure;
			return this;
		}

		/// <inheritdoc />
		public override void AppendContexts(ResultContextCollector contexts)
			=> AddDictionaryContext(contexts, Actual);

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("contains key ", "contain key "));
			stringBuilder.Append(Formatter.Format(expected).Indent(indentation, false));
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" did not contain key ");
			stringBuilder.Append(Formatter.Format(expected).Indent(indentation, false));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("does not contain key ", "do not contain key "));
			stringBuilder.Append(Formatter.Format(expected).Indent(indentation, false));
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" did");
	}
}
