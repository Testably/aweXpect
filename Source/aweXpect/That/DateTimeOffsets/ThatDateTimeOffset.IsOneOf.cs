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

public static partial class ThatDateTimeOffset
{
	private const string IsOneOfSummary =
		"Verifies that the subject is one of the <paramref name=\"expected\" /> values.";

	private const string IsNotOneOfSummary =
		"Verifies that the subject is not one of the <paramref name=\"unexpected\" /> values.";

	[CreateCollectionExpectation("Is{Not}OneOf", Summary = IsOneOfSummary, NegatedSummary = IsNotOneOfSummary)]
	[CreateCollectionExpectation("Is{Not}OneOf", Params = true,
		Summary = IsOneOfSummary, NegatedSummary = IsNotOneOfSummary)]
	internal static TimeToleranceResult<DateTimeOffset, IThat<DateTimeOffset>> IsOneOfCore(
		IThat<DateTimeOffset> subject,
		IEnumerable<DateTimeOffset?> expected,
		bool negated)
	{
		expected.ThrowIfNull(negated);
		TimeTolerance tolerance = new();
		return new TimeToleranceResult<DateTimeOffset, IThat<DateTimeOffset>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOneOfConstraint(it, grammars, expected, tolerance).InvertIf(negated)),
			subject,
			tolerance);
	}

	[CreateCollectionExpectation("Is{Not}OneOf", Summary = IsOneOfSummary, NegatedSummary = IsNotOneOfSummary)]
	internal static TimeToleranceResult<DateTimeOffset, IThat<DateTimeOffset>> IsOneOfForValuesCore(
		IThat<DateTimeOffset> subject,
		IEnumerable<DateTimeOffset> expected,
		bool negated)
	{
		expected.ThrowIfNull(negated);
		return IsOneOfCore(subject, expected.Cast<DateTimeOffset?>(), negated);
	}

	private sealed class IsOneOfConstraint(
		string it,
		ExpectationGrammars grammars,
		IEnumerable<DateTimeOffset?> expected,
		TimeTolerance tolerance)
		: ConstraintResult.WithValue<DateTimeOffset>(it, grammars),
			IValueConstraint<DateTimeOffset>
	{
		public ConstraintResult IsMetBy(DateTimeOffset actual)
		{
			Actual = actual;
			TimeSpan timeTolerance = tolerance.Tolerance ??
			                         Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Get();
			bool hasValues = false;
			foreach (DateTimeOffset? value in expected)
			{
				hasValues = true;
				if (value != null &&
				    actual - value.Value <= timeTolerance &&
				    actual - value.Value >= timeTolerance.Negate())
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
			stringBuilder.Append("is one of ");
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(tolerance);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was ");
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("is not one of ");
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(tolerance);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
}
