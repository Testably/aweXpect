using System.Collections.Generic;
using System.Linq;
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
	private const string ContainsValuesSummary =
		"Verifies that the dictionary contains all <paramref name=\"expected\" /> values.";

	private const string DoesNotContainValuesSummary =
		"Verifies that the dictionary contains none of the <paramref name=\"unexpected\" /> values.";

	private const string DoesNotContainValuesRemarks =
		"It fails when the dictionary contains any of the values. This is stricter than negating\n" +
		"<c>ContainsValues</c> with <c>DoesNotComplyWith</c>, which only fails when the dictionary\n" +
		"contains all of them.";

	[CreateCollectionExpectation("ContainsValues", NegatedName = "DoesNotContainValues", PerSubject = true,
		GuaranteesNotNull = true, Summary = ContainsValuesSummary, NegatedSummary = DoesNotContainValuesSummary,
		NegatedRemarks = DoesNotContainValuesRemarks)]
	[CreateCollectionExpectation("ContainsValues", NegatedName = "DoesNotContainValues", PerSubject = true,
		GuaranteesNotNull = true, Params = true, Summary = ContainsValuesSummary,
		NegatedSummary = DoesNotContainValuesSummary, NegatedRemarks = DoesNotContainValuesRemarks)]
	internal static ObjectEqualityResult<TCollection, IThat<TCollection?>, TValue>
		ContainsValuesCore<TCollection, TKey, TValue>(
			IThat<TCollection?> subject,
			IEnumerable<TValue> expected,
			string? expectedExpression,
			bool negated)
		where TCollection : IEnumerable<KeyValuePair<TKey, TValue>>
	{
		TValue[] values = expected.ToNonEmptyValues(negated).ToArray();
		ObjectEqualityOptions<TValue> options = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectEqualityResult<TCollection, IThat<TCollection?>, TValue>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new ContainValuesConstraint<TCollection, TKey, TValue>(expectationBuilder, it, grammars,
					expectedExpression?.TrimCommonWhiteSpace() ?? Formatter.Format(values), values, options)
					.InvertIf(negated)),
			subject,
			options);
	}

	[CreateCollectionExpectation("ContainsValues", NegatedName = "DoesNotContainValues", PerSubject = true,
		GuaranteesNotNull = true, Summary = ContainsValuesSummary, NegatedSummary = DoesNotContainValuesSummary,
		NegatedRemarks = DoesNotContainValuesRemarks)]
	[CreateCollectionExpectation("ContainsValues", NegatedName = "DoesNotContainValues", PerSubject = true,
		GuaranteesNotNull = true, Params = true, Summary = ContainsValuesSummary,
		NegatedSummary = DoesNotContainValuesSummary, NegatedRemarks = DoesNotContainValuesRemarks)]
	internal static StringEqualityResult<TCollection, IThat<TCollection?>>
		ContainsValuesForStringsCore<TCollection, TKey>(
			IThat<TCollection?> subject,
			IEnumerable<string?> expected,
			string? expectedExpression,
			bool negated)
		where TCollection : IEnumerable<KeyValuePair<TKey, string?>>
	{
		string?[] values = expected.ToNonEmptyValues(negated).ToArray();
		StringEqualityOptions options = new(negated ? "unexpected" : "expected");
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new StringEqualityResult<TCollection, IThat<TCollection?>>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new ContainValuesConstraint<TCollection, TKey, string?>(expectationBuilder, it, grammars,
					expectedExpression?.TrimCommonWhiteSpace() ?? Formatter.Format(values), values, options)
					.InvertIf(negated)),
			subject,
			options);
	}

	private sealed class ContainValuesConstraint<TDictionary, TKey, TValue>(
		ExpectationBuilder expectationBuilder,
		string it,
		ExpectationGrammars grammars,
		string expectedExpression,
		TValue[] expected,
		IOptionsEquality<TValue> options)
		: ConstraintResult.WithNotNullValue<TDictionary?>(it, grammars),
			IAsyncConstraint<TDictionary?>
		where TDictionary : IEnumerable<KeyValuePair<TKey, TValue>>
	{
		private List<TValue>? _existingValues;
		private List<TValue>? _missingValues;

		public async Task<ConstraintResult> IsMetBy(TDictionary? actual, CancellationToken cancellationToken)
		{
			Actual = actual;
			if (actual is not null)
			{
				_missingValues = [];
				_existingValues = [];
				foreach (TValue item in expected)
				{
					if (await ContainsValue(actual, item, options))
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
			stringBuilder.Append(Grammars.Verb("contains values ", "contain values ")).Append(expectedExpression);
			stringBuilder.Append(options);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" did not contain ");
			Formatter.Format(stringBuilder, _missingValues, FormattingOptions.MultipleLines);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("does not contain values ", "do not contain values "))
				.Append(expectedExpression);
			stringBuilder.Append(options);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" contained ");
			Formatter.Format(stringBuilder, _existingValues, FormattingOptions.MultipleLines);
		}
	}
}
