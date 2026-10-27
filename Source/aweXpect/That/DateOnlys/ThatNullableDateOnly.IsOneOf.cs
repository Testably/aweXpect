#if NET8_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Linq;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;
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

	[CreateExpectationFamily("Is{Not}OneOf", Summary = IsOneOfSummary, NegatedSummary = IsNotOneOfSummary)]
	[CreateExpectationFamily("Is{Not}OneOf", Params = true,
		Summary = IsOneOfSummary, NegatedSummary = IsNotOneOfSummary)]
	internal static TimeToleranceResult<DateOnly?, IThat<DateOnly?>> IsOneOfCore(
		IThat<DateOnly?> subject,
		IEnumerable<DateOnly?> expected,
		string? expectedExpression,
		bool negated)
	{
		IEnumerable<DateOnly?> expectedValues = expected.ToNonEmptyValues(negated);
		return new TimeToleranceResult<DateOnly?, IThat<DateOnly?>>(
			subject.Get().ExpectationBuilder.AddConstraint(
				(ExpectedValues: expectedValues, ExpectedExpression: expectedExpression, Negated: negated),
				static (state, it, grammars) =>
					new IsOneOfConstraint(it, grammars, state.ExpectedValues, state.ExpectedExpression)
						.InvertIf(state.Negated),
				out IsOneOfConstraint constraint),
			subject,
			constraint);
	}

	[CreateExpectationFamily("Is{Not}OneOf", Summary = IsOneOfSummary, NegatedSummary = IsNotOneOfSummary)]
	internal static TimeToleranceResult<DateOnly?, IThat<DateOnly?>> IsOneOfForValuesCore(
		IThat<DateOnly?> subject,
		IEnumerable<DateOnly> expected,
		string? expectedExpression,
		bool negated)
	{
		expected.ThrowIfNull(negated);
		return IsOneOfCore(subject, expected.Cast<DateOnly?>(), expectedExpression, negated);
	}

	private sealed class IsOneOfConstraint(
		string it,
		ExpectationGrammars grammars,
		IEnumerable<DateOnly?> expected,
		string? expectedExpression)
		: ConstraintResult.WithValue<DateOnly?>(it, grammars),
			IValueConstraint<DateOnly?>,
			IOptionsHolderSource<TimeTolerance>
	{
		private OptionsHolder<TimeTolerance>? _toleranceHolder;

		private TimeTolerance CurrentTolerance
			=> _toleranceHolder is null ? TimeTolerance.Days : _toleranceHolder.Value;

		/// <inheritdoc cref="IOptionsHolderSource{TOptions}.Holder" />
		OptionsHolder<TimeTolerance> IOptionsHolderSource<TimeTolerance>.Holder
			=> _toleranceHolder ??= new OptionsHolder<TimeTolerance>(TimeTolerance.Days);

		public ConstraintResult IsMetBy(DateOnly? actual)
		{
			Actual = actual;
			if (actual is null)
			{
				Outcome = expected.Any(x => x is null) ? Outcome.Success : Outcome.Failure;
			}
			else
			{
				TimeSpan timeTolerance = CurrentTolerance.GetToleranceOrDefault();
				Outcome = expected.Any(value => value != null &&
				                                Math.Abs(actual.Value.DayNumber - value.Value.DayNumber) <=
				                                (int)timeTolerance.TotalDays)
					? Outcome.Success
					: Outcome.Failure;
			}

			return this;
		}

		/// <inheritdoc />
		public override void AppendContexts(ResultContextCollector contexts)
			=> contexts.AddExpectedValuesContext(expectedExpression, expected, Grammars);

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is one of ", "are one of "));
			stringBuilder.Append(expectedExpression?.TrimCommonWhiteSpace() ?? Formatter.Format(expected));
			stringBuilder.Append(CurrentTolerance.ToDayString());
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
			Formatter.Format(stringBuilder, Actual);
			stringBuilder.AppendDayDifferenceToClosest(Actual, expected);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is not one of ", "are not one of "));
			stringBuilder.Append(expectedExpression?.TrimCommonWhiteSpace() ?? Formatter.Format(expected));
			stringBuilder.Append(CurrentTolerance.ToDayString());
		}
	}
}
#endif
