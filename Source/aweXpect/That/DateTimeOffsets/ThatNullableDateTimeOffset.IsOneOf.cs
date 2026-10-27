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

public static partial class ThatNullableDateTimeOffset
{
	private const string IsOneOfSummary =
		"Verifies that the subject is one of the <paramref name=\"expected\" /> values.";

	private const string IsNotOneOfSummary =
		"Verifies that the subject is not one of the <paramref name=\"unexpected\" /> values.";

	[CreateExpectationFamily("Is{Not}OneOf", Summary = IsOneOfSummary, NegatedSummary = IsNotOneOfSummary)]
	[CreateExpectationFamily("Is{Not}OneOf", Params = true,
		Summary = IsOneOfSummary, NegatedSummary = IsNotOneOfSummary)]
	internal static TimeToleranceResult<DateTimeOffset?, IThat<DateTimeOffset?>> IsOneOfCore(
		IThat<DateTimeOffset?> subject,
		IEnumerable<DateTimeOffset?> expected,
		string? expectedExpression,
		bool negated)
	{
		IEnumerable<DateTimeOffset?> expectedValues = expected.ToNonEmptyValues(negated);
		return new TimeToleranceResult<DateTimeOffset?, IThat<DateTimeOffset?>>(
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
	internal static TimeToleranceResult<DateTimeOffset?, IThat<DateTimeOffset?>> IsOneOfForValuesCore(
		IThat<DateTimeOffset?> subject,
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
		string? expectedExpression)
		: ConstraintResult.WithValue<DateTimeOffset?>(it, grammars),
			IValueConstraint<DateTimeOffset?>,
			IOptionsHolderSource<TimeTolerance>
	{
		private OptionsHolder<TimeTolerance>? _toleranceHolder;

		private TimeTolerance CurrentTolerance
			=> _toleranceHolder is null ? TimeTolerance.Default : _toleranceHolder.Value;

		/// <inheritdoc cref="IOptionsHolderSource{TOptions}.Holder" />
		OptionsHolder<TimeTolerance> IOptionsHolderSource<TimeTolerance>.Holder
			=> _toleranceHolder ??= new OptionsHolder<TimeTolerance>(TimeTolerance.Default);

		public ConstraintResult IsMetBy(DateTimeOffset? actual)
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
				                                actual - value.Value <= timeTolerance &&
				                                actual - value.Value >= timeTolerance.Negate())
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
			stringBuilder.Append(CurrentTolerance);
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
			stringBuilder.Append(CurrentTolerance);
		}
	}
}
