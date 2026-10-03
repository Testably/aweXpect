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

public static partial class ThatDateTimeOffset
{
	private const string IsOneOfSummary =
		"Verifies that the subject is one of the <paramref name=\"expected\" /> values.";

	private const string IsNotOneOfSummary =
		"Verifies that the subject is not one of the <paramref name=\"unexpected\" /> values.";

	[CreateExpectationFamily("Is{Not}OneOf", Summary = IsOneOfSummary, NegatedSummary = IsNotOneOfSummary)]
	[CreateExpectationFamily("Is{Not}OneOf", Params = true,
		Summary = IsOneOfSummary, NegatedSummary = IsNotOneOfSummary)]
	internal static TimeToleranceResult<DateTimeOffset, IThat<DateTimeOffset>> IsOneOfCore(
		IThat<DateTimeOffset> subject,
		IEnumerable<DateTimeOffset?> expected,
		string? expectedExpression,
		bool negated)
	{
		IEnumerable<DateTimeOffset?> expectedValues = expected.ToNonEmptyValues(negated);
		TimeTolerance tolerance = new();
		return new TimeToleranceResult<DateTimeOffset, IThat<DateTimeOffset>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOneOfConstraint(it, grammars, expectedValues, expectedExpression, tolerance)
					.InvertIf(negated)),
			subject,
			tolerance);
	}

	[CreateExpectationFamily("Is{Not}OneOf", Summary = IsOneOfSummary, NegatedSummary = IsNotOneOfSummary)]
	internal static TimeToleranceResult<DateTimeOffset, IThat<DateTimeOffset>> IsOneOfForValuesCore(
		IThat<DateTimeOffset> subject,
		IEnumerable<DateTimeOffset> expected,
		string? expectedExpression,
		bool negated)
	{
		expected.ThrowIfNull(negated);
		return IsOneOfCore(subject, expected.Cast<DateTimeOffset?>(), expectedExpression, negated);
	}

	private sealed class IsOneOfConstraint(
		string it,
		ExpectationGrammars grammars,
		IEnumerable<DateTimeOffset?> expected,
		string? expectedExpression,
		TimeTolerance tolerance)
		: ConstraintResult.WithValue<DateTimeOffset>(it, grammars),
			IValueConstraint<DateTimeOffset>
	{
		/// <inheritdoc />
		public override void AppendContexts(ResultContextCollector contexts)
			=> contexts.AddExpectedValuesContext(expectedExpression, expected, Grammars.IsNegated());

		public ConstraintResult IsMetBy(DateTimeOffset actual)
		{
			Actual = actual;
			TimeSpan timeTolerance = tolerance.GetToleranceOrDefault();
			foreach (DateTimeOffset? value in expected)
			{
				if (value != null &&
				    actual - value.Value <= timeTolerance &&
				    actual - value.Value >= timeTolerance.Negate())
				{
					Outcome = Outcome.Success;
					return this;
				}
			}

			Outcome = Outcome.Failure;
			return this;
		}

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
