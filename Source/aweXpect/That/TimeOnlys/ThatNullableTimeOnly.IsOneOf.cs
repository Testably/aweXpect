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

public static partial class ThatNullableTimeOnly
{
	private const string IsOneOfSummary =
		"Verifies that the subject is one of the <paramref name=\"expected\" /> values.";

	private const string IsNotOneOfSummary =
		"Verifies that the subject is not one of the <paramref name=\"unexpected\" /> values.";

	[CreateCollectionExpectation("Is{Not}OneOf", Summary = IsOneOfSummary, NegatedSummary = IsNotOneOfSummary)]
	[CreateCollectionExpectation("Is{Not}OneOf", Params = true,
		Summary = IsOneOfSummary, NegatedSummary = IsNotOneOfSummary)]
	internal static TimeToleranceResult<TimeOnly?, IThat<TimeOnly?>> IsOneOfCore(
		IThat<TimeOnly?> subject,
		IEnumerable<TimeOnly?> expected,
		bool negated)
	{
		expected.ThrowIfNull(negated);
		TimeTolerance tolerance = new();
		return new TimeToleranceResult<TimeOnly?, IThat<TimeOnly?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOneOfConstraint(it, grammars, expected, tolerance).InvertIf(negated)),
			subject,
			tolerance);
	}

	[CreateCollectionExpectation("Is{Not}OneOf", Summary = IsOneOfSummary, NegatedSummary = IsNotOneOfSummary)]
	internal static TimeToleranceResult<TimeOnly?, IThat<TimeOnly?>> IsOneOfForValuesCore(
		IThat<TimeOnly?> subject,
		IEnumerable<TimeOnly> expected,
		bool negated)
	{
		expected.ThrowIfNull(negated);
		return IsOneOfCore(subject, expected.Cast<TimeOnly?>(), negated);
	}

	private sealed class IsOneOfConstraint(
		string it,
		ExpectationGrammars grammars,
		IEnumerable<TimeOnly?> expected,
		TimeTolerance tolerance)
		: ConstraintResult.WithValue<TimeOnly?>(it, grammars),
			IValueConstraint<TimeOnly?>
	{
		private IEnumerable<TimeOnly?> _expected = expected;

		public ConstraintResult IsMetBy(TimeOnly? actual)
		{
			IReadOnlyList<TimeOnly?> expectedValues = ThrowHelper.EnsureNotEmpty(_expected);
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
				                                      actual.Value.CircularDistanceTicks(value.Value) <=
				                                      timeTolerance.Ticks)
					? Outcome.Success
					: Outcome.Failure;
			}

			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is one of ", "are one of "));
			Formatter.Format(stringBuilder, _expected);
			stringBuilder.Append(tolerance);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
			Formatter.Format(stringBuilder, Actual);
			stringBuilder.AppendTimeDifferenceToClosest(Actual, _expected);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is not one of ", "are not one of "));
			Formatter.Format(stringBuilder, _expected);
			stringBuilder.Append(tolerance);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
}
#endif
