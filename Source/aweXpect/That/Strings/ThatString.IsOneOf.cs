using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Core.Extending;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;
using aweXpect.SourceGenerators;

namespace aweXpect;

public static partial class ThatString
{
	private const string IsOneOfSummary =
		"Verifies that the subject is one of the <paramref name=\"expected\" /> values.";

	private const string IsNotOneOfSummary =
		"Verifies that the subject is not one of the <paramref name=\"unexpected\" /> values.";

	[CreateExpectationFamily("Is{Not}OneOf", Summary = IsOneOfSummary, NegatedSummary = IsNotOneOfSummary)]
	[CreateExpectationFamily("Is{Not}OneOf", Params = true,
		Summary = IsOneOfSummary, NegatedSummary = IsNotOneOfSummary)]
	internal static StringEqualityTypeResult<string?, IThat<string?>> IsOneOfCore(
		IThat<string?> subject,
		IEnumerable<string?> expected,
		string? expectedExpression,
		bool negated)
	{
		IEnumerable<string?> expectedValues = expected.ToNonEmptyValues(negated);
		return new StringEqualityTypeResult<string?, IThat<string?>>(
			subject.Get().ExpectationBuilder.AddConstraint(
				(ExpectedValues: expectedValues, ExpectedExpression: expectedExpression, Negated: negated),
				static (state, it, grammars)
					=> new IsOneOfConstraint(it, grammars, state.ExpectedValues, state.ExpectedExpression,
							state.Negated ? "unexpected" : "expected")
						.InvertIf(state.Negated),
				out IsOneOfConstraint constraint),
			subject,
			constraint);
	}

	private sealed class IsOneOfConstraint(
		string it,
		ExpectationGrammars grammars,
		IEnumerable<string?> expectedValues,
		string? expectedExpression,
		string parameterName)
		: ConstraintResult.WithValue<string?>(it, grammars),
			IAsyncContextConstraint<string?>,
			IOptionsHolderSource<StringEqualityOptions>
	{
		private OptionsHolder<StringEqualityOptions>? _optionsHolder;

		private StringEqualityOptions CurrentOptions
			=> _optionsHolder is null ? StringEqualityOptions.Default : _optionsHolder.Value;

		/// <inheritdoc cref="IOptionsHolderSource{TOptions}.Holder" />
		OptionsHolder<StringEqualityOptions> IOptionsHolderSource<StringEqualityOptions>.Holder
			=> _optionsHolder ??= new OptionsHolder<StringEqualityOptions>(StringEqualityOptions.Default);

		private StringEqualityOptions? _evaluationOptions;

		/// <remarks>
		///     A match type that inspects the content of the subject, e.g. a prefix or a pattern, cannot answer for a
		///     <see langword="null" /> subject, because it has no content.
		///     <para />
		///     Such a match type validates its pattern while it is compared, so a collection of patterns is compared
		///     completely, which rejects an unusable pattern whichever value the subject matches. Any other sequence is
		///     only enumerated up to the first match, as it may be infinite.
		/// </remarks>
		public async ValueTask<ConstraintResult> IsMetBy(string? actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			Actual = actual;
			StringEqualityOptions stringEqualityOptions = CurrentOptions.ForEvaluation(context, cancellationToken);
			_evaluationOptions = stringEqualityOptions;
			bool comparesAllValues = CurrentOptions.InspectsSubject && expectedValues is ICollection<string?>;
			bool isOneOf = false;
			foreach (string? value in expectedValues)
			{
				if (await stringEqualityOptions
					    .AreConsideredEqual(actual, value, parameterName))
				{
					isOneOf = true;
					if (!comparesAllValues)
					{
						break;
					}
				}
			}

			if (actual is null && CurrentOptions.InspectsSubject)
			{
				Outcome = Outcome.FailureBothWays;
			}
			else
			{
				Outcome = isOneOf ? Outcome.Success : Outcome.Failure;
			}

			return this;
		}

		/// <inheritdoc />
		public override void AppendContexts(ResultContextCollector contexts)
		{
			contexts.AddExpectedValuesContext(expectedExpression, expectedValues, Grammars);
			(_evaluationOptions ?? CurrentOptions).AppendContexts(contexts);
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is one of ", "are one of "));
			stringBuilder.Append(expectedExpression?.TrimCommonWhiteSpace() ?? Formatter.Format(expectedValues));
			stringBuilder.Append(CurrentOptions);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is not one of ", "are not one of "));
			stringBuilder.Append(expectedExpression?.TrimCommonWhiteSpace() ?? Formatter.Format(expectedValues));
			stringBuilder.Append(CurrentOptions);
		}
	}
}
