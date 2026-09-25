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

public static partial class ThatNullableDateTime
{
	private const string IsOneOfSummary =
		"Verifies that the subject is one of the <paramref name=\"expected\" /> values.";

	private const string IsNotOneOfSummary =
		"Verifies that the subject is not one of the <paramref name=\"unexpected\" /> values.";

	[CreateCollectionExpectation("Is{Not}OneOf", Summary = IsOneOfSummary, NegatedSummary = IsNotOneOfSummary)]
	[CreateCollectionExpectation("Is{Not}OneOf", Params = true,
		Summary = IsOneOfSummary, NegatedSummary = IsNotOneOfSummary)]
	internal static TimeToleranceResult<DateTime?, IThat<DateTime?>> IsOneOfCore(
		IThat<DateTime?> subject,
		IEnumerable<DateTime?> expected,
		bool negated)
	{
		IEnumerable<DateTime?> expectedValues = expected.ToNonEmptyValues(negated);
		TimeTolerance tolerance = new();
		return new TimeToleranceResult<DateTime?, IThat<DateTime?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOneOfConstraint(it, grammars, expectedValues, tolerance).InvertIf(negated)),
			subject,
			tolerance);
	}

	[CreateCollectionExpectation("Is{Not}OneOf", Summary = IsOneOfSummary, NegatedSummary = IsNotOneOfSummary)]
	internal static TimeToleranceResult<DateTime?, IThat<DateTime?>> IsOneOfForValuesCore(
		IThat<DateTime?> subject,
		IEnumerable<DateTime> expected,
		bool negated)
	{
		expected.ThrowIfNull(negated);
		return IsOneOfCore(subject, expected.Cast<DateTime?>(), negated);
	}

	private sealed class IsOneOfConstraint(
		string it,
		ExpectationGrammars grammars,
		IEnumerable<DateTime?> expected,
		TimeTolerance tolerance)
		: ConstraintResult.WithValue<DateTime?>(it, grammars),
			IValueConstraint<DateTime?>
	{
		private DateTimeKind? _incompatibleKind;

		public ConstraintResult IsMetBy(DateTime? actual)
		{
			Actual = actual;
			if (actual is null)
			{
				Outcome = expected.Any(x => x is null) ? Outcome.Success : Outcome.Failure;
			}
			else
			{
				Outcome = GetOutcomeFor(actual.Value);
			}

			return this;
		}

		private Outcome GetOutcomeFor(DateTime actual)
		{
			TimeSpan timeTolerance = tolerance.Tolerance ??
			                         Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Get();
			bool hasComparableValue = false;
			DateTimeKind? incomparableKind = null;
			foreach (DateTime? value in expected)
			{
				if (value is null)
				{
					continue;
				}

				if (!actual.IsKindCompatibleWith(value.Value))
				{
					incomparableKind = value.Value.Kind;
					continue;
				}

				hasComparableValue = true;
				if (actual - value.Value <= timeTolerance &&
				    actual - value.Value >= timeTolerance.Negate())
				{
					return Outcome.Success;
				}
			}

			_incompatibleKind = hasComparableValue ? null : incomparableKind;
			return Outcome.Failure;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is one of ", "are one of "));
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(tolerance);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_incompatibleKind is not null)
			{
				stringBuilder.Append(It).Append(" had Kind ").Append(Actual?.Kind)
					.Append(", which cannot be compared with ").Append(_incompatibleKind);
			}
			else
			{
				stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
				Formatter.Format(stringBuilder, Actual);
				stringBuilder.AppendTimeDifferenceToClosest(Actual, expected);
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is not one of ", "are not one of "));
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(tolerance);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
}
