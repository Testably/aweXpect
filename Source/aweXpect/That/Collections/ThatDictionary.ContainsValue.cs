using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Results;
using aweXpect.SourceGenerators;

namespace aweXpect;

public static partial class ThatDictionary
{
	[CreateCollectionExpectation("ContainsValue", NegatedName = "DoesNotContainValue", PerSubject = true,
		GuaranteesNotNull = true,
		Summary = "Verifies that the dictionary contains the <paramref name=\"expected\" /> value.",
		NegatedSummary = "Verifies that the dictionary does not contain the <paramref name=\"unexpected\" /> value.")]
	internal static AndOrResult<TCollection, IThat<TCollection?>>
		ContainsValueCore<TCollection, TKey, TValue>(
			IThat<TCollection?> subject,
			TValue expected,
			bool negated)
		where TCollection : IEnumerable<KeyValuePair<TKey, TValue>>
	{
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new AndOrResult<TCollection, IThat<TCollection?>>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new ContainValueConstraint<TCollection, TKey, TValue>(expectationBuilder, it, grammars,
					expected).InvertIf(negated)),
			subject
		);
	}

	private sealed class ContainValueConstraint<TDictionary, TKey, TValue>(
		ExpectationBuilder expectationBuilder,
		string it,
		ExpectationGrammars grammars,
		TValue expected)
		: ConstraintResult.WithNotNullValue<TDictionary?>(it, grammars),
			IAsyncConstraint<TDictionary?>
		where TDictionary : IEnumerable<KeyValuePair<TKey, TValue>>
	{
		public async Task<ConstraintResult> IsMetBy(TDictionary? actual, CancellationToken cancellationToken)
		{
			Actual = actual;
			Outcome = actual is not null && await ContainsValue(actual, expected) ? Outcome.Success : Outcome.Failure;
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
