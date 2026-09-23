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

public static partial class ThatNullableDateOnly
{
	private const string IsOneOfSummary =
		"Verifies that the subject is one of the <paramref name=\"expected\" /> values.";

	private const string IsNotOneOfSummary =
		"Verifies that the subject is not one of the <paramref name=\"unexpected\" /> values.";

	[CreateCollectionExpectation("Is{Not}OneOf", Summary = IsOneOfSummary, NegatedSummary = IsNotOneOfSummary)]
	[CreateCollectionExpectation("Is{Not}OneOf", Params = true,
		Summary = IsOneOfSummary, NegatedSummary = IsNotOneOfSummary)]
	internal static TimeToleranceResult<DateOnly?, IThat<DateOnly?>> IsOneOfCore(
		IThat<DateOnly?> subject,
		IEnumerable<DateOnly?> expected,
		bool negated)
	{
		expected.ThrowIfNull(negated);
		TimeTolerance tolerance = new();
		return new TimeToleranceResult<DateOnly?, IThat<DateOnly?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOneOfConstraint(it, grammars, expected, tolerance).InvertIf(negated)),
			subject,
			tolerance);
	}

	[CreateCollectionExpectation("Is{Not}OneOf", Summary = IsOneOfSummary, NegatedSummary = IsNotOneOfSummary)]
	internal static TimeToleranceResult<DateOnly?, IThat<DateOnly?>> IsOneOfForValuesCore(
		IThat<DateOnly?> subject,
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
		: ConstraintResult.WithValue<DateOnly?>(it, grammars),
			IValueConstraint<DateOnly?>
	{
		private IEnumerable<DateOnly?> _expected = expected;

		public ConstraintResult IsMetBy(DateOnly? actual)
		{
			ThrowHelper.ThrowIfToleranceIsNotWholeDays(tolerance.Tolerance);
			IReadOnlyList<DateOnly?> expectedValues = ThrowHelper.EnsureNotEmpty(_expected);
			_expected = expectedValues;
			Actual = actual;
			if (actual is null)
			{
				Outcome = expectedValues.Any(x => x is null) ? Outcome.Success : Outcome.Failure;
			}
			else
			{
				TimeSpan timeTolerance = tolerance.Tolerance ??
				                         Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Get();
				Outcome = expectedValues.Any(value => value != null &&
				                                      Math.Abs(actual.Value.DayNumber - value.Value.DayNumber) <=
				                                      (int)timeTolerance.TotalDays)
					? Outcome.Success
					: Outcome.Failure;
			}

			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("is one of ");
			Formatter.Format(stringBuilder, _expected);
			stringBuilder.Append(tolerance.ToDayString());
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was ");
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("is not one of ");
			Formatter.Format(stringBuilder, _expected);
			stringBuilder.Append(tolerance.ToDayString());
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
}
#endif
