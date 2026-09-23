#if NET8_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Linq;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Customization;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;
using aweXpect.SourceGenerators;

namespace aweXpect;

public static partial class ThatDateOnly
{
	private const string IsOneOfSummary =
		"Verifies that the subject is one of the <paramref name=\"expected\" /> values.";

	private const string IsNotOneOfSummary =
		"Verifies that the subject is not one of the <paramref name=\"unexpected\" /> values.";

	[CreateCollectionExpectation("Is{Not}OneOf", Summary = IsOneOfSummary, NegatedSummary = IsNotOneOfSummary)]
	[CreateCollectionExpectation("Is{Not}OneOf", Params = true,
		Summary = IsOneOfSummary, NegatedSummary = IsNotOneOfSummary)]
	internal static TimeToleranceResult<DateOnly, IThat<DateOnly>> IsOneOfCore(
		IThat<DateOnly> subject,
		IEnumerable<DateOnly?> expected,
		bool negated)
	{
		expected.ThrowIfNull(negated);
		TimeTolerance tolerance = new();
		return new TimeToleranceResult<DateOnly, IThat<DateOnly>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOneOfConstraint(it, grammars, expected, tolerance).InvertIf(negated)),
			subject,
			tolerance);
	}

	[CreateCollectionExpectation("Is{Not}OneOf", Summary = IsOneOfSummary, NegatedSummary = IsNotOneOfSummary)]
	internal static TimeToleranceResult<DateOnly, IThat<DateOnly>> IsOneOfForValuesCore(
		IThat<DateOnly> subject,
		IEnumerable<DateOnly> expected,
		bool negated)
	{
		expected.ThrowIfNull(negated);
		return IsOneOfCore(subject, expected.Cast<DateOnly?>(), negated);
	}

	private sealed class IsOneOfConstraint(
		string it,
		ExpectationGrammars grammars,
		IEnumerable<DateOnly?> expected,
		TimeTolerance tolerance)
		: ConstraintResult.WithValue<DateOnly>(it, grammars),
			IValueConstraint<DateOnly>
	{
		public ConstraintResult IsMetBy(DateOnly actual)
		{
			ThrowHelper.ThrowIfToleranceIsNotWholeDays(tolerance.Tolerance);
			Actual = actual;
			TimeSpan timeTolerance = tolerance.Tolerance ??
			                         Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Get();
			bool hasValues = false;
			foreach (DateOnly? value in expected)
			{
				hasValues = true;
				if (value != null && Math.Abs(actual.DayNumber - value.Value.DayNumber) <= (int)timeTolerance.TotalDays)
				{
					Outcome = Outcome.Success;
					return this;
				}
			}

			if (!hasValues)
			{
				throw Tracing.WriteException(ThrowHelper.EmptyCollection());
			}

			Outcome = Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is one of ", "are one of "));
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(tolerance.ToDayString());
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is not one of ", "are not one of "));
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(tolerance.ToDayString());
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
}
#endif
