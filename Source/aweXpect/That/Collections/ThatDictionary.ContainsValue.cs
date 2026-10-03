using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;
using aweXpect.SourceGenerators;

namespace aweXpect;

public static partial class ThatDictionary
{
	private const string ContainsValueSummary =
		"Verifies that the dictionary contains the <paramref name=\"expected\" /> value.";

	private const string DoesNotContainValueSummary =
		"Verifies that the dictionary does not contain the <paramref name=\"unexpected\" /> value.";

	[CreateExpectationFamily("ContainsValue", NegatedName = "DoesNotContainValue", PerSubject = true,
		GuaranteesNotNull = true, Summary = ContainsValueSummary, NegatedSummary = DoesNotContainValueSummary)]
	internal static ObjectEqualityResult<TCollection, IThat<TCollection?>, TValue>
		ContainsValueCore<TCollection, TKey, TValue>(
			IThat<TCollection?> subject,
			TValue expected,
			bool negated)
		where TCollection : IEnumerable<KeyValuePair<TKey, TValue>>
	{
		ObjectEqualityOptions<TValue> options = ObjectEqualityWithToleranceOptionsFactory.ForValuesOf<TValue>();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectEqualityResult<TCollection, IThat<TCollection?>, TValue>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new ContainValueConstraint<TCollection, TKey, TValue>(expectationBuilder, it, grammars,
					expected, options).InvertIf(negated)),
			subject,
			options);
	}

	[CreateExpectationFamily("ContainsValue", NegatedName = "DoesNotContainValue", PerSubject = true,
		GuaranteesNotNull = true, Summary = ContainsValueSummary, NegatedSummary = DoesNotContainValueSummary)]
	internal static StringEqualityResult<TCollection, IThat<TCollection?>>
		ContainsValueForStringsCore<TCollection, TKey>(
			IThat<TCollection?> subject,
			string? expected,
			bool negated)
		where TCollection : IEnumerable<KeyValuePair<TKey, string?>>
	{
		StringEqualityOptions options = new(negated ? "unexpected" : "expected");
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new StringEqualityResult<TCollection, IThat<TCollection?>>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new ContainValueConstraint<TCollection, TKey, string?>(expectationBuilder, it, grammars,
					expected, options).InvertIf(negated)),
			subject,
			options);
	}

	private sealed class ContainValueConstraint<TDictionary, TKey, TValue>(
		ExpectationBuilder expectationBuilder,
		string it,
		ExpectationGrammars grammars,
		TValue expected,
		IOptionsEquality<TValue> options)
		: ConstraintResult.WithNotNullValue<TDictionary?>(it, grammars),
			IAsyncConstraint<TDictionary?>
		where TDictionary : IEnumerable<KeyValuePair<TKey, TValue>>
	{
		public async Task<ConstraintResult> IsMetBy(TDictionary? actual, CancellationToken cancellationToken)
		{
			Actual = actual;
			Outcome = actual is not null && await ContainsValue(actual, expected, options)
				? Outcome.Success
				: Outcome.Failure;
			AddDictionaryContext(expectationBuilder, actual);
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("contains value ", "contain value "));
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(options);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" did not contain value ");
			Formatter.Format(stringBuilder, expected);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("does not contain value ", "do not contain value "));
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(options);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" did");
	}
}
