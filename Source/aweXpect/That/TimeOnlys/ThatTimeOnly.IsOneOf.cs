#if NET8_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Linq;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;
using aweXpect.SourceGenerators;

namespace aweXpect;

public static partial class ThatTimeOnly
{
	private const string IsOneOfSummary =
		"Verifies that the subject is one of the <paramref name=\"expected\" /> values.";

	private const string IsNotOneOfSummary =
		"Verifies that the subject is not one of the <paramref name=\"unexpected\" /> values.";

	[CreateExpectationFamily("Is{Not}OneOf", Summary = IsOneOfSummary, NegatedSummary = IsNotOneOfSummary)]
	[CreateExpectationFamily("Is{Not}OneOf", Params = true,
		Summary = IsOneOfSummary, NegatedSummary = IsNotOneOfSummary)]
	internal static TimeToleranceResult<TimeOnly, IThat<TimeOnly>> IsOneOfCore(
		IThat<TimeOnly> subject,
		IEnumerable<TimeOnly?> expected,
		string? expectedExpression,
		bool negated)
	{
		IEnumerable<TimeOnly?> expectedValues = expected.ToNonEmptyValues(negated);
		TimeTolerance tolerance = new();
		return new TimeToleranceResult<TimeOnly, IThat<TimeOnly>>(
			subject.Get().ExpectationBuilder.AddConstraint(
				(ExpectedValues: expectedValues, ExpectedExpression: expectedExpression, Tolerance: tolerance,
					Negated: negated),
				static (state, it, grammars) =>
					new IsOneOfConstraint(it, grammars, state.ExpectedValues, state.ExpectedExpression, state.Tolerance)
						.InvertIf(state.Negated)),
			subject,
			tolerance);
	}

	[CreateExpectationFamily("Is{Not}OneOf", Summary = IsOneOfSummary, NegatedSummary = IsNotOneOfSummary)]
	internal static TimeToleranceResult<TimeOnly, IThat<TimeOnly>> IsOneOfForValuesCore(
		IThat<TimeOnly> subject,
		IEnumerable<TimeOnly> expected,
		string? expectedExpression,
		bool negated)
	{
		expected.ThrowIfNull(negated);
		return IsOneOfCore(subject, expected.Cast<TimeOnly?>(), expectedExpression, negated);
	}

	private sealed class IsOneOfConstraint(
		string it,
		ExpectationGrammars grammars,
		IEnumerable<TimeOnly?> expected,
		string? expectedExpression,
		TimeTolerance tolerance)
		: ConstraintResult.WithValue<TimeOnly?>(it, grammars),
			IValueConstraint<TimeOnly>
	{
		public ConstraintResult IsMetBy(TimeOnly actual)
		{
			Actual = actual;
			TimeSpan timeTolerance = tolerance.GetToleranceOrDefault();
			foreach (TimeOnly? value in expected)
			{
				if (value != null &&
				    actual.CircularDistanceTicks(value.Value) <= timeTolerance.Ticks)
				{
					Outcome = Outcome.Success;
					return this;
				}
			}

			Outcome = Outcome.Failure;

			return this;
		}

		/// <inheritdoc />
		public override void AppendContexts(ResultContextCollector contexts)
			=> contexts.AddExpectedValuesContext(expectedExpression, expected, Grammars.IsNegated());

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is one of ", "are one of "));
			stringBuilder.Append(expectedExpression?.TrimCommonWhiteSpace() ?? Formatter.Format(expected));
			stringBuilder.Append(tolerance);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
			Formatter.Format(stringBuilder, Actual);
			stringBuilder.AppendTimeDifferenceToClosest(Actual, expected);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is not one of ", "are not one of "));
			stringBuilder.Append(expectedExpression?.TrimCommonWhiteSpace() ?? Formatter.Format(expected));
			stringBuilder.Append(tolerance);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
}
#endif
