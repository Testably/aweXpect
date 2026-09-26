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
	[CreateCollectionExpectation("ContainsValues", NegatedName = "DoesNotContainValues", PerSubject = true,
		GuaranteesNotNull = true, Params = true,
		Summary = "Verifies that the dictionary contains all <paramref name=\"expected\" /> values.",
		NegatedSummary = "Verifies that the dictionary contains none of the <paramref name=\"unexpected\" /> values.",
		NegatedRemarks = "It fails when the dictionary contains any of the values. This is stricter than negating\n" +
		                 "<c>ContainsValues</c> with <c>DoesNotComplyWith</c>, which only fails when the dictionary\n" +
		                 "contains all of them.")]
	internal static AndOrResult<TCollection, IThat<TCollection?>>
		ContainsValuesCore<TCollection, TKey, TValue>(
			IThat<TCollection?> subject,
			TValue[] expected,
			bool negated)
		where TCollection : IEnumerable<KeyValuePair<TKey, TValue>>
	{
		expected.ThrowIfNullOrEmpty(negated);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new AndOrResult<TCollection, IThat<TCollection?>>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new ContainValuesConstraint<TCollection, TKey, TValue>(expectationBuilder, it, grammars,
					expected).InvertIf(negated)),
			subject
		);
	}

	private sealed class ContainValuesConstraint<TDictionary, TKey, TValue>(
		ExpectationBuilder expectationBuilder,
		string it,
		ExpectationGrammars grammars,
		TValue[] expected)
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
					if (await ContainsValue(actual, item))
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
			stringBuilder.Append(Grammars.Verb("contains values ", "contain values "));
			Formatter.Format(stringBuilder, expected);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" did not contain ");
			Formatter.Format(stringBuilder, _missingValues, FormattingOptions.MultipleLines);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("does not contain values ", "do not contain values "));
			Formatter.Format(stringBuilder, expected);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" contained ");
			Formatter.Format(stringBuilder, _existingValues, FormattingOptions.MultipleLines);
		}
	}
}
